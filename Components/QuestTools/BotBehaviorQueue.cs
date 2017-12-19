using System;
using System.Collections.Generic;
using System.Linq;
using Trinity;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace Trinity.Components.QuestTools
{
    /// <summary>
    /// Runs ProfileBehaviors using the BotBehavior hook && IEnhancedProfileBehavior
    /// Usage: BotBehaviorQueue.Queue(myProfileBehavior); 
    /// </summary>
    public static partial class BotBehaviorQueue
    {
        public delegate bool ShouldRunCondition(List<ProfileBehavior> profileBehaviors);
        private static readonly HashSet<QueueItem> Q = new HashSet<QueueItem>();
        private static bool _hooksInserted;
        private static bool _wired;
        private static Decorator _hook;
        internal static QueueItemEqualityComparer QueueItemComparer = new QueueItemEqualityComparer();
        private static QueueItem _active;
        public static List<QueueItem> Shelf = new List<QueueItem>();
        private static DateTime LastChecked = DateTime.UtcNow;
        private static int MinCheckInterval = 100;

        static BotBehaviorQueue()
        {
            WireUp();

            if (!_hooksInserted)
                InsertHooks();
        }

        public static int Count
        {
            get { return Q.Count; }
        }

        public static bool IsActive
        {
            get { return Q.Any() || (_active != null && _active.ActiveNode != null && !_active.ActiveNode.IsDone); }
        }

        public static bool IsEnabled
        {
            get { return _hooksInserted && _wired; }
        }

        public static ProfileBehavior CurrentBehavior
        {
            get { return _active != null ? _active.ActiveNode : null; }
        }

        private static bool CheckCondition(QueueItem item)
        {
            return item.Condition != null && item.Condition.Invoke(item.Nodes);
        }

        /// <summary>
        /// Marks QueueItems as ready if their condition is True
        /// </summary>
        private static void CheckConditions()
        {
            if (DateTime.UtcNow.Subtract(LastChecked).TotalMilliseconds < MinCheckInterval)
                return;

            foreach (var item in Q.Where(item => !item.ConditionPassed).Where(CheckCondition))
            {
                item.ConditionPassed = true;

                Core.Logger.Log("触发 {1} 与 {0} 行为在下一个机会运行",
                    item.Nodes.Count, (!string.IsNullOrEmpty(item.Name)) ? item.Name : "Unnamed");
            }

            LastChecked = DateTime.UtcNow;
        }

        /// <summary>
        /// Magic self-updating composite
        /// </summary>
        /// <returns></returns>
        private static Decorator CreateMasterHook()
        {
            return new Decorator(ret =>
            {
                var child = _hook.DecoratedChild as PrioritySelector;
                if (child != null)
                    child.Children = new List<Composite> { Next() };

                return true;

            }, new PrioritySelector());
        }

        /// <summary>
        /// Selects a composite to be run
        /// This is called every tick and returns a composite
        /// Which composite is returned changes based on the QueueItem currently being processed.
        /// </summary>
        private static Composite Next()
        {
            // 1. No active node
            if (_active == null)
            {
                // 1.1 Nothing in the Queue.
                if (!Q.Any())
                    return Continue;

                // 1.2 Start the next QueueItem that has passed its condition
                var nextItem = Q.FirstOrDefault(n => n.ConditionPassed);
                if (nextItem != null)
                {
                    Core.Logger.Verbose("Starting QueueItem");
                    if (Q.Remove(nextItem))
                    {
                        _active = nextItem;
                        _active.OnStart?.Invoke(_active);
                        return Loop;
                    };
                }

                // 1.3 Nothing has passed condition yet.
                return Continue;
            }

            // 2. We're currently processing a QueueItem
            // But havent started processing its nodes.
            if (_active.ActiveNode == null)
            {
                // 2.1 Handle starting the first Node
                _active.ActiveNode = _active.Nodes.First();
                _active.ActiveNode.Run();
                _active.OnNodeStart?.Invoke(_active);
                return _active.ActiveNode.Behavior;
            }

            BotMain.StatusText = _active.ActiveNode.StatusText;

            // 3. We're currently processing a QueueItem
            // And the current node is Done
            if (_active.ActiveNode.IsDone)
            {
                // 3.1 Handle ActiveNode has finished                
                _active.CompletedNodes++;
                _active.ActiveNode.OnDone();
                Core.Logger.Verbose("[{0}] Complete {1}/{2} ({3})", _active.Name, _active.CompletedNodes, _active.Nodes.Count, _active.ActiveNode.GetType());
                _active.OnNodeDone?.Invoke(_active);

                // 3.2 All nodes are finished, so the QueueItem is now Done.
                if (_active.IsComplete)
                {
                    // 3.2.1 Handle all nodes are finished
                    _active.OnDone?.Invoke(_active);
                    Core.Logger.Verbose("[{1}] Completed {0}", _active.CompletedNodes, _active.Name);

                    // 3.2.2 Traverse Upwards
                    // If this QueueItem is a child, we need to continue with its parent
                    // Parent gets taken off the shelf (unpaused) and set as the new active Queueitem.
                    var parent = Shelf.FirstOrDefault(i => i.ParentOf == _active.Id);
                    Core.Logger.Verbose("All Nodes Complete ParentId={0} ThisId={1}", parent?.Id.ToString() ?? "Null", _active.Id);
                    if (parent != null)
                    {
                        _active = parent;
                        Shelf.Remove(parent);
                        Core.Logger.Verbose("ShelfCount={0}", Shelf.Count);
                        return Loop;
                    }

                    // 3.2.3 Shove it back at the bottom of the queue if it should be repeated
                    if (_active.Repeat)
                    {
                        var temp = _active;
                        _active.Reset();
                        _active = null;
                        Queue(temp);
                        return Loop;
                    }

                    // 3.2.4 No parent, No Repeat, so just end the QueueItem 
                    _active = null;
                    return Loop;
                }

                // 3.3 Handle start of next node
                var currentPosition = _active.Nodes.IndexOf(_active.ActiveNode);
                _active.ActiveNode = _active.Nodes.ElementAt(currentPosition + 1);
                _active.ActiveNode.Run();
                _active.OnNodeStart?.Invoke(_active);
                return _active.ActiveNode.Behavior;
            }

            // 4.1 Traverse Downwards
            // We're currently processing a QueueItem
            // And the current node is NOT Done
            // And the current node has children
            Core.Logger.Verbose("ShelfCount={0}", Shelf.Count);
            var children = _active.ActiveNode.GetChildren();
            if (children.Count > 0)
            {
                Core.Logger.Log("处理 {0} 儿童 '{1}' ({2})", children.Count, _active.Name, _active.Id);

                // Copy QueueItem so we can resume it later.
                var queueItemToShelve = _active;

                // Wrap the children as a new QueueItem                
                var childQueueItem = new QueueItem
                {
                    Name = $"Children of {_active.Name}",
                    Nodes = _active.ActiveNode.GetChildren()
                };

                // Store a references between parent and child
                queueItemToShelve.ParentOf = childQueueItem.Id;
                childQueueItem.ChildOf = _active.Id;

                // Pause the active QueueItem by moving it to the shelf
                Shelf.Add(queueItemToShelve);

                // Start working on the children.
                _active = childQueueItem;
                return Loop;
            }

            // Handle continuing an in-progress Node
            LogBehavior(_active);
            return _active.ActiveNode.Behavior;

        }

        private static Composite Continue
        {
            get { return new Action(ret => RunStatus.Failure); }
        }

        private static Composite Loop
        {
            get { return new Action(ret => RunStatus.Success); }
        }

        #region Methods for Queueing Items

        public static void Queue(IEnumerable<ProfileBehavior> profileBehaviors, string name = "")
        {
            Queue(profileBehaviors, ret => true, name);
        }

        public static void Queue(ProfileBehavior profileBehavior, ShouldRunCondition condition)
        {
            Queue(new List<ProfileBehavior> { profileBehavior }, condition);
        }

        public static void Queue(ProfileBehavior behavior, string name = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                name = behavior.ToString();
            Queue(new List<ProfileBehavior> { behavior }, ret => true, name);
        }

        public static void Queue(IEnumerable<ProfileBehavior> profileBehaviors, ShouldRunCondition condition, string name = "")
        {
            var behaviorList = profileBehaviors.ToList();
            if (string.IsNullOrWhiteSpace(name))
                name = behaviorList.Aggregate(name, (current, behavior) => current + (behavior.ToString() + ","));

            var item = new QueueItem
            {
                Name = name,
                Nodes = behaviorList,
                Condition = condition,
            };
            Queue(item);

            if (!BotMain.IsRunning || BotMain.IsPausedForStateExecution)
                BehaviorTreeExecutor.CreateBotThread();
        }

        public static void Queue(IEnumerable<QueueItem> items)
        {
            items.ForEach(Queue);
        }

        public static void Queue(QueueItem item)
        {
            if (!item.Nodes.Any())
            {
                Core.Logger.Debug("Item {0} was queued without any nodes", item.Name);
                return;
            }

            if (QueueItemComparer.Equals(item, _active) || Q.Contains(item, QueueItemComparer))
            {
                string activeNodeName = "";
                if (item.ActiveNode != null)
                    activeNodeName = item.ActiveNode.GetType().Name;
                Core.Logger.Verbose("Discarding Duplicate Queue Request Name='{0}' Id='{1}' Type='{2}'", item.Name, item.Id, activeNodeName);
                return;
            }

            if (item.Condition == null)
                item.Condition = ret => true;

            ProfileUtils.ReplaceTags(item.Nodes);
            Q.Add(item);
        }

        #endregion

        #region Binding, Events, Hooks, Thread Etc

        public static void WireUp()
        {
            if (_wired) return;

            BotMain.OnStart += OnStartHandler;
            GameEvents.OnGameChanged += OnGameChangedHandler;
            TreeHooks.Instance.OnHooksCleared += OnHooksClearedHandler;
            Pulsator.OnPulse += OnPulse;
            ProfileManager.OnProfileLoaded += OnProfileLoaded;
            _wired = true;
        }

        private static void OnProfileLoaded(object sender, EventArgs eventArgs)
        {
            Reset();
        }

        private static void OnStartHandler(IBot bot)
        {
            InsertHooks();
            Reset(true);
        }

        private static void OnGameChangedHandler(object sender, EventArgs eventArgs)
        {
            Reset(true);
        }

        private static void OnHooksClearedHandler(object sender, EventArgs eventArgs)
        {
            InsertHooks();
        }

        private static void OnPulse(object sender, EventArgs eventArgs)
        {
            CheckConditions();
        }

        public static void UnWire()
        {
            if (!_wired) return;

            BotMain.OnStart -= OnStartHandler;
            GameEvents.OnGameChanged -= OnGameChangedHandler;
            TreeHooks.Instance.OnHooksCleared -= OnHooksClearedHandler;
            Pulsator.OnPulse -= OnPulse;
            ProfileManager.OnProfileLoaded -= OnProfileLoaded;

            _wired = false;
        }

        private static void InsertHooks()
        {
            Core.Logger.Debug("Inserting BotBehaviorQueue Hook");
            _hook = CreateMasterHook();
            TreeHooks.Instance.InsertHook("BotBehavior", 0, _hook);
            _hooksInserted = true;
        }

        #endregion

        #region Utilities

        private static void Log(string message, params object[] args)
        {
            Core.Logger.Log(message, args);
        }

        private static void LogBehavior(QueueItem item)
        {
            if (item != null && item.ActiveNode != null)
            {
                Core.Logger.Log("{4}标签 {0} 已经完成了={1} 完成缓存={2} 最后状态={3}",
                    item.ActiveNode.GetType(),
                    item.ActiveNode.IsDone,
                    item.ActiveNode.IsDoneCache,
                    item.ActiveNode.Behavior != null ? item.ActiveNode.Behavior.LastStatus.ToString() : "null",
                    string.IsNullOrEmpty(item.Name) ? string.Empty : "[" + item.Name + "] "
                    );
            }
        }

        public static void Reset(bool forceClearAll = false)
        {
            if (forceClearAll)
                Q.Clear();
            else
                Q.RemoveWhere(i => !i.Persist);

            Shelf.Clear();
            _active = null;
        }

        #endregion

    }

    public class QueueItem
    {
        public int Id { get; private set; }

        public string Name { get; set; }

        public int CompletedNodes = 0;

        public bool IsComplete { get { return CompletedNodes == Nodes.Count; } }

        public BotBehaviorQueue.ShouldRunCondition Condition;

        private List<ProfileBehavior> _nodes = new List<ProfileBehavior>();

        public ProfileBehavior ActiveNode { get; set; }

        public delegate void QueueItemDelegate(QueueItem item);

        public QueueItemDelegate OnNodeStart { get; set; }

        public QueueItemDelegate OnNodeDone { get; set; }

        public QueueItemDelegate OnDone { get; set; }

        public QueueItemDelegate OnStart { get; set; }

        public bool ConditionPassed { get; set; }

        public bool Persist { get; set; }

        public int ParentOf { get; set; }

        public int ChildOf { get; set; }

        public bool Repeat { get; set; }

        public List<ProfileBehavior> Nodes
        {
            get { return _nodes; }
            set
            {
                var hash = value.Aggregate(0, (current, node) => current ^ node.GetHashCode());

                if (Name != null)
                    hash = hash ^ Name.GetHashCode();

                Id = hash;

                _nodes = value;
            }
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public void Reset()
        {
            ActiveNode = null;
            ConditionPassed = false;
            CompletedNodes = 0;
            ChildOf = 0;
            ParentOf = 0;
            Nodes.ForEach(n => n.ResetCachedDone(true));
        }
    }

    public class QueueItemEqualityComparer : IEqualityComparer<QueueItem>
    {
        public bool Equals(QueueItem x, QueueItem y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.Name == y.Name;
        }

        public int GetHashCode(QueueItem obj)
        {
            return ReferenceEquals(obj, null) ? 0 : obj.Id;
        }
    }




}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Serilog;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Common;

namespace Trinity.Framework.Helpers
{
    // 黑白灰 修改访问级别
    public class GenericCacheObject
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public DateTime Expires { get; set; }

        public GenericCacheObject()
        {
        }

        public GenericCacheObject(string key, object value, TimeSpan expirationDuration)
        {
            Key = key;
            Value = value;
            Expires = DateTime.UtcNow.Add(expirationDuration);
        }

        public override bool Equals(object obj)
        {
            var other = obj as GenericCacheObject;
            if (other == null)
                return false;
            if (other.Key.Trim() == string.Empty)
                return false;

            return this.Key == other.Key;
        }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }

        public override string ToString()
        {
            return $"Key={Key} Value={Value} Expires={Expires}";
        }
    }

    // 黑白灰 修改黑名单支持同时到期时间多个对象
    public class GenericBlacklist
    {
        private static readonly ILogger s_logger = Logger.GetLoggerInstanceForType();

        private static void AddToExpireCache(DateTime k, string v)
        {
            HashSet<string> ls = null;
            if (s_expireCache.ContainsKey(k))
            {
                ls = s_expireCache[k];
            }
            else
            {
                ls = new HashSet<string>();
                s_expireCache.Add(k, ls);
            }
            ls.Add(v);
        }

        private static bool RemoveFromExpireCache(DateTime k, string v)
        {
            if (!s_expireCache.ContainsKey(k))
            {
                return false;
            }
            var ls = s_expireCache[k];
            return ls != null && ls.Remove(v);
        }

        public static void Blacklist(TrinityActor objectToBlacklist, TimeSpan duration = default(TimeSpan), string reason = "")
        {
            s_logger.Debug($@"[{nameof(Blacklist)}] Blacklisting {objectToBlacklist.InternalName} ActorSnoId: {objectToBlacklist.ActorSnoId} RActorId: {objectToBlacklist.RActorId}
            Because: {reason}
            Duration: {duration:g}");

            DateTime expires;
            if (duration == default(TimeSpan))
            {
                expires = objectToBlacklist.IsMarker
                    ? DateTime.UtcNow.AddSeconds(60)
                    : DateTime.UtcNow.AddSeconds(30);
            }
            else
            {
                expires = DateTime.UtcNow.Add(duration);
            }

            AddToBlacklist(new GenericCacheObject
            {
                Key = objectToBlacklist.ObjectHash,
                Value = null,
                Expires = expires
            });
        }

        private static readonly Dictionary<string, GenericCacheObject> s_dataCache = new Dictionary<string, GenericCacheObject>();
        private static readonly Dictionary<DateTime, HashSet<string>> s_expireCache = new Dictionary<DateTime, HashSet<string>>();

        private static readonly object s_synchronizer = new object();

        private static Thread _manager;

        static GenericBlacklist()
        {
            MaintainBlacklist();
        }

        public static bool AddToBlacklist(GenericCacheObject obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Key))
                return false;

            lock (s_synchronizer)
            {
                if (ContainsKey(obj.Key))
                    return false;
                s_dataCache.Add(obj.Key, obj);
                AddToExpireCache(obj.Expires, obj.Key);
                return true;
            }
        }

        public static bool UpdateObject(GenericCacheObject obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Key))
                return false;

            lock (s_synchronizer)
            {
                RemoveObject(obj.Key);

                s_dataCache.Add(obj.Key, obj);
                AddToExpireCache(obj.Expires, obj.Key);

                return true;
            }
        }

        public static bool RemoveObject(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            lock (s_synchronizer)
            {
                if (!ContainsKey(key))
                    return false;
                GenericCacheObject oldObj = s_dataCache[key];
                s_dataCache.Remove(key);
                RemoveFromExpireCache(oldObj.Expires, oldObj.Key);
                return true;
            }
        }

        public static bool ContainsKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            lock (s_synchronizer)
            {
                return s_dataCache.ContainsKey(key);
            }
        }

        public static bool Contains(GenericCacheObject obj)
        {
            if (obj.Key == "")
                return false;

            lock (s_synchronizer)
            {
                return ContainsKey(obj.Key);
            }
        }

        public static GenericCacheObject GetObject(string key)
        {
            lock (s_synchronizer)
            {
                if (ContainsKey(key))
                {
                    return s_dataCache[key];
                }
                return new GenericCacheObject();
            }
        }

        public static void MaintainBlacklist()
        {
            using (new PerformanceLogger("GenericBlacklist.MaintainBlacklist"))
            {
                try
                {
                    if (_manager == null || (_manager != null && !_manager.IsAlive))
                    {
                        s_logger.Information($"[{nameof(MaintainBlacklist)}] Starting up Generic Blacklist Manager thread");
                        _manager = new Thread(Manage)
                        {
                            Name = "TrinityPlugin Generic Blacklist",
                            IsBackground = true,
                            Priority = ThreadPriority.Lowest
                        };
                        _manager.Start();
                    }
                }
                catch (Exception ex)
                {
                    s_logger.Error(ex, $"[{nameof(MaintainBlacklist)}] Exception in Generic Blacklist Manager");
                }
            }
        }

        private static void Manage()
        {
            while (true)
            {
                try
                {
                    long nowTicks = DateTime.UtcNow.Ticks;

                    lock (s_synchronizer)
                    {
                        foreach (KeyValuePair<DateTime, HashSet<string>> kv in s_expireCache.Where(kv => kv.Key.Ticks < nowTicks).ToList())
                        {
                            s_expireCache.Remove(kv.Key);
                            foreach (var v in kv.Value)
                            {
                                s_dataCache.Remove(v);
                            }
                        }
                    }

                    Thread.Sleep(100);
                }
                catch (ThreadAbortException) { }
                catch (Exception ex)
                {
                    s_logger.Error(ex, $"[{nameof(Manage)}] Blacklist manager crashed");
                }
            }
        }

        public static void Shutdown()
        {
            if (_manager != null && _manager.IsAlive)
            {
                _manager.Abort();
            }
        }

        public static void ClearBlacklist()
        {
            lock (s_synchronizer)
            {
                s_dataCache.Clear();
                s_expireCache.Clear();
            }
        }
    }
}

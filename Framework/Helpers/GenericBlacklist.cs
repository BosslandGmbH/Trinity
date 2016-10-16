using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Trinity.Framework.Actors.ActorTypes;

namespace Trinity.Framework.Helpers
{
    internal class GenericCacheObject
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public DateTime Expires { get; set; }

        public GenericCacheObject() { }

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

    internal class GenericBlacklist
    {
        public static void Blacklist(TrinityActor objectToBlacklist, TimeSpan duration = default(TimeSpan), string reason = "")
        {
            Logger.Log($@"Blacklisting {objectToBlacklist.InternalName} ActorSnoId: {objectToBlacklist.ActorSnoId} RActorId: {objectToBlacklist.RActorId}
            Because: {reason}
            Duration: {duration:g}   
            ");

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

        private static readonly Dictionary<string, GenericCacheObject> DataCache = new Dictionary<string, GenericCacheObject>();
        private static readonly Dictionary<DateTime, string> ExpireCache = new Dictionary<DateTime, string>();

        private static readonly object Synchronizer = new object();

        private static Thread _manager;

        static GenericBlacklist()
        {
            MaintainBlacklist();
        }

        public static bool AddToBlacklist(GenericCacheObject obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Key))
                return false;

            lock (Synchronizer)
            {
                if (ContainsKey(obj.Key))
                    return false;
                DataCache.Add(obj.Key, obj);
                ExpireCache.Add(obj.Expires, obj.Key);
                return true;
            }
        }

        public static bool UpdateObject(GenericCacheObject obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Key))
                return false;

            lock (Synchronizer)
            {
                RemoveObject(obj.Key);

                DataCache.Add(obj.Key, obj);
                ExpireCache.Add(obj.Expires, obj.Key);

                return true;
            }
        }

        public static bool RemoveObject(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            lock (Synchronizer)
            {
                if (!ContainsKey(key))
                    return false;
                GenericCacheObject oldObj = DataCache[key];
                DataCache.Remove(key);
                ExpireCache.Remove(oldObj.Expires);
                return true;
            }
        }
        
        public static bool ContainsKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            lock (Synchronizer)
            {
                return DataCache.ContainsKey(key);
            }
        }

        public static bool Contains(GenericCacheObject obj)
        {
            if (obj.Key == "")
                return false;

            lock (Synchronizer)
            {
                return ContainsKey(obj.Key);
            }
        }

        public static GenericCacheObject GetObject(string key)
        {
            lock (Synchronizer)
            {
                if (ContainsKey(key))
                {
                    return DataCache[key];
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
                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Starting up Generic Blacklist Manager thread");
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
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Exception in Generic Blacklist Manager");
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, ex.ToString());
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

                    lock (Synchronizer)
                    {
                        foreach (KeyValuePair<DateTime, string> kv in ExpireCache.Where(kv => kv.Key.Ticks < nowTicks).ToList())
                        {
                            ExpireCache.Remove(kv.Key);
                            DataCache.Remove(kv.Value);
                        }
                    }

                    Thread.Sleep(100);
                }
                catch (ThreadAbortException) { }
                catch (Exception ex)
                {
                    Logger.LogError("Blacklist manager crashed: {0}", ex.Message);
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
            lock (Synchronizer)
            {
                DataCache.Clear();
                ExpireCache.Clear();
            }
        }
    }

}

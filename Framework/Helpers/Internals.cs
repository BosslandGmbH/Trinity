using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Framework.Helpers
{
    public static class Internals
    {
        private class BuildMemoryInfo
        {
            public Version Version;
            public IntPtr ObjectManagerPtr;
            public IntPtr SymbolManagerPtr;
            public IntPtr SnoGroupsAddr;
            public IntPtr AttributeDescripterAddr;
            public int GlobalsOffset;
            public int StorageOffset;
        }

        private static BuildMemoryInfo _currentBuild;

        public static class DemonBuddyObjects
        {
            public static ACDManager AcdManager => _acdManager.Value;
            private static readonly StaticRouter<ACDManager> _acdManager = new StaticRouter<ACDManager>(typeof(ZetaDia));

            public static RActorManager RActorManager => _ractorManager.Value;
            private static readonly StaticRouter<RActorManager> _ractorManager = new StaticRouter<RActorManager>(typeof(ZetaDia));

            public static ActivePlayerData ActivePlayerData => _activePlayerData.Value;
            private static readonly StaticRouter<ActivePlayerData> _activePlayerData = new StaticRouter<ActivePlayerData>(typeof(ZetaDia));

            public static IntPtr ObjectManagerAddr => _storageAddr.Value - (int)ObjectManagerOffsets.Storage;
            private static readonly StaticRouter<IntPtr> _storageAddr = new StaticRouter<IntPtr>(typeof(ZetaDia), 0);

            public static IntPtr PlayerDataAddr => _playerData.Value.Value;
            private static readonly StaticRouter<DynamicPointer<CPlayer>> _playerData = new StaticRouter<DynamicPointer<CPlayer>>(typeof(ZetaDia), 0);
        }

        public static class Addresses
        {
            static Addresses()
            {
                // Use the memory info from the most recent version lower than current.
                var d3Version = new Version(ZetaDia.Memory.Process.MainModule.FileVersionInfo.FileVersion.Replace(", ", "."));
                _currentBuild = SupportedBuilds.Where(o => o.Version <= d3Version).OrderBy(o => o.Version).LastOrDefault();
                Logger.LogDebug($"D3: {d3Version}; Data: {_currentBuild?.Version}");

                var test = DemonBuddyOffsets.SNOGroups;
            }

            public static IntPtr ObjectManager => DemonBuddyObjects.ObjectManagerAddr;
            public static IntPtr RActorManager => DemonBuddyObjects.RActorManager.BaseAddress;
            public static IntPtr AcdManager => DemonBuddyObjects.AcdManager.BaseAddress;
            public static IntPtr ActivePlayerData => DemonBuddyObjects.ActivePlayerData.BaseAddress;
            public static IntPtr SymbolManager => ZetaDia.Memory.Read<IntPtr>(_currentBuild.SymbolManagerPtr);
            public static IntPtr Hero => ZetaDia.Service.Hero.BaseAddress;
            public static IntPtr Globals => ObjectManager + (int)ObjectManagerOffsets.Globals;
            public static IntPtr SnoGroups => DemonBuddyOffsets.SNOGroups; //_currentBuild.SnoGroupsAddr;
            public static IntPtr AttributeDescripters => DemonBuddyOffsets.AttributeDescripter; //_currentBuild.AttributeDescripterAddr;
            public static IntPtr Storage => ObjectManager + (int)ObjectManagerOffsets.Storage;
            public static IntPtr PlayerData => DemonBuddyObjects.PlayerDataAddr;
        }

        private static class DemonBuddyOffsets
        {
            // Here's the thing, i need AttributeDescripter and SNOGroups pointers,
            // you really don't want to have to rely on me to update these manually every patch.

            private static List<IntPtr> _offsetsA;
            private static List<IntPtr> _offsetsB;
            private static List<IntPtr> _offsetsC;
            private static List<IntPtr> _offsetsD;
            private static List<IntPtr> _offsetsE;

            static DemonBuddyOffsets()
            {
                foreach (var field in typeof(ZetaDia).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    var val = field.GetValue(null);
                    if (val == null)
                        continue;

                    var type = val.GetType();
                    if (type.IsPublic)
                        continue;

                    var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fields.Count() != 5)
                        continue;

                    try
                    {
                        _offsetsA = FieldToList<IntPtr>(fields[0], val);
                        _offsetsB = FieldToList<IntPtr>(fields[1], val);
                        _offsetsC = FieldToList<IntPtr>(fields[2], val);
                        _offsetsD = FieldToList<IntPtr>(fields[3], val);
                        _offsetsE = FieldToList<IntPtr>(fields[4], val);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            public static List<T> FieldToList<T>(FieldInfo fieldInfo, object parent)
            {
                var value = fieldInfo.GetValue(parent);
                var valEnumerable = value as IEnumerable<T>;
                if (valEnumerable != null)
                {
                    return valEnumerable.ToList();
                }
                var fields = value.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                return fields.Select(t => t.GetValue(value)).Cast<T>().ToList();
            }

            public static IntPtr SNOGroups => _offsetsC.ElementAtOrDefault(12) + 0x38;

            public static IntPtr AttributeDescripter => _offsetsC.ElementAtOrDefault(15) - 0x04;
        }

        public enum ObjectManagerOffsets
        {
            Storage = 0x798,
            Globals = 0x790
        }

        private static readonly List<BuildMemoryInfo> SupportedBuilds = new List<BuildMemoryInfo>
        {

            new BuildMemoryInfo
            {
                Version = new Version("2.4.1.36595"),
                ObjectManagerPtr = (IntPtr)0x01E9F8EC,
                SymbolManagerPtr = (IntPtr)0x01EE7598,
                SnoGroupsAddr = (IntPtr)0x1EA0BC8,
                AttributeDescripterAddr = (IntPtr) 0x1EEFE70,
            },
            new BuildMemoryInfo
            {
                Version = new Version("2.4.2.37893"),
                ObjectManagerPtr = (IntPtr)0x1C8755B0,
                SymbolManagerPtr = (IntPtr)0x01F01900,
                SnoGroupsAddr = (IntPtr)0x01EA73A8,
                AttributeDescripterAddr = (IntPtr)0x01EBEB78,
            },
            new BuildMemoryInfo
            {
                Version = new Version("2.4.2.38247"),
                ObjectManagerPtr = (IntPtr)0x01EA60CC,
                SymbolManagerPtr = (IntPtr)0x01F01950,
                SnoGroupsAddr = (IntPtr)0x01EA73A8,
                AttributeDescripterAddr = (IntPtr)0x1EBF020,
            },
            new BuildMemoryInfo
            {
                Version = new Version("2.4.2.38682"),
                ObjectManagerPtr = (IntPtr)0x01EA60D4,
                SymbolManagerPtr = (IntPtr)0x01F01958,
                SnoGroupsAddr = (IntPtr)0x01EA73B0,
                AttributeDescripterAddr = (IntPtr)0x1EBF028,
            },
        };
    }

}
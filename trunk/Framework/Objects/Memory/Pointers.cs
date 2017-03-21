using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Framework.Objects.Memory
{
    public static class Pointers
    {
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
            private static readonly StaticRouter<DynamicPointer<ActivePlayerData>> _playerData = new StaticRouter<DynamicPointer<ActivePlayerData>>(typeof(ZetaDia), 0);
        }

        public static class Addresses
        {
            public static IntPtr ObjectManager => DemonBuddyObjects.ObjectManagerAddr;
            public static IntPtr RActorManager => DemonBuddyObjects.RActorManager.BaseAddress;
            public static IntPtr AcdManager => DemonBuddyObjects.AcdManager.BaseAddress;
            public static IntPtr ActivePlayerData => DemonBuddyObjects.ActivePlayerData.BaseAddress;
            public static IntPtr SymbolManager => ZetaDia.Memory.Read<IntPtr>((IntPtr)0x6F690164) - 0x74;
            public static IntPtr Hero => ZetaDia.Service.Hero.BaseAddress;
            public static IntPtr Globals => ObjectManager + (int)ObjectManagerOffsets.Globals;
            public static IntPtr SnoGroups => DemonBuddyOffsets.SNOGroups;
            public static IntPtr AttributeDescripters => DemonBuddyOffsets.AttributeDescripter;
            public static IntPtr Storage => ObjectManager + (int)ObjectManagerOffsets.Storage;
            public static IntPtr PlayerData => DemonBuddyObjects.PlayerDataAddr;
            public static IntPtr MapManager => ZetaDia.Memory.Read<IntPtr>(DemonBuddyOffsets.MapManagerPtr);
            public static IntPtr ActorSnoTable => DemonBuddyOffsets.SNOTableActor;
            public static IntPtr PowerSnoTable => DemonBuddyOffsets.SNOTablePower;
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

            public static IntPtr MapManagerPtr => _offsetsC.ElementAtOrDefault(12) - 0x68;
            //public static IntPtr SNOGroups => (IntPtr)0x01C12C90;
            //public static IntPtr AttributeDescripter => (IntPtr)0x01C65660;

            public static IntPtr SNOGroups => (IntPtr)0x02059EA0;

            [Obsolete("now using ZetaDia AttributeDescripter")]
            public static IntPtr AttributeDescripter => (IntPtr)0x020B2A80;

            public static IntPtr SNOTableGameBalance => _offsetsC.ElementAtOrDefault(16);
            public static IntPtr SNOTableActor => _offsetsC.ElementAtOrDefault(18);
            public static IntPtr SNOTablePower => _offsetsC.ElementAtOrDefault(19);
        }

        public enum ObjectManagerOffsets
        {
            Storage = 0x798,
            Globals = 0x790
        }
    }
}

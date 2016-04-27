using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using GongSolutions.Wpf.DragDrop.Utilities;
using Trinity.Cache;
using Trinity.Objects.Native;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Helpers
{
    public static class Internals
    {
        public static class Objects
        {
            public static ACDManager AcdManager => _acdManager.Value;
            private static readonly StaticRouter<ACDManager> _acdManager = new StaticRouter<ACDManager>(typeof (ZetaDia));

            public static RActorManager RActorManager => _ractorManager.Value;
            private static readonly StaticRouter<RActorManager> _ractorManager = new StaticRouter<RActorManager>(typeof (ZetaDia));

            public static ActivePlayerData ActivePlayerData => _activePlayerData.Value;
            private static readonly StaticRouter<ActivePlayerData> _activePlayerData = new StaticRouter<ActivePlayerData>(typeof (ZetaDia));
        }

        public static class Addresses
        {
            public static IntPtr ObjectManager => ZetaDia.Memory.Read<IntPtr>(Pointers.ObjectManagerPtr);
            public static IntPtr RActorManager => Objects.RActorManager.BaseAddress;
            public static IntPtr AcdManager => Objects.AcdManager.BaseAddress;        
            public static IntPtr ActivePlayerData => Objects.ActivePlayerData.BaseAddress;
            public static IntPtr SymbolManager => ZetaDia.Memory.Read<IntPtr>(Pointers.SymbolManagerPtr);
            public static IntPtr Hero => ZetaDia.Service.Hero.BaseAddress;
            public static IntPtr Globals => ObjectManager + 0x790;
            public static IntPtr SnoGroups => (IntPtr)0x1EA0BC8;
            public static IntPtr AttributeDescripters => (IntPtr)0x1EEFE70;
        }

        public static class Pointers 
        {
            public static IntPtr ObjectManagerPtr => (IntPtr)0x01E9F8EC;
            public static IntPtr SymbolManagerPtr => (IntPtr)0x01EE7598;
        }

    }
}


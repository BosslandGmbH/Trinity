using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using GongSolutions.Wpf.DragDrop.Utilities;
using Microsoft.Scripting.Utils;
using Trinity.Cache;
using Trinity.Objects.Native;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Helpers
{
    public static class ZetaInternals
    {
        public static class Objects
        {
            public static ACDManager ACDManager => _acdManager.Value;
            private static readonly StaticRouter<ACDManager> _acdManager = new StaticRouter<ACDManager>(typeof (ZetaDia));

            public static RActorManager RActorManager => _ractorManager.Value;
            private static readonly StaticRouter<RActorManager> _ractorManager = new StaticRouter<RActorManager>(typeof (ZetaDia));

            public static ActivePlayerData ActivePlayerData => _activePlayerData.Value;
            private static readonly StaticRouter<ActivePlayerData> _activePlayerData = new StaticRouter<ActivePlayerData>(typeof (ZetaDia));
        }


        public static class Addresses
        {
            public static IntPtr ObjectManager => ZetaDia.Memory.Read<IntPtr>(Offsets.ObjectManager);

            public static IntPtr RActorManager => Objects.RActorManager.BaseAddress;

            public static IntPtr AcdManager => Objects.ACDManager.BaseAddress;        

            public static IntPtr ActivePlayerData => Objects.ActivePlayerData.BaseAddress;
        }

        public static class Offsets 
        {
            public static IntPtr ObjectManager => (IntPtr)0x01E9A234;
        }

    }
}

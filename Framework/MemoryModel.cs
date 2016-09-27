using System;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.UX;
using Trinity.Helpers;
using Zeta.Game;

namespace Trinity.Framework
{
    public class MemoryModel
    {
        private Storage _storage { get; set; }
        private Globals _globals { get; set; }

        public Hero Hero { get; } = new Hero(Internals.Addresses.Hero);

        public Globals Globals
        {
            get
            {
                if(!_globals.IsValid)
                    _globals = new Globals(Internals.Addresses.Globals);

                return _globals;
            }
        }

        public Storage Storage
        {
            get
            {
                if(_storage == null || _storage.IsValid)
                    _storage = new Storage(Internals.Addresses.Storage);

                return _storage;
            }
        }


        public GameInfo GameInfo { get; } = new Objects.Memory.GameInfo(ZetaDia.Memory.Read<IntPtr>(ZetaDia.Service.Games.BaseAddress + 0x28));
        public ActivePlayer ActivePlayer { get; } = new ActivePlayer(Internals.Addresses.ActivePlayerData);
        public UXMinimap Minimap => UXHelper.GetControl<UXMinimap>(10917491887468455961);
    }

}

using System;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.UX;
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

        private Allocator _movementManager;
        public Allocator MovementManager
        {
            get
            {
                if (_storage != null && !_storage.IsValid) return _movementManager;
                var movementPtr = ZetaDia.Memory.Read<IntPtr>(Internals.Addresses.ObjectManager + 0xA0C);
                _movementManager = MemoryWrapper.Create<Allocator>(movementPtr);
                return _movementManager;
            }
        }


        public GameInfo GameInfo { get; } = new Objects.Memory.GameInfo(ZetaDia.Memory.Read<IntPtr>(ZetaDia.Service.Games.BaseAddress + 0x28));
        public ActivePlayer ActivePlayer { get; } = new ActivePlayer(Internals.Addresses.ActivePlayerData);
        public UXMinimap Minimap => UXHelper.GetControl<UXMinimap>(10917491887468455961);
    }

}

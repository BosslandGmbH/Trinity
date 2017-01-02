using System;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Sno;
using Trinity.Framework.Objects.Memory.Sno.Helpers;
using Trinity.Framework.Objects.Memory.UX;
using Trinity.Reference;
using Zeta.Bot;
using Zeta.Game;

namespace Trinity.Framework
{
    public class MemoryModel
    {
        private Storage _storage { get; set; }
        private Globals _globals { get; set; }

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

        public SnoManager.SnoGroups SnoGroups => SnoManager.Groups;
        //public PowerHelper PowerHelper => PowerHelper.Instance;
        //public GameBalanceHelper GameBalanceHelper => SnoManager.GameBalanceHelper;
        //public StringListHelper StringListHelper => SnoManager.StringListHelper;
        //public ActivePlayer ActivePlayer { get; } = new ActivePlayer(Internals.Addresses.ActivePlayerData);
        public UXMinimap Minimap => UXHelper.GetControl<UXMinimap>(10917491887468455961);
        //public MapManager MapManager { get; } = new MapManager(Internals.Addresses.MapManager);

        public Hero Hero { get; } = new Hero(Internals.Addresses.Hero);

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Objects.Memory.Debug;
using Trinity.Framework.Objects.Memory.Misc;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Framework.Objects.Memory
{
    //public enum GameCreationState
    //{
    //    None = 0,
    //    Creating,
    //    NotCreated,
    //    Created,
    //}

    //public class Games : MemoryWrapper
    //{
    //    public Games(IntPtr ptr) : base(ptr) { }        

    //    public GameCreationState GameCreationState => ReadOffset<GameCreationState>(0x120); // 1 = creating, 2 = not created, 3 = created.

    //    public bool IsTransitioning => ReadOffset<int>(0x138) == 0; // GFX freeze between loading, possibly network related.

    //    public GameInfo GameInfo => ReadPointer<GameInfo>(0x28);        
    //}

    public enum GameState
    {
        Idle = 0,
        Loading,
        InGame,
        NotInGame,
    }

    public class GameInfo : MemoryWrapper
    {
        public GameInfo() { }
        public GameInfo(IntPtr ptr) : base(ptr) { }
        public GameState GameState => ReadOffset<GameState>(0x4FC);
    }


}






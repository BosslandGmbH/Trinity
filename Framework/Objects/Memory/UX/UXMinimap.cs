using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Trinity.UI.UIComponents;
using Zeta.Game;
using Zeta.Common;

namespace Trinity.Framework.Objects.Memory.UX
{
    public class UXMinimap : UXControl
    {
        public List<MinimapIcon> Items => ReadObjects<MinimapIcon>(0x00C74, Count);
        public int Count => ReadOffset<int>(0x12074);
        public Vector2 MinimapOffset => ReadOffset<Vector2>(0x12090);
        public Vector2 PlayerPosition => ReadOffset<Vector2>(0x120B4);
        public int MouseOverIndex => ReadOffset<int>(0x120C0);
        public int MouseOverTick => ReadOffset<int>(0x120C4);        
        public Vector2 Screen => new Vector2(ReadOffset<float>(0x468), ReadOffset<float>(0x470));
    }

    public class MinimapIcon : MemoryWrapper
    {
        public const int SizeOf = 0x114;
        public string Name => ReadString(0x000, 0x100);
        public Vector2 ScreenPosition => ReadOffset<Vector2>(0x100);
        public int AcdId => ReadOffset<int>(0x108);
        public int Id => ReadOffset<int>(0x10C);
    }

}

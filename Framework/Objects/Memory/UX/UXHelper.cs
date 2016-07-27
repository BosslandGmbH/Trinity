using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Crypto.Tls;
using Trinity.Framework.Objects.Enums;
using Zeta.Game;
using UIElement = Zeta.Game.Internals.UIElement;

namespace Trinity.Framework.Objects.Memory.UX
{
    public static class UXHelper
    {
        public static T GetControl<T>(ulong hash) where T : UXControl, new()
        {
            return MemoryWrapper.Create<T>(UIMap[hash].BaseAddress);
        }

        public static T GetControl<T>(UXReference reference) where T : UXControl, new()
        {
            return MemoryWrapper.Create<T>(UIMap[reference.Hash].BaseAddress);
        }

        public static List<T> GetControlsByName<T>(string name) where T : UXControl, new()
        {
            var needle = name.ToLower();
            var items = UIMap.Where(i => i.Value.Self.Name.ToLower().Contains(needle));
            if (items.Any())
            {
                return items.Select(i => MemoryWrapper.Create<T>(i.Value.BaseAddress)).ToList();
            }
            return null;
        }

        private static Dictionary<ulong, UXControl> _uiMapByHash;
        public static Dictionary<ulong, UXControl> UIMap => _uiMapByHash ?? (_uiMapByHash = GetUIMap());
        private static Dictionary<ulong, UXControl> GetUIMap()
        {
            var map = UIElement.UIMap.ToDictionary(k => k.Hash, GetControl);

            // For some reason DB's UIMap doesnt include all map items.                
            // In order to use it for now we'll check the sibling controls.

            foreach (var item in map.ToList())
            {
                if (item.Value == null)
                    continue;

                var next = item.Value.Next;
                if (next != null)
                {
                    var nextHash = next.Self.Hash;
                    if (!map.ContainsKey(nextHash))
                        map.Add(nextHash, GetControl(next.BaseAddress));
                }

                var prev = item.Value.Previous;
                if (prev != null)
                {
                    var prevHash = prev.Self.Hash;
                    if (!map.ContainsKey(prevHash))
                        map.Add(prevHash, GetControl(prev.BaseAddress));
                }
            }
            return map;
        }

        public static UXControl GetControl(UIElement el)
        {
            return GetControl(el.BaseAddress);
        }

        public static UXControl GetControl(IntPtr ptr)
        {
            var type = ZetaDia.Memory.Read<ControlType>(ptr + 0x430);
            switch (type)
            {
                case ControlType.Text:
                    return MemoryWrapper.Create<UXLabel>(ptr);
            }
            return MemoryWrapper.Create<UXControl>(ptr);
        }

        private static ILookup<ControlType, UXControl> _uiMapByType;
        public static ILookup<ControlType, UXControl> UIMapByType
        {
            get { return _uiMapByType ?? (_uiMapByType = UIMap.ToLookup(k => k.Value.Type, v => v.Value)); }
        }
    }

}

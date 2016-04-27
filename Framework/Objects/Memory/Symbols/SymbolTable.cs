using System;
using System.Collections.Generic;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory.Misc
{
    public class SymbolTable : MemoryWrapper
    {
        public IntPtr NextTableAddress;
        public List<Symbol> Symbols;
        public string Name;
        public int Index;

        protected override void OnUpdated()
        {
            var symbols = new List<Symbol>();
            var address = BaseAddress;
            var symbol = Create<Symbol>(address);

            // first int of data after last table. This could change over time.
            var endOfSymbols = 1702129225; 

            while (symbol != null)
            {
                if (symbol.Id == endOfSymbols)
                    break;

                if (symbol.Id < 1000)
                {
                    // Exactly 8 bytes of zero seperates each table. Don't compare to epsilon.
                    if (ZetaDia.Memory.Read<double>(symbol.BaseAddress) == 0) 
                        break;

                    symbols.Add(symbol);
                }
                address += Symbol.SizeOf;
                symbol = Create<Symbol>(address);
            }
            NextTableAddress = address + Symbol.SizeOf;
            Symbols = symbols;
        }

        public override string ToString()
        {
            return $"{GetType().Name}, {Name} ({Symbols.Count})";
        }
    }
}
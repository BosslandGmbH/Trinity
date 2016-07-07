using Trinity.Framework.Objects.Memory;
using Trinity.Helpers;

namespace Trinity.Framework
{
    public class MemoryModel
    {
        public Hero Hero { get; } = new Hero(Internals.Addresses.Hero);
        public Globals Globals { get; } = new Globals(Internals.Addresses.Globals);
        public Storage Storage { get; } = new Storage(Internals.Addresses.Storage);
    }

}
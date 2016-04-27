
namespace Trinity
{
    /// <summary>
    /// A simple two-integer struct for fast comparison purposes and storage in HashSets
    /// </summary>
    internal struct DoubleInt
    {
        public int A;
        public int B;
        public DoubleInt(int a, int b)
        {
            this.A = a;
            this.B = b;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return A ^ B;
        }

        public static bool operator ==(DoubleInt left, DoubleInt right)
        {
            return (left.A ^ left.B) == (right.A ^ right.B);
        }

        public static bool operator !=(DoubleInt left, DoubleInt right)
        {
            return (left.A ^ left.B) != (right.A ^ right.B);
        }
    }
}

using Zeta.Common;

namespace Trinity.Framework.Objects.Memory
{
    public struct UXRect
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public float Width => Right - Left;
        public float Height => Bottom - Top;
        public Vector2 Center => new Vector2(Left + (Width / 2), Top + (Height / 2));
        public override string ToString() => $"{Width} x {Height} - L: {Left}, T: {Top}, R: {Right}, B: {Bottom}";
    }
}

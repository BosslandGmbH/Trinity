using System;

namespace Trinity.UI.Overlays._3D.Perspective.Primitives
{
    /// <summary>
    /// A Flags enumeration to identify the sides of a cube 
    /// (seen from an X, Y, Z positive position).
    /// </summary>
    [Flags()]
    public enum BoxSides
    {
        /// <summary>
        /// The front side of the cube.
        /// </summary>
        Front = 1,

        /// <summary>
        /// The back side of the cube.
        /// </summary>
        Back = 2,

        /// <summary>
        /// The left side of the cube.
        /// </summary>
        Left = 4,

        /// <summary>
        /// The right side of the cube.
        /// </summary>
        Right = 8,

        /// <summary>
        /// The upper side of the cube.
        /// </summary>
        Up = 16,

        /// <summary>
        /// The down side of the cube.
        /// </summary>
        Down = 32,

        /// <summary>
        /// All the sides of the cube.
        /// </summary>
        All = Front | Back | Left | Right | Up | Down
    }
}

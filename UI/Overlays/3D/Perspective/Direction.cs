//------------------------------------------------------------------
//
//  For licensing information and to get the latest version go to:
//  http://www.codeplex.com/perspective
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY
//  OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//  LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//  FITNESS FOR A PARTICULAR PURPOSE.
//
//------------------------------------------------------------------

using System;

namespace Trinity.UI.Overlays._3D.Perspective
{
    /// <summary>
    /// A flags enumeration type to handle 3D directions.
    /// </summary>
    [Flags()]
    public enum Direction
    {
        /// <summary>
        /// The direction is forward.
        /// </summary>
        Forward = 1,

        /// <summary>
        /// The direction is backward.
        /// </summary>
        Backward = 2,

        /// <summary>
        /// The direction is Left.
        /// </summary>
        Left = 4,

        /// <summary>
        /// The direction is Right.
        /// </summary>
        Right = 8,

        /// <summary>
        /// The direction is Up.
        /// </summary>
        Up = 16,

        /// <summary>
        /// The direction is Down.
        /// </summary>
        Down = 32
    };
}

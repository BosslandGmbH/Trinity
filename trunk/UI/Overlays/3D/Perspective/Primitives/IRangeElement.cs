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

namespace Perspective.Core.Primitives
{
    /// <summary>
    /// An interface to handle multi-selection of 2D and 3D range elements.
    /// </summary>
    public interface IRangeElement
    {
        /// <summary>
        /// Gets or sets the Minimum possible value of the range element.
        /// </summary>
        double Minimum { get; set; }

        /// <summary>
        /// Gets or sets the highest possible value of the range element.
        /// </summary>
        double Maximum { get; set; }

        /// <summary>
        /// Gets or sets the current magnitude of the range element.
        /// </summary>
        double Value { get; set; }

        /// <summary>
        /// Gets a value that indicates whether this element has keyboard focus.
        /// </summary>
        bool IsKeyboardFocused { get; }

        /// <summary>
        /// Gets a value that indicates whether this element or one of his children has keyboard focus.
        /// </summary>
        bool IsKeyboardFocusWithin { get; }    
    }
}

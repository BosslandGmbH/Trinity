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

using System.Windows;

namespace Trinity.UI.Overlays._3D.Perspective.Shapes
{
    /// <summary>
    /// A 3D hexagonal bar element, with a specific texture for the ends.
    /// By default, the direction of the bar is the Z axis, and the length is 1.0.
    /// Default radius is 1.0.
    /// </summary>
    public class BeeBar3D : CompositeBar3D
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static BeeBar3D()
        {
            SideCountProperty.OverrideMetadata(
                typeof(BeeBar3D),
                new PropertyMetadata(
                    6, 
                    VisualPropertyChanged));
            DefaultTextureMappingProperty.OverrideMetadata(
                typeof(BeeBar3D),
                new PropertyMetadata(
                    true,
                    VisualPropertyChanged));
        
            RoundingRateProperty.OverrideMetadata(
                typeof(BeeBar3D),
                new PropertyMetadata(
                    0.20, 
                    VisualPropertyChanged));
        }
    }
}

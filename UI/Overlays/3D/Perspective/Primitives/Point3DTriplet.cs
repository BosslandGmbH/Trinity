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

using System.Windows.Media.Media3D;

namespace Trinity.UI.Overlays._3D.Perspective.Primitives
{
    /// <summary>
    /// A class to handle the points of a triangle.
    /// </summary>
    public class Point3DTriplet
    {
        /// <summary>
        /// Initializes a new instance of Point3DTriplet.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Thirst point</param>
        public Point3DTriplet(Point3D p1, Point3D p2, Point3D p3)
        {
            _points.Add(p1);
            _points.Add(p2);
            _points.Add(p3);
        }

        private Point3DCollection _points = new Point3DCollection(3);

        /// <summary>
        /// Gets the triangle's points collection.
        /// </summary>
        public Point3DCollection Points
        {
            get { return _points; }
        }
    }
}

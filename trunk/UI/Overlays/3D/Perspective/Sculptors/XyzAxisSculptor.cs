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
using System.Windows.Media.Media3D;
using Trinity.UI.Overlays._3D.Perspective.Primitives;

namespace Trinity.UI.Overlays._3D.Perspective.Sculptors
{
    /// <summary>
    /// A class to handle points and triangles of an XyzAxis3D model.
    /// </summary>
    public class XyzAxisSculptor : Sculptor
    {
        internal const double DefaultLength = 1.0;
        internal const double DefaultRadius = 0.025;

        private enum Sign { Positive, Negative };

        private double _length = DefaultLength;
        private double _radius = DefaultRadius;
        private bool _signed;
        private CoordinateSystemKind _coordinateSystemKind;

        /// <summary>
        /// Initializes an existing instance of XyzAxisSculptor.
        /// </summary>
        /// <param name="length">Length of each axis.</param>
        /// <param name="radius">Radius of each axis.</param>
        /// <param name="signed">Axis signed characteristic.</param>
        /// <param name="coordinateSystemKind">The kind of coordinate system.</param>
        public void Initialize(
            double length, 
            double radius, 
            bool signed, 
            CoordinateSystemKind coordinateSystemKind)
        {
            _length = length;
            _radius = radius;
            _signed = signed;
            _coordinateSystemKind = coordinateSystemKind;
        }

        /// <summary>
        /// Initializes the Points and Triangles collections.
        /// Called By Sculptor.BuildMesh()
        /// </summary>
        protected override void CreateTriangles()
        {
            SphericalSculptor ssOrigin = new SphericalSculptor();
            ssOrigin.BuildMesh();
            // double sphereScaleFactor = _radius / 2.0;
            double sphereScaleFactor = _radius;
            ssOrigin.Transform(new ScaleTransform3D(sphereScaleFactor, sphereScaleFactor, sphereScaleFactor));
            CopyFrom(ssOrigin);

            CreateArrows(Sign.Positive);
            if (_signed)
            {
                CreateArrows(Sign.Negative);
            }
        }

        /// <summary>
        /// Creates the 3 arrows objects
        /// </summary>
        /// <param name="s">Sign enum value.</param>
        private void CreateArrows(Sign s)
        {
            // double arrowScaleFactor = _radius * 10.0 / 2.0;
            double arrowScaleFactor = _radius * 10.0;
            ScaleTransform3D arrowScaling = new ScaleTransform3D(arrowScaleFactor, arrowScaleFactor, 1.0);

            ArrowSculptor asZ = new ArrowSculptor();
            asZ.Initialize(_length);
            asZ.BuildMesh();
            Transform3DGroup tgZ = new Transform3DGroup();
            tgZ.Children.Add(arrowScaling);
            // if (s != Sign.Positive)
            if (((s == Sign.Negative) && (_coordinateSystemKind == CoordinateSystemKind.RightHanded))
                ||((s == Sign.Positive) && (_coordinateSystemKind == CoordinateSystemKind.LeftHanded)))
            {
                AxisAngleRotation3D rZ = new AxisAngleRotation3D(
                    new Vector3D(0.0, 1.0, 0.0),
                    180.0);
                tgZ.Children.Add(new RotateTransform3D(rZ, 0.0, 0.0, 0.0));
            }
            asZ.Transform(tgZ);
            CopyFrom(asZ);

            ArrowSculptor asX = new ArrowSculptor();
            asX.Initialize(_length);
            asX.BuildMesh();
            Transform3DGroup tgX = new Transform3DGroup();
            tgX.Children.Add(arrowScaling);
            AxisAngleRotation3D rX = new AxisAngleRotation3D(
                new Vector3D(0.0, 1.0, 0.0),
                (s == Sign.Positive) ? 90.0 : -90.0);
            tgX.Children.Add(new RotateTransform3D(rX, 0.0, 0.0, 0.0));
            asX.Transform(tgX);
            CopyFrom(asX);

            ArrowSculptor asY = new ArrowSculptor();
            asY.Initialize(_length);
            asY.BuildMesh();
            Transform3DGroup tgY = new Transform3DGroup();
            tgY.Children.Add(arrowScaling);
            AxisAngleRotation3D rY = new AxisAngleRotation3D(
                new Vector3D(1.0, 0.0, 0.0),
                (s == Sign.Positive) ? -90.0 : 90.0);
            tgY.Children.Add(new RotateTransform3D(rY, 0.0, 0.0, 0.0));
            asY.Transform(tgY);
            CopyFrom(asY);

            if (_length > 1.0)
            {
                double sepRadius = _radius * 2.0;
                int sep = Convert.ToInt32(_length) - 1;
                BarSculptor[][] bars = new BarSculptor[sep][];
                AxisAngleRotation3D[] r = new AxisAngleRotation3D[3];
                r[0] = new AxisAngleRotation3D(new Vector3D(0.0, 1.0, 0.0),
                    (s == Sign.Positive) ? 90.0 : -90.0);
                r[1] = new AxisAngleRotation3D(new Vector3D(1.0, 0.0, 0.0),
                    (s == Sign.Positive) ? -90.0 : 90.0);
                for (int i = 0; i < sep; i++)
                {
                    int offset = (s == Sign.Positive ? i + 1 : -i - 1);
                    int offsetZ = (
                        (((s == Sign.Positive) && (_coordinateSystemKind == CoordinateSystemKind.RightHanded))
                            || ((s == Sign.Negative) && (_coordinateSystemKind == CoordinateSystemKind.LeftHanded))) ? 
                            i + 1 : -i - 1);
                    bars[i] = new BarSculptor[3];
                    for (int j = 0; j < 3; j++)
                    {
                        bars[i][j] = new BarSculptor();
                        bars[i][j].Initialize(40, 0.0, 0.0);
                        bars[i][j].BuildMesh();
                        Transform3DGroup tg = new Transform3DGroup();
                        tg.Children.Add(new ScaleTransform3D(sepRadius, sepRadius, 0.01));
                        if (j != 2)
                        {
                            tg.Children.Add(new RotateTransform3D(r[j], 0.0, 0.0, 0.0));
                        }
                        tg.Children.Add(new TranslateTransform3D(
                            j == 0 ? offset : 0.0,
                            j == 1 ? offset : 0.0,
                            j == 2 ? offsetZ : 0.0));
                        bars[i][j].Transform(tg);
                        CopyFrom(bars[i][j]);
                    }
                }
            }
        }
    }
}

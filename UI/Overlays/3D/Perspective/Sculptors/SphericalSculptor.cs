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
using System.Windows;
using System.Windows.Media.Media3D;
using Trinity.UI.Overlays._3D.Perspective.Primitives;

namespace Trinity.UI.Overlays._3D.Perspective.Sculptors
{
    /// <summary>
    /// A class to handle points and triangles of a spherical model.
    /// Default radius is 1.0.
    /// </summary>
    public class SphericalSculptor : Sculptor
    {
        /// <summary>
        /// Initializes a new instance of SphericalSculptor.
        /// </summary>
        public SphericalSculptor() 
        {
        }

        /// <summary>
        /// Initializes a new instance of SphericalSculptor.
        /// </summary>
        /// <param name="parallelCount">Parallel Count.</param>
        public SphericalSculptor(int parallelCount)
        {
            Initialize(parallelCount);
        }

        /// <summary>
        /// Initializes an existing instance of SphericalSculptor.
        /// </summary>
        /// <param name="parallelCount">Parallel Count.</param>
        public void Initialize(int parallelCount)
        {
            if (parallelCount < 2)
            {
                throw new ArgumentException("parallelCount < 2");
            }
            _parallelCount = parallelCount;
            CreatePoints();
        }
        /// <summary>
        /// Initializes the Points and Triangles collections.
        /// Called By Sculptor.BuildMesh()
        /// </summary>
        protected override void CreateTriangles()
        {
            if (Points.Count == 0)
            {
                CreatePoints(this.ParallelCount);
            }
            if ((Triangles.Count == 0) && (Points.Count > 0))
            {
                BuildTriangles();
            }
        }

        internal const int DefaultParallelCount = 20;
        private int _parallelCount = DefaultParallelCount;

        /// <summary>
        /// Gets or sets the side count for half of the circumference.
        /// </summary>
        public int ParallelCount
        {
            get { return _parallelCount; }
            set { _parallelCount = value; }
        }

        /// <summary>
        /// Creates the structure points.
        /// </summary>
        /// <param name="parallelCount">Side count for half of the circumference.</param>
        public void CreatePoints(int parallelCount)
        {
            _parallelCount = parallelCount;
            CreatePoints();
        }

        /// <summary>
        /// Creates the points.
        /// Points are added by vertical rank from top to bottom (on Z-Y first)
        /// and then counter-clockwise.
        /// </summary>
        public void CreatePoints()
        {
            this.Points.Clear();

            // theta : vertical slices
            // phi : horizontal slices

            double thetaStep = Math.PI / (_parallelCount + 1);
            double phiStep = Math.PI / (_parallelCount + 1);
            double theta = 0.0;
            double phi = 0.0;

            for (int i = 0; i <= ((_parallelCount + 1) * 2); i++)
            // for (int i = 0; i <= 2; i++)
            {
                theta = (thetaStep * i);
                for (int j = 0; j <= _parallelCount + 1; j++)
                // for (int j = 0; j <= 3; j++)
                {
                    phi = phiStep * j;
                    Point3D p = new Point3D();
                    p.X = Math.Sin(theta) * Math.Sin(phi);
                    p.Y = Math.Cos(phi);
                    p.Z = Math.Cos(theta) * Math.Sin(phi);
                    this.Points.Add(p);
                }
            }
        }

        /// <summary>
        /// Builds the structure triangles.
        /// 2 triangles are built on each side, except at the top and at the bottom (only 1 trinagle by side).
        /// Triangles are added by vertical rank from top to bottom (on Z-Y first)
        /// and then counter-clockwise.
        /// On each side, the upperleft triangle is built before the bottomright one.
        /// </summary>
        public void BuildTriangles()
        {
            if (Points.Count < 2)
            {
                throw new ArgumentException("Points.Count < 2");
            }
            this.Triangles.Clear();
            for (int i = 0; i <= Points.Count - 1; i++)
            {
                // vRank : vertical point rank (counter-clockwise)
                int vRank = i / (_parallelCount + 2);

                // index : index of current point in the vertical rank
                int index = i % (_parallelCount + 2);
                if (vRank >= 1)
                {
                    // Upperleft triangle
                    // (the only one at the bottom of the rank)
                    // if (index > 0)
                    if ((index > 0) && (index < _parallelCount + 1))
                    {
                        this.Triangles.Add(new Point3DTriplet(
                            Points[i],
                            Points[i - _parallelCount - 2],
                            Points[i - _parallelCount - 1]));
                    }
                    // Bottomright triangle
                    // (the only one at the top of the rank)
                    // if (index < _parallelCount + 1)
                    if (index < _parallelCount)
                    {
                        this.Triangles.Add(new Point3DTriplet(
                            Points[i],
                            Points[i - _parallelCount - 1],
                            Points[i + 1]));
                    }
                }
            }
        }
        /// <summary>
        /// A method for building the TextureCoordinates collection of the mesh.
        /// </summary>
        internal override void MapTexture()
        {
            Mesh.TextureCoordinates.Clear();
            double vSideCount = _parallelCount + 1;
            double hSideCount = (_parallelCount + 1) * 2;
            for (int i = 0; i <= Points.Count - 1; i++)
            {
                // vRank : vertical point rank (counter-clockwise)
                int vRank = i / (_parallelCount + 2);

                // index : index of current point in the vertical rank
                int index = i % (_parallelCount + 2);
                if (vRank >= 1)
                {
                    double minX = vRank / hSideCount;
                    double maxX = (vRank + 1) / hSideCount;
                    double minY = index / vSideCount;
                    double maxY = (index + 1) / vSideCount;

                    // Upperleft triangle
                    // (the only one at the bottom of the rank)
                    if ((index > 0) && (index < _parallelCount + 1))
                    {
                        Mesh.TextureCoordinates.Add(new Point(maxX, minY));
                        Mesh.TextureCoordinates.Add(new Point(minX, minY));
                        Mesh.TextureCoordinates.Add(new Point(minX, maxY));
                    }
                    // Bottomright triangle
                    // (the only one at the top of the rank)
                    if (index < _parallelCount)
                    {
                        Mesh.TextureCoordinates.Add(new Point(maxX, minY));
                        Mesh.TextureCoordinates.Add(new Point(minX, maxY));
                        Mesh.TextureCoordinates.Add(new Point(maxX, maxY));
                    }
                }
            }
        }
    }
}

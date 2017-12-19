using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Game.Exploration;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;
using GridPoint = Trinity.Components.Adventurer.Game.Exploration.GridPoint;

namespace Trinity.Framework.Helpers
{
    internal class MathUtil
    {


        //http://totologic.blogspot.co.nz/2014/01/accurate-point-in-triangle-test.html

        private const double Epsilon = 0.001;
        private const double EpsilonSquare = Epsilon * Epsilon;

        public bool IsNaivePointInTriangle(Vector3 triPoint1, Vector3 triPoint2, Vector3 triPoint3, GridPoint point)
        {
            return IsNaivePointInTriangle(triPoint1.X, triPoint1.Y, triPoint2.X, triPoint2.Y, triPoint3.X, triPoint3.Y, point.X, point.Y);
        }

        public bool IsNaivePointInTriangle(float x1, float y1, float x2, float y2, float x3, float y3, float x, float y)
        {
            var checkSide1 = Side(x1, y1, x2, y2, x, y) >= 0;
            var checkSide2 = Side(x2, y2, x3, y3, x, y) >= 0;
            var checkSide3 = Side(x3, y3, x1, y1, x, y) >= 0;
            return checkSide1 && checkSide2 && checkSide3;
        }

        public float Side(float x1, float y1, float x2, float y2, float x, float y)
        {
            return (y2 - y1) * (x - x1) + (-x2 + x1) * (y - y1);
        }

        public bool IsPointInTriangleBoundingBox(float x1, float y1, float x2, float y2, float x3, float y3, float x, float y)
        {
            var xMin = Math.Min(x1, Math.Min(x2, x3)) - Epsilon;
            var xMax = Math.Max(x1, Math.Max(x2, x3)) + Epsilon;
            var yMin = Math.Min(y1, Math.Min(y2, y3)) - Epsilon;
            var yMax = Math.Max(y1, Math.Max(y2, y3)) + Epsilon;
            return !(x < xMin) && !(xMax < x) && !(y < yMin) && !(yMax < y);
        }

        public static Vector3 Centroid(IEnumerable<Vector3> points)
        {
            var result = points.Aggregate(Vector3.Zero, (current, point) => current + point);
            result /= points.Count();
            return result;
        }

        private float[] ArrayAggregate(Func<IEnumerable<float>, float> aggregate, params float[][] arrays)
        {
            //var output = ArrayAggregate(Enumerable.Average, array1, array2, array3, array4);

            return Enumerable.Range(0, arrays[0].Length)
                       .Select(i => aggregate(arrays.Select(a => a.Skip(i).First())))
                       .ToArray();
        }

        private T[] ArrayAggregate<T>(Func<IEnumerable<T>, T> aggregate, params T[][] arrays)
        {
            return Enumerable.Range(0, arrays[0].Length)
                       .Select(i => aggregate(arrays.Select(a => a.Skip(i).First())))
                       .ToArray();
        }

        /* Stare at this for a while:
         * http://upload.wikimedia.org/wikipedia/commons/9/9a/Degree-Radian_Conversion.svg
         */

        public static IList<T> RandomShuffle<T>(IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public static List<Vector3> GetCirclePoints(int points, double radius, Vector3 center)
        {
            var result = new List<Vector3>();
            var slice = 2 * Math.PI / points;
            for (var i = 0; i < points; i++)
            {
                var angle = slice * i;
                var newX = (int)(center.X + radius * Math.Cos(angle));
                var newY = (int)(center.Y + radius * Math.Sin(angle));

                var newpoint = new Vector3(newX, newY, center.Z);
                result.Add(newpoint);
            }
            return result;
        }

        internal static bool PositionIsInCircle(Vector3 position, Vector3 center, float radius)
        {
            return center.Distance2DSqr(position) < (Math.Pow((double)radius, (double)radius));
        }

        internal static bool PositionIsInsideArc(Vector3 position, Vector3 center, float radius, float rotation, float arcDegrees)
        {
            if (PositionIsInCircle(position, center, radius))
            {
                return GetIsFacingPosition(position, center, rotation, arcDegrees);
            }
            return false;
        }

        internal static bool GetIsFacingPosition(Vector3 position, Vector3 center, float rotation, float arcDegrees)
        {
            var DirectionVector = GetDirectionVectorFromRotation(rotation);
            if (DirectionVector != Vector2.Zero)
            {
                Vector3 u = position - center;
                u.Z = 0f;
                Vector3 v = new Vector3(DirectionVector.X, DirectionVector.Y, 0f);
                bool result = ((MathEx.ToDegrees(Vector3.AngleBetween(u, v)) <= arcDegrees) ? 1 : 0) != 0;
                return result;
            }
            else
                return false;
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        internal static Vector2 GetDirectionVectorFromRotation(double rotation)
        {
            return new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        }

        #region Angle Finding

        /// <summary>
        /// Find the angle between two vectors. This will not only give the angle difference, but the direction.
        /// For example, it may give you -1 radian, or 1 radian, depending on the direction. Angle given will be the
        /// angle from the FromVector to the DestVector, in radians.
        /// </summary>
        /// <param name="FromVector">Vector to start at.</param>
        /// <param name="DestVector">Destination vector.</param>
        /// <param name="DestVectorsRight">Right vector of the destination vector</param>
        /// <returns>Signed angle, in radians</returns>
        /// <remarks>All three vectors must lie along the same plane.</remarks>
        public static double GetSignedAngleBetween2DVectors(Vector3 FromVector, Vector3 DestVector, Vector3 DestVectorsRight)
        {
            FromVector.Z = 0;
            DestVector.Z = 0;
            DestVectorsRight.Z = 0;

            FromVector.Normalize();
            DestVector.Normalize();
            DestVectorsRight.Normalize();

            float forwardDot = Vector3.Dot(FromVector, DestVector);
            float rightDot = Vector3.Dot(FromVector, DestVectorsRight);

            // Keep dot in range to prevent rounding errors
            forwardDot = MathEx.Clamp(forwardDot, -1.0f, 1.0f);

            double angleBetween = Math.Acos(forwardDot);

            if (rightDot < 0.0f)
                angleBetween *= -1.0f;

            return angleBetween;
        }

        public float UnsignedAngleBetweenTwoV3(Vector3 v1, Vector3 v2)
        {
            v1.Z = 0;
            v2.Z = 0;
            v1.Normalize();
            v2.Normalize();
            double Angle = (float)Math.Acos(Vector3.Dot(v1, v2));
            return (float)Angle;
        }

        /// <summary>
        /// Returns the Degree angle of a target location
        /// </summary>
        /// <param name="vStartLocation"></param>
        /// <param name="vTargetLocation"></param>
        /// <returns></returns>
        public static float FindDirectionDegree(Vector3 vStartLocation, Vector3 vTargetLocation)
        {
            return (float)RadianToDegree(NormalizeRadian((float)Math.Atan2(vTargetLocation.Y - vStartLocation.Y, vTargetLocation.X - vStartLocation.X)));
        }

        public static float FixAngleTo360(float angleDegrees)
        {
            var x = Math.IEEERemainder(angleDegrees, 360);
            if (x < 0)
                x += 360;
            return (float)x;
        }

        public static double FindDirectionRadian(Vector3 start, Vector3 end)
        {
            double radian = Math.Atan2(end.Y - start.Y, end.X - start.X);

            if (radian < 0)
            {
                double mod = -radian;
                mod %= Math.PI * 2d;
                mod = -mod + Math.PI * 2d;
                return mod;
            }
            return (radian % (Math.PI * 2d));
        }

        public Vector3 GetDirection(Vector3 origin, Vector3 destination)
        {
            Vector3 direction = destination - origin;
            direction.Normalize();
            return direction;
        }

        #endregion Angle Finding

        public static bool IntersectsPath(Vector3 obstacle, float radius, Vector3 start, Vector3 destination)
        {
            // fake-it to 2D
            obstacle.Z = 0;
            start.Z = 0;
            destination.Z = 0;

            return MathEx.IntersectsPath(obstacle, radius, start, destination);
        }

        public static bool TrinityIntersectsPath(Vector3 start, Vector3 obstacle, Vector3 destination, float distanceToObstacle = -1, float distanceToDestination = -1)
        {
            var toObstacle = distanceToObstacle >= 0 ? distanceToObstacle : start.Distance(obstacle);
            var toDestination = distanceToDestination >= 0 ? distanceToDestination : start.Distance(destination);

            if (toDestination > 500)
                return false;

            var relativeAngularVariance = GetRelativeAngularVariance(start, obstacle, destination);

            // Angular Variance at 20yd distance
            const int angularVarianceBase = 45;

            // Halve/Double required angle every 20yd; 60* @ 15yd, 11.25* @ 80yd
            var angularVarianceThreshold = Math.Min(angularVarianceBase / (toDestination / 20), 90);

            //Core.Logger.Log("DistToObj={0} DistToDest={1} relativeAV={2} AVThreshold={3} Result={4}",
            //    toObstacle, toDestination, relativeAngularVariance, angularVarianceThreshold,
            //    toObstacle < toDestination && relativeAngularVariance <= angularVarianceThreshold);

            // Obstacle must be than destination
            if (toObstacle < toDestination)
            {
                // If the radius between lines (A) from start to obstacle and (B) from start to destination
                // are small enough then we know both targets are in the same-ish direction from start.
                if (relativeAngularVariance <= angularVarianceThreshold)
                {
                    return true;
                }
            }
            return false;
        }

        public static Vector2 GetDirectionVector(Vector3 start, Vector3 end)
        {
            return new Vector2(end.X - start.X, end.Y - start.Y);
        }

        #region Angular Measure Unit Conversion

        public static double Normalize180(double angleA, double angleB)
        {
            //Returns an angle in the range -180 to 180
            double diffangle = (angleA - angleB) + 180d;
            diffangle = (diffangle / 360.0);
            diffangle = ((diffangle - Math.Floor(diffangle)) * 360.0d) - 180d;
            return diffangle;
        }

        public static float NormalizeRadian(float radian)
        {
            if (radian < 0)
            {
                double mod = -radian;
                mod %= Math.PI * 2d;
                mod = -mod + Math.PI * 2d;
                return (float)mod;
            }
            return (float)(radian % (Math.PI * 2d));
        }

        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        ///// <summary>
        ///// Headingin degrees from Cardinal/InterCardinal direction
        ///// </summary>
        //public static int GetHeadingFromCardinal(Direction direction)
        //{
        //    return (int)direction * 45;
        //}

        #endregion Angular Measure Unit Conversion

        public static double GetRelativeAngularVariance(Vector3 origin, Vector3 destA, Vector3 destB)
        {
            float fDirectionToTarget = NormalizeRadian((float)Math.Atan2(destA.Y - origin.Y, destA.X - origin.X));
            float fDirectionToObstacle = NormalizeRadian((float)Math.Atan2(destB.Y - origin.Y, destB.X - origin.X));
            return AbsAngularDiffernce(RadianToDegree(fDirectionToTarget), RadianToDegree(fDirectionToObstacle));
        }

        public static double AbsAngularDiffernce(double angleA, double angleB)
        {
            return 180d - Math.Abs(180d - Math.Abs(angleA - angleB));
        }

        #region Human Readable Headings

        public static string GetHeadingToPoint(Vector3 TargetPoint)
        {
            return GetHeading(FindDirectionDegree(Core.Player.Position, TargetPoint));
        }

        /// <summary>
        /// Gets string heading NE,S,NE etc
        /// </summary>
        /// <param name="headingDegrees">heading in degrees</param>
        /// <returns></returns>
        public static string GetHeading(float headingDegrees)
        {
            var directions = new string[] {
              //"n", "ne", "e", "se", "s", "sw", "w", "nw", "n"
                "s", "se", "e", "ne", "n", "nw", "w", "sw", "s"
            };

            var index = (((int)headingDegrees) + 23) / 45;
            if (index > 8 || index < 0)
                return "ERR";

            return directions[index].ToUpper();
        }

        #endregion Human Readable Headings

        /// <summary>
        /// Gets the center of a given Navigation Zone
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        internal static Vector3 GetNavZoneCenter(NavZone zone)
        {
            float x = zone.ZoneMin.X + ((zone.ZoneMax.X - zone.ZoneMin.X) / 2);
            float y = zone.ZoneMin.Y + ((zone.ZoneMax.Y - zone.ZoneMin.Y) / 2);

            return new Vector3(x, y, 0);
        }

        /// <summary>
        /// Gets the center of a given Navigation Cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        internal static Vector3 GetNavCellCenter(NavCell cell, NavZone zone)
        {
            return GetNavCellCenter(cell.Min, cell.Max, zone);
        }

        /// <summary>
        /// Gets the center of a given box with min/max, adjusted for the Navigation Zone
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        internal static Vector3 GetNavCellCenter(Vector3 min, Vector3 max, NavZone zone)
        {
            float x = zone.ZoneMin.X + min.X + ((max.X - min.X) / 2);
            float y = zone.ZoneMin.Y + min.Y + ((max.Y - min.Y) / 2);
            float z = min.Z + ((max.Z - min.Z) / 2);

            return new Vector3(x, y, z);
        }

        public static Vector3 GetEstimatedPosition(Vector3 startPosition, double headingRadians, double time, double targetVelocity)
        {
            double x = startPosition.X + targetVelocity * time * Math.Sin(headingRadians);
            double y = startPosition.Y + targetVelocity * time * Math.Cos(headingRadians);
            return new Vector3((float)x, (float)y, 0);
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Uses the Douglas Peucker algorithm to reduce the number of points.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="Points">The points.</param></span>
        /// <span class="code-SummaryComment"><param name="Tolerance">The tolerance.</param></span>
        /// <span class="code-SummaryComment"><returns></returns></span>
        public static List<Vector3> DouglasPeuckerReduction(List<Vector3> Points, Double Tolerance)
        {
            if (Points == null || Points.Count < 3)
                return Points;

            Int32 firstPoint = 0;
            Int32 lastPoint = Points.Count - 1;
            List<Int32> pointIndexsToKeep = new List<Int32>();

            //Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            //The first and the last point cannot be the same
            while (Points[firstPoint].Equals(Points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReduction(Points, firstPoint, lastPoint,
                Tolerance, ref pointIndexsToKeep);

            List<Vector3> returnPoints = new List<Vector3>();
            pointIndexsToKeep.Sort();
            foreach (Int32 index in pointIndexsToKeep)
            {
                returnPoints.Add(Points[index]);
            }

            return returnPoints;
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Douglases the peucker reduction.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="points">The points.</param></span>
        /// <span class="code-SummaryComment"><param name="firstPoint">The first point.</param></span>
        /// <span class="code-SummaryComment"><param name="lastPoint">The last point.</param></span>
        /// <span class="code-SummaryComment"><param name="tolerance">The tolerance.</param></span>
        /// <span class="code-SummaryComment"><param name="pointIndexsToKeep">The point index to keep.</param></span>
        private static void DouglasPeuckerReduction(List<Vector3> points, Int32 firstPoint, Int32 lastPoint, Double tolerance, ref List<Int32> pointIndexsToKeep)
        {
            Double maxDistance = 0;
            Int32 indexFarthest = 0;

            for (Int32 index = firstPoint; index < lastPoint; index++)
            {
                Double distance = PerpendicularDistance
                    (points[firstPoint], points[lastPoint], points[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(points, firstPoint,
                    indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest,
                    lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// The distance of a point from a line made from point1 and point2.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="pt1">The PT1.</param></span>
        /// <span class="code-SummaryComment"><param name="pt2">The PT2.</param></span>
        /// <span class="code-SummaryComment"><param name="p">The p.</param></span>
        /// <span class="code-SummaryComment"><returns></returns></span>
        public static Double PerpendicularDistance(Vector3 Point1, Vector3 Point2, Vector3 Point)
        {
            //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
            //Base = v((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
            //Area = .5*Base*H                                          *Solve for height
            //Height = Area/.5/Base

            Double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X *
                                         Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X *
                                         Point2.Y - Point1.X * Point.Y));
            Double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) +
                                      Math.Pow(Point1.Y - Point2.Y, 2));
            Double height = area / bottom * 2;

            return height;

            //Another option
            //Double A = Point.X - Point1.X;
            //Double B = Point.Y - Point1.Y;
            //Double C = Point2.X - Point1.X;
            //Double D = Point2.Y - Point1.Y;

            //Double dot = A * C + B * D;
            //Double len_sq = C * C + D * D;
            //Double param = dot / len_sq;

            //Double xx, yy;

            //if (param < 0)
            //{
            //    xx = Point1.X;
            //    yy = Point1.Y;
            //}
            //else if (param > 1)
            //{
            //    xx = Point2.X;
            //    yy = Point2.Y;
            //}
            //else
            //{
            //    xx = Point1.X + param * C;
            //    yy = Point1.Y + param * D;
            //}

            //Double d = DistanceBetweenOn2DPlane(Point, new Point(xx, yy));
        }

        /// <summary>
        /// Utility for Predictive Firing
        /// </summary>
        public class Intercept
        {
            /*
                Intercept intercept = new Intercept();

                intercept.calculate (
                        ourRobotPositionX,
                        ourRobotPositionY,
                        currentTargetPositionX,
                        currentTargetPositionY,
                        curentTargetHeading_deg,
                        currentTargetVelocity,
                        bulletPower,
                        0 // Angular velocity
                );

                // Helper function that converts any angle into
                // an angle between +180 and -180 degrees.
                    double turnAngle = normalRelativeAngle(intercept.bulletHeading_deg - robot.getGunHeading());

                // Move gun to target angle
                    robot.setTurnGunRight (turnAngle);

                    if (Math.abs (turnAngle)
                        <= intercept.angleThreshold) {
                  // Ensure that the gun is pointing at the correct angle
                  if ((intercept.impactPoint.x > 0)
                                && (intercept.impactPoint.x < getBattleFieldWidth())
                                && (intercept.impactPoint.y > 0)
                                && (intercept.impactPoint.y < getBattleFieldHeight())) {
                    // Ensure that the predicted impact point is within
                            // the battlefield
                            fire(bulletPower);
                        }
                    }
                }
             */

            public Vector2 impactPoint = new Vector2(0, 0);
            public double bulletHeading_deg;

            protected Vector2 bulletStartingPoint = new Vector2();
            protected Vector2 targetStartingPoint = new Vector2();
            public double targetHeading;
            public double targetVelocity;
            public double bulletPower;
            public double angleThreshold;
            public double distance;

            protected double impactTime;
            protected double angularVelocity_rad_per_sec;

            public void Calculate(
                    // Initial bullet position x coordinate
                    double xb,
                    // Initial bullet position y coordinate
                    double yb,
                    // Initial target position x coordinate
                    double xt,
                    // Initial target position y coordinate
                    double yt,
                    // Target heading
                    double tHeading,
                    // Target velocity
                    double vt,
                    // Power of the bullet that we will be firing
                    double bPower,
                    // Angular velocity of the target
                    double angularVelocityDegPerSec,
                    // target object's radius
                    double targetsRadius
            )
            {
                angularVelocity_rad_per_sec = DegreeToRadian(angularVelocityDegPerSec);

                bulletStartingPoint = new Vector2((float)xb, (float)yb);
                targetStartingPoint = new Vector2((float)xt, (float)yt);

                targetHeading = tHeading;
                targetVelocity = vt;
                bulletPower = bPower;
                double vb = 20 - 3 * bulletPower;

                // Start with initial guesses at 10 and 20 ticks
                impactTime = GetImpactTime(10, 20, 0.01);
                impactPoint = GetEstimatedPosition(impactTime);

                double dX = (impactPoint.X - bulletStartingPoint.X);
                double dY = (impactPoint.Y - bulletStartingPoint.Y);

                distance = Math.Sqrt(dX * dX + dY * dY);

                bulletHeading_deg = RadianToDegree(Math.Atan2(dX, dY));
                angleThreshold = RadianToDegree(Math.Atan(targetsRadius / distance));
            }

            protected Vector2 GetEstimatedPosition(double time)
            {
                double x = targetStartingPoint.X + targetVelocity * time * Math.Sin(DegreeToRadian(targetHeading));
                double y = targetStartingPoint.Y + targetVelocity * time * Math.Cos(DegreeToRadian(targetHeading));
                return new Vector2((float)x, (float)y);
            }

            private double F(double time)
            {
                double vb = 20 - 3 * bulletPower;

                Vector2 targetPosition = GetEstimatedPosition(time);
                double dX = (targetPosition.X - bulletStartingPoint.X);
                double dY = (targetPosition.Y - bulletStartingPoint.Y);

                return Math.Sqrt(dX * dX + dY * dY) - vb * time;
            }

            private double GetImpactTime(double t0,
                    double t1, double accuracy)
            {
                double X = t1;
                double lastX = t0;
                int iterationCount = 0;
                double lastfX = F(lastX);

                while ((Math.Abs(X - lastX) >= accuracy)
                        && (iterationCount < 15))
                {
                    iterationCount++;
                    double fX = F(X);

                    if ((fX - lastfX) == 0.0)
                    {
                        break;
                    }

                    double nextX = X - fX * (X - lastX) / (fX - lastfX);
                    lastX = X;
                    X = nextX;
                    lastfX = fX;
                }

                return X;
            }
        }

        public class CircularIntercept : Intercept
        {
            protected new Vector2 GetEstimatedPosition(double time)
            {
                if (Math.Abs(angularVelocity_rad_per_sec)
                        <= DegreeToRadian(0.1))
                {
                    return base.GetEstimatedPosition(time);
                }

                double initialTargetHeading = DegreeToRadian(targetHeading);
                double finalTargetHeading = initialTargetHeading
                        + angularVelocity_rad_per_sec * time;
                double x = targetStartingPoint.X - targetVelocity
                        / angularVelocity_rad_per_sec * (Math.Cos(finalTargetHeading)
                        - Math.Cos(initialTargetHeading));
                double y = targetStartingPoint.Y - targetVelocity
                        / angularVelocity_rad_per_sec
                        * (Math.Sin(initialTargetHeading)
                        - Math.Sin(finalTargetHeading));

                return new Vector2((float)x, (float)y);
            }
        }

        public static float ToRadians(float degrees)
        {
            // This method uses double precission internally,
            // though it returns single double
            // Factor = pi / 180
            return (float)(degrees * 0.017453292519943295769236907684886);
        }


        #region Human Readable Headings

        public static Direction GetDirectionToPoint(Vector3 targetPoint)
        {
            return GetDirection(FindDirectionDegree(ZetaDia.Me.Position, targetPoint));
        }

        public static Direction GetDirectionToPoint(Vector3 targetPoint, Vector3 startingPoint)
        {
            return GetDirection(FindDirectionDegree(startingPoint, targetPoint));
        }

        public static Direction GetDirection(float heading)
        {
            var index = ((((int)heading) + 23) / 45) + 1;
            if (index == 9)
                index = 1;
            return (Direction)index; ;
        }



        #endregion
    }

    public static class Quartiles
    {
        public static double LowerQuartile(this IOrderedEnumerable<double> list)
        {
            return GetQuartile(list, 0.25);
        }

        public static double UpperQuartile(this IOrderedEnumerable<double> list)
        {
            return GetQuartile(list, 0.75);
        }

        public static double MiddleQuartile(this IOrderedEnumerable<double> list)
        {
            return GetQuartile(list, 0.50);
        }

        public static double InterQuartileRange(this IOrderedEnumerable<double> list)
        {
            return list.UpperQuartile() - list.LowerQuartile();
        }

        private static double GetQuartile(IOrderedEnumerable<double> list, double quartile)
        {
            double result;

            // Get roughly the index
            double index = quartile * (list.Count() + 1);

            // Get the remainder of that index value if exists
            double remainder = index % 1;

            // Get the integer value of that index
            index = Math.Floor(index) - 1;

            if (remainder.Equals(0))
            {
                // we have an integer value, no interpolation needed
                result = list.ElementAt((int)index);
            }
            else
            {
                // we need to interpolate
                double value = list.ElementAt((int)index);
                double interpolationValue = value
                    .Interpolate(list.ElementAt((int)(index + 1)), remainder);

                result = value + interpolationValue;
            }

            return result;
        }

        private static double Interpolate(this double a, double b, double remainder)
        {
            return (b - a) * remainder;
        }


    }
}
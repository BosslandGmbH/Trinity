using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zeta.Common;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;
using GridPoint = Trinity.Components.Adventurer.Game.Exploration.GridPoint;

namespace Trinity.Framework.Helpers
{
    class MathUtil
    {

        //http://totologic.blogspot.co.nz/2014/01/accurate-point-in-triangle-test.html

        const double Epsilon = 0.001;
        const double EpsilonSquare = Epsilon * Epsilon;

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


        //        function side(x1, y1, x2, y2, x, y:Number):Number
        //{
        // return (y2 - y1)*(x - x1) + (-x2 + x1)*(y - y1);

        //    function naivePointInTriangle(x1, y1, x2, y2, x3, y3, x, y:Number):Boolean
        //{
        // var checkSide1:Boolean = side(x1, y1, x2, y2, x, y) >= 0;
        // var checkSide2:Boolean = side(x2, y2, x3, y3, x, y) >= 0;
        // var checkSide3:Boolean = side(x3, y3, x1, y1, x, y) >= 0;
        // return checkSide1 && checkSide2 && checkSide3;
        //}

        //function pointInTriangleBoundingBox(x1, y1, x2, y2, x3, y3, x, y:Number):Boolean
        //{
        // var xMin:Number = Math.min(x1, Math.min(x2, x3)) - EPSILON;
        // var xMax:Number = Math.max(x1, Math.max(x2, x3)) + EPSILON;
        // var yMin:Number = Math.min(y1, Math.min(y2, y3)) - EPSILON;
        // var yMax:Number = Math.max(y1, Math.max(y2, y3)) + EPSILON;

        // if ( x<xMin || xMax<x || y<yMin || yMax<y )
        //  return false;
        // else
        //  return true;
        //}

        //function distanceSquarePointToSegment(x1, y1, x2, y2, x, y:Number):Number
        //{
        // var p1_p2_squareLength:Number = (x2 - x1)*(x2 - x1) + (y2 - y1)*(y2 - y1);
        // var dotProduct:Number = ((x - x1)*(x2 - x1) + (y - y1)*(y2 - y1)) / p1_p2_squareLength;
        // if ( dotProduct< 0 )
        // {
        //  return (x - x1)*(x - x1) + (y - y1)*(y - y1);
        // }
        // else if ( dotProduct <= 1 )
        // {
        //  var p_p1_squareLength:Number = (x1 - x)*(x1 - x) + (y1 - y)*(y1 - y);
        //  return p_p1_squareLength - dotProduct* dotProduct * p1_p2_squareLength;
        // }
        // else
        // {
        //  return (x - x2)*(x - x2) + (y - y2)*(y - y2);
        // }
        //}

        //function accuratePointInTriangle(x1, y1, x2, y2, x3, y3, x, y:Number):Boolean
        //{
        // if (! pointInTriangleBoundingBox(x1, y1, x2, y2, x3, y3, x, y))
        //  return false;

        // if (naivePointInTriangle(x1, y1, x2, y2, x3, y3, x, y))
        //  return true;

        // if (distanceSquarePointToSegment(x1, y1, x2, y2, x, y) <= EPSILON_SQUARE)
        //  return true;
        // if (distanceSquarePointToSegment(x2, y2, x3, y3, x, y) <= EPSILON_SQUARE)
        //  return true;
        // if (distanceSquarePointToSegment(x3, y3, x1, y1, x, y) <= EPSILON_SQUARE)
        //  return true;

        // return false;
        //}

        public static Vector3 Centroid(IEnumerable<Vector3> points)
        {
            var result = points.Aggregate(Vector3.Zero, (current, point) => current + point);
            result /= points.Count();
            return result;
        }


        float[] ArrayAggregate(Func<IEnumerable<float>, float> aggregate, params float[][] arrays)
        {
            //var output = ArrayAggregate(Enumerable.Average, array1, array2, array3, array4);

            return Enumerable.Range(0, arrays[0].Length)
                       .Select(i => aggregate(arrays.Select(a => a.Skip(i).First())))
                       .ToArray();
        }

        T[] ArrayAggregate<T>(Func<IEnumerable<T>, T> aggregate, params T[][] arrays)
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
        #endregion


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

            //Logger.Log("DistToObj={0} DistToDest={1} relativeAV={2} AVThreshold={3} Result={4}", 
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

        #endregion
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
        #endregion

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

                bulletStartingPoint = new Vector2((float) xb, (float) yb);
                targetStartingPoint = new Vector2((float) xt, (float) yt);

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
                return new Vector2((float) x, (float) y);
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

        public class CircularIntercept : Intercept {

            protected new Vector2 GetEstimatedPosition(double time) {
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

                return new Vector2((float) x, (float) y);
            }

        }

        public static float ToRadians(float degrees)
        {
            // This method uses double precission internally,
            // though it returns single double
            // Factor = pi / 180
            return (float)(degrees * 0.017453292519943295769236907684886);
        }

        //#region LeagueSharp + SharpDX MathUtils

        //public static bool CheckLineIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        //{
        //    return a.Intersection(b, c, d).Intersects;
        //}

        //public static bool CheckLineIntersectionEx(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        //{
        //    Tuple<float, float> ret = LineToLineIntersection(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y);

        //    var t1 = ret.Item1;
        //    var t2 = ret.Item2;

        //    if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static Vector2 CheckLineIntersectionEx2(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        //{
        //    Tuple<float, float> ret = LineToLineIntersection(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y);

        //    var t1 = ret.Item1;
        //    var t2 = ret.Item2;

        //    if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
        //    {
        //        return new Vector2(t1, t2);
        //    }
        //    else
        //    {
        //        return Vector2.Zero;
        //    }
        //}

        //public static Vector2 RotateVector(Vector2 start, Vector2 end, float angle)
        //{
        //    angle = angle * ((float)(Math.PI / 180));
        //    Vector2 ret = end;
        //    ret.X = ((float)Math.Cos(angle) * (end.X - start.X) -
        //        (float)Math.Sin(angle) * (end.Y - start.Y) + start.X);
        //    ret.Y = ((float)Math.Sin(angle) * (end.X - start.X) +
        //        (float)Math.Cos(angle) * (end.Y - start.Y) + start.Y);
        //    return ret;
        //}

        //public static Tuple<float, float> LineToLineIntersection(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        //{
        //    var d = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);

        //    if (d == 0)
        //    {
        //        return Tuple.Create(float.MaxValue, float.MaxValue); //lines are parallel or coincidental
        //    }
        //    else
        //    {
        //        return Tuple.Create(((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / d,
        //            ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / d);
        //    }
        //}

        ///*
        // * 
        // * //from leaguesharp.commons
        //        var spellPos = spell.GetCurrentSpellPosition(true);
        //        var sol = Geometry.VectorMovementCollision(spellPos, spell.endPos, spell.info.projectileSpeed, heroPos, ObjectCache.myHeroCache.moveSpeed);

        //        var startTime = 0f;
        //        var endTime = spellPos.Distance(spell.endPos) / spell.info.projectileSpeed;

        //        var time = (float) sol[0];
        //        var pos = (Vector2) sol[1];

        //        if (pos.IsValid() && time >= startTime && time <= startTime + endTime)
        //        {
        //            return true;
        //        }
        // * 
        // */


        //public static float VectorMovementCollisionEx(Vector2 targetPos, Vector2 targetDir, float targetSpeed, Vector2 sourcePos, float projSpeed, out bool collision, float extraDelay = 0, float extraDist = 0)
        //{
        //    Vector2 velocity = targetDir * targetSpeed;
        //    targetPos = targetPos - velocity * (extraDelay / 1000);

        //    float velocityX = velocity.X;
        //    float velocityY = velocity.Y;

        //    Vector2 relStart = targetPos - sourcePos;

        //    float relStartX = relStart.X;
        //    float relStartY = relStart.Y;

        //    float a = velocityX * velocityX + velocityY * velocityY - projSpeed * projSpeed;
        //    float b = 2 * velocityX * relStartX + 2 * velocityY * relStartY;
        //    float c = Math.Max(0, relStartX * relStartX + relStartY * relStartY + extraDist * extraDist);

        //    float disc = b * b - 4 * a * c;

        //    if (disc >= 0)
        //    {
        //        float t1 = -(b + (float)Math.Sqrt(disc)) / (2 * a);
        //        float t2 = -(b - (float)Math.Sqrt(disc)) / (2 * a);

        //        collision = true;

        //        if (t1 > 0 && t2 > 0)
        //        {
        //            return (t1 > t2) ? t2 : t1;

        //        }
        //        else if (t1 > 0)
        //            return t1;
        //        else if (t2 > 0)
        //            return t2;
        //    }

        //    collision = false;

        //    return 0;
        //}

        //public static bool PointOnLineSegment(Vector2 point, Vector2 start, Vector2 end)
        //{
        //    var dotProduct = Vector2.Dot((end - start), (point - start));
        //    if (dotProduct < 0)
        //        return false;

        //    var lengthSquared = Vector2.DistanceSquared(start, end);
        //    if (dotProduct > lengthSquared)
        //        return false;

        //    return true;
        //}

        //public static bool isPointOnLineSegment(Vector2 point, Vector2 start, Vector2 end)
        //{
        //    if (Math.Max(start.X, end.X) > point.X && point.X > Math.Min(start.X, end.X)
        //        && Math.Max(start.Y, end.Y) > point.Y && point.Y > Math.Min(start.Y, end.Y))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        ////https://code.google.com/p/xna-circle-collision-detection/downloads/detail?name=Circle%20Collision%20Example.zip&can=2&q=

        //public static float GetCollisionTime(Vector2 Pa, Vector2 Pb, Vector2 Va, Vector2 Vb, float Ra, float Rb, out bool collision)
        //{
        //    Vector2 Pab = Pa - Pb;
        //    Vector2 Vab = Va - Vb;
        //    float a = Vector2.Dot(Vab, Vab);
        //    float b = 2 * Vector2.Dot(Pab, Vab);
        //    float c = Vector2.Dot(Pab, Pab) - (Ra + Rb) * (Ra + Rb);

        //    float discriminant = b * b - 4 * a * c;

        //    float t;
        //    if (discriminant < 0)
        //    {
        //        t = -b / (2 * a);
        //        collision = false;
        //    }
        //    else
        //    {
        //        float t0 = (-b + (float)Math.Sqrt(discriminant)) / (2 * a);
        //        float t1 = (-b - (float)Math.Sqrt(discriminant)) / (2 * a);

        //        if (t0 >= 0 && t1 >= 0)
        //            t = Math.Min(t0, t1);
        //        else
        //            t = Math.Max(t0, t1);

        //        if (t < 0)
        //            collision = false;
        //        else
        //            collision = true;
        //    }

        //    if (t < 0)
        //        t = 0;

        //    return t;
        //}

        //public static float GetCollisionDistanceEx(Vector2 Pa, Vector2 Va, float Ra,
        //                                           Vector2 Pb, Vector2 Vb, float Rb,
        //                                           out Vector2 PA, out Vector2 PB)
        //{
        //    bool collision;
        //    var collisionTime = GetCollisionTime(Pa, Pb, Va, Vb, Ra, Rb, out collision);

        //    if (collision)
        //    {
        //        PA = Pa + (collisionTime * Va);
        //        PB = Pb + (collisionTime * Vb);

        //        return PA.Distance(PB);
        //    }

        //    PA = Vector2.Zero;
        //    PB = Vector2.Zero;

        //    return float.MaxValue;
        //}

        //public static float GetCollisionDistance(Vector2 Pa, Vector2 PaEnd, Vector2 Va, float Ra,
        //                                         Vector2 Pb, Vector2 PbEnd, Vector2 Vb, float Rb)
        //{
        //    bool collision;
        //    var collisionTime = GetCollisionTime(Pa, Pb, Va, Vb, Ra, Rb, out collision);

        //    if (collision)
        //    {
        //        Vector2 PA = Pa + (collisionTime * Va);
        //        Vector2 PB = Pb + (collisionTime * Vb);

        //        PA = PA.ProjectOn(Pa, PaEnd).SegmentPoint;
        //        PB = PB.ProjectOn(Pb, PbEnd).SegmentPoint;

        //        return PA.Distance(PB);
        //    }

        //    return float.MaxValue;
        //}

        ////http://csharphelper.com/blog/2014/09/determine-where-a-line-intersects-a-circle-in-c/
        //// Find the points of intersection.
        //public static int FindLineCircleIntersections(
        //    Vector2 center, float radius,
        //    Vector2 from, Vector2 to,
        //    out Vector2 intersection1, out Vector2 intersection2)
        //{
        //    float cx = center.X;
        //    float cy = center.Y;
        //    float dx, dy, A, B, C, det, t;

        //    dx = to.X - from.X;
        //    dy = to.Y - from.Y;

        //    A = dx * dx + dy * dy;
        //    B = 2 * (dx * (from.X - cx) + dy * (from.Y - cy));
        //    C = (from.X - cx) * (from.X - cx) +
        //        (from.Y - cy) * (from.Y - cy) -
        //        radius * radius;

        //    det = B * B - 4 * A * C;
        //    if ((A <= 0.0000001) || (det < 0))
        //    {
        //        // No real solutions.
        //        intersection1 = new Vector2(float.NaN, float.NaN);
        //        intersection2 = new Vector2(float.NaN, float.NaN);
        //        return 0;
        //    }
        //    else if (det == 0)
        //    {
        //        // One solution.
        //        t = -B / (2 * A);
        //        intersection1 =
        //            new Vector2(from.X + t * dx, from.Y + t * dy);
        //        intersection2 = new Vector2(float.NaN, float.NaN);

        //        var projection1 = intersection1.ProjectOn(from, to);
        //        if (projection1.IsOnSegment)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    else
        //    {
        //        // Two solutions.
        //        t = (float)((-B + Math.Sqrt(det)) / (2 * A));
        //        intersection1 =
        //            new Vector2(from.X + t * dx, from.Y + t * dy);
        //        t = (float)((-B - Math.Sqrt(det)) / (2 * A));
        //        intersection2 =
        //            new Vector2(from.X + t * dx, from.Y + t * dy);

        //        var projection1 = intersection1.ProjectOn(from, to);
        //        var projection2 = intersection2.ProjectOn(from, to);

        //        if (projection1.IsOnSegment && projection2.IsOnSegment)
        //        {
        //            return 2;
        //        }
        //        else if (projection1.IsOnSegment && !projection2.IsOnSegment)
        //        {
        //            return 1;
        //        }
        //        else if (!projection1.IsOnSegment && projection2.IsOnSegment)
        //        {
        //            intersection1 = intersection2;
        //            return 1;
        //        }

        //        return 0;
        //    }
        //}

        //#endregion
    }

    public static class RectExtentions
    {
        //improved name from original
        public static IEnumerable<LineEquation> LineSegments(this Rect rectangle)
        {
            var lines = new List<LineEquation>
            {
                new LineEquation(new Point(rectangle.X, rectangle.Y),
                                 new Point(rectangle.X, rectangle.Y + rectangle.Height)),

                new LineEquation(new Point(rectangle.X, rectangle.Y + rectangle.Height),
                                 new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height)),

                new LineEquation(new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height),
                                 new Point(rectangle.X + rectangle.Width, rectangle.Y)),

                new LineEquation(new Point(rectangle.X + rectangle.Width, rectangle.Y),
                                 new Point(rectangle.X, rectangle.Y)),
            };

            return lines;
        }

        //improved from original at http://www.codeproject.com/Tips/403031/Extension-methods-for-finding-centers-of-a-rectang

        /// <summary>
        /// Returns the center point of the rectangle
        /// </summary>
        /// <param name="r"></param>
        /// <returns>Center point of the rectangle</returns>
        public static Point Center(this Rect r)
        {
            return new Point(r.Left + (r.Width / 2D), r.Top + (r.Height / 2D));
        }
        /// <summary>
        /// Returns the center right point of the rectangle
        /// i.e. the right hand edge, centered vertically.
        /// </summary>
        /// <param name="r"></param>
        /// <returns>Center right point of the rectangle</returns>
        public static Point CenterRight(this Rect r)
        {
            return new Point(r.Right, r.Top + (r.Height / 2D));
        }
        /// <summary>
        /// Returns the center left point of the rectangle
        /// i.e. the left hand edge, centered vertically.
        /// </summary>
        /// <param name="r"></param>
        /// <returns>Center left point of the rectangle</returns>
        public static Point CenterLeft(this Rect r)
        {
            return new Point(r.Left, r.Top + (r.Height / 2D));
        }
        /// <summary>
        /// Returns the center bottom point of the rectangle
        /// i.e. the bottom edge, centered horizontally.
        /// </summary>
        /// <param name="r"></param>
        /// <returns>Center bottom point of the rectangle</returns>
        public static Point CenterBottom(this Rect r)
        {
            return new Point(r.Left + (r.Width / 2D), r.Bottom);
        }
        /// <summary>
        /// Returns the center top point of the rectangle
        /// i.e. the topedge, centered horizontally.
        /// </summary>
        /// <param name="r"></param>
        /// <returns>Center top point of the rectangle</returns>
        public static Point CenterTop(this Rect r)
        {
            return new Point(r.Left + (r.Width / 2D), r.Top);
        }
    }

    public class LineEquation
    {
        public LineEquation(Point start, Point end)
        {
            Start = start;
            End = end;

            IsVertical = Math.Abs(End.X - start.X) < 0.00001f;
            M = (End.Y - Start.Y) / (End.X - Start.X);
            A = -M;
            B = 1;
            C = Start.Y - M * Start.X;
        }

        public bool IsVertical { get; private set; }

        public double M { get; private set; }

        public Point Start { get; private set; }
        public Point End { get; private set; }

        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }

        public bool IntersectsWithLine(LineEquation otherLine, out Point intersectionPoint)
        {
            intersectionPoint = new Point(0, 0);
            if (IsVertical && otherLine.IsVertical)
                return false;
            if (IsVertical || otherLine.IsVertical)
            {
                intersectionPoint = GetIntersectionPointIfOneIsVertical(otherLine, this);
                return true;
            }
            double delta = A * otherLine.B - otherLine.A * B;
            bool hasIntersection = Math.Abs(delta - 0) > 0.0001f;
            if (hasIntersection)
            {
                double x = (otherLine.B * C - B * otherLine.C) / delta;
                double y = (A * otherLine.C - otherLine.A * C) / delta;
                intersectionPoint = new Point(x, y);
            }
            return hasIntersection;
        }

        private static Point GetIntersectionPointIfOneIsVertical(LineEquation line1, LineEquation line2)
        {
            LineEquation verticalLine = line2.IsVertical ? line2 : line1;
            LineEquation nonVerticalLine = line2.IsVertical ? line1 : line2;

            double y = (verticalLine.Start.X - nonVerticalLine.Start.X) *
                       (nonVerticalLine.End.Y - nonVerticalLine.Start.Y) /
                       ((nonVerticalLine.End.X - nonVerticalLine.Start.X)) +
                       nonVerticalLine.Start.Y;
            double x = line1.IsVertical ? line1.Start.X : line2.Start.X;
            return new Point(x, y);
        }

        public bool IntersectWithSegementOfLine(LineEquation otherLine, out Point intersectionPoint)
        {
            bool hasIntersection = IntersectsWithLine(otherLine, out intersectionPoint);
            if (hasIntersection)
                return intersectionPoint.IsBetweenTwoPoints(otherLine.Start, otherLine.End);
            return false;
        }

        public override string ToString()
        {
            return "[" + Start + "], [" + End + "]";
        }
    }

    public static class DoubleExtensions
    {
        //SOURCE: https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/Precision.cs
        //        https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/Precision.Equality.cs
        //        http://referencesource.microsoft.com/#WindowsBase/Shared/MS/Internal/DoubleUtil.cs
        //        http://stackoverflow.com/questions/2411392/double-epsilon-for-equality-greater-than-less-than-less-than-or-equal-to-gre

        /// <summary>
        /// The smallest positive number that when SUBTRACTED from 1D yields a result different from 1D.
        /// The value is derived from 2^(-53) = 1.1102230246251565e-16, where IEEE 754 binary64 &quot;double precision&quot; floating point numbers have a significand precision that utilize 53 bits.
        ///
        /// This number has the following properties:
        ///     (1 - NegativeMachineEpsilon) &lt; 1 and
        ///     (1 + NegativeMachineEpsilon) == 1
        /// </summary>
        public const double NegativeMachineEpsilon = 1.1102230246251565e-16D; //Math.Pow(2, -53);

        /// <summary>
        /// The smallest positive number that when ADDED to 1D yields a result different from 1D.
        /// The value is derived from 2 * 2^(-53) = 2.2204460492503131e-16, where IEEE 754 binary64 &quot;double precision&quot; floating point numbers have a significand precision that utilize 53 bits.
        ///
        /// This number has the following properties:
        ///     (1 - PositiveDoublePrecision) &lt; 1 and
        ///     (1 + PositiveDoublePrecision) &gt; 1
        /// </summary>
        public const double PositiveMachineEpsilon = 2D * NegativeMachineEpsilon;

        /// <summary>
        /// The smallest positive number that when SUBTRACTED from 1D yields a result different from 1D.
        ///
        /// This number has the following properties:
        ///     (1 - NegativeMachineEpsilon) &lt; 1 and
        ///     (1 + NegativeMachineEpsilon) == 1
        /// </summary>
        public static readonly double MeasuredNegativeMachineEpsilon = MeasureNegativeMachineEpsilon();

        private static double MeasureNegativeMachineEpsilon()
        {
            double epsilon = 1D;

            do
            {
                double nextEpsilon = epsilon / 2D;

                if ((1D - nextEpsilon) == 1D) //if nextEpsilon is too small
                    return epsilon;

                epsilon = nextEpsilon;
            }
            while (true);
        }

        /// <summary>
        /// The smallest positive number that when ADDED to 1D yields a result different from 1D.
        ///
        /// This number has the following properties:
        ///     (1 - PositiveDoublePrecision) &lt; 1 and
        ///     (1 + PositiveDoublePrecision) &gt; 1
        /// </summary>
        public static readonly double MeasuredPositiveMachineEpsilon = MeasurePositiveMachineEpsilon();

        private static double MeasurePositiveMachineEpsilon()
        {
            double epsilon = 1D;

            do
            {
                double nextEpsilon = epsilon / 2D;

                if ((1D + nextEpsilon) == 1D) //if nextEpsilon is too small
                    return epsilon;

                epsilon = nextEpsilon;
            }
            while (true);
        }

        const double DefaultDoubleAccuracy = NegativeMachineEpsilon * 10D;

        public static bool IsClose(this double value1, double value2)
        {
            return IsClose(value1, value2, DefaultDoubleAccuracy);
        }

        public static bool IsClose(this double value1, double value2, double maximumAbsoluteError)
        {
            if (double.IsInfinity(value1) || double.IsInfinity(value2))
                return value1 == value2;

            if (double.IsNaN(value1) || double.IsNaN(value2))
                return false;

            double delta = value1 - value2;

            //return Math.Abs(delta) <= maximumAbsoluteError;

            if (delta > maximumAbsoluteError ||
                delta < -maximumAbsoluteError)
                return false;

            return true;
        }

        public static bool LessThan(this double value1, double value2)
        {
            return (value1 < value2) && !IsClose(value1, value2);
        }

        public static bool GreaterThan(this double value1, double value2)
        {
            return (value1 > value2) && !IsClose(value1, value2);
        }

        public static bool LessThanOrClose(this double value1, double value2)
        {
            return (value1 < value2) || IsClose(value1, value2);
        }

        public static bool GreaterThanOrClose(this double value1, double value2)
        {
            return (value1 > value2) || IsClose(value1, value2);
        }

        public static bool IsOne(this double value)
        {
            double delta = value - 1D;

            //return Math.Abs(delta) <= PositiveMachineEpsilon;

            if (delta > PositiveMachineEpsilon ||
                delta < -PositiveMachineEpsilon)
                return false;

            return true;
        }

        public static bool IsZero(this double value)
        {
            //return Math.Abs(value) <= PositiveMachineEpsilon;

            if (value > PositiveMachineEpsilon ||
                value < -PositiveMachineEpsilon)
                return false;

            return true;
        }
    }

    public static class PointExtensions
    {
        public static double DistanceToPoint(this Point point, Point point2)
        {
            return Math.Sqrt((point2.X - point.X) * (point2.X - point.X) + (point2.Y - point.Y) * (point2.Y - point.Y));
        }

        public static double SquaredDistanceToPoint(this Point point, Point point2)
        {
            return (point2.X - point.X) * (point2.X - point.X) + (point2.Y - point.Y) * (point2.Y - point.Y);
        }

        public static bool IsBetweenTwoPoints(this Point targetPoint, Point point1, Point point2)
        {
            double minX = Math.Min(point1.X, point2.X);
            double minY = Math.Min(point1.Y, point2.Y);
            double maxX = Math.Max(point1.X, point2.X);
            double maxY = Math.Max(point1.Y, point2.Y);

            double targetX = targetPoint.X;
            double targetY = targetPoint.Y;

            return minX.LessThanOrClose(targetX)
                  && targetX.LessThanOrClose(maxX)
                  && minY.LessThanOrClose(targetY)
                  && targetY.LessThanOrClose(maxY);
        }
    }



}

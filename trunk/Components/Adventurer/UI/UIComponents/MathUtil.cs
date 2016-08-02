using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using Zeta.Bot.Pathfinding;
using Zeta.Common;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;
using Point = System.Windows.Point;

namespace Trinity.Components.Adventurer.UI.UIComponents
{
    class MathUtil
    {
        /* Stare at this for a while:
         * http://upload.wikimedia.org/wikipedia/commons/9/9a/Degree-Radian_Conversion.svg
         */

        internal static bool PositionIsInCircle(Vector3 position, Vector3 center, float radius)
        {
            if (center.Distance2DSqr(position) < (Math.Pow((double)radius, (double)radius)))
                return true;
            return false;
        }

        /// <summary>
        /// Creates a rectangle based on two points.
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>Rectangle</returns>
        public static RectangleF GetRectangle(Vector3 p1, Vector3 p2)
        {
            float top = Math.Min(p1.Y, p2.Y);
            float bottom = Math.Max(p1.Y, p2.Y);
            float left = Math.Min(p1.X, p2.X);
            float right = Math.Max(p1.X, p2.X);
            RectangleF rect = RectangleF.FromLTRB(left, top, right, bottom);
            return rect;
        }

      



        //Normalizes any number to an arbitrary range 
        //by assuming the range wraps around when going below min or above max 
        public static double Normalise(double value, double start, double end) 
        {
          var width = end - start; 
          var offsetValue = value - start ;   // value relative to 0
          return offsetValue - ( Math.Floor(( offsetValue / width ) * width ) ) + start;
        }

        //public static PathGeometry GetRotatedGeometry(Rect rect, float rotationDegrees, Vector3 rotationCenter)
        //{
        //    var positions = new List<Vector3>
        //    {
        //        new Vector3((float)rect.BottomRight.X,(float)rect.BottomRight.Y,0),                
        //        new Vector3((float)rect.BottomLeft.X,(float)rect.BottomLeft.Y,0),
        //        new Vector3((float)rect.TopLeft.X,(float)rect.TopLeft.Y,0),
        //        new Vector3((float)rect.TopRight.X,(float)rect.TopRight.Y,0),
        //    };

        //    return GetRotatedGeometry(positions, rotationDegrees, rotationCenter);
        //}

        //public static PathGeometry GetRotatedGeometry(List<Vector3> positions, float rotationDegrees, Vector3 rotationCenter)
        //{
        //    if (!positions.Any())
        //        return null;

        //    var rotationTransform = new RotateTransform(rotationDegrees, rotationCenter.X, rotationCenter.Y);
        //    var figures = new List<PathFigure>();
        //    var segments = positions.Select(position =>
        //    {
        //        var point = new Point(position.X, position.Y);
        //        var rotatedPoint = rotationTransform.Transform(point);
        //        return new System.Windows.Media.LineSegment(rotatedPoint, false);

        //    }).ToList();

        //    var firstSegment = segments.First();
        //    segments.RemoveAt(0);
        //    figures.Add(new PathFigure(firstSegment.Point, segments, true));
        //    var geo = new PathGeometry(figures, FillRule.Nonzero, null);
        //    geo.GetOutlinedPathGeometry();                        
        //    return geo;
        //}

        public static bool IsPointWithinGeometry(Geometry geom, Vector3 point)
        {
            return geom.FillContains(new Point(point.X, point.Y));
        }

        // Return True if the point is in the polygon.
        public bool PointInPolygon(float x, float y, IList<Vector2> points)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int maxPoint = points.Count - 1;
            var pointsArray = points.ToArray();
            float totalAngle = GetAngle(
                pointsArray[maxPoint].X, pointsArray[maxPoint].Y,
                x, y,
                pointsArray[0].X, pointsArray[0].Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < maxPoint; i++)
            {
                totalAngle += GetAngle(
                    pointsArray[i].X, pointsArray[i].Y,
                    x, y,
                    pointsArray[i + 1].X, pointsArray[i + 1].Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(totalAngle) > 0.000001);
        }

        // Return the angle ABC.
        // Return a value between PI and -PI.
        // Note that the value is the opposite of what you might
        // expect because Y coordinates increase downward.
        public static float GetAngle(float Ax, float Ay,
            float Bx, float By, float Cx, float cy)
        {
            // Get the dot product.
            float dotProduct = DotProduct(Ax, Ay, Bx, By, Cx, cy);

            // Get the cross product.
            float crossProduct = CrossProductLength(Ax, Ay, Bx, By, Cx, cy);

            // Calculate the angle.
            return (float)Math.Atan2(crossProduct, dotProduct);
        }

        // Return the dot product AB · BC.
        // Note that AB · BC = |AB| * |BC| * Cos(theta).
        private static float DotProduct(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }

        // Return the cross product AB x BC.
        // The cross product is a vector perpendicular to AB
        // and BC having length |AB| * |BC| * Sin(theta) and
        // with direction given by the right-hand rule.
        // For two vectors in the X-Y plane, the result is a
        // vector with X and Y components 0 so the Z component
        // gives the vector's length and direction.
        public static float CrossProductLength(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
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
            var directionVector = GetDirectionVectorFromRotation(rotation);
            if (directionVector != Vector2.Zero)
            {
                Vector3 u = position - center;
                u.Z = 0f;
                Vector3 v = new Vector3(directionVector.X, directionVector.Y, 0f);
                bool result = ((MathEx.ToDegrees(Vector3.AngleBetween(u, v)) <= arcDegrees) ? 1 : 0) != 0;
                return result;
            }
            else
                return false;
        }

        public static double DegreeToRadian(double angle) => Math.PI * angle / 180.0;

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

            if (toObstacle < toDestination)
            {
                // If the angle between lines (A) from start to obstacle and (B) from start to destination
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
            return directions[index].ToUpper();
        }



        public static float FixAngleTo360(float angleDegrees)
        {
            //if (angleDegrees > 360)
            //    angleDegrees = Math.Abs(360 - angleDegrees);

            //if (angleDegrees < 0)
            //    angleDegrees = 360 + angleDegrees;

            //return angleDegrees;


            var x = Math.IEEERemainder(angleDegrees, 360);
            if (x < 0)
                x += 360;
            return (float)x;

        }




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

        //private bool IsTouching(Vector3 p1, Vector3 p2)
        //{
        //    if (p1.X + p1.Width < p2.X)
        //        return false;
        //    if (p2.X + p2.Width < p1.X)
        //        return false;
        //    if (p1.Y + p1.Height < p2.Y)
        //        return false;
        //    if (p2.Y + p2.Height < p1.Y)
        //        return false;
        //    return true;
        //}

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

        public static float SnapAngle(float rotationToSnap)
        {
            if (rotationToSnap == 0)
                return 0.0f;

            var modRot = rotationToSnap % PiOver4;

            double finalRot;

            if (modRot < RoundedPiOver8)
            {
                if (modRot < -RoundedPiOver8)
                    finalRot = (rotationToSnap + -PiOver4 - modRot);
                else
                    finalRot = (rotationToSnap - modRot);
            }
            else
                finalRot = (rotationToSnap + PiOver4 - modRot);

            return (float)Math.Round(finalRot, 3);
        }


        public const double PiOver8 = (double)(Math.PI / 8.0);
        public static double RoundedPiOver8 = Math.Round(PiOver8, 6, MidpointRounding.AwayFromZero);
       

        public static double Barycentric(double value1, double value2, double value3, double amount1, double amount2)
        {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }

        public static double CatmullRom(double value1, double value2, double value3, double value4, double amount)
        {
            // Using formula from http://www.mvps.org/directx/articles/catmull/
            // Internally using doubles not to lose precission
            double amountSquared = amount * amount;
            double amountCubed = amountSquared * amount;
            return (double)(0.5 * (2.0 * value2 +
                (value3 - value1) * amount +
                (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));            
        }

        public static double Clamp(double value, double min, double max)
        {
            // First we check to see if we're greater than the max
            value = (value > max) ? max : value;

            // Then we check to see if we're less than the min.
            value = (value < min) ? min : value;

            // There's no check to see if min > max.
            return value;
        }

        public static float ToRadians(float degrees)
        {
            // This method uses double precission internally,
            // though it returns single double
            // Factor = pi / 180
            return (float)(degrees * 0.017453292519943295769236907684886);
        }

        public static float WrapAngle(float angle)
        {
            angle = (float)Math.IEEERemainder((float)angle, 6.2831854820251465);
            if (angle <= -3.14159274f)
            {
                angle += 6.28318548f;
            }
            else
            {
            if (angle > 3.14159274f)
            {
                angle -= 6.28318548f;
            }
            }
            return angle;
        }

        public static bool IsPowerOfTwo(int value)
        {
            return (value > 0) && ((value & (value - 1)) == 0);
        }


        public const float E = 2.718282f;
        public const float Log2E = 1.442695f;
        public const float Log10E = 0.4342945f;
        public const float Pi = 3.141593f;
        public const float TwoPi = 6.283185f;
        public const float PiOver2 = 1.570796f;
        public const float PiOver4 = 0.7853982f;

        public static float ToDegrees(float radians)
        {
            return radians * 57.29578f;
        }

        public static float Distance(float value1, float value2)
        {
            return Math.Abs(value1 - value2);
        }

        public static float Min(float value1, float value2)
        {
            return Math.Min(value1, value2);
        }

        public static float Max(float value1, float value2)
        {
            return Math.Max(value1, value2);
        }

        public static float Clamp(float value, float min, float max)
        {
            value = (double)value > (double)max ? max : value;
            value = (double)value < (double)min ? min : value;
            return value;
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static float Barycentric(float value1, float value2, float value3, float amount1, float amount2)
        {
            return (float)((double)value1 + (double)amount1 * ((double)value2 - (double)value1) + (double)amount2 * ((double)value3 - (double)value1));
        }

        public static float SmoothStep(float value1, float value2, float amount)
        {
            float num = Clamp(amount, 0.0f, 1f);
            return MathHelper.Lerp(value1, value2, (float)((double)num * (double)num * (3.0 - 2.0 * (double)num)));
        }

        public static float CatmullRom(float value1, float value2, float value3, float value4, float amount)
        {
            float num1 = amount * amount;
            float num2 = amount * num1;
            return (float)(0.5 * (2.0 * (double)value2 + (-(double)value1 + (double)value3) * (double)amount + (2.0 * (double)value1 - 5.0 * (double)value2 + 4.0 * (double)value3 - (double)value4) * (double)num1 + (-(double)value1 + 3.0 * (double)value2 - 3.0 * (double)value3 + (double)value4) * (double)num2));
        }

        public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            float num1 = amount;
            float num2 = num1 * num1;
            float num3 = num1 * num2;
            float num4 = (float)(2.0 * (double)num3 - 3.0 * (double)num2 + 1.0);
            float num5 = (float)(-2.0 * (double)num3 + 3.0 * (double)num2);
            float num6 = num3 - 2f * num2 + num1;
            float num7 = num3 - num2;
            return (float)((double)value1 * (double)num4 + (double)value2 * (double)num5 + (double)tangent1 * (double)num6 + (double)tangent2 * (double)num7);
        }

        public static float Flip(float value, float origin)
        {
            return origin - (value - origin);     
        }

        public static Vector2 GetNextLinePoint(int x, int y, int x2, int y2)
        {
            //http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }

            return new Vector2(x, y);
        }

    }

    public static class VectorExt
    {
        private const double DegToRad = Math.PI / 180;

        public static Vector2 Rotate(this Vector2 v, double degrees)
        {
            return v.RotateRadians(degrees * DegToRad);
        }

        public static Vector2 RotateRadians(this Vector2 v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector2((float)(ca * v.X - sa * v.Y), (float)(sa * v.X + ca * v.Y));
        }

        public static Vector3 Rotate(this Vector3 v, double degrees)
        {
            return v.RotateRadians(degrees * DegToRad);
        }

        public static Vector3 RotateRadians(this Vector3 v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector3((float)(ca * v.X - sa * v.Y), (float)(sa * v.X + ca * v.Y), v.Z);
        }

        public static Point Rotate(this Point p, Point origin, double degrees)
        {
            var theta = degrees * DegToRad;
            var x = Math.Cos(theta) * (p.X - origin.X) - Math.Sin(theta) * (p.Y - origin.Y) + origin.X;
            var y = Math.Sin(theta) * (p.X - origin.X) + Math.Cos(theta) * (p.Y - origin.Y) + origin.Y;
            return new Point(x,y);
        }

        public static Point RotateRadians(this Point v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Point((float)(ca * v.X - sa * v.Y), (float)(sa * v.X + ca * v.Y));
        }

        public static Point FlipX(this Point p, Point origin)
        {
            return new Point(origin.X - (p.X - origin.X),p.Y);
        }
        
        public static Point FlipY(this Point p, Point origin)
        {
            return new Point(p.X, origin.Y - (p.Y - origin.Y));
        }

        public static Point FlipBoth(this Point p, Point origin)
        {
            return new Point(origin.X - (p.X - origin.X), origin.Y - (p.Y - origin.Y));
        }




    }

}

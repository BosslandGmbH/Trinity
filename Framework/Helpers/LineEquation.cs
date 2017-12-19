using System;
using System.Windows;

namespace Trinity.Framework.Helpers
{
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
}
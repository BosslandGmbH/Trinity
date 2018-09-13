using System;
namespace Trinity.Framework.Objects
{
    public class Range
    {
        public double SpecialMax;
        public double SpecialMin;
        public double Max;
        public double Min;

        public float AbsMax
        {
            get
            {
                if (SpecialMax == 0)
                    return (float)Max;

                return (float)Math.Max(SpecialMax, Max);
            }
        }

        public float AbsMin
        {
            get
            {
                if (SpecialMin == 0)
                    return (float)Min;

                return (float)Math.Min(SpecialMin, Min);
            }
        }

        public float AbsStep => (float)GetStep(AbsMin, AbsMax);

        /// <summary>
        /// Friendly Increment amount between maximum and minimum
        /// </summary>
        public double GetStep(float min, float max)
        {
            var result = 1;
            var range = max - min;

            if (min >= 0 && max <= 1)
                return 0.05;

            if (min >= -1 && max <= 1)
                return 0.05;

            if (Math.Abs(min) < float.Epsilon && Math.Abs(max - 2) < float.Epsilon)
                return 0.05;

            if (range > 0 && range > 10)
                result = (int)Math.Round(Math.Ceiling((range / 10) * 100) / 100, 0);

            return result;
        }
    }
}

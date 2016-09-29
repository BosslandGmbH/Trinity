using System;

namespace Trinity.Framework.Objects
{
    public class ItemStatRange
    {
        public double AncientMax;
        public double AncientMin;
        public double Max;
        public double Min;

        public double AbsMax
        {
            get
            {
                if (AncientMax == 0) 
                    return Max;

                return Math.Max(AncientMax, Max);
            }
        }

        public double AbsMin
        {
            get
            {
                if (AncientMin == 0) 
                    return Min;

                return Math.Min(AncientMin, Min);
            }
        }

        public double AbsStep
        {
            get { return GetStep(AbsMin, AbsMax); }
        }

        /// <summary>
        /// Friendly Increment amount between maximum and minimum
        /// </summary>
        public int GetStep(double min, double max)
        {
            var result = 1;
            var range = max - min;

            if (range > 0 && range > 10)
                result = (int)Math.Round(Math.Ceiling((range / 10) * 100) / 100, 0);

            return result;
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel;

namespace Trinity.Framework.Objects.Enums
{
    public enum ComparisonOperator
    {
        [Description("=")]
        Equal,

        [Description("!=")]
        Unequal,

        [Description("<")]
        LessThan,

        [Description("<=")]
        LessThanOrEqualTo,

        [Description(">")]
        GreaterThan,

        [Description(">=")]
        GreaterThanOrEqualTo
    }

    public static class ComparisonOperatorExtensions
    {
        public static bool Compare<T>(this ComparisonOperator op, T A, T B)
        {
            switch (op)
            {
                case ComparisonOperator.Equal:
                    return ReferenceEquals(A, B);

                case ComparisonOperator.Unequal:
                    return !ReferenceEquals(A, B);

                case ComparisonOperator.LessThan:
                    return Comparer<T>.Default.Compare(A, B) < 0;

                case ComparisonOperator.GreaterThan:
                    return Comparer<T>.Default.Compare(A, B) > 0;

                case ComparisonOperator.GreaterThanOrEqualTo:
                    return Comparer<T>.Default.Compare(A, B) >= 0;

                case ComparisonOperator.LessThanOrEqualTo:
                    return Comparer<T>.Default.Compare(A, B) <= 0;
            }
            return false;
        }
    }
}
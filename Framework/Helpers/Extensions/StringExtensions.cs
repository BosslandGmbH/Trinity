using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Framework.Helpers
{
    public static class StringExtensions
    {
        public static string TrimEnd(this string input, string suffixToRemove, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            return input;
        }
    }
}


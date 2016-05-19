using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Trinity.Helpers
{
    public static class RegexExtensions
    {
        public class RegexGroupCollectionResult
        {
            public HashSet<RegexNumberResult> Numbers = new HashSet<RegexNumberResult>();
            public HashSet<RegexStringResult> Strings = new HashSet<RegexStringResult>();
            public HashSet<RegexResult> Values = new HashSet<RegexResult>();

            public ILookup<string, RegexNumberResult> NumberByGroupName;
            public ILookup<string, RegexStringResult> StringByGroupName;
            public ILookup<string, RegexResult> ValueByGroupName; 
           
            public Regex Regex { get; set; }
            public string Input { get; set; }
            public MatchCollection Matches { get; set; }            
        }

        public class RegexResult
        {
            public string GroupName;
            public int GroupIndex;
            public string StringValue;
            public double NumberValue;
        }

        public class RegexNumberResult
        {
            public string GroupName;
            public int GroupIndex;
            public double NumberValue;
        }

        public class RegexStringResult
        {
            public string GroupName;
            public int GroupIndex;
            public string StringValue;
        }
        
        /// <summary>
        /// Find matches in regex pattern containing groups
        /// </summary>
        public static RegexGroupCollectionResult GroupMatches(this Regex regex, string input, string pattern = "")
        {        
            if(!string.IsNullOrEmpty(pattern))
                regex = new Regex(pattern);

            var matches = regex.Matches(input);

            var result = new RegexGroupCollectionResult
            {
                Input = input,
                Regex = regex,
                Matches = matches
            };

            foreach (var m in matches)
            {
                var match = m as Match;

                if (match == null || !match.Success)
                    continue;

                var i = -1;

                foreach (var g in match.Groups)
                {
                    i++;

                    var group = g as Group;
                    if (group == null || i == 0 || group.Captures.Count == 0)
                        continue;

                    var capture = group.Captures.Cast<Capture>().LastOrDefault();
                    if (capture == null)
                        continue;

                    double n;
                    var isNumber = double.TryParse(capture.Value, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out n);
                    var groupName = regex.GroupNameFromNumber(i);

                    if (isNumber)
                    {
                        result.Numbers.Add(new RegexNumberResult
                        {
                            GroupName = groupName,
                            GroupIndex = i,
                            NumberValue = n
                        });
                    }
                    else
                    {
                        result.Strings.Add(new RegexStringResult
                        {
                            GroupName = groupName,
                            GroupIndex = i,
                            StringValue = capture.Value
                        });
                    }

                    result.Values.Add(new RegexResult
                    {
                        GroupName = groupName,
                        NumberValue = n,
                        GroupIndex = i,
                        StringValue = capture.Value
                    });
                }

               
            }

            result.ValueByGroupName = result.Values.ToLookup(o => o.GroupName, o => o);
            result.NumberByGroupName = result.Numbers.ToLookup(o => o.GroupName, o => o);
            result.StringByGroupName = result.Strings.ToLookup(o => o.GroupName, o => o);

            return result;
        }
    }
}

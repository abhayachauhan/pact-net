using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers
{
    public class RegExMatcher : Matcher
    {
        public string RegEx { get; private set; }

        public RegExMatcher(string regex)
        {
            RegEx = regex;
        }

        public override bool IsMatch(JToken expected, JToken actual)
        {
            var act = actual as JValue;
            return act != null && Regex.IsMatch(act.Value.ToString(), RegEx);
        }
    }
}
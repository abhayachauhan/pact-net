using System;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers
{
    public class MinMatcher : Matcher
    {
        public int Minimum { get; private set; }

        public MinMatcher(int minimum)
        {
            Minimum = minimum;
        }

        public override bool IsMatch(JToken expected, JToken actual)
        {
            if (actual is JArray)
            {
                return (actual as JArray).Count == Minimum;
            }
            return false;
        }
    }
}
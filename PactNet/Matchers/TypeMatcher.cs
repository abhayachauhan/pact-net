using Newtonsoft.Json.Linq;

namespace PactNet.Matchers
{
    public class TypeMatcher : Matcher
    {
        public override bool IsMatch(JToken expected, JToken actual)
        {
            return ((JProperty)expected).Value.Type == ((JProperty)actual).Value.Type;
        }
    }
}
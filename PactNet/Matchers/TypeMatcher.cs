using Newtonsoft.Json.Linq;

namespace PactNet.Matchers
{
    public class TypeMatcher : Matcher
    {
        public override bool IsMatch(JToken expected, JToken actual)
        {
            var expectedType = TypeOfData(expected);
            var actualType = TypeOfData(actual);
            return expectedType == actualType;
        }

        private JTokenType TypeOfData(JToken token)
        {
            if (token is JProperty)
                return ((JProperty)token).Value.Type;
            return token.Type;
        }
    }
}
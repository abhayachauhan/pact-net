using System.Linq;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers
{
    public abstract class Matcher
    {
        public abstract bool IsMatch(JToken expected, JToken actual);

        public static bool TryParse(JToken json, out Matcher matcher)
        {
            if (json.OfType<JProperty>().Any(property => (property).Name == "regex"))
            {
                string regex = ((JProperty)json.First).Value.ToString();
                matcher = new RegExMatcher(regex);
                return true;
            }

            if (json.OfType<JProperty>().Any(property =>
                (property).Name == "match" && property.Value.ToString() == "type"))
            {
                matcher = new TypeMatcher();
                return true;
            }

            matcher = null;
            return false;
        }
    }
}
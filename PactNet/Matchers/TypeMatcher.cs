using System.Linq;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers
{
	public class TypeMatcher : IMatcher
	{
		public bool IsMatch(JToken expected, JToken actual)
		{
			return ((JProperty)expected).Value.Type == ((JProperty)actual).Value.Type;
		}

		public static bool TryParse(JToken json, out TypeMatcher typeMatcher)
		{
			typeMatcher = null;
			if (!json.OfType<JProperty>().Any(property =>
				(property).Name == "match" && property.Value.ToString() == "type")) return false;

			typeMatcher = new TypeMatcher();
			return true;
		}
	}
}
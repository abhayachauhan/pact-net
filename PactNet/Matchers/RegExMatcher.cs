using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers
{
	public class RegExMatcher : IMatcher
	{
		public string RegEx { get; private set; }

		public RegExMatcher(string regex)
		{
			RegEx = regex;
		}

		public bool IsMatch(JToken expected, JToken actual)
		{
			var act = actual as JProperty;
			return act != null && Regex.IsMatch(act.Value.ToString(), RegEx);
		}

		public static bool TryParse(JToken json, out RegExMatcher regExMatcher)
		{
			regExMatcher = null;
			if (json.OfType<JProperty>().All(property => (property).Name != "regex")) return false;

			string regex = ((JProperty)json.First).Value.ToString();
			regExMatcher = new RegExMatcher(regex);
			return true;
		}
	}
}
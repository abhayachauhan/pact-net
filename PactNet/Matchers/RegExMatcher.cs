using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PactNet.Matchers;

public class RegExMatcher : Matcher
{
	public string RegEx { get; set; }
	public dynamic Example { get; set; }

	public RegExMatcher(object example, string regEx)
	{
		Example = example;
		RegEx = regEx;
	}

	public override bool Match(object input)
	{
		return Regex.IsMatch(input.ToString(), RegEx);
	}

	[JsonProperty(PropertyName = "$type")]
	public string Name
	{
		get { return GetType().FullName; }
	}

	public static string Type
	{
		get { return typeof(RegExMatcher).FullName; }
	}

	public override dynamic ResponseMatchingRule
	{
		get
		{
			return new
			{
				regex = RegEx
			};
		}
	}
}
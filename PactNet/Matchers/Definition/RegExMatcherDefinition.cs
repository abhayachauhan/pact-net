using Newtonsoft.Json;

namespace PactNet.Matchers.Definition
{
	public class RegExMatcherDefinition : DefineMatcher
	{
		[JsonProperty("regex")]
		public string RegEx { get; set; }

		public RegExMatcherDefinition(object example, string regEx)
			: base(example)
		{
			RegEx = regEx;
		}

		public new static string Type
		{
			get { return typeof(RegExMatcherDefinition).FullName; }
		}

		[JsonIgnore]
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
}
using Newtonsoft.Json;

namespace PactNet.Matchers.Definition
{
	public abstract class DefineMatcher
	{
		protected DefineMatcher(dynamic example)
		{
			Example = example;
		}

		public dynamic Example { get; set; }

		[JsonProperty("$type")]
		public string Name
		{
			get { return GetType().FullName; }
		}

		public abstract dynamic ResponseMatchingRule { get; }

		public static TypeMatcherDefinition TypeEg(object example)
		{
			return new TypeMatcherDefinition(example);
		}

		public static RegExMatcherDefinition RegExEg(object example, string regEx)
		{
			return new RegExMatcherDefinition(example, regEx);
		}
	}
}
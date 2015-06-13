using Newtonsoft.Json;

namespace PactNet.Matchers.Definition
{
	public class TypeMatcherDefinition : DefineMatcher
	{
		public TypeMatcherDefinition(object example)
			: base(example)
		{
		}

		[JsonIgnore]
		public override dynamic ResponseMatchingRule
		{
			get
			{
				return new
				{
					match = "type"
				};
			}
		}

		public new static string Type
		{
			get { return typeof(TypeMatcherDefinition).FullName; }
		}
	}
}
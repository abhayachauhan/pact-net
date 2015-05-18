namespace PactNet.Matchers
{
	public abstract class Matcher
	{
		public dynamic Example { get; set; }

		public static TypeMatcher TypeEg(object example, TypeMatcher.DataType type)
		{
			return new TypeMatcher(example, type);
		}

		public static RegExMatcher RegExEg(object example, string regEx)
		{
			return new RegExMatcher(example, regEx);
		}

		public abstract bool Match(object input);
	}
}
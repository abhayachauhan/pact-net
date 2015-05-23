using System.CodeDom;
using System.ComponentModel;
using Newtonsoft.Json;

namespace PactNet.Matchers
{
	public class Matcher
	{
		public Matcher(object example)
		{
			Example = example;
		}

		public static string Type
		{
			get { return typeof(Matcher).FullName; }
		}

		[JsonProperty("$type")]
		public string Name
		{
			get { return GetType().FullName; }
		}

		public dynamic Example { get; set; }

		//public static TypeMatcher TypeEg(object example)
		//{
		//	return new TypeMatcher(example);
		//}

		//public static RegExMatcher RegExEg(object example, string regEx)
		//{
		//	return new RegExMatcher(example, regEx);
		//}

		//public abstract dynamic ResponseMatchingRule { get; }
	}
}
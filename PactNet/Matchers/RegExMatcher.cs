//using System.Text.RegularExpressions;
//using Newtonsoft.Json;

//namespace PactNet.Matchers
//{
//	public class RegExMatcher : Matcher
//	{
//		[JsonProperty("regex")]
//		public string RegEx { get; set; }

//		public RegExMatcher(object example, string regEx)
//		{
//			Example = example;
//			RegEx = regEx;
//		}

//		public bool IsMatch(object input)
//		{
//			return Regex.IsMatch(input.ToString(), RegEx);
//		}

//		[JsonProperty(PropertyName = "$type")]
//		public string Name
//		{
//			get { return GetType().FullName; }
//		}

//		public static string Type
//		{
//			get { return typeof(RegExMatcher).FullName; }
//		}

//		//public override dynamic ResponseMatchingRule
//		//{
//		//	get
//		//	{
//		//		return new
//		//		{
//		//			regex = RegEx
//		//		};
//		//	}
//		//}
//	}
//}
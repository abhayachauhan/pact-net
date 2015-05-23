//using System;
//using Newtonsoft.Json;

//namespace PactNet.Matchers
//{
//	public class TypeMatcher : Matcher
//	{
//		[JsonProperty("match")]
//		public readonly string MATCH = "type";

//		public TypeMatcher(object example)
//		{
//			Example = example;
//		}

//		public bool IsMatch(object input)
//		{
//			Type typeOfExample = Example.GetType();

//			if (input.GetType() == typeOfExample)
//				return true;
//			return false;
//		}

//		//public override dynamic ResponseMatchingRule
//		//{
//		//	get
//		//	{
//		//		return new
//		//		{
//		//			match = "type"
//		//		};
//		//	}
//		//}

//		[JsonProperty(PropertyName = "$type")]
//		public string Name
//		{
//			get { return GetType().FullName; }
//		}

//		public static string Type
//		{
//			get { return typeof(TypeMatcher).FullName; }
//		}
//	}
//}
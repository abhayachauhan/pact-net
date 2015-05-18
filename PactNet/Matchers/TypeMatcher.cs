using System;
using Newtonsoft.Json;

namespace PactNet.Matchers
{
	public class TypeMatcher : Matcher
	{
		public enum DataType
		{
			String,
			Boolean,
			Number,
			Object,
			Array
		};

		public DataType TypeOfData { get; set; }

		public TypeMatcher(object example, DataType typeOfData)
		{
			Example = example;
			TypeOfData = typeOfData;
		}

		public override bool Match(object input)
		{
			switch (TypeOfData)
			{
				case DataType.Array:
					return input is Array;
				case DataType.Boolean:
					return input is bool;
				case DataType.Number:
					return input is int;
				case DataType.Object:
					return false;
				case DataType.String:
					return input is string;
				default:
					return false;
			}
		}

		[JsonProperty(PropertyName = "$type")]
		public string Name
		{
			get { return GetType().FullName; }
		}

		public static string Type
		{
			get { return typeof(TypeMatcher).FullName; }
		}
	}
}
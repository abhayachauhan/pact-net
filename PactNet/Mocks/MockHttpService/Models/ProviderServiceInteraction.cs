using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Matchers;
using PactNet.Models;

namespace PactNet.Mocks.MockHttpService.Models
{
	public class ProviderServiceInteraction : Interaction
	{
		private ProviderServiceResponse _response;

		[JsonProperty(PropertyName = "request")]
		public ProviderServiceRequest Request { get; set; }

		[JsonProperty(PropertyName = "response")]
		public ProviderServiceResponse Response
		{
			get { return _response; }
			set { _response = CleanOutMatchers(value); }
		}

		[JsonProperty(PropertyName = "responseMatchingRules")]
		public PactProviderResponseMatchingRules MatchingRules { get; private set; }

		private ProviderServiceResponse CleanOutMatchers(ProviderServiceResponse value)
		{
			if (value.Body is string || value.Body is int || value.Body is bool)
			{
			}
			else if (value.Body is Matcher)
			{
				var matcher = (Matcher)value.Body;

				MatchingRules = MatchingRules ?? new PactProviderResponseMatchingRules();

				MatchingRules.Body.Add("$.", matcher);

				value.Body = matcher.Example;
			}
			else if (value.Body != null)
			{
				JToken body = JToken.FromObject(value.Body);
				JToken body2 = body.DeepClone();
				ParseJson(body, body2);

				//foreach (var item in body) //Array
				//{
				//	foreach (var item2 in item) //Body Item Object
				//	{
				//		foreach (var item3 in item2) //Properties
				//		{
				//			foreach (var item5 in item3)
				//			{
				//				if (item5 is JProperty)
				//				{
				//					JProperty item4 = item5 as JProperty;
				//					if (item4.Name.Equals("$type") &&
				//						item4.Value.ToString().Equals(RegExMatcher.Type)) //."PactNet, PactNet.Models.RegexMatcher"))
				//					{
				//						var matcher = (RegExMatcher)JsonConvert.DeserializeObject(item3.ToString(),
				//							typeof(RegExMatcher));

				//						MatchingRules = MatchingRules ?? new PactProviderResponseMatchingRules();
				//						MatchingRules.Body = MatchingRules.Body ?? new Dictionary<string, dynamic>();

				//						var index = item3.Path.IndexOf('.');
				//						var key = item3.Path.Substring(index, item3.Path.Length - index);

				//						MatchingRules.Body.Add("$" + item3.Path, matcher);
				//						//Matcher.RegExEg(matchnew Matcher { Regex = matcher.Regex }));

				//						body2.SelectToken(item3.Path).Replace(matcher.Example); //TODO: Maybe we dont need another copy of the object
				//					}
				//					else if (item4.Name.Equals("$type") &&
				//							 item4.Value.ToString().Equals(TypeMatcher.Type)) //."PactNet, PactNet.Models.RegexMatcher"))
				//					{
				//						var matcher = (TypeMatcher)JsonConvert.DeserializeObject(item3.ToString(),
				//							typeof(TypeMatcher));

				//						MatchingRules = MatchingRules ?? new PactProviderResponseMatchingRules();
				//						MatchingRules.Body = MatchingRules.Body ?? new Dictionary<string, dynamic>();

				//						var index = item3.Path.IndexOf('.');
				//						var key = item3.Path.Substring(index, item3.Path.Length - index);

				//						MatchingRules.Body.Add("$" + item3.Path, matcher); //new Matcher { Regex = matcher.Regex });

				//						body2.SelectToken(item3.Path).Replace(matcher.Example); //TODO: Maybe we dont need another copy of the object
				//					}
				//				}
				//			}
				//		}
				//	}
				//}

				if (MatchingRules != null && MatchingRules.Body != null && MatchingRules.Body.Any())
				{
					value.Body = body2;
				}
			}

			return value;
		}

		private void ParseJson(JToken json, JToken body2)
		{
			if (json is JObject)
			{
				foreach (var property in json)
				{
					ParseJson(property, body2);
				}
			}
			else if (json is JArray)
			{
				foreach (var property in json)
				{
					ParseJson(property, body2);
				}
			}
			else if (json is JProperty)
			{
				JProperty item4 = (JProperty)json;
				if (item4.Name.Equals("$type") &&
					item4.Value.ToString().Equals(RegExMatcher.Type)) //."PactNet, PactNet.Models.RegexMatcher"))
				{
					var matcher = (RegExMatcher)JsonConvert.DeserializeObject(item4.Parent.ToString(),
						typeof(RegExMatcher));

					MatchingRules = MatchingRules ?? new PactProviderResponseMatchingRules();
					MatchingRules.Body = MatchingRules.Body ?? new Dictionary<string, dynamic>();

					var index = item4.Parent.Path.IndexOf('.');
					var key = item4.Parent.Path.Substring(index, item4.Parent.Path.Length - index);

					MatchingRules.Body.Add("$" + item4.Parent.Path, matcher);
					//Matcher.RegExEg(matchnew Matcher { Regex = matcher.Regex }));

					body2.SelectToken(item4.Parent.Path).Replace(matcher.Example); //TODO: Maybe we dont need another copy of the object
				}
				else if (item4.Name.Equals("$type") &&
						 item4.Value.ToString().Equals(TypeMatcher.Type)) //."PactNet, PactNet.Models.RegexMatcher"))
				{
					var matcher = (TypeMatcher)JsonConvert.DeserializeObject(item4.Parent.ToString(),
						typeof(TypeMatcher));

					MatchingRules = MatchingRules ?? new PactProviderResponseMatchingRules();
					MatchingRules.Body = MatchingRules.Body ?? new Dictionary<string, dynamic>();

					var index = item4.Parent.Path.IndexOf('.');
					var key = item4.Parent.Path.Substring(index, item4.Parent.Path.Length - index);

					MatchingRules.Body.Add("$" + item4.Parent.Path, matcher); //new Matcher { Regex = matcher.Regex });

					body2.SelectToken(item4.Parent.Path).Replace(matcher.Example); //TODO: Maybe we dont need another copy of the object
				}
			}

		}
	}
}
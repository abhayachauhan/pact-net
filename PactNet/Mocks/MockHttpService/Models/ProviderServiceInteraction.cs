using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Matchers.Definition;
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
			set
			{
				_response = CleanOutBodyMatchers(value);
			}
		}

		private ProviderServiceResponse CleanOutBodyMatchers(ProviderServiceResponse value)
		{
			if (value.Body is string || value.Body is int || value.Body is bool)
			{
			}
			else if (value.Body is DefineMatcher)
			{
				var matcher = (DefineMatcher)value.Body;

				value.ResponseMatchingRules = value.ResponseMatchingRules ?? new Dictionary<string, dynamic>();

				value.ResponseMatchingRules.Add("$.body", matcher);

				value.Body = matcher.Example;
			}
			else if (value.Body != null)
			{
				JToken body = JToken.FromObject(value.Body);
				var bodyClone = body.DeepClone();

				ParseJToken(body, bodyClone, value);

				if (value.ResponseMatchingRules != null && value.ResponseMatchingRules.Any())
				{
					value.Body = bodyClone;
				}
			}

			return value;
		}

		private void ParseJToken(JToken json, JToken body, ProviderServiceResponse response)
		{
			if (json is JValue) return;

			foreach (var property in json)
			{
				ParseJToken(property, body, response);
			}

			if (json is JProperty)
			{
				ParseMatchers(json, body, response);
			}
		}

		private void ParseMatchers(JToken json, JToken body, ProviderServiceResponse response)
		{
			JProperty item4 = (JProperty)json;
			if (item4.Name.Equals("$type") &&
				item4.Value.ToString().Equals(TypeMatcherDefinition.Type))
			{
				var matcher = (TypeMatcherDefinition)JsonConvert.DeserializeObject(item4.Parent.ToString(),
					typeof(TypeMatcherDefinition));

				response.ResponseMatchingRules = response.ResponseMatchingRules ?? new Dictionary<string, dynamic>();

				response.ResponseMatchingRules.Add("$." + item4.Parent.Path, matcher.ResponseMatchingRule);

				body.SelectToken(item4.Parent.Path).Replace(matcher.Example);
			}
			else if (item4.Name.Equals("$type") &&
					 item4.Value.ToString().Equals(RegExMatcherDefinition.Type))
			{
				var matcher = (RegExMatcherDefinition)JsonConvert.DeserializeObject(item4.Parent.ToString(),
					typeof(RegExMatcherDefinition));

				response.ResponseMatchingRules = response.ResponseMatchingRules ?? new Dictionary<string, dynamic>();

				response.ResponseMatchingRules.Add("$." + item4.Parent.Path, matcher.ResponseMatchingRule);

				body.SelectToken(item4.Parent.Path).Replace(matcher.Example);
			}
		}
	}
}
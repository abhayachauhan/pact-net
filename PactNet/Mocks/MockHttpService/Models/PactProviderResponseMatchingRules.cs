using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Models
{
	public class PactProviderResponseMatchingRules
	{
		[JsonProperty(PropertyName = "body")]
		public IDictionary<string, dynamic> Body { get; set; }

		public PactProviderResponseMatchingRules()
		{
			Body = new Dictionary<string, dynamic>();
		}
	}
}

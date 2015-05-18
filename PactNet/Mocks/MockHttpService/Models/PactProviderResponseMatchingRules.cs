using System.Collections.Concurrent;
using System.Collections.Generic;
using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Models
{
	public class PactProviderResponseMatchingRules
	{
		public IDictionary<string, dynamic> Body { get; set; }

		public PactProviderResponseMatchingRules()
		{
			Body = new ConcurrentDictionary<string, dynamic>();
		}
	}
}

using System.Collections.Generic;
using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
	internal interface IHttpBodyComparer
	{
		ComparisonResult Compare(dynamic expected, dynamic actual, IDictionary<string, dynamic> matchingRules, bool useStrict = false);
	}
}
using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
	internal interface IHttpBodyComparer
	{
		ComparisonResult Compare(dynamic expected, dynamic actual, PactProviderResponseMatchingRules matchingRules, bool useStrict = false);
	}
}
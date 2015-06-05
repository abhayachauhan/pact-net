using System.Collections.Generic;
using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
	internal interface IProviderServiceResponseComparer// : IComparer<ProviderServiceResponse>
	{
		ComparisonResult Compare(ProviderServiceResponse expected, ProviderServiceResponse actual,
			IDictionary<string, dynamic> matchingRules);
	}
}
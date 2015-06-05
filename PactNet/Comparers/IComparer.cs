using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Comparers
{
    internal interface IComparer<in T>
    {
        ComparisonResult Compare(T expected, T actual);
    }
}

using System.Collections.Generic;

namespace PactNet.Comparers
{
    internal interface IComparer<in T>
    {
        ComparisonResult Compare(T expected, T actual, IDictionary<string, dynamic> matchers);
    }

    internal interface IBasicComparer<in T>
    {
        ComparisonResult Compare(T expected, T actual);
    }
}

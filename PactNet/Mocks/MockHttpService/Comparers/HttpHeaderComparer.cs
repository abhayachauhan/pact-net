using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;
using PactNet.Configuration.Json;
using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpHeaderComparer : IHttpHeaderComparer
    {
        public ComparisonResult Compare(IDictionary<string, string> expected, IDictionary<string, string> actual, IDictionary<string, dynamic> matchers)
        {
            var result = new ComparisonResult("includes headers");

            if (actual == null)
            {
                result.RecordFailure(new ErrorMessageComparisonFailure("Actual Headers are null"));
                return result;
            }

            actual = MakeDictionaryCaseInsensitive<string>(actual);

            foreach (var header in expected)
            {
                var headerResult = new ComparisonResult("'{0}' with value {1}", header.Key, header.Value);

                string actualValue;

                if (actual.TryGetValue(header.Key, out actualValue))
                {
                    var expectedValue = header.Value;

                    dynamic matcher = GetMatcherForKey(matchers, header.Key);
                    if (matcher != null)
                    {
                        ExecuteMatcher(matcher, expectedValue, actualValue, result);
                    }
                    else
                    {
                        var actualValueSplit = actualValue.Split(new[] { ',', ';' });
                        if (actualValueSplit.Length == 1)
                        {
                            if (!header.Value.Equals(actualValue))
                            {
                                headerResult.RecordFailure(new DiffComparisonFailure(expectedValue, actualValue));
                            }
                        }
                        else
                        {
                            var expectedValueSplit = expectedValue.Split(new[] { ',', ';' });
                            var expectedValueSplitJoined = String.Join(",", expectedValueSplit.Select(x => x.Trim()));
                            var actualValueSplitJoined = String.Join(",", actualValueSplit.Select(x => x.Trim()));

                            if (!expectedValueSplitJoined.Equals(actualValueSplitJoined))
                            {
                                headerResult.RecordFailure(new DiffComparisonFailure(expectedValue, actualValue));
                            }
                        }
                    }
                }
                else
                {
                    headerResult.RecordFailure(new ErrorMessageComparisonFailure(String.Format("Header with key '{0}', does not exist in actual", header.Key)));
                }

                result.AddChildResult(headerResult);
            }

            return result;
        }

        private void ExecuteMatcher(dynamic matcherDefinition, string expected, string actual, ComparisonResult headerResult)
        {
            JToken matchingRule = JToken.FromObject(matcherDefinition, JsonConfig.ComparisonSerializerSettings);
            Matcher matcher;
            if (!Matcher.TryParse(matchingRule, out matcher))
                throw new Exception("Invalid Matcher: " + matchingRule);

            if (!matcher.IsMatch(expected, actual))
            {
                headerResult.RecordFailure(new DiffComparisonFailure(expected, actual));
            }
        }

        private dynamic GetMatcherForKey(IDictionary<string, dynamic> matchers, string key)
        {
            var matcherKey = "$.headers." + key;
            if ((matchers != null) && (matchers.ContainsKey(matcherKey)))
                return matchers[matcherKey];
            return null;
        }

        private IDictionary<string, string> MakeDictionaryCaseInsensitive<T>(IDictionary<string, string> from)
        {
            return new Dictionary<string, string>(from, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
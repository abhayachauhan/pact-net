using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;
using PactNet.Configuration.Json;
using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpBodyComparer : IHttpBodyComparer
    {
        private List<string> _matchedPaths = new List<string>();

        //TODO: Remove boolean and add "matching" functionality
        public ComparisonResult Compare(dynamic expected, dynamic actual, IDictionary<string, dynamic> matchingRules, bool useStrict = false)
        {
            var result = new ComparisonResult("has a matching body");

            if (expected == null)
            {
                return result;
            }

            if (expected != null && actual == null)
            {
                result.RecordFailure(new ErrorMessageComparisonFailure("Actual Body is null"));
                return result;
            }

            var expectedToken = JToken.FromObject(expected, JsonConfig.ComparisonSerializerSettings);
            var actualToken = JToken.FromObject(actual, JsonConfig.ComparisonSerializerSettings);

            if ((useStrict) && ((matchingRules == null) || (matchingRules.Count == 0)))
            {
                if (!JToken.DeepEquals(expectedToken, actualToken))
                {
                    result.RecordFailure(new DiffComparisonFailure(expectedToken, actualToken));
                }
                return result;
            }

            AssertMatchersMatch(expectedToken, actualToken, matchingRules, result);
            AssertPropertyValuesMatch(expectedToken, actualToken, result);

            return result;
        }

        private void AssertMatchersMatch(JToken expected, JToken actual, IDictionary<string, object> matchingRules, ComparisonResult result)
        {
            if (matchingRules == null || !matchingRules.Any()) return;

            foreach (KeyValuePair<string, dynamic> keyValuePair in matchingRules)
            {
                JToken matchingRule = JToken.FromObject(keyValuePair.Value);
                Matcher matcher;
                if (!Matcher.TryParse(matchingRule, out matcher))
                    throw new Exception("Invalid Matcher: " + matchingRule);

                JToken newExpected = expected.DeepClone();

                if (keyValuePair.Key.Contains("[*]"))
                {
                    newExpected = MultiplyExpected(newExpected, actual, keyValuePair.Key);
                }

                IEnumerable<JToken> actualTokens = actual.SelectTokens(ConvertPactPathToJsonPath(keyValuePair.Key));
                IEnumerable<JToken> expectedTokens = newExpected.SelectTokens(ConvertPactPathToJsonPath(keyValuePair.Key));

                // If matching on root level array
                if (keyValuePair.Key.EndsWith("[*]"))
                {
                    var trimmed = keyValuePair.Key.Replace("[*]", "");
                    _matchedPaths.Add(trimmed.Replace("$.body", ""));
                }

                foreach (var expectedToken in expectedTokens)
                {
                    var matchedActualTokens = actualTokens.Where(t => t.Path == expectedToken.Path);
                    foreach (var actualToken in matchedActualTokens)
                    {
                        _matchedPaths.Add(expectedToken.Path);

                        var isMatch = matcher.IsMatch(expectedToken, actualToken);
                        if (isMatch) continue;
                        result.RecordFailure(new DiffComparisonFailure(expected.Root, actual.Root));
                    }
                }
            }
        }

        private JToken MultiplyExpected(JToken expected, JToken actual, string matchingRule)
        {
            var jsonPath = ConvertPactPathToJsonPath(matchingRule);
            var preWildcardPath = jsonPath.Substring(0, jsonPath.IndexOf("[*]", StringComparison.Ordinal) + 3);

            _matchedPaths.Add(jsonPath.Substring(0, jsonPath.IndexOf("[*]", StringComparison.Ordinal)));

            var numberOfDuplications = actual.SelectTokens(preWildcardPath).Count();
            var tokenToDuplicate = expected.SelectToken(preWildcardPath);

            for (int cloneNumber = 0; cloneNumber < numberOfDuplications - 1; cloneNumber++)
            {
                tokenToDuplicate.AddAfterSelf(tokenToDuplicate.DeepClone());
            }
            return expected;
        }

        private bool AssertPropertyValuesMatch(JToken expected, JToken actual, ComparisonResult result)
        {
            if (_matchedPaths.Contains(expected.Path))
                return true;

            switch (expected.Type)
            {
                case JTokenType.Array:
                    {
                        if (expected.Count() != actual.Count())
                        {
                            result.RecordFailure(new DiffComparisonFailure(expected.Root, actual.Root));
                            return false;
                        }

                        for (var i = 0; i < expected.Count(); i++)
                        {
                            if (actual.Count() > i)
                            {
                                var isMatch = AssertPropertyValuesMatch(expected[i], actual[i], result);
                                if (!isMatch)
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case JTokenType.Object:
                    {
                        foreach (JProperty item1 in expected)
                        {
                            var item2 = actual.Cast<JProperty>().SingleOrDefault(x => x.Name == item1.Name);

                            if (item2 != null)
                            {
                                var isMatch = AssertPropertyValuesMatch(item1, item2, result);
                                if (!isMatch)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                result.RecordFailure(new DiffComparisonFailure(expected.Root, actual.Root));
                                return false;
                            }
                        }
                        break;
                    }
                case JTokenType.Property:
                    {
                        var httpBody2Item = actual.SingleOrDefault();
                        var httpBody1Item = expected.SingleOrDefault();

                        if (httpBody2Item == null && httpBody1Item == null)
                        {
                            return true;
                        }

                        if (httpBody2Item != null && httpBody1Item != null)
                        {
                            AssertPropertyValuesMatch(httpBody1Item, httpBody2Item, result);
                        }
                        else
                        {
                            result.RecordFailure(new DiffComparisonFailure(expected.Root, actual.Root));
                            return false;
                        }
                        break;
                    }
                case JTokenType.Integer:
                case JTokenType.String:
                    {
                        if (!expected.Equals(actual))
                        {
                            result.RecordFailure(new DiffComparisonFailure(expected.Root, actual.Root));
                            return false;
                        }
                        break;
                    }
                default:
                    {
                        if (!JToken.DeepEquals(expected, actual))
                        {
                            result.RecordFailure(new DiffComparisonFailure(expected.Root, actual.Root));
                            return false;
                        }
                        break;
                    }
            }

            return true;
        }

        private string ConvertPactPathToJsonPath(string path)
        {
            var removeBody = path.Replace("$.body", "");

            if (removeBody.StartsWith("."))
                return removeBody.Substring(1, removeBody.Length - 1);
            return removeBody;
        }
    }
}
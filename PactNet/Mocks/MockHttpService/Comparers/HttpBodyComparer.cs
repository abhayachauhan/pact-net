using System;
using System.Collections.Generic;
using System.Linq;
using Nancy.ViewEngines;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;
using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpBodyComparer : IHttpBodyComparer
    {
        private List<string> _matcherPaths = new List<string>();

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

            var expectedToken = JToken.FromObject(expected);
            var actualToken = JToken.FromObject(actual);

            if (useStrict)
            {
                if (!JToken.DeepEquals(expectedToken, actualToken))
                {
                    result.RecordFailure(new DiffComparisonFailure(expectedToken, actualToken));
                }
                return result;
            }

            AssertMatchersMatch(expectedToken, actualToken, matchingRules, result);
            AssertPropertyValuesMatch(expectedToken, actualToken, matchingRules, result);

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

                JToken newExpected = expected;

                if (keyValuePair.Key.Contains("[*]"))
                {
                    newExpected = MultiplyExpected(expected, actual, keyValuePair.Key);
                }

                IEnumerable<JToken> actualTokens = actual.SelectTokens(ConvertPactPathToJsonPath(keyValuePair.Key));
                IEnumerable<JToken> expectedTokens;
                expectedTokens = newExpected.SelectTokens(ConvertPactPathToJsonPath(keyValuePair.Key));

                // If matching on root level array
                if (keyValuePair.Key.EndsWith("[*]"))
                {
                    var trimmed = keyValuePair.Key.Replace("[*]", "");
                    _matcherPaths.Add(trimmed.Replace("$.body", ""));
                }

                foreach (var expectedToken in expectedTokens)
                {
                    var matchedActualTokens = actualTokens.Where(t => t.Path == expectedToken.Path);
                    foreach (var actualToken in matchedActualTokens)
                    {
                        _matcherPaths.Add(expectedToken.Path);

                        var isMatch = matcher.IsMatch(expectedToken, actualToken);
                        if (isMatch) continue;
                        result.RecordFailure(new DiffComparisonFailure(expected.Root, actual.Root));
                    }
                }
                //foreach (var token in tokens)
                //{
                //    _matcherPaths.Add(token.Path);

                //    var isMatch = matcher.IsMatch(expectedTokens, token);
                //    if (isMatch) continue;
                //    result.RecordFailure(new DiffComparisonFailure(expected.Root, actual.Root));
                //}
            }
        }

        private JToken MultiplyExpected(JToken expected, JToken actual, string matchingRule)
        {
            var expectedClone = expected.DeepClone();
            var jsonPath = ConvertPactPathToJsonPath(matchingRule);
            var preWildcardPath = jsonPath.Substring(0, jsonPath.IndexOf("[*]", StringComparison.Ordinal) + 3);

            _matcherPaths.Add(jsonPath.Substring(0, jsonPath.IndexOf("[*]", StringComparison.Ordinal)));

            var numberOfDuplications = actual.SelectTokens(preWildcardPath).Count();
            var tokenToDuplicate = expectedClone.SelectToken(preWildcardPath);

            for (int cloneNumber = 0; cloneNumber < numberOfDuplications - 1; cloneNumber++)
            {
                tokenToDuplicate.AddAfterSelf(tokenToDuplicate.DeepClone());
            }
            return expectedClone;
        }

        private bool AssertPropertyValuesMatch(JToken expected, JToken actual, IDictionary<string, dynamic> matchingRules, ComparisonResult result)
        {
            if (_matcherPaths.Contains(expected.Path))
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
                                var isMatch = AssertPropertyValuesMatch(expected[i], actual[i], matchingRules, result);
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
                                var isMatch = AssertPropertyValuesMatch(item1, item2, matchingRules, result);
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
                            AssertPropertyValuesMatch(httpBody1Item, httpBody2Item, matchingRules, result);
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

        //private string BuildMatchingRulePath(string path)
        //{
        //    const string prepend = "$.body";
        //    if (path == string.Empty)
        //        return prepend;
        //    // Refactor this to find a better way to detect if path is an array
        //    return string.Format(path.StartsWith("[") ? "{0}{1}" : "{0}.{1}", prepend, path);
        //}

        private string ConvertPactPathToJsonPath(string path)
        {
            return path.Replace("$.body", "");
        }
    }
}
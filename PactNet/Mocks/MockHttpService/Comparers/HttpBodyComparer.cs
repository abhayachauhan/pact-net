﻿using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpBodyComparer : IHttpBodyComparer
    {
        //TODO: Remove boolean and add "matching" functionality
        public ComparisonResult Compare(dynamic expected, dynamic actual, bool useStrict = false)
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

            //TODO: Maybe look at changing these to JToken.FromObject(...)
            string expectedJson = JsonConvert.SerializeObject(expected);
            string actualJson = JsonConvert.SerializeObject(actual);
            var expectedToken = JsonConvert.DeserializeObject<JToken>(expectedJson);
            var actualToken = JsonConvert.DeserializeObject<JToken>(actualJson);

            if (useStrict)
            {
                if (!JToken.DeepEquals(expectedToken, actualToken))
                {
                    result.RecordFailure(new DiffComparisonFailure(expectedToken, actualToken));
                }
                return result;
            }

            AssertPropertyValuesMatch(expectedToken, actualToken, result);

            return result;
        }

        private bool AssertPropertyValuesMatch(JToken expected, JToken actual, ComparisonResult result)
        {
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
    }
}
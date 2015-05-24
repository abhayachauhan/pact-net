using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
	internal class HttpBodyComparer : IHttpBodyComparer
	{
		//TODO: Remove boolean and add "matching" functionality
		public ComparisonResult Compare(dynamic expected, dynamic actual, PactProviderResponseMatchingRules matchingRules, bool useStrict = false)
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

			AssertPropertyValuesMatch(expectedToken, actualToken, matchingRules == null ? null : matchingRules.Body, result);

			return result;
		}

		private bool AssertPropertyValuesMatch(JToken expected, JToken actual, IDictionary<string, dynamic> matchingRules, ComparisonResult result)
		{
			if (matchingRules != null && matchingRules.ContainsKey(BuildMatchingRulePath(expected.Path)))
			{
				JToken matchingRule = JToken.FromObject(matchingRules[BuildMatchingRulePath(expected.Path)]);

				Matcher matcher;
				if (Matcher.TryParse(matchingRule, out matcher))
				{
					var isMatch = matcher.IsMatch(expected, actual);
					if (!isMatch)
						result.RecordFailure(new DiffComparisonFailure(expected.Root, actual.Root));
					return isMatch;
				}
			}

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

		private string BuildMatchingRulePath(string path)
		{
			return "$." + path;
		}
	}
}
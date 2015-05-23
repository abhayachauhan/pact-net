using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Comparers
{
	//TODO: Split these up into separate tests for each of the individual counterpart
	public class ProviderServiceResponseComparerTests
	{
		private IProviderServiceResponseComparer GetSubject()
		{
			return new ProviderServiceResponseComparer();
		}

		[Fact]
		public void Compare_WithMatchingStatusCodes_NoErrorsAreAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.False(result.HasFailure, "There should not be any errors");
		}

		[Fact]
		public void Compare_WithNonMatchingStatusCodes_OneErrorIsAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201
			};

			var actual = new ProviderServiceResponse
			{
				Status = 400
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithMatchingHeaders_NoErrorsAreAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.False(result.HasFailure, "There should not be any errors");
		}

		[Fact]
		public void Compare_WithMatchingHeadersButWithDifferentCasingOnName_NoErrorsAreAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    { "content-Type", "application/json" }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.False(result.HasFailure, "There should not be any errors");
		}

		[Fact]
		public void Compare_WithMatchingHeadersButWithDifferentCasingOnValue_OneErrorIsAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "Application/Json" }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithMatchingHeadersButResponseHasAdditionalHeaders_NoErrorsAreAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    { "X-Test", "MyCustomThing" },
                    { "X-Test-2", "MyCustomThing2" },
                    { "Content-Type", "application/json" }
                }
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.False(result.HasFailure, "There should not be any errors");
		}

		[Fact]
		public void Compare_WithNonMatchingHeadersValues_OneErrorIsAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain" }
                }
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithNonMatchingHeaderNames_OneErrorIsAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    {"X-Test", "Tester"}
                }
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithResponseThatHasNoHeaders_OneErrorIsAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithMatchingObjectBody_NoErrorsAreAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myString = "Tester",
					myInt = 1,
					myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
				}
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myString = "Tester",
					myInt = 1,
					myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.False(result.HasFailure, "There should not be any errors");
		}

		[Fact]
		public void Compare_WithMatchingObjectBodyOutOfOrder_NoErrorsAreAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myString = "Tester",
					myInt = 1,
					myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
				}
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
					myInt = 1,
					myString = "Tester"
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.False(result.HasFailure, "There should not be any errors");
		}

		[Fact]
		public void Compare_WithMatchingObjectBodyButResponseHasAdditionalProperties_NoErrorsAreAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myString = "Tester",
					myInt = 1,
					myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
				}
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myString = "Tester",
					myInt = 1,
					myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
					additional = "Hello"
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.False(result.HasFailure, "There should not be any errors");
		}

		[Fact]
		public void Compare_WithNonMatchingObject_OneErrorIsAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myString = "Tester",
					myInt = 1,
					myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
					myDouble = 2.0
				}
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myString = "Tester",
					MyInt = 1,
					MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithMatchingObjectAndANonMatchingValue_OneErrorIsAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myNumberMatcher = 2,
					myString = "Tester",
					myInt = 1,
					myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
				}
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myNumberMatcher = 2,
					myString = "Tester2",
					myInt = 1,
					myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithMatchingObjectHoweverPropertyNameCasingIsDifferent_OneErrorIsAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					MyString = "Tester",
					MyInt = 1,
					MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
				}
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myString = "Tester",
					myInt = 1,
					myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithNullBodyInResponse_OneErrorIsAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new
				{
					myString = "Tester",
					myInt = 1,
					myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
				}
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithMatchingCollection_NoErrorsAreAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Tester",
                        myInt = 1,
                        myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                    }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Tester",
                        myInt = 1,
                        myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                    }
                }
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.False(result.HasFailure, "There should not be any errors");
		}

		[Fact]
		public void Compare_WithNonMatchingCollection_OneErrorIsAddedToTheComparisonResult()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Tester",
                        myInt = 1,
                        myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                    }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Tester2",
                        myInt = 1,
                        myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                    }
                }
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, null);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithMatchingCollection_WithTypeMatching()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Example Tester",
                        myInt = 55,
                        myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
						myArray = new[] { "Blah"},
						myBoolean = false,
						myObject = new { RandomProperty = 1 }
                    }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Tester",
                        myInt = 1,
                        myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
						myArray = new[] { "Random", "String" },
						myBoolean = true,
						myObject = new { AnotherProperty = "Test string" }
                    }
                }
			};

			var matchingRules = new PactProviderResponseMatchingRules()
			{
				Body = new Dictionary<string, dynamic>
				{
					{ "$.[0].myString", new { match = "type"} },
					{ "$.[0].myInt", new { match = "type"} },
					{ "$.[0].myArray", new { match = "type"} },
					{ "$.[0].myBoolean", new { match = "type"} },
					{ "$.[0].myObject", new { match = "type"} }
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, matchingRules);

			Assert.False(result.HasFailure, "There should not be any errors");
		}

		[Fact]
		public void Compare_WithMatchingCollection_WithObjectMismatch()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
						myObject = new { RandomProperty = 1 }
                    }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
						myObject = new[] {"Random Property" }
                    }
                }
			};

			var matchingRules = new PactProviderResponseMatchingRules()
			{
				Body = new Dictionary<string, dynamic>
				{
					{ "$.[0].myObject", new { match = "type"} }
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, matchingRules);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithMatchingCollection_WithBooleanMismatch()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
						myBoolean = false
                    }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
						myBoolean = "false"
                    }
                }
			};

			var matchingRules = new PactProviderResponseMatchingRules()
			{
				Body = new Dictionary<string, dynamic>
				{
					{ "$.[0].myBoolean", new { match = "type"} }
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, matchingRules);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithMatchingCollection_WithArrayMismatching()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
						myArray = new[] { "Blah"}
                    }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
						myArray = "Blah"
                    }
                }
			};

			var matchingRules = new PactProviderResponseMatchingRules()
			{
				Body = new Dictionary<string, dynamic>
				{
					{ "$.[0].myArray", new { match = "type"} }
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, matchingRules);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithMatchingCollection_WithIntMismatch()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myInt = 55
                    }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myInt = "String"
                    }
                }
			};

			var matchingRules = new PactProviderResponseMatchingRules()
			{
				Body = new Dictionary<string, dynamic>
				{
					{ "$.[0].myInt", new { match = "type"} }
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, matchingRules);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithMatchingCollection_WithStringMismatch()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Example Tester"
                    }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myString = 5
                    }
                }
			};

			var matchingRules = new PactProviderResponseMatchingRules()
			{
				Body = new Dictionary<string, dynamic>
				{
					{ "$.[0].myString", new { match = "type"} }
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, matchingRules);

			Assert.Equal(1, result.Failures.Count());
		}

		[Fact]
		public void Compare_WithRegExMatching_WithStringMatch()
		{
			var expected = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Example Tester"
                    }
                }
			};

			var actual = new ProviderServiceResponse
			{
				Status = 201,
				Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Another string"
                    }
                }
			};

			var matchingRules = new PactProviderResponseMatchingRules()
			{
				Body = new Dictionary<string, dynamic>
				{
					{ "$.[0].myString", new { regex = @"\w+"} }
				}
			};

			var comparer = GetSubject();

			var result = comparer.Compare(expected, actual, matchingRules);

			Assert.False(result.HasFailure, "There should not be any errors");
		}
	}
}

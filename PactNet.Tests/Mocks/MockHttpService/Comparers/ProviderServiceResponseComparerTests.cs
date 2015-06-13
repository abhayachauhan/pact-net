using System;
using System.Collections.Generic;
using System.Linq;
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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

            Assert.False(result.HasFailure, "There should not be any errors");
        }

        [Fact]
        public void Compare_WithRegexMatchingHeaders_NoErrorsAreAddedToTheComparisonResult()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"},
                    {"Accept", "any string"}
                },
                MatchingRules = new Dictionary<string, dynamic>
                {
                    {"$.headers.Accept", new { regex = @"\w+" }}
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Accept", "another string" }
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.False(result.HasFailure, "There should not be any errors");
        }

        [Fact]
        public void Compare_WithTypeMatchingHeaders_NoErrorsAreAddedToTheComparisonResult()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                MatchingRules = new Dictionary<string, dynamic>
                {
                    {"$.headers.Content-Type", new { match = "type" }}
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

            var result = comparer.Compare(expected, actual);

            Assert.False(result.HasFailure, "There should not be any errors");
        }

        [Fact]
        public void Compare_WithIncorrectRegexMatchingHeaders_ErrorsAreAddedToTheComparisonResult()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"},
                    {"Accept", "any string"}
                },
                MatchingRules = new Dictionary<string, dynamic>
                {
                    {"$.headers.Accept", new { regex = @"\d+" }}
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Accept", "another string" }
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.True(result.Failures.Count() == 1, "There should be one error");
        }

        [Fact]
        public void Compare_WithRegexMatchingHeadersButWithDifferentCasingOnName_NoErrorsAreAddedToTheComparisonResult()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"accept", "any string"}
                },
                MatchingRules = new Dictionary<string, dynamic>
                {
                    {"$.headers.accept", new { regex = @"\w+" }}
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Accept", "another string" }
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.False(result.HasFailure, "There should be no errors");
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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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

            var result = comparer.Compare(expected, actual);

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
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body[0].myString", new { match = "type"} },
					{ "$.body[0].myInt", new { match = "type"} },
					{ "$.body[0].myArray", new { match = "type"} },
					{ "$.body[0].myBoolean", new { match = "type"} },
					{ "$.body[0].myObject", new { match = "type"} }
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

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

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
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body[0].myObject", new { match = "type"} }
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

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

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
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body.[0].myBoolean", new { match = "type"} }
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

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

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
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body.[0].myArray", new { match = "type"} }
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

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

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
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body.[0].myInt", new { match = "type"} }
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

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

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
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body.[0].myString", new { match = "type"} }
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

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

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
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body[0].myString", new { regex = @"\w+"} }
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

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.False(result.HasFailure, "There should not be any errors");
        }

        [Fact]
        public void Compare_WithTypeAndWildcardMatching_WithStringMatch()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    "Example Tester"
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body[*]", new { match = "type"} }
				}
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    "Another string",
                    "Should cause failure"
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.False(result.HasFailure, "There should be no errors");
        }

        [Fact]
        public void Compare_TypeAndWildcardArrayMatchingWithAdditionalProperty_WithMatch()
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
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body[*].*", new { match = "type"} }
				}
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Another string",
                        myNumber = 1
                    },
                    new 
                    {
                        myString = "Another string",
                        myNumber = 2
                    }
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.False(result.HasFailure, "There should be no errors.");
        }

        [Fact]
        public void Compare_WithTypeAndWildcardPropertyMatching_WithMatch()
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
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body[0].*", new { match = "type"} }
				}
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Another string",
                        myNumber = 1
                    }
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.False(result.HasFailure, "There should be no errors.");
        }

        [Fact]
        public void Compare_WithRegExMatching_WithGuidMatch()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body =
                    new
                    {
                        myString = "A9526723-C3BC-4A36-ADF8-9AC7CBDCEE52"
                    },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body.myString", new { regex = @"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$"} }
				}
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body =
                    new
                    {
                        myString = "A9526723-C3BC-4A36-ADF8-9AC7CBDCEE52"
                    }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.False(result.HasFailure, "There should not be any errors");
        }

        [Fact]
        public void Compare_WithRegExMatching_WithIntegerMatch()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body =
                    new
                    {
                        myInt = 5
                    },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body.myInt", new { regex = @"^1[0-9]$"} }
				}
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body =
                    new
                    {
                        myInt = 12
                    }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.False(result.HasFailure, "There should not be any errors");
        }

        [Fact]
        public void Compare_WithRegExMatching_WithStringMismatch()
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
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body.[0].myString", new { regex = @"[Rr]egex"} }
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

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.Equal(result.Failures.Count(), 1);
        }

        [Fact]
        public void Compare_WithTypeMatchingWildcards_WithStringMatch()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    "Example Tester 1"
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body[*]", new { match = @"type"} }
				}
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    "Another string",
                    "Example Tester 2",
                    "Example Tester 3"
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.Equal(result.Failures.Count(), 0);
        }

        [Fact]
        public void Compare_WithTypeMatchingWildcards_WithStringMismatch()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    "Example Tester 1"
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body[*]", new { match = @"type"} }
				}
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    5
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.Equal(result.Failures.Count(), 1);
        }

        [Fact]
        public void Compare_WithTypeMatchingWildcards_WithArrayPropertiesMatch()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    new
                    {
                        First = "Example Tester 1",
                        Second = "Example Tester 2"
                    }
                },
                MatchingRules = new Dictionary<string, dynamic>
                {
                    { "$.body[*].*", new { match = @"type"} }
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    new
                    {
                        First = "Another string",
                        Second = "Another string!!"
                    }
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.Equal(result.Failures.Count(), 0);
        }

        [Fact]
        public void Compare_WithMinMatching_WithArrayLengthTooSmallMismatch()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    "Example Tester 1"
                },
                MatchingRules = new Dictionary<string, dynamic>
                {
                    { "$.body", new { min = 2 } }
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    5
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.Equal(result.Failures.Count(), 1);
        }

        [Fact]
        public void Compare_WithMinMatchingArrayPassesMinCheck_WithMinMatch()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    "Example Tester 1"
                },
                MatchingRules = new Dictionary<string, dynamic>
                {
                    { "$.body", new { min = 2 } }
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    5,
                    100,
                    24
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.False(result.HasFailure, "Should have no errors");
        }

        [Fact]
        public void Compare_WithMinMatching_WithMatch()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    "Example Tester 1"
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body", new { min = 2 } }
				}
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    5,
                    12
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.Equal(result.Failures.Count(), 0);
        }

        [Fact]
        public void Compare_WithMinMatching_WithNotArrayMismatch()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    "Example Tester 1"
                },
                MatchingRules = new Dictionary<string, dynamic>
				{
					{ "$.body", new { min = 2 } }
				}
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    Test = "Random String"
                }
            };

            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.Equal(result.Failures.Count(), 1);
        }
    }
}

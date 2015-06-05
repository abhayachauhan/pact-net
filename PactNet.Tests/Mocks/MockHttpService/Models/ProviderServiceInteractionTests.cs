using System.Collections.Generic;
using Nancy;
using PactNet.Matchers;
using PactNet.Matchers.Definition;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Models
{
	public class ProviderServiceInteractionTests
	{
		[Fact]
		public void ToString_WhenCalled_ReturnsJsonRepresentation()
		{
			const string expectedInteractionJson = "{\"description\":\"My description\",\"provider_state\":\"My provider state\",\"request\":{\"method\":\"delete\",\"path\":\"/tester\",\"query\":\"test=2\",\"headers\":{\"Accept\":\"application/json\"},\"body\":{\"test\":\"hello\"}},\"response\":{\"status\":407,\"headers\":{\"Content-Type\":\"application/json\"},\"body\":{\"yep\":\"it worked\"}}}";
			var interaction = new ProviderServiceInteraction
			{
				Request = new ProviderServiceRequest
				{
					Method = HttpVerb.Delete,
					Body = new
					{
						test = "hello"
					},
					Headers = new Dictionary<string, string>
                    {
                        { "Accept", "application/json" }
                    },
					Path = "/tester",
					Query = "test=2"
				},
				Response = new ProviderServiceResponse
				{
					Status = (int)HttpStatusCode.ProxyAuthenticationRequired,
					Body = new
					{
						yep = "it worked"
					},
					Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    }
				},
				Description = "My description",
				ProviderState = "My provider state",
			};

			var actualInteractionJson = interaction.AsJsonString();

			Assert.Equal(expectedInteractionJson, actualInteractionJson);
		}

		[Fact]
		public void SetResponse_WhenCalled_SplitsOutResponseMatchers()
		{
			var interaction = new ProviderServiceInteraction
			{
				Response = new ProviderServiceResponse
				{
					Body = new
					{
						typematcher = DefineMatcher.TypeEg("it worked"),
						regexmatcher = DefineMatcher.RegExEg(5, @"^\d+$")
					}
				},
				Description = "My description",
				ProviderState = "My provider state",
			};

			Assert.Equal(interaction.MatchingRules.Keys.Count, 2);
			Assert.True(interaction.MatchingRules.ContainsKey("$.typematcher"));
			Assert.True(interaction.MatchingRules.ContainsKey("$.regexmatcher"));
			Assert.True(interaction.Response.Body.typematcher == "it worked");
			Assert.True(interaction.Response.Body.regexmatcher == 5);
		}

	}
}

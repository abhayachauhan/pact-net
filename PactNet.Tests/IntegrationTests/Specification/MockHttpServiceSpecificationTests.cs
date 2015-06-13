using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute.Exceptions;
using Newtonsoft.Json;
using PactNet.Tests.IntegrationTests.Specification.Models;
using Xunit;

namespace PactNet.Tests.IntegrationTests.Specification
{
    public class MockHttpServiceSpecificationTests
    {
        [Fact]
        public void ValidateRequestSpecification()
        {
            var failedTestCases = RunPactSpecificationTests<RequestTestCase>("..\\..\\IntegrationTests\\Specification\\pact-specification\\testcases\\request");

            if (failedTestCases.Any())
            {
                Console.WriteLine("### FAILED ###");
                foreach (var failedTestCase in failedTestCases)
                {
                    Console.WriteLine(failedTestCase);
                }
            }

            Assert.Empty(failedTestCases);
        }

        [Fact]
        public void ValidateResponseSpecification()
        {
            var failedTestCases = RunPactSpecificationTests<ResponseTestCase>("..\\..\\IntegrationTests\\Specification\\pact-specification\\testcases\\response");

            if (failedTestCases.Any())
            {
                Console.WriteLine("### FAILED ###");
                foreach (var failedTestCase in failedTestCases)
                {
                    Console.WriteLine(failedTestCase);
                }
            }

            Assert.Empty(failedTestCases);
        }

        private IEnumerable<string> RunPactSpecificationTests<T>(string pathToTestCases)
            where T : class, IVerifiable
        {
            var failedTestCases = new List<string>();

            if (!Directory.Exists(pathToTestCases))
            {
                throw new InvalidOperationException(String.Format("Specification tests not found in path '{0}'. Please ensure pact-specification git submodule has been pulled (git submodule update --init).", pathToTestCases));
            }

            foreach (var testCaseSubDirectory in Directory.EnumerateDirectories(pathToTestCases))
            {
                var testCaseFileNames = Directory.GetFiles(testCaseSubDirectory);
                foreach (var testCaseFileName in testCaseFileNames)
                {
                    if (testCaseFileName !=
                        @"..\..\IntegrationTests\Specification\pact-specification\testcases\request\body\array with nested array that matches.json")
                    {
                        var testCaseJson = File.ReadAllText(testCaseFileName);
                        var testCase = JsonConvert.DeserializeObject<T>(testCaseJson,
                            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

                        try
                        {
                            Console.WriteLine("Running test: " + testCaseFileName);
                            testCase.Verify();
                        }
                        catch (SubstituteException)
                        {
                            failedTestCases.Add(String.Format("[Failed] {0}", testCaseFileName));

                        }
                    }

                }
            }

            return failedTestCases;
        }
    }
}
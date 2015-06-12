using System.Collections.Generic;
using Newtonsoft.Json;

namespace PactNet.Matchers.Definition
{
    public class MinMatcherDefinition : DefineMatcher
    {
        [JsonProperty("min")]
        public int Minimum { get; set; }

        public MinMatcherDefinition(IEnumerable<dynamic> example, int minimum)
            : base(example)
        {
            Minimum = minimum;
        }

        public new static string Type
        {
            get { return typeof(MinMatcherDefinition).FullName; }
        }

        [JsonIgnore]
        public override dynamic ResponseMatchingRule
        {
            get
            {
                return new
                {
                    min = Minimum
                };
            }
        }
    }
}
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PactNet.Matchers.Definition
{
    public abstract class DefineMatcher
    {
        protected DefineMatcher(dynamic example)
        {
            Example = example;
        }

        public string Path { get; set; }
        public dynamic Example { get; set; }

        [JsonProperty("$type")]
        public string Name
        {
            get { return GetType().FullName; }
        }

        public abstract dynamic ResponseMatchingRule { get; }

        public static TypeMatcherDefinition TypeEg(object example)
        {
            return new TypeMatcherDefinition(example);
        }

        public static TypeMatcherDefinition AllElementsInArrayTypeEg(object example)
        {
            return new TypeMatcherDefinition(new List<dynamic> { example }, "[*]");
        }

        public static TypeMatcherDefinition MatchTypeToAllPropertiesInObjectEg(object example)
        {
            return new TypeMatcherDefinition(example, ".*");
        }

        public static RegExMatcherDefinition RegExEg(object example, string regEx)
        {
            return new RegExMatcherDefinition(example, regEx);
        }

        public static MinMatcherDefinition MinEg(IEnumerable<dynamic> example, int minimum)
        {
            return new MinMatcherDefinition(example, minimum);
        }
    }
}
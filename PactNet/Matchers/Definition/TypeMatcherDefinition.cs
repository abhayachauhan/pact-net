using Newtonsoft.Json;

namespace PactNet.Matchers.Definition
{
    public class TypeMatcherDefinition : DefineMatcher
    {
        public TypeMatcherDefinition(object example, string path = null)
            : base(example)
        {
            Path = path;
        }

        [JsonIgnore]
        public override dynamic ResponseMatchingRule
        {
            get
            {
                return new
                {
                    match = "type"
                };
            }
        }

        public new static string Type
        {
            get { return typeof(TypeMatcherDefinition).FullName; }
        }
    }
}
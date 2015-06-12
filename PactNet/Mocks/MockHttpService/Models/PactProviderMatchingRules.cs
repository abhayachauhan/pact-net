using System.Collections.Generic;
using Newtonsoft.Json;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class PactProviderMatchingRules
    {
        [JsonProperty(PropertyName = "body")]
        public IDictionary<string, dynamic> Body { get; set; }

        public PactProviderMatchingRules()
        {
            Body = new Dictionary<string, dynamic>();
        }
    }
}

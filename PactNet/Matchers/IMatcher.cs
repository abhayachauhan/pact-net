using Newtonsoft.Json.Linq;

namespace PactNet.Matchers
{
	public interface IMatcher
	{
		bool IsMatch(JToken expected, JToken actual);
	}
}
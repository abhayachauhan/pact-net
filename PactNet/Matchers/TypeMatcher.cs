public class TypeMatcher : Matcher
{
	public enum DataType
	{
		String,
		Boolean,
		Number,
		Object,
		Array
	};

	public DataType Type { get; set; }

	public TypeMatcher(DataType type)
	{
		Type = type;
	}
}
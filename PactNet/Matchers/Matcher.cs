public abstract class Matcher
{
	public 
	public static TypeMatcher Type(TypeMatcher.DataType type)
	{
		return new TypeMatcher(type);
	}

	public static RegExMatcher RegEx(string regEx)
	{
		return new RegExMatcher(regEx);
	}
}
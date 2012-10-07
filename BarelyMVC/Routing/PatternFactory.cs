using System;
using System.Text.RegularExpressions;

namespace Earlz.BarelyMVC
{
	public static class PatternFactory
	{
		public static IPatternMatcher GetPattern(PatternTypes type, string pattern)
		{
			switch(type)
			{
			case PatternTypes.Plain:
				return new PlainPatternMatcher(pattern);
				break;
			case PatternTypes.Regex:
				return new RegexPatternMatcher(pattern);
				break;
			case PatternTypes.Simple:
				return new SimplePattern(pattern);
				break;
			default:
				throw new NotSupportedException("PatternMatcher of that type not found");
			}
		}
	}
	public class RegexPatternMatcher : IPatternMatcher
	{
		Regex regex;
		public RegexPatternMatcher(string pattern)
		{
			regex=new Regex(pattern,RegexOptions.Compiled);
		}
		public ParameterDictionary Params
		{
			get
			{
				return null;
			}
		}
		public bool IsMatch(string input)
		{
			return regex.IsMatch(input);
		}
	}
	public class PlainPatternMatcher : IPatternMatcher
	{
		string Pattern;
		public PlainPatternMatcher(string pattern)
		{
			Pattern=pattern;
		}
		public bool IsMatch(string input)
		{
			return Pattern==input;
		}
		public ParameterDictionary Params
		{
			get
			{
				return null;
			}
		}
	}
	
}


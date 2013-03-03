using System;
using Earlz.LucidMVC;

namespace Earlz.LucidMVC.Tests
{
	public class FakePatternMatcher : IPatternMatcher 
	{
		public MatchResult Match (string input)
		{
			return new MatchResult(Pattern==input, Params);
		}
		ParameterDictionary Params;
		public string Pattern;
		public FakePatternMatcher(string pattern, ParameterDictionary param=null)
		{
			Pattern=pattern;
			Params=param;
		}
	}
}


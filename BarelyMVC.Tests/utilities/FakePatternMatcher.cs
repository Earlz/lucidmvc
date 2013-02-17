using System;
using Earlz.BarelyMVC;

namespace Earlz.BarelyMVC.Tests
{
	public class FakePatternMatcher : IPatternMatcher 
	{
		public bool IsMatch (string input)
		{
			return Pattern==input;
		}

		public ParameterDictionary Params {
			get;
			set;
		}

		public string Pattern;
		public FakePatternMatcher(string pattern)
		{
			Pattern=pattern;
		}
	}
}


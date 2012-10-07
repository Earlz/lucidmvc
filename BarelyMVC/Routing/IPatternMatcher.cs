using System;

namespace Earlz.BarelyMVC
{
	public interface IPatternMatcher
	{
		/// <summary>
		/// Determines rather the given input matches this pattern
		/// </summary>
		bool IsMatch(string input);

		/// <summary>
		/// The list of parameters and values for the last match. Yields null if parameters are not supported
		/// </summary>
		ParameterDictionary Params
		{
			get;
		}
	}
}


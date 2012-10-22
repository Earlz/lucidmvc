using System;
using System.Collections.Generic;

namespace Earlz.BarelyMVC
{
	public class TrackedDictionary<T,U> : IDictionary<T, U>
	{
		public TrackedDictionary(IDictionary<T,U> initial)
			: base(initial)
		{
			Changes=new LinkedList<KeyValuePair<T, U>>();
		}
		public IList<KeyValuePair<T,U>> Changes
		{
			get;private set;
		}
		public IList<KeyValuePair<T,U>> Removes
		{
			get;private set;
		}
		public IList<KeyValuePair<T,U>> Additions
		{
			get; private set;
		}
	}
}


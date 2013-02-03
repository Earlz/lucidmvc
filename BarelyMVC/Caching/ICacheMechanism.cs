using System;
using System.Collections.Generic;

namespace Earlz.BarelyMVC.Caching
{
	public interface ICacheMechanism
	{
		/// <summary>
		/// Will add the specified item to the cache. This will overwrite any key that already exists
		/// If the value is null, the key should be removed
		/// This must be thread-safe
		/// </summary>
		void Set(string key, object obj, CacheInfo info);
		/// <summary>
		/// Will get a value from the cache. If the item does not exist, null should be returned
		/// An Exists method does not exist because it has inherit concurrency issues
		/// This must be thread-safe
		/// </summary>
		object Get(string key);
		/// <summary>
		/// A place to store key names mapped to cacheinfo. 
		/// This is to make the exposed API much nicer and is unavoidable. Just use a regular dictionary
		/// This must be thread-safe to read, but not to modify. This should only be modified within a single-use non-concurrent static constructor
		/// </summary>
		IDictionary<string, CacheInfo> KeyInfo{get;}
	}
}


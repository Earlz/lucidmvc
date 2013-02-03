using System;
using System.Collections.Generic;

namespace Earlz.BarelyMVC.Caching
{
	/// <summary>
	/// A very simplistic interface to different caching mechanisms. 
	/// The methods provided are extremely simple and designed to be easy to implement
	/// Unless otherwise noted, everything should be thread-safe and not throw exceptions
	/// </summary>
	public interface ICacheMechanism
	{
		/// <summary>
		/// Will add the specified item to the cache. This will overwrite any key that already exists
		/// If the value is null, the key should be removed
		/// If info is null, it should be looked up in KeyInfo by name (and an exception thrown if not found) 
		/// This must be thread-safe
		/// </summary>
		void Set(string key, object obj, CacheInfo info=null);
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


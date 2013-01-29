using System;

namespace Earlz.BarelyMVC.Caching
{
	public interface ICacheMechanism
	{
		/// <summary>
		/// Will add the specified item to the cache. This will overwrite any key that already exists
		/// If the value is null, the key should be removed
		/// </summary>
		object Set(string key, object obj, CacheInfo info);
		/// <summary>
		/// Will get a value from the cache. If the item does not exist, null should be returned
		/// An Exists method does not exist because it has inherit concurrency issues
		/// </summary>
		object Get(string key);
	}
}


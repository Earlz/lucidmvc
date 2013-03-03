using System;

namespace Earlz.LucidMVC.Caching
{
	//StoreToCache will handle removal, replacement, and addition to the cache
	public interface ICacheDictionary<K,V>
	{
		/// <summary>
		/// Can't enfore a constructor, so we use this to do the inital setup of the dictionary
		/// </summary>
		void Setup(string basekey, ICacheMechanism cacher);
		ICacheMechanism Cacher
		{
			get;
		}
		/// <summary>
		/// The CacheObject to be used by default(by the indexer)
		/// </summary>
		CacheInfo CacheInfo
		{
			get;set;
		}
		/// <summary>
		/// Adds, removes, updates, or gets a value correlating to the specified key
		/// If value is null and the key exists, it is removed. If the key doesn't exist, it is added. If the key does exist, it is updated
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>
		V this[K key]
		{
			get;
			set;
		}
		void Remove(K key);
		void Set(K key, V value, CacheInfo info=null);
		/// <summary>
		/// Clear the cache. Dictionaries which do not support this should throw NotSupportedException
		/// </summary>
		void Clear();

	}
}


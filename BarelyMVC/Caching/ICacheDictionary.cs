using System;

namespace Earlz.BarelyMVC.Caching
{
	//StoreToCache will handle removal, replacement, and addition to the cache
	public delegate object StoreToCache(string key, object value, CacheInfo info);
	public delegate object GetFromCache(string key);

	public interface ICacheDictionary<K,V>
	{
		/// <summary>
		/// Can't enfore a constructor, so we use this to do the inital setup of the dictionary
		/// </summary>
		void Setup(string basekey, StoreToCache store, GetFromCache get);
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
		V Remove(K key);
		V Set(K key, V value, CacheInfo info);
		void Clear();

	}
}


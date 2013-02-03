using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Earlz.BarelyMVC.Caching.Experimental
{

	/// <summary>
	/// This should not be used in most cases! Make sure you know what you're doing if you use this!
	/// This dictionary will keep track of of key's reference value as described by operator==. 
	/// This is ideal for using reference types as keys where the ToString value is not to be considered unique or suitable for caching
	/// This WILL NOT scale to a distributed cache, because reference types are inheritely "unique" no matter if the content is the same. 
	/// </summary>
	public class TrackingCacheDictionary<K,V> : ICacheDictionary<K,V>
	{
		public void Set (K key, V value, CacheInfo info)
		{
			if(value==null)
			{
				Remove(key);
			}
			else
			{
				Add (key, value, info);
			}
		}

		public CacheInfo CacheInfo {
			get;
			set;
		}


		ConcurrentDictionary<K, string> RealKeys=new ConcurrentDictionary<K, string>();
		string BaseKey;
		/// <summary>
		/// This returns the amount of keys we are tracking within this CacheDictionary.
		/// Note: This does not necessarily indicate how many items are actually still in the cache! 
		/// </summary>
		/// <returns>
		/// The count.
		/// </returns>
		public int TrackedCount
		{
			get
			{
				return RealKeys.Count;
			}
		}
		public void Setup(string basekey, ICacheMechanism cacher)
		{
			Cacher=cacher;
			BaseKey=basekey;
		}
		public ICacheMechanism Cacher
		{
			get;
			private set;
		}
		void Add (K key, V value, CacheInfo info)
		{
			string realkey=RealKeys.GetOrAdd(key, (s) => GenerateKey(key));
			lock(realkey)
			{
				Cacher.Set(realkey, value, info);
			}
		}
		public void Remove (K key)
		{
			Cacher.Set(GetKey(key), null, CacheInfo);
			string trash=null;
			RealKeys.TryRemove(key, out trash);
		}
		static long CurrentKey=0;
		string GenerateKey(K key)
		{
			long tmp=Interlocked.Increment(ref CurrentKey);
			return BaseKey+(tmp).ToString();
		}
		string GetKey(K key)
		{
			string tmp=null;
			if(!RealKeys.TryGetValue(key, out tmp))
			{
				return null;
			}
			return tmp;
		}
		public V this [K key] {
			get {
				string realkey=GetKey(key);
				if(realkey==null)
				{
					return default(V);
				}
				lock(realkey)
				{
					object tmp=Cacher.Get(realkey);
					if(tmp!=null && tmp is V)
					{
						return (V)tmp;
					}
					else
					{
						string trash=null;
						RealKeys.TryRemove(key, out trash); //cleanup
						return default(V);
					}
				}
			}
			set {
				Set(key, value, CacheInfo);
			}
		}
		public void Clear ()
		{
			foreach(var key in RealKeys.Keys)
			{
				var realkey=GetKey(key);
				if(realkey!=null) //ConcurrentDictionary's enumator represents a snapshot, so while iterating, the key may no longer exist
				{
					Cacher.Set(GetKey(key), null, CacheInfo);
				}
			}
			RealKeys.Clear();
		}
	}
}


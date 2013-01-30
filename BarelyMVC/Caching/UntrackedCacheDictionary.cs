using System;
using System.Threading;

namespace Earlz.BarelyMVC.Caching
{
	/// <summary>
	/// This is a cache pass through dictionary which does no tracking of what is possibly cached.
	/// It relies on the `.ToString` function of the key type being unique. 
	/// This means that two identical objects with different references will be detected as the same cache key, which may or may not be desirable.
	/// </summary>
	public class UntrackedCacheDictionary<K,V> : ICacheDictionary<K,V>
	{
		public delegate string CustomToString(K key);
		StoreToCache StoreTo;
		GetFromCache GetFrom;
		string BaseKey;
		CustomToString customToString;

		int Initialized=0;

		public void Setup(string basekey, StoreToCache store, GetFromCache get, CustomToString custom=null)
		{
			Interlocked.CompareExchange(ref Initialized, 1, 0);
			BaseKey=basekey;
			StoreTo=store;
			GetFrom=get;
			customToString=custom;
		}
		public void Setup(string basekey, StoreToCache store, GetFromCache get)
		{
			Setup(basekey, store, get);
		}
		public V Remove (K key)
		{
			return Set(key, default(V), CacheInfo);
		}
		public V Set (K key, V value, CacheInfo info)
		{
			object tmp=StoreTo(BaseKey+ConvertToString(key), value, info);
			if(tmp!=null && tmp is V)
			{
				return (V) tmp;
			}
			else
			{
				return default(V);
			}
		}
		public void Clear ()
		{
			//This isn't support because it's not tracked. 
			throw new NotSupportedException();
		}
		public CacheInfo CacheInfo {
			get;
			set; 
		}
		public V this [K key] {
			get 
			{
				object tmp=GetFrom(BaseKey+ConvertToString(key));
				if(tmp!=null && tmp is V)
				{
					return (V) tmp;
				}
				else
				{
					return default(V);
				}
			}
			set 
			{
				Set(key, value, CacheInfo);
			}
		}
		string ConvertToString(K key)
		{
			if(customToString!=null)
			{
				return customToString(key);
			}
			else
			{
				return key.ToString();
			}
		}
	}
}


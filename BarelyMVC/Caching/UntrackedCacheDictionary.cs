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
		string BaseKey;
		CustomToString customToString;


		public void Setup(string basekey, ICacheMechanism cacher, CustomToString custom=null)
		{
			Cacher=cacher;
			BaseKey=basekey;
			customToString=custom;
		}
		public void Setup(string basekey, ICacheMechanism cacher)
		{
			Setup(basekey, cacher, null);
		}
		public ICacheMechanism Cacher
		{
			get;
			private set;
		}
		public void Remove (K key)
		{
			Set(key, default(V), CacheInfo);
		}
		public void Set (K key, V value, CacheInfo info=null)
		{
			Cacher.Set(BaseKey+ConvertToString(key), value, info);
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
				object tmp=Cacher.Get(BaseKey+ConvertToString(key));
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


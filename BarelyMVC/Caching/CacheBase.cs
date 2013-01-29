using System;
using Earlz.BarelyMVC.Caching;

namespace Earlz.BarelyMVC
{
	public class CacheBase
	{
		protected CacheBase()
		{
		}
		static CacheBase()
		{
			Cacher=new ASPCacheMechanism();
		}
		public static void Set(string name, object value, CacheInfo info)
		{
		}
		public static ICacheDictionary<K,V> GetDictionary<K,V>(string name, CacheInfo info)
		{
			return null;
		}
		public static V Get<V>(string name)
		{
			return default(V);
		}
		public static ICacheMechanism Cacher
		{
			get;
			set;
		}
	}
}


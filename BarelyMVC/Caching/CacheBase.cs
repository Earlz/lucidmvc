using System;
using Earlz.BarelyMVC.Caching;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Earlz.BarelyMVC
{
	/// <summary>
	/// The base class to make the creation of cache proxy classes much easier than would otherwise be possible
	/// Note, this is really a static class, but we mark it non-static so that inheritance is possible
	/// </summary>
	// scrap and use extension methods on ICacheMechanism!
	public class CacheBase 
	{
		protected CacheBase()
		{
		}
		static CacheBase()
		{
			Cacher=new ASPCacheMechanism();
		}
		public static Dictionary<string, CacheInfo> Infos=new Dictionary<string, CacheInfo>();

		public static void Set(string name, CacheInfo info, object value, ICacheMechanism cacher=null)
		{
			cacher.Set(name, value, cacher);
		}
		public static void Setup(string name, CacheInfo info, ICacheMechanism cacher=null)
		{
			Infos.Add(name, info);
		}
		public static ICacheDictionary<K,V> SetupDictionary<K,V>(string name, CacheInfo info, ICacheDictionary<K,V> usethis=null, ICacheMechanism cacher=null)
		{
			ICacheDictionary<K,V> d=usethis ?? new UntrackedCacheDictionary<K, V>();
			//d.Setup(name, 
			//return null;
		}
		public static V Get<V>(string name, ICacheMechanism cacher=null)
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


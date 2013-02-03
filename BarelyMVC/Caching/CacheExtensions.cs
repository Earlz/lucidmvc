using System;
using Earlz.BarelyMVC.Caching;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Earlz.BarelyMVC.Caching
{
	/// <summary>
	/// A set of extension methods on ICacheMechanism to make caching as concise and awesome as possible
	/// </summary>
	public static class CacheExtensions
	{
		public static void Set(this ICacheMechanism cacher, string name, object value, CacheInfo info=null)
		{
			cacher.Set(name, value, info ?? cacher.KeyInfo[name]);
		}
		public static void Setup(this ICacheMechanism cacher, string name, CacheInfo info)
		{
			cacher.KeyInfo.Add(name, info);
		}
		public static ICacheDictionary<K,V> SetupDictionary<K,V>(this ICacheMechanism cacher, string name, CacheInfo info, ICacheDictionary<K,V> usethis=null)
		{
			var d=usethis ?? new UntrackedCacheDictionary<K, V>();
			d.Setup(name, cacher);
			d.CacheInfo=info;
			return d;
		}
		public static V Get<V>(this ICacheMechanism cacher, string name)
		{
			object tmp=cacher.Get(name);
			if(tmp is V)
			{
				return (V)tmp;
			}
			return default(V);
		}
	}
}


using System;
using Earlz.LucidMVC.Caching;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Earlz.LucidMVC.Caching
{
	/// <summary>
	/// A set of extension methods on ICacheMechanism to make caching as concise and awesome as possible
	/// </summary>
	public static class CacheExtensions
	{
		public static ICacheDictionary<K,V> SetupDictionary<K,V>(this ICacheMechanism cacher, string name, CacheInfo info, ICacheDictionary<K,V> usethis=null)
		{
			var d=usethis ?? new UntrackedCacheDictionary<K, V>();
			d.Setup(name, cacher);
			d.CacheInfo=info;
			return d;
		}
		public static V TryGet<V>(this ICacheMechanism cacher, string name)
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


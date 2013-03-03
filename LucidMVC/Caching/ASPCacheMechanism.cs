using System;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;

namespace Earlz.LucidMVC.Caching
{
	public class ASPCacheMechanism : ICacheMechanism
	{
		public ASPCacheMechanism()
		{
			KeyInfo=new Dictionary<string, CacheInfo>();
		}
		public IDictionary<string, CacheInfo> KeyInfo {
			get;
			private set;
		}


		public object Get (string key)
		{
			return HttpRuntime.Cache[key];
		}
		public void Set (string key, object obj, CacheInfo info=null)
		{
			if(obj==null)
			{
				HttpRuntime.Cache.Remove(key);
			}
			DateTime absolute=Cache.NoAbsoluteExpiration;
			if(info.AbsoluteExpirationFromNow!=null)
			{
				absolute=DateTime.Now;
				absolute.Add(info.AbsoluteExpirationFromNow.Value);
			}
			HttpRuntime.Cache.Insert(key, obj, null, absolute, info.SlidingExpiration ?? Cache.NoSlidingExpiration, ConvertPriority(info.Priority), null);
		}
		public CacheItemPriority ConvertPriority(CachePriority p)
		{
			switch(p)
			{
			case CachePriority.AboveNormal:
				return CacheItemPriority.AboveNormal;
			case CachePriority.BelowNormal:
				return CacheItemPriority.BelowNormal;
			case CachePriority.Default:
				return CacheItemPriority.Default;
			case CachePriority.High:
				return CacheItemPriority.High;
			case CachePriority.Low:
				return CacheItemPriority.Low;
			case CachePriority.Normal:
				return CacheItemPriority.Normal;
			case CachePriority.NotRemovable:
				return CacheItemPriority.NotRemovable;
			default:
				return CacheItemPriority.Default;
			}
		}
	}
}


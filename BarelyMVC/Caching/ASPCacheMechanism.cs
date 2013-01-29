using System;
using System.Web;
using System.Web.Caching;

namespace Earlz.BarelyMVC.Caching
{
	public class ASPCacheMechanism : ICacheMechanism
	{
		public object Get (string key)
		{
			return HttpRuntime.Cache[key];
		}
		public object Set (string key, object obj, CacheInfo info)
		{
			if(obj==null)
			{
				return HttpRuntime.Cache.Remove(key);
			}
			DateTime absolute=Cache.NoAbsoluteExpiration;
			if(info.AbsoluteExpirationFromNow!=null)
			{
				absolute=DateTime.Now;
				absolute.Add(info.AbsoluteExpirationFromNow.Value);
			}
			HttpRuntime.Cache.Insert(key, obj, null, absolute, info.SlidingExpiration ?? Cache.NoSlidingExpiration, ConvertPriority(info.Priority), null);
			return obj;
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


using System;
using System.Collections.Concurrent;
using Earlz.BarelyMVC.Caching;
using System.Collections.Generic;

namespace BarelyMVC.Tests
{
	public class MockCacheMechanism : ICacheMechanism
	{
		public MockCacheMechanism()
		{
			KeyInfo=new Dictionary<string, CacheInfo>();
		}
		public IDictionary<string, CacheInfo> KeyInfo {
			get;
			private set;
		}


		//make it concurrent so we can eventually test concurrency
		public ConcurrentDictionary<string, object> Cache=new ConcurrentDictionary<string, object>();
		public ConcurrentDictionary<string, CacheInfo> CacheInfo=new ConcurrentDictionary<string, CacheInfo>();
		public object Get (string key)
		{
			object tmp=null;
			Cache.TryGetValue(key, out tmp);
			return tmp;
		}
		public void Set (string key, object obj, CacheInfo info)
		{
			if(obj==null)
			{
				CacheInfo trash=null;
				Cache.TryRemove(key, out obj);
				return;
			}

			Cache[key]=obj;
		}
		public void Reset()
		{
			Cache.Clear();
			CacheInfo.Clear();
		}
	}
}


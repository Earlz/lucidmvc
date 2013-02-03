using System;

namespace Earlz.BarelyMVC.Caching
{
	/// <summary>
	/// Priority of the cache item
	/// </summary>
	public enum CachePriority
	{
		Low,
		BelowNormal,
		Normal,
		Default,
		AboveNormal,
		High,
		NotRemovable
	}
	/// <summary>
	/// Describes a cached object
	/// </summary>
	public class CacheInfo
	{
		public CacheInfo(CachePriority priority=CachePriority.Default, TimeSpan? absolute=null, TimeSpan? sliding=null)
		{
			AbsoluteExpirationFromNow=absolute;
			SlidingExpiration=sliding;
			Priority=priority;
		}
		//provide a second overload so that people uninterested in priority don't have to specify CachePriorty.Default
		public CacheInfo(TimeSpan? absolute=null, TimeSpan? sliding=null, CachePriority priority=CachePriority.Default) 
		{
			AbsoluteExpirationFromNow=absolute;
			SlidingExpiration=sliding;
			Priority=priority;
		}
		/// <summary>
		/// Will add this timespan to DateTime.Now (upon cache creation) to get an absolute expiration value
		/// </summary>
		public TimeSpan? AbsoluteExpirationFromNow
		{
			get;
			private set;
		}
		/// <summary>
		/// A sliding expiration so that each time the item is accessed, it's expiration gets pushed by this value
		/// </summary>
		public TimeSpan? SlidingExpiration
		{
			get;
			private set;
		}
		public CachePriority Priority
		{
			get;
			private set;
		}
		/*/// <summary>
		/// This cache mechanism should be used instead of the default(or null to use default)
		/// This should only be filled in if you are doing some crazy awesome caching algorithms to segregate them conditionally
		/// </summary>
		public ICacheMechanism CacheOverride
		{
			get;
			private set;
		}
		*/
	}
}


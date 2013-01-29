using System;

namespace Earlz.BarelyMVC
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
		/// <summary>
		/// The name of the property to be generated
		/// </summary>
		public string Name;
		/// <summary>
		/// The type returned from the cache object's property. This should be null if KeyType is not null (it'll use CacheDictionary)
		/// Note: This should only be used from T4
		/// </summary>
		public string Type;
		/// <summary>
		/// The key type of the cached dictionary. Leave null to just generate a normal property and not a dictionary
		/// Note: To be used only from T4
		/// </summary>
		public string KeyType=null;
		/// <summary>
		/// Will add this timespan to DateTime.Now (upon cache creation) to get an absolute expiration value
		/// </summary>
		public TimeSpan? AbsoluteExpirationFromNow;
		public TimeSpan? SlidingExpiration;
		/// <summary>
		/// The value type of the cached dictionary. 
		/// Note: To be used only from T4
		/// </summary>
		public string ValueType;
		public CachePriority Priority=CachePriority.Default;
		public string DictionaryType="UntrackedCacheDictionary";
	}
}


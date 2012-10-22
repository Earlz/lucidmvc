using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Earlz.BarelyMVC
{
	static internal class CurrentRequest
	{
		static ConcurrentDictionary<Thread, RequestContext> ThreadContexts=new ConcurrentDictionary<Thread, RequestContext>();
		/// <summary>
		/// Called from Router at the earliest part of the request
		/// </summary>
		internal static void InitRequest(RequestContext context)
		{
			//Don't worry about tracking this on spawned threads. It appears to be impossible and ASP.Net doesn't, so fuck it
			ThreadContexts.AddOrUpdate(Thread.CurrentThread, context, (x,y)=>y=context);
		}
		internal static void EndRequest()
		{
			RequestContext tmp; 
			ThreadContexts.TryRemove(Thread.CurrentThread, out tmp); //do we really care if it fails?
		}

	}
}


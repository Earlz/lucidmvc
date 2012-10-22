using System;
using System.Threading;
using System.Collections.Generic;

namespace Earlz.BarelyMVC
{
	static internal class CurrentRequest
	{
		static Dictionary<Thread, RequestContext> ThreadContexts=new Dictionary<Thread, RequestContext>();
		/// <summary>
		/// Called from Router at the earliest part of the request
		/// </summary>
		internal static void InitRequest(RequestContext context)
		{
			if(ThreadContexts.ContainsKey(Thread.CurrentThread))
			{
				ThreadContexts[Thread.CurrentThread]=context;
			}
			else
			{
				ThreadContexts.Add(Thread.CurrentThread, context);
			}
		}

	}
}


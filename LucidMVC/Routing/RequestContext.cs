using System;
using Earlz.BarelyMVC.Caching;

namespace Earlz.BarelyMVC
{
	public class RequestContext
	{
		public IServerContext Context{get;private set;}
		public Router Router{get;private set;}
		public Route Route{get;private set;}
		public ParameterDictionary RouteParams{get;set;}
		public RequestContext(IServerContext context, Router router, Route route, ParameterDictionary routeparams)
		{
			Context=context;
			Router=router;
			Route=route;
			RouteParams=routeparams;
		}
	}
}


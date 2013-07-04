/*
Copyright (c) 2010 - 2012 Jordan "Earlz/hckr83" Earls  <http://lastyearswishes.com>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.
   
THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL
THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Earlz.LucidMVC.ViewEngine;
using System.Linq;
using Earlz.LucidMVC.Authentication;
using System.Collections.ObjectModel;
using Earlz.LucidMVC.Caching;

namespace Earlz.LucidMVC
{
	public delegate ILucidView HandlerInvoker<T>(T httphandler) where T:HttpController;
	public delegate T HandlerCreator<T>(Router r) where T:HttpController;
	public delegate ICacheMechanism CacheMechanismRetriever();
	public interface IRouter
	{

		/// <summary>
		/// Gets a new or existing ICacheMechanism
		/// Defaults to getting a new ASPCacheMechanism
		/// </summary>
		CacheMechanismRetriever GetCacher{get;set;}
		Route[] GetRoutes();
		void AddRoute(Route r);
		ControllerBox<T, object> Controller<T>(ControllerCreator<T> creator) where T:HttpController;
		ControllerBox<T, object> Controller<T>(ControllerCreator<T> creator, string root) where T:HttpController;


		bool Execute(IServerContext context);
	}
    /**The routing engine of EFramework.
     * This is a simple, but powerful router utilizing simple route pattern matching and lambdas for initializing the HttpHandler for a request.**/
    public class Router : IRouter
    {

		/// <summary>
		/// Gets a new or existing ICacheMechanism
		/// Defaults to getting a new ASPCacheMechanism
		/// </summary>
		public CacheMechanismRetriever GetCacher
		{
			get;
			set;
		}
		protected IList<Route> Routes
		{
			get;
			private set;
		}
		public Route[] GetRoutes()
		{
			return Routes.ToArray();
		}
		public Router()
		{
			GetCacher=() => new ASPCacheMechanism(); //default to ASP.Net
			Routes=new List<Route>();
		}
		public virtual void AddRoute(Route r)
		{
			Routes.Add(r);
		}
		public virtual ControllerBox<T, object> Controller<T>(ControllerCreator<T> creator) where T:HttpController
		{
			return new ControllerBox<T, object>(this, creator);
		}
		public virtual ControllerBox<T, object> Controller<T>(ControllerCreator<T> creator, string root) where T:HttpController
		{
			return new ControllerBox<T, object>(this, creator, root);
		}


		public virtual bool Execute(IServerContext context)
		{
			var defaultallowed=new string[]{"get"};
			foreach(var route in Routes)
			{
				if(route.Pattern==null) continue;
				var allowed=route.AllowedMethods ?? defaultallowed;
				var match=route.Pattern.Match(context.RequestUrl.AbsolutePath);
				if(match.IsMatch &&
				   allowed.Any(x=>x.ToLower()==context.HttpMethod.ToLower()))
				{
					var request=new RequestContext(context, this, route, match.Params);
					bool skip=false;
					var view=route.Responder(request, ref skip);
					if(view==null)
					{
						throw new NotSupportedException("The returned view from a controller must not be null!");
					}
					if(!skip)
					{
						view.RenderView(context.Writer);
						return true;
					}
				}
			}
			return false;
		}
	}

	/*example fluent API usage:
	 * 
	 * Router.Route("/foo").
	 * 		IsHandledBy((r) => new MyHandler(r), (h) => h.Foo()).
	 * 		Accepts(HttpMethod.Get).
	 * 		AlsoIncludes("/foobar").
	 * 		AlsoIncludes("/{a}/{b}").
	 * 		RouteParam("a").MustBe(GroupMatchType.Integer).
	 * 		RouteParam("a").MustMatch("/someregex/").
	 * 		RedirectFrom("/foo/oldurl").
	 * 		IsProtected
	 * 
	 * 
	 * ORRRRR
	 * (if this is possible!)
	 * var blog=Router.Controller((r) => new BlogHandler(r));
	 * blog.Handles("/blog/view/{foo}").With((h) => h.Viewblog());
	 * blog.Handles("/blog/new").
	 * 		With((h) => h.New()).
	 * 		IsProtected().
	 * 		AlsoIncludes("/new").
	 * 		Accepts(HttpMethod.Get | HttpMethod.Post);
	 * 
	 * */

}





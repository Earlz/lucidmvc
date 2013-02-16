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
using Earlz.BarelyMVC.ViewEngine;
using System.Linq;
using Earlz.BarelyMVC.Authentication;
using System.Collections.ObjectModel;

namespace Earlz.BarelyMVC
{
	public delegate IBarelyView HandlerInvoker<T>(T httphandler) where T:HttpController;
	public delegate T HandlerCreator<T>(Router r) where T:HttpController;
    
    /**The routing engine of EFramework.
     * This is a simple, but powerful router utilizing simple route pattern matching and lambdas for initializing the HttpHandler for a request.**/
    public class Router
    {
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
			Routes=new List<Route>();
		}
		public virtual void AddRoute(Route r)
		{
			Routes.Add(r);
		}
		public virtual ControllerBox<T> Controller<T>(ControllerCreator<T> creator) where T:HttpController
		{
			return new ControllerBox<T>(this, creator);
		}

		public virtual bool Execute(IServerContext context)
		{
			var defaultallowed=new string[]{"get"};
			foreach(var route in Routes)
			{
				var allowed=route.AllowedMethods ?? defaultallowed;
				if(route.Pattern!=null && route.Pattern.IsMatch(context.RequestUrl.AbsolutePath) &&
				   allowed.Any(x=>x.ToLower()==context.RawHttpMethod.ToLower()))
				{
					context.Writer.Write(route.Responder(context).RenderView());
					return true;
				}
			}
			return false;
		}


        /// <summary>
        /// Adds a route to the router
        /// </summary>
		/// 
		/*
        public void AddRoute<T>(HttpMethod method, string pattern, HandlerCreator<T> creator, HandlerInvoker<T> invoker) where T:HttpHandl
        {
            //var r=new Route{Pattern=new SimplePattern(pattern), Invoker=handler, ID=id, Method=method};
            //Routes.Add(r);
        }*/
		/*
        public void AddRoute(string id,HttpMethod method, IPatternMatcher pattern, HandlerInvoker handler)
        {
            var r=new Route{Pattern=pattern, ID=id, Invoker=handler, Method=method};
            Routes.Add(r);
        }
		/// <summary>
		/// Adds a route to the router which requires authentication
		/// </summary>
		public void AddSecureRoute(string id, HttpMethod method, IPatternMatcher pattern, HandlerInvoker handler)
		{
			var r=new Route{Pattern=pattern, ID=id, Invoker=handler, Method=method, Secure=true};
			Routes.Add(r);
		}
		public void AddSecureRoute(string id, HttpMethod method, string pattern, HandlerInvoker invoker)
		{
			var r=new Route{Pattern=new SimplePattern(pattern), Invoker=invoker, ID=id, Method=method, Secure=true};
			Routes.Add(r);
		}
        */
        void DoHandler (Route r,IServerContext c,ParameterDictionary p)
        {
			/*
            HttpHandler.RouteRequest=r;
            HttpHandler.Method=c.SaneHttpMethod();
            HttpHandler.RawRouteParams=p;

            CallMethod(c, r.Invoker);
            */
        }
		/*
        /// <summary>
        /// Handles the current request
        /// </summary>
        public bool DoRoute(IServerContext c){
			bool foundwrongmethod=false;
            foreach(var r in Routes){
                if(r.Pattern.IsMatch(c.RequestUrl.AbsolutePath))
                {
                    var m=c.SaneHttpMethod();
                    if(r.Method == HttpMethod.Any || m==r.Method || 
                       (r.Method==HttpMethod.Get && m==HttpMethod.Head))
                    {

						if(r.Secure)
						{
							FSCAuth.RequiresLogin();
						}
                        DoHandler(r, c, r.Pattern.Params);
                        return true;
                    }else
					{
						foundwrongmethod=true;
					}
                }
            }
			if(foundwrongmethod)
			{
				throw new HttpException(405, "Method not allowed");;
			}
            return false;
        }

        void CallMethod<T>(IServerContext context, HandlerInvoker<T> invoker) where T:HttpController 
		{
			IBarelyView view=invoker(null);//HttpHandler.RawRouteParams, HttpHandler.Form.ToParameters());
            int length=0;
            var r=context.Writer;
            if(view!=null){
                //even if "directly-rendered", if ignoring the view, it won't really be rendered
                var s=view.RenderView();
                
                length+=s.Length;
                
                if(!view.RenderedDirectly){
                    r.Write(s);
                }
            }
                
        }
        */
        
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






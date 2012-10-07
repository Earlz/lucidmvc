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

namespace Earlz.BarelyMVC
{
	public enum HttpMethod{
		Any,
		Get,
		Put,
		Post,
		Delete,
		Head
	};
	public delegate HttpHandler HandlerInvoker();
	
	/**The routing engine of EFramework.
	 * This is a simple, but powerful router utilizing simple route pattern matching and lambdas for initializing the HttpHandler for a request.**/
	public class Router
	{
		List<Route> Routes=new List<Route>();

		/// <summary>
		/// Adds a route to the router using the given pattern type
		/// </summary>
		public void AddRoute(string id,PatternTypes type,string pattern,HandlerInvoker handler)
		{
			var r=new Route{Pattern=PatternFactory.GetPattern(type,pattern), Handler=handler, ID=id};
			Routes.Add(r);
		}
		/// <summary>
		/// Adds a route to the router
		/// </summary>
		public void AddRoute(string id,string pattern, HandlerInvoker handler)
		{
			var r=new Route{Pattern=PatternFactory.GetPattern(PatternTypes.Simple,pattern), Handler=handler, ID=id};
			Routes.Add(r);
		}
		public void AddRoute(string id, IPatternMatcher pattern, HandlerInvoker handler)
		{
			var r=new Route{Pattern=pattern, ID=id, Handler=handler};
			Routes.Add(r);
		}
		
		void DoHandler (Route r,HttpContext c,ParameterDictionary p)
		{
			HttpHandler h=r.Handler();
			h.Context=c;
			h.RouteRequest=r;
			h.Method=ConvertMethod(c.Request.HttpMethod);
			h.RouteID=r.ID;
			h.RouteParams=p;
			CallMethod(h);
		}
		/// <summary>
		/// Handles the current request
		/// </summary>
		public bool DoRoute(HttpContext c){
			foreach(var r in Routes){
				if(r.Pattern.IsMatch(c.Request.Url.AbsolutePath))
				{
					DoHandler(r, c, r.Pattern.Params);
					return true;
				}
			}
			return false;
		}

		HttpMethod ConvertMethod(string m){
			switch(m.ToUpper()){
				case "GET":
					return HttpMethod.Get;
				case "PUT":
					return HttpMethod.Put;
				case "POST":
					return HttpMethod.Post;
				case "DELETE":
					return HttpMethod.Delete;
				case "HEAD":
					return HttpMethod.Head;
				default:
					throw new ApplicationException("Cannot convert method name to a method type.");
			}
		}
		void CallMethod(HttpHandler h){
			IBarelyView view;
			bool IgnoreView=false; //wtf is this used for? 
			switch(h.Method){
				case HttpMethod.Get:
					view=h.Get();
					break;
				case HttpMethod.Delete:
					view=h.Delete();
					break;
				case HttpMethod.Post:
					view=h.Post();
					break;
				case HttpMethod.Put:
					view=h.Put();
					break;
				case HttpMethod.Head:
					view=h.Head();
					break;
				default:
					throw new ApplicationException("Cannot call appropriate method handler");
			}
			int length=0;
			var r=HttpContext.Current.Response;
			if(view!=null){
				//even if "directly-rendered", if ignoring the view, it won't really be rendered
				var s=view.RenderView();
				
				length+=s.Length;
				
				if(!IgnoreView && !view.RenderedDirectly){
					r.Write(s);
				}
			}
			if(IgnoreView){
				length+=h.ContentLength; //TODO Note this could be incorrect. We assume 1 byte per character here! Not sure how to correctly handle this?
				try{
					if(r.Headers.AllKeys.Contains("Content-Length")){
						r.Headers["Content-Length"]=length.ToString();
					}else{
						r.AddHeader("Content-Length",length.ToString());
					}
				}
				catch(PlatformNotSupportedException)
				{
					throw new NotSupportedException("HEAD requests are not supported when running under Cassini(try Mono's XSP or using IIS)");
				}
			}
				
		}
		
	}

	public enum PatternTypes{
		/**Use a Regular Expression for pattern matching. Allows the most expression.**/
		Regex, 
		/**Use a plain and exact match for the pattern.**/
		Plain, 
		/**Use the SimplePattern type. 
		 * This is the easiest pattern to use, but is also fairly expressive and allows simple parameter extraction
		 */
		Simple  
	};
	
	public class Route
	{
		public IPatternMatcher Pattern;
		public HandlerInvoker Handler;  
		public string ID;
	}
	
	
	
}






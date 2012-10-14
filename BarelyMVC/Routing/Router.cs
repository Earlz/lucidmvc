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
		/// <summary>
		/// Note you should never need to explicitly support this. If a HEAD request is made and a GET route exists, it will use the GET route
		/// </summary>
		Head
	};
	public delegate IBarelyView HandlerInvoker(ParameterDictionary route, ParameterDictionary form);
	
	/**The routing engine of EFramework.
	 * This is a simple, but powerful router utilizing simple route pattern matching and lambdas for initializing the HttpHandler for a request.**/
	public class Router
	{
		List<Route> Routes=new List<Route>();

		/// <summary>
		/// Adds a route to the router
		/// </summary>
		public void AddRoute(string id,HttpMethod method, string pattern, HandlerInvoker handler)
		{
			var r=new Route{Pattern=new SimplePattern(pattern), Invoker=handler, ID=id};
			Routes.Add(r);
		}
		public void AddRoute(string id,HttpMethod method, IPatternMatcher pattern, HandlerInvoker handler)
		{
			var r=new Route{Pattern=pattern, ID=id, Invoker=handler};
			Routes.Add(r);
		}
		
		void DoHandler (Route r,HttpContext c,ParameterDictionary p)
		{
			//HttpHandler h=r.Handler(p);
			HttpHandler.RouteRequest=r;
			HttpHandler.Method=ConvertMethod(c.Request.HttpMethod);
			HttpHandler.RawRouteParams=p;

			CallMethod(r.Invoker);
		}
		/// <summary>
		/// Handles the current request
		/// </summary>
		public bool DoRoute(HttpContext c){
			foreach(var r in Routes){
				if(r.Pattern.IsMatch(c.Request.Url.AbsolutePath))
				{
					var m=ConvertMethod(c.Request.HttpMethod);
					if(r.Method == HttpMethod.Any || m==r.Method || 
					   (r.Method==HttpMethod.Get && m==HttpMethod.Head))
					{
						DoHandler(r, c, r.Pattern.Params);
						return true;
					}
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
		void CallMethod(HandlerInvoker invoker){
			IBarelyView view=invoker(HttpHandler.RawRouteParams, HttpHandler.Form.ToParameters());
			int length=0;
			var r=HttpContext.Current.Response;
			if(view!=null){
				//even if "directly-rendered", if ignoring the view, it won't really be rendered
				var s=view.RenderView();
				
				length+=s.Length;
				
				if(!view.RenderedDirectly){
					r.Write(s);
				}
			}
				
		}
		
	}

	public class Route
	{
		public IPatternMatcher Pattern;
		public HandlerInvoker Invoker;  
		public HttpMethod Method;
		public string ID;
	}
	
	
	
}






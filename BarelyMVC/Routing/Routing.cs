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
using System.Web;
namespace Earlz.BarelyMVC
{
	/**A static helper class for use inside of Global.asax(usually)**/
	public static class Routing
	{
		public static Router Router{get{return Router;}}
		static Router router;
		/// <summary>
		/// Handles the current HttpRequest and calls the appropriate HttpHandler
		/// </summary>
		static public void DoRequest(HttpContext c,HttpApplication app){
			c.Response.ContentType="text/html"; //default
			if(c.Request.Url.AbsolutePath.Substring(0,Math.Min(c.Request.Url.AbsolutePath.Length,8))=="/static/"){
				return; //let it just serve the static files
			}
			if(router.DoRoute(c)){
				app.CompleteRequest();
			}
		}
		/// <summary>
		/// Adds a route to the router using the given pattern type
		/// </summary>
		static public void AddRoute(string id,PatternTypes type,string pattern,HandlerInvoker handler){
			/**TODO: This needs to be smart enough so that routes can not be added while routes are being parsed, else get a 
			 * "collection modified" exception from .Net. **/
			if(router==null){
				router=new Router();
			}
			router.AddRoute(id,type,pattern,handler);
		}
		/// <summary>
		/// Adds a route to the router
		/// </summary>
		static public void AddRoute(string id,string pattern,HandlerInvoker handler){
			/**TODO: This needs to be smart enough so that routes can not be added while routes are being parsed, else get a 
			 * "collection modified" exception from .Net. **/
			if(router==null){
				router=new Router();
			}
			router.AddRoute(id,pattern,handler);
		}
	}
}


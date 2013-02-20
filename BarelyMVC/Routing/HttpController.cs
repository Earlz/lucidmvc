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
using System.Collections.Generic;
using Earlz.BarelyMVC.Authentication;
using Earlz.BarelyMVC.ViewEngine;
using System.IO;
using System.Collections.Specialized;
using Earlz.BarelyMVC.Caching;


namespace Earlz.BarelyMVC
{
    /**The base class used to handle HTTP requests.
     * This class should be derived from for every different handler for HTTP requests.
     */
    public abstract class HttpController
    {
        public HttpController (RequestContext context)
        {
			Context=context.Context;
			RouteRequest=context.Route;
			RouteParams=context.RouteParams;
			CurrentRouter=context.Router;
			Cache=Router.GetCacher();
        }
		public virtual ICacheMechanism Cache
		{
			get;
			protected set;
		}
        /**Writes to the output stream**/
        public virtual void Write(string s){
			Context.Writer.Write(s);
        }
        /// <summary>
        /// The current ServerContext
        /// </summary>
        public virtual IServerContext Context
        {
			get;
			protected set;
        }
        /// <summary>
        /// The route that is currently being handled
        /// </summary>
        public virtual Route RouteRequest
        {
			get;
			protected internal set;
        }
        /// <summary>
        /// The HTTP Form NameValueCollection. This is populated during POST and PUT requests
        /// </summary>
        public ParameterDictionary Form{
            get
			{
				return Context.Form;
            }
        }
		public Router CurrentRouter
		{
			get;
			private set;
		}
        /// <summary>
        /// When using SimplePattern, this will be populated with router variables
        /// </summary>
        public ParameterDictionary RouteParams{
			get;
			protected internal set;
        }
		/*
        /// <summary>
        /// The current user logged in with FSCAuth
        /// </summary>
        public static UserData CurrentUser{
            get{
                return FSCAuth.CurrentUser;
            }
        } */
    }
	/*
	public delegate IBarelyView TestFoo<T>(T handler) where T:HttpHandler;
	public delegate T HandlerFoo<T>() where T:HttpHandler;

	public class Foo
	{
		delegate IBarelyView DoShit();
		public static void AddRoute<T>(string name, HandlerFoo<T> handler, TestFoo<T> caller) where T:HttpHandler
		{
			DoShit del=()=>
			{
				var h=handler();
				return caller(h);
			};
			tmp.Add(del);
		}
		static List<DoShit> tmp;
		static List<HandlerFoo<HttpHandler>> list;
		class TestHandler : HttpHandler
		{
			public IBarelyView FooBar(){return null;}
		}
		public static void Test()
		{
			Foo.AddRoute("", ()=> new TestHandler(), (h)=>h.FooBar());
		}
	}
	*/


}
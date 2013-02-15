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
namespace Earlz.BarelyMVC
{
    /**The base class used to handle HTTP requests.
     * This class should be derived from for every different handler for HTTP requests.
     */
    public abstract class HttpController
    {

        public static TextWriter CurrentWriter{
            get{
                if(HttpContext.Current.Items.Contains("BarelyMVC_Writer")){
                    return (TextWriter)HttpContext.Current.Items["BarelyMVC_Writer"];
                }else{
                    return HttpContext.Current.Response.Output;
                }
            }
            set{
                HttpContext.Current.Items["BarelyMVC_Writer"]=value;
            }
        }



        public HttpController (IServerContext context)
        {
        }

        /// <summary>
        /// Mostly an internal thing. Used to calculate content length for HEAD requests(does not include views that are "returned" to the router)
        /// You normally shouldn't need to touch this
        /// </summary>
        /// <value>
        /// The length of the content.
        /// </value>
        public int ContentLength
        {
            get
            {
                return HttpContext.Current.Items["BarelyMVC_ContentLength"] as int? ?? 0;
            }
            set
            {
                HttpContext.Current.Items["BarelyMVC_ContentLength"]=value;
            }
        }
        /// <summary>
        /// Handles HTTP HEAD requests
        /// The default implementation will make it so that nothing is rendered, but otherwise does a regular Get function.
        /// This is enough for most cases. Only override if you expect to obey the standard behavior of GET, but with no content. 
        /// The returned view is "rendered", but not sent to the Response stream. It is only rendered to get content-length
        /// </summary>
		/// TODO fix this somehow?
        /*public virtual IBarelyView Head(){
            CurrentWriter=null; //force 
            return Get();
        }*/
        /**Writes to the output stream**/
        public void Write(string s){
            ContentLength+=s.Length;
            if(CurrentWriter!=null){
                CurrentWriter.Write(s);
            }
        }
        public void Write(IBarelyView view){
            string s=view.RenderView();
            ContentLength+=s.Length;
            if(CurrentWriter!=null){
                CurrentWriter.Write(s);
            }
        }
        /// <summary>
        /// The current HttpContext
        /// </summary>
        public static HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }
        /// <summary>
        /// The route that is currently being handled
        /// </summary>
        public static Route RouteRequest
        {
            get
            {
                return HttpContext.Current.Items["BarelyMVC_RouteRequest"] as Route;
            }
            internal set
            {
                HttpContext.Current.Items["BarelyMVC_RouteRequest"]=value;
            }
        }
        /// <summary>
        /// The current HttpRequest being handled
        /// </summary>
        public static HttpRequest Request{
            get{
                return Context.Request;
            }
        }
        /// <summary>
        /// The current HttpResponse being written to
        /// </summary>
        public static HttpResponse Response{
            get{
                return Context.Response;
            }
        }
        /// <summary>
        /// The current HTTP Method
        /// </summary>
        public static HttpMethod Method
        {
            get
            {
                return (HttpMethod) HttpContext.Current.Items["BarelyMVC_RouteMethod"];
            }
            internal set
            {
                HttpContext.Current.Items["BarelyMVC_RouteMethod"]=value;
            }

        }
        /// <summary>
        /// The HTTP Form NameValueCollection. This is populated during POST and PUT requests
        /// </summary>
        public static System.Collections.Specialized.NameValueCollection Form{
            get{
                return Request.Form;
            }
        }
        /// <summary>
        /// When using SimplePattern, this will be populated with router variables
        /// </summary>
        public static ParameterDictionary RawRouteParams{
            get{
                return (ParameterDictionary) HttpContext.Current.Items["BarelyMVC_RawRouteParams"];
            }
            internal set{
                HttpContext.Current.Items["BarelyMVC_RawRouteParams"]=value;
            }
        }
        /// <summary>
        /// The current user logged in with FSCAuth
        /// </summary>
        public static UserData CurrentUser{
            get{
                return FSCAuth.CurrentUser;
            }
        }
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
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
	public abstract class HttpHandler
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
		public HttpHandler ()
		{
		}
		/**Handles the HTTP GET method**/
		public virtual IBarelyView Get(){
			throw new NotImplementedException();
		}
		/**Handles the HTTP POST method**/
		public virtual IBarelyView Post(){
			throw new NotImplementedException();
		}
		/**Handles the HTTP PUT method**/
		public virtual IBarelyView Put(){
			throw new NotImplementedException();
		}
		/**Handles the HTTP DELETE method**/
		public virtual IBarelyView Delete(){
			throw new NotImplementedException();
		}
		public virtual IBarelyView Error(){
			throw new NotImplementedException();
		}
		/// <summary>
		/// Mostly an internal thing. Used to calculate content length for HEAD requests(does not include views that are "returned" to the router)
		/// </summary>
		/// <value>
		/// The length of the content.
		/// </value>
		public int ContentLength{get; protected set;}
		/// <summary>
		/// The default implementation will make it so that nothing is rendered, but otherwise does a regular Get function.
		/// This is enough for most cases. Only override if you expect to obey the standard behavior of GET, but with no content. 
		/// The returned view is "rendered", but not sent to the Response stream. It is only rendered to get content-length
		/// </summary>
		public virtual IBarelyView Head(){
			CurrentWriter=null; //force 
			return Get();
		}
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
		/**The current HttpContext**/
		public HttpContext Context{get;set;}
		/**The route that handled the request.**/
		public Route RouteRequest{get;set;}
		/**A shortcut for Context.Request**/
		public HttpRequest Request{
			get{
				return Context.Request;
			}
		}
		/**A shortcut for Context.Response**/
		public HttpResponse Response{
			get{
				return Context.Response;
			}
		}
		/**The current HttpMethod for the request**/
		public HttpMethod Method{get;set;}
		public System.Collections.Specialized.NameValueCollection Form{
			get{
				return Request.Form;
			}
		}
		ParameterDictionary routeparams=null;
		/**When using SimplePattern, this is populated with the route parameters.**/
		public ParameterDictionary RouteParams{
			get{
				return routeparams;
			}
			set{
				if(routeparams!=null){
					throw new ArgumentException("RouteParams is already set.","value");
				}
				routeparams=value;
			}
		}
		/**The route's id that handled the current request**/
		public string RouteID{get;set;}
		
		public UserData CurrentUser{
			get{
				return FSCAuth.CurrentUser;
			}
		}
	}
}
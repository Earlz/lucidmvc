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
namespace Earlz.BarelyMVC
{
	/**The base class used to handle HTTP requests.
	 * This class should be derived from for every different handler for HTTP requests.
	 */
	public abstract class HttpHandler
	{
		public HttpHandler ()
		{
		}
		/**Handles the HTTP GET method**/
		public virtual void Get(){
			throw new NotImplementedException();
		}
		/**Handles the HTTP POST method**/
		public virtual void Post(){
			throw new NotImplementedException();
		}
		/**Handles the HTTP PUT method**/
		public virtual void Put(){
			throw new NotImplementedException();
		}
		/**Handles the HTTP DELETE method**/
		public virtual void Delete(){
			throw new NotImplementedException();
		}
		/**Writes to the output stream**/
		public void Write(string s){
			Response.Write(s);
		}
		public void Write(IBarelyView view){
			Response.Write(view.RenderView());
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
		
		public IUserStore CurrentUser{
			get{
				return FSCAuth.UserStore;
			}
		}
	}
}
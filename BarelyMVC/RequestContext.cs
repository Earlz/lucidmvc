using System;
using System.Collections.Generic;
using System.IO;
using Earlz.BarelyMVC.Authentication;
using System.Collections.Specialized;
using System.Text;

namespace Earlz.BarelyMVC
{
	public abstract class RequestContext
	{
		static public RequestContext Current
		{
			get
			{
				return CurrentRequest.Request;
			}
		}
		public RequestContext ()
		{
		}
		public virtual NameValueCollection Form
		{
			get;set;
		}
		public virtual UserData CurrentUser
		{
			get
			{
				return FSCAuth.CurrentUser;
			}
		}
		public virtual NameValueCollection QueryString{get; set;}
		public string RawUrl{get; set;}
		public string RawRequest{get; set;}
		public System.Web.HttpBrowserCapabilities BrowserCapability{get;set;}
		public HttpRequest Request{get;set;}
		public HttpResponse Response{get;set;}
		/*
		 * Notable exclusions:
		 * AcceptTypes : Basically useless
		 * ClientCertificate : Not reliable on IIS and I'm sure there is a better built-in way for other servers
		 */
		public RequestStore Items{get;set;}

	}

	public class HttpResponse
	{
		public virtual bool OutputBuffering{get;set;}
		public TextWriter Output{get;set;}

		public virtual Encoding ContentEncoding{get;set;}
		public virtual NameValueCollection Cookies{get;set;}
		public virtual int ContentLength{get;set;}
		public virtual NameValueCollection Headers{get;set;}
		public virtual int StatusCode{get;set;}
		public virtual string Status{get;set;}
		public virtual Stream OutputStream{get;set;}
		public virtual string ContentType{get;set;}

		public virtual void AddHeader(string name, string value)
		{
			Headers.Add(name,value);
		}


	}
	public class HttpRequest
	{
		public TextWriter Input{get;set;}
		public virtual Stream InputStream{get;set;}
		public virtual int ContentLength{get;set;}
		public virtual Encoding ContentEncoding{get;set;}
		public virtual NameValueCollection Form{get;set;}
		public virtual NameValueCollection Cookies{get;set;}
		public virtual NameValueCollection Headers{get;set;}
	}

}


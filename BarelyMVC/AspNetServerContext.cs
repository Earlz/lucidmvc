using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Earlz.BarelyMVC
{
	public class AspNetServerContext : IServerContext
	{
		public string HttpMethod {
			get
			{
				return Current.Request.HttpMethod;
			}
		}

		public string ContentType {
			get
			{
				return Current.Response.ContentType;
			}
			set
			{
				Current.Response.ContentType=value;
			}
		}


		ParameterDictionary formcache;
		public ParameterDictionary Form {
			get
			{
				if(formcache==null)
				{
					formcache=Current.Request.Form.ToParameters();
				}
				return formcache;
			}
		}

		public Uri RequestUrl {
			get
			{
				return Current.Request.Url;
			}
		}


		public HttpContext Current{
			get
			{
				return HttpContext.Current;
			}
		}
		public object GetItem (string name)
		{
			if(Current.Items.Contains(name))
			{
				return Current.Items[name];
			}
			else
			{
				return null;
			}
		}
		public object SetItem (string name, object value)
		{
			object tmp=GetItem(name);
			Current.Items[name]=value;
			return tmp;
		}

		public HttpCookie GetCookie (string name)
		{
			return Current.Request.Cookies[name];
		}

		public void SetCookie (HttpCookie cookie)
		{
			Current.Response.SetCookie(cookie);
		}

		public void KillIt ()
		{
			Current.ApplicationInstance.KillIt();
		}

		public string MapPath (string path)
		{
			return Current.Server.MapPath(path);
		}

		public IList<string> GetHeaders(string name)
		{
			return Current.Request.Headers.GetValues(name).ToList();;
		}

		public void SetHeader (string name, string value)
		{
			Current.Response.AddHeader(name, value);
		}

		public void Transfer (string url)
		{
			Current.Server.Transfer(url);
			KillIt();
		}

		public void Redirect (string url)
		{
			Current.Response.Redirect(url);
			KillIt();
		}

		public string UserAgent {
			get {
				return Current.Request.UserAgent;
			}
		}

		public string UserIP {
			get {
				return Current.Request.UserHostAddress;
			}
		}

		public string HttpStatus {
			get {
				return Current.Response.Status;
			}
			set {
				Current.Response.Status=value;
			}
		}
		public TextWriter Writer
		{
			get
			{
				return HttpContext.Current.Response.Output;
			}
		}

		public AspNetServerContext ()
		{
		}
	}
}


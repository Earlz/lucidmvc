using System;
using Earlz.BarelyMVC;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Earlz.BarelyMVC.Tests
{
	public class FakeServerKilledException : Exception
	{
		public FakeServerKilledException(){}
	}
	public class FakeServerContext : IServerContext
	{
		public List<HttpCookie> RequestCookies=new List<HttpCookie>();
		public HttpCookie GetCookie (string name)
		{
			return RequestCookies.SingleOrDefault(x=>x.Name==name);
		}
		public List<HttpCookie> ResponseCookies=new List<HttpCookie>();
		public void SetCookie (HttpCookie cookie)
		{
			ResponseCookies.Add(cookie);
		}

		public void KillIt ()
		{
			throw new FakeServerKilledException();
		}

		public string MapPath (string path)
		{
			throw new NotImplementedException ();
		}
		public Dictionary<string, string> RequestHeaders=new Dictionary<string, string>();
		public IList<string> GetHeaders(string name)
		{
			if(!RequestHeaders.ContainsKey(name))
			{
				return new string[]{};
			}
			return new string[]{RequestHeaders[name]};
		}
		public Dictionary<string, string> ResponseHeaders=new Dictionary<string, string>();
		public void SetHeader (string name, string value)
		{
			ResponseHeaders.Add(name, value);
		}
		public string TransferredTo;
		public void Transfer (string url)
		{
			TransferredTo=url;
			KillIt();
		}
		public string RedirectedTo;
		public void Redirect (string url)
		{
			RedirectedTo=url;
			KillIt();
		}
		public Dictionary<string, object> Items=new Dictionary<string, object>();
		public object GetItem (string name)
		{
			if(!Items.ContainsKey(name))
			{
				return null;
			}
			return Items[name];
		}

		public object SetItem (string name, object value)
		{
			return Items[name]=value;
		}

		public string UserAgent {
			get;
			set;
		}

		public string UserIP {
			get;
			set;
		}

		public string HttpStatus {
			get;
			set;
		}

		public TextWriter Writer {
			get;
			set;
		}

		public ParameterDictionary Form {
			get;
			set;
		}

		public Uri RequestUrl {
			get;
			set;
		}

		public string ContentType {
			get;
			set;
		}

		public string HttpMethod {
			get;
			set;
		}
		public string WrittenText()
		{
			return ((StringWriter)Writer).GetStringBuilder().ToString();
		}

		public FakeServerContext ()
		{
			Writer=new StringWriter();
		}
	}
}


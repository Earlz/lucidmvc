using System;
using Earlz.BarelyMVC;
using System.Web;
using System.IO;
using System.Collections.Generic;

namespace Earlz.BarelyMVC.Tests
{
	public class FakeServerContext : IServerContext
	{

		public HttpCookie GetCookie (string name)
		{
			throw new NotImplementedException ();
		}

		public void SetCookie (HttpCookie cookie)
		{
			throw new NotImplementedException ();
		}

		public void KillIt ()
		{
			throw new NotImplementedException ();
		}

		public string MapPath (string path)
		{
			throw new NotImplementedException ();
		}

		public IList<string> GetHeaders(string name)
		{
			throw new NotImplementedException ();
		}

		public void SetHeader (string name, string value)
		{
			throw new NotImplementedException ();
		}

		public void Transfer (string url)
		{
			throw new NotImplementedException ();
		}

		public void Redirect (string url)
		{
			throw new NotImplementedException ();
		}

		public object GetItem (string name)
		{
			throw new NotImplementedException ();
		}

		public object SetItem (string name, object value)
		{
			throw new NotImplementedException ();
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


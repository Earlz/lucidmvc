using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Earlz.BarelyMVC.TestingUtilities
{
	/*
	/// <summary>
	/// This is a mock IServerContext. This is to make unit testing easier on your fingers 
	/// Get/Set Item/cookie/Header stuff just passes through to dictionaries. Properties include set methods so you can set them manually if needed
	/// </summary>
	public class MockServerContext : IServerContext
	{

		public ParameterDictionary Form {
			get;
			set;
		}

		public Uri RequestUrl {
			get;
			set;
		}


		public List<HttpCookie> InputCookies=new List<HttpCookie>();
		public HttpCookie GetCookie (string name)
		{
			throw new NotImplementedException();
		}
		public List<HttpCookie> OutputCookies=new List<HttpCookie>();
		public void SetCookie (HttpCookie cookie)
		{
			throw new NotImplementedException ();
		}

		public void KillIt ()
		{
			throw new StopExecutionException();
		}

		public string MapPath (string path)
		{
			return "/foo/bar/"+path;
		}
		public List<KeyValuePair<string, string>> InputHeaders=new List<KeyValuePair<string, string>>();
		public string GetHeader (string name)
		{
			throw new NotImplementedException ();
		}
		public List<KeyValuePair<string, string>> OutputHeaders=new List<KeyValuePair<string, string>>();
		public void SetHeader (string name, string value)
		{
			throw new NotImplementedException ();
		}

		public void Transfer (string url)
		{
			throw new StopExecutionException();
		}

		public void Redirect (string url)
		{
			throw new StopExecutionException();
		}
		public ConcurrentDictionary<string, object> Items=new ConcurrentDictionary<string, object>();
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

		public MockServerContext ()
		{
		}
	}
	*/
}


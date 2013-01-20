using System;
using System.Web;
using System.Collections.Generic;

namespace Earlz.BarelyMVC
{
	public interface IServerContext
	{
		HttpCookie GetCookie(string name);
		void AddCookie(HttpCookie cookie);
		void KillIt();
		string UserAgent{get;}
		string MapPath(string path);
		string UserIP{get;}
		string GetHeader(string name);
		void AddHeader(string name, string value);
		void Transfer(string url);
		void Redirect(string url);
		object GetItem(string name);
		object SetItem(string name, object value);
		string HttpStatus{get;set;}
	}
}


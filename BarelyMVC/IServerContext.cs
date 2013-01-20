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
		void Redirect(string url);
		IDictionary<string, object> Items{get;}
	}
}


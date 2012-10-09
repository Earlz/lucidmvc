using System;

namespace Earlz.BarelyMVC
{
	public class RedirectHandler : HttpHandler
	{
		string Url;
		bool Permanent;
		/// <summary>
		/// Sends a redirect to the specified URL. 
		/// If permanent is true, will send a 301 permanent redirect rather than a 302
		/// </summary>
		public RedirectHandler (bool permanent, string url)
		{
			Url=url;
			Permanent=permanent;
		}
		public override Earlz.BarelyMVC.ViewEngine.IBarelyView Get()
		{
			if(Permanent)
			{
				//no support for 301s built into ASP.Net
				Response.Status = "301 Moved Permanently";
				Response.AddHeader("Location",Url);
				Context.ApplicationInstance.CompleteRequest();
			}
			else
			{
				Response.Redirect(Url, true);
			}
			return null;
		}
	}
}


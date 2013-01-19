using System;
using System.Web;
using System.Threading;

namespace Earlz.BarelyMVC.Extensions
{
	static public class ExtensionMethods
	{
		/// <summary>
		/// Kills the damn response! This is harder than you'd think. 
		/// This ensures that nothing will execute past this point
		/// </summary>
		/// <param name='app'>
		/// App.
		/// </param>
		public static void KillIt(this HttpApplication app)
		{
			app.CompleteRequest();
			app.Response.End();
			while(true) Thread.Sleep(1000); //hang until the garbage collector kills us
			throw new StopExecutionException(); //just in case... 
		}
	}
}


using System;
using System.Web;
using System.Threading;
using System.Linq;
using System.Collections.Specialized;

namespace Earlz.BarelyMVC
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
		public static string GetHeader(this IServerContext c, string name)
		{
			var v=c.GetHeaders(name); //TODO what happens if there is more than one header here? 
			if(v!=null)
			{
				return v.SingleOrDefault();
			}
			return null;
		}

        public static ParameterDictionary ToParameters(this NameValueCollection c)
        {
            var p=new ParameterDictionary();
            foreach(string key in c.Keys)
            {
                if(key!=null)
                {
                    p.Add(key, c.GetValues(key).ToList()); //use GetValues because it could be more than 1
                }
            }
            return p;
        }
	}
	static internal class InternalExtensions
	{
		public static T GetInstance<T>(string type)
		{
    		return (T)Activator.CreateInstance(Type.GetType(type));
		}
	}
}


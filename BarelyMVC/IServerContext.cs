using System;
using System.Web;
using System.Collections.Generic;
using System.IO;

namespace Earlz.BarelyMVC
{
	/// <summary>
	/// This is a very simplified context for server requests and responses
	/// It should not cover "everything", but should instead have only what BarelyMVC needs to function
	/// As a result, it will usually only have the most commonly used things in it
	/// All "get" methods should normally return null if a given name/key doesn't exist
	/// </summary>
	public interface IServerContext
	{
		/// <summary>
		/// Gets a cookie from the current request
		/// </summary>
		HttpCookie GetCookie(string name);
		/// <summary>
		/// Sets a cookie to send with the current response
		/// </summary>
		void SetCookie(HttpCookie cookie);
		/// <summary>
		/// Kills the current request. Kills it with fire! This function should never return(for security purposes)
		/// </summary>
		void KillIt();
		/// <summary>
		/// Gets the user agent sent by the current request
		/// </summary>
		string UserAgent{get;}
		/// <summary>
		/// Maps a relative URL path to an absolute path on the host (is this needed?)
		/// </summary>
		string MapPath(string path);
		/// <summary>
		/// Gets the client's IP address
		/// </summary>
		string UserIP{get;}
		/// <summary>
		/// Gets an HTTP header by name from the current request
		/// </summary>
		string GetHeader(string name);
		/// <summary>
		/// Sets an HTTP header to be sent back in the response to the current request
		/// </summary>
		void SetHeader(string name, string value);
		/// <summary>
		/// Performs a server-side transfer to another URL to service the current request (is this needed?)
		/// Should perform a KillIt() afterwards and never return
		/// </summary>
		void Transfer(string url);
		/// <summary>
		/// Send a 302 redirect back to the client for the given URL and use KillIt() to end the request
		/// </summary>
		void Redirect(string url);
		/// <summary>
		/// Gets a generic item from a key/value store associated with the current request only
		/// This is needed because [ThreadStatic] isn't a safe thing to assume will actually "work"
		/// </summary>
		object GetItem(string name);
		/// <summary>
		/// Sets a generic item to a key/value store associated with the current request only
		/// This is needed because [ThreadStatic] isn't a safe thing to assume will actually "work"
		/// </summary>
		object SetItem(string name, object value);
		/// <summary>
		/// Gets or sets the HTTP status code
		/// </summary>
		string HttpStatus{get;set;}
		/// <summary>
		/// Gets a TextWriter which can be used to send content back as the response
		/// </summary>
		TextWriter Writer{get;}
		/// <summary>
		/// Gets a parameter dictionary corresponding to the FORM values passed in by a POST request
		/// </summary>
		ParameterDictionary Form{get;}
		/// <summary>
		/// Gets the URL of the current request
		/// </summary>
		Uri RequestUrl{get;}
		/// <summary>
		/// The HTTP Content-Type header value to be sent back with the response
		/// </summary>
		string ContentType{get;set;}
		/// <summary>
		/// The HTTP method for the request
		/// </summary>
		string RawHttpMethod{get;}
	}
	public static class ServerContextExtensions
	{
		public static HttpMethod SaneHttpMethod(this IServerContext c)
		{
			string m=c.RawHttpMethod;
            switch(m.ToUpper()){
                case "GET":
                    return HttpMethod.Get;
                case "PUT":
                    return HttpMethod.Put;
                case "POST":
                    return HttpMethod.Post;
                case "DELETE":
                    return HttpMethod.Delete;
                case "HEAD":
                    return HttpMethod.Head;
                default:
					throw new HttpException(405, "Method not allowed");
            }
		}
	}
}


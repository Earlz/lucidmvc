using System;

namespace Earlz.BarelyMVC.Authentication
{
    public delegate HashWithSalt HashInvoker(string plain, string salt);
    public class FSCAuthConfig
    {
        /// <summary>
        /// Sets where anonymous users should be redirected to when trying to access a protected resource.
        /// If a generic 401 Access Denied response should be given instead, then set to null.
        /// </summary>
        public string LoginPage = null;    
        /// <summary>
        /// Set the HttpOnly property for login cookies
        /// </summary>
        public bool CookieHttpOnly=true;    
        /// <summary>
        /// Use browser info in constructing the login cookie hash
        /// </summary>
        public bool CookieUseBrowserInfo=true;    
        /// <summary>
        /// Tie each login cookie to the client's IP address
        /// </summary>
        public bool CookieUseIP=true;
        /// <summary>
        /// Hash the the physical path of URL / in login cookies. There is no reason to turn this off unless you are operating in a cluster where / may be different for each node
        /// </summary>
        public bool CookieUseBase=true;
        /// <summary>
        /// This is to provide redundant-ish login security. This is also put in login cookie hashes and password hashes.
        /// Just fill it in with some random string of your choice. Do not use a word or password. Use a purely random string of moderate length.
        /// I recommend using random.org to generate a string, or a GUID generator. If this is not set from code, it will attempt to read it from the web.config.
        /// If this value can not be populated from one of those two methods, an exception will be thrown in Authenticate.
        /// </summary>
		public string UniqueHash;            
        /// <summary>
        /// If true, then the cookie will only be transmitted over HTTPS.
        /// </summary>
        public bool CookieSecure=false;
        
        /// <summary>
        ///The length of the salt-string to be used(in characters)
        /// </summary>
		public int SaltLength=16;

        /// <summary>
        /// This calls a delegate which will create a hash using a given input and salt. By default, it uses SHA256 for this.
        /// For an example of how to  implement a new Hash algorithm, please see the FAQ document or the `DefaultHasher` implementation
        /// </summary>
		public HashInvoker HasherInvoker=null;

        /// <summary>
        /// How many iterations the hashing function should be used. 
        /// The default is 1. If storing more secretive information, you may wish to increase this. Note that too high of a number may make your website slow
        /// </summary>
		public int HashIterations=1;
	}
}


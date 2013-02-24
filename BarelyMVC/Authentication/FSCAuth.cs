/*
Copyright (c) 2010 - 2012 Jordan "Earlz/hckr83" Earls  <http://lastyearswishes.com>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.
   
THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL
THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Web.Security;
using System.Security;

namespace Earlz.BarelyMVC.Authentication
{
    /// <summary>
    /// The main FSCAuth class. It is purely static.
    /// </summary>
    /// <remarks>
    /// To opt-in for authentication, all that is needed is an IUserStore to be made and SiteName initialized.
    /// </remarks>
    public class FSCAuth : IAuthMechanism
    {
		public FSCAuth(IServerContext server, FSCAuthConfig config, IUserStore store, bool defaultbasic=false)
		{
			Server=server;
			Config=config;
			if(Config.HasherInvoker==null)
			{
				Config.HasherInvoker=DefaultHasher;
			}
			BasicByDefault=defaultbasic;
			UserStore=store;
		}

		bool BasicByDefault=false;
		public void RequiresAuthentication ()
		{
            if(IsAuthenticated){
                return;
			}

            if(BasicByDefault){
                SendHttp401();
            }
            if (string.IsNullOrEmpty(Config.LoginPage))
            {
                throw new HttpException(401, "You must be authenticated to access this resource");
            }
            else
            {
                Server.Redirect(Config.LoginPage); //this will cause an exception, but the redirect will work fine
				Server.KillIt();
            }
		}

		public bool Login (string username, string password, DateTime expiration)
		{
            var user = UserStore.GetUserByName(username);
            if (user == null)
            {
                return false;
            }
            string hash = PasswordHash(user,password).Text;
            if (hash == user.PasswordHash)
            {
                LoginFromHash(user, expiration);
                return true;
            }
            else
            {
                return false;
            }
		}
		public bool Login (string username, string password, TimeSpan expires, DateTime? timeout)
		{
			throw new NotSupportedException();
		}
		public bool Login (string username, string password)
		{
            var user = UserStore.GetUserByName(username);
            if (user == null)
            {
                return false;
            }
            string hash = PasswordHash(user,password).Text;
            if (hash == user.PasswordHash)
            {
                LoginFromHash(user, null);
                return true;
            }
            else
            {
                return false;
            }
		}
		public FSCAuthConfig Config{get; private set;}
		public IServerContext Server{get;private set;}
		static bool SupportsUnmanagedCrypto;
static FSCAuth(){
            try
            {
                SupportsUnmanagedCrypto=true;
                new SHA256CryptoServiceProvider();
            }
            catch (PlatformNotSupportedException)
            { //Windows XP doesn't support native
                SupportsUnmanagedCrypto=false;
            }
        }
        HashWithSalt DefaultHasher(string plain, string salt)
        {
            var v=new HashWithSalt();
            if(salt==null){
                v.Salt=HashHelper.GetSalt(Config.SaltLength);
            }else{
                v.Salt=salt;
            }
            HashAlgorithm hash;
            if(SupportsUnmanagedCrypto){
                hash=new SHA256CryptoServiceProvider();
            }else{
                hash=new SHA256Managed();
            }
            v.Text=HashHelper.FromBytes(hash.ComputeHash(HashHelper.ToRawBytes(plain+v.Salt)));
            return v;
        }
        
        
        /// <summary>
        /// The IUserStore to use. This must be populated before Authenticate or anything else can be used in this class.
        /// </summary>
		public IUserStore UserStore{get;private set;}

		bool formlogin;
        /// <summary>
        /// Is true if the current user was authenticated with Forms Authentication(cookie based).
        /// If false, then no user is logged in, or they were logged in with HTTP Basic Authentication.
        /// </summary>
        public bool FormLogin
        {
            get
            {
				Authenticate();
				return formlogin;
            }
            private set
            {
				formlogin=value;
            }
        }
		UserData currentuser;
		bool TriedToAuthenticate=false;
        /// <summary>
        /// The current user logged in for the HTTP request. If there is not a user logged in, this will be null.
        /// </summary>
        public UserData CurrentUser{
            get
			{
				if(currentuser==null)
				{
					Authenticate();
				}
				return currentuser;
                
            }
            set
			{
				currentuser=value;
            }
        }
		public bool IsAuthenticated{get { return CurrentUser!=null; }}
		/// <summary>
		/// Returns true if a user is "probably" logged in. This is true when a valid cookie or HttpAuthentication is used(but not verified)
		/// DO NOT USE THIS TO SAFE-GUARD ANYTHING IMPORTANT! This is only intended to be used in things such as views
		/// where you'd want to display a link to a control panel or some such.
		/// The purpose of this is that this will not try to look up the user in the database, making it so that
		/// you can display aesthetic items such as profile or administration links on non-authenticated pages which don't hide anything important
		/// </summary>
		/// <value>
		/// <c>true</c> if probably logged in; otherwise, <c>false</c>.
		/// </value>
		public bool ProbablyLoggedIn
		{
			get
			{
				return ProbableUserName!=null;
			}
		}

		string probableusername;
		bool TriedProbableLogin=false;

		/// <summary>
		/// Returns the name of the person "probably" logged in. This is true when a valid cookie or HttpAuthentication is used(but not verified)
		/// DO NOT USE THIS TO SAFE-GUARD ANYTHING IMPORTANT! This is only intended to be used in things such as views
		/// where you'd want to display a link to a control panel or some such.
		/// The purpose of this is that this will not try to look up the user in the database, so that
		/// you can display aesthetic items such as profile or administration links on non-authenticated pages which don't hide anything important
		/// </summary>
		/// <value>
		/// The name of the probable user.
		/// </value>
		public string ProbableUserName
		{
			get
			{
				if(currentuser!=null) //if we're already authenticated, don't bother with this
				{
					return currentuser.Username;
				}
				if(probableusername==null)
				{
					return ProbableUserName=ProbablyLoggedInName();
				}
				return probableusername;
			}
			private set
			{
				probableusername=value;
			}
		}
		/// <summary>
		/// Will lookup who is "probably" looked in without hitting the database. 
		/// This is not secure and should not guard anything important
		/// </summary>
		/// <returns>
		/// The logged in name.
		/// </returns>
		string ProbablyLoggedInName()
		{
			if(TriedProbableLogin)
			{
				return probableusername;
			}
			TriedProbableLogin=true;
			string username=null;
			string authHeader=Server.GetHeader("Authorization");
			if(Config.SiteName!=null && string.IsNullOrEmpty(authHeader))
			{
                string userNameAndPassword = Encoding.Default.GetString(Convert.FromBase64String(authHeader.Substring(6)));
                string[] parts = userNameAndPassword.Split(':');
                return parts[0];
			}
			var cookie=Server.GetCookie(Config.SiteName+"_login");
			if(cookie==null)
			{
				return null;
			}
            DateTime expires=ConvertFromUnixTimestamp(long.Parse(cookie["expire"]));
            if(expires<DateTime.Now){
                ForceCookieExpiration();
                return null;
            }
			if(string.IsNullOrEmpty(cookie["secret"]))
			{
				return null;
			}
			return cookie["name"];
		}


        /// <summary>
        /// This will check for a login cookie and check that it is valid. It will then assign CurrentUser.
        /// </summary>
        /// <exception cref="ArgumentException">Throws if UniqueHash is not complete </exception>
        public void Authenticate(){
			if(TriedToAuthenticate)
			{
				return;
			}
			TriedToAuthenticate=true;
            if(Config.UniqueHash==null){
                throw new ArgumentException("You MUST fill in UniqueHash before using the AuthenticationModule!");
            }
            string username;
            string hash;
            string password;
            UserData user;
            string authHeader = Server.GetHeader("Authorization");
            if(Config.SiteName!=null && !string.IsNullOrEmpty(authHeader)){ //try HTTP basic auth
                string userNameAndPassword = Encoding.Default.GetString(Convert.FromBase64String(authHeader.Substring(6)));
                string[] parts = userNameAndPassword.Split(':');
                username=parts[0];
                password=parts[1];
                user=UserStore.GetUserByName(username);
                if(user!=null){
                    hash=PasswordHash(user,password).Text;
                    if(hash==user.PasswordHash){
                        CurrentUser=user;
                        FormLogin = false;
                        return;
                    }else{
                        CurrentUser=null; //just drop through and try cookies instead.
                    }
                }
            }
            //try forms/cookie auth
            HttpCookie cookie=Server.GetCookie(Config.SiteName+"_login");
            if(cookie==null){
                return;
            }
            username=cookie.Values["name"];
            username=username.Trim();
            hash=cookie.Values["secret"];
            user=UserStore.GetUserByName(username);
            if(user==null){ //user wasn't found in the database
                ForceCookieExpiration();
                return;
            }
            DateTime expires=ConvertFromUnixTimestamp(long.Parse(cookie["expire"]));
            string hash2=ComputeLoginHash(user.PasswordHash,user.Salt,expires);
            if(expires<DateTime.Now){
                ForceCookieExpiration();
                return;
            }
            if(hash==hash2){
                CurrentUser=user;
                FormLogin = true;
                return;
            }else{
                CurrentUser=null;
                ForceCookieExpiration();
                return;
            }
            
        }
        /// <summary>
        /// Will delete the current login cookie, if it exists.
        /// </summary>
        public void Logout(){
            if (FormLogin==false)
            {
                throw new NotSupportedException("Can not log out a user logged in using HTTP Basic Authentication");
            }
            var c=Server.GetCookie(Config.SiteName+"_login");
            if(c!=null){
                ForceCookieExpiration();
            }
        }

		HashWithSalt PasswordHash(UserData user, string password)
		{
            /**Note: The way this hash is computed can not be changed without breaking all password hashes.
             * DO NOT TOUCH unless absolutely needed!. We do not use SiteName here because UniqueID is already a salt, and
             * SiteName could change in the future, so don't want to limit that flexibility. We DO use UniqueHash however.
             * It is also used in generating login cookies. This makes it so if you wanted, you could publish your user database and you're still 99.9% protected
             * from hackers figuring out someone's password, or even enough to figure out enough to login.
             */
            string text="fscauth"+password+user.UniqueID+Config.UniqueHash;
            if(user.Salt==null){
                var v=NewHash(text);
				return v;
            }else{
				return new HashWithSalt(){Text=ComputeHash(text, user.Salt), Salt=user.Salt};
            }
		}

        /// <summary>
        /// Computes a password hash. 
        /// </summary>
        /// <param name="password">The plain text password</param>
        /// <param name="username">The user's username</param>
        /// <param name="uniqueid">The user's unique ID</param>
        /// <returns></returns>
        public void ComputePasswordHash(UserData user, string password)
        {
			user.Salt=null;
			var v=PasswordHash(user, password);
			user.PasswordHash=v.Text;
			user.Salt=v.Salt;
        }


        private void LoginFromHash(UserData user, DateTime? expires)
        {
            var c = new HttpCookie(Config.SiteName + "_login");
            if (expires.HasValue)
            {
                c.Expires = (DateTime)expires;
            }
            else
            {
                expires = DateTime.Now.AddHours(4);
            }
            c.HttpOnly = Config.CookieHttpOnly;
            c.Secure = Config.CookieSecure;
            c.Values["name"] = user.Username;
            c.Values["secret"] = ComputeLoginHash(user.PasswordHash,user.Salt, expires.Value);
            c.Values["expire"] = ConvertToUnixTimestamp(expires.Value).ToString();
            Server.SetCookie(c);
            CurrentUser = user;
        }

        string ComputeLoginHash(string passwordhash,string salt,DateTime expires){
            StringBuilder sb=new StringBuilder();
            sb.Append(passwordhash);
            if(Config.CookieUseIP){
                sb.Append(Server.UserIP);
            }
            if(Config.CookieUseBrowserInfo){
                sb.Append(Server.UserAgent);
            }
            sb.Append(ConvertToUnixTimestamp(expires).ToString());
            sb.Append(Config.UniqueHash);
            sb.Append(Config.SiteName);
            Debug.Assert(salt!=null);
            return ComputeHash(sb.ToString(),salt);
        }
        /// <summary>
        /// Computes a hash using an existing salt 
        /// </summary>
        string ComputeHash(string input, string salt)
        {
            Debug.Assert(salt != null);
            HashWithSalt v=new HashWithSalt();
            v.Text=input;
            v.Salt=salt;
            v=Config.HasherInvoker(v.Text,v.Salt);
            return v.Text;
        }
        /// <summary>
        /// Computes a new hash and gets a new salt
        /// </summary>
        HashWithSalt NewHash(string input)
        {
            HashWithSalt v=new HashWithSalt();
            v.Text=input;
            v.Salt=null;
            v=Config.HasherInvoker(v.Text,v.Salt);
            return v;
        }
       
        void ForceCookieExpiration(){
            var tmp=new HttpCookie(Config.SiteName+"_login");
            tmp.Expires=DateTime.Now.AddYears(-10); //force expiration
            HttpContext.Current.Response.Cookies.Add(tmp);
        }
        static string GetUniqueHash()
        {
            return ConfigurationManager.AppSettings["FSCAuth_UniqueHash"];
        }
        
        static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return (long)Math.Floor(diff.TotalSeconds);
        }
        static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(Convert.ToDouble(timestamp));
        }
        
        void SendHttp401(){ //this will directly write the error, rather than throwing an exception. 
            Server.HttpStatus="401 Not Authenticated";
            if(Config.SiteName!=null){
                Server.SetHeader("WWW-Authenticate", "Basic Realm=\""+Config.SiteName+"\"");
            }
            try
            {
                Server.Transfer(CustomErrorsFixer.GetCustomError("401")); //output the 401 page
            }
            catch(SecurityException) //running under medium trust
            {
                Server.Transfer(Config.LoginPage);
            }
			Server.KillIt();
        }
        
    }
                    
}


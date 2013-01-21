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
using Earlz.BarelyMVC.Extensions;

namespace Earlz.BarelyMVC.Authentication
{
    /// <summary>
    /// The main FSCAuth class. It is purely static.
    /// </summary>
    /// <remarks>
    /// To opt-in for authentication, all that is needed is an IUserStore to be made and SiteName initialized.
    /// </remarks>
    static public class FSCAuth
    {
        public delegate HashWithSalt HashInvoker(string plain, string salt);
        static FSCAuth(){
            if(Config.UniqueHash==null) //must do a null check because references to Config can be done before this static constructor executes
			{
				Config.UniqueHash=GetUniqueHash();
			}
            Config.HashIterations=1;
            Config.SaltLength=16;
            try
            {
                SupportsUnmanagedCrypto=true;
                new SHA256CryptoServiceProvider();
            }
            catch (PlatformNotSupportedException)
            { //Windows XP doesn't support native
                SupportsUnmanagedCrypto=false;
            }
            Config.HasherInvoker=DefaultHasher;
        }
        static bool SupportsUnmanagedCrypto;
        static HashWithSalt DefaultHasher(string plain, string salt)
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
        static public class Config
        {
			static Config()
			{
				Config.Server=new AspNetServerContext(); //use ASP.Net by default(ie, almost always)
			}
            /// <summary>
            /// Authentication page. Only required when running in Medium trust. 
            /// This is the page you want to be served as the 401 auth required page.
            /// </summary>
            static public string AuthPage;
            /// <summary>
            /// Sets where anonymous users should be redirected to when trying to access a protected resource.
            /// If a generic 401 Access Denied response should be given instead, then set to null.
            /// </summary>
            static public string LoginPage = null;    
            /// <summary>
            /// Set the HttpOnly property for login cookies
            /// </summary>
            static public bool CookieHttpOnly=true;    
            /// <summary>
            /// Use browser info in constructing the login cookie hash
            /// </summary>
            static public bool CookieUseBrowserInfo=true;    
            /// <summary>
            /// Tie each login cookie to the client's IP address
            /// </summary>
            static public bool CookieUseIP=true;
            /// <summary>
            /// Hash the the physical path of URL / in login cookies. There is no reason to turn this off unless you are operating in a cluster where / may be different for each node
            /// </summary>
            static public bool CookieUseBase=true;
            /// <summary>
            /// This is to provide redundant-ish login security. This is also put in login cookie hashes and password hashes.
            /// Just fill it in with some random string of your choice. Do not use a word or password. Use a purely random string of moderate length.
            /// I recommend using random.org to generate a string, or a GUID generator. If this is not set from code, it will attempt to read it from the web.config.
            /// If this value can not be populated from one of those two methods, an exception will be thrown in Authenticate.
            /// </summary>
            static public string UniqueHash{get;set;}
            
            /// <summary>
            /// If true, then the cookie will only be transmitted over HTTPS.
            /// </summary>
            static public bool CookieSecure=false;
            
            /// <summary>
            /// This is used to name cookies.
            /// </summary>
            static public string SiteName{get;set;}
            
            /// <summary>
            /// Set to the authentication-realm for your site. Setting this to non-null enables HTTP Authentication 
            /// </summary>
            static public string HttpRealm{get;set;}
            /// <summary>
            /// Use HTTP Basic Authentication by default whenever plain RequiresLogin() is called with no argument.
            /// </summary>
            static public bool UseBasicAuthByDefault{get;set;}
            /// <summary>
            ///The length of the salt-string to be used(in characters)
            /// </summary>
            static public int SaltLength{get;set;}
    
    
            /// <summary>
            /// This calls a delegate which will create a hash using a given input and salt. By default, it uses SHA256 for this.
            /// For an example of how to  implement a new Hash algorithm, please see the FAQ document or the `DefaultHasher` implementation
            /// </summary>
            public static HashInvoker HasherInvoker { get; set; }
    
            /// <summary>
            /// How many iterations the hashing function should be used. 
            /// The default is 1. If storing more secretive information, you may wish to increase this. Note that too high of a number may make your website slow
            /// </summary>
            public static int HashIterations{get;set;}

			/// <summary>
			/// The ServerContext to use
			/// (this should usually only be changed when unit testing and mocking.. unless you're feeling very risque and think this will work elsewhere)
			/// </summary>
			/// <value>
			/// The server.
			/// </value>
			public static IServerContext Server{get;set;}
        }
        
        
        /// <summary>
        /// The IUserStore to use. This must be populated before Authenticate or anything else can be used in this class.
        /// </summary>
        public static IUserStore UserStore{get;set;}

        /// <summary>
        /// Is true if the current user was authenticated with Forms Authentication(cookie based).
        /// If false, then no user is logged in, or they were logged in with HTTP Basic Authentication.
        /// </summary>
        public static bool FormLogin
        {
            get
            {
                var u=CurrentUser; //Doing this internally causes a side effect that Authenticate runs if it hasn't already
				return (bool?)Config.Server.GetItem("fscauth_formlogin") ?? false;
            }
            private set
            {
                Config.Server.SetItem("fscauth_formlogin", value);
            }
        }

        /// <summary>
        /// The current user logged in for the HTTP request. If there is not a user logged in, this will be null.
        /// </summary>
        public static UserData CurrentUser{
            get{
				UserData user=null;
                if((user=Config.Server.GetItem("fscauth_currentuser") as UserData)!=null){
					return user;
                }else{
                    if(Authenticate()==false){
                        CurrentUser=null; //a bit confusing doing all this here. Avoid infinite recursion
                    }
					return Config.Server.GetItem("fscauth_currentuser") as UserData; //code duplication, but the alternative is recursive properties. Ick! 
                }
                
            }
            private set{
                Config.Server.SetItem("fscauth_currentuser",value);
            }
        }
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
		public static bool ProbablyLoggedIn
		{
			get
			{
				return ProbableUserName!=null;
			}
		}
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
		public static string ProbableUserName
		{
			get
			{
				string name=null;
				if((name=Config.Server.GetItem("fscauth_probableusername") as string) != null)
				{
					return name;
				}else
				{
					return ProbableUserName=ProbablyLoggedInName();
				}
			}
			private set
			{
				Config.Server.SetItem("fscauth_probableusername", value);
			}
		}
        /// <summary>
        /// Will not allow the request to continue if the user is not in the specified group.
        /// </summary>
        /// <param name="group">The group the user must be assigned</param>
        /// <exception cref="HttpException">Throws 401 error if not in group</exception>
        public static void RequiresInGroup(string group){
            if(IsInGroup(group)){
                return;
            }
            throw new HttpException(403, "You are not permitted to access this resource");
        }
        /// <summary>
        /// Will return true or false depending on if the current user belongs to the specified group.
        /// </summary>
        /// <param name="group">the group to check for</param>
        /// <returns>true if in the group, else false</returns>
        public static bool IsInGroup(string group){
            if(CurrentUser==null){
                return false;
            }
            return CurrentUser.Groups.Exists(x => x.Name == group);
        }
        /// <summary>
        /// Will not allow the request to continue if the user is not in all of the groups(comma seperated)
        /// </summary>
        /// <param name="groups">The group the user must be assigned to</param>
        /// <exception cref="HttpException">Throws 401 error if not in group</exception>
        public static void RequiresInAllGroups(string groups){
            if(IsInAllGroups(groups)){
                return;
            }
            throw new HttpException(403, "You are not permitted to access this resource");
        }
        /// <summary>
        /// Will return true or false depending on if the current user belongs all of the groups(comma seperated)
        /// </summary>
        /// <param name="groups">the group to check for. To specify multiple groups, use a comma to seperate group names.</param>
        /// <returns>true if in the groups, else false</returns>
        public static bool IsInAllGroups(string groups){
            if(CurrentUser==null){
                return false;
            }
            foreach(var g in groups.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries)){
                if(!CurrentUser.Groups.Exists(x=>x.Name==g)){
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Will not allow the request to continue if the user is not in any of the groups(comma seperated)
        /// </summary>
        /// <param name="groups">The group the user must be assigned to(any of them)</param>
        /// <exception cref="HttpException">Throws 401 error if not in group</exception>
        public static void RequiresInAnyGroups(string groups){
            if(IsInAnyGroups(groups)){
                return;
            }
            throw new HttpException(403, "You are not permitted to access this resource");
        }
        /// <summary>
        /// Will return true or false depending on if the current user belongs to any of the groups(comma seperated)
        /// </summary>
        /// <param name="groups">the groups to check for</param>
        /// <returns>true if in any of the groups, else false</returns>
        public static bool IsInAnyGroups(string groups){
            if(CurrentUser==null){
                return false;
            }
            foreach(var g in groups.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries)){
                if(CurrentUser.Groups.Exists(x=>x.Name==g)){
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Will not allow the request to continue if no one is authenticated
        /// </summary>
        /// <param name="usebasic">
        /// Use HTTP Basic authentication or not
        /// </param>
        /// <exception cref="HttpException">Throws if no one is logged in</exception>
        public static void RequiresLogin(bool usebasic){
            if(IsAuthenticated()){
                return;
            }else{
                if(usebasic){
                    SendHttp401();
                }
                if (string.IsNullOrEmpty(Config.LoginPage))
                {
                    throw new HttpException(401, "You must be authenticated to access this resource");
                }
                else
                {
                    Config.Server.Redirect(Config.LoginPage); //this will cause an exception, but the redirect will work fine
                }
            }
        }

        /// <summary>
        /// Will not allow the request to continue if no one is authenticated
        /// </summary>
        /// <exception cref="HttpException">Throws if no one is logged in</exception>
        public static void RequiresLogin(){
            RequiresLogin(false);
        }
        
        /// <summary>
        /// Returns true if a user is logged in, else false.
        /// </summary>
        /// <returns></returns>
        public static bool IsAuthenticated(){
            return CurrentUser!=null;
        }
		/// <summary>
		/// Will lookup who is "probably" looked in without hitting the database. 
		/// This is not secure and should not guard anything important
		/// </summary>
		/// <returns>
		/// The logged in name.
		/// </returns>
		static string ProbablyLoggedInName()
		{
			string username=null;
			string authHeader=Config.Server.GetHeader("Authorization");
			if(Config.HttpRealm!=null && string.IsNullOrEmpty(authHeader))
			{
                string userNameAndPassword = Encoding.Default.GetString(Convert.FromBase64String(authHeader.Substring(6)));
                string[] parts = userNameAndPassword.Split(':');
                return parts[0];
			}
			var cookie=Config.Server.GetCookie(Config.SiteName+"_login");
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
        /// Will initialize CurrentUser. This should be called at the beginning of the request.
        /// This will check for a login cookie and check that it is valid. It will then assign CurrentUser.
        /// </summary>
        /// <returns>True if there is a current user. False if there is not a current user logged in.</returns>
        /// <exception cref="ArgumentException">Throws if UniqueHash is not complete </exception>
        static bool Authenticate(){
            //HttpContext.Current.Response.Buffer=true; //needed?
            if(Config.UniqueHash==null){
                throw new ArgumentException("You MUST fill in UniqueHash before using the AuthenticationModule!");
            }
            string username;
            string hash;
            string password;
            UserData user;
            string authHeader = Config.Server.GetHeader("Authorization");
            if(Config.HttpRealm!=null && !string.IsNullOrEmpty(authHeader)){ //try HTTP basic auth
                string userNameAndPassword = Encoding.Default.GetString(Convert.FromBase64String(authHeader.Substring(6)));
                string[] parts = userNameAndPassword.Split(':');
                username=parts[0];
                password=parts[1];
                user=UserStore.GetUserByName(username);
                if(user!=null){
                    hash=ComputePasswordHash(user,password);
                    if(hash==user.PasswordHash){
                        CurrentUser=user;
                        FormLogin = false;
                        return true;
                    }else{
                        CurrentUser=null; //just drop through and try cookies instead.
                    }
                }
            }
            //try forms/cookie auth
            HttpCookie cookie=Config.Server.GetCookie(Config.SiteName+"_login");
            if(cookie==null){
                return false;
            }
            username=cookie.Values["name"];
            username=username.Trim();
            hash=cookie.Values["secret"];
            user=UserStore.GetUserByName(username);
            if(user==null){ //user wasn't found in the database
                ForceCookieExpiration();
                return false;
            }
            DateTime expires=ConvertFromUnixTimestamp(long.Parse(cookie["expire"]));
            string hash2=ComputeLoginHash(user.PasswordHash,user.Salt,expires);
            if(expires<DateTime.Now){
                ForceCookieExpiration();
                return false;
            }
            if(hash==hash2){
                CurrentUser=user;
                FormLogin = true;
                return true;
            }else{
                CurrentUser=null;
                ForceCookieExpiration();
                return false;
            }
            
        }
        /// <summary>
        /// Logs in a user. This will try to log in a user. It will check username and password against the UserStore.
        /// If valid, then it will create a new login cookie and populate CurrentUser.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="expires">The expiration date of the login cookie.
        /// If null, then defaults to the cookie expiring at the end of the user's browser session(or 4 hours, whichever is sooner)
        /// </param>
        /// <returns>True if the user was successfully logged in.</returns>
        public static bool Login(string username, string password, DateTime? expires)
        {
            var user = UserStore.GetUserByName(username);
            if (user == null)
            {
                return false;
            }
            string hash = ComputePasswordHash(user,password);
            if (hash == user.PasswordHash)
            {
                LoginFromHash(user, expires);
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// Logs in a user. This will try to log in a user. It will check username and password against the UserStore.
        /// If valid, then it will create a new login cookie and assign CurrentUser.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="remember">The "Remember Me" option. If true, then it will set the cookie to expire 1 year from now.</param>
        /// <returns>True if the user was successfully logged in.</returns>
        public static bool Login(string username,string password,bool remember){
            return Login(username,password,remember?(DateTime?)DateTime.Today.AddYears(1):null);
        }
        /// <summary>
        /// Will delete the current login cookie, if it exists.
        /// </summary>
        public static void Logout(){
            if (FormLogin==false)
            {
                throw new NotSupportedException("Can not log out a user logged in using HTTP Basic Authentication");
            }
            var c=Config.Server.GetCookie(Config.SiteName+"_login");
            if(c!=null){
                ForceCookieExpiration();
            }
        }
        /// <summary>
        /// Adds a user to the UserStore.
        /// </summary>
        /// <param name="user">User's information, not including UniqueID or PasswordHash</param>
        /// <param name="password">User's plaintext password</param>
        /// <returns></returns>
        public static bool AddUser(UserData user,string password){
            if(!UserStore.AddUser(user)){
                return false;
            }
			if(user.UniqueID==null)
			{
				throw new ApplicationException("The UniqueID of the UserData must be filled in by the UserStore");
			}
            user.Salt=null;
            user.PasswordHash=ComputePasswordHash(user,password);
            if(!UserStore.UpdateUserByID(user)){
                return false;
            }
            return true;
        }
        /// <summary>
        /// Computes a password hash. 
        /// </summary>
        /// <param name="password">The plain text password</param>
        /// <param name="username">The user's username</param>
        /// <param name="uniqueid">The user's unique ID</param>
        /// <returns></returns>
        public static string ComputePasswordHash(UserData user, string password)
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
                user.Salt=v.Salt;
                user.PasswordHash=v.Text;
                return user.PasswordHash;
            }else{
                return ComputeHash(text,user.Salt);
            }
        }
        /// <summary>
        /// This will reset the password of a user.
        /// </summary>
        /// <param name="user">
        /// </param>
        /// <returns>
        /// The generated password
        /// </returns>
        public static string ResetPassword(UserData user)
        {
            string pass=Membership.GeneratePassword(8,2);
            user.Salt=null; //regenerate salt
            user.PasswordHash=ComputePasswordHash(user,pass);
            if(!UserStore.UpdateUserByID(user)){
                throw new ApplicationException("User could not be updated");
            }
            return pass;
        }
        /// <summary>
        /// Resets the user's password 
        /// </summary>
        /// <param name="username">
        /// </param>
        /// <returns>
        /// The generated password
        /// </returns>
        public static string ResetPasswordByName(string username)
        {
            return ResetPassword(UserStore.GetUserByName(username));
        }


        private static void LoginFromHash(UserData user, DateTime? expires)
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
            Config.Server.AddCookie(c);
            CurrentUser = user;
        }

        static string ComputeLoginHash(string passwordhash,string salt,DateTime expires){
            StringBuilder sb=new StringBuilder();
            sb.Append(passwordhash);
            if(Config.CookieUseIP){
                sb.Append(Config.Server.UserIP);
            }
            if(Config.CookieUseBase){
                sb.Append(Config.Server.MapPath("/"));
            }
            if(Config.CookieUseBrowserInfo){
                sb.Append(Config.Server.UserAgent);
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
        static string ComputeHash(string input, string salt)
        {
            if(Config.HashIterations==0){
                throw new NotSupportedException("HashIterations must be at least one!");
            }
            Debug.Assert(salt != null);
            HashWithSalt v=new HashWithSalt();
            v.Text=input;
            v.Salt=salt;
            for(int i=0;i<Config.HashIterations;i++){
                v=Config.HasherInvoker(v.Text,v.Salt);
            }
            return v.Text;
        }
        /// <summary>
        /// Computes a new hash and gets a new salt
        /// </summary>
        static HashWithSalt NewHash(string input)
        {
            if(Config.HashIterations==0){
                throw new NotSupportedException("HashIterations must be at least one!");
            }
            HashWithSalt v=new HashWithSalt();
            v.Text=input;
            v.Salt=null;
            for(int i=0;i<Config.HashIterations;i++){
                v=Config.HasherInvoker(v.Text,v.Salt);
            }
            return v;
        }
        static void SendHttp401(){ //this will directly write the error, rather than throwing an exception. 
            var c=Config.Server;
            c.HttpStatus="401 Not Authenticated";
            if(Config.HttpRealm!=null){
                c.AddHeader("WWW-Authenticate", "Basic Realm=\""+Config.HttpRealm+"\"");
            }
            try
            {
                c.Transfer(CustomErrorsFixer.GetCustomError("401")); //output the 401 page
            }
            catch(SecurityException) //running under medium trust
            {
                c.Transfer(Config.AuthPage);
            }
			c.KillIt();
        }
        
        static void ForceCookieExpiration(){
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
        
        

        
    }
                    
}


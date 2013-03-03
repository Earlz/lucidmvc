using System;
namespace Earlz.LucidMVC.Authentication
{
    public interface IAuthMechanism
    {
		/// <summary>
		/// Computes a hash of the given password using the given UserData and populates PasswordHash and Salt in the UserData with new values
		/// </summary>
        void ComputePasswordHash(UserData user, string password);
		/// <summary>
		/// Gets the current logged in user. Returns null if not authenticated
		/// </summary>
        UserData CurrentUser { get; }
		/// <summary>
		/// Will force authentication to not be "lazy". This method should be completely optional to calling code
		/// </summary>
		void Authenticate();
		/// <summary>
		/// Logs out the current user
		/// </summary>
        void Logout();
		/// <summary>
		/// Gets the user name of the probably logged in user. 
		/// This is in NO WAY guaranteed to be the actual user name. This should only be used for non-essential security,
		/// such as displaying a username in a status bar or some such. This should not be used to actually protect anything!
		/// If an implementation doesn't support this, it should just return null
		/// </summary>
        string ProbableUserName { get; }
		/// <summary>
		/// Gets whether someone is "probably" logged in.
		/// This is in NO WAY guaranteed to be accurate. This should only be used for non-essential security,
		/// such as displaying a username in a status bar or some such. This should not be used to actually protect anything!
		/// If an implementation doesn't support this, it should just return false
		/// </summary>
        bool ProbablyLoggedIn { get; }
		/// <summary>
		/// Gets the current user store.
		/// </summary>
        IUserStore UserStore { get; }
		IServerContext Server{get;}
		/// <summary>
		/// Login the specified user. Expiration is an absolute time of expiration for the session
		/// If the authentication mechanism doesn't support this, it should throw a NotsSupportedException
		/// </summary>
		bool Login(string username, string password, DateTime expiration);
		/// <summary>
		/// Login the specified user. The expiration date of the session should be set to `expires` amount of time away from DateTime.Now
		/// Each time the user hits an authenticated page though, `expires` is amount of time away from last time they hit an authenticated page
		/// `timeout` is an absolute expiration for the session. By setting these to fairly large values, you can have "remember me" functionality
		/// </summary>
		bool Login(string username, string password, TimeSpan expires, DateTime? timeout);
		/// <summary>
		/// Login the specified user with default session expiration. This should be a fairly short amount of time OR when the user closes the browser, if possible. 
		/// </summary>
		bool Login(string username, string password);

		/// <summary>
		/// Used to indicate the calling application requires authentication. If no user is logged in, then this method should be used to
		/// redirect to a login page, send a 401 HTTP error, or whatever else you need to allow a user to login. This method should NEVER return
		/// This method must call Server.KillIt(); and never return for security purposes
		/// </summary>
		void RequiresAuthentication();

    }
}

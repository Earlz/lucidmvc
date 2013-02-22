using System;
namespace Earlz.BarelyMVC.Authentication
{
    public interface IAuthMechanism
    {
		/// <summary>
		/// Computes a hash of the given password using the given UserData
		/// </summary>
        string ComputePasswordHash(UserData user, string password);
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
		string GetLoginToken(UserData user);

    }
}

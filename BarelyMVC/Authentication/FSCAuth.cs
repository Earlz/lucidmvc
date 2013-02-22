using System;

namespace Earlz.BarelyMVC.Authentication
{
	public class FSCAuth : IAuthMechanism
	{
		public FSCAuth()
		{
		}
		public string ComputePasswordHash (UserData user, string password)
		{

			throw new NotImplementedException ();
		}

		public void Authenticate ()
		{
			throw new NotImplementedException ();
		}

		public void Logout ()
		{
			throw new NotImplementedException ();
		}

		public string GetLoginToken (UserData user)
		{
			throw new NotImplementedException ();
		}

		public UserData CurrentUser {
			get;
			private set;
		}

		public string ProbableUserName {
			get;
			private set;
		}

		public bool ProbablyLoggedIn {
			get;
			private set;
		}

		public IUserStore UserStore {
			get;
			private set;
		}

		public IServerContext Server {
			get;
			private set;
		}

	}
}


using System;

namespace Earlz.BarelyMVC.Authentication
{
	public interface IAuthPersister
	{
		/// <summary>
		/// Will check if a user is logged in and make the appropriate calls to `auth` to validate that they are
		/// </summary>
		void Check(IAuthMechanism auth);
		/// <summary>
		/// Persist the specified token and set it to expire at set expiration. If expiration is null, the token should unpersist at the end of the session
		/// If any part of this is not supported, it should throw a NotSupportedException
		/// </summary>
		void Persist(IAuthMechanism auth, string token, UserData user, DateTime? expiration);
		/// <summary>
		/// Persist the specified token and set it to have a sliding expiration which is "bumped" by this amount on each visit
		/// </summary>
		void Persist(IAuthMechanism auth, string token, UserData user, TimeSpan expiration);
		void Unpersist(IAuthMechanism auth, string token);
		void RequiresLogin(IAuthMechanism auth);
	}
}


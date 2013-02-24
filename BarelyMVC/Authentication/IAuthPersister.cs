using System;

namespace Earlz.BarelyMVC.Authentication
{
	public interface IAuthPersister
	{
		/// <summary>
		/// Will check if a user is logged in and make the appropriate calls to `auth` to validate that they are
		/// </summary>
		void Check();
		void Persist(string token, DateTime? expiration);
		void Unpersist(string token);
		void RequiresLogin();
	}
}


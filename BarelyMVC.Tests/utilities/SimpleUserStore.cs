using System;
using Earlz.BarelyMVC.Authentication;
using System.Collections.Generic;
using System.Linq;

namespace Earlz.BarelyMVC.Tests
{
	public class SimpleUserStore : IUserStore
	{
		List<UserData> users=new List<UserData>();
		int unique=0;
		public UserData GetUserByName (string name)
		{
			return users.SingleOrDefault(x=>x.Username==name);
		}

		public bool UpdateUserByID (UserData user)
		{
			foreach(var u in users)
			{
				if(u.UniqueID==user.UniqueID)
				{
					u.PasswordHash=user.PasswordHash;
					u.Salt=user.Salt;
					u.Username=user.Username;
					u.Groups=user.Groups;
					u.EmailAddress=user.EmailAddress;
					return true;
				}
			}
			return false;
		}

		public bool AddUser (UserData user)
		{
			if(users.Any(x=>x.Username==user.Username))
			{
				return false;
			}
			users.Add(user);
			user.UniqueID=(unique++).ToString();
			return true;
		}

		public bool DeleteUserByID (UserData user)
		{
			UserData del=null;
			foreach(var u in users)
			{
				if(u.UniqueID==user.UniqueID)
				{
					del=u;
					break;
				}
			}
			if(del==null)
			{
				return false;
			}
			users.Remove(del);
			return true;
		}
	}
}


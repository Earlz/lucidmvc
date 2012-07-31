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
namespace Earlz.BarelyMVC.Authentication
{
	/// <summary>
	/// The minimalistic interface from Authentication to the underlying database(or other storage medium). 
	/// Note, as a best practice, I recommend the classes implementing this interface to be a singleton.
	/// </summary>
	public interface IUserStore
	{
		/* Note these are not all the features that should probably be implemented for a typical website.
		 * Most sites will need other features here such as GetUserByID. But these are the only features that Authentication cares about */
		
		/// <summary>
        /// Gets a UserData object by only the username.
        /// Usernames must be unique
		/// </summary>
		/// <param name="id"></param>
		/// <returns>The UserData object associated with username, if not found then null.</returns>
		UserData GetUserByName(string name);
		/// <summary>
		/// Updates the specified UserData, searching by UniqueID only. (in some cases, only UniqueID will be populated)
		/// </summary>
		/// <param name="user">The UserData object</param>
		/// <returns></returns>
        /// <remarks>
        /// This is intended to search by UniqueID. UniqueID can never change.
        /// This function is used by Authentication.AddUser, so it must be implemented to add a user account. 
        /// </remarks>
		bool UpdateUserByID(UserData user);

		/// <summary>
        /// Adds a new user, populating the UniqueID of user in the process.
		/// </summary>
		/// <param name="user">The UserData object. This should have at least Username populated. </param>
		/// <returns>true if successful in adding it.</returns>
		/// <exception cref="UserExistsException">Thrown if the username is already taken </exception>
		bool AddUser(UserData user);
		
		/// <summary>
		/// Deletes a user by UniqueID. This is optional. FSCAuth does not use this function at all internally.
		/// </summary>
		/// <param name="user">
		/// User to delete
		/// </param>
		/// <returns>
		/// true if deletion was successful
		/// </returns>
		bool DeleteUserByID(UserData user);
	}
}


/*
    Copyright 2011 Jordan "Earlz" Earls <http://lastyearswishes.com>

    This file is part of FSCAuth.
    This project is dual licensed under the GPL and a commercial license. Please see http://www.binpress.com/app/fscauth/231 to purchase a license
    for use in commercial/non-GPL projects.
 
    FSCAuth is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation, version 3 of the License.
 
    FSCAuth is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along with FSCAuth. If not, see http://www.gnu.org/licenses/.
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


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
	/// thrown when trying to create a new user and the username is already in use(if applicable)
	/// </summary>
	public class UserExistsException : ApplicationException
	{
		public UserExistsException ()
		{
		}
	}
}


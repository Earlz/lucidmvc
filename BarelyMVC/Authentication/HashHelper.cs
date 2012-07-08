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
using System.Text;
using System.Security.Cryptography;
using System.Web;
namespace Earlz.BarelyMVC.Authentication
{
	public class HashWithSalt
	{
		/// <summary>
		/// the hashed output
		/// </summary>
		public string Text;
		/// <summary>
		/// The salt used for hashing
		/// </summary>
		public string Salt;
	}
	
	static public class HashHelper
	{
		/// <summary>
		/// decodes a URL base64 string to byte array
		/// </summary>
		static public byte[] ToBytes(string v)
		{
			return HttpServerUtility.UrlTokenDecode(v);
			

		}
		/// <summary>
		/// Converts a byte array to a URL base64 string
		/// </summary>
		static public string FromBytes(byte[] v)
		{
			return HttpServerUtility.UrlTokenEncode(v);
		}
		/// <summary>
		/// Converts a string to byte array(using UTF8)
		/// </summary>
		static public byte[] ToRawBytes(string v)
		{
			return Encoding.UTF8.GetBytes(v);
		}
		/// <summary>
		/// Converts a byte array to a string(using UTF8)
		/// </summary>
		static public string FromRawBytes(byte[] v)
		{
			return BitConverter.ToString(v);
		}
		/// <summary>
		/// Generates a new random salt of specified length using RNGCryptoServiceProvider
		/// </summary>
		public static string GetSalt(int length)
		{
			//Create and populate random byte array
			byte[] randomArray = new byte[length];
			string randomString;
			//Create random salt and convert to string
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(randomArray);
			randomString = Convert.ToBase64String(randomArray);
			return randomString.Substring(0,Math.Min(randomString.Length,length)); //cut to specified length
		}
	}
}


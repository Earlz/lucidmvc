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
using System.Text;
using System.Security.Cryptography;
using System.Web;
namespace Earlz.LucidMVC.Authentication
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


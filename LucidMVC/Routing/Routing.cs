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
using System.Web;
using System.Text.RegularExpressions;


namespace Earlz.LucidMVC
{
	public class StopExecutionException : Exception
	{
	}
    /**A static helper class for use inside of Global.asax(usually)**/
    public static class Routing
    {
        static public int SlugMaxWords=7;
        static public int SlugMaxChars=70;
        /// <summary>
        /// Will match everything that isn't alphanumeric, dash, or space
        /// </summary>
        static Regex NonAlphaNumeric;
		/// <summary>
		/// This is considered a "just-in-case" type thing. After the slug is created, this will run to find
		/// any possibly unexpected output. This is to prevent redirect scams and generally unsafe things (like breaking HTML and other fun stuff)
		/// </summary>
		static Regex SafetyStrip;
        static Routing()
        {
            NonAlphaNumeric=new Regex(@"[^a-zA-Z0-9]\ ", RegexOptions.Compiled);
			SafetyStrip=new Regex(@"[^a-zA-Z0-9\-]", RegexOptions.Compiled);
        }
        public static Router Router{
			get{
				if(router==null)
				{
					router=new Router();
				}
				return router;
			}
		}
        static Router router;
        /// <summary>
        /// Handles the current HttpRequest and calls the appropriate HttpHandler
        /// </summary>
		/*
        static public void DoRequest(IServerContext c){
            c.ContentType="text/html"; //default
            if(c.RequestUrl.AbsolutePath.Substring(0,Math.Min(c.RequestUrl.AbsolutePath.Length,8))=="/static/"){
                return; //let it just serve the static files
            }
            if(Router.DoRoute(c)){
				c.KillIt();
            }
        } */
        /// <summary>
        /// Will strip all non-alphanumeric characters and replace all spaces with `-` to make a URL friendly "slug"
        /// </summary>
        static public string Slugify(string text)
        {
			string tmp=NonAlphaNumeric.Replace(text," ").Replace(" ","-").ToLower();
            //remove insignificant duplicate `-` characters
            tmp=string.Join("-", tmp.Split(new string[]{"-"}, StringSplitOptions.RemoveEmptyEntries));
            if(tmp.Length>0 && tmp[tmp.Length-1]=='-')
            {
                tmp=tmp.Substring(0,tmp.Length-1); //remove trailing - if needed
            }
            if(tmp.Length>0 && tmp[0]=='-')
            {
                tmp=tmp.Substring(1); //skip ahead one
            }
            int wordcount=0;
            if(tmp.Length>SlugMaxChars)
            {
                tmp=tmp.Substring(0,SlugMaxChars);
            }
            for(int i=0;i<tmp.Length;i++)
            {
                if(tmp[i]=='-')
                {
                    wordcount++;
                    if(wordcount>SlugMaxWords)
                    {
                        tmp=tmp.Substring(0,i);
                        break;
                    }
                }
            }
            return SafetyStrip.Replace(tmp, "");
        }
    }
}


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
using System.Web.Configuration;
using System.Configuration;
namespace Earlz.LucidMVC.Authentication
{
    public static class CustomErrorsFixer
    {
        /// <summary>
        /// Fixes a bug in some configurations of ASP.Net and Mono where a thrown HttpException will cause a redirect with an improper status code
        /// </summary>
        static public void HandleErrors(HttpContext Context)
        {
            try
            {
                if (RunIt(Context) == false)
                {
                    return;
                }
            }
            catch
            {
                throw new NotSupportedException("CustomErrorsFixer does not work in Medium trust");
            }
            HttpException ex = Context.Error as HttpException;
            if(ex==null){
                try{
                    ex=Context.Error.InnerException as HttpException;
                }catch{
                    ex=null;
                }
            }
            
            if (ex != null){
                Context.Response.StatusCode = ex.GetHttpCode();
            }else{
                Context.Response.StatusCode = 500;
            }
            Context.ClearError();
            Context.Server.Transfer(GetCustomError(Context.Response.StatusCode.ToString()));

            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// Will get the error page to be used by the specified code in Web.config
        /// </summary>
        static public string GetCustomError(string code)
        {
            CustomErrorsSection section = ConfigurationManager.GetSection("system.web/customErrors") as CustomErrorsSection;
        
            if (section != null)
            {
                CustomError page = section.Errors[code];
            
                if (page != null)
                {
                    return page.Redirect;
                }
            }
            return section.DefaultRedirect;
        }
        static private bool RunIt(HttpContext context){
            CustomErrorsSection section = ConfigurationManager.GetSection("system.web/customErrors") as CustomErrorsSection;
            switch(section.Mode){
                case CustomErrorsMode.Off:
                    return false;
                case CustomErrorsMode.On:
                    return true;
                case CustomErrorsMode.RemoteOnly:
                    return !(context.Request.UserHostAddress=="127.0.0.1");
                default:
                    return true;
            }
        }
                        
    }
}


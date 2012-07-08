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
using System.Web;
using System.Web.Configuration;
using System.Configuration;
namespace Earlz.BarelyMVC.Authentication
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


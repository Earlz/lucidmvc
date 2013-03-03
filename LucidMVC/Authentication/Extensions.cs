using System;
using System.Web;
using System.Web.Security;

namespace Earlz.BarelyMVC.Authentication
{
	static public class IAuthMechanismExtensions
	{

        /// <summary>
        /// Will not allow the request to continue if the user is not in the specified group.
        /// </summary>
        /// <param name="group">The group the user must be assigned</param>
        /// <exception cref="HttpException">Throws 401 error if not in group</exception>
        public static void RequiresInGroup(this IAuthMechanism auth, string group){
            if(auth.IsInGroup(group)){
                return;
            }
            throw new HttpException(403, "You are not permitted to access this resource");
        }
        /// <summary>
        /// Will return true or false depending on if the current user belongs to the specified group.
        /// </summary>
        /// <param name="group">the group to check for</param>
        /// <returns>true if in the group, else false</returns>
        public static bool IsInGroup(this IAuthMechanism auth, string group){
            if(auth.CurrentUser==null){
                return false;
            }
            return auth.CurrentUser.Groups.Exists(x => x.Name == group);
        }
        /// <summary>
        /// Will not allow the request to continue if the user is not in all of the groups(comma seperated)
        /// </summary>
        /// <param name="groups">The group the user must be assigned to</param>
        /// <exception cref="HttpException">Throws 401 error if not in group</exception>
        public static void RequiresInAllGroups(this IAuthMechanism auth, string groups){
            if(auth.IsInAllGroups(groups)){
                return;
            }
            throw new HttpException(403, "You are not permitted to access this resource");
        }
        /// <summary>
        /// Will return true or false depending on if the current user belongs all of the groups(comma seperated)
        /// </summary>
        /// <param name="groups">the group to check for. To specify multiple groups, use a comma to seperate group names.</param>
        /// <returns>true if in the groups, else false</returns>
        public static bool IsInAllGroups(this IAuthMechanism auth, string groups){
            if(auth.CurrentUser==null){
                return false;
            }
            foreach(var g in groups.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries)){
                if(!auth.CurrentUser.Groups.Exists(x=>x.Name==g)){
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Will not allow the request to continue if the user is not in any of the groups(comma seperated)
        /// </summary>
        /// <param name="groups">The group the user must be assigned to(any of them)</param>
        /// <exception cref="HttpException">Throws 401 error if not in group</exception>
        public static void RequiresInAnyGroups(this IAuthMechanism auth, string groups){
            if(auth.IsInAnyGroups(groups)){
                return;
            }
            throw new HttpException(403, "You are not permitted to access this resource");
        }
        /// <summary>
        /// Will return true or false depending on if the current user belongs to any of the groups(comma seperated)
        /// </summary>
        /// <param name="groups">the groups to check for</param>
        /// <returns>true if in any of the groups, else false</returns>
        public static bool IsInAnyGroups(this IAuthMechanism auth, string groups){
            if(auth.CurrentUser==null){
                return false;
            }
            foreach(var g in groups.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries)){
                if(auth.CurrentUser.Groups.Exists(x=>x.Name==g)){
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns true if a user is logged in, else false.
        /// </summary>
        /// <returns></returns>
        public static bool IsAuthenticated(this IAuthMechanism auth){
            return auth.CurrentUser!=null;
        }
        /// <summary>
        /// This will reset the password of a user.
        /// </summary>
        /// <param name="user">
        /// </param>
        /// <returns>
        /// The generated password
        /// </returns>
        public static string ResetPassword(this IAuthMechanism auth, UserData user)
        {
            string pass=Membership.GeneratePassword(8,2);
            auth.ComputePasswordHash(user,pass);
            if(!auth.UserStore.UpdateUserByID(user)){
                throw new ApplicationException("User could not be updated");
            }
            return pass;
        }
        /// <summary>
        /// Resets the user's password 
        /// </summary>
        /// <param name="username">
        /// </param>
        /// <returns>
        /// The generated password
        /// </returns>
        public static string ResetPasswordByName(this IAuthMechanism auth, string username)
        {
            return auth.ResetPassword(auth.UserStore.GetUserByName(username));
        }
	}
}


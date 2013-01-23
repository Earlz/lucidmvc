using System;
using Earlz.BarelyMVC;
using Moq;
using System.Web;
using Earlz.BarelyMVC.Authentication;
using System.Collections.Generic;

namespace BarelyMVC.Tests.Extensions.ServerMocking
{
	/// <summary>
	/// Server mocking helper extensions
	/// Prefix Keywords:
	/// "Has" -- Will use mock.Verify and should be used after the call has taken place. These are past-tense
	/// "Will" -- Uses mock.Setup and should be used before the call has taken place, followed by mock.Verify. These are future-tense
	/// "Is" -- Uses mock.Setup to setup initial parameters for a scenario, but does not place anything verifiable into the mock. These are present-tense
	/// "Get" -- Gets a captured piece of information from an earlier call. These are past-tense
	/// "WillCapture" -- Uses mock.Setup to capture a piece of information that can later be retrieved using the matching "Get" method. these are future-tense
	/// </summary>
	public static class ServerMockingHelpers
	{
		public static Mock<IServerContext> IsNotLoggedIn(this Mock<IServerContext> mock)
		{
			mock.Setup(x=>x.GetHeader("Authorization")).Returns<string>(null);
			mock.Setup(x=>x.GetItem("fscauth_currentuser")).Returns<UserData>(null);
			mock.Setup(x=>x.GetCookie(FSCAuth.Config.SiteName+"_login")).Returns<string>(null);
			return mock;
		}
		public static Mock<IServerContext> HasLoggedIn(this Mock<IServerContext> mock)
		{
			mock.Verify(x=>x.AddCookie(
				It.Is<HttpCookie>(c=>
                     c.Name==FSCAuth.Config.SiteName+"_login" &&
			         c.Values["secret"].Length>0
            	)
			));
			return mock;
		}
		static HttpCookie logincookie=null;
		public static HttpCookie LoginCookie
		{
			get
			{
				if(logincookie==null)
				{
					throw new ApplicationException("The login cookie has not been captured!");
				}
				return logincookie;
			}
			private set
			{
				logincookie=value;
			}
		}
		public static Mock<IServerContext> WillCaptureLoginCookie(this Mock<IServerContext> mock)
		{
			LoginCookie=null;
			mock.Setup(x=>x.AddCookie(
				It.Is<HttpCookie>(c=>true) //don't care about the parameter specification
			)).Callback<HttpCookie>(c=>LoginCookie=c);
			return mock;
		}
		public static HttpCookie GetCapturedLoginCookie(this Mock<IServerContext> mock)
		{
			return LoginCookie;
		}
		public static Mock<IServerContext> WillNotBeKilled(this Mock<IServerContext> mock)
		{
			mock.Setup(x=>x.KillIt()).Throws(new ApplicationException("This should not be killed -- KillIt should not be called"));
			return mock;
		}
		public static Mock<IServerContext> HasNotBeenKilled(this Mock<IServerContext> mock)
		{
			mock.Verify(x=>x.KillIt(), Times.Never());
			return mock;
		}
		public static Mock<IServerContext> WillBeKilled(this Mock<IServerContext> mock)
		{
			mock.Setup(x=>x.KillIt()).Verifiable();
			return mock;
		}

		public static Mock<IServerContext> HasBeenKilled(this Mock<IServerContext> mock)
		{
			mock.Verify(x=>x.KillIt());
			return mock;
		}

		static Dictionary<string, object> Items=new Dictionary<string, object>();
		public static Mock<IServerContext> IsUsingItems(this Mock<IServerContext> mock)
		{
			mock.Setup(x=>x.GetItem(It.IsAny<string>())).Returns<string>(k=>Items.ContainsKey(k) ? Items[k] : null);
			mock.Setup(x=>x.SetItem(It.IsAny<string>(), It.IsAny<object>())).Returns<string, object>((k, v) => Items[k]=v); //this actually isn't a proper mock. Oh well
			return mock;
		}
		public static void RemoveLoginItem(this Mock<IServerContext> mock)
		{
			Items.Remove("fscauth_currentuser");
		}


	}
}


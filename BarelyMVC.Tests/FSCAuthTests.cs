using System;
using NUnit.Framework;
using Earlz.BarelyMVC.Authentication;
using Moq;
using Earlz.BarelyMVC;
using System.Web;
using System.Collections.Generic;
using Earlz.BarelyMVC.Tests.Extensions.ServerMocking;

namespace Earlz.BarelyMVC.Tests
{
	[Category("FSCAuth")]
	[TestFixture]
	public class FSCAuthTests
	{
		void Prep(FSCAuth auth)
		{
			auth.Config.UniqueHash="foo";
			auth.Config.LoginPage="/login";
			auth.Config.SiteName="test";
			new UserData(){Username="user", Groups=new List<GroupData>(){new GroupData("admin")}}.SaveNew(auth, "pass");
		}
		[Test]
		public void RequiresLogin_should_redirect_when_not_logged_in()
		{
			var mock=new Mock<IServerContext>();
			var auth=new FSCAuth(mock.Object, new FSCAuthConfig(), new SimpleUserStore());
			Prep(auth);
			mock.IsNotLoggedIn();

			auth.RequiresAuthentication();
			mock.Verify(x=>x.Redirect("/login"), "Should redirect to login page");
			mock.HasBeenKilled();
		}
		[Test]
		public void Login_should_work_when_given_proper_credentials()
		{
			var mock=new Mock<IServerContext>();
			var auth=new FSCAuth(mock.Object, new FSCAuthConfig(), new SimpleUserStore());
			Prep (auth);
			auth.Login("user", "pass");
			mock.HasLoggedIn(auth);
			mock.Verify(x=>x.SetCookie(
				It.Is<HttpCookie>(c=>
                     c.Name==auth.Config.SiteName+"_login" &&
			         c.Values["secret"].Length>0
            	)
			));
		}
		[Test]
		public void RequiresLogin_should_pass_through_when_authorized()
		{
			var mock=new Mock<IServerContext>();
			var auth=new FSCAuth(mock.Object, new FSCAuthConfig(), new SimpleUserStore());
			Prep (auth);
			mock.IsUsingItems().WillCaptureLoginCookie();
			auth.Login("user", "pass");
			mock.RemoveLoginItem(); //we're testing that cookies work
			mock.Setup(x=>x.GetCookie(auth.Config.SiteName+"_login")).Returns(mock.GetCapturedLoginCookie()).Verifiable();
			mock.WillNotBeKilled();
			auth.RequiresAuthentication();
			mock.Verify();
		}
		[Test]
		public void RequiresInGroup_should_redirect()
		{
			var mock=new Mock<IServerContext>();
			var auth=new FSCAuth(mock.Object, new FSCAuthConfig(), new SimpleUserStore());
			Prep(auth);
			mock.IsUsingItems();
			auth.Login("user", "pass");
			try
			{
				auth.RequiresInGroup("foo");
			}catch(HttpException e)
			{
				Assert.AreEqual(403, e.GetHttpCode());
				return;
			}
			Assert.Fail("Should not reach here. No exception thrown");
		}

	}
}


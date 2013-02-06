using System;
using NUnit.Framework;
using Earlz.BarelyMVC.Authentication;
using Moq;
using Earlz.BarelyMVC;
using System.Web;
using System.Collections.Generic;
using BarelyMVC.Tests.Extensions.ServerMocking;

namespace BarelyMVC.Tests
{
	[Category("FSCAuth")]
	[TestFixture]
	public class FSCAuthTests
	{
		Mock<IServerContext> mock=null;
		[SetUp]
		public void SetUp()
		{
			FSCAuth.Config.UniqueHash="foo";
			FSCAuth.Config.LoginPage="/login";
			FSCAuth.Config.SiteName="test";
			FSCAuth.Config.Server=null; //must be mocked
			FSCAuth.UserStore=new SimpleUserStore();
			FSCAuth.AddUser(new UserData(){Username="user", Groups=new List<GroupData>(){new GroupData("admin")}}, "pass");
			mock=new Mock<IServerContext>();
			FSCAuth.Config.Server=mock.Object;
		}
		[Test]
		public void RequiresLogin_should_redirect_when_not_logged_in()
		{
			mock.IsNotLoggedIn();

			FSCAuth.RequiresLogin();
			mock.Verify(x=>x.Redirect("/login"), "Should redirect to login page");
			mock.HasBeenKilled();
		}
		[Test]
		public void Login_should_work_when_given_proper_credentials()
		{
			FSCAuth.Login("user", "pass", false);
			mock.HasLoggedIn();
			mock.Verify(x=>x.SetCookie(
				It.Is<HttpCookie>(c=>
                     c.Name==FSCAuth.Config.SiteName+"_login" &&
			         c.Values["secret"].Length>0
            	)
			));
		}
		[Test]
		public void RequiresLogin_should_pass_through_when_authorized()
		{
			mock.IsUsingItems().WillCaptureLoginCookie();
			FSCAuth.Login("user", "pass", false);
			mock.RemoveLoginItem(); //we're testing that cookies work
			mock.Setup(x=>x.GetCookie(FSCAuth.Config.SiteName+"_login")).Returns(mock.GetCapturedLoginCookie()).Verifiable();
			mock.WillNotBeKilled();
			FSCAuth.RequiresLogin();
			mock.Verify();
		}
		[Test]
		public void RequiresInGroup_should_redirect()
		{
			mock.IsUsingItems();
			FSCAuth.Login("user", "pass", false);
			try
			{
			FSCAuth.RequiresInGroup("foo");
			}catch(HttpException e)
			{
				Assert.AreEqual(403, e.GetHttpCode());
				return;
			}
			Assert.Fail("Should not reach here. No exception thrown");
		}
		[Test]
		public void TestTemp()
		{
			Assert.IsFalse(true, "blah blah testing CI");
		}
	}
}


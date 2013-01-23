using System;
using NUnit.Framework;
using Earlz.BarelyMVC.Authentication;
using Moq;
using Earlz.BarelyMVC;
using System.Web;
using System.Collections.Generic;

namespace BarelyMVC.Tests
{
	[TestFixture]
	public class FSCAuthTests
	{
		[SetUp]
		public void SetUp()
		{
			FSCAuth.Config.UniqueHash="foo";
			FSCAuth.Config.LoginPage="/login";
			FSCAuth.Config.SiteName="test";
			FSCAuth.Config.Server=null; //must be mocked
			FSCAuth.UserStore=new SimpleUserStore();
			FSCAuth.AddUser(new UserData(){Username="user", Groups=new List<GroupData>(){new GroupData("foobar")}}, "pass");
		}
		[Test]
		public void RequiresLogin_should_redirect_when_not_logged_in()
		{
			var mock=new Mock<IServerContext>();
			FSCAuth.Config.Server=mock.Object;
			mock.Setup(x=>x.GetHeader("Authorization")).Returns<string>(null);
			mock.Setup(x=>x.GetItem("fscauth_currentuser")).Returns<UserData>(null);
			mock.Setup(x=>x.GetCookie(FSCAuth.Config.SiteName+"_login")).Returns<string>(null);


			FSCAuth.RequiresLogin();

			mock.Verify(x=>x.Redirect("/login"), "Should redirect to login page");
		}
		[Test]
		public void Login_should_work_when_given_proper_credentials()
		{
			var mock=new Mock<IServerContext>();
			FSCAuth.Config.Server=mock.Object;
			FSCAuth.Login("user", "pass", false);
			mock.Verify(x=>x.AddCookie(
				It.Is<HttpCookie>(c=>
                     c.Name==FSCAuth.Config.SiteName+"_login" &&
			         c.Values["secret"].Length>0
            	)
			));
		}
		[Test]
		public void RequiresLogin_should_pass_through_when_authorized()
		{
			var mock=new Mock<IServerContext>();
			FSCAuth.Config.Server=mock.Object;
			HttpCookie logincookie=null;
			mock.Setup(x=>x.AddCookie(
				It.Is<HttpCookie>(c=>true) //don't care about the parameter specification
			)).Callback<HttpCookie>(cookie=>logincookie=cookie);
			FSCAuth.Login("user", "pass", false);
			Assert.NotNull(logincookie);

			mock.Setup(x=>x.GetCookie(FSCAuth.Config.SiteName+"_login")).Returns(logincookie).Verifiable();
			mock.Setup(x=>x.KillIt()).Throws(new ApplicationException("KillIt should not be called as this indicates that the login did not succeed"));
			FSCAuth.RequiresLogin();
			mock.Verify();
		}
	}
}


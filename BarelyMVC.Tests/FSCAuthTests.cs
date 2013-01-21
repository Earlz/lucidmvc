using System;
using NUnit.Framework;
using Earlz.BarelyMVC.Authentication;
using Moq;
using Earlz.BarelyMVC;
using System.Web;

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
			FSCAuth.AddUser(new UserData(){Username="user"}, "pass");
		}
		[Test]
		public void RequiresLoginShouldRedirect()
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
		public void LoginShouldWork()
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
	}
}


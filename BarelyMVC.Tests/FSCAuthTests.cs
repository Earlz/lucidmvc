using System;
using NUnit.Framework;
using Earlz.BarelyMVC.Authentication;
using Moq;
using Earlz.BarelyMVC;

namespace BarelyMVC.Tests
{
	[TestFixture]
	public class FSCAuthTests
	{
		public FSCAuthTests()
		{
			SetUp();
		}
		[SetUp]
		public void SetUp()
		{
			FSCAuth.Config.UniqueHash="foo";
			FSCAuth.Config.LoginPage="/login";
			FSCAuth.Config.SiteName="test";
			FSCAuth.Config.Server=null; //must be mocked
		}
		[Test]
		public void RequiresLoginShouldRedirect()
		{
			SetUp();
			var mock=new Mock<IServerContext>();

			FSCAuth.Config.Server=mock.Object;
			mock.Setup(x=>x.GetHeader("Authorization")).Returns<string>(null);
			mock.Setup(x=>x.GetItem("fscauth_currentuser")).Returns<UserData>(null);
			mock.Setup(x=>x.GetCookie(FSCAuth.Config.SiteName+"_login")).Returns<string>(null);


			FSCAuth.RequiresLogin();

			mock.Verify(x=>x.Redirect("/login"), "Should redirect to login page");
		}
	}
}


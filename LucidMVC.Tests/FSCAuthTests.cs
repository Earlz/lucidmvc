using System;
using NUnit.Framework;
using Earlz.LucidMVC.Authentication;
using Moq;
using Earlz.LucidMVC;
using System.Web;
using System.Collections.Generic;
using Earlz.LucidMVC.Tests.Extensions.ServerMocking;
using System.Linq;
using Earlz.LucidMVC.Authentication.Experimental;

namespace Earlz.LucidMVC.Tests
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
			var mock=new FakeServerContext();
			var auth=new FSCAuth(mock, new FSCAuthConfig(), new SimpleUserStore());
			Prep(auth);
			bool threw=false;
			try
			{
				auth.RequiresAuthentication();
			}
			catch(FakeServerKilledException)
			{
				threw=true;
			}
			Assert.IsTrue(threw);
			Assert.AreEqual("/login", mock.RedirectedTo);
		}
		[Test]
		public void Login_should_work_when_given_proper_credentials()
		{
			var mock=new FakeServerContext();
			var auth=new FSCAuth(mock, new FSCAuthConfig(), new SimpleUserStore());
			Prep (auth);
			auth.Login("user", "pass");
			var cookie=mock.ResponseCookies.Single();
			Assert.AreEqual(auth.Config.SiteName+"_login", cookie.Name);
			Assert.IsTrue(cookie["secret"].Length>0);
			Assert.IsNotNull(auth.CurrentUser);
		}
		[Test]
		public void RequiresLogin_should_pass_through_when_authorized()
		{
			var mock=new FakeServerContext();
			var auth=new FSCAuth(mock, new FSCAuthConfig(), new SimpleUserStore());
			Prep(auth);
			auth.Login("user", "pass");
			var cookie=mock.ResponseCookies.Single();
			mock.ResponseCookies.Clear();
			mock.RequestCookies.Add(cookie);
			auth.RequiresAuthentication(); //ensure this doesn't cause FakeServerContext to throw FakeServerKilled
		}
		[Test]
		public void RequiresInGroup_should_redirect()
		{
			var mock=new FakeServerContext();
			var auth=new FSCAuth(mock, new FSCAuthConfig(), new SimpleUserStore());
			Prep(auth);
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
		[Test]
		public void Login_Should_Send_401_When_Basic()
		{

			var mock=new FakeServerContext();
			var auth=new FSCAuth(mock,new FSCAuthConfig(), new SimpleUserStore(), true);
			auth.Config.AllowBasicAuth=true;
			auth.Config.SiteName="Foo bar";
			Prep(auth);
			bool threw=false;
			try
			{
				auth.RequiresAuthentication();
			}
			catch(HttpException e)
			{
				Assert.AreEqual(401, e.GetHttpCode());
				return;
			}
			Assert.Fail("Should not reach here");

		}
	}
}


using System;
using NUnit.Framework;
using Earlz.BarelyMVC;
using Earlz.BarelyMVC.ViewEngine;

namespace BarelyMVC.Tests
{
	[TestFixture]
	public class RouterTests
	{
		class TestController : HttpController
		{
			public TestController(IServerContext c) : base(c)
			{
			}
			public IBarelyView Tester()
			{
				return new WrapperView("meh");
			}
		}
		[Test]
		public void Controller_LinksToThis()
		{
			var router=new Router();
			var controller=router.Controller((c) => new TestController(c));
			Assert.AreEqual(router, controller.Router);
		}
		[Test]
		public void GetRoutes_HasNoEffects()
		{
			var router=new Router();
			router.AddRoute(new Route());
			router.AddRoute(new Route());
			var r=new Route();
			router.AddRoute(r);
			var x=new Route();
			router.GetRoutes()[2]=x;
			Assert.AreNotEqual(router.GetRoutes()[2], x);
		}
	}
}

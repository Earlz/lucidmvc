using System;
using NUnit.Framework;
using Earlz.BarelyMVC;
using Earlz.BarelyMVC.ViewEngine;
using Moq;
using System.Linq;

namespace BarelyMVC.Tests
{
	[TestFixture]
	public class ControllerBoxTests
	{
		class TestController : HttpController
		{
			public TestController(IServerContext c) : base(c)
			{
			}
			public IBarelyView Test()
			{
				return new WrapperView("foo");
			}
		}

		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void With_CreatesProperDelegate()
		{
			var r=new Router();
			var t=r.Controller((c) => new TestController(c));
			t.Handles("/foo").With((tester) => tester.Test());
			Assert.AreEqual("foo", t.Current.Responder(null).RenderView());
		}
		[Test]
		public void Handles_ReturnsLimitedInterface()
		{
			var t=new Router().Controller((c) => new TestController(c));
			Assert.IsTrue(t.Handles("") is IControllerRoute<TestController>);

		}
		[Test]
		public void Handles_AddsToRouter()
		{
			var mock=new Mock<Router>();
			mock.Setup(x=>x.AddRoute(It.IsAny<Route>())).Verifiable();
			var c=new ControllerBox<TestController>(mock.Object, null);
			c.Handles("foo");
			mock.Verify();
		}
		[Test]
		public void Handles_TwiceCreatesNewRoute()
		{
			var r=new Router();
			var ctrl=r.Controller<TestController>(null);
			ctrl.Handles("/foo");
			var first=ctrl.Current;
			ctrl.Handles("/biz");
			var second=ctrl.Current;
			Assert.IsTrue(first.Pattern.IsMatch("/foo"));
			Assert.IsTrue(second.Pattern.IsMatch("/biz"));
			Assert.IsTrue(r.GetRoutes().Contains(first));
			Assert.IsTrue(r.GetRoutes().Contains(second));
			Assert.AreEqual(2, r.GetRoutes().Count());
		}
	}
}


using System;
using NUnit.Framework;
using Earlz.BarelyMVC;
using Earlz.BarelyMVC.ViewEngine;
using Moq;
using System.IO;

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
		[Test]
		public void Execute_FindsProperRoute()
		{
			var router=new Router();
			router.AddRoute(new Route());
			var rt=new Route();
			router.AddRoute(rt);
			rt.Pattern=new FakePatternMatcher("/foo");
			var context=new Mock<IServerContext>();
			rt.Responder=(c) => {
				var view=new WrapperView("foo");
				Assert.AreEqual(c, context.Object);
				return view;
			};
			context.Setup(x=>x.RequestUrl).Returns(new Uri("http://meh.com/foo"));
			var writer=new StringWriter();
			context.Setup(x=>x.Writer).Returns(writer);

			var res=router.Execute(context.Object);
			Assert.IsTrue(res);

			Assert.AreEqual("foo", writer.GetStringBuilder().ToString());
		}
		[Test]
		public void Execute_ReturnsFalseOnNotFound()
		{
			var router=new Router();
			router.AddRoute(new Route());
			router.AddRoute(new Route{Pattern=new FakePatternMatcher("/meh")});
			var context=new Mock<IServerContext>();
			context.Setup(x=>x.RequestUrl).Returns(new Uri("http://meh.com/foo"));
			Assert.IsFalse(router.Execute(context.Object));
		}
	}
}


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
			var context=new FakeServerContext();
			context.RawHttpMethod="GET";
			context.RequestUrl=new Uri("http://meh.com/foo");
			rt.Responder=(c) => {
				var view=new WrapperView("foo");
				Assert.AreEqual(c, context);
				return view;
			};

			var res=router.Execute(context);
			Assert.IsTrue(res);

			Assert.AreEqual("foo", context.WrittenText());
		}
		[Test]
		public void Execute_ReturnsFalseOnNotFound()
		{
			var router=new Router();
			router.AddRoute(new Route());
			router.AddRoute(new Route{Pattern=new FakePatternMatcher("/meh")});
			var context=new FakeServerContext();
			context.RequestUrl=new Uri("http://meh.com/foo");
			Assert.IsFalse(router.Execute(context));
		}
		[Test]
		public void Execute_RespectsHttpMethodsAllowed()
		{
			var router=new Router();
			router.AddRoute(new Route
			{
				Responder=(c) => new WrapperView("foo"),
				AllowedMethods=new string[]{"POST"},
				Pattern=new FakePatternMatcher("/foo")
			});
			var context=new FakeServerContext();
			context.RequestUrl=new Uri("http://meh.com/foo");
			context.RawHttpMethod="GET";
			Assert.IsFalse(router.Execute(context));
		}
		public void Execute_RespectsMultipleAllowedHttpMethods()
		{
			var router=new Router();
			router.AddRoute(new Route
			{
				Responder=(c) => new WrapperView("foo"),
				AllowedMethods=new string[]{"POST", "meh"},
				Pattern=new FakePatternMatcher("/foo")
			});
			var context=new FakeServerContext();
			context.RequestUrl=new Uri("http://meh.com/foo");
			context.RawHttpMethod="meh";
			Assert.IsTrue(router.Execute(context));
		}
	}
}


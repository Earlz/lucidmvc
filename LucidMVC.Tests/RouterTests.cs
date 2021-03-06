using System;
using NUnit.Framework;
using Earlz.LucidMVC;
using Earlz.LucidMVC.ViewEngine;
using Moq;
using System.IO;
using System.Collections.Generic;

namespace Earlz.LucidMVC.Tests
{
	[TestFixture]
	public class RouterTests
	{
		class TestController : HttpController
		{
			public TestController(RequestContext c) : base(c)
			{
			}
			public ILucidView Tester()
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
			context.HttpMethod="GET";
			context.RequestUrl=new Uri("http://meh.com/foo");
			rt.Responder=(RequestContext c,ref bool skip ) => {
				var view=new WrapperView("foo");
				Assert.AreEqual(c.Context, context);
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
				Responder=(RequestContext c, ref bool skip) => new WrapperView("foo"),
				AllowedMethods=new string[]{"POST"},
				Pattern=new FakePatternMatcher("/foo")
			});
			var context=new FakeServerContext();
			context.RequestUrl=new Uri("http://meh.com/foo");
			context.HttpMethod="GET";
			Assert.IsFalse(router.Execute(context));
		}
		public void Execute_RespectsMultipleAllowedHttpMethods()
		{
			var router=new Router();
			router.AddRoute(new Route
			{
				Responder=(RequestContext c, ref bool skip) => new WrapperView("foo"),
				AllowedMethods=new string[]{"POST", "meh"},
				Pattern=new FakePatternMatcher("/foo")
			});
			var context=new FakeServerContext();
			context.RequestUrl=new Uri("http://meh.com/foo");
			context.HttpMethod="meh";
			Assert.IsTrue(router.Execute(context));
		}
		[Test]
		public void Execute_DefaultsToOnlyGetHttpMethod()
		{
			var router=new Router();
			var r=new Route
			{
				Responder=(RequestContext c, ref bool skip) => new WrapperView("foo"),
				AllowedMethods=null,
				Pattern=new FakePatternMatcher("/foo")
			};
			router.AddRoute(r);
			var context=new FakeServerContext();
			context.RequestUrl=new Uri("http://meh.com/foo");
			context.HttpMethod="post";
			Assert.IsFalse(router.Execute(context));
			context.HttpMethod="get";
			Assert.IsTrue(router.Execute(context));
		}
        [Test]
        public void Execute_ChecksValidators()
        {
			var router=new Router();
            var parameters = new ParameterDictionary();
            parameters.Add("id", "1234");
            var goodvalidators = new List<RouteParamsMustMatch>();
            goodvalidators.Add(x => x["id"] == "1234");
            var good = new Route
            {
                Responder = (RequestContext c, ref bool skip) => new WrapperView("good"),
                Pattern = new FakePatternMatcher("/foo/1234", parameters),
                ParameterValidators = goodvalidators
            };
            router.AddRoute(good);
			var context=new FakeServerContext();
			context.RequestUrl=new Uri("http://meh.com/foo/1234");
			context.HttpMethod="get";
			Assert.IsTrue(router.Execute(context));
        }
        [Test]
        public void Execute_ChecksValidatorsForBadParameters()
        {
			var router=new Router();
            var parameters = new ParameterDictionary();
            parameters.Add("id", "1234");
            var badvalidators = new List<RouteParamsMustMatch>();
            badvalidators.Add(x => x["id"] == "xxx");
			var bad=new Route
			{
				Responder=(RequestContext c, ref bool skip) => new WrapperView("bad"),
				Pattern=new FakePatternMatcher("/foo/1234", parameters),
                ParameterValidators=badvalidators
			};
			router.AddRoute(bad);
			var context=new FakeServerContext();
			context.RequestUrl=new Uri("http://meh.com/foo/1234");
			context.HttpMethod="get";
			Assert.IsFalse(router.Execute(context));
        }
		[Test]
		public void Execute_PopulatesHttpController()
		{

		}
	}
}


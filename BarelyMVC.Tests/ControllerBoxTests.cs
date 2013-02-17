using System;
using NUnit.Framework;
using Earlz.BarelyMVC;
using Earlz.BarelyMVC.ViewEngine;
using Moq;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Earlz.BarelyMVC.Tests
{
	[TestFixture]
	public class ControllerBoxTests
	{
		class TestController : HttpController
		{
			public TestController(RequestContext c) : base(c)
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
			var request=new RequestContext(null, null, null, null);
			Assert.AreEqual("foo", t.Current.Responder(request).ToString());
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
			Assert.IsTrue(first.Pattern.Match("/foo").IsMatch);
			Assert.IsTrue(second.Pattern.Match("/biz").IsMatch);
			Assert.IsTrue(r.GetRoutes().Contains(first));
			Assert.IsTrue(r.GetRoutes().Contains(second));
			Assert.AreEqual(2, r.GetRoutes().Count());
		}
		[Test]
		public void Handles_AcceptsCustomPatterns()
		{
			var r=new Router();
			var ctrl=r.Controller<TestController>(null);
			var p=new Mock<IPatternMatcher>();
			p.Setup(x=>x.Match("/foo")).Returns(new MatchResult(true, null)).Verifiable();
			ctrl.Handles(p.Object);
			Assert.IsTrue(ctrl.Current.Pattern.Match("/foo").IsMatch);
			p.Verify();

		}
		[Test]
		public void Handles_StringDefaultsToSimplePattern()
		{
			var r=new Router();
			var ctrl=r.Controller<TestController>(null);
			ctrl.Handles("/foo");
			Assert.IsTrue(ctrl.Current.Pattern is SimplePattern);
			Assert.IsTrue(ctrl.Current.Pattern.Match("/foo").IsMatch);
		}
		[Test]
		public void Allows_CreatesAllowedMethods()
		{
			var r=new Router();
			var ctrl=r.Controller<TestController>(null);
			ctrl.Handles("/foo").Allows("biz");
			Assert.IsNotNull(ctrl.Current.AllowedMethods);
		}

		[Test]
		public void Allows_AddsMethods()
		{
			var r=new Router();
			var ctrl=r.Controller<TestController>(null);
			ctrl.Handles("/foo").Allows("biz").Allows("baz");
			Assert.IsTrue(ctrl.Current.AllowedMethods.Any(x=>x=="biz"));
			Assert.IsTrue(ctrl.Current.AllowedMethods.Any(x=>x=="baz"));
		}

		[Test]
		public void Allows_DoesNotDuplicate()
		{
			var r=new Router();
			var ctrl=r.Controller<TestController>(null);
			ctrl.Handles("/foo").Allows("biz").Allows("biz");
			Assert.IsTrue(ctrl.Current.AllowedMethods.Any(x=>x=="biz"));
			Assert.AreEqual(1, ctrl.Current.AllowedMethods.Count());
		}
		[Test]
		public void Allows_ThrowsNotSupportedForBadType()
		{
			var r=new Router();
			var ctrl=r.Controller<TestController>(null);
			var tmp=ctrl.Handles("/foo");
			ctrl.Current.AllowedMethods=new ReadOnlyCollection<string>(new List<string>());
			bool threw=false;
			try
			{
				tmp.Allows("biz");
			}
			catch(NotSupportedException e)
			{
				threw=true;
			}
			Assert.IsTrue(threw);

		}
	}
}


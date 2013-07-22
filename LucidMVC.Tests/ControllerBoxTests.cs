using System;
using NUnit.Framework;
using Earlz.LucidMVC;
using Earlz.LucidMVC.ViewEngine;
using Moq;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Earlz.LucidMVC.Authentication;

namespace Earlz.LucidMVC.Tests
{
	[TestFixture]
	public class ControllerBoxTests
	{
		class TestController : HttpController
		{
			public TestController(RequestContext c, IAuthMechanism auth=null)
			{
				Authentication=auth;
			}
			public ILucidView Test()
			{
				return new WrapperView("foo");
			}
			public ILucidView TestWithModel(TestModel model)
			{
				return new WrapperView("foo" + model.Foo+(model.Bar ?? ""));
			}
		}
		class TestModel
		{
			public string Foo{ get; set; }
			public string Bar{ get; set; }
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
			bool skip=false;
			Assert.AreEqual("foo", t.Current.Responder(request, ref skip).ToString());
		}
		[Test]
		public void Handles_ReturnsLimitedInterface()
		{
			var t=new Router().Controller((c) => new TestController(c));
			Assert.IsTrue(t.Handles("") is IControllerRoute<TestController, object>);

		}
		[Test]
		public void Handles_AddsToRouter()
		{
			var mock=new Mock<Router>();
			mock.Setup(x=>x.AddRoute(It.IsAny<Route>())).Verifiable();
			var c=new ControllerBox<TestController, object>(mock.Object, null);
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
		[Test]
		public void RequiresAuthentication_AddsAuth()
		{
			var r=new Router();
			var mock=new Mock<IAuthMechanism>();
			var ctrl=r.Controller(x=>new TestController(x, mock.Object));
			ctrl.Handles("/foo").With(x=>x.Test()).RequiresAuthentication();
			bool skip=false;
			ctrl.Current.Responder(new RequestContext(null, r, null, null), ref skip);
			Assert.IsTrue(skip); //should be null and therefore not logged in
			mock.VerifyGet(x=>x.CurrentUser);
		}
		[Test]
		public void Requires_ShouldUseWhenTrue()
		{
			var r=new Router();
			var controller=new TestController(new RequestContext(null, r, null,null));
			var ctrl=r.Controller(x=>controller);
			bool calledrequire=false;
			ctrl.Handles("/foo").With(x=>x.Test()).Requires(
			x => {
				calledrequire=true;
				Assert.AreEqual(controller, x);
				return true;
			});
			bool skip=false;
			ctrl.Current.Responder(null, ref skip);
			Assert.IsFalse(skip);
			Assert.IsTrue(calledrequire);
		}

		[Test]
		public void Requires_ShouldNotUseWhenFalse()
		{
			var r=new Router();
			var controller=new TestController(new RequestContext(null, r, null,null));
			var ctrl=r.Controller(x=>controller);
			bool calledrequire=false;
			ctrl.Handles("/foo").With(x=>x.Test()).Requires(
			x => {
				calledrequire=true;
				return false;
			});
			bool skip=false;
			ctrl.Current.Responder(null, ref skip);
			Assert.IsTrue(skip);
			Assert.IsTrue(calledrequire);
		}
		[Test]
		public void AlsoExecute_Should_Be_Added()
		{
			var r=new Router();
			var controller=new TestController(new RequestContext(null, r, null, null));
			var ctrl=r.Controller(x=>controller);
			bool calledexecute=false;
			ctrl.Handles("/foo").With(x=>x.Test()).AlsoExecute(c=>calledexecute=true);
			bool skip=false;
			ctrl.Current.Responder(null, ref skip);
			Assert.IsTrue(calledexecute);
		}
		[Test]
		public void RootParameter_ShouldBeRootOfRoutes()
		{
			var r=new Router();
			var ctrl=r.Controller(c => new TestController(c), "/foo");
			var tmp=ctrl.Handles("index").With((c) => c.Test());
			Assert.IsTrue(tmp.Current.Pattern.Match("/foo/index").IsMatch);
		}
		[Test]
		public void UsingModel_Should_Use_New_Model()
		{
			var r = new Router();
			var ctrl = r.Controller(c => new TestController(c));
			var foo = ctrl.Handles("/meh").UsingModel(c=>"foo");
			Assert.IsTrue(foo is IControllerRoute<TestController, string>);
		}
		[Test]
		public void With_Using_Model_Should_Use_Correct_Model()
		{
			var r = new Router();
			var ctrl = r.Controller(c => new TestController(c));
			var foo = ctrl.Handles("/foo").UsingModel(
				c => new TestModel(){Foo="bar"}).With((c, m) => c.TestWithModel(m));
			bool trash = false;
			var view=foo.Current.Responder(
				new RequestContext(null, null, null, null), ref trash);
			Assert.AreEqual("foobar", view.ToString());
		}
		[Test]
		public void When_Should_Skip_Incorrect_Models()
		{
			var r = new Router();
			var ctrl = r.Controller(c => new TestController(c));
			var foo = ctrl.Handles("/foo").UsingModel(
				c => new TestModel(){Foo="bar"}).With((c, m) => c.TestWithModel(m))
				.When(m => m.Foo == "foo");
			bool skip = false;
			var view=foo.Current.Responder(
				new RequestContext(null, null, null, null), ref skip);
			Assert.AreEqual(null, view);
			Assert.IsTrue(skip);
		}
		[Test]
		public void When_Should_Not_Skip_Correct_Models()
		{
			var r = new Router();
			var ctrl = r.Controller(c => new TestController(c));
			var foo = ctrl.Handles("/foo").UsingModel(
				c => new TestModel(){Foo="bar"}).With((c, m) => c.TestWithModel(m))
				.When(m => m.Foo == "bar");
			bool skip = false;
			var view=foo.Current.Responder(
				new RequestContext(null, null, null, null), ref skip);
			Assert.AreNotEqual(null, view);
			Assert.IsFalse(skip);
		}
		[Test]
		public void FromRoute_Should_Populate_Model()
		{
			var r = new Router();
			var ctrl = r.Controller(c => new TestController(c));
			var foo = ctrl.Handles("/foo").
				UsingModel(c => new TestModel()).
				FromRoute().
				With((c, m) => c.TestWithModel(m));
			bool skip = false;
			var routeparams=new ParameterDictionary();
			routeparams.Add("Foo", new string[]{"bar"});
			var view=foo.Current.Responder(
				new RequestContext(null, null, null, routeparams), ref skip);
			Assert.AreEqual("foobar", view.ToString());
		}
		[Test]
		public void FromXXX_Should_Populate_Model_In_An_Additive_Manner()
		{
			var r = new Router();
			var ctrl = r.Controller(c => new TestController(c));
			var foo = ctrl.Handles("/foo").
				UsingModel(c => new TestModel()).
				FromForm().
				FromRoute().
				With((c, m) => c.TestWithModel(m));
			bool skip = false;
			var formparams=new ParameterDictionary();
			formparams.Add("Foo", new string[]{"bar"});
			var context = new FakeServerContext();
			context.Form = formparams;
			var routeparams = new ParameterDictionary();
			routeparams.Add("Bar", new string[]{"baz"});
			var view=foo.Current.Responder(
				new RequestContext(context, null, null, routeparams), ref skip);
			Assert.AreEqual("foobarbaz", view.ToString());
		}
	}
}


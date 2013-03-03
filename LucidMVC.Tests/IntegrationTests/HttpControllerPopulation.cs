using System;
using NUnit.Framework;
using Earlz.BarelyMVC;
using Earlz.BarelyMVC.ViewEngine;

namespace Earlz.BarelyMVC.Tests
{
	[TestFixture]
	[Category("integration")]
	public class HttpControllerPopulation
	{
		[SetUp]
		public void SetUp()
		{

		}
		[Test]
		public void ControllerGetsPopulated()
		{
			var router=new Router();
			var home=router.Controller(c => new HomeController(c));
			home.Handles("/home").With(x=>x.GetHome());
			var context=new FakeServerContext();
			context.RequestUrl=new Uri("http://foo.bar/home");
			context.HttpMethod="get";
			router.Execute(context);
			Assert.AreEqual("mehfoobar", context.WrittenText());
		}

		class HomeView : BarelyViewBase
		{
			public string Name;
			public string Foo;
			public override void RenderView (System.IO.TextWriter outputStream)
			{
				outputStream.Write(Name);
				outputStream.Write(Foo);
			}
		}

		class HomeController : HttpController
		{
			public HomeController(RequestContext c) : base(c){}
			public IBarelyView GetHome()
			{
				Assert.IsNotNull(this.Context);
				Assert.IsNotNull(this.RouteParams);
				Assert.IsNotNull(this.RouteRequest);
				Write("meh");
				return new HomeView(){Name="foo", Foo="bar"};
			}
		}
	}
}


using System;
using NUnit.Framework;
using Earlz.LucidMVC;
using Earlz.LucidMVC.ViewEngine;

namespace Earlz.LucidMVC.Tests
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
			Assert.AreEqual("foobar", context.WrittenText());
		}

		class HomeView : LucidViewBase
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
			public ILucidView GetHome()
			{
				Assert.IsNotNull(this.Context);
				Assert.IsNotNull(this.RouteParams);
				Assert.IsNotNull(this.RouteRequest);
				return new HomeView(){Name="foo", Foo="bar"};
			}
		}
	}
}


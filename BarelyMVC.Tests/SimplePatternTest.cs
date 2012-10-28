using System;
using NUnit.Framework;
using Earlz.BarelyMVC;

namespace BarelyMVC.Tests
{
	[TestFixture]
	public class SimplePatternTest
	{
		[Test]
		public void RootUrl()
		{
			var x=new SimplePattern("/");
			Assert.IsTrue(x.IsMatch("/"));
			Assert.IsFalse(x.IsMatch("/foo/bar"));
			Assert.IsFalse(x.IsMatch("/123"));
		}
		[Test]
		public void BasicParameters()
		{
			var x=new SimplePattern("/{foo}/{bar}");
			Assert.IsTrue(x.IsMatch("/biz/baz"));
			Assert.AreEqual(x.Params["foo"],"biz");
			Assert.AreEqual(x.Params["bar"],"baz");
			Assert.IsFalse(x.IsMatch("/foo/bar/biz/baz"));
		}
	}
}


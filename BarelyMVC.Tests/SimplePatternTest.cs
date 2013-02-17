using System;
using NUnit.Framework;
using Earlz.BarelyMVC;

namespace Earlz.BarelyMVC.Tests
{
    [TestFixture]
    public class SimplePatternTest
    {
        [Test]
        public void RootUrl()
        {
            var x=new SimplePattern("/");
            Assert.IsTrue(x.Match("/").IsMatch);
            Assert.IsFalse(x.Match("/foo/bar").IsMatch);
            Assert.IsFalse(x.Match("/123").IsMatch);
        }
        [Test]
        public void BasicParameters()
        {
            var x=new SimplePattern("/{foo}/{bar}");
			var res=x.Match("/biz/baz");
            Assert.IsTrue(res.IsMatch);
            Assert.AreEqual(res.Params["foo"],"biz");
            Assert.AreEqual(res.Params["bar"],"baz");
            Assert.IsFalse(x.Match("/foo/bar/biz/baz").IsMatch);
        }
        [Test]
        public void ChoiceParameters()
        {
            var x=new SimplePattern("/{foo=[fizz,buzz,bazz]}/{bar=[a,b,c]}");
            Assert.IsTrue(x.Match("/fizz/b").IsMatch);
			var res=x.Match("/bazz/c");
            Assert.IsTrue(res.IsMatch);
            Assert.AreEqual(res.Params["foo"],"bazz");
            Assert.AreEqual(res.Params["bar"],"c");
            Assert.IsFalse(x.Match("/fizz/c/d").IsMatch);
            Assert.IsFalse(x.Match("/meh/blah").IsMatch);
            Assert.IsFalse(x.Match("/fizz/meh").IsMatch);
        }
        [Test]
        public void MixedParameters()
        {
            var x=new SimplePattern("/{foo=[fizz,buzz,bazz]}/{bar}/meh");
            Assert.IsTrue(x.Match("/fizz/foobar/meh").IsMatch);
			var res=x.Match("/bazz/biz/meh");
            Assert.IsTrue(res.IsMatch);
            Assert.AreEqual(res.Params["foo"],"bazz");
            Assert.AreEqual(res.Params["bar"], "biz");
            Assert.IsFalse(x.Match("/blah/foo/meh").IsMatch);
            Assert.IsFalse(x.Match("/buzz/bar/foo").IsMatch);
        }
        [Test]
        public void WhereParameters()
        {
            var x=new SimplePattern("/{foo}/{id}").Where("id",GroupMatchType.AlphaNumeric);
            Assert.IsTrue(x.Match("/meh/fjk2j49a0").IsMatch);
            Assert.IsFalse(x.Match("/meh/hjkfd-jhk@$^%jkf").IsMatch);
            x.Where("id",GroupMatchType.HexString);
            Assert.IsTrue(x.Match("/meh/194abdf").IsMatch);
            Assert.IsFalse(x.Match("/meh/10a9jkot").IsMatch);
            x.Where("id",GroupMatchType.Float);
            Assert.IsTrue(x.Match("/meh/102.29").IsMatch);
            Assert.IsTrue(x.Match("/meh/-1024.29").IsMatch);
            Assert.IsFalse(x.Match("/meh/193jhkf").IsMatch);
            x.Where("id",GroupMatchType.Integer);
            Assert.IsTrue(x.Match("/meh/123456").IsMatch);
            Assert.IsTrue(x.Match("/meh/-12345").IsMatch);
            Assert.IsFalse(x.Match("/meh/194jfjkd").IsMatch);
        }
		[Test]
		public void ShortcutTest()
		{
			SimplePattern.AddShortcut("foo", "/biz/{baz}");
			var x=new SimplePattern("/meh/{!foo!}");
			var res=x.Match("/meh/biz/foo");
			Assert.IsTrue(res.IsMatch);
			Assert.AreEqual("foo", res.Params["baz"]);
			Assert.IsFalse(x.Match("/meh/foo").IsMatch);
		}

    }
}


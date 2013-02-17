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
        [Test]
        public void ChoiceParameters()
        {
            var x=new SimplePattern("/{foo=[fizz,buzz,bazz]}/{bar=[a,b,c]}");
            Assert.IsTrue(x.IsMatch("/fizz/b"));
            Assert.IsTrue(x.IsMatch("/bazz/c"));
            Assert.AreEqual(x.Params["foo"],"bazz");
            Assert.AreEqual(x.Params["bar"],"c");
            Assert.IsFalse(x.IsMatch("/fizz/c/d"));
            Assert.IsFalse(x.IsMatch("/meh/blah"));
            Assert.IsFalse(x.IsMatch("/fizz/meh"));
        }
        [Test]
        public void MixedParameters()
        {
            var x=new SimplePattern("/{foo=[fizz,buzz,bazz]}/{bar}/meh");
            Assert.IsTrue(x.IsMatch("/fizz/foobar/meh"));
            Assert.IsTrue(x.IsMatch("/bazz/biz/meh"));
            Assert.AreEqual(x.Params["foo"],"bazz");
            Assert.AreEqual(x.Params["bar"], "biz");
            Assert.IsFalse(x.IsMatch("/blah/foo/meh"));
            Assert.IsFalse(x.IsMatch("/buzz/bar/foo"));
        }
        [Test]
        public void WhereParameters()
        {
            var x=new SimplePattern("/{foo}/{id}").Where("id",GroupMatchType.AlphaNumeric);
            Assert.IsTrue(x.IsMatch("/meh/fjk2j49a0"));
            Assert.IsFalse(x.IsMatch("/meh/hjkfd-jhk@$^%jkf"));
            x.Where("id",GroupMatchType.HexString);
            Assert.IsTrue(x.IsMatch("/meh/194abdf"));
            Assert.IsFalse(x.IsMatch("/meh/10a9jkot"));
            x.Where("id",GroupMatchType.Float);
            Assert.IsTrue(x.IsMatch("/meh/102.29"));
            Assert.IsTrue(x.IsMatch("/meh/-1024.29"));
            Assert.IsFalse(x.IsMatch("/meh/193jhkf"));
            x.Where("id",GroupMatchType.Integer);
            Assert.IsTrue(x.IsMatch("/meh/123456"));
            Assert.IsTrue(x.IsMatch("/meh/-12345"));
            Assert.IsFalse(x.IsMatch("/meh/194jfjkd"));
        }
		[Test]
		public void ShortcutTest()
		{
			SimplePattern.AddShortcut("foo", "/biz/{baz}");
			var x=new SimplePattern("/meh/{!foo!}");
			Assert.IsTrue(x.IsMatch("/meh/biz/foo"));
			Assert.AreEqual("foo", x.Params["baz"]);
			Assert.IsFalse(x.IsMatch("/meh/foo"));
		}

    }
}


using System;
using NUnit.Framework;
using Earlz.BarelyMVC;

namespace BarelyMVC.Tests
{
	[TestFixture]
	public class SlugifyTest
	{
		[Test]
		public void TestQuotes()
		{
			Assert.AreEqual("foo-bar", Routing.Slugify("\"foo\" bar"));
			Assert.AreEqual("testing-a-real-whatever-post", Routing.Slugify("Testing a \"real\"/ ''whatever post"));
			Assert.AreEqual("foo-meh-bar-tmp", Routing.Slugify("\"foo / meh /bar /tmp/"));
		}

	}
}


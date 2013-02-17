using System;
using NUnit.Framework;
using Earlz.BarelyMVC;

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
		}
	}
}


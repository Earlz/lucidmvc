using System;
using NUnit.Framework;
using Earlz.BarelyMVC;

namespace BarelyMVC.Tests
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


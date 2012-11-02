using System;
using NUnit.Framework;
using Earlz.BarelyMVC.ViewEngine.Internal;

namespace BarelyMVC.Tests
{
	[TestFixture]
	public class CodeDataObjects
	{
		[Test]
		public void PropertyTest()
		{
			//TODO
			var p=new Property();
			p.Accessibility="public";
			p.GetMethod="get{foo}";
			p.SetMethod="set{foo}";
			p.Name="Foo";
			p.Type="Bar";

			//Assert.AreEqual(p.ToString(),
			//                Property.GetTab(2)+"public Bar Foo"+Property.GetTab(2)+"{
		}
	}
}


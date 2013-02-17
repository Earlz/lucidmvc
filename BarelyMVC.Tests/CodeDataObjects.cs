using System;
using NUnit.Framework;
using Earlz.BarelyMVC.ViewEngine.Internal;
using System.Text.RegularExpressions;

namespace Earlz.BarelyMVC.Tests
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
            var regex=new Regex(@"[^\w{};]", RegexOptions.Multiline);
            //compare everything with most characters stripped out (the beginning summary is for XML documentation)
            Assert.AreEqual(regex.Replace(p.ToString(), ""), "summarysummarypublicBarFoo{get{foo}set{foo}}"); 
        }
    }
}


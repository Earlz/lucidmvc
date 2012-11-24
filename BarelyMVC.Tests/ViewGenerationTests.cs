using System;
using System.Linq;
using System.Collections.Generic;
using Earlz.BarelyMVC.ViewEngine.Internal;
using NUnit.Framework;

namespace BarelyMVC.Tests
{
    [TestFixture]
    public class ViewGenerationTests
    {
        [Test]
        public void EnsureVariablesCreated()
        {
            string view=
@"{@
foo as bar;
list as List<T>;
@}
Hello there {=foo=}
";
            var gen=new ViewGenerator("test", "test",false, false, "defaultwriter");
            gen.Input=view;
            gen.BaseClass="defaultbase";
            gen.Generate();
            Assert.IsTrue(gen.Properties.Any(x=>x.Name=="foo" && x.Accessibility.Contains("public") && x.Accessibility.Contains("virtual") && x.Type=="bar"));
            Assert.AreEqual(gen.Properties.Count(x=>x.Name=="foo"), 1); //ensure not greater than 1
            Assert.IsTrue(gen.Properties.Any(x=>x.Name=="list" && x.Type=="List<T>"));

        }
        [Test]
        public void EnsureFlashPassthrough()
        {
            string view=" foo bar!";
            var gen=new ViewGenerator("test", "test", false, false, "defaultwriteR");
            gen.BaseClass="defaultbase";
            gen.Input=view;
            gen.Generate();
            Assert.IsTrue(gen.Properties.Any((x)=>
                {
                    string tmp=x.ToString().Replace(" ","");
                    return tmp.Contains("returnLayout.Flash") && tmp.Contains("Layout.Flash=value");
                }
            ));
        }

    }
}


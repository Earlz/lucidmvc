using System;
using System.Linq;
using System.Collections.Generic;
using Earlz.LucidMVC.ViewEngine.Internal;
using NUnit.Framework;

namespace Earlz.LucidMVC.Tests
{
    [TestFixture]
    public class ViewGenerationTests
    {
		ViewConfiguration Config;
		[TestFixtureSetUp]
		public void SetUp()
		{
			Config=new ViewConfiguration
			{
					AutoInterfaces=false,
					DefaultNamespace="Earlz.LucidMVC.MyViews",
					RenderDirectly=false,
					DetectChainedNulls=false,
					BaseClass="Earlz.LucidMVC.ViewEngine.Internal.LucidViewDummy",
					UsePartials=false,
					UseInternal=false,
			};
		}
        [Test]
        public void EnsureVariablesCreated()
        {
            string view=
@"{@
foo as bar;
list as List<T>;

virtual protected biz as
baz;

{test doc}
meh as that;

@}
Hello there {=foo=}
";
            var gen=new ViewGenerator("test", Config);
            gen.Input=view;
            gen.BaseClass="defaultbase";
            gen.Generate();
            Assert.IsTrue(gen.Properties.Any(x=>x.Name=="foo" && x.Accessibility.Contains("public") && x.Type=="bar"));
			Assert.IsTrue(gen.Properties.Any(x=>x.Name=="biz" && x.Accessibility.Contains("protected") && x.Accessibility.Contains("virtual") && x.Type=="baz"));
			Assert.IsTrue(gen.Properties.Any(x=>x.Name=="meh" && x.PrefixDocs.Contains("test doc")));
            Assert.AreEqual(gen.Properties.Count(x=>x.Name=="foo"), 1); //ensure not greater than 1
            Assert.IsTrue(gen.Properties.Any(x=>x.Name=="list" && x.Type=="List<T>"));

        }
        [Test]
        public void EnsureFlashPassthrough()
        {
            string view=" foo bar!";
            var gen=new ViewGenerator("test",  Config);
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
		[Test]
		public void NoSpacingBetweenDeclarations()
		{
			string view="{@ foo as bar @}{@ meh as biz @}";
			var gen=new ViewGenerator("test", Config);
			gen.BaseClass="defaultbase";
			gen.Input=view;
			gen.Generate();
			Assert.IsTrue(gen.Properties.Any(x=>x.Name=="foo"));
			Assert.IsTrue(gen.Properties.Any(x=>x.Name=="meh"));
		}

    }
}


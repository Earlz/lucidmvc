using System;
using NUnit.Framework;
using Earlz.BarelyMVC.Caching.Experimental;
using System.Threading;
using Earlz.BarelyMVC.Caching;

namespace BarelyMVC.Tests
{
	public class CacheTest
	{
		public static class TestCache
		{
			static TestCache()
			{
				Cacher.Setup("Testfoo", new CacheInfo());
				//testdictionary=Cacher.SetupDictionary("testdictionary", new CacheInfo());
			}
			public static ICacheMechanism Cacher=new MockCacheMechanism();

			public static string Testfoo
			{
				get
				{
					return Cacher.Get<string>("Testfoo");
				}
				set
				{
					Cacher.Set("Testfoo", value);
				}
			}
			public static ICacheDictionary<int, string> testdictionary{get;set;}

		}
		MockCacheMechanism Cacher;
		[TestFixtureSetUp]
		public void Setup()
		{
			Cacher=(MockCacheMechanism)TestCache.Cacher;
		}
		[TestFixtureTearDown]
		public void Teardown()
		{
			Cacher.Reset();
		}

		[Test]
		public void BasicOperations()
		{
			//these look really retarded, but remember that the methods behind these properties are what we're testing
			//test initial add
			TestCache.Testfoo="Foo";
			Assert.AreEqual("Foo", TestCache.Testfoo);
			//test that clearing the cache doesn't cause any exceptions
			Cacher.Reset();
			Assert.AreEqual(null, TestCache.Testfoo);
			//test replacement
			TestCache.Testfoo="meh";
			Assert.AreEqual("meh", TestCache.Testfoo);
			TestCache.Testfoo="biz";
			Assert.AreEqual("biz", TestCache.Testfoo);
		}
		[Test]
		public void DictionaryOperations()
		{
			var d=(TrackingCacheDictionary<int, string>)TestCache.testdictionary;
			d.Clear();
			d[0]="foo";
			d[1]="bar";
			Assert.AreEqual(d[0], "foo");
			Assert.AreEqual(d[1], "bar");

			Assert.AreEqual(2, d.TrackedCount);

			Cacher.Reset();
			Assert.AreEqual(2, d.TrackedCount);
			Assert.AreEqual(d[0], null);
			Assert.AreEqual(d[1], null);
			Assert.AreEqual(0, d.TrackedCount); //ensure keys are removed after we know they don't exist

			d[0]="foo";
			d[0]="meh";
			Assert.AreEqual(1, d.TrackedCount); //ensure no duplicate keys for the same object
			Assert.AreEqual(d[0], "meh");
			//Assert.AreEqual(d.Remove(0), "meh");
			Assert.AreEqual(0, d.TrackedCount); //ensure keys removed after Remove()
			Assert.AreEqual(d[0], null);
			d[0]="biz";
			d[10]="baz";
			Assert.AreEqual(d[2], null);
			d.Clear();
			Assert.AreEqual(0, d.TrackedCount); //ensure empty after clearing
			Assert.AreEqual(Cacher.Cache.Count, 0);

		}

		[Test]
		public void ConcurrencyTest()
		{
			long runtime=1000 * 10000; //ticks (milliseconds * 100ns)
			int insanity=6; //number of threads to spawn of each function
			//var randomread=new Random();
			//var randomwrite=new Random();
			Exception monkeycatch=null;
			ThreadStart monkey=()=> //the thing that screws up our cache
			{
				try
				{
					Cacher.Reset();
					Thread.Sleep(10);
					TestCache.testdictionary.Clear();
					Thread.Sleep(10);
					TestCache.testdictionary.Remove(20);
					Thread.Sleep(10);
					TestCache.testdictionary[20]=null;
					Thread.Sleep(10);
				}
				catch(Exception e)
				{
					monkeycatch=e;
				}
			};
			Exception writecatch=null;
			ThreadStart write = ()=>
			{
				try
				{
					while(true)
					{
						TestCache.Testfoo=new string('c', 20);
						TestCache.testdictionary[1]=new string('d', 20);
						TestCache.testdictionary[20]=new string('9', 20);
						TestCache.testdictionary[1]=null;
					}
				}
				catch(Exception e)
				{
					writecatch=e;
				}
			};
			Exception readcatch=null;
			ThreadStart read = () =>
			{
				try
				{
					while(true)
					{
						string tmp=TestCache.testdictionary[1];
						tmp=TestCache.Testfoo;
						tmp=TestCache.testdictionary[20];
					}
				}
				catch(Exception e)
				{
					readcatch=e;
				}
			};
			var monkeythreads=new Thread[insanity];
			var readthreads=new Thread[insanity];
			var writethreads=new Thread[insanity];
			for(int i=0;i<insanity;i++)
			{
				monkeythreads[i]=new Thread(monkey);
				readthreads[i]=new Thread(read);
				writethreads[i]=new Thread(write);

			}
			long ms=DateTime.Now.Ticks;
			for(int i=0;i<insanity;i++)
			{
				monkeythreads[i].Start();
				readthreads[i].Start();
				writethreads[i].Start();
			}
			while(DateTime.Now.Ticks < ms+runtime)
			{
				Thread.Sleep(10);
			}
			if(readcatch!=null)
			{
				throw readcatch;
			}
			if(writecatch!=null)
			{
				throw writecatch;
			}
			if(monkeycatch!=null)
			{
				throw monkeycatch;
			}
			for(int i=0;i<insanity;i++)
			{
				readthreads[i].Abort();
				writethreads[i].Abort();
				monkeythreads[i].Abort();
			}
		}

	}
}


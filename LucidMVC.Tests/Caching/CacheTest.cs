using System;
using NUnit.Framework;
using System.Threading;
using Earlz.BarelyMVC.Caching;

namespace Earlz.BarelyMVC.Tests
{
	public class CacheTest
	{
		public static class TestCache
		{
			static TestCache()
			{
				Cacher.KeyInfo.Add("Testfoo", new CacheInfo()); //new CacheInfo(CachePriority.Default));
				testdictionary=Cacher.SetupDictionary<int, string>("testdictionary", new CacheInfo());
			}
			public static ICacheMechanism Cacher=new MockCacheMechanism();

			public static string Testfoo
			{
				get
				{
					return Cacher.TryGet<string>("Testfoo");
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
			Cacher.Cache.Clear();
		}

		[Test]
		public void BasicOperations()
		{
			lock(LockForTest)
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
		}
		[ThreadStatic]
		static object LockForTest=new object(); //TODO make it so this isn't necessary (smarting clearing of the cache within tests)
		[Test]
		public void DictionaryOperations()
		{
			lock(LockForTest)
			{
				var d=(UntrackedCacheDictionary<int, string>)TestCache.testdictionary;
				Cacher.Reset();
				d[0]="foo";
				d[1]="bar";
				Assert.AreEqual("foo", d[0]);
				Assert.AreEqual("bar", d[1]);

				Assert.AreEqual(2, Cacher.Cache.Count);

				Cacher.Cache.Clear();
				//Assert.AreEqual(0, Cacher.Cache.Count);
				Assert.AreEqual(d[0], null);
				Assert.AreEqual(d[1], null);
				Assert.AreEqual(0, Cacher.Cache.Count); //ensure keys are removed after we know they don't exist

				d[0]="foo";
				d[0]="meh";
				Assert.AreEqual(1, Cacher.Cache.Count); //ensure no duplicate keys for the same object
				Assert.AreEqual("meh", d[0]);
				d.Remove(0);
				Assert.AreEqual(0, Cacher.Cache.Count); //ensure keys removed after Remove()
				Assert.AreEqual(null, d[0]);
				d[0]="biz";
				d[10]="baz";
				Assert.AreEqual(null, d[2]);
				Cacher.Cache.Clear();
				Assert.AreEqual(0, Cacher.Cache.Count); //ensure empty after clearing
				Assert.AreEqual(Cacher.Cache.Count, 0);
			}
		}

		[Test]
		[Ignore] //I don't know why this doesn't work, but it doesn't. It causes other tests to break because Nunit and Mono Develop's runner is retarded
		//running just this test alone results in passing as of this moment
		public void ConcurrencyTest()
		{
			lock(LockForTest)
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
						Cacher.Cache.Clear();
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
}


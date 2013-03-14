## Welcome!

LucidMVC is a simple MVC framework built ontop of vanilla ASP.Net and especially designed for easy integration on Mono/Linux(though it works on .Net/Windows just as well) 

## What's so awesome about it
1. Clearly see what code will execute
2. Non-strict MVC compliance. If you need to do something non-MVCish, it won't be as easy but it also won't be some huge hack
3. Code generating view engine. Automatically writes matching interfaces for your views for easy mocking and supporting trivially multiple formats seemlessly(HTML, JSON, XML, etc)

More awesomeness to come, but I'll only list things here which are "complete" 

## Design Principles

1. Use as little magic as possible. Everything should be obvious as to how things hook up(ie, few assumptions or "hidden" rules)
2. Be as concise as possible while obeying principle #1
3. Be portable and not tied to one platform. Currently this means it runs on Linux and Windows and has been tested thoroughly under IIS and Apache
4. Be easily testable and allow for the applications built ontop of it to be easily testable (this is a new goal, so it's still being worked on)

## Puzzle Pieces

LucidMVC has always been designed to be minimalist. Despite this, it has quite a few good parts

* Powerful `SimplePattern` pattern matcher
* Highly scaleable `FSCAuth` authentication system
* Powerful `IServerContext` bridge meaning LucidMVC will eventually be portable to places other than ASP.Net.. and very easy to mock and test
* Caching framework capable of transparently caching into places other than ASP.Net and ensuring generation of cache proxy classes is as concise as possible
* Very effecient compile time T4 view engine. Misspell a property in a view and get a compiler error. No magical anonymous classes or dictionaries. Real properties in plain ol' classes
* Fluent router. Very clear what will execute. Place rules on routes to ensure your controllers only receive valid data



## What you won't find

There are a few things I see some people "like" that aren't in LucidMVC. This is a list of things which I will (most likely) never implement:

* Wizards and "rich" HTML controls for authentication that look like crap
* Automatic class discovery and method names mapping to route names(ie, magic)
* Strict MVC enforcement. I make it easy to adhere to, but if you need to do something hacky, I don't actively prevent you from doing it
* GPL or LGPL licensed code. Everything here will be BSD or similar license. Likewise, if you contribute, your code must be under BSD or compatible license
* Asynchronize all the things! (see also: WinRT) 
* XML configuration. This I loathe almost more than magic

## Some Code

Because there is so little magic, you don't have to know any special naming schemes or special attributes to apply. However, this also means there will inheritely be a bit more setup required. 

A barebones example using only the routing and HTTP controller component

		//Global.asax.cs:
		static Router router=null;
		protected virtual void Application_Start (Object sender, EventArgs e)
		{
			router=new Router();
			var blog=router.Controller(ctx => new BlogController(ctx));
			blog.Handles("/blog/{id}").With(ctrl => ctrl.View());
			Routing.AddRoute("/blog/{id}", (r, f) => new TextHandler("ID is "+r["id"]).Get());
		}
		protected virtual void Application_BeginRequest (Object sender, EventArgs e)
		{
			router.Execute(new AspNetServerContext());
		}

Notice that everything but the URL pattern is statically typed and non-magical, yet concise. 

## Current Status

Currently LucidMVC is at late-Alpha. It'll be stabilized into Beta in a few months, but at this moment the API is still unstable. 
If you write code with it right now I can't guarantee you won't have to rewrite it using a different API in 6 months. Although, most of the core(routing/handler/view enigne) API should be stable now. 

Platforms:
* Tested on Windows 8/Visual Studio 2012 w/ IIS express and Cassini
* Requires .Net 4.0. No plans(yet) to upgrade to .Net 4.5
* Tested on Mono 2.10.8/Arch Linux 64 bit
* Thoroughly tested under Apache and tested somewhat well under IIS 7 (IIS 6 is filled with horrors. Not supported)
* Actively developed using MonoDevelop on Arch Linux(so it'll always work pretty good there)

As the API stabilizes, I'll be providing more and more samples. For a large up-to-date(usually) sample though you can view my blog's source code at https://bitbucket.org/earlz/lastyearswishes

Source can be downloaded or forked via Mercurial

* Mercurial@bitbucket: https://bitbucket.org/earlz/Lucidmvc

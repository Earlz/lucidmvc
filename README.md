## Welcome!

BarelyMVC is a simple MVC framework built ontop of vanilla ASP.Net and especially designed for easy integration on Mono/Linux(though it works on .Net/Windows just as well) 

## Design Principles

1. Use as little magic as possible. Everything should be obvious as to how things hook up(ie, few assumptions or "hidden" rules)
2. Be as concise as possible while obeying principle #1
3. Be portable and not tied to one platform. Currently this means it runs on Linux and Windows and has been tested thoroughly under IIS and Apache
4. Be easily testable and allow for the applications built ontop of it to be easily testable (this is a new goal, so it's still being worked on)

## Puzzle Pieces

BarelyMVC has always been designed to be minimalist. Despite this, it has quite a few good parts

* Router
* Powerful `SimplePattern` pattern matcher
* Highly scaleable `FSCAuth` authentication system
* Powerful `IServerContext` bridge meaning BarelyMVC will eventually be portable to places other than ASP.Net.. and very easy to mock and test
* Caching framework capable of transparently caching into places other than ASP.Net and ensuring generation of cache proxy classes is as concise as possible
* Very effecient compile time T4 view engine. Misspell a property in a view and get a compiler error. No magical anonymous classes or dictionaries. Real properties in plain ol' classes

## What you won't find

There are a few things I see some people "like" that aren't in BarelyMVC. This is a list of things which I will (most likely) never implement:

* Wizards and "rich" HTML controls for authentication that look like crap
* Automatic class discovery and method names mapping to route names(ie, magic)
* Strict MVC enforcement. I make it easy to adhere to, but if you need to do something hacky, I don't actively prevent you from doing it
* GPL or LGPL licensed code. Everything here will be BSD or similar license. Likewise, if you contribute, your code must be under BSD or compatible license
* Asynchronize all the things! (see also: WinRT) 

## Some Code

Because there is so little magic, you don't have to know any special naming schemes or special attributes to apply. However, this also means there will inheritely be a bit more setup required. 

A barebones example using only the routing and HTTP handler component

		//Global.asax.cs:
		protected virtual void Application_Start (Object sender, EventArgs e)
		{
			Routing.AddRoute("/blog/{id}", (r, f) => new TextHandler("ID is "+r["id"]).Get());
		}
		protected virtual void Application_BeginRequest (Object sender, EventArgs e)
		{
			Routing.DoRequest(Context,this);
		}

Of course, this super simple example isn't represenative of real-world use cases, but it shows how explicit, yet concise things are. Here is an example route from my blog:

			SimplePattern.AddShortcut("dateslug","/{Year}/{Month}/{Day}/{Time}/{*}");
			Routing.AddRoute("view", HttpMethod.Get, new SimplePattern("/view/{!dateslug!}").Dateify(), 
			                 (r,f)=>new BlogHandler().View(r.Fill(new BlogRouteModel()))
		                 );
		                 
I have a quite complex URL format for my blog. Here I use a shortcut for the pattern matcher and use the `.Dateify` extension method to apply rules that the fields must be integers

And there is also the `r.Fill`. This is the only bit of magic in the system and something unavoidable without forcing all sorts of crappy magic strings. 
Anyway, it'll take the `route` dictionary and use reflection to populate an instance of one of your classes. 

## Current Status

Currently BarelyMVC is at late-Alpha. It'll be stabilized into Beta in a few months, but at this moment the API is still unstable. 
If you write code with it right now I can't guarantee you won't have to rewrite it using a different API in 6 months. Although, most of the core(routing/handler/view enigne) API should be stable now. 

Platforms:
* Tested on Windows 8/Visual Studio 2012 w/ IIS express and Cassini
* Requires .Net 4.0. No plans(yet) to upgrade to .Net 4.5
* Tested on Mono 2.10.8/Arch Linux 64 bit
* Thoroughly tested under Apache and tested somewhat well under IIS 7 (IIS 6 is filled with horrors. Not supported)

As the API stabilizes, I'll be providing more and more samples. For a large up-to-date(usually) sample though you can view my blog's source code at https://bitbucket.org/earlz/lastyearswishes

Source can be downloaded(and possibly pushed?) via Git or Mercurial

* Mercurial@bitbucket: https://bitbucket.org/earlz/barelymvc
* Git@github: https://github.com/Earlz/barelymvc

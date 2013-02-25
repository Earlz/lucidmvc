using System;
using Earlz.BarelyMVC.ViewEngine;
using System.Collections.Generic;

namespace Earlz.BarelyMVC
{
	public delegate T ControllerCreator<T>(RequestContext context);
	public delegate IBarelyView ControllerInvoker<T>(T controller);
	public delegate IBarelyView ControllerResponse(RequestContext context, ref bool skip);
	public delegate bool ControllerRequires<T>(T controller);


	public interface IControllerRoute<T>
	{
		IControllerRoute<T> With(ControllerInvoker<T> invoker);
		IControllerRoute<T> Allows(string httpmethod);
		IControllerRoute<T> RequiresAuthentication();
		IControllerRoute<T> Requires(ControllerRequires<T> requires);
	}


	public class ControllerBox<T> : IControllerRoute<T> where T:HttpController
	{
		public Router Router
		{
			get;
			private set;
		}
		ControllerCreator<T> Creator;
		public ControllerBox(Router r, ControllerCreator<T> creator)
		{
			Router=r;
			Creator=creator;
		}
		/// <summary>
		/// The current route we're messing with for the Fluent API
		/// </summary>
		public Route Current
		{
			get;
			private set;
		}

		public IControllerRoute<T> Handles(string pattern)
		{
			Current=new Route();
			Current.Pattern=new SimplePattern(pattern);
			Router.AddRoute(Current);
			return this;
		}
		public IControllerRoute<T> Handles (IPatternMatcher pattern)
		{
			Current=new Route();
			Current.Pattern=pattern;
			Router.AddRoute(Current);
			ControllerRequirements=new List<ControllerRequires<T>>(); //new up requirements list
			return this;
		}
		List<ControllerRequires<T>> ControllerRequirements=new List<ControllerRequires<T>>();
		IControllerRoute<T> IControllerRoute<T>.With (ControllerInvoker<T> invoker)
		{
			Current.Responder = (RequestContext c, ref bool skip) =>
			{
				var controller=Creator(c);
				foreach(var check in ControllerRequirements)
				{
					if(!check(controller))
					{
						skip=true;
						return null;
					}
				}
				return invoker(controller);
			};
			return this;
		}

		IControllerRoute<T> IControllerRoute<T>.Allows(string method)
		{
			if(Current.AllowedMethods==null)
			{
				Current.AllowedMethods=new List<string>();
			}
			var list=Current.AllowedMethods as ICollection<string>;

			if(list==null && list.IsReadOnly)
			{
				throw new NotSupportedException("To use ControllerBox.Allows, the exact type of AllowedMethods must implement ICollection<string> and it must not be readonly");
			}
			if(!list.Contains(method))
			{
				list.Add(method);
			}
			return this;
		}

		IControllerRoute<T> IControllerRoute<T>.RequiresAuthentication()
		{
			throw new NotImplementedException();
		}
		
		IControllerRoute<T> IControllerRoute<T>.Requires(ControllerRequires<T> requires)
		{
			ControllerRequirements.Add(requires);
			return this;
		}
	}
}


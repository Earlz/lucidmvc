using System;
using Earlz.BarelyMVC.ViewEngine;
using System.Collections.Generic;

namespace Earlz.BarelyMVC
{
	public delegate T ControllerCreator<T>(RequestContext context);
	public delegate IBarelyView ControllerInvoker<T>(T controller);
	public delegate IBarelyView ControllerResponse(RequestContext context, ref bool skip);
	public delegate bool ControllerRequires<T>(T controller);

	public delegate bool RouteParamsMustMatch(ParameterDictionary param);

	public interface IControllerRoute<T, MODEL>
	{
		IControllerRoute<T, MODEL> With(ControllerInvoker<T> invoker);
		IControllerRoute<T, MODEL> Allows(string httpmethod);
		IControllerRoute<T, MODEL> RequiresAuthentication();
		IControllerRoute<T, MODEL> Requires(ControllerRequires<T> requires);
		IControllerRoute<T, MODEL> AlsoExecute(Action<T> action);
		IControllerRoute<T, MODEL> WithRouteParamLike(string param, Func<string, bool> match);
		IControllerRoute<T, MODEL> WithRouteParamsLike(RouteParamsMustMatch matcher);
	}


	public class ControllerBox<T, MODEL> : IControllerRoute<T, MODEL> where T:HttpController
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
			Root="";
		}

		readonly string Root;

		public ControllerBox(Router r, ControllerCreator<T> creator, string root)
		{
			Root=root;
		}
		/// <summary>
		/// The current route we're messing with for the Fluent API
		/// </summary>
		public Route Current
		{
			get;
			private set;
		}

		public IControllerRoute<T, MODEL> Handles(string pattern)
		{
			Current=new Route();
			Current.Pattern=new SimplePattern(pattern);
			Router.AddRoute(Current);
			return this;
		}
		public IControllerRoute<T, MODEL> Handles (IPatternMatcher pattern)
		{
			Current=new Route();
			Current.Pattern=pattern;
			Router.AddRoute(Current);
			ControllerRequirements=new List<ControllerRequires<T>>(); //new up requirements list
			return this;
		}
		List<ControllerRequires<T>> ControllerRequirements=new List<ControllerRequires<T>>();
		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.With (ControllerInvoker<T> invoker)
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

		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.Allows(string method)
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

		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.RequiresAuthentication()
		{
			ControllerRequires<T> check = (ctrl) =>
			{
				return ctrl.Authentication.CurrentUser!=null;
			};
			ControllerRequirements.Add(check);
			return this;
		}
		
		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.Requires(ControllerRequires<T> requires)
		{
			ControllerRequirements.Add(requires);
			return this;
		}


		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.WithRouteParamLike(string param, Func<string, bool> match)
		{
			throw new NotImplementedException();
		}

		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.WithRouteParamsLike(RouteParamsMustMatch matcher)
		{
			throw new NotImplementedException();
		}

		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.AlsoExecute(Action<T> action)
		{
			ControllerRequirements.Add((c)=>{action(c); return true;});
			return this;
		}
	}
}


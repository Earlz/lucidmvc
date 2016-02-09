using System;
using Earlz.LucidMVC.ViewEngine;
using System.Collections.Generic;

namespace Earlz.LucidMVC
{
	public delegate T ControllerCreator<T>(RequestContext context);
	public delegate ILucidView ControllerInvoker<T>(T controller);
	public delegate ILucidView ControllerInvokerWithModel<T, M>(T controller, M model);
	public delegate ILucidView ControllerResponse(RequestContext context, ref bool skip);
	public delegate bool ControllerRequires<T>(T controller);
	public delegate bool ModelRequires<M>(M model);
	public delegate bool RouteParamsMustMatch(ParameterDictionary param);

	public interface IControllerRoute<T, MODEL>
	{
		/// <summary>
		/// Handle the current route with the specified method on the controller
		/// In general, this should usually be the last "word" of the statement
		/// </summary>
		/// <param name="invoker">Invoker.</param>
		IControllerRoute<T, MODEL> With(ControllerInvoker<T> invoker);
		/// <summary>
		/// Handle the current route with the specified method on the controller using a model 
		/// In general, this should usually be the last "word" of the statement
		/// </summary>
		/// <param name="invoker">Invoker.</param>
		IControllerRoute<T, MODEL> With(ControllerInvokerWithModel<T, MODEL> invoker);
		/// <summary>
		/// Specify the allowed HTTP method (GET is allowed by default).
		/// Can be called multiple times for multiple methods.
		/// Is case insensitive.
		/// </summary>
		/// <param name="httpmethod">Httpmethod.</param>
		IControllerRoute<T, MODEL> Allows(string httpmethod);
		/// <summary>
		/// The current route requires authentication
		/// </summary>
		/// <returns>The authentication.</returns>
		IControllerRoute<T, MODEL> RequiresAuthentication();
		/// <summary>
		/// The current route requires the Controller to be in a specified state
		/// </summary>
		/// <param name="requires">Requires.</param>
		IControllerRoute<T, MODEL> Requires(ControllerRequires<T> requires);
		/// <summary>
		/// Also execute this delegate when the route is requested
		/// (useful for logging and analytics)
		/// </summary>
		/// <returns>The execute.</returns>
		/// <param name="action">Action.</param>
		IControllerRoute<T, MODEL> AlsoExecute(Action<T> action);
		/// <summary>
		/// 
		/// </summary>
		IControllerRoute<T, MODEL> WithRouteParamLike(string param, Func<string, bool> match);
		IControllerRoute<T, MODEL> WithRouteParamsLike(RouteParamsMustMatch matcher);
		/// <summary>
		/// Use the specified model
		/// Can only be used if the model has a public default constructor
		/// </summary>
		/// <returns>The model.</returns>
		/// <typeparam name="NEW">The 1st type parameter.</typeparam>
		IControllerRoute<T, NEW> UsingModel<NEW>();
		/// <summary>
		/// Use the delegate to generate an appropriate model 
		/// (which can later be populated by FromRoute and friends)
		/// </summary>
		/// <returns>The model.</returns>
		/// <param name="creator">Creator.</param>
		/// <typeparam name="NEW">The 1st type parameter.</typeparam>
		IControllerRoute<T, NEW> UsingModel<NEW>(Func<T, NEW> creator);
		/// <summary>
		/// Populate the model from passed in HTTP FORM values
		/// Unspecified values won't be modified
		/// </summary>
		/// <returns>The form.</returns>
		IControllerRoute<T, MODEL> FromForm();
		/// <summary>
		/// Populate the model from passed in Route parameter values
		/// Unspecified values won't be modified
		/// </summary>
		/// <returns>The route.</returns>
		IControllerRoute<T, MODEL> FromRoute();
		/// <summary>
		/// Populate the model from passed in query string values
		/// Unspecified values won't be modified
		/// </summary>
		/// <returns>The query string.</returns>
		IControllerRoute<T, MODEL> FromQueryString();
		/// <summary>
		/// The model for this route must be in this particular state to not be skipped.
		/// </summary>
		/// <param name="whenlike">Whenlike.</param>
		IControllerRoute<T, MODEL> When(ModelRequires<MODEL> whenlike);
		Route Current{get;}
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
			Router=r;
			Creator=creator;
			Root=root;
		}
		protected ControllerBox(Router r, ControllerCreator<T> creator, string root, Route current,
		                        Func<T, MODEL> model)
		{
			Router = r;
			Creator = creator;
			Root = root;
			Current = current;
			ModelCreator = model;
		}
		public Func<T, MODEL> ModelCreator
		{
			get;
			set;
		}
		Action<RequestContext, MODEL> ModelPopulator;
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
			if(string.IsNullOrEmpty(Root))
			{
				Current.Pattern=new SimplePattern(pattern);
			}
			else
			{
				Current.Pattern=new SimplePattern(Root+"/"+pattern);
			}
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

		void GenerateResponder(ControllerInvoker<T> invoker, 
		                       ControllerInvokerWithModel<T, MODEL> withmodel)
		{
			Current.Responder = (RequestContext c, ref bool skip) =>
			{
                using (var controller = Creator(c))
                {
                    foreach (var check in ControllerRequirements)
                    {
                        if (!check(controller))
                        {
                            skip = true;
                            return null;
                        }
                    }
                    ILucidView view;
                    if (ModelCreator != null)
                    {
                        var model = ModelCreator(controller);
                        if (ModelPopulator != null)
                        {
                            ModelPopulator(c, model);
                        }
                        foreach (var check in ModelRequirements)
                        {
                            if (!check(model))
                            {
                                skip = true;
                                return null;
                            }
                        }
                        view = withmodel(controller, model);
                    }
                    else
                    {
                        view = invoker(controller);
                    }
                    return view;
                }
			};
		}

		List<ControllerRequires<T>> ControllerRequirements=new List<ControllerRequires<T>>();
		List<ModelRequires<MODEL>> ModelRequirements=new List<ModelRequires<MODEL>>();
		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.With (ControllerInvoker<T> invoker)
		{
			GenerateResponder(invoker, null);
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

		IControllerRoute<T, NEW> IControllerRoute<T, MODEL>.UsingModel<NEW>()
		{
			throw new NotImplementedException();
		}
		IControllerRoute<T, NEW> IControllerRoute<T, MODEL>.UsingModel<NEW>(Func<T, NEW> creator)
		{
			return new ControllerBox<T, NEW>(Router, Creator, Root, Current, creator);
		}
		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.FromForm()
		{
			ModelPopulator += (r, m) =>
			{
				r.Context.Form.Fill(m);
			};
			return this;
		}
		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.FromRoute()
		{
			ModelPopulator += (r, m) =>
			{
				r.RouteParams.Fill(m);
			};
			return this;
		}
		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.FromQueryString()
		{
			throw new NotImplementedException();
		}
		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.When(ModelRequires<MODEL> whenlike)
		{
			ModelRequirements.Add(whenlike);
			return this;
		}

		IControllerRoute<T, MODEL> IControllerRoute<T, MODEL>.With(ControllerInvokerWithModel<T, MODEL> invoker)
		{
			GenerateResponder(null, invoker);
			return this;
		}

	}
}


using System;
using System.Collections.Generic;
using System.Reflection;

namespace Earlz.BarelyMVC
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ParameterIgnoreAttribute : ParameterBaseAttribute
	{
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class ParameterMapAttribute : ParameterBaseAttribute
	{
		public ParameterMapAttribute(string paramname)
		{
			name=paramname;
		}
		private string name;
		public string Name{get{return name;}}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class ParameterDefaultAttribute : ParameterBaseAttribute
	{
		public ParameterDefaultAttribute(object val)
		{
			value=val;
		}
		object value;
		public object Value{get{return value;}}
	}
	public delegate object ParameterConverter(string val);
	
	/*Attributes don't allow method arguments. TODO
		 * [AttributeUsage(AttributeTargets.Property)]
		public class ParameterConverterAttribute : ParameterBaseAttribute
		{
			ParameterConverter Converter;
			public ParameterConverterAttribute(ParameterConverter converter)
			{
				Converter=converter;
			}
			public object Convert(string val)
			{
				return Converter(val);
			}
		}*/
	public abstract class ParameterBaseAttribute : Attribute
	{
	}
	
	///<summary>
	/// This class is will take a dictionary of parameters to values and will in turn take an object
	/// and dynamically fill it in as appropriate using reflection. (hint: awesome!)
	/// Only properties are supported right now
	/// </summary>			
	public class ParameterFiller
	{
		IDictionary<string, string> Values;
		public object Target{get{return target_;}}
		object target_;
		public ParameterFiller(IDictionary<string, string> values, object target)
		{
			target_=target;
			Values=values;
		}
		public object Fill()
		{
			foreach(var p in Target.GetType().GetProperties())
			{
				MatchProperty(p);
			}
			return Target;
		}
		void MatchProperty(PropertyInfo p)
		{
			string matchname=p.Name;
			object defaultval=null;
			//ParameterConverterAttribute converter=null;
			if(!p.CanWrite)
			{
				return; //don't bother if we can't do anything with this property
			}
			foreach(object attrib in Attribute.GetCustomAttributes(p, typeof(ParameterBaseAttribute), true))
			{
				if(attrib is ParameterIgnoreAttribute)
				{
					return; //exit if we need to ignore this property
				}
				else if(attrib is ParameterMapAttribute)
				{
					var tmp=(ParameterMapAttribute)attrib;
					matchname=tmp.Name;
				}else if(attrib is ParameterDefaultAttribute)
				{
					var tmp=(ParameterDefaultAttribute)attrib;
					defaultval=tmp.Value;
				}/*else if(attrib is ParameterConverterAttribute)
					{
						converter=(ParameterConverterAttribute)attrib;
					}*/
			}
			if(!Values.ContainsKey(matchname))
			{
				if(defaultval!=null)
				{
					p.SetValue(Target, defaultval, null); //set to default if not found (and only if we have a default attribute)
				}
				return; //don't bother if we don't find a match
			}
			string val=Values[matchname];
			
			/*if(converter!=null)
				{
					p.SetValue(Target, converter.Convert(val), null);
				}*/
			p.SetValue(Target, ConvertValue(val, p.PropertyType, defaultval), null);
		}
		object ConvertValue(string fromval, Type totype, object defaultval)
		{
			try
			{
				object tmp=Convert.ChangeType(fromval, totype);
				if(tmp==null)
				{
					return defaultval;
				}
				return tmp;
			}
			catch
			{
				return defaultval;
			}
		}
	}

}


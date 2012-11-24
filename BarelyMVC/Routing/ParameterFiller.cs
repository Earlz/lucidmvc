using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;

namespace Earlz.BarelyMVC
{
    public interface IParameterConverter
    {
        /// <summary>
        /// Converts string(or set of strings) to a different type
        /// </summary>
        object Convert(string key, ParameterDictionary dictionary);
    }

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
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterConverterAttribute : ParameterBaseAttribute
    {
        public Type Converter{get;private set;}
        public ParameterConverterAttribute(Type converter)
        {
            Converter=converter;
        }
    }

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
    public class ParameterFiller<T>
    {
        //because this is always deterministic, the cache is extremely simple without a need to ever clear.
        static Dictionary<Type, HashSet<ParameterCacheObject<T>>> Cache=new Dictionary<Type, HashSet<ParameterCacheObject<T>>>();
        ParameterDictionary Values;
        public T Target{get{return target_;}}
        T target_;
        public ParameterFiller(ParameterDictionary values, T target)
        {
            target_=target;
            Values=values;
            if(!Cache.ContainsKey(Target.GetType()))
            {
                Cache.Add(Target.GetType(),new HashSet<ParameterCacheObject<T>>());
                FillCache();
            }
        }
        public object Fill()
        {
            foreach(var p in Cache[Target.GetType()])
            {
                MatchProperty(p);
            }
            return Target;
        }
        void FillCache()
        {
            foreach(var p in Target.GetType().GetProperties())
            {
                CacheProperty(p);
            }
        }
        void CacheProperty(PropertyInfo p)
        {
            string matchname=p.Name;
            object defaultval=null;
            IParameterConverter converter=null;
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
                }else if(attrib is ParameterConverterAttribute)
                {
                    var tmp=(ParameterConverterAttribute)attrib;
                    if(!tmp.Converter.GetInterfaces().Any(x=>x==typeof(IParameterConverter)))
                    {
                        throw new NotSupportedException("The ParameterConverterAttribute must reference a type which implements IParameterConverter");
                    }
                    converter=(IParameterConverter)Activator.CreateInstance(tmp.Converter);
                    var constr=tmp.Converter.GetConstructor(new Type[]{});

                }
            }
            var c=new ParameterCacheObject<T>();
            c.Caller=MakeSetterDelegate(p); //(Action<object>) Delegate.CreateDelegate(typeof(Action<object>), p.GetSetMethod());
            c.Default=defaultval;
            c.MappedName=matchname;
            c.PropertyType=p.PropertyType;
            c.Converter=converter;
            if(!Cache[Target.GetType()].Add(c))
            {
                throw new ApplicationException("The (mapped) property "+c.MappedName+" is already mapped and cached!");
            }
        }
        void MatchProperty(ParameterCacheObject<T> p){
            if(!Values.ContainsKey(p.MappedName))
            {
                if(p.Default!=null)
                {
                    p.Caller(Target, p.Default);
                    //p.SetValue(Target, defaultval, null); //set to default if not found (and only if we have a default attribute)
                }
                return; //don't bother if we don't find a match
            }
            p.Caller(Target, ConvertValue(Values[p.MappedName],p.PropertyType, p.Default, p.Converter));

            /*if(converter!=null)
                {
                    p.SetValue(Target, converter.Convert(val), null);
                }*/
            //p.SetValue(Target, ConvertValue(val, p.PropertyType, defaultval), null);
        }
        object ConvertValue(string fromval, Type totype, object defaultval, IParameterConverter converter)
        {
            try
            {
                object tmp;
                if(converter==null)
                {
                    tmp=Convert.ChangeType(fromval, totype);
                }else{
                    tmp=converter.Convert(fromval, Values);
                }
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
        /// <summary>
        /// http://stackoverflow.com/a/4085834/69742
        /// </summary>
        static Action<T, object> MakeSetterDelegate(PropertyInfo property)
        {
            MethodInfo setMethod = property.GetSetMethod();
            if (setMethod != null && setMethod.GetParameters().Length == 1)
            {
                var target = Expression.Parameter(typeof(T),"");
                var value = Expression.Parameter(typeof(object),"");
                var body = Expression.Call(target, setMethod,
                                           Expression.Convert(value, property.PropertyType));
                return Expression.Lambda<Action<T, object>>(body, target, value)
                    .Compile();
            }
            else
            {
                return null;
            }
        }

        //public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

        /*public static Func<T, object> MakeDelegate<U>(MethodInfo @get)
        {
            var f = (Func<T, U>)Delegate.CreateDelegate(typeof(Func<T, U>), @get);
            return t => f(t);
        }*/

    }

    internal class ParameterCacheObject<T>
    {
        public Action<T, object> Caller{get;set;}
        public Type PropertyType{get;set;}
        public string MappedName{get;set;}
        public object Default{get;set;}
        public IParameterConverter Converter{get;set;}
    }

}


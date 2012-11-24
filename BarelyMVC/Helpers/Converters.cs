using System;

namespace Earlz.BarelyMVC.Converters
{
    public class CheckboxToBoolConverter : IParameterConverter
    {
        public object Convert(string key, ParameterDictionary d)
        {
            if((d[key] ?? "").ToLower()=="yes")
            {
                return true;
            }else{
                return false;
            }
        }
    }
    public class DateTimeConverter : IParameterConverter
    {
        public object Convert(string key, ParameterDictionary d)
        {
            return DateTime.Parse(d[key]);
        }
    }

    public class HtmlEncodeConverter : IParameterConverter
    {
        public object Convert(string key, ParameterDictionary d)
        {
            return System.Web.HttpUtility.HtmlEncode(d[key]);
        }
    }
    public class UrlEncodeConverter : IParameterConverter
    {
        public object Convert(string key, ParameterDictionary d)
        {
            return System.Web.HttpUtility.UrlEncodeUnicode(d[key]);
        }
    }
}


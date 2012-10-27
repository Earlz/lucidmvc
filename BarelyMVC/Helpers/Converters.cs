using System;

namespace Earlz.BarelyMVC.Converters
{
	public class CheckboxToBoolConverter : IParameterConverter
	{
		public object Convert(string key, ParameterDictionary d)
		{
			//if(d[key
			if((d[key] ?? "").ToLower()=="yes")
			{
				return true;
			}else{
				return false;
			}
		}
	}
}


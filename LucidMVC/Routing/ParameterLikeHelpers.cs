using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earlz.LucidMVC
{
    public static class ParameterLikeHelpers
    {
        public static bool IsInt32(string x)
        {
            int outtmp;
            return Int32.TryParse(x, out outtmp);
        }
    }
}

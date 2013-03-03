using System;
using System.Collections.Generic;

namespace Earlz.LucidMVC
{

    public class Route
    {
        public IPatternMatcher Pattern;
		public ControllerResponse Responder;
		public IEnumerable<string> AllowedMethods=null;
		public string Name;
		public bool Secure;
    }
}


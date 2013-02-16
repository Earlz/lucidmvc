using System;
using System.Collections.Generic;

namespace Earlz.BarelyMVC
{

    public class Route
    {
        public IPatternMatcher Pattern;
		public ControllerResponse Responder;
		public IEnumerable<string> AllowedMethods=new string[]{"get"};
		public string Name;
		public bool Secure;
    }
}


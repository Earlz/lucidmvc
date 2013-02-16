using System;

namespace Earlz.BarelyMVC
{

    public class Route
    {
        public IPatternMatcher Pattern;
		public ControllerResponse Responder;
        public HttpMethod AllowedMethods;
		public string Name;
		public bool Secure;
    }
}


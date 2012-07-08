using System;


//Internal base class
namespace Earlz.BarelyMVC.ViewEngine
{
	public interface IBarelyView
	{
		string RenderView();
		string Flash{get;set;}
	}
}
//internal empty namespace declaration
namespace Earlz.BarelyMVC.ViewEngine.Helpers{}

namespace Earlz.BarelyMVC.ViewEngine.Internal{
	public abstract class BarelyViewDummy : Earlz.BarelyMVC.ViewEngine.IBarelyView{ /*This is needed because we have to make a function overridden. */
		public virtual string RenderView(){throw new NotImplementedException();}
		public override string ToString(){return RenderView();}
		public virtual string Flash{get;set;}
	}
}


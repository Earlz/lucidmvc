/*
Copyright (c) 2010 - 2012 Jordan "Earlz/hckr83" Earls  <http://lastyearswishes.com>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.
   
THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL
THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Text;


//Internal base class
namespace Earlz.BarelyMVC.ViewEngine
{
    public interface IBarelyView
    {
        string RenderView();
        string Flash{get;set;}
        bool RenderedDirectly{get;}
    }
    /// <summary>
    /// A psuedo view for wrapping just plain old text. 
    /// </summary>
    public class WrapperView: IBarelyView
    {
        public WrapperView(string text)
        {
            Text=text;
        }
        string Text;
        public virtual bool RenderedDirectly{
            get{
                return false;
            }
        }
        public virtual string RenderView()
        {
            return Text;
        }
        public virtual string Flash{get;set;}
    }
}
//internal empty namespace declaration
namespace Earlz.BarelyMVC.ViewEngine.Helpers{}

namespace Earlz.BarelyMVC.ViewEngine.Internal{
    public abstract class BarelyViewDummy : Earlz.BarelyMVC.ViewEngine.IBarelyView{ /*This is needed because we have to make a function overridden. */
        public virtual string RenderView(){throw new NotImplementedException();}
        public override string ToString(){return RenderView();}
        public virtual string Flash{get;set;}
        public abstract bool RenderedDirectly{get;}
    }
}


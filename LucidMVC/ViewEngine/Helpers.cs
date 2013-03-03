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
using System.IO;


namespace Earlz.LucidMVC.ViewEngine
{
    public interface ILucidView
    {
        void RenderView(TextWriter outputStream);
        string Flash{get;set;}
    }
    /// <summary>
    /// A psuedo view for wrapping just plain old text. 
    /// </summary>
    public class WrapperView : LucidViewBase
    {
        public WrapperView(string text)
        {
            Text=text;
        }
        string Text;
        public override void RenderView(TextWriter outputStream)
        {
			outputStream.Write(Text);
        }
    }
	public abstract class LucidViewBase : Earlz.LucidMVC.ViewEngine.ILucidView{ /*This is needed because we have to make a function overridden and to provide a useful ToString implementation */
        public virtual void RenderView(TextWriter outputStream){throw new NotImplementedException();}
        public override string ToString()
		{
			var s=new StringWriter();
			RenderView(s);
			return s.GetStringBuilder().ToString();
		}
        public virtual string Flash{get;set;}
    }
}
//internal empty namespace declaration
namespace Earlz.LucidMVC.ViewEngine.Helpers{}

    


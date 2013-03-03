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
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;


namespace Earlz.BarelyMVC
{
    /// <summary>
    /// This is just a simple "specialized" dictionary. 
    /// The reason it's values are of IList is because with Form values(and elsewhere), it is completely acceptable for there to be duplicate keys
    /// Note if you use ParameterFiller with this containing duplicate keys, behavior can be undefined. Use a Converter for consistent and guaranteed behavior
    /// </summary>
    public class ParameterDictionary : Dictionary<string, IList<string>>
    {
        public T Fill<T>(T target)
        {
            ParameterFiller<T> p=new ParameterFiller<T>(this, target);
            return (T)p.Fill();
        }
        public void Add(string key, string value)
        {
            Add (key, new List<string>());
            base[key].Add(value);
        }
        public string this[string key]
        {
            get
            {
                if(ContainsKey(key))
                {
                    return base[key].FirstOrDefault();
                }
                return null;
            }
        }
        public IList<string> GetValues(string key)
        {
            return base[key];
        }
        public void SetValues(string key, IList<string> values)
        {
            base[key]=values;
        }
    }
}
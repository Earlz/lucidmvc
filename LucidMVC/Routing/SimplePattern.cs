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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
// needs to parse /products/{action=[view, edit, delete]}/{id} and /products/{action=[new]}
// also needs to parse /products/{id}/{*} where the * is optional for instance for /products/20/some-product-name
using System.Web.Caching;
using System.Web;
using System.Text.RegularExpressions;
using System.Linq;
using Earlz.LucidMVC.Caching;


namespace Earlz.LucidMVC
{
    public enum GroupMatchType
    {
        Integer,
        Float,
        HexString,
        AlphaNumeric
    }

    public class SimplePattern : IPatternMatcher
    {
		static Dictionary<string, string> Shortcuts=new Dictionary<string, string>();
		/// <summary>
		/// Adds a pattern shortcut. This can make it so instead of constantly having to duplicate routes, you can instead make shortcuts
		/// This is NOT thread-safe. It should only be used at the initialization of all of your routes
		/// Example: 
		/// /foo/{!shortcut!} becomes /foo/bar/biz/{baz}
		/// when a shortcut named `shortcut` is created with value "/bar/biz/{bar}"
		/// </summary>
		public static void AddShortcut(string name, string text)
		{
			Shortcuts.Add(name, text);
		}
        //.Where("var","pattern")
        public SimplePattern Where(string variable, string regexPattern)
        {
            Groups.Single(x=>x.ParamName==variable).MatchType=new Regex(regexPattern,RegexOptions.Compiled);
            return this;
        }

        public SimplePattern Where(string variable, GroupMatchType matchtype)
        {
            switch(matchtype)
            {
            case GroupMatchType.AlphaNumeric:
                return Where(variable, "^[0-9a-zA-Z]+$");
            case GroupMatchType.Float:
                return Where(variable, @"^[-+]?[0-9]*\.?[0-9]+$");
            case GroupMatchType.Integer:
                return Where(variable, "^[-+]?[0-9]*$");
            case GroupMatchType.HexString:
                return Where(variable, "^[0-9A-Fa-f]+$");
            default:
                throw new NotSupportedException("That match type isn't supported");
            }
        }

        private string Pattern;
        private List<Group> Groups;
        
        private class Group {
            public string ParamName;
            public string Text;
            public bool IsParam=false;
            public List<string> ValidMatches=new List<string>();
            public bool MatchAll=true;
            public bool Optional=false;
            public char End;
            public Regex MatchType=null;
        }
        
        public SimplePattern (string pattern)
        {
            Pattern = pattern;
			foreach(var item in Shortcuts)
			{
				Pattern=Pattern.Replace("/{!"+item.Key+"!}", item.Value);
			}
            UpdateGroups();
        }
        


        /**This method returns true(and populates Params) if the input string matches the Pattern string. **/
        public MatchResult Match (string input)
        {
            var Params=new ParameterDictionary();
            string s=input;
            if(Groups.Count==0){
                throw new ApplicationException("Groups.Count==0 matches all. This shouldn't happen");
            }
            foreach(var g in Groups){
                if(!g.IsParam){
                    if(g.Text.Length>=s.Length){
                        if(Groups[Groups.Count-1]==g){
							return new MatchResult(g.Text==s, Params); //to check for exact matches(but only for the last group)
                        }else{
                            if(g.Optional){
                                return new MatchResult(true, Params);
                            }else{
                                return new MatchResult(false, Params);
                            }
                        }
                    }
                    string tmp=CutString(s,0,g.Text.Length);
                    if(g.Text==tmp){
                        s=s.Substring(g.Text.Length);
                    }else{
                        return new MatchResult(false, Params);
                    }
                }else{
                    int end;
                    if(g.End=='\0'){
                        end=s.Length;
                    }else{
                        end=s.IndexOf(g.End);
                        if(end==-1){
                            return new MatchResult(false, Params);
                        }
                    }
                    if(g.MatchAll){
                        if(s.Substring(0,end)==""){
                            return new MatchResult(false, Params);
                        }
                        int slash=s.IndexOf('/');
                        if(slash==-1 || g.Optional){
                            if(g.MatchType!=null)
                            {
                                if(!g.MatchType.IsMatch(s.Substring(0,end)))
                                {
                                    return new MatchResult(false, Params);
                                }
                            }
                            //Params.Add(g.ParamName, new List<string>());
                            //Params[g.ParamName]=s.Substring(0,end);
                            Params.Add(g.ParamName, s.Substring(0,end));
                            s=""; //doesn't matter. 
                        }else{
                            if(g.MatchType!=null)
                            {
                                if(!g.MatchType.IsMatch(s.Substring(0,slash)))
                                {
                                    return new MatchResult(false, Params);
                                }

                            }

                            Params.Add(g.ParamName,s.Substring(0,slash));
                            s=s.Substring(slash); //doesn't matter. 
                                
                        }
                    }else{
                        string t=s.Substring(0,end);
                        bool matched=false;
                        foreach(var match in g.ValidMatches){
                            if(match==t){
                                matched=true;
                                //break;
                            }
                        }
                        if(matched==false){
                            return new MatchResult(false, Params);
                        }
                        if(g.MatchType!=null)
                        {
                            if(!g.MatchType.IsMatch(t))
                            {
                                return new MatchResult(false, Params);
                            }
                        }
                        Params.Add(g.ParamName,t);
                        s=s.Substring(end);
                    }
                }
                
                
            }
            if(s.Length!=0){
                return new MatchResult(false, Params);
            }else{
                return new MatchResult(true, Params);
            }
        }
        /** This will parse the Pattern string one group at a time. **/
        int ParseParam (int start, ref Group g)
        {
            start++;
            
            int end=Pattern.Substring(start).IndexOf('}')+start;
            if(end+1>=Pattern.Length-1){
                g.End='\0';
            }else{
                g.End=Pattern[end+1];
            }
            string p=CutString(Pattern,start,end);
            g.Text=p;
            int tmp=p.IndexOf('[');
            if(tmp==-1){ //not found. Just trim it up and get the paramname
                p=p.Trim();
                if(p=="*"){
                    g.Optional=true; //meh. Still add it as a match-all group for the hell of it
                }
                g.MatchAll=true;
            }else{
                //return end;
                g.MatchAll=false;
                string l=CutString(p,tmp+1,p.IndexOf(']'));
                l=l.Replace(" ","");
                p=p.Substring(0,p.IndexOf("=")).Trim();
                int count=0;
                while(true){
                    if(l.Length==0){
                        break;
                    }
                    int endm=l.IndexOf(',');
                    if(endm==-1){
                        endm=l.Length;
                        g.ValidMatches.Add(l);
                        break;
                    }
                    g.ValidMatches.Add(l.Remove(endm));
                    l=l.Substring(endm+1);
                    count++;
                    if(count>100){
                        throw new ApplicationException("inifinite loop detected");
                    }
                }
            }
            
            g.ParamName=p;
            return end;
        }
        /**Little helper method to cut a string from start to end point. Just shorter than typing .Remove(end).Substring(start) **/
        private string CutString(string s,int start,int end){
            return s.Remove(end).Substring(start);
        }
        /** This will update all of the "groups" or parameter names/values for the pattern string. */
        private void UpdateGroups ()
        {
            List<Group> groups = new List<Group> ();
            Group g=new Group();
            for(int i=0;i<Pattern.Length;i++){
                if(Pattern[i]=='{'){
                    if(g!=null)
                        groups.Add(g);
                    g=new Group();
                    g.IsParam=true;
                    i=ParseParam(i,ref g);
                    groups.Add(g);
                    g=null;
                }else if(g==null){
                    g=new Group();
                    g.IsParam=false;
                    g.Text+=Pattern[i];
                }else{
                    g.Text+=Pattern[i];
                }
            }
            if(g!=null){
                groups.Add(g);
            }
            Groups=groups;
        }
    }
}




<#+
	class ViewGenerator{
		const bool UseAutoVars=false; //set to true to enable the autovars(recommended off. Also, no longer tested)
		class Variable{
			public string Access="public";
			public string Name;
			public string VarType;
		}
		public string Blah{get;set;}
		bool HasFlash=false;
		string Name;
		string Namespace;
		string Layout;
		string LayoutField;
		StringBuilder output=new StringBuilder();
		StringBuilder view=new StringBuilder();
		StringBuilder external=new StringBuilder();
		String input;
		bool RenderDirectly=false;
		string BaseClass=DefaultBaseClass;
		public static string DefaultBaseClass=null;
		public bool DetectNulls=true;
		List<Variable> Variables=new List<Variable>();
		string DefaultWriter="";
		public ViewGenerator(string file,string name,string namespace_,bool renderdirectly,bool detectnulls,string defaultwriter){
			var f=File.OpenText(file);
			input=f.ReadToEnd();
			Name=name;
			Namespace=namespace_;
			RenderDirectly=renderdirectly;
			DetectNulls=detectnulls;
			DefaultWriter=defaultwriter;
		}
		
		
		
		int DoVariables (int start)
		{
			int end=input.Substring(start+1).IndexOf("@}");
			if(end==-1){
				throw new ApplicationException("Could not find end of variable block");
			}
			string block=input.Substring(start+1,end);
			block=block.Trim();
			List<string> words=new List<string>(block.Split(new char[]{' ','\t','\n','\r'}));
			while(words.Contains("")){
				words.Remove("");
			}
			int i;
			for(i=0;i<words.Count-2;i+=3){
				Variable v=new Variable();
				v.Name=words[i];
				if(v.Name=="private" || v.Name=="protected" || v.Name=="public"){
					i++;
					v.Access=v.Name;
					v.Name=words[i];
				}
				if(words[i+1]!="as"){
					throw new ApplicationException("'as' expected. Found: "+words[i+1]);
				}
				v.VarType=words[i+2];
				
				if(v.VarType[v.VarType.Length-1]!=';' && (i<words.Count-3 || words[i+3]!=";")){
					throw new ApplicationException("';' expected. Found: "+words[i+3]);
				}
				if(i<words.Count-3 && words[i+3]==";"){
					i++;
				}else{
					v.VarType=v.VarType.Substring(0,v.VarType.Length-1);
				}
				if(v.Name=="Flash"){
					HasFlash=true;
					if(v.VarType.ToLower()!="string" && v.Access!="public"){
						throw new ApplicationException("Flash variable must be a public string");
					}
				}else{
					Variables.Add(v);
				}
			}		
			return end+=3; //+=2 for @} ending
		}
		int WriteVariable(int start){
			int end=input.Substring(start+1).IndexOf("=}");
			if(end==-1){
				throw new ApplicationException("Could not find end of output block");
			}
			string block=input.Substring(start+1,end);
			block=block.Trim();
			
			var v=Variables.Find(x=>x.Name==block);
			string code;
			if(v==null){
				code=block;
				if(UseAutoVars){
					v=new Variable(){Name=block, VarType="string"};
					Variables.Add(v);
				}else{
					//don't throw an error I guess?
				}
			}else{
				code=v.Name;
			}
			view.AppendLine(@"{
				object v;
				");
			if(DetectNulls){
				view.AppendLine(@"
				try{
					v="+code+@";
				}catch(NullReferenceException){
					v=null;
				}
				");
			}else{
				view.AppendLine(@"
					v="+code+@";
				");
			}
				view.AppendLine(@"
				if(v!=null){
					var e=v as System.Collections.IEnumerable;
					if (e!=null)
					{
						foreach(var item in e){ 
							var view=item as Earlz.BarelyMVC.ViewEngine.IBarelyView;
							if(view!=null){
								__Write(view);
							}else{
								__Write(item.ToString());
							}
						}
					}else{
						var view=v as Earlz.BarelyMVC.ViewEngine.IBarelyView;
						if(view!=null){
							__Write(view);
						}else{
							__Write(v.ToString());
						}
					}
					
				}}");
			
			return end+=3;
		}
		
		int WriteRawOutput(int start){
			int end=input.Substring(start+1).IndexOf("-}");
			if(end==-1){
				throw new ApplicationException("Could not find end of code output block");
			}
			string block=input.Substring(start+1,end);
			block=block.Trim();
			
			view.AppendLine(@"
				__Write("+block+");");
			
			return end+=3;
		}
		
		int WriteCode(int start){
			int end=input.Substring(start+1).IndexOf("#}");
			if(end==-1){
				throw new ApplicationException("Could not find end of code block");
			}
			string block=input.Substring(start+1,end);
			block=block.Trim();
			view.AppendLine(block);
			return end+=3;
		}
		int WriteExternalCode(int start){
			int end=input.Substring(start+1).IndexOf("+}");
			if(end==-1){
				throw new ApplicationException("Could not find end of external code block");
			}
			string block=input.Substring(start+1,end);
			block=block.Trim();
			external.AppendLine(block);
			return end+=3;
		}
		int WriteHelper(int start){
			int end=input.Substring(start+1).IndexOf("?}");
			if(end==-1){
				throw new ApplicationException("Could not find end of helper block");
			}
			string block=input.Substring(start+1,end);
			block.Trim();
			int stop=block.IndexOfAny(new char[]{' ','='});
			string classname;
			classname=block.Substring(0,stop);
			
			block=block.Substring(stop);
			block=block.Trim();
			view.AppendLine(@"
			{
				var __v=new "+classname+"{"+block+@"};
				__v.Layout=null; //HACK
				__Write(__v);
			}
			");
			return end+=3;
		}
		int ParseKeyword(int start){
			int end=input.Substring(start+1).IndexOf("!}");
			if(end==-1){
				throw new ApplicationException("Could not find end of keyword block");
			}
			string block=input.Substring(start+1,end);
			
			int stop=block.IndexOfAny(new char[]{' ','='});
			string keyword;
			if(stop<0){
				keyword=block.Substring(0,end);
			}else{
				keyword=block.Substring(0,stop);
			}
			switch(keyword){
				case "base":
					BaseClass=block.Substring(stop+1);
				break;
				case "name":
					Name=block.Substring(stop+1);
				break;
				case "namespace":
					Namespace=block.Substring(stop+1);
				break;
				case "layout":
					Layout=block.Substring(stop+1);
				break;
				case "layout_field":
					LayoutField=block.Substring(stop+1);
				break;
				case "if":
					view.Append("if(");
					view.Append(block.Substring(stop+1));
					view.AppendLine("){");
				break;
				case "endif":
					view.AppendLine("}");
				break;
				case "else":
					view.AppendLine("}else{");
				break;
				case "elseif":
					view.Append("}else if(");
					view.Append(block.Substring(stop+1));
					view.AppendLine("){"); 
				break;
				case "foreach":
					DoForeach(block,stop);
				break;
				case "endforeach":
					view.AppendLine("}");
					break;
				case "use_once":
					DoUseOnce(block,stop);
				break;
				case "render_directly":
					RenderDirectly=true;
				break;
				case "render_tostring":
					RenderDirectly=false;
				break;
				case "detect_nulls":
					DetectNulls=true;
				break;
				case "do_not_detect_nulls":
					DetectNulls=false;
				break;
				default:
					throw new ApplicationException("Unknown keyword used");
			}
			
			return end+=3;
		}
		public void DoForeach(string block,int stop){
			view.Append("foreach(");
			var pieces=block.Substring(stop+1).Split(new char[]{' '});
			if(pieces.Count()<3){
				throw new ApplicationException("Expected format of {!foreach variable in enumerator!}");
			}
			view.Append("var ");
			view.Append(block.Substring(stop+1));
			view.AppendLine("){");
		}
		public void DoUseOnce(string block,int stop){
			//How to implement this?
			//Intention is to have this create a public variable, and write it out only once. 
			//Is it even needed?
			Variable v=new Variable();
			var pieces=block.Substring(stop+1).Split(new char[]{' '});
			if(pieces.Count()<3){
				throw new ApplicationException("Expected format of {!use_once variablename as variabletype!}");
			}
			v.Name=pieces[0];
			v.VarType=pieces[2];
			Variables.Add(v);
		}
		
		public string Generate(){
			char last='\0';
			view.Append("__Write(@\"");
			for(int i=0;i<input.Length;i++){
				if(last=='{' && input[i]=='@'){
					view.AppendLine("\");");
					i+=DoVariables(i);
					view.Append("__Write(@\"");
				}else if (last=='{' && input[i]=='='){
					view.AppendLine("\");");
					i+=WriteVariable(i);
					view.Append("__Write(@\"");
				}else if (last=='{' && input[i]=='#'){
					view.AppendLine("\");");
					i+=WriteCode(i);
					view.Append("__Write(@\"");
				}else if(last=='{' && input[i]=='+'){
					i+=WriteExternalCode(i);
				}else if (last=='{' && input[i]=='!'){
					view.AppendLine("\");");
					i+=ParseKeyword(i);
					view.Append("__Write(@\"");
				}else if(last=='{' && input[i]=='-'){
					view.AppendLine("\");");
					i+=WriteRawOutput(i);
					view.Append("__Write(@\"");
				}else if(last=='{' && input[i]=='?'){
					view.AppendLine("\");");
					i+=WriteHelper(i);
					view.Append("__Write(@\"");
				}else if(last=='\\' && input[i]=='{'){
					view.Append(Escape(input[i]));
					last='\0';
					continue;
				}else{
					view.Append(Escape(last));
					if(i==input.Length-1){
						view.Append(Escape(input[i]));
					}
				}
				if(i<input.Length){
					last=input[i];
				}
			}
			view.AppendLine("\");");
			
			output.Append(
"namespace "+Namespace+@"{
class "+Name+" : "+BaseClass+@"{
//Internal variables
	StringBuilder __Output=new StringBuilder();
public "+(Layout??"IBarelyView")+@" Layout{get;set;}
//vIEW VARIABLES//
");
			foreach(var v in Variables){
				output.AppendLine("\t"+v.Access+" "+v.VarType+" "+v.Name+"{get;set;}");
			}      
			output.Append(@"
//END VARIABLES//
//EXTERNAL CODE
");
			output.Append(external);
			output.Append(@"
//END EXTERNAL CODE
	void BuildOutput(){
//VIEW OUTPUT CODE//
");
			output.Append(view);
			output.Append(@"
//END OUTPUT//
	}
//Internal code
	
");
	if(!HasFlash){
		output.Append(@"
	public override string Flash{
		get{
			return Layout.Flash;
		}
		set{
			Layout.Flash=value;
		}
	}");
	}
	output.Append(@"
	
	bool __InLayout=false; //internally used to prevent recursive loops in RenderView()
	public override string RenderView(){
		__Output=new StringBuilder();
		if(Layout==null){
			BuildOutput();
			return __Output.ToString();
		}
		if(__InLayout){
			//If we get here, then the layout is currently trying to render itself(and we are being rendered as a partial/sub view)
			__InLayout=false;
			BuildOutput();
			return __Output.ToString();
		}else{
			//otherwise, we are here and someone called RenderView on us(and we have a layout to render first)
			__InLayout=true;
			return Layout.RenderView(); 
		}
		//This should recurse no more than 2 times
		//Basically, this will go to hell if there is ever more than 1 partial view with a Layout set. 
	}
	TextWriter __Writer;
	bool __RenderDirectly="+RenderDirectly.ToString().ToLower()+@";
	public override bool RenderedDirectly{
		get{
			return __RenderDirectly;
		}
	}
	private void __Init(TextWriter writer){
		__Writer=writer;
");
	if(Layout!=null){
		output.Append("Layout=new "+Layout+"();\n");
		output.Append("Layout."+LayoutField+"=this;\n");
	}
	
	output.Append(@"
	}
	public "+Name+@"(){
		if(__RenderDirectly){
			__Init("+DefaultWriter+@");
		}else{
			__Init(null);
		}
	}
	public "+Name+@"(bool UseHttpResponse){
		if(UseHttpResponse){
			__RenderDirectly=true;
			__Init("+DefaultWriter+@");
		}else{
			__Init(null);
		}
	}
	public "+Name+@"(TextWriter writer){
		__Init(writer);
	}
	protected virtual void __Write(string s){
		if(__Writer!=null){
			__Writer.Write(s);
		}
		__Output.Append(s);
	}
	protected virtual void __Write(IBarelyView v){
		string s=v.RenderView();
		if(!v.RenderedDirectly){
			//__Write(s);
		}
		__Output.Append(s);
	}
}
} //for namespace
");
			return output.ToString();
		
		}
		string Escape(char c){
			if(c=='\"'){
				return "\"\"";
			}
			if(c=='\0'){
				return "";
			}
			return c.ToString();
		}
	}
	
#>
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the expression reference class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/25/2015   rcs     Initial Implementation
//  =====================================================================================================
//
//  BSD 3-Clause License
//  Copyright (c) 2020, Robert C. Steiner
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//  1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following
//  disclaimer.
//
//  2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the
//  following disclaimer in the documentation and/or other materials provided with the distribution.
//
//  3. Neither the name of the copyright holder nor the names of its
//  contributors may be used to endorse or promote products derived from
//  this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
//  INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
//  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
//  USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.Reserved.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Text;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Expression reference.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PExpressionReference : PExpression
    {
        #region Properties

        /// <summary>
        ///  The Clause if any (after array[]. clause
        /// </summary>
        public PExpressionReference Clause;

        /// <summary>
        /// The variable (the member if it is an object)
        /// </summary>
        public PVariable Variable;


        /// <summary>
        /// The object reference if this is an object like InstanceRef.member
        /// </summary>
        public PVariable InstanceRef;


        /// <summary>
        /// The module if it is a call.
        /// </summary>
        public PModule Module;

        /// <summary>
        /// The name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The object name.
        /// </summary>
        public string ObjectName;

        /// <summary>
        /// The member name
        /// </summary>
        public string MemberName {get { return ObjectName != null ? Name : null; }}

        /// <summary>
        /// Gets a value indicating whether this instance is object reference.
        /// </summary>
        public bool IsObjectRef {get { return ObjectName != null; }}

        /// <summary>
        /// The class reference if any
        /// </summary>
        public PClass ClassRef {get { return Variable != null ? Variable.ClassRef : null; }}

        /// <summary>
        /// Gets the full name of the member.
        /// </summary>
        public string FullMemberName {get { return ClassRef != null ? string.Format("{0}.{1}", ClassRef.Name, Name) : Name; }}

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        public FunctionInfo Function;

        /// <summary>
        /// The is call.
        /// </summary>
        public bool IsCall;

        /// <summary>
        /// The is array.
        /// </summary>
        public bool IsArray;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is used as a reference in a call.
        /// </summary>
        public bool IsRef
        {
            get { return _isRef || (Variable != null && Variable.IsRef); }
            set { _isRef = value; }
        }

        /// <summary>
        /// Gets the index of the run.
        /// </summary>
        public int RunIndex
        {
            get { return (int) Variable.Value.LValue; }
        }

        /// <summary>
        /// true if is reference.
        /// </summary>
        private bool _isRef;

        /// <summary>
        /// The is extension function
        /// </summary>
        private bool IsExtension;


        /// <summary>
        /// used to store the current class if any.
        /// </summary>
        public PClass MyClass;            

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        public PExpressionList Args { get; set; }

        #endregion

        #region Value

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public Accumulator Value {get {return GetValue();} set{SetValue(value);}}

        public PBlock Scope { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is constructor.
        /// </summary>
        public bool IsConstructor { get; set; }

        #endregion

        #region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        /// <param name="name">     . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpressionReference(PContext context, string name) : base(context)
        {
            Name       = name;
            LineNumber = Source.LineNumber;
            Offset     = Source.Start;
            Length     = name.Length;
            Scope      = Context.ScopeStack.Count > 0 ? Context.ScopeStack.Peek() : null;
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets a value.
        /// </summary>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetValue(Accumulator value)
        {
            var old = Context.RunStack.ObjectRef;

            if (InstanceRef != null)
            {
                Context.RunStack.ObjectRef = InstanceRef.Value.OValue;
            }
           
            // member fields
            if (Variable.IsMember)
            {
                var obj =  Context.RunStack.ObjectRef;
                if (obj != null)
                {
                    obj.Fields[Variable.ValueOffset].Value = value;
                }
                else
                {
                    // it must be a local variable set
                    
                    RuntimeFatal( Offset, Length, LineNumber, "Can't seem to access variable {0}", this);
                }
            }

            // member functions
            else if (IsObjectRef )
            {
                var obj = Variable.Value.OValue;
                if (obj != null)
                {
                    obj.Fields[Variable.ValueOffset].Value = value;
                }
                else
                {
                    RuntimeFatal( Offset, Length, LineNumber, "Can't seem to access method {0}", this);
                }
            }

            // variable arrays
            else if (IsArray)
            {
                var index = Variable.GetArrayIndex( Args );
                Variable.Value.Array[index] = value.ConvertTo( Variable.Value.Store );
            }
            else
            {
                // variable ref array elements
                var array = Variable.Value;
                if (array.IsArray)
                {

                    var index = (Context.RunStack[Variable.StackIndex].IValue / PVariable.ARRAY_MARKER) - 1;
                    array.Array[index] = value.ConvertTo( array.Store );
                }
                else
                {
                    //varaibles
                    Variable.Value = value;
                }
            }
            Context.RunStack.ObjectRef = old;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>
        /// The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Accumulator GetValue()
        {
            Accumulator v;
            var old = Context.RunStack.ObjectRef;

            if (InstanceRef != null)
            {
                Context.RunStack.ObjectRef = InstanceRef.Value.OValue;
            }
            // member fields
            if (Variable.IsMember)
            {
                var obj =  Context.RunStack.ObjectRef;
                if (obj != null)
                {
                    v = obj.Fields[Variable.ValueOffset].Value;
                }
                else
                {
                    v = new Accumulator();
                }
            }

            // member functions
            else if (IsObjectRef)
            {
                var obj = Variable.Value.OValue;
                if (obj != null)
                {
                    v = obj.Fields[Variable.ValueOffset].Value;
                }
                else
                {
                    v = new Accumulator();
                }
            }
            
            // variable arrays
            else if (IsArray)
            {
                var index = Variable.GetArrayIndex( Args );
                v = Variable.Value.Array[index];
            }
            else 
            {

                // variable ref array elements
                var array = Variable.Value;
                if (array.IsArray)
                {

                    var index = (Context.RunStack[Variable.StackIndex].IValue/PVariable.ARRAY_MARKER) - 1;
                    v = array.Array[index];
                }

                else
                {
                    v = Variable.Value;
                }
                
            }
            Context.RunStack.ObjectRef = old;
            return v;
        }


        #region Parse

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parses this element.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            MyClass = Context.CurrentClass;
            int start = Source.Start;
            if (Name != null)
            {
                if (Source.Match('.'))
                {
                    ParseObjectRef();
                    // continue with myClass.Method() and myClass.value[]
                }

                if (Source.MatchFlush('('))
                {
                    return ParseCallArguments();
                }

                while (Source.Match('['))
                {
                    IsArray = true;

                    ParseArrayDimension();
                    if (Args != null && Args.Count == 0)
                    {
                        SyntaxError(start,Source.Position-start,"Array's are referenced with subscripts, {0}[] should just be {0}",Name);
                    }
                    if (Source.CurrentChar == '.' )
                    {
                        Clause = new PExpressionReference(Context,Name);
                        Clause.Parse();
                        
                        // person[index].setValue(v)
                      // SyntaxError(start,Source.Position-start,"Arrays of objects cannot access members, directly, assign to a variable first.");
                      //  Source.MoveNextLine();
                      //  return false;
                    }

                }
                BindToVariable(start,false);
                return true;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse object reference.
        /// 
        /// like  myClass.x
        /// or    myClass.SetValue(y)
        /// or    y = new myClass()
        /// 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ParseObjectRef()
        {
            ObjectName = Name;
            Name       = Parser.ParseName();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse call arguments and resolves the function (possible an extension function).
        /// Begins After the first (
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected bool ParseCallArguments()
        {
            Args  = new PExpressionList(Context);
            IsCall = true;
            ParseArgs(')');

            IsExtension = FindExtensionMethod();
            if (IsExtension) return true;

            Context.UnresolvedFunctions.Add(this);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first extension method.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool FindExtensionMethod()
        {
            FunctionInfo function;
            if (Std.Methods.TryGetValue( Name, out function ))
            {
                // its a extension function so handle it.
                Function = function;
                return true;
            }
            return false;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse array dimension.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ParseArrayDimension()
        {
            // array declaration
            if (Args == null)
            {
                Args = new PExpressionList(Context);
            }
            PExpression dim;
            var pos = Source.Position;
            Source.ParseToken();
            if (Source.Token != "]")
            {
                if ((dim = Parser.ParseExpression()) == null)
                {
                    SyntaxError(pos, 1, "Expected a expression for the array dimension like {0}[EXPRESSION].",Name);
                    dim = new PLiteralInteger(Context, "0");
                }
                pos = Source.Position;
                Args.Add(dim);
                if (!Source.Match(']'))
                {
                    SyntaxError(pos, 1, "Expected a closing ']' after the dimension of the array. {0}[{1}]",Name, dim.ToString());
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse arguments.
        /// </summary>
        /// <param name="end">  The end character. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ParseArgs(char end)
        {
            if (ObjectName!=null)
            {
                // put the reference to the object as the first arg.
                var arg = new PExpressionReference( Context,ObjectName);
                Context.UnresolvedReferences.Add( arg );

                Args.Add( arg );
            }
            while (!Source.MatchFlush( end ))
            {
                Source.ParseToken();
                PExpression arg = Parser.ParseExpression();
                if (arg != null)
                {
                    Args.Add( arg );
                }
                var pos = Source.Position;

                if (!Source.Match( ',' ))
                {
                    if (!Source.Match( end ))
                    {
                        SyntaxError( pos, 1, "Expected a closing ')' on function argument list." );
                    }
                    return;
                }
            }
           // Source.MoveNext();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the variable if not found the creates one.
        /// </summary>
        /// <param name="offset">       offset of reference. </param>
        /// <param name="reportError">  (optional) true to report error. </param>
        /// <param name="resolving">    (optional) true to resolving stage (after parsing) </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void BindToVariable(int offset, bool reportError = true, bool resolving=false)
        {
            if (InstanceRef == null && ObjectName != null)
            {
                InstanceRef = Bind( offset, reportError, ObjectName, resolving );
            }

            if (!IsCall && Variable == null)
            {
                Variable  = Bind( offset, reportError, Name ,resolving);
                if (Variable == null)
                {
                    SyntaxError( offset, Name.Length, "Can't resolve variable: {0}.  \nAdd a Declare TYPE {0}  where TYPE is a Data type like Real or Integer or String. like this:\n Declare Integer {0}", Name );
                    return;
                }
                IsArray = Variable.IsArray || IsArray;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Binds.
        /// </summary>
        /// <param name="offset">       offset of reference. </param>
        /// <param name="reportError">  true to report error. </param>
        /// <param name="name">         . </param>
        /// <param name="resolving">    (optional) true to resolving stage (after parsing) </param>
        /// <returns>
        /// the varaible or null if can't find it.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private PVariable Bind(int offset, bool reportError, string name, bool resolving=false)
        {
            if (name == null)
            {
                return null;
            }

            PVariable variable=null;

            if (InstanceRef != null)
            {
                // variable is on the InstanceRef's field list so get its type
                var type  = InstanceRef.ClassRef;
                if (type != null)
                {
                    var field = type.Fields.Find(f => f.Variable.Name == name);
                    if (field == null)
                    {
                        if (!resolving)
                        {
                            Context.UnresolvedReferences.Add(this);
                            return null;
                        }
                        SyntaxError(Offset,Length,"Reference to a class {0} member {0} cannot be found",type.Name,name);
                        return null;
                    }
                    variable  = field.Variable;
                    return variable;
                }
                if (!resolving)
                {
                    Context.UnresolvedReferences.Add(this);
                }
                return null;
            }

            variable = Context.FindVariable(name, Source.LineNumber);

            if (variable == null)
            {
                if (!resolving)
                {
                    // NOTE: Todo this may be a problem later with objects 
                    // TODO: may have to wait until the resolving phase to report this.
                    Context.UnresolvedReferences.Add( this );
                }
                else
                {
                    variable = BindCreateVariable( offset, reportError, name );
                }
            }
            return variable;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Bind create variable.
        /// </summary>
        /// <param name="offset">       offset of reference. </param>
        /// <param name="reportError">  true to report error. </param>
        /// <param name="name">         . </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private PVariable BindCreateVariable(int offset, bool reportError, string name)
        {
            if (reportError)
            {
                if (Name == null)
                {
                    SemanticError(-1, -1, -1, "Can't process variable.");
                }
                else
                {
                    SemanticError(offset, Name.Length, -1, "Variable: '{0}' is not defined yet or defined in another scope! ",Name);
                }
            }
            var variable               = new PVariable(Context, false);
            variable.Name              = name;
            variable.Type              = "Integer";
            variable.InitialExpression = new PLiteralInteger(Context, "0");
            var decl                   = new PDeclare(Context, false);
            decl.Variable              = variable;
            var block                  = Context.ScopeStack.Peek();

            block.Statements.Insert(0, decl);
            block.Scope.Add(variable);
            variable.References.Add(Source.LineNumber);
            return variable;
        }

        #endregion

        #region Execute

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// </summary>
        /// <returns>
        /// Next if continue else condition.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            base.Execute(); // sets line number

            if (IsCall)
            {
                return ExecuteCall();
            }

            if (IsArray)
            {
                return ExecutePushArray();
            }


            if (IsRef && Context.RunStack.InCall && (Context.RunStack.CurrentArg!=null && Context.RunStack.CurrentArg.IsRef))
            {
                return ExecutePushRef();
            }

            if (Name == "this")
            {
                return ExecuteThis();
            }
            
            if (Name == "Super")
            {
                ExecuteSuper();
            }


            return ExecutePushValue();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the super operation.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState ExecuteSuper()
        {
            Context.RunStack.Push( Context.RunStack.ObjectRef );
            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <returns>
        /// the next state
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState ExecuteThis()
        {
            Context.RunStack.Push( Context.RunStack.ObjectRef );
            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the push variable on the stack.
        /// </summary>
        /// <returns>
        /// the next state.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState ExecutePushValue()
        {
            var old = Context.RunStack.ObjectRef;

            if (InstanceRef != null)
            {
                Context.RunStack.ObjectRef = InstanceRef.Value.OValue;
            }

            var value = Value;
            if (value.Store == StoreEnum.NULL)
            {
                Context.RunStack.ObjectRef = old;
                return RuntimeError( Offset, Length, 0, "Variable {0} was not initialized before it was used", Name);
            }

            Context.RunStack.Push(value);

            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the call argument operation.
        /// then this is pointing back to the original variable passed in, just put the
        /// stack index on.
        /// </summary>
        /// <returns>
        /// the next state.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState ExecutePushRef()
        {
            Context.RunStack.Push(Variable.IsRef? Context.RunStack.GetValue( Variable.StackIndex,true).IValue :Variable.StackIndex);
            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes putting an array on the stack.
        /// </summary>
        /// <returns>
        /// the next state.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState ExecutePushArray()
        {
            var index = Variable.GetArrayIndex(Args);
            if (index == -1)
            {
                if (Variable.IsMember)
                {
                    Context.RunStack.Push(Context.RunStack.ObjectRef.Fields[Variable.ValueOffset].Value.Array);
                }
                // push the array itself
                else
                {
                    Context.RunStack.Push( Variable.Value );
                }
                return ReturnState.NEXT;
            }

            if (Variable.Type=="String" && Variable.DimensionRank==0)
            {
                // string element
                string s = Variable.Value.SValue;
                if (s==null || index >= s.Length)
                {
                    return RuntimeFatal( Offset, Length, LineNumber,"String index is out of range {0}[{1}] ",Variable.Name, index);
                }
                Context.RunStack.Push( new Accumulator( s[index] ) );
            }
            else
            {
                IPCArray array;
                if (Variable.IsMember)
                {
                    array = Context.RunStack.ObjectRef.Fields[Variable.ValueOffset].Value.Array;
                }
                else if (Variable.IsArgument)
                {
                    // if may be a array passed from an object so it's just the array not the variable
                   var x = Context.RunStack[Variable.RunIndex];

                    array = x.Array;
                }
                else
                {
                    array = Variable.Value.Array;
                }

                if (array != null && index < array.Length)
                {
                    if (IsRef && Context.RunStack.InCall)
                    {
                        // ref array's are passed as index *0x10000 + base index of array
                        Context.RunStack.Push( Variable.StackIndex + (index + 1) * PVariable.ARRAY_MARKER );
                    }
                    else
                    {
                        Context.RunStack.Push( array[index] );
                    }
                }
                else
                {
                    return RuntimeFatal( Offset, Length,LineNumber, "Array {0} index out of range", ToString());
                }
            }

            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the call operation.
        /// </summary>
        /// <returns>
        /// the next state.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState ExecuteCall()
        {
            var old = Context.RunStack.ObjectRef;
            var module = Module;

            if (IsObjectRef )//|| MyClass!=null)
            {
                // object is the first parameter and is set on the execution.
                var exprRef = Args[0] as PExpressionReference;
                PObject obj=null;
                if (exprRef.Name == "this")
                {
                    obj = old;
                }
                else if (exprRef.Variable!=null)
                {
                    obj = exprRef.Variable.Value.OValue;
                }
                if (obj!=null)
                {
                    Context.RunStack.ObjectRef = obj;
                    module = obj.ClassRef.RefFunction(Name);
                    if (module == null)
                    {
                        module = Module;
                    }
                }
            }

            Context.RunStack.CalledModule = module;
            Context.RunStack.InCall = true;

            if (module!=null && Args.Count != module.Parameters.Count)
            {
                 module.RuntimeFatal( Offset, module.Name.Length,LineNumber, "Wrong number of arguments to: {0}  Found {1}, Expected {2}", module.Name,
                            Args.Count, module.Parameters.Count);

                 return ReturnState.STOP;
            }

            for (int index = Args.Count - 1; index >= 0; index--)
            {
                var argument                = Args.Expressions[index];
                if (module != null)
                {
                    Context.RunStack.CurrentArg = module.Parameters[index];
                }

                if (argument.Execute() == ReturnState.STOP)
                {
                    Context.RunStack.ObjectRef = old;
                    return ReturnState.STOP;
                }
            }
            Context.RunStack.InCall       = false;
            Context.RunStack.CalledModule = null;
            Context.RunStack.CurrentArg   = null;

            if (IsExtension)
            {
                Context.RunStack.ObjectRef = old;
                return ExecuteExtension();
            }
            // execute the call to the module
            if (module == null)
            {
                RuntimeFatal( Offset, Length,LineNumber, "Can't find module {0}, is it spelled right?", Name);
                Context.RunStack.ObjectRef = old;
                return ReturnState.STOP;
            }
            var ret = module.Execute();
            if (ret == ReturnState.STOP)
            {
                Context.RunStack.ObjectRef = old;
                return ret;
            }
            if (module.HasReturnValue)
            {
                Context.RunStack.CallReturn(Args.Count);
            }
            else
            {
                Context.RunStack.CallLeave(Args.Count);
            }

            Context.RunStack.ObjectRef = old;
            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the extension operation.
        /// </summary>
        /// <returns>
        /// the next state.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState ExecuteExtension()
        {
            // get the arguments into an array
            return Function.ExecuteFun(Args,this);
        }

        #endregion

        #region To String

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            if (ObjectName != null)
            {
                builder.AppendFormat("{0}.{1}", ObjectName, Name);
            }
            else
            {
                builder.Append(Name);
            }
            if (IsArray)
            {
                if (Args != null)
                {
                    foreach (PExpression dimension in Args.Expressions)
                    {
                        builder.Append("[");
                        dimension.ToString(builder);
                        builder.Append(']');
                    }
                }
                else
                {
                    builder.Append( "[]" );
                }

                if (Clause != null)
                {
                    Clause.ToString(builder);
                }
                return;
            }
            if (IsCall)
            {
                builder.Append('(');
                Args.ToString(builder);
                builder.Append(')');
            }
        }

        #endregion
    }
}
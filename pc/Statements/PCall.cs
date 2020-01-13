////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the call class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/27/2015   rcs     Initial Implementation
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
using ZCore;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Call Statement in Pseudo
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PCall : PStatement
    {
        public PModule         Module;
        public string          Name;
        public string          ObjectName;
        public PExpressionList Arguments;
        public PVariable       ObjectRef;
        public PClass MyClass;              // used to store the current class if any.

        /// <summary>
        /// Gets the name of the object type.
        /// </summary>
        public string ObjectTypeName { get { return ObjectRef != null ? ObjectRef.Type:""; } }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        public string MethodName { get {return ObjectName != null ? string.Format("{0}.{1}", ObjectTypeName, Name) : Name;} }

        /// <summary>
        /// Gets the object class.
        /// </summary>
        public PClass ObjClass { get { return ObjectRef!=null? ObjectRef.ClassRef:null; } }

        /// <summary>
        /// Gets or sets the function if it is builtin.
        /// </summary>
        public FunctionInfo Function { get; set; }


        #region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PCall(PContext context) : base(context)
        {
            Arguments = new PExpressionList(context);
        }

        #endregion

        #region Parse

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parses this call, expects call is already processed.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            LineNumber = Source.LineNumber;
            Source.FlushWhitespace();

            if (!ParseMethodName())
            {
                return false;
            }

            MyClass = Context.CurrentClass;

            ParseSetModule();

            if (Source.MatchFlush('('))
            {
                return ParseArguments();
            }

            return ParseFixMissingParens();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse method name and checks for objects
        /// 
        /// Call  Method()
        /// Call  myClass.Method()
        /// 
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseMethodName()
        {
            var pos = Source.Position;
            if (Source.ParseName() != TokenTypeEnum.NAME)
            {
                SyntaxError(pos, 2, "Expected the name of the Module/Function to call after the 'Call' keyword.");
                Source.MoveNextLine();
                return false;
            }
            ObjectName = Source.Token;
            Offset     = Source.Start;
            Length     = ObjectName.Length;


            // look to see if it is Name.Function()

            if (Source.MatchFlush('.'))
            {
                // this is a member function call
                Name = Parser.ParseName();
            }
            else
            {
                Name = ObjectName;
                ObjectName = null;
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse arguments.
        /// If it is an object call, then add the this arg.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseArguments()
        {
            Source.FlushWhitespace();

            // handle special call of member call
            if (ObjectName != null || (Module!=null && Module.IsMember))
            {
                Arguments.Expressions.Add(new PExpressionReference(Context, "this"));
            }

            // now handle reqular arguments
            if (!Source.IsChar(')'))
            {
                if (!Arguments.Parse())
                {
                    return false;
                }
            }

            if (!Source.Match(')'))
            {
                SyntaxError(-1, -1, "Expected ')' to end the list of arguments passed to the module '{0}.", Name);
                return false;
            }
            if (Module!=null)
            {
                Module.FixupRefArgs( this );
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse/fix missing parens. 
        /// Report error is not there and move forward
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseFixMissingParens()
        {
            Source.Start = Source.Position;
            if (Source.CurrentChar == '\n')
            {
                SyntaxError(-1, -1, "Expected '()' to follow the name of the module to call. For Example  Call {0}()",
                    Name);
                Source.MoveNextLine();
                return true;
            }
            SyntaxError(-1, -1,
                "Expected '(' to follow the name of the module to call. For Example  Call {0}( args... )", Name);
            Source.MoveNextLine();
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse, set module if it exists else add it to the unresolved calls.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ParseSetModule()
        {
            if (  ObjectName == "Super")
            {
                foreach (var block in Context.ScopeStack)
                {
                    PClass cls = block as PClass;
                    if (cls != null)
                    {
                        Module = cls.Modules.Find( x => x.Name == Name );
                        if (Module == null)
                        {
                            Context.UnresolvedCalls.Add( this );
                        }
                        return;
                    }
                }
            }
            if (ObjectName != null)
            {
                // find the variable and get its type (it must be defined already)
                var variable = Context.FindVariable( ObjectName, LineNumber );
                if (variable == null)
                {
                    SemanticError( Offset, ObjectName.Length, LineNumber, "Undefined variable: {0}", ObjectName );
                    return;
                }
                ObjectRef = variable;
            }

            Module = Context.FindModule(MethodName, Source.LineNumber);


            if (Module == null)
            {
                if (FindExtensionMethod())
                {
                    return;
                }
                Context.UnresolvedCalls.Add(this);
            }
        }

        #endregion


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
            if (Name != null && Std.Methods.TryGetValue( Name, out function ))
            {
                // its a extension function so handle it.
                Function = function;
                return true;
            }
            return false;
        }



        
        #region Execute

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails., pushes arguments, last to first on the stack (first is on top)
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            // sets line number
            base.Execute();

            Context.RunStack.CalledModule = Module;
            Context.RunStack.InCall       = true;

            if (Function.Function != null)
            {
                for (int index = Arguments.Count - 1; index >= 0; index--)
                {
                    var argument                = Arguments.Expressions[index];
                    if (Module != null)
                    {
                        Context.RunStack.CurrentArg = Module.Parameters[index];
                    }
                    if (argument.Execute()      == ReturnState.STOP)
                    {
                        return ReturnState.STOP;
                    }
                }

                Context.RunStack.CurrentArg   = null;
                Context.RunStack.InCall       = false;
                Context.RunStack.CalledModule = null;
                return Function.ExecuteFun( Arguments, this );
            }

            // Module
            var old = Context.RunStack.ObjectRef;


            if (ObjectName == "Super")
            {
                var method = Context.RunStack.CallStack.Top();
                var cls    = method.ClassRef;
                var super  = cls.SuperClass;
                Module     = super.Modules.Find( x => x.Name == Name );
            }
            else if (ObjectRef !=null)
            {
                PClass cls;
                if (ObjectRef.Value.OValue != null)
                {
                    Context.RunStack.ObjectRef = ObjectRef.Value.OValue;
                    cls = ObjectRef.Value.OValue.ClassRef;
                }
                else
                {
                    cls = ObjectRef.ClassRef;
                }

                var mod = cls.RefModule(Name);
                if (mod != null)
                {
                    Module = mod;
                }
                else
                {
                    mod = cls.RefFunction(Name);
                    if (mod != null)
                    {
                        Module = mod;
                    }
                }
            }


            else if (Context.RunStack.ObjectRef!=null)
            {
                // check if call is polymorphic in a call to an object so use its overrides
                var cls =Context.RunStack.ObjectRef.ClassRef;
                var mod = cls.RefModule(Name);
                if (mod != null)
                {
                    Module = mod;
                }
                else
                {
                    mod = cls.RefFunction(Name);
                    if (mod != null)
                    {
                        Module = mod;
                    }
                }
            }

            for (int index = Arguments.Count - 1; index >= 0; index--)
            {
                var argument                           = Arguments.Expressions[index];
                Context.RunStack.CurrentArg            = Module.Parameters[index];
                if (ExecuteArgument( argument, index ) == ReturnState.STOP)
                {
                    Context.RunStack.ObjectRef = old;
                    return ReturnState.STOP;
                }
            }


            Context.RunStack.InCall       = false;
            Context.RunStack.CalledModule = null;
            Context.RunStack.CurrentArg   = null;

            // execute the call to the module
            var ret = Module.Execute();

            // restore the stack
            Context.RunStack.CallLeave(Arguments.Count);
        
            Context.RunStack.CallStack.Top().SetStackIndexForArgs();
            Context.RunStack.ObjectRef = old;
            return ret;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the argument operation.
        /// </summary>
        /// <param name="argument"> The argument. </param>
        /// <param name="index">    Zero-based index of the argument. </param>
        /// <returns>
        /// the return state to continue
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState ExecuteArgument(PExpression argument, int index)
        {
            var refarg = argument as PExpressionReference;

            if (refarg != null && refarg.IsObjectRef && refarg.Name == "this")
            {
                // push the object onto the stack.
                var obj = refarg.Variable;
                if (obj != null)
                {
                    obj.Execute();
                   // return ReturnState.NEXT;
                }
                return RuntimeFatal( Offset, Length, LineNumber, "This call requires an object");
            }

            var returnState = argument.Execute();
            if (returnState != ReturnState.NEXT)
            {
                return returnState;
            }
            StoreEnum argType;
           
            // check argument type for compatibility and convert to the target type
            var parameter = Module.Parameters[index];
            var paramType = parameter.Type;
            var top       = Context.RunStack.Pop();
          
            if (refarg != null && refarg.IsRef && !refarg.IsArray && parameter.IsRef)
            {
                    argType = Context.RunStack[top.IValue].Store;
            }
            else
            {
                argType = top.Store;
            }

            if (parameter.IsArray && top.IsArray)
            {
                Context.RunStack.Push( top );
                return ReturnState.NEXT;
            }
            // if it's null then it probably uniitialized so it should be ok since we already fixed the types.
            if (argType != StoreEnum.NULL)
            {
                switch (paramType)
                {
                    case "Real":
                        if (argType == StoreEnum.REAL || argType == StoreEnum.INTEGER) break;
                        return TypeConversionError( parameter, argument, argType );

                    case "Integer":
                        if (argType == StoreEnum.INTEGER || argType == StoreEnum.CHAR) break;
                        return TypeConversionError( parameter, argument, argType );
                    case "Char":
                        if (argType == StoreEnum.INTEGER || argType == StoreEnum.CHAR) break;
                        return TypeConversionError( parameter, argument, argType );
                    case "String":
                        break;
                    case "Boolean":
                        if (argType == StoreEnum.BOOL) break;
                        return TypeConversionError( parameter, argument, argType );

                }
            }

            Context.RunStack.Push( top );
            return ReturnState.NEXT;

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Type conversion error.
        /// </summary>
        /// <param name="parameter">    The parameter. </param>
        /// <param name="argument">     The argument. </param>
        /// <param name="argType">      Store type of argument. </param>
        /// <returns>
        /// Stop resturn  state.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState TypeConversionError(PVariable parameter, PExpression argument, StoreEnum argType)
        {
            return RuntimeError(argument.Offset, argument.Length, 0,
                "Cannot convert:  {0}  {1}  to {2}",
                argType.ToString().ToLower().Capitalize(), argument.ToString(), parameter.Type);
        }

        #endregion

        #region To String

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void HeaderToString(StringBuilder builder)
        {
            if (ObjectName == null)
            {
                builder.AppendFormat("{0}(", Name);
            }
            else
            {
                builder.AppendFormat("{0}.{1}(", ObjectName, Name);
            }
            for (int index = 0; index < Arguments.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }
                Arguments[index].ToString(builder);
            }
            builder.Append(")");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            if (ObjectName == null)
            {
                builder.Append("Call ");
            }
            HeaderToString(builder);
            base.ToString(builder);

        }

        #endregion
    }
}
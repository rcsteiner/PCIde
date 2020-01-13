////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the module class
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
using System.Collections.Generic;
using System.Text;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Module declaration
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PModule : PBlock
    {
        /// <summary>
        /// The name of the module
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameters for this module.
        /// </summary>
        public List<PVariable> Parameters { get; set; }


        //todo: THIS IS WRONG,
        /// <summary>
        /// Gets the size of the parameter frame.
        /// </summary>
        public int ParameterFrameSize { get { return  Parameters.Count  ; } }

        /// <summary>
        /// Gets or sets the references.
        /// </summary>
        public List<int> References { get; set; }

        /// <summary>
        /// Gets the header text
        /// </summary>
        public string Header
        {
            get
            {
                StringBuilder b = new StringBuilder();
                HeaderToString(b);
                return b.ToString();
            }
        }

        /// <summary>
        /// The visibility
        /// </summary>
        public VisibilityEnum Visibility;

        /// <summary>
        /// The is member
        /// </summary>
        public bool IsMember;

        /// <summary>
        /// Gets or sets the type of the module.
        /// </summary>
        /// <value>
        /// The type of the module.
        /// </value>
        public string  ModuleType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is constructor.
        /// </summary>
        public bool IsConstructor { get; set; }

        /// <summary>
        /// Gets or sets the class reference.
        /// </summary>
        public PClass ClassRef { get; set; }


        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string FullName { get { return ClassRef != null ? string.Format("{0}.{1}", ClassRef.Name, Name): Name; } }

        /// <summary>
        /// has return value
        /// </summary>
        public bool HasReturnValue;


        private const int MAX_CALL_DEPTH = 1000;

        /// <summary>
        ///   in Parse.
        /// </summary>
        protected static bool _inParse = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="visibility">The visibility.</param>
        /// <param name="isMember">if set to <c>true</c> [is member of a class].</param>
        /// <param name="pClass"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PModule(PContext context, VisibilityEnum visibility = VisibilityEnum.PUBLIC, bool isMember = false, PClass pClass=null) : base(context)
        {
            Parameters = new List<PVariable>();
            References = new List<int>();
            IsMember   = isMember;
            ModuleType = "Module";
            ClassRef   = pClass;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse module.  Just parsed off the module
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            if (_inParse)
            {
                SyntaxError(Source.Position, 2, "Expected the previous module/Function to end before starting this module. ");
            }
            _inParse = true;
            Source.FlushWhitespace();

           Source.ExpectedEnd.Push("Module");

            var pos = Source.Position;
            if (Source.ParseName() != TokenTypeEnum.NAME)
            {
                SyntaxError( pos, 2, "Expected the name of the Module after the 'Module' keyword." );
                if (Source.ParseName() != TokenTypeEnum.NAME)
                {
                    Source.MoveNextLine();
                }
            }

            Offset = Source.Start;
            var ret= ParseArgsAndBody( Source.Token );
            // now add a default return to force it to exit.

            if (Statements.Count >0)
            {
                var lastStatement = Statements[Statements.Count - 1];
                if (lastStatement is PReturn) return ret;
            }
            // force an exit statement
            Statements.Add(new PReturn(Context));
            return ret;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse arguments and body.
        /// </summary>
        /// <param name="name"> The name of the module. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ParseArgsAndBody(string name)
        {
            Source.FlushWhitespace();

            Name = name;
            // add it to the list of valid modules for this program
            Context.Root.AddModule(this);
          
            LineNumber = Source.LineNumber;

            EnterScope();
            var pos = Source.Position;

            if (!Source.Match('('))
            {
                SyntaxError(pos,1,"Expected a opening '(' after the module name. For example:  Module {0}(TYPE param1, '..)",Name);
                Source.MoveNextLine();
                // try parse body now.
            }
            else
            {

                if (IsMember)
                {
                    // if this is a member, the the first parameter is the object so, put a place holder variable
                    PVariable variable = new PVariable( Context, true );
                    variable.Name      = "this";
                    variable.Type      = ClassRef.Name;
                    variable.Module     = this;
                    Parameters.Add( variable );
                    Scope.Add( variable );
                }
                // parse parameters
                do
                {
                    if (Source.FlushWhitespace() == ')') break;

                    PVariable variable = new PVariable(Context, true);

                    variable.Module = this;

                    if (!variable.Parse())
                    {
                        Source.MoveNextLine(')');
                        break;
                    }

                    Parameters.Add(variable);
                    //if (!Variables.Add(variable))
                    //{
                    //    SyntaxError(variable.Offset,variable.Name.Length,"Variable '{0}' is already defined.",variable.Name);
                    //}
                } while (Source.MatchFlush(','));

                pos = Source.Position;
                if (!Source.MatchFlush(')'))
                {
                    SyntaxError(pos,1,"Missing closing ')' on module parameter list.");
                    Source.MoveNextLine();
                }
            }


            if (!ParseStatements())
            {
                LeaveScope();
                return false;
            }

            LeaveScope();

            ParseEnd();
            _inParse = false;

            // end of module
            return true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse end.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void ParseEnd()
        {
            Source.MatchEndStatement("Module");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// The stack has the parameters set set them up into the local frame.
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            if (Context.RunStack.CallStack.Count > MAX_CALL_DEPTH)
            {
                return RuntimeFatal(Offset, Name.Length, LineNumber, "Recursive depth exceeded.  Do you have a exit sentinel in the method?");
            }

            Context.RunStack.CallStack.Push(this);
            //todo: THIS IS WRONG
            Context.RunStack.CallEnter(VariableFrameSize - ParameterFrameSize);

            SetStackIndexForArgs();

            // if this is an object, then the first parameter is the object itself at -1
          
            // set the objectref to the current this pointer

        
            return ExecuteBody();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the stack index for arguments.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetStackIndexForArgs()
        {
            //TODO: this really only works for non-recursive code (CSC119 kind) the general fix
            //TODO: pmodule max require a separate variable storage  (runtime variable {Variable; stackIndex;} (a view model)

            foreach (var variable in Scope.Variables.Values)
            {
                variable.SetStackIndex();
                //if (variable.IsObject)
                //{
                //    foreach (var field in variable.ClassRef.Fields)
                //    {
                //    }
                //}
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Fixup reference arguments.
        /// </summary>
        /// <param name="call">     The call. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FixupRefArgs( PCall call)
        {
            if (Parameters.Count != call.Arguments.Count)
            {
                SyntaxError( call.Offset, call.Length, "{0} has {1} arguments but Expected {2} on Call {3}\r\nFor example: {4}", call.ToString(), call.Arguments.Count, Parameters.Count, Header,CallStringExample() );
            }
            for (int index = 0; index < Parameters.Count; index++)
            {
                var parameter = Parameters[index];
                var arg = call.Arguments[index];
                if (arg == null)
                {
                    SyntaxError( call.Offset, call.Length, "Missing parameter on {0} to  {1}\r\nFor example: {2}", call.ToString(), Header ,CallStringExample());
                    return;
                }
                if (parameter.IsRef)
                {
                    var eref = arg as PExpressionReference;
                    if (eref == null)
                    {
                        SemanticError( arg.Offset, arg.Length, call.LineNumber,
                            "The call to {0} Parameter {1} must be a variable to be passed by reference, Found {2}\r\nFor example: {3}",
                            Name,
                            parameter.Name, arg,CallStringExample() );
                    }
                    else
                    {
                        eref.IsRef = true;
                    }
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the body operation.
        /// </summary>
        /// <returns>
        /// the next step
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected ReturnState ExecuteBody()
        {
            var ret = base.Execute();
            if (ret == ReturnState.RETURN) ret = ReturnState.NEXT;
            return ret;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.Append( VisiblilityText( Visibility ) );
            HeaderToString(builder);
            ToStringBody(builder);
            builder.Indent();
            TailToString(builder);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Tail to string.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void TailToString(StringBuilder builder)
        {
            builder.AppendLine("End "+ModuleType);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Header to string.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void HeaderToString(StringBuilder builder)
        {
            builder.AppendFormat(ModuleType+" {0}(", Name);
            ToStringParameters(builder);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Converts a builder to a string parameters.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ToStringParameters(StringBuilder builder)
        {
            for (int i = 0; i < Parameters.Count; ++i)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                Parameters[i].ToString(builder);
            }
            builder.AppendLine(")");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Generate a call string example.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CallStringExample()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("{0}(", Name);
            for (int i = 0; i < Parameters.Count; ++i)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(Parameters[i].Name);
            }
            builder.AppendLine(")");
            return builder.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Converts a builder to a string body.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ToStringBody(StringBuilder builder)
        {
            base.ToString(builder);
        }

    }
}
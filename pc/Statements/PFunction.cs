////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the function class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   3/3/2015   rcs     Initial Implementation
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

using System;
using System.Text;

namespace pc
{


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Pseudo Function. 
    /// Function Type Name(args...)  Statements* End Function
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PFunction : PModule
    {
        /// <summary>
        /// The return type
        /// </summary>
        public String ReturnType;

        /// <summary>
        /// Gets or sets the function if it is builtin.
        /// </summary>
        public FunctionInfo Function { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">      The context. </param>
        /// <param name="visibility">   (optional) the visibility. </param>
        /// <param name="isMember">     (optional) true if is member. </param>
        /// <param name="pClass">       (optional) the class. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PFunction(PContext context, VisibilityEnum visibility = VisibilityEnum.PUBLIC, bool isMember = false, PClass pClass=null)
            : base( context,visibility,isMember )
        {
            HasReturnValue = true;
            ModuleType     = "Function";
            ClassRef       = pClass;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">      The context. </param>
        /// <param name="name">         The name. </param>
        /// <param name="returnType">   Type of the return. </param>
        /// <param name="function">     (optional) the function to support this method. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PFunction(PContext context, string name, FunctionInfo  function ) : base(context)
        {
            Name           = name;
            ReturnType     = function.ReturnType.ToString().ToLower();
            HasReturnValue = true;
            ModuleType     = "Function";
            Function       = function;

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
                SyntaxError(Source.Position, 2, "Expected the previous module/Function to end before starting this Function. ");
            }
            _inParse = true;
            Source.ExpectedEnd.Push("Function");

            Source.ParseToken();
            var pos = Source.Position;
            if (Source.CheckSystemType())
            {
                ReturnType = Source.Token;
            }
            else
            {
                SyntaxError(pos,Source.Token.Length,"Functions require a return type. for example: Function Integer MyFunction(). Assuming Integer");
                ReturnType = "Integer";
            }
            Source.FlushWhitespace();

            if (Source.ParseName() != TokenTypeEnum.NAME)
            {

                SyntaxError(-1,-1, "Expected the name of the Function after the 'Function Type' keyword. for example: Function {0} MyFunction()",ReturnType );
                Source.MoveNextLine();
            }
            Offset = Source.Start;

            var ret = ParseArgsAndBody( Source.Token );

            // MUST end in return to force it to exit.

            if (Statements.Count > 0)
            {
                var lastStatement = Statements[Statements.Count - 1];
                if (lastStatement is PReturn)
                {
                    //TODO: this is not really good enough (could be if (x) return y else return x (detectable at runtime)
                    return ret;
                }
            }
            // force an exit statement
            Statements.Add( new PReturn( Context ) );
            return ret;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse end.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void ParseEnd()
        {
            Source.MatchEndStatement( "Function" );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack. The stack has the parameters set set them up into the local frame.
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            if (Function.Function != null)
            {
                return ExecuteBuiltIn();
            }
            return base.Execute();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the built in operation.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState ExecuteBuiltIn()
        {
            //TODO add builtin function execute.
                return ReturnState.NEXT;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Invokes a method by name
        ///// </summary>
        ///// <param name="methodName">   Name of the method. </param>
        ///// <param name="arguments">    A variable-length parameters list containing arguments. </param>
        ///// <returns>
        ///// the accumalator, will throw an exeception if wrong parameters or fails.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public static Accumulator Invoke(string methodName, params Accumulator[] arguments)
        //{
        //    BuiltIn method;

        //    if (_methods.TryGetValue( methodName, out method ))
        //    {
        //        return method( arguments );
        //    }
        //    throw new ArgumentException( "Function Name = " + methodName + " is not a valid method." );
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Header to string.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void HeaderToString(StringBuilder builder)
        {
            builder.AppendFormat(ModuleType+" {0} {1}(", ReturnType, Name);
            ToStringParameters( builder );
        }
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the display class
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
    /// Input. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    class PInput : PStatement
    {
        /// <summary>
        /// The expressions.
        /// </summary>
        public List<PExpressionReference> Expressions;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PInput(PContext context) : base(context)
        {
            Expressions =new List<PExpressionReference>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Inputs the elements of a comma separated list of tokens.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            do
            {
                switch (Source.ParseToken())
                {
                    case TokenTypeEnum.NAME:
                        // token is variable name, 
                        int pos = Source.Start;
                        int len = Source.TokenLength;
                        var variable = new PExpressionReference(Context,Source.Token);
                        variable.Parse();
                        if (variable.IsCall )
                        {
                             SyntaxError(pos,len,"Expected a simple variable, but found function:   {0}",variable.ToString());
                            Source.MoveNextLine();
                            return false;
                        }
                        Expressions.Add( variable);
                        break;

                    case TokenTypeEnum.EOL:
                    case TokenTypeEnum.EOF:
                    case TokenTypeEnum.COMMENT:
                        SyntaxError(-1,-1,"Expected a variable on input statement. Example:  'Input myVariable'");
                        return false;

                    case TokenTypeEnum.INTEGER:
                    case TokenTypeEnum.REAL:
                        SyntaxError(-1,-1, "Numbers can't be used as a variable for an input statement." );
                        return false;

                    case TokenTypeEnum.STRING:
                        SyntaxError(-1,-1, "Strings can't be used as a variable for an input statement." );
                        return false;

                    default:
                        SyntaxError(-1,-1,"Only variables are allowed in input statement.");
                        return false;
                }
            } while (Source.Match( ','));
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            base.Execute(); // sets line number

            foreach (var v in Expressions)
            {

                PVariable variable = v.Variable;
                if (variable.IsConstant)
                {
                   return  RuntimeError(v.Offset, v.Length, 0, "The Constant '{0}' cannot be used for input", variable.Name);
                }

                Accumulator vx = new Accumulator();

                switch (variable.Type)
                {
                    case "Real" :
                        vx = new Accumulator(Context.UserInput.InputDouble());
                        break;
                    case "Integer":
                        vx = new Accumulator( Context.UserInput.InputInteger() );
                        break;
                    case "String":
                        vx = new Accumulator(Context.UserInput.InputString());
                        break;
                    case "Char":
                        vx = new Accumulator( Context.UserInput.InputChar() );
                        break;
                    case "Object":
                        vx = new Accumulator( Context.UserInput.InputChar() );
                        break;
                    case "Bool":
                        vx = new Accumulator( Context.UserInput.InputChar() );
                        break;
                }

                //if (v.IsRef)
                //{
                //    int runIndex = v.RunIndex;
                //    Context.RunStack.SetValue( vx, runIndex, true );
                //}
                //else
                //{
       
                v.Value = vx;
               //     variable.Value = vx;
                //}

            }
            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.Append("Input ");
            for (int index = 0; index < Expressions.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }
                Expressions[index].ToString(builder);
            }
            base.ToString( builder );

        }

    }
}
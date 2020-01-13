////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the set class
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
    /// Set a variable to a expression
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    class PExpressionAssign : PExpression
    {
        /// <summary>
        /// The variable reference
        /// </summary>
        public PExpressionReference VariableReference;

        /// <summary>
        /// The expression
        /// </summary>
        public PExpression Expression;

        /// <summary>
        /// The operator to use
        /// </summary>
      //  public OperatorEnum Op;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpressionAssign(PContext context) : base(context)
        {
            LineNumber = Source.LineNumber;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the value of a variable to a value (possible expression here!)
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            if (Source.ParseToken() != TokenTypeEnum.NAME)
            {
                SyntaxError(-1,-1," An assignment must have a variable after the reserved word Set");
                return false;
            }
            VariableReference = new PExpressionReference(Context,Source.Token);
            if (VariableReference.Parse())
            {
                if (Source.MatchFlush('='))
                {
                    Source.ParseToken();
                    var pos = Source.Position;
                    Expression = Parser.ParseExpression();
                    if (Expression == null)
                    {
                        SyntaxError(pos,2,"Expected a expression in this Set statement to follow the '='");
                        return false;
                    }
                    return true;
                }
                SyntaxError(-1,-1,"Expected a '=' after the variable name");
                return false;
            }
            SyntaxError(-1,-1,"Expected a variable name to follow the 'Set' reserved word.");
            return false;
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
            base.Execute();
            if (Expression.Execute() == ReturnState.STOP) return ReturnState.STOP;
            VariableReference.Variable.Value = Context.RunStack.Pop();
            //if (VariableReference.IsRef)
            //{
            //    int runIndex = VariableReference.RunIndex;
            //    Context.RunStack.SetValue(Context.RunStack.Pop(),runIndex,true);
            //}
            //else
            //{
            //    VariableReference.Variable.Value = Context.RunStack.Pop();
            //}
            return ReturnState.NEXT;


   /*      future assignment types   
                        case OperatorEnum.EQUMOD: //   "%=" 
                        case OperatorEnum.EQUMUL: //   "*=" 
                        case OperatorEnum.EQUDIV: //   "/=" 
                        case OperatorEnum.EQUPLS: //   "+=" 
                        case OperatorEnum.EQUMIN: //   "-=" 
                        case OperatorEnum.EQUSHL: //   "<<="
                        case OperatorEnum.EQUSHR: //   ">>="
                        case OperatorEnum.EQUOR:  //   "|=" 
                        case OperatorEnum.EQUAND: //   "&=" 
                        case OperatorEnum.EQUXOR: //   "^=" 

                        case OperatorEnum.EQU:    //   "="  
                            l = r;
                            break;
*/
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.AppendFormat("{0} = ", VariableReference.Name);
            Expression.ToString(builder);
        }

    }
}
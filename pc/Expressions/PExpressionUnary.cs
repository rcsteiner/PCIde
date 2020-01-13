////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the expression unary class
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
    /// Expression unary. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PExpressionUnary : PExpression
    {
        /// <summary>
        /// The right.
        /// </summary>
        public PExpression Right;

        /// <summary>
        /// The operator
        /// </summary>
        public OperatorEnum Operator;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpressionUnary(PContext context) : base(context)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        /// <param name="expr">     The expression. </param>
        /// <param name="op">       The operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpressionUnary(PContext context, PExpression expr, OperatorEnum op ) : base(context)
        {
            Right = expr;
            Operator = op;
            LineNumber = Source.LineNumber;
        }

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

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this unary operator onto the runtime stack.
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            base.Execute(); // sets line number

            var r = Right.Execute();
            if (r != ReturnState.NEXT)
            {
                return r;
            }
            var acc = Context.RunStack.Pop();

            switch (Operator.Value)
            {
                case OperatorEnum.NOT: //   "!"  
                    acc.BValue =! acc.BValue;
                    break;
                case OperatorEnum.INC: //   "++" 
                    acc.DValue ++;
                    break;
                case OperatorEnum.ADD: //   "+"  
                    break;
                case OperatorEnum.DEC: //   "--" 
                    acc.DValue--;
                    break;
                case OperatorEnum.SUB: //   "-"  
                    acc.DValue = -acc.DValue;
                    break;
                case OperatorEnum.BINV:   //   "~"  
                    acc.LValue = ~acc.LValue;
                    break;

                case OperatorEnum.POSTDEC:
                    acc.DValue--;
                    break;
                case OperatorEnum.POSTINC:
                    acc.DValue++;
                    break;
                default:
                    return RuntimeError(Offset,Length, 0, "Unknown operator {0}", Operator.ToString());
            }
            Context.RunStack.Push(acc);
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
            builder.AppendFormat( " {0}", Operator.ToString() );
            Right.ToString( builder );
        }
    }
}
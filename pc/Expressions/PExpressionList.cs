////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the expression list class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/26/2015   rcs     Initial Implementation
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
    /// List of p expressions. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PExpressionList : PExpression
    {
        /// <summary>
        /// The expressions that form the list.
        /// </summary>
        public List<PExpression> Expressions;

        /// <summary>
        /// Gets or sets the <see cref="PExpression"/> at the specified index.
        /// </summary>
        public PExpression this[int index] {
            get
            {
                if (Expressions.Count > index)
                {
                    return Expressions[index];
                }
                return null;
            }
            set { Expressions[index] = value; }}

        /// <summary>
        /// Gets the count of expressions in the list.
        /// </summary>
        public int Count { get { return Expressions.Count; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpressionList(PContext context) : base(context)
        {
            Expressions = new List<PExpression>();
            LineNumber = Source.LineNumber;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds expression to the list.
        /// </summary>
        /// <param name="expression">   The PExpression to add. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Add(PExpression expression)
        {
            Expressions.Add(expression);
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
            Source.FlushWhitespace();
            Offset = Source.Position;
            do
            {
                var p = Source.Position;
                Source.ParseToken();
                var expr = Parser.ParseExpression();
                if (expr == null)
                {
                    // backup
                    Source.Position = p;
                    break;
                }
                Expressions.Add(expr);
                //switch (Source.ParseToken())
                //{
                //    case TokenTypeEnum.NAME:
                //        // token is variable name, 
                //        var vRef = new PExpressionReference( Context, Source.Token );
                //        vRef.BindToVariable();
                //        Expressions.Add( vRef );
                //        break;

                //    case TokenTypeEnum.REAL:
                //        Expressions.Add( new PLiteralReal( Context, Source.Token ) );
                //        break;

                //    case TokenTypeEnum.INTEGER:
                //        Expressions.Add( new PLiteralInteger( Context, Source.Token ) );
                //        break;

                //    case TokenTypeEnum.STRING:
                //        Expressions.Add( new PLiteralString( Context, Source.Token ) );
                //        break;

                //    case TokenTypeEnum.COMMENT:
                //        // token is a comment, end of statement
                //        break;
                //    case TokenTypeEnum.EOL:
                //        // token is end of line, end of statement
                //        return true;
                //    case TokenTypeEnum.EOF:
                //        // token is end of file, endo of statement
                //        return true;
                //    default:
                //        SyntaxError(-1,-1, "Only variables and expressions are allowed argument list." );
                //        return false;
                //}
            } while (Source.MatchFlush( ',' ));
            Length = Source.Position - Offset;
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// The values are left on the stack in reverse order.
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        /// exit if not Next
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            base.Execute();
            for (int index = Expressions.Count - 1; index >= 0; index--)
            {
                var argument = Expressions[index];
                var r = argument.Execute();
                if (r != ReturnState.NEXT)
                {
                    return r;
                }
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
            for (int index = 0; index < Expressions.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append( ", " );
                }
                Expressions[index].ToString( builder );
            }
        }

    }
}
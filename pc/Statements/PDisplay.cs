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

using System.Text;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Display. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PDisplay : PStatement
    {
        /// <summary>
        /// The expressions.
        /// </summary>
        public PExpressionList Expressions;


        /// <summary>
        /// The no line feed flag ';' at the end of the line.
        /// </summary>
        public bool NoLineFeed;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PDisplay(PContext context) : base(context)
        {
            Expressions =new PExpressionList(context);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Displays the elements of a comma separated list of tokens.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            if (Expressions.Parse() && Expressions.Count >0)
            {
                if (Source.FlushWhitespace() == ';')
                {
                    Source.MoveNext();
                    NoLineFeed = true;
                }
                return true;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.Append("Display ");
            for (int index = 0; index < Expressions.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }
                if (NoLineFeed)
                {
                    builder.Append(';');
                }
                Expressions[index].ToString(builder);
            }
            base.ToString( builder );

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

            for (int i = 0; i < Expressions.Count; i++)
            {
                if (Expressions[i].Execute() == ReturnState.STOP)
                {
                    return ReturnState.STOP;
                }
                var expr = Context.RunStack.Pop();
                switch (expr.Store)
                {
                    case StoreEnum.REAL:
                        UserOutput.Output("{0}",expr.DValue);
                        break;
                    case StoreEnum.BOOL:
                        UserOutput.Output("{0}",expr.BValue);
                        break;
                    case StoreEnum.STRING:
                        var text = expr.SValue;
                        if (text == "\\r" )
                        {
                            UserOutput.OutputLine(text);
                            return ReturnState.NEXT;
                        }
                        if ( text=="\r")
                        {
                            UserOutput.OutputLine( "\\r" );
                            return ReturnState.NEXT;
                        }
                        if (text == "\\a" || text=="\a")
                        {
                            Beep();
                            return ReturnState.NEXT;
                        }
                        UserOutput.Output("{0}",text);
                        break;

                    case StoreEnum.INTEGER:
                        UserOutput.Output("{0}",expr.LValue);
                        break;
                    case StoreEnum.CHAR:
                        UserOutput.Output( "{0}", expr.CValue );
                        break;
                }
            }
            if (!NoLineFeed)
            {
                UserOutput.OutputLine("");
            }
            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Beep sound.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Beep()
        {
            System.Media.SystemSounds.Exclamation.Play();
           // Console.Beep(1000,2000);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
//   Descrption:
//  
//  Defines a do loop statement.
//  
//  Author: Robert C. Steiner
//  
//  ======================== History ======================
//  
//  Date        Who      What
// ----------- ------   ---------------------------
//  02/03/2012  rcs     Refactored from original
// 
//  ======================================================
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
// 
// --------------------------------------------------------------------------------------------------------------------

using System.Text;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   
    /// Pseudo Statement do loop. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PDoLoop : PLoop
    {
        public bool IsUntil;

        public string LoopType {get { return IsUntil ? "Until" : "While"; }}



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Constructor. 
        /// </summary>
        /// <param name="context">         Context for the parse. </param>
        /// <param name="continueCondition">    The continue condition. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PDoLoop(PContext context, PExpression continueCondition=null) : base(context, continueCondition)
        {
        }

        #region Parse

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Parse this language element. 
        /// </summary>
        /// <param name="parser">   The Parser. </param>
        /// <returns>   
        /// true if a successful parse. 
        /// </returns>
        /// <remarks>   
        /// sDo  = 'do' Statement 'While'  Expression  
        ///      |  Do Statement 'Until' Expresson
        /// </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {

            Context.DoLevel++;

           Source.ExpectedEnd.Push("While|Until");

            var pos = Source.Position;

            if (!ParseBody())
            {
                 SyntaxError(pos-1,2, "do statement requires at least one statement." );
            }

            switch (Source.Token)
            {
                case "while":
                    Source.MatchTokenCheckCase("While", "Expected a {0} found {1}");
                    IsUntil = false;
                    break;
                case "While":
                    IsUntil = false;
                    break;

                case "Until":
                    IsUntil = true;
                    break;
                   

                case "until":
                    Source.MatchTokenCheckCase("Until", "Expected a {0} found {1}");
                    IsUntil = true;
                    break;
                default:
                SyntaxError(-1,-1, "do statement requires a 'While' or an 'Until' after the 'Do statement(s)'." );
                    return true;
            }
            Source.ExpectedEnd.Pop();
            Source.ParseToken();
            ContinueCondition = Parser.ParseBoolExpression();

            if (ContinueCondition == null)
            {
                SyntaxError(-1,-1, "Do {0} statement requires a test expression." ,LoopType);
            }

            Context.DoLevel--;

            return true;

        }


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
            do
            {
               var body = base.Execute();
               
                switch (body)
                {
                    case ReturnState.NEXT:
                    case ReturnState.CONTINUE:
                        break;
                  
                    case ReturnState.STOP:
                        return ReturnState.STOP;

            
                    case ReturnState.BREAK:
                        return ReturnState.NEXT;
                }

                ContinueCondition.Execute();
                var test = Context.RunStack.Pop();
                var ret = (test.ConvertToBool() && test.BValue);
                if (!IsUntil)
                {
                    if (!ret) break;
                }
                else
                {
                    if (ret) break;
                }
            } while (true);

            return ReturnState.NEXT;
        }



        #endregion

        #region ToString

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Members to string. 
        /// </summary>
        /// <param name="builder">      The builder. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.AppendLine( "Do" );
            base.ToString( builder );
            builder.Indent();
            builder.Append( LoopType );
            builder.Append(' ');
            if (ContinueCondition != null)
            {
                ContinueCondition.ToString(builder);
            }
            builder.AppendLine();
        }

        #endregion


    }
}
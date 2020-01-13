// --------------------------------------------------------------------------------------------------------------------
//   Descrption:
//  
//  Defines a while loop.
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
    /// Z coordinate statement while loop. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PWhileLoop : PLoop
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Constructor. 
        /// </summary>
        /// <param name="context">         Context for the parse. </param>
        /// <param name="continueCondition">    The continue condition. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PWhileLoop(PContext context, PExpression continueCondition=null) : base(context, continueCondition)
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
        ///  <remarks>   
        ///   sWhile  = 'While' '(' Expression ')' Statement. 
        /// </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            Source.ParseToken();

           Source.ExpectedEnd.Push("While");

            var pos = Source.Position;

            ContinueCondition = Parser.ParseBoolExpression();

            if (ContinueCondition == null)
            {
                 SyntaxError(pos,2, "while statement requires a boolean test expression." );
            }

             ParseBody();
            Source.MatchEndStatement( "While" );
            return true;
        }


        #endregion

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
            Context.RunStack.SetCurrentLine(LineNumber, this);

            do
            {
                ContinueCondition.Execute();
                var test = Context.RunStack.Pop();
                var ret = (test.ConvertToBool() && test.BValue);
                if (!ret) break;

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

            }
            while (true);

            return ReturnState.NEXT;
        }

        #region ToString

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Members to string. 
        /// </summary>
        /// <param name="builder">      The builder. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.Append("While");
            if (ContinueCondition != null)
            {
                ContinueCondition.ToString(builder);
            }
            builder.AppendLine();
            base.ToString( builder );
            builder.Indent();
            builder.AppendLine( "End While" );
        }

        #endregion


    }
}
// --------------------------------------------------------------------------------------------------------------------
//   Descrption:
//  
//  Defines a foreach loop.
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
// --------------------------------------------------------------------------------------------------------------------

using System.Text;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   
    /// Z  statement for each loop. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PForEachLoop : PLoop
    {
        /// <summary>
        /// Gets or sets the iterator variable.
        /// </summary>
        public PExpressionReference Iterator { get; set; }
      
        /// <summary>
        /// Gets or sets the iterator list.
        /// </summary>
        public PExpressionReference IteratorList { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">              The context. </param>
        /// <param name="continueCondition">    (optional) the continue condition. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PForEachLoop(PContext context, PExpression continueCondition = null) : base(context, continueCondition)
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
        /// sForEach    = 'ForEach' Entity 'In' NamedPool CompoundStatement.            (syntax A) CCal
        /// sForEach    = 'ForEach' '(' variable  'In' NamedPool ')' CompoundStatement. (syntax B) C#
        /// sForEach    = 'ForEach' variable  'In' NamedPool                            (syntax B) pseudo
        /// </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            var pos = Source.Position;
           Source.ExpectedEnd.Push("ForEach");

            var name = Parser.ParseName();
            if (name == null)
            {
                 SyntaxError(pos,2, "Foreach iterator must have a variable name to use as iterator." );
                 name = "<iterator>";
            }

            Iterator = new PExpressionReference( Context,name);
            Iterator.BindToVariable( Offset, false );

            if (!ParseInClause()) return false;

            ParseBody();
            return Source.MatchEndStatement( "ForEach" );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Parse in xxxx clause. 
        /// </summary>
        /// <returns>   
        /// true if it succeeds, false if it fails. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseInClause()
        {
            var pos = Source.Position;

            if (!Source.Match( "In" ))
            {
                 SyntaxError(pos,2, "Foreach statement requires the word 'In' before the array name." );
            }
            // parse collection
            Source.ParseToken();
            IteratorList = Parser.ParseExpression() as PExpressionReference;

            if (IteratorList == null)
            {
                SyntaxError( pos, 2, "foreach statement requires a collection reference expression." );
                IteratorList = new PExpressionReference(Context,"<Collection>");
            }
            return true;
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// On entry execute the initial expression
        /// Execute the test immediately
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        /// Next if continue else condition.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            Context.RunStack.SetCurrentLine( LineNumber, this );

            int len;

            if (IteratorList.Variable.IsArgument)
            {
                len = Context.RunStack[IteratorList.Variable.RunIndex].Array.Dimensions[0];
            }
            else
            {
                len = IteratorList.Variable.DimensionSize[0];
            }
            // we now have the info we need to execute the loop

            for (int count=0; count < len; ++count)
            {
                var i = IteratorList.Variable.Value.Array[count];
                Iterator.Value = i;
                // execute the next element in the array 
                var returnState = base.Execute();

                switch (returnState)
                {
                    case ReturnState.NEXT:
                    case ReturnState.CONTINUE:
                        continue;

                    case ReturnState.BREAK:
                        Context.RunStack.Pop();
                        return ReturnState.NEXT;

                    case ReturnState.STOP:
                        Context.RunStack.Pop();
                        return returnState;

                    case ReturnState.RETURN:
                        return returnState;
                }
            }
         //   Context.RunStack.Pop();
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
            builder.AppendFormat( "ForEach {0}", Iterator.Name );
            builder.Append( " In " );
            if (ContinueCondition != null) ContinueCondition.ToString(builder);
            else builder.Append("???");
            builder.AppendLine();
            base.ToString( builder );
            builder.Indent();
            builder.AppendLine( "End ForEach" );
        }

        #endregion

    }
}
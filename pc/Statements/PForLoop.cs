// --------------------------------------------------------------------------------------------------------------------
//   Descrption:
//  
//  Defines a for loop.
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
    /// Pseudo Statement for loop. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PForLoop : PLoop
    {
        /// <summary>
        /// Gets or sets the intial statement.
        /// </summary>
        public  PExpressionReference VariableRef { get; set; }

        /// <summary>
        /// Gets or sets the increment.
        /// </summary>
        public PExpression Increment { get; set; }

        /// <summary>
        /// Gets or sets the initial expression.
        /// </summary>
        public PExpression InitialExpression { get; set; }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Constructor. 
        /// </summary>
        /// <param name="context">         Context for the parse. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PForLoop(PContext context ) : base(context)
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
        /// sFor  = 'For' Variable = Initial To Max statements End For 
        /// </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
           Source.ExpectedEnd.Push("For");

            VariableRef = Source.ParseToken() == TokenTypeEnum.NAME ? new PExpressionReference( Context, Source.Token ) : new PExpressionReference( Context, "<ForVariable>");
          
            // find variable and create it if it doesn't exist.
            VariableRef.BindToVariable(Offset,false);

            if (Source.MatchFlush('='))
            {
                Source.ParseToken();
                InitialExpression = Parser.ParseExpression();
                var pos = Source.Position;
                Source.ParseToken();
                if (!Source.MatchTokenCheckCase("To","Expected {0} found {1}"))
                {
                    SyntaxError(pos,Source.Token.Length, "For statement requires a 'To' after 'For'." );
                }

                //InitialStatement = ParseStatement();
                pos = Source.Position;
             
                Source.ParseToken();
                ContinueCondition = Parser.ParseExpression();
                if (ContinueCondition == null)
                {
                    SyntaxError(pos,2, "for statement requires an end value." );
                    ContinueCondition = new PLiteralInteger( Context, "10" );
                }
                pos = Source.Position;
                Source.ParseToken();
                if (Source.MatchTokenCheckCase("Step", "It appears to be a Skip clause but found {1}"))
                {
                    pos = Source.Position;
                    Source.ParseToken();
                    Increment = Parser.ParseExpression();
                    if (Increment == null)
                    {
                        SyntaxError(pos,2,"Expected a Step increment expression.");
                        Increment = new PLiteralInteger(Context,"1");
                    }
                }
                else
                {
                    Source.Position = pos;
                }

                
            }            //if (ContinueCondition == null)
            //{
            //    SyntaxError(-1,-1, "for statement requires a continue expression." );
            //}

            //if (!Parser.Match( ';' ))
            //{
            //    return SyntaxError(-1,-1, "for statement requires a ';' after test." );
            //}

            //Increment = ParseExpression();
            //if (Parser.Match(','))
            //{
            //    // increment is an expression list
            //    var ilist = new PExpressionList(PContext);
            //    ilist.Add(Increment);
            //    Increment = ilist;
            //    ilist.Parse(parser);
            //}

            //if (!Parser.Match( ')' ))
            //{
            //    return SyntaxError(-1,-1, "for statement requires a ')' after 'increment'." );
            //}
            //return ParseBody();
            ParseBody();
            Source.MatchEndStatement("For");
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
           // base.Execute(); // sets line number

            if (InitialExpression.Execute() == ReturnState.STOP)
            {
                return ReturnState.STOP;
            }
            VariableRef.Variable.Value = Context.RunStack.Pop();
            int value = (int) VariableRef.Variable.Value.LValue;


            if (ContinueCondition.Execute()==ReturnState.STOP)
            {
                return ReturnState.STOP;
            }
            int end = (int) Context.RunStack.Pop().LValue;
      
            int step = 1;
       
            if (Increment != null)
            {
                if (Increment.Execute()==ReturnState.STOP)
                {
                    return ReturnState.STOP;
                }
                step = (int)Context.RunStack.Pop().LValue;
            }

            // we now have the info we need to execute the loop


            while ( step<0 ? (value>= end): (value <=end))
            {
                var returnState = base.Execute();

                switch (returnState)
                {
                    case ReturnState.NEXT:
                    case ReturnState.CONTINUE:
                        value += step;
                        VariableRef.Variable.Value = new Accumulator(value);
                        continue;

                    case ReturnState.BREAK:
                        return ReturnState.NEXT;

                    case ReturnState.RETURN:
                    case ReturnState.STOP:
                        return returnState;
                }
            }
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
            builder.AppendFormat("For {0}", VariableRef.Name);
            if (InitialExpression != null)
            {
                builder.Append(" = ");
                InitialExpression.ToString(builder);
            }

            builder.Append(" To ");
            if (ContinueCondition != null)
            {
                ContinueCondition.ToString(builder);
            }
            else
            {
                builder.Append("???");
            }
            if (Increment != null)
            {
                builder.Append(" Step ");
                Increment.ToString(builder);
            }
            builder.AppendLine();
            base.ToString(builder);
            builder.Indent();
            builder.AppendLine("End For");
        }

        #endregion
    }
}
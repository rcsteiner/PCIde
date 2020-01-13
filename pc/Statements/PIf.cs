////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the if statement class
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
    /// If else statement 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PIf : PStatement
    {
        /// <summary>
        /// The elements.
        /// </summary>
        public PBlock IfStatements;

        /// <summary>
        /// The else statements
        /// </summary>
        public PBlock ElseStatements;
    
        /// <summary>
        /// The test
        /// </summary>
        public PExpression Test;


        /// <summary>
        /// The else if clauses
        /// </summary>
        public List<PIf> ElseIfClauses = new List<PIf>();


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PIf(PContext context) : base(context)
        {
            IfStatements   = new PBlock(context);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Enter scope.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void EnterScopeIf()
        {
            Context.ScopeStack.Push( IfStatements );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Enter scope.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void EnterScopeElse()
        {
            Context.ScopeStack.Push( ElseStatements );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Leave scope.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LeaveScope()
        {
                Context.ScopeStack.Pop();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parses this if statement 
        /// If expression Then statements [ Else statements] End If.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            var pos = Source.Position;
           Source.ExpectedEnd.Push("If");

            Source.ParseToken();
            Test = Parser.ParseBoolExpression();

            if (Test == null)
            {
                SyntaxError(pos,2,"If statement requires a test expression that evaluates to a boolean.");
            }
            pos = Source.TokenLength>0?  Source.Position-Source.TokenLength:Source.Position;
            Source.ParseToken();
            switch (Source.Type)
            {
                case TokenTypeEnum.PUNCTUATION:
                    SyntaxError(-1,-1, "Expected a 'Then' after the If test expression. But found punctuation '{0}'",Source.Token );
                    Source.ParseToken();
                    break;
                case TokenTypeEnum.OPERATOR:
                    SyntaxError(-1,-1, "Expected a 'Then' after the If test expression. But found operator '{0}'",Source.Token );
                    Source.MoveNextLine();
                    break;

            }
            if (!Source.MatchTokenCheckCase( "Then", "Expected a {0} but found {1}", pos ))
            {
                SyntaxError( pos, 4, "Expected a 'Then' after the If test expression. Assuming Then here.\r\nExample: If {0} Then", Test != null ? Test.ToString() : "condition" );
            }
            EnterScopeIf();
            Parser.ParseStatements(IfStatements.Statements);
            LeaveScope();

            again:

            if (Source.Token.Equals( "Else" ))
            {
                Source.FlushWhitespaceEol();
                if (Source.Match("If"))
                {
                    // this is an Else If clause instead so mark it as such
                    var elseIf = new PIf(Context);
                    ElseIfClauses.Add(elseIf);
                    Source.ParseToken();
                    elseIf.Test = Parser.ParseBoolExpression();
                    pos = Source.TokenLength > 0 ? Source.Position - Source.TokenLength : Source.Position;
                    Source.ParseToken();
                    if (!Source.MatchTokenCheckCase( "Then", "Expected a {0} but found {1}", pos ))
                    {
                        SyntaxError( pos, 4, "Expected a 'Then' after the Else If test expression. Assuming Then here.\r\nExample: Else If {0} Then",elseIf.Test != null ? elseIf.Test.ToString() : "condition");
                    }
                    EnterScopeIf();
                    Parser.ParseStatements( elseIf.IfStatements.Statements);
                    LeaveScope();
                    goto again;
                }
                ElseStatements = new PBlock( Context );
                EnterScopeElse();
                Parser.ParseStatements( ElseStatements.Statements );
                LeaveScope();
            }

 //           Source.FlushToToken();
            if (!Source.MatchEndStatement("If"))
            {
                SyntaxError( -1, -1, "Missing 'End If' statement here. " );

            }
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

            Test.Execute();
            Accumulator test = Context.RunStack.Pop();
            if (!test.ConvertToBool())
            {
                return RuntimeError(Test.Offset, Test.Length, 0, "Can't convert {0} to boolean value.", Test.ToString());
            }
            if (test.BValue)
            {
                return IfStatements.Execute();
            }

            foreach (var elseIfClause in ElseIfClauses)
            {
                elseIfClause.Test.Execute();
                test = Context.RunStack.Pop();
                if (!test.ConvertToBool())
                {
                    return ReturnState.STOP;
                }
                if (test.BValue)
                {
                    return elseIfClause.IfStatements.Execute();
                }
            }

            if (ElseStatements!=null)
            {
                return ElseStatements.Execute();
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
            builder.Append("If ");
            if (Test != null)
            {
                Test.ToString(builder);
            }
            builder.AppendLine(" Then");
            IfStatements.ToString(builder);
            if (ElseIfClauses.Count > 0)
            {
                foreach (var elseIf in ElseIfClauses)
                {
                    builder.Indent();
                    builder.Append("Else If ");
                    if (elseIf.Test != null) elseIf.Test.ToString(builder);
                    builder.AppendLine(" Then");
                    if (elseIf.IfStatements != null) elseIf.IfStatements.ToString(builder);
                }
            }

            if (ElseStatements != null)
            {
                builder.Indent();
                builder.AppendLine("Else");
                ElseStatements.ToString(builder);
            }
            builder.Indent();
            builder.AppendLine("End If");
        }
    }
}
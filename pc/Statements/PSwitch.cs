////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the statement switch class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   3/3/2015   rcs     Initial Implementation
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
using System;
using System.Collections.Generic;
using System.Text;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   
    /// Z  statement switch. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PSwitch : PStatement
    {
        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        public PExpression Selector { get; set; }
     
        /// <summary>
        /// Gets or sets the cases.
        /// </summary>
        public List<PCase> Cases { get; set; }
      
        /// <summary>
        /// Gets or sets the default.
        /// </summary>
        public PCase Default { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Constructor. 
        /// </summary>
        /// <param name="context"> Context for the parse. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PSwitch(PContext context) : base(context)
        {
            Cases    = new List<PCase>();
        }


        #region Add Cases

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a case.
        /// </summary>
        /// <param name="caseLiteral">  The case value. </param>
        /// <param name="comment">      (optional) the comment. </param>
        /// <returns>
        /// the new case.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PCase AddCase(PExpression caseLiteral, string comment=null)
        {
            var zCase = Cases.Find( x => x.Label == caseLiteral.ToString() );
            if (zCase == null)
            {
                zCase = new PCase( Context,this, caseLiteral );
                Cases.Add( zCase );
                zCase.Comment = comment;
            }
            return zCase;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Makes the case.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// the new case.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PCase AddCase(string name)
        {
            var c = new PCase( Context, this, new PExpressionReference(Context,name) );
            Cases.Add(c);
            return c;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Check if case exits. 
        /// </summary>
        /// <param name="caseText"> case value as a string. </param>
        /// <returns>   
        /// true if it has it.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool HasCase(string caseText)
        {
            return Cases.Find( x => x.Label.Equals(caseText))!=null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Sorts this cases by labels.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Sort()
        {
            Cases.Sort( (a, b) => String.CompareOrdinal( a.Label.ToString(), b.Label.ToString() ) );
        }

        #endregion

        #region Parse

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Parse this language element. 
        /// </summary>
        /// <param name="parser">   The Parser. </param>
        /// <returns>   
        /// true if a successful parse. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
           Source.ExpectedEnd.Push("Select");

            LineNumber = Source.LineNumber;
            Source.ParseToken();
            var pos = Source.Position;

            var selectorExpr = Parser.ParseExpression();

            if (selectorExpr != null)
            {
                Selector = selectorExpr;
            }
            else
            {
                SyntaxError(pos,2,"Expected a selector expression.");
                Source.MoveNextLine();
                return true;
            }

            Source.ParseToken();

            while (!Source.EOF)
            {
                switch (Source.Type)
                {
                    case TokenTypeEnum.RESERVED:
                        if (Source.Reserved == ReservedEnum.CASE)
                        {
                            ParseCase();
                            continue;
                        }
                        if (Source.Reserved == ReservedEnum.DEFAULT)
                        {
                            ParseDefault();
                            continue;
                        }
                        goto end;

                    case TokenTypeEnum.EOL:
                    case TokenTypeEnum.COMMENT:
                        Source.ParseToken();
                        break;
                    default:
                        goto end;
                }

            }
            end:
            if (Source.Reserved == ReservedEnum.END)
            {
                Source.ExpectedEnd.Pop();

                if (!Source.Match("Select"))
                {
                    SyntaxError(Source.Position,2,"Expected a End Select but found an End {0}", Source.Token);
                }
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse default.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ParseDefault()
        {
            if (!Source.Match( ':' ))
            {
                SyntaxError(Source.Position,1, "Expected a ':' after default case keyword.\r\nFor example:\r\n  Default:" );
            }

            Default = new PCase( Context, this );
            Default.Parse();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse case.  case has been processed s/b at the value: statements
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ParseCase()
        {
            Source.ParseToken();

            var caseExpr = Parser.ParseExpression();

            if (!Source.Match( ':' ))
            {
                SyntaxError(Source.Position,1, "Expected a ':' after the case expression.\r\nFor example:\r\n  Case {0}:" ,caseExpr);
            }

            if (caseExpr != null)
            {
                var caseStatement = new PCase( Context, this, caseExpr );

                if (caseStatement.Parse())
                {
                    Cases.Add( caseStatement );
                }
            }
        }

        #endregion

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
            Context.RunStack.SetCurrentLine( LineNumber, this );

            // TODO move this to a member expr
            var expr = new PExpressionBinary(Context, Selector, null, OperatorEnum.CMPE,Offset);
            foreach (var choice in Cases)
            {
                expr.Right = choice.CaseValue;
                expr.Offset = choice.CaseValue.Offset;
                expr.Length = choice.CaseValue.Length;
                if (expr.ExecuteBool())
                {
                    return choice.Execute();
                }
            }

            if (Default != null)
            {
                return Default.Execute();
            }
            return  ReturnState.NEXT;
        }

 
        #region ToString

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Members to string. 
        /// </summary>
        /// <param name="builder">      The builder. </param>
        /// <param name="showComments"> true to show, false to hide the comments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.Append( "Select " );
            if (Selector != null)
            {
                Selector.ToString(builder);
            }
            builder.AppendLine();

            for (var index = 0; index < Cases.Count; index++)
            {
                //if (index > 0)
                //{
                //    builder.AppendLine();
                //}
                var vcase = Cases[index];
                vcase.ToString(builder);
            }

            if (Default != null)
            {
                Default.ToString(builder);
            }

            builder.AppendLine("End Select");
        }

   
        #endregion

    }

 }
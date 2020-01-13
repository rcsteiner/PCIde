////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the fails block statement class
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
    /// ExpectFail Statements End ExpectFail 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PExpectFail : PStatement
    {
        /// <summary>
        /// The elements.
        /// </summary>
        public PBlock Statements;


 
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpectFail(PContext context) : base(context)
        {
            Statements   = new PBlock(context);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parses this istatement 
        /// ExpectFail statements End ExpectFail
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            var pos = Source.Position;

            Parser.ParseStatements(Statements.Statements);
            if (!Source.MatchEndStatement( "ExpectFail" ))
            {
                SyntaxError( -1, -1, "Missing 'End ExpectFail' statement here. " );

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

            Context.RunStack.ErrorExpected = true;
            Context.RunStack.ErrorFound = 0;
            Statements.Execute();
            if (Context.RunStack.ErrorFound == 0)
            {
                Context.UserOutput.Output("Failed! Expected an error");
            }
            else
            {
                Context.UserOutput.Output("Pass!   Expected error caught.");
            }
            Context.RunStack.ErrorExpected = false;
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
            builder.AppendLine("ExpectFail ");
            Statements.ToString(builder);
            builder.Indent();
            builder.AppendLine("End ExpectFail");
        }
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the case statement class
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
using System.Text;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   
    ///A single case in a switch selection. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PCase : PBlock
    {
        /// <summary>
        /// Gets or sets the case value.
        /// </summary>
        public PExpression CaseValue { get; set; }

        /// <summary>
        /// Gets or sets the statements.
        /// </summary>
        public PSwitch SwitchStatement { get; set; }

        /// <summary>
        /// Gets or sets the comment placed after the label
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        public string Comment { get; set; }
      
        /// <summary>
        /// Gets the label.
        /// </summary>
        public string Label {get { var lit = CaseValue as PLiteral;return (lit != null) ? lit.Text : CaseValue.ToString();}}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Constructor. 
        /// </summary>
        /// <param name="context">     Context for the parse. </param>
        /// <param name="switchStatement">  The switch statement. </param>
        /// <param name="caseValue">        The case value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PCase(PContext context, PSwitch switchStatement, PExpression caseValue = null)
            : base( context)
        {
            CaseValue       = caseValue;
            SwitchStatement = switchStatement;
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            // stop parsing at the next case xxx: or default or end
            LineNumber = Source.LineNumber;

            return ParseStatements();

        }
        #endregion

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
            if (CaseValue != null)
            {
                builder.Append( "Case " );
                CaseValue.ToString(builder);
            }
            else
            {
                builder.Append("Default");
            }
            builder.Append( ':' );
            builder.AppendLine();
            base.ToString(builder);
        }


        #endregion

    }
}
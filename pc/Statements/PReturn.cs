////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the statement return class
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
    /// Pseudo return. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PReturn : PStatement
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public PExpression Value { get; set; }

        /// <summary>
        /// Gets or sets the method this return is handling.
        /// </summary>
        public PFunction Method { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Constructor. 
        /// </summary>
        /// <param name="context"> context for the parse. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PReturn(PContext context) : base(context)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            base.Execute(); // sets line number

            if (Value != null)
            {
                Value.Execute();
            }
            return ReturnState.RETURN;
            // leave on stack
        }

        #region Parse

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse this language element.
        /// </summary>
        /// <returns>
        /// true if a successful parse.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            // at the expression, no check  for expression required, we 
            // will know this in the method.
            var p = Source.Position;
            Source.ParseToken();

            Value  = Parser.ParseExpression();

            if (Value == null)
            {
                Source.Position = p;
            }

            // handle ';'
            return true;
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
            builder.Append("Return");
            if (Value!=null)
            {
                builder.Append(' ');
                Value.ToString(builder);
            }
            builder.AppendLine();
        }

        #endregion

    }
}
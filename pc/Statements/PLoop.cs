// --------------------------------------------------------------------------------------------------------------------
//   Descrption:
//  
//  Defines the base class for loops
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

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   
    /// Pseudo Statement loop base class.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public abstract class PLoop : PBlock
    {
        /// <summary>
        /// Gets or sets the continue condition.
        /// </summary>
        public PExpression ContinueCondition { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">              The context. </param>
        /// <param name="continueCondition">    (Optional) The continue condition. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected PLoop(PContext context, PExpression continueCondition=null) : base(context)
        {
            ContinueCondition = continueCondition;
        }

        #region Parser

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Parses the body. 
        /// </summary>
        /// <param name="parser">   The Parser. </param>
        /// <returns>   
        /// true if body parsed ok. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool ParseBody()
        {
             Parser.ParseStatements(Statements);
             return true;
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        /// Next if continue else condition.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
           return  base.Execute(); // sets line number

            //foreach (var statement in Statements)
            //{
            //    var rs = statement.Execute();
            //    if (rs != ReturnState.NEXT)
            //    {
            //        return rs;
            //    }
            //}
            //return ReturnState.NEXT;
        }
    }
}
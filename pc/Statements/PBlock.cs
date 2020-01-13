////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the block class
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

using System;
using System.Collections.Generic;
using System.Text;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Block. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PBlock : PStatement
    {
       
        /// <summary>
        /// The elements.
        /// </summary>
        public List<PStatement> Statements;

        /// <summary>
        /// The variables.
        /// </summary>
        public PScope Scope { get; set; }

        /// <summary>
        /// Gets the size of the variable frame.
        /// </summary>
        public int VariableFrameSize { get { return Scope.Variables.Count; } }
        //{
        //    get { return _variableFrameSize >= 0 ? _variableFrameSize : _variableFrameSize = ComputeFrameSize(); }
        //    private set { _variableFrameSize = value; }
        //}

        /// <summary>
        /// Gets or sets the OuterBlock scope of this scope (parent scope)
        /// </summary>
        public PBlock OuterBlock { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PBlock(PContext context) : base(context)
        {
            OuterBlock =  context.ScopeStack.Count>0?context.ScopeStack.Peek():null;
            Scope      = new PScope();
            Statements = new List<PStatement>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Enter scope.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void EnterScope()
        {
            Context.ScopeStack.Push( this );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Leave scope.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LeaveScope()
        {
            if (Context.ScopeStack.Peek() == this)
            {
                Context.ScopeStack.Pop();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse statements.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ParseStatements()
        {
            Parser.ParseStatements( Statements );
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parses this element.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            LineNumber = Source.LineNumber;

            try
            {
                EnterScope();
                if (!ParseStatements())
                {
                    LeaveScope();
                    return false;
                }
                LeaveScope();
                if (Source.Token == "End")
                {
                    // end of program, or end block indicator
                    return true;
                }
                // must be the end of program.
                return true;
            }
            catch (Exception )
            {
                
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clears all breakpoints.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ClearAllBreakpoints()
        {
            foreach (var statement in Statements)
            {
                statement.BreakpointSet = false;
                var block = statement as PBlock;
                if (block != null)
                {
                    block.ClearAllBreakpoints();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        ///  Next if continue else condition.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            base.Execute(); // sets line number

            foreach (var statement in Statements)
            {
                var module = statement as PModule;
                if (module != null) continue;

                var returnState = statement.Execute();
                switch (returnState)
                {
                    case ReturnState.NEXT:
                        continue;

                    case ReturnState.CONTINUE:

                    case ReturnState.BREAK:
                    case ReturnState.STOP:
                    case ReturnState.RETURN:
                        return returnState;
                }
            }
            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Calculates the frame size.
        /// </summary>
        /// <returns>
        /// The calculated frame size.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int ComputeFrameSize()
        {
            int n = 0;
            foreach (var variable in Scope.Variables.Values)
            {
                n += variable.Slots;
            }
           return n;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.IncIndent();
            foreach (var statement in Statements)
            {
                builder.Indent();
                statement.ToString(builder);
            }
            builder.DecIndent();
        }
    }
}
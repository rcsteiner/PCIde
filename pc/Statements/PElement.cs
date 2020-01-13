////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the element class
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
using LiftLib.View;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Element. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public abstract class PElement
    {
        /// <summary>
        /// The context.
        /// </summary>
        public readonly PContext Context;

        /// <summary>
        /// Gets source for the.
        /// </summary>
        protected PseudoSource Source { get { return Context.Source; } }

        /// <summary>
        /// Gets the error.
        /// </summary>
        protected IUserOutput UserOutput { get { return Context.UserOutput; } }
      
        /// <summary>
        /// Gets the Parser.
        /// </summary>
        protected PseudoParser Parser { get { return Context.Parser; } }

        /// <summary>
        /// Gets or sets the offset declared
        /// </summary>
        public int Offset { get; set; }


        /// <summary>
        /// Gets or sets the length of this element.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        public int LineNumber { get; set; }
    
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected PElement(PContext context)
        {
            Context = context;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parses this element.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract bool Parse();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element. 
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract void ToString(StringBuilder builder);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// </summary>
        /// <returns>
        ///  Next if continue else condition.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual ReturnState Execute()
        {
             Context.RunStack.SetCurrentLine(LineNumber, this);
             return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Visiblility text.
        /// </summary>
        /// <param name="visibility">   The visibility. </param>
        /// <returns>
        /// the text name
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string VisiblilityText(VisibilityEnum visibility)
        {
            return visibility == VisibilityEnum.PUBLIC
                ? "Public "
                : (visibility == VisibilityEnum.PROTECTED 
                ? "Protected " 
                : "Private ");
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Warning.
        /// </summary>
        /// <param name="offset">   The offset. </param>
        /// <param name="length">   The length. </param>
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Warning(int offset, int length, string format, params object[] args)
        {
            Context.Parser.Warning(offset, length, format,args);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Syntax error.
        /// </summary>
        /// <param name="offset">   The offset. </param>
        /// <param name="length">   The length. </param>
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SyntaxError(int offset, int length, string format, params object[] args)
        {
            Context.Parser.SyntaxError(offset,length, format,args);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Lexical error.
        /// </summary>
        /// <param name="offset">   The offset. </param>
        /// <param name="length">   The length. </param>
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LexicalError(int offset, int length, string format, params object[] args)
        {
            Context.Parser.LexicalError(offset,length, format, args );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Semantic error.
        /// </summary>
        /// <param name="offset">       The offset -1 is use current </param>
        /// <param name="length">       The length. </param>
        /// <param name="lineNumber">   The line number. -1 is unknown </param>
        /// <param name="format">       Describes the format to use. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SemanticError(int offset, int length, int lineNumber, string format, params object[] args)
        {
            Context.Parser.SemanticError(offset,length,lineNumber, format, args );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Runtime error.
        /// </summary>
        /// <param name="offset">       The offset. </param>
        /// <param name="length">       The length. </param>
        /// <param name="lineNumber">   The line number. -1 is unknown. </param>
        /// <param name="format">       Describes the format to use. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// the return state
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ReturnState RuntimeError(int offset, int length, int lineNumber, string format, params object[] args)
        {
            Context.Parser.RuntimeError(offset,length, lineNumber, format, args);
            Context.RunStack.ErrorFound++;
            return  Context.RunStack.ErrorExpected?ReturnState.NEXT: ReturnState.STOP;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Runtime fatal.
        /// </summary>
        /// <param name="offset">       The offset. </param>
        /// <param name="length">       The length. </param>
        /// <param name="lineNumber">   The line number. -1 is unknown. </param>
        /// <param name="format">       Describes the format to use. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// the return state
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ReturnState RuntimeFatal(int offset, int length, int lineNumber, string format, params object[] args)
        {
            Context.Parser.RuntimeFatal( offset, length, lineNumber, format, args);
            Context.RunStack.ErrorFound++;
            return Context.RunStack.ErrorExpected ? ReturnState.NEXT : ReturnState.STOP;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.  This is shared by all elements.
        /// </summary>
        /// <returns>
        /// A string that represents the current element.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            var builder = new StringBuilder();

            ToString( builder );

            return builder.ToString();
        }


    }
}
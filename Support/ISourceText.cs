////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Declares the ISourceText interface
//
// Author:
//  Robert C. Steiner
//
// History:
//  ===================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------
//   9/12/2013   rcs     Initial implementation.
//  ===================================================================================
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   
    /// Interface for accessing text on a character basis.  Used for parsing 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface ISourceText
    {
        /// <summary>
        /// Gets the current position stored as an integer. it may be encoded into line/offset or offset.
        /// </summary>
        int Position { get; set; }
      
        /// <summary>
        /// Gets a value indicating whether at end.
        /// </summary>
        bool AtEnd { get; }

        /// <summary>
        /// Gets or sets the current char.
        /// </summary>
        char CurrentChar { get; }

        /// <summary>
        /// Gets the next char.  Does not move the position. 
        /// 0 if at end.
        /// </summary>
        char NextChar { get; }

        /// <summary>
        /// Gets the line start index.
        /// </summary>
        int LineStart { get;  }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        ZPath FilePath { get; set; }

        /// <summary>
        /// Gets current Line number. 
        /// </summary>
        int LineNumber { get; }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Move next character and sets the current character to new character. 
        /// </summary>
        /// <returns>   
        /// the next character or '\0' if is at the end of document.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        char MoveNext();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Resets the parser back to starting position.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Reset();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the current line.
        /// </summary>
        /// <returns>
        /// The current line.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        string GetCurrentLine();
    }
}
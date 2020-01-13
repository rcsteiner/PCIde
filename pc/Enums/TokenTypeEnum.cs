﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the tokentype class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/24/2015   rcs     Initial Implementation
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
namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Types of tokens.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public enum TokenTypeEnum : byte
    {
        /// <summary>
        /// The name.
        /// </summary>
        NAME,
        /// <summary>
        /// The operator.
        /// </summary>
        OPERATOR,
        /// <summary>
        /// The real.
        /// </summary>
        REAL,
        /// <summary>
        /// The integer.
        /// </summary>
        INTEGER,
        /// <summary>
        /// The string.
        /// </summary>
        STRING,
        /// <summary>
        /// A single character.
        /// </summary>
        CHAR,
        /// <summary>
        /// The punctuation
        /// </summary>
        PUNCTUATION,
        /// <summary>
        /// Reserved word
        /// </summary>
        RESERVED,
        /// <summary>
        /// A Line comment.
        /// </summary>
        COMMENT,
        /// <summary>
        /// The end of line.
        /// </summary>
        EOL,
        /// <summary>
        /// The end of file.
        /// </summary>
        EOF
    }
}
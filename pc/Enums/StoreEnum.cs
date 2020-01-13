////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the store enum class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/27/2015   rcs     Initial Implementation
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
    /// Values that represent StoreEnum.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public enum StoreEnum: byte
    {
        /// <summary>
        ///  null.
        /// </summary>
        NULL,
        /// <summary>
        /// double.
        /// </summary>
        REAL,
        /// <summary>
        /// The bool.
        /// </summary>
        BOOL,
        /// <summary>
        ///  string.
        /// </summary>
        STRING,
        /// <summary>
        ///  integer.
        /// </summary>
        INTEGER,
        /// <summary>
        ///  char.
        /// </summary>
        CHAR,
        /// <summary>
        ///  object.
        /// </summary>
        OBJECT,

        /// <summary>
        ///  list of expressions
        /// </summary>
        LIST,

        /// <summary>
        /// void return type    
        /// </summary>
        VOID,

        /// <summary>
        /// Defines a function with a predefined signature to be called back on.
        /// </summary>
        FUNCTION,
    }
}
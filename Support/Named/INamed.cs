////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Declares the INamed interface
//
// Author:
//  Robert C. Steiner
//
// History:
//  ===================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------
//   1/31/2014   rcs     Initial implementation.
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
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Provides for a name and a parent owning list for this object
    /// This allows it to be placed in a dictionary or summary
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface INamed
    {
        /// <summary>
        /// Get the name of this object
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Get the description of this object
        /// </summary>
        string Description { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Renames this object.
        /// </summary>
        /// <param name="newName">the new name.</param>
        /// <returns>true if successful</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool Rename(string newName);


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Renames this object.
        /// </summary>
        /// <param name="refObject">The ref object.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">the new name.</param>
        /// <returns>true if successful</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool Rename(object refObject, string oldName, string newName);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This instance is about to be deleted.
        /// Cleans up any associations and data.
        /// </summary>
        /// <returns>true if it is ok to delete this object</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool Remove(INamed named);
    }
}
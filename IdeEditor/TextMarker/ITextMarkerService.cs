////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Declares the ITextMarkerService interface
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/17/2015   rcs     Initial Implementation
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
using ICSharpCode.AvalonEdit.Document;
using ZCore;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Interface for marker service
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface ITextMarkerService
    {
        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        TextDocument Document { get; set; }
   
        /// <summary>
        ///     Gets the list of text markers.
        /// </summary>
        TextSegmentCollection<TextMarker> TextMarkers { get; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Creates a new text marker. The text marker will be invisible at first, you need to set one of the Color
        /// properties to make it visible.
        /// </summary>
        /// <param name="startOffset">  The start offset. </param>
        /// <param name="length">       The length. </param>
        /// <param name="error">        (optional) the error. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ITextMarker Create(int startOffset, int length, Error error=null);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Finds all text markers at the specified offset.
        /// </summary>
        /// <param name="offset">   The offset. </param>
        /// <returns>
        ///     The markers at offset.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        IEnumerable<ITextMarker> GetMarkersAtOffset(int offset);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Removes the specified text marker.
        /// </summary>
        /// <param name="marker">   The ITextMarker to remove. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Remove(ITextMarker marker);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Removes all text markers that match the condition.
        /// </summary>
        /// <param name="predicate">    The predicate. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void RemoveAll(Predicate<ITextMarker> predicate);
    }
}
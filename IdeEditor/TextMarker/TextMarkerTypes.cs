////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the text marker types class
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

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Bitfield of flags for specifying TextMarkerTypes.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Flags]
    public enum TextMarkerTypes
    {
        /// <summary>
        ///     Use no marker
        /// </summary>
        None = 0x0000,

        /// <summary>
        ///     Use squiggly underline marker
        /// </summary>
        SquigglyUnderline = 0x001,

        /// <summary>
        ///     Normal underline.
        /// </summary>
        NormalUnderline = 0x002,

        /// <summary>
        ///     Dotted underline.
        /// </summary>
        DottedUnderline = 0x004,

        /// <summary>
        ///     Horizontal line in the scroll bar.
        /// </summary>
        LineInScrollBar = 0x0100,

        /// <summary>
        ///     Small triangle in the scroll bar, pointing to the right.
        /// </summary>
        ScrollBarRightTriangle = 0x0400,

        /// <summary>
        ///     Small triangle in the scroll bar, pointing to the left.
        /// </summary>
        ScrollBarLeftTriangle = 0x0800,

        /// <summary>
        ///     Small circle in the scroll bar.
        /// </summary>
        CircleInScrollBar = 0x1000
    }
}
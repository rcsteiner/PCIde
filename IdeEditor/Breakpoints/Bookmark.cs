////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the bookmark class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/25/2015   rcs     Initial Implementation
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

using System.Windows.Input;
using System.Windows.Media;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Bookmark.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Bookmark : BookmarkBase, IBookmark
    {
        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int ZOrder { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public ImageSource Image { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we can drag drop.
        /// </summary>
        public bool CanDragDrop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the displays tooltip.
        /// </summary>
        public bool DisplaysTooltip { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Bookmark"/> is enabled.
        /// </summary>
        public bool Enabled { get; set; }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lineNumber">   The line number. </param>
        /// <param name="image">        The image. </param>
        /// <param name="text">         The text. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Bookmark(int lineNumber, ImageSource image, string text)
        {
            LineNumber = lineNumber;
            Image = image;
            Text = text;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Creates the tooltip content.
        /// </summary>
        /// <returns>
        /// the content.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public object CreateTooltipContent()
        {
            return Text;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Drops.
        /// </summary>
        /// <param name="line"> The line. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Drop(int line)
        {
            LineNumber = line;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Mouse down.
        /// </summary>
        /// <param name="mouseButtonEventArgs"> Mouse button event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MouseDown(MouseButtonEventArgs mouseButtonEventArgs)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Mouse up.
        /// </summary>
        /// <param name="mouseButtonEventArgs"> Mouse button event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MouseUp(MouseButtonEventArgs mouseButtonEventArgs)
        {
        }
    }
}
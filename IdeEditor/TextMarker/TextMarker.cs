////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the text marker class
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
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ZCore;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Text marker.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public sealed class TextMarker : TextSegment, ITextMarker
    {
        /// <summary>
        /// The service.
        /// </summary>
        private readonly TextMarkerService service;

        /// <summary>
        /// The background color.
        /// </summary>
        private Color? backgroundColor;

        /// <summary>
        /// The font style.
        /// </summary>
        private FontStyle? fontStyle;

        /// <summary>
        /// The font weight.
        /// </summary>
        private FontWeight? fontWeight;

        /// <summary>
        /// The foreground color.
        /// </summary>
        private Color? foregroundColor;

        /// <summary>
        /// The marker color.
        /// </summary>
        private Color markerColor;

        /// <summary>
        /// List of types of the markers.
        /// </summary>
        private TextMarkerTypes markerTypes;

        /// <summary>
        /// Event that occurs when the text marker is deleted.
        /// </summary>
        public event EventHandler Deleted;

        /// <summary>
        /// Gets or sets/Sets an object with additional data for this text marker.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets whether the text marker was deleted.
        /// </summary>
        public bool IsDeleted
        {
            get { return !IsConnectedToCollection; }
        }

        /// <summary>
        /// Gets or sets/Sets an object that will be displayed as tooltip in the text editor.
        /// </summary>
        public object ToolTip { get; set; }


        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        public Error Error { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="service">      The service. </param>
        /// <param name="startOffset">  The start offset. </param>
        /// <param name="length">       The length. </param>
        /// <param name="error">        (Optional) The error. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TextMarker(TextMarkerService service, int startOffset, int length, Error error=null)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            this.service = service;
            StartOffset  = startOffset;
            Length       = length;
            markerTypes  = TextMarkerTypes.None;
            Error        = error;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Deletes the text marker.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Delete()
        {
            service.Remove(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets/Sets the background color.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Color? BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    Redraw();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets/Sets the foreground color.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Color? ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                if (foregroundColor != value)
                {
                    foregroundColor = value;
                    Redraw();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets/Sets the font weight.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FontWeight? FontWeight
        {
            get { return fontWeight; }
            set
            {
                if (fontWeight != value)
                {
                    fontWeight = value;
                    Redraw();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets/Sets the font style.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FontStyle? FontStyle
        {
            get { return fontStyle; }
            set
            {
                if (fontStyle != value)
                {
                    fontStyle = value;
                    Redraw();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets/Sets the type of the marker. Use TextMarkerType.None for normal markers.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TextMarkerTypes MarkerTypes
        {
            get { return markerTypes; }
            set
            {
                if (markerTypes != value)
                {
                    markerTypes = value;
                    Redraw();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets/Sets the color of the marker.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Color MarkerColor
        {
            get { return markerColor; }
            set
            {
                if (markerColor != value)
                {
                    markerColor = value;
                    Redraw();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the deleted action.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void OnDeleted()
        {
            if (Deleted != null)
            {
                Deleted(this, EventArgs.Empty);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Redraws this object.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Redraw()
        {
         //   service.Redraw(this);
        }
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the highlight curent line renderer class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/27/2015   rcs     Initial Implementation
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
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace Views.Editors
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Highlight curent line renderer. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class HighlightCurentLineRenderer : IBackgroundRenderer
    {
        private Brush _background;
        private Pen _border;
   
        /// <summary>
        /// The editor.
        /// </summary>
        IdeEditor _editor;

        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets the layer on which this background renderer should draw.
        /// </summary>
        public KnownLayer Layer {get { return KnownLayer.Selection; }}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="editor">       The editor. </param>
        /// <param name="background">   The background. </param>
        /// <param name="border">       The border. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public HighlightCurentLineRenderer(IdeEditor editor, Color background, Color border)
        {
            _background = new SolidColorBrush(background);
            _border     = new Pen(new SolidColorBrush(border),1 );
            _editor     = editor;
            _editor.TextArea.TextView.BackgroundRenderers.Add(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Causes the background renderer to draw.
        /// </summary>
        /// <param name="textView">         The text view. </param>
        /// <param name="drawingContext">   Context for the drawing. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (IsVisible)
            {
                textView.EnsureVisualLines();
                var line = _editor.Document.GetLineByOffset(_editor.CaretOffset);
                var segment = new TextSegment {StartOffset = line.Offset, EndOffset = line.EndOffset};
                foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
                {
                    drawingContext.DrawRoundedRectangle(_background, _border,
                        new Rect(r.Location, new Size(textView.ActualWidth, r.Height)), 3, 3);
                }
            }
        }
    }
}
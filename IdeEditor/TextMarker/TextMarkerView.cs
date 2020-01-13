////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the text marker view class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/22/2015   rcs     Initial Implementation
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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ZCore;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Handles the text _textMarkerService.TextMarkers UI
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public sealed class TextMarkerView : DocumentColorizingTransformer, IBackgroundRenderer, ITextViewConnect
    {
        private const TextMarkerTypes underlineMarkerTypes = TextMarkerTypes.SquigglyUnderline | TextMarkerTypes.NormalUnderline | TextMarkerTypes.DottedUnderline;
        private ITextMarkerService    _textMarkerService;
        private readonly IdeEditor    _editor;
        private readonly ToolTip     toolTip;
        private readonly StackPanel toolStack ;
        private readonly TextBlock errorTitle ;
        private readonly TextBlock errorMsg   ;


        /// <summary>
        ///     Gets the layer on which this background renderer should draw.  draw behind selection
        /// </summary>
        public KnownLayer Layer {get { return KnownLayer.Selection; }}


     
        /// <summary>
        ///     Event queue for all listeners interested in RedrawRequested events.
        /// </summary>
        public event EventHandler RedrawRequested;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="editor">   The editor. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TextMarkerView(IdeEditor editor)
        {
            toolTip    = new ToolTip();
            toolStack  = new StackPanel();
            errorTitle = new TextBlock();
            errorMsg   = new TextBlock();
        
            toolTip.Content = toolStack;
            toolStack.Children.Add(errorTitle);
            toolStack.Children.Add(errorMsg);

            toolTip.Background = Brushes.Yellow;
            toolTip.Foreground = Brushes.Black;
           
            errorTitle.FontWeight          = FontWeights.Bold;
            errorTitle.FontSize            = 18;
            errorTitle.HorizontalAlignment = HorizontalAlignment.Center;
            errorMsg.TextWrapping          = TextWrapping.Wrap;
            errorMsg.MaxWidth              = 350;
            errorMsg.HorizontalAlignment   = HorizontalAlignment.Left;
            errorMsg.FontSize              = 16;

            _editor                           = editor;
            _editor.MouseHover               += TextEditorMouseHover;
            _editor.PreviewMouseHoverStopped += TextEditorMouseHoverStopped;
            _textMarkerService                = _editor.TextMarkerService;

            _editor.TextArea.TextView.BackgroundRenderers.Add( this );
            _editor.TextArea.TextView.LineTransformers.Add( this );


        }

        #region Tooltip support for errors...

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Text editor mouse hover.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Mouse event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TextEditorMouseHover(object sender, MouseEventArgs e)
        {
            var pos = _editor.GetPositionFromPoint(e.GetPosition(_editor));
            if (pos != null)
            {
                var tp = (TextViewPosition) pos;
                if (_textMarkerService == null || _textMarkerService.TextMarkers == null)
                {
                    return;
                }
                var m = _textMarkerService.GetMarkersAtOffset(_editor.doc.GetOffset(tp.Location));

                foreach (var marker in m)
                {
                    var desc =(string) marker.ToolTip;
                    var title = desc.FirstElement( ':');
                    if (title.Length > 0)
                    {
                        desc = desc.Substring(title.Length + 1);
                    }
                    else
                    {
                        title = (string) marker.Tag;
                    }
                    toolTip.Background = MapBackground( marker );
                    toolTip.Foreground = MapForeground(marker);

                    toolTip.PlacementTarget = _editor; // required for property inheritance
                    errorTitle.Text         = title;
                    errorMsg.Text           = desc; // pos.ToString();
                    e.Handled               = true;
                    toolTip.IsOpen          = true;
                    break;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Map background.
        /// </summary>
        /// <param name="marker">   The marker. </param>
        /// <returns>
        /// the color
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Brush MapBackground(ITextMarker marker)
        {
            var c = toolTip.Background;
            if (marker.Error != null)
            {
                switch (marker.Error.Level)
                {
                    case ErrorLevel.OUTPUT:
                    case ErrorLevel.TRACE:
                    case ErrorLevel.INFO:
                        c = Brushes.Green;

                        break;
                    case ErrorLevel.WARNING:
                        c = Brushes.Yellow;
                        break;
                    case ErrorLevel.ERROR:
                        c = Brushes.Red;
                        break;

                    case ErrorLevel.FATAL:
                    case ErrorLevel.EXCEPTION:
                        c = Brushes.DarkMagenta;
                        break;
                }
            }
            return c;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Map foreground.
        /// </summary>
        /// <param name="marker">   The marker. </param>
        /// <returns>
        /// the color
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Brush MapForeground(ITextMarker marker)
        {
            var c = toolTip.Foreground;
            if (marker.Error != null)
            {
                switch (marker.Error.Level)
                {
                    case ErrorLevel.OUTPUT:
                    case ErrorLevel.TRACE:
                    case ErrorLevel.INFO:
                    case ErrorLevel.ERROR:
                    case ErrorLevel.FATAL:
                    case ErrorLevel.EXCEPTION:
                        c = Brushes.White;
                        break;
                    case ErrorLevel.WARNING:
                        c = Brushes.Black;
                        break;
                }
            }
            return c;
        }  

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Text editor mouse hover stopped.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Mouse event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TextEditorMouseHoverStopped(object sender, MouseEventArgs e)
        {
            toolTip.IsOpen = false;
        }

        #endregion

        #region DocumentColorizingTransformer

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Override this method to colorize an individual document line.
        /// </summary>
        /// <param name="line"> The line. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void ColorizeLine(DocumentLine line)
        {
            if (_textMarkerService == null)
            {
                return;
            }
            var textMarkers = _textMarkerService.TextMarkers;
            if (textMarkers == null)
            {
                return;
            }
            var lineStart = line.Offset;
            var lineEnd = lineStart + line.Length;
            foreach (var marker in textMarkers.FindOverlappingSegments(lineStart, line.Length))
            {
                Brush foregroundBrush = null;
                if (marker.ForegroundColor != null)
                {
                    foregroundBrush = new SolidColorBrush(marker.ForegroundColor.Value);
                    foregroundBrush.Freeze();
                }
                ChangeLinePart(Math.Max(marker.StartOffset, lineStart), Math.Min(marker.EndOffset, lineEnd), element =>
                {
                    if (foregroundBrush != null)
                    {
                        element.TextRunProperties.SetForegroundBrush(foregroundBrush);
                    }
                    var tf = element.TextRunProperties.Typeface;
                    element.TextRunProperties.SetTypeface(new Typeface(tf.FontFamily, marker.FontStyle ?? tf.Style, marker.FontWeight ?? tf.Weight, tf.Stretch));
                });
            }
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Redraws the specified text segment.
        /// </summary>
        /// <param name="segment">  The segment. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void Redraw(ISegment segment)
        {
            foreach (var view in textViews)
            {
                view.Redraw(segment, DispatcherPriority.Normal);
            }
            if (RedrawRequested != null)
            {
                RedrawRequested(this, EventArgs.Empty);
            }
        }

        #region IBackgroundRenderer

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Causes the background renderer to draw.
        ///     Uses the _textMarkerService.TextMarkers
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="textView">         The text view. </param>
        /// <param name="drawingContext">   Context for the drawing. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }
            if (drawingContext == null)
            {
                throw new ArgumentNullException("drawingContext");
            }
            if (_textMarkerService == null || (_textMarkerService.TextMarkers == null || !textView.VisualLinesValid))
            {
                return;
            }

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
            {
                return;
            }
            var viewStart = visualLines.First().FirstDocumentLine.Offset;
            var viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
            foreach (var marker in _textMarkerService.TextMarkers.FindOverlappingSegments(viewStart, viewEnd - viewStart))
            {
                // draw rounded radius background if different color
                if (marker.BackgroundColor != null)
                {
                    var geoBuilder = new BackgroundGeometryBuilder();
                    geoBuilder.AlignToWholePixels = true;
                    geoBuilder.CornerRadius = 3;
                    geoBuilder.AddSegment(textView, marker);
                    var geometry = geoBuilder.CreateGeometry();
                    if (geometry != null)
                    {
                        var color = marker.BackgroundColor.Value;
                        var brush = new SolidColorBrush(color);
                        brush.Freeze();
                        drawingContext.DrawGeometry(brush, null, geometry);
                    }
                }

                // draw underline type _textMarkerService.TextMarkers
                if ((marker.MarkerTypes & underlineMarkerTypes) != 0)
                {
                    foreach (var r in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker))
                    {
                        var startPoint = r.BottomLeft;
                        var endPoint = r.BottomRight;

                        Brush usedBrush = new SolidColorBrush(marker.MarkerColor);
                        usedBrush.Freeze();

                        // squiggly line - create geometry for length needed
                        if ((marker.MarkerTypes & TextMarkerTypes.SquigglyUnderline) != 0)
                        {
                            var offset = 2.5;

                            var count = Math.Max((int) ((endPoint.X - startPoint.X)/offset) + 1, 4);

                            var geometry = new StreamGeometry();

                            using (var ctx = geometry.Open())
                            {
                                ctx.BeginFigure(startPoint, false, false);
                                ctx.PolyLineTo(CreatePoints(startPoint, endPoint, offset, count).ToArray(), true, false);
                            }

                            geometry.Freeze();

                            var usedPen = new Pen(usedBrush, 2);
                            usedPen.Freeze();
                            drawingContext.DrawGeometry(Brushes.Transparent, usedPen, geometry);
                        }
                        // underline
                        if ((marker.MarkerTypes & TextMarkerTypes.NormalUnderline) != 0)
                        {
                            var usedPen = new Pen(usedBrush, 2);
                            usedPen.Freeze();
                            drawingContext.DrawLine(usedPen, startPoint, endPoint);
                        }
                        // dotted-underline
                        if ((marker.MarkerTypes & TextMarkerTypes.DottedUnderline) != 0)
                        {
                            var usedPen = new Pen(usedBrush, 2);
                            usedPen.DashStyle = DashStyles.Dot;
                            usedPen.Freeze();
                            drawingContext.DrawLine(usedPen, startPoint, endPoint);
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Creates the points.
        /// </summary>
        /// <param name="start">    The start. </param>
        /// <param name="end">      The end. </param>
        /// <param name="offset">   The offset. </param>
        /// <param name="count">    Number of. </param>
        /// <returns>
        ///     .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private IEnumerable<Point> CreatePoints(Point start, Point end, double offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Point(start.X + i*offset, start.Y - ((i + 1)%2 == 0 ? offset : 0));
            }
        }

        #endregion

        #region ITextViewConnect

        /// <summary>
        ///     The text views.
        /// </summary>
        private readonly List<TextView> textViews = new List<TextView>();


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Called when added to a text view.
        /// </summary>
        /// <param name="textView"> The text view. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void ITextViewConnect.AddToTextView(TextView textView)
        {
            if (textView != null && _textMarkerService != null && !textViews.Contains(textView))
            {
                textViews.Add(textView);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Called when removed from a text view.
        /// </summary>
        /// <param name="textView"> The text view. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void ITextViewConnect.RemoveFromTextView(TextView textView)
        {
            if (textView != null)
            {
                textViews.Remove(textView);
            }
        }

        #endregion
    }
}
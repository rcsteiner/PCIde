// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

// extensively modified by Robert Steiner.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Icon bar: contains breakpoints and other icons.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class IconBarMargin : AbstractMargin, IDisposable
    {
        /// <summary>
        /// The hover logic.
        /// </summary>
        private readonly MouseHoverLogic hoverLogic;

        /// <summary>
        /// The manager.
        /// </summary>
        private readonly IBookmarkMargin manager;

        /// <summary>
        /// bookmark being dragged (!=null if drag'n'drop is active)
        /// </summary>
        private IBookmark dragDropBookmark;

        /// <summary>
        /// The drag drop current point.
        /// </summary>
        private double dragDropCurrentPoint;

        /// <summary>
        /// The drag drop start point.
        /// </summary>
        private double dragDropStartPoint;

        /// <summary>
        /// whether drag'n'drop operation has started (mouse was moved minimum distance)
        /// </summary>
        private bool dragStarted;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="manager">  The manager. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IconBarMargin(IBookmarkMargin manager)
        {
            if (manager == null)
            {
                return;
            }
            this.manager                 = manager;
            hoverLogic                   = new MouseHoverLogic(this);
            hoverLogic.MouseHover        += (sender, e) => MouseHover(this, e);
            hoverLogic.MouseHoverStopped += (sender, e) => MouseHoverStopped(this, e);
            Unloaded                     += OnUnloaded;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Cancel drag drop.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void CancelDragDrop()
        {
            if (dragDropBookmark != null)
            {
                dragDropBookmark = null;
                dragStarted = false;
                if (TextView != null)
                {
                    var area = TextView.GetService(typeof (TextArea)) as TextArea;
                    if (area != null)
                    {
                        area.PreviewKeyDown -= TextArea_PreviewKeyDown;
                    }
                }
                ReleaseMouseCapture();
                InvalidateVisual();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a bookmark from line.
        /// </summary>
        /// <param name="line"> The line. </param>
        /// <returns>
        /// The bookmark from line.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private IBookmark GetBookmarkFromLine(int line)
        {
            IBookmark result = null;
            foreach (var bm in manager.Bookmarks)
            {
                if (bm.LineNumber == line)
                {
                    if (result == null || bm.ZOrder > result.ZOrder)
                    {
                        result = bm;
                    }
                }
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a line from mouse position.
        /// </summary>
        /// <param name="e">    Mouse event information. </param>
        /// <returns>
        /// The line from mouse position.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int GetLineFromMousePosition(MouseEventArgs e)
        {
            var textView = TextView;
            if (textView == null)
            {
                return 0;
            }
            var vl = textView.GetVisualLineFromVisualTop(e.GetPosition(textView).Y + textView.ScrollOffset.Y);
            if (vl == null)
            {
                return 0;
            }
            return vl.FirstDocumentLine.LineNumber;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Implements <see cref="M:System.Windows.Media.Visual.HitTestCore(System.Windows.Media.PointHitTestParameters)" />
        /// to supply base element hit testing behavior (returning <see cref="T:System.Windows.Media.HitTestResult" />).
        /// </summary>
        /// <param name="hitTestParameters">    Describes the hit test to perform, including the initial hit point. </param>
        /// <returns>
        /// Results of the test, including the evaluated point.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            // accept clicks even when clicking on the background
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size
        /// for the <see cref="T:System.Windows.FrameworkElement" />-derived class.
        /// </summary>
        /// <param name="availableSize">    The available size that this element can give to child elements. Infinity can be
        ///                                 specified as a value to indicate that the element will size to whatever content is
        ///                                 available. </param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(18, 0);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.LostMouseCapture" /> attached event reaches an
        /// element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">    The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains event data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            CancelDragDrop();
            base.OnLostMouseCapture(e);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown" /> attached event reaches an element
        /// in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">    The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data.
        ///                     This event data reports details about the mouse button that was pressed and the handled state. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            CancelDragDrop();
            base.OnMouseDown(e);
            var line = GetLineFromMousePosition(e);
            if (!e.Handled && line > 0)
            {
                var bm = GetBookmarkFromLine(line);
                if (bm != null)
                {
                    bm.MouseDown(e);
                    if (!e.Handled)
                    {
                        if (e.ChangedButton == MouseButton.Left && bm.CanDragDrop && CaptureMouse())
                        {
                            StartDragDrop(bm, e);
                            e.Handled = true;
                        }
                    }
                }
            }
            // don't allow selecting text through the IconBarMargin
            if (e.ChangedButton == MouseButton.Left)
            {
                e.Handled = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove" /> attached event reaches an element
        /// in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">    The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (dragDropBookmark != null)
            {
                dragDropCurrentPoint = e.GetPosition(this).Y;
                if (Math.Abs(dragDropCurrentPoint - dragDropStartPoint) > SystemParameters.MinimumVerticalDragDistance)
                {
                    dragStarted = true;
                }
                InvalidateVisual();
            }
            TextViewMouseMove(TextView, e);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp" /> routed event reaches an element in
        /// its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">    The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data.
        ///                     The event data reports that the mouse button was released. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            var line = GetLineFromMousePosition(e);
            if (!e.Handled && dragDropBookmark != null)
            {
                if (dragStarted)
                {
                    if (line != 0)
                    {
                        dragDropBookmark.Drop(line);
                    }
                    e.Handled = true;
                }
                CancelDragDrop();
            }
            if (!e.Handled && line != 0)
            {
                var bm = GetBookmarkFromLine(line);
                if (bm != null)
                {
                    bm.MouseUp(e);
                    if (e.Handled)
                    {
                        return;
                    }
                }
                if (e.ChangedButton == MouseButton.Left && TextView != null)
                {
                    // no bookmark on the line: create a new breakpoint
                    var textEditor = TextView.GetService(typeof (IdeEditor)) as IdeEditor;
                    if (textEditor != null)
                    {
                        //TODO: toggle breakpoint
                        //  SD.Debugger.ToggleBreakpointAt( textEditor, line );
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system.
        /// The rendering instructions for this element are not used directly when this method is invoked, and are instead
        /// preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">   The drawing instructions for a specific element. This context is provided to the
        ///                                 layout system. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnRender(DrawingContext drawingContext)
        {
            var renderSize = RenderSize;
            drawingContext.DrawRectangle(SystemColors.ControlBrush, null,
                new Rect(0, 0, renderSize.Width, renderSize.Height));
            drawingContext.DrawLine(new Pen(SystemColors.ControlDarkBrush, 1),
                new Point(renderSize.Width - 0.5, 0),
                new Point(renderSize.Width - 0.5, renderSize.Height));

            var textView = TextView;
            if (textView != null && textView.VisualLinesValid)
            {
                // create a dictionary line number => first bookmark
                var bookmarkDict = new Dictionary<int, IBookmark>();
                foreach (var bm in manager.Bookmarks)
                {
                    var line = bm.LineNumber;
                    IBookmark existingBookmark;
                    if (!bookmarkDict.TryGetValue(line, out existingBookmark) || bm.ZOrder > existingBookmark.ZOrder)
                    {
                        bookmarkDict[line] = bm;
                    }
                }
                var pixelSize = PixelSnapHelpers.GetPixelSize(this);
                foreach (var line in textView.VisualLines)
                {
                    var lineNumber = line.FirstDocumentLine.LineNumber;
                    IBookmark bm;
                    if (bookmarkDict.TryGetValue(lineNumber, out bm))
                    {
                        var lineMiddle =
                            line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.TextMiddle) -
                            textView.VerticalOffset;
                        var rect = new Rect(0, PixelSnapHelpers.Round(lineMiddle - 8, pixelSize.Height), 16, 16);
                        if (dragDropBookmark == bm && dragStarted)
                        {
                            drawingContext.PushOpacity(0.5);
                        }
                        drawingContext.DrawImage(bm.Image ?? BookmarkBase.DefaultBookmarkImage, rect);
                        if (dragDropBookmark == bm && dragStarted)
                        {
                            drawingContext.Pop();
                        }
                    }
                }
                if (dragDropBookmark != null && dragStarted)
                {
                    var rect = new Rect(0, PixelSnapHelpers.Round(dragDropCurrentPoint - 8, pixelSize.Height), 16, 16);
                    drawingContext.DrawImage(dragDropBookmark.Image ?? BookmarkBase.DefaultBookmarkImage, rect);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Starts a drag drop.
        /// </summary>
        /// <param name="bm">   The bm. </param>
        /// <param name="e">    Mouse event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void StartDragDrop(IBookmark bm, MouseEventArgs e)
        {
            dragDropBookmark = bm;
            dragDropStartPoint = dragDropCurrentPoint = e.GetPosition(this).Y;
            if (TextView != null)
            {
                var area = TextView.GetService(typeof (TextArea)) as TextArea;
                if (area != null)
                {
                    area.PreviewKeyDown += TextArea_PreviewKeyDown;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by TextArea for preview key down events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Key event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TextArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // any key press cancels drag'n'drop
            CancelDragDrop();
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
            }
        }

        #region OnTextViewChanged

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <inheritdoc/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
        {
            if (oldTextView != null)
            {
                oldTextView.VisualLinesChanged -= OnRedrawRequested;
                oldTextView.MouseMove          -= TextViewMouseMove;
                manager.RedrawRequested        -= OnRedrawRequested;
            }
            base.OnTextViewChanged(oldTextView, newTextView);
            if (newTextView != null)
            {
                newTextView.VisualLinesChanged += OnRedrawRequested;
                newTextView.MouseMove          += TextViewMouseMove;
                manager.RedrawRequested        += OnRedrawRequested;
            }
            InvalidateVisual();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the redraw requested action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnRedrawRequested(object sender, EventArgs e)
        {
            // Don't invalidate the IconBarMargin if it'll be invalidated again once the
            // visual lines become valid.
            if (TextView != null && TextView.VisualLinesValid)
            {
                InvalidateVisual();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Dispose()
        {
            TextView = null; // detach from TextView (will also detach from manager)
        }

        #endregion

        #region Tooltip

        /// <summary>
        /// The tool tip control.
        /// </summary>
        private ToolTip toolTip;

        /// <summary>
        /// The popup tool tip.
        /// </summary>
        private Popup popupToolTip;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Mouse hover.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Mouse event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MouseHover(object sender, MouseEventArgs e)
        {
            Debug.Assert(sender == this);

            if (!TryCloseExistingPopup(false))
            {
                return;
            }

            var line = GetLineFromMousePosition(e);
            if (line < 1)
            {
                return;
            }
            var bm = manager.Bookmarks
                .Where(m => m.LineNumber == line && m.DisplaysTooltip)
                .OrderBy(m => m.ZOrder)
                .FirstOrDefault();
            if (bm == null)
            {
                return;
            }
            var content = bm.CreateTooltipContent();
            popupToolTip = content as Popup;

            if (popupToolTip != null)
            {
                var popupPosition = GetPopupPosition(line);
                popupToolTip.Closed += ToolTipClosed;
                popupToolTip.HorizontalOffset = popupPosition.X;
                popupToolTip.VerticalOffset = popupPosition.Y;
                popupToolTip.StaysOpen = true; // We will close it ourselves

                e.Handled = true;
                popupToolTip.IsOpen = true;
                distanceToPopupLimit = double.PositiveInfinity;
                    // reset limit; we'll re-calculate it on the next mouse movement
            }
            else if (content != null)
            {
                if (toolTip == null)
                {
                    toolTip = new ToolTip();
                    toolTip.Closed += ToolTipClosed;
                }
                toolTip.PlacementTarget = this; // required for property inheritance

                if (content is string)
                {
                    toolTip.Content = new TextBlock
                    {
                        Text = content as string,
                        TextWrapping = TextWrapping.Wrap
                    };
                }
                else
                {
                    toolTip.Content = content;
                }

                e.Handled = true;
                toolTip.IsOpen = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Try close existing popup.
        /// </summary>
        /// <param name="mouseClick">   true to mouse click. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool TryCloseExistingPopup(bool mouseClick)
        {
            if (popupToolTip != null)
            {
                if (popupToolTip.IsOpen && !mouseClick && popupToolTip is ITooltip &&
                    !((ITooltip) popupToolTip).CloseWhenMouseMovesAway)
                {
                    return false; // Popup does not want to be closed yet
                }
                popupToolTip.IsOpen = false;
                popupToolTip = null;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a popup position.
        /// </summary>
        /// <param name="line"> The line. </param>
        /// <returns>
        /// The popup position.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Point GetPopupPosition(int line)
        {
            var positionInPixels =
                TextView.PointToScreen(
                    TextView.GetVisualPosition(new TextViewPosition(line, 1), VisualYPosition.LineBottom) -
                    TextView.ScrollOffset);
            positionInPixels.X -= 50;
            // use device independent units, because Popup Left/Top are in independent units
            return positionInPixels; // positionInPixels.TransformFromDevice( this );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Mouse hover stopped.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Mouse event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MouseHoverStopped(object sender, MouseEventArgs e)
        {
            // Non-popup tooltips get closed as soon as the mouse starts moving again
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
                e.Handled = true;
            }
        }

        /// <summary>
        /// The distance to popup limit.
        /// </summary>
        private double distanceToPopupLimit;

        /// <summary>
        /// The maximum movement away from popup.
        /// </summary>
        private const double MaxMovementAwayFromPopup = 5;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a distance to popup.
        /// </summary>
        /// <param name="e">    Mouse event information. </param>
        /// <returns>
        /// The distance to popup.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private double GetDistanceToPopup(MouseEventArgs e)
        {
            var p = popupToolTip.Child.PointFromScreen(PointToScreen(e.GetPosition(this)));
            var size = popupToolTip.Child.RenderSize;
            double x = 0;
            if (p.X < 0)
            {
                x = -p.X;
            }
            else if (p.X > size.Width)
            {
                x = p.X - size.Width;
            }
            double y = 0;
            if (p.Y < 0)
            {
                y = -p.Y;
            }
            else if (p.Y > size.Height)
            {
                y = p.Y - size.Height;
            }
            return Math.Sqrt(x*x + y*y);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseLeave" /> attached event is raised on this
        /// element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">    The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (popupToolTip != null && !popupToolTip.IsMouseOver && GetDistanceToPopup(e) > 10)
            {
                // do not close popup if mouse moved from editor to popup
                TryCloseExistingPopup(false);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Text view mouse move.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Mouse event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TextViewMouseMove(object sender, MouseEventArgs e)
        {
            if (popupToolTip != null)
            {
                var distanceToPopup = GetDistanceToPopup(e);
                if (distanceToPopup > distanceToPopupLimit)
                {
                    // Close popup if mouse moved away, exceeding the limit
                    TryCloseExistingPopup(false);
                }
                else
                {
                    // reduce distanceToPopupLimit
                    distanceToPopupLimit = Math.Min(distanceToPopupLimit, distanceToPopup + MaxMovementAwayFromPopup);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the unloaded action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnUnloaded(object sender, EventArgs e)
        {
            // Close popup when another document gets selected
            // TextEditorMouseLeave is not sufficient for this because the mouse might be over the popup when the document switch happens (e.g. Ctrl+Tab)
            TryCloseExistingPopup(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Tool tip closed.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ToolTipClosed(object sender, EventArgs e)
        {
            if (toolTip == sender)
            {
                toolTip = null;
            }
            if (popupToolTip == sender)
            {
                // Because popupToolTip instances are created by the tooltip provider,
                // they might be reused; so we should detach the event handler
                popupToolTip.Closed -= ToolTipClosed;
                popupToolTip = null;
            }
        }

        #endregion
    }
}
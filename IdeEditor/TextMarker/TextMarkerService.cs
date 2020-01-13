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

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ZCore;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Text mark service. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public sealed class TextMarkerService : ITextMarkerService
    {
        private TextDocument _document;
        private TextSegmentCollection<TextMarker> markers;

        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        public TextDocument Document {get { return _document; }set { _document = value; }}

        /// <summary>
        /// Gets the list of text markers.
        /// </summary>
        public TextSegmentCollection<TextMarker> TextMarkers {get { return markers; }}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="document"> The document. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TextMarkerService(TextDocument document)
        {
            _document = document;
            markers = new TextSegmentCollection<TextMarker>(document);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Creates a new text marker. The text marker will be invisible at first, you need to set one of the Color
        /// properties to make it visible.
        /// </summary>
        /// <param name="startOffset">  The start offset. </param>
        /// <param name="length">       The length. </param>
        /// <param name="error">        (optional) the error. </param>
        /// <returns>
        /// the marker
        /// </returns>
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ITextMarker Create(int startOffset, int length, Error error=null)
        {
            var textLength = _document.TextLength;
       
            if (textLength < 1) return null;
            if (startOffset < 0) startOffset = 0;
            if (startOffset > textLength - 1)
            {
                startOffset = textLength - 1;
            }

            if (length < 1) length = 1;

            if (startOffset + length >= textLength)
            {
                length = textLength - startOffset;
            }

            var old = GetMarkersAtOffset( startOffset );
            if (old.Any())
            {
                return null;
            }

            var m = new TextMarker(this, startOffset, length ,error);
            markers.Add( m );
            // no need to mark segment for redraw: the text marker is invisible until a property is set
            return m;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Finds all text markers at the specified offset.
        /// </summary>
        /// <param name="offset">   The offset. </param>
        /// <returns>
        /// The markers at offset.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEnumerable<ITextMarker> GetMarkersAtOffset(int offset)
        {
            return markers.FindSegmentsContaining( offset );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes all text markers that match the condition.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="predicate">    The predicate. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveAll(Predicate<ITextMarker> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException( "predicate" );
            }
            if (markers != null)
            {
                foreach (var m in markers.ToArray())
                {
                    if (predicate( m ))
                    {
                        Remove( m );
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes the specified text marker.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="marker">   The ITextMarker to remove. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Remove(ITextMarker marker)
        {
            if (marker == null)
            {
                throw new ArgumentNullException( "marker" );
            }
            var m = marker as TextMarker;
            if (markers != null && markers.Remove( m ))
            {
               // Redraw( m );
                m.OnDeleted();
            }
        }

    }
 
}
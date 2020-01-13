////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the bookmark margin class
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ZCore;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Stores the entries in the icon bar margin. Multiple icon bar margins can use the same manager if split view is
    ///     used.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class IconBarManager : IBookmarkMargin
    {
        /// <summary>
        ///     The bookmarks.
        /// </summary>
        private readonly ObservableCollection<IBookmark> bookmarks = new ObservableCollection<IBookmark>();

        /// <summary>
        ///     Gets the bookmarks.
        /// </summary>
        public IList<IBookmark> Bookmarks {get { return bookmarks; }}
   
        /// <summary>
        ///     Event queue for all listeners interested in RedrawRequested events.
        /// </summary>
        public event EventHandler<EventArgs> RedrawRequested;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IconBarManager()
        {
            bookmarks.CollectionChanged += bookmarks_CollectionChanged;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds an entity bookmarks to 'document'.
        /// </summary>
        /// <param name="lineNumber">   The c. </param>
        /// <param name="text">         The document. </param>
        /// <returns>
        /// the bookmarker
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Bookmark AddBreakpoint(int lineNumber, string text )
        {
            var bookmark = new Bookmark(lineNumber,IconUtil.GetImage("Break"),text );
            bookmarks.Add( bookmark );
            bookmark.CanDragDrop = true;
            bookmark.DisplaysTooltip = true;
            bookmark.ZOrder = 0;
            return bookmark;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds an entity bookmarks to 'document'.
        /// </summary>
        /// <param name="lineNumber">   The c. </param>
        /// <param name="text">         The document. </param>
        /// <returns>
        /// the bookmarker
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Bookmark AddCurrentLine(int lineNumber, string text)
        {
            var bookmark = new Bookmark( lineNumber, IconUtil.GetImage( "currentLine" ), text );
            bookmarks.Add( bookmark );
            bookmark.CanDragDrop = true;
            bookmark.DisplaysTooltip = true;
            bookmark.ZOrder = 1;
            return bookmark;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Has book mark at current.
        /// </summary>
        /// <param name="lineNumber">   The c. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IBookmark HasBookMarkAtCurrent(int lineNumber)
        {
            return bookmarks.FirstOrDefault(x => x.LineNumber == lineNumber);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        /////     Adds an entity bookmarks to 'document'.
        ///// </summary>
        ///// <param name="c">        The c. </param>
        ///// <param name="document"> The document. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void AddEntityBookmarks(IUnresolvedTypeDefinition c, IDocument document)
        //{
        //    if (c.IsSynthetic)
        //    {
        //        return;
        //    }
        //    if (!c.Region.IsEmpty)
        //    {
        //        bookmarks.Add(new EntityBookmark(c, document));
        //    }
        //    foreach (var innerClass in c.NestedTypes)
        //    {
        //        AddEntityBookmarks(innerClass, document);
        //    }
        //    foreach (var m in c.Members)
        //    {
        //        if (m.Region.IsEmpty || m.IsSynthetic)
        //        {
        //            continue;
        //        }
        //        bookmarks.Add(new EntityBookmark(m, document));
        //    }
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Event handler. Called by bookmarks for collection changed events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Notify collection changed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void bookmarks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Redraw();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Redraws this object.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Redraw()
        {
            if (RedrawRequested != null)
            {
                RedrawRequested(this, EventArgs.Empty);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        /////     Updates the class member bookmarks.
        ///// </summary>
        ///// <param name="parseInfo">    Information describing the parse. </param>
        ///// <param name="document">     The document. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public void UpdateClassMemberBookmarks(IUnresolvedFile parseInfo, IDocument document)
        //{
        //    for (var i = bookmarks.Count - 1; i >= 0; i--)
        //    {
        //        if (bookmarks[i] is EntityBookmark)
        //        {
        //            bookmarks.RemoveAt(i);
        //        }
        //    }
        //    if (parseInfo == null)
        //    {
        //        return;
        //    }
        //    foreach (var c in parseInfo.TopLevelTypeDefinitions)
        //    {
        //        AddEntityBookmarks(c, document);
        //    }
        //}
    }
}
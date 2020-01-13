////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the tree list view class
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

using System.Windows.Controls;
using System.Windows;


namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Tree list view. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class TreeListView : TreeView
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Creates a <see cref="T:System.Windows.Controls.TreeViewItem" /> to use to display content.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Windows.Controls.TreeViewItem" /> to use as a container for content.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Determines whether the specified item is its own container or can be its own container.
        /// </summary>
        /// <param name="item"> The object to evaluate. </param>
        /// <returns>
        /// true if <paramref name="item" /> is a <see cref="T:System.Windows.Controls.TreeViewItem" />; otherwise, false.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        #region Public Properties

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GridViewColumn List.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public GridViewColumnCollection Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new GridViewColumnCollection();
                }

                return _columns;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The columns.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private GridViewColumnCollection _columns;

        #endregion
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Tree list view item. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class TreeListViewItem : TreeViewItem
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Item's hierarchy in the tree.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int Level
        {
            get
            {
                if (_level == -1)
                {
                    TreeListViewItem parent = ItemsControlFromItemContainer(this) as TreeListViewItem;
                    _level = (parent != null) ? parent.Level + 1 : 0;
                }
                return _level;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Creates a new <see cref="T:System.Windows.Controls.TreeViewItem" /> to use to display the object.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Windows.Controls.TreeViewItem" />.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Determines whether an object is a <see cref="T:System.Windows.Controls.TreeViewItem" />.
        /// </summary>
        /// <param name="item"> The object to evaluate. </param>
        /// <returns>
        /// true if <paramref name="item" /> is a <see cref="T:System.Windows.Controls.TreeViewItem" />; otherwise, false.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The level.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int _level = -1;
    }

}

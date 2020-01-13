////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the folder tree view item class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/15/2015   rcs     Initial Implementation
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

using System.IO;
using System.Windows;
using Views.Support;

namespace IDECore.Tree
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Folder tree view item. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class TreeViewItemFolder : TreeViewItemImage
    {
        /// <summary>
        /// Pathname of the folder.
        /// </summary>
        private readonly DirectoryInfo _folder;

        /// <summary>
        /// Gets the pathname of the folder.
        /// </summary>
        public DirectoryInfo Folder { get { return _folder; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="folder">   The pathname of the folder. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TreeViewItemFolder(DirectoryInfo folder)
        {
            _folder         = folder;
            Text            = folder.Name;
            SelectedImage   = IconUtil.GetImage("folderopen");
            UnselectedImage = IconUtil.GetImage("folderclosed") ;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the image.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void SetImage()
        {
            _image.Source = (IsSelected || IsExpanded) ? _selectedImage : _unselectedImage;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Populates this object.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Populate()
        {
            DirectoryInfo[] folders;

            try
            {
                folders = _folder.GetDirectories();
            }
            catch
            {
                return;
            }
            foreach (var folder in folders)
            {
                Items.Add(new TreeViewItemFolder(folder));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Raises an <see cref="E:System.Windows.Controls.TreeViewItem.Expanded" /> event when the
        /// <see cref="P:System.Windows.Controls.TreeViewItem.IsExpanded" /> property changes from false to true.
        /// </summary>
        /// <param name="e">    The event arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnExpanded(RoutedEventArgs e)
        {
            base.OnExpanded(e);
            foreach (var o in Items)
            {
                TreeViewItemFolder item = o as TreeViewItemFolder;
                if (item != null)
                {
                    item.Populate();
                }
            }
            SetImage();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Raises a <see cref="E:System.Windows.Controls.TreeViewItem.Collapsed" /> event when the
        /// <see cref="P:System.Windows.Controls.TreeViewItem.IsExpanded" /> property changes from true to false.
        /// </summary>
        /// <param name="e">    The event arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnCollapsed(RoutedEventArgs e)
        {
            base.OnCollapsed(e);
            SetImage();

        }
    }
}
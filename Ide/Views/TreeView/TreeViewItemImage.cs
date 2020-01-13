////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the image tree view item class
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IDECore.Tree
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Image tree view item. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class TreeViewItemImage : TreeViewItem
    {
        /// <summary>
        /// The text.
        /// </summary>
        private TextBlock _text;

        /// <summary>
        /// The stack.
        /// </summary>
        private StackPanel _stack;

        /// <summary>
        /// The image.
        /// </summary>
        protected Image _image;

        /// <summary>
        /// The selected image.
        /// </summary>
        protected ImageSource _selectedImage;

        /// <summary>
        /// The unselected image.
        /// </summary>
        protected ImageSource _unselectedImage;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get { return _text.Text; } set { _text.Text = value; } }

        /// <summary>
        /// Gets or sets the selected image.
        /// </summary>
        public ImageSource SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                _selectedImage = value;
                SetImage();
            }
        }


        /// <summary>
        /// Gets or sets the unselected image.
        /// </summary>
        public ImageSource UnselectedImage
        {
            get { return _unselectedImage; }
            set
            {
                _unselectedImage = value;
                SetImage();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TreeViewItemImage()
        {
            _image                   = new Image();
            _image.VerticalAlignment = VerticalAlignment.Center;
            _image.Margin            = new Thickness(1,2,4,2);
            _image.Height = 16;
            _image.Width = 16;
           
            _text                    = new TextBlock();
            _text.VerticalAlignment  = VerticalAlignment.Center;

            _stack                   = new StackPanel();
            _stack.Orientation       = Orientation.Horizontal;

            _stack.Children.Add(_image);
            _stack.Children.Add(_text);

            Header = _stack;
        }
      
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the image.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void SetImage()
        {
            _image.Source = IsSelected ? _selectedImage : _unselectedImage;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.TreeViewItem.Selected" /> routed event when the
        /// <see cref="P:System.Windows.Controls.TreeViewItem.IsSelected" /> property changes from false to true.
        /// </summary>
        /// <param name="e">    The event arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            _image.Source = _selectedImage;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.TreeViewItem.Unselected" /> routed event when the
        /// <see cref="P:System.Windows.Controls.TreeViewItem.IsSelected" /> property changes from true to false.
        /// </summary>
        /// <param name="e">    The event arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            _image.Source = _unselectedImage;
        }
    }
}




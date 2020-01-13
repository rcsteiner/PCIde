////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the solution tree.xaml class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/5/2015   rcs     Initial Implementation
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
using System.Windows.Input;
using ViewModels;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interaction logic for ViewSolutionTree.xaml.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class ViewSolutionTree : UserControl
    {
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
            "Selected", typeof (object), typeof (ViewSolutionTree), new PropertyMetadata(default(object)));

        /// <summary>
        /// Gets or sets the selected.
        /// </summary>
        public object Selected
        {
            get { return  GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }
     
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewSolutionTree()
        {
            InitializeComponent();

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by SolutionView for on key down events.
        /// </summary>
        /// <param name="sender">   . </param>
        /// <param name="e">        Key event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SolutionView_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                SetCurrentItemInEditMode();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Sets a current item in edit mode.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetCurrentItemInEditMode()
        {
            
            // Make sure that the SelectedItem is actually a TreeViewItem
            // and not null or something else
            var item = SolutionView.SelectedItem as ViewModelTreePath;
            if (item !=null && item.RenameCanExecute())
            {
                   item.RenameExecute();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by SolutionView for on selected item changed events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        The e. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SolutionView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Selected = SolutionView.SelectedItem;
        }
    }
}

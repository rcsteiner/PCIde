////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the property grid.xaml class
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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Interaction logic for PropertyGrid.xaml.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class PropertyGrid : UserControl
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PropertyGrid()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Event handler. Called by DockPanel for is keyboard focus within changed events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Dependency property changed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DockPanel_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement) sender;
            if (element.IsKeyboardFocusWithin)
            {
                Visual cur = element;
                while (cur != null && !(cur is ListBoxItem))
                {
                    cur = (Visual) VisualTreeHelper.GetParent(cur);
                }
                ((ListBoxItem) cur).IsSelected = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Event handler. Called by DockPanel for mouse down events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Mouse event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DockPanel_MouseDown(object sender, MouseEventArgs e)
        {
            ((FrameworkElement) sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        /////     Initializes the view.
        ///// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void InitializeView()
        //{
        //    var view = CollectionViewSource.GetDefaultView(Parameters);
        //    if (view.GroupDescriptions.Count == 0)
        //    {
        //        view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
        //    }

        //    if (view.SortDescriptions.Count == 0)
        //    {
        //        view.SortDescriptions.Add(new SortDescription("Category", ListSortDirection.Ascending));
        //        view.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
        //    }
        //}
    }
}
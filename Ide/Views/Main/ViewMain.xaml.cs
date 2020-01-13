////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the main window.xaml class
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

using System;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Interaction logic for ViewMain.xaml.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class ViewMain : IView
    {
        private UserWindowSettings _windowSettings;

        /// <summary>
        ///     Manager for docking.
        /// </summary>
        public DockingManager DockingManager;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewMain()
        {
            InitializeComponent();
            _windowSettings = new UserWindowSettings();

            Height      = _windowSettings.WindowHeight;
            Width       = _windowSettings.WindowWidth;
            Top         = _windowSettings.WindowTop;
            Left        = _windowSettings.WindowLeft;
            WindowState = _windowSettings.WindowState;
            //  highlightingComboBox.DropDownOpened += HighlightingComboBoxOnDropDownOpened;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closed" /> event.
        /// </summary>
        /// <param name="e">    An <see cref="T:System.EventArgs" /> that contains the event data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _windowSettings.WindowHeight = Height;
            _windowSettings.WindowWidth  = Width;
            _windowSettings.WindowTop    = Top;
            _windowSettings.WindowLeft   = Left;
            _windowSettings.WindowState  = WindowState;

            _windowSettings.Save();

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     View model closing handler.
        /// </summary>
        /// <param name="dialogResult"> The dialog result. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ViewModelClosingHandler(bool dialogResult)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     View model activating handler.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ViewModelActivatingHandler()
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Highlighting combo box on drop down opened.
        ///// </summary>
        ///// <param name="sender">       Source of the event. </param>
        ///// <param name="eventArgs">    Event information. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void HighlightingComboBoxOnDropDownOpened(object sender, EventArgs eventArgs)
        //{
        //    highlightingComboBox.Items.Clear();
        //    foreach (var h in HighlightingManager.Instance.HighlightingDefinitions)
        //    {
        //        highlightingComboBox.Items.Add(h);
        //    }
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Event handler. Called by HighlightingComboBox for selection changed events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Selection changed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var doc = _app.VMActiveDocument;
            //doc.UpdateLanguage();
        }
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the goto line view.xaml class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/20/2015   rcs     Initial Implementation
//  =====================================================================================================
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
using System.Windows.Input;

namespace Views.Dialogs
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// This class implement the view part of a goto text editor line dialog as Custom (look-less) WPF Control.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class GotoLineDialog
    {
        private static GotoLineDialog _gotoDialog;

        /// <summary>
        ///     Goto line n in a given document
        /// </summary>
        public static RoutedUICommand GotoLine
        {
            get { return _gotoLine; }
        }

        private static RoutedUICommand _gotoLine;

        /// <summary>
        /// Gets the line number
        /// </summary>
        public int Line {get{int r;  return int.TryParse(TextLineNumber.Text, out r) ? r :-1;}}


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Style key for look-less control.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static GotoLineDialog()
        {
            var inputs = new InputGestureCollection {new KeyGesture(Key.G, ModifierKeys.Control, "Ctrl+G")};
            _gotoLine = new RoutedUICommand("Goto Line", "GotoLine", typeof (GotoLineDialog), inputs);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructur.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public GotoLineDialog(int max)
        {
            InitializeComponent();
            LabelMax.Text = string.Format("Enter a line number between ({0} - {1})", 1, max);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the goto dialog.
        /// </summary>
        /// <param name="lineMax">  The line maximum. </param>
        /// <returns>
        /// the line number or -1 if cancelled.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static int ShowGotoDialog(int lineMax)
        {
            if (_gotoDialog == null)
            {
                _gotoDialog = new GotoLineDialog(lineMax);
            }
            _gotoDialog.Owner = Application.Current.MainWindow;

            _gotoDialog.ShowDialog();
            int r =  _gotoDialog.Line;
            _gotoDialog = null;
            return r>0 && r<=lineMax ?r:-1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by Ok for click events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            //  var iCurrLine = TextArea.Caret.Line;
            Close();
        }
    }
}
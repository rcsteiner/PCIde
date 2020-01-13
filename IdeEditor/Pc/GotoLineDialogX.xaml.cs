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
//
// Copyright:
//  Copyright Advanced Performance
//  All Rights Reserved THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Advanced Performance
//  The copyright notice above does not evidence any actual or intended publication of such source code.
//  Some third-party source code components may have been modified from their original versions
//  by Advanced Performance. The modifications are Copyright Advanced Performance., All Rights Reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;

namespace Views.Dialogs
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// This class implement the view part of a goto text editor line dialog as Custom (look-less) WPF Control.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class GotoLineDialogX
    {
        private static GotoLineDialogX  _gotoDialog;
        /// <summary>
        /// Gets the line number
        /// </summary>
        public int Line {get{int r; return int.TryParse( TextLineNumber.Text, out r ) ? r : 0;}}


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Style key for look-less control.
        /// </summary>
        /// <param name="max">  The maximum line. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static GotoLineDialogX()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructur.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public GotoLineDialogX(int max) 
        {
            InitializeComponent();
            LabelMax.Text = string.Format("Enter a line number between ({0} - {1})", 1, max);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the goto dialog.
        /// </summary>
        /// <param name="editor">   The editor. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void ShowGotoDialog(TextEditor editor)
        {
            if (_gotoDialog == null)
            {
                _gotoDialog = new GotoLineDialogX(editor.Document.LineCount);
            }
            _gotoDialog.Owner = Application.Current.MainWindow;

            _gotoDialog.ShowDialog();
            //_gotoDialog.Activate();

           var r =  _gotoDialog.DialogResult;

            if (r != null && (bool)r)
            {
                 //  var iCurrLine = TextArea.Caret.Line;
                var l = editor.Document.GetLineByNumber(_gotoDialog.Line);

                editor.Select(l.Offset, 0);
                var loc = editor.Document.GetLocation(l.Offset);
                editor.ScrollTo(loc.Line, loc.Column);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by Window for closed events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Window_Closed(object sender, EventArgs e)
        {
        }
    }
}
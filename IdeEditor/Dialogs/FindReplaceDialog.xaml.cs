////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the findreplacedialog.xaml class
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
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;

namespace Views.Dialogs
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interaction logic for FindReplaceDialog.xaml.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class FindReplaceDialog : Window
    {
        /// <summary>
        /// The text to find.
        /// </summary>
        private static string _textToFind = "";

        /// <summary>
        /// The case sensitive.
        /// </summary>
        private static bool _caseSensitive = true;

        /// <summary>
        /// The whole word.
        /// </summary>
        private static bool _wholeWord = true;

        /// <summary>
        /// true to use regex.
        /// </summary>
        private static bool _useRegex;

        /// <summary>
        /// true to use wildcards.
        /// </summary>
        private static bool _useWildcards;

        /// <summary>
        /// true to search up.
        /// </summary>
        private static bool _searchUp;

        /// <summary>
        /// The find replace dialog
        /// </summary>
        private static FindReplaceDialog _findReplaceDialog;

        /// <summary>
        /// The editor.
        /// </summary>
        private readonly Editors.IdeEditor _editor;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="editor">   The editor. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FindReplaceDialog(Editors.IdeEditor editor)
        {
            InitializeComponent();

            _editor                   = editor;
            txtFind.Text              = txtFind2.Text = _textToFind;
            cbCaseSensitive.IsChecked = _caseSensitive;
            cbWholeWord.IsChecked     = _wholeWord;
            cbRegex.IsChecked         = _useRegex;
            cbWildcards.IsChecked     = _useWildcards;
            cbSearchUp.IsChecked      = _searchUp;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Finds the next item.
        /// </summary>
        /// <param name="textToFind">   The text to find. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool FindNext(string textToFind)
        {
            Regex regex = GetRegEx(textToFind);
            int start   = regex.Options.HasFlag(RegexOptions.RightToLeft)? _editor.SelectionStart: _editor.SelectionStart + _editor.SelectionLength;
            var match   = regex.Match(_editor.Text, start);

            if (!match.Success) // start again from beginning or end
            {
                match = regex.Match(_editor.Text, regex.Options.HasFlag(RegexOptions.RightToLeft) ? _editor.Text.Length : 0);
            }

            if (match.Success)
            {
                _editor.Select(match.Index, match.Length);
                TextLocation loc = _editor.doc.GetLocation(match.Index);
                _editor.ScrollTo(loc.Line, loc.Column);
            }

            return match.Success;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first next 2 click.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FindNext2Click(object sender, RoutedEventArgs e)
        {
            if (!FindNext(txtFind2.Text))
            {
                SystemSounds.Beep.Play();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first next click.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FindNextClick(object sender, RoutedEventArgs e)
        {
            if (!FindNext(txtFind.Text))
            {
                SystemSounds.Beep.Play();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a reg ex.
        /// </summary>
        /// <param name="textToFind">   The text to find. </param>
        /// <param name="leftToRight">  (optional) true to left to right. </param>
        /// <returns>
        /// The reg ex.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Regex GetRegEx(string textToFind, bool leftToRight = false)
        {
            var options = RegexOptions.None;
            if (cbSearchUp.IsChecked == true && !leftToRight)
            {
                options |= RegexOptions.RightToLeft;
            }
            if (cbCaseSensitive.IsChecked == false)
            {
                options |= RegexOptions.IgnoreCase;
            }

            if (cbRegex.IsChecked == true)
            {
                return new Regex(textToFind, options);
            }
            var pattern = Regex.Escape(textToFind);
            if (cbWildcards.IsChecked == true)
            {
                pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
            }
            if (cbWholeWord.IsChecked == true)
            {
                pattern = "\\b" + pattern + "\\b";
            }
            return new Regex(pattern, options);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Replace all click.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ReplaceAllClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to Replace All occurences of \"{0}\" with \"{1}\"?", txtFind2.Text, txtReplace.Text),
                "Replace All", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Regex regex = GetRegEx(txtFind2.Text, true);
                var offset = 0;
                _editor.BeginChange();
                foreach (Match match in regex.Matches(_editor.Text))
                {
                    _editor.doc.Replace(offset + match.Index, match.Length, txtReplace.Text);
                    offset += txtReplace.Text.Length - match.Length;
                }
                _editor.EndChange();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Replace click.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ReplaceClick(object sender, RoutedEventArgs e)
        {
            Regex regex     = GetRegEx(txtFind2.Text);
            string input    = _editor.doc.Text.Substring(_editor.SelectionStart, _editor.SelectionLength);
            var match       = regex.Match(input);
            var replaced    = false;
            if (match.Success && match.Index == 0 && match.Length == input.Length)
            {
                _editor.doc.Replace(_editor.SelectionStart, _editor.SelectionLength, txtReplace.Text);
                replaced = true;
            }

            if (!FindNext(txtFind2.Text) && !replaced)
            {
                SystemSounds.Beep.Play();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows for replace.
        /// </summary>
        /// <param name="editor">   The editor. </param>
        /// <param name="showFind"> start with the find tab else replace tab </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void ShowForReplace(Editors.IdeEditor editor, bool showFind)
        {
            if (_findReplaceDialog == null)
            {
                _findReplaceDialog = new FindReplaceDialog(editor);
                _findReplaceDialog.Show();
            }
 
            _findReplaceDialog.tabMain.SelectedIndex = showFind?0:1;
            _findReplaceDialog.Activate();

            if (!editor.TextArea.Selection.IsMultiline)
            {
                _findReplaceDialog.txtFind.Text = _findReplaceDialog.txtFind2.Text = editor.TextArea.Selection.GetText();
                _findReplaceDialog.txtFind.SelectAll();
                _findReplaceDialog.txtFind2.SelectAll();
                _findReplaceDialog.txtFind2.Focus();
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
            _textToFind    = txtFind2.Text;
            _caseSensitive = (cbCaseSensitive.IsChecked == true);
            _wholeWord     = (cbWholeWord.IsChecked == true);
            _useRegex      = (cbRegex.IsChecked == true);
            _useWildcards  = (cbWildcards.IsChecked == true);
            _searchUp      = (cbSearchUp.IsChecked == true);

            _findReplaceDialog = null;
        }
    }
}
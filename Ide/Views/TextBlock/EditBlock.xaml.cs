////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the editable text block.xaml class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/14/2015   rcs     Initial Implementation
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

namespace Controls
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Editable text block. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class EditBlock
    {
        #region Member Variables
        /// <summary>
        /// The old text.
        ///  We keep the old text when we go into editmode
        /// in case the user aborts with the escape key
        /// </summary>
        private string _oldText;

        #endregion Member Variables

        #region Properties

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this object is editable.
        /// </summary>
        public bool IsEditable
        {
            get { return (bool) GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
  
        /// <summary>
        /// Gets or sets a value indicating whether this object is in edit mode.
        /// set only if editable
        /// </summary>
        public bool IsEditingName
        {
            get { return IsEditable && (bool) GetValue(IsEditingNameProperty); }
            set
            {
               if (IsEditable)
                {
                    if (value)
                    {
                        _oldText = Text;
                    }
                    SetValue( IsEditingNameProperty, value );
                }
            }
        }

        /// <summary>
        /// Gets or sets the text format.
        /// </summary>
        public string TextFormat
        {
            get { return (string) GetValue(TextFormatProperty); }
            set
            {
                if (value == "")
                {
                    value = "{0}";
                }
                SetValue(TextFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets the formatted text.
        /// </summary>
        public string FormattedText
        {
            get { return string.Format( TextFormat, Text ); }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The text property.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof (string),
                typeof (EditBlock),
                new PropertyMetadata(""));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The is editable property.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(
                "IsEditable",
                typeof (bool),
                typeof (EditBlock),
                new PropertyMetadata(true));


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The is in edit mode property.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static readonly DependencyProperty IsEditingNameProperty =
            DependencyProperty.Register(
                "IsEditingName",
                typeof (bool),
                typeof (EditBlock),
                new PropertyMetadata(false));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The text format property.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static readonly DependencyProperty TextFormatProperty =
            DependencyProperty.Register(
                "TextFormat",
                typeof (string),
                typeof (EditBlock),
                new PropertyMetadata("{0}"));


        #endregion Properties

        #region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public EditBlock()
        {
            InitializeComponent();
            Focusable        = true;
            FocusVisualStyle = null;
        }

        #endregion Constructor

        #region Event Handlers


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by TextBox for loaded events. Invoked when we enter edit mode.
        ///
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;

            // Give the TextBox input focus
            txt.Focus();
            txt.SelectAll();
            _oldText = Text;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by TextBox for lost focus events. Invoked when we exit edit mode.
        ///
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsEditingName = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by TextBox for key down events. Invoked when the user edits the annotation.
        ///
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Key event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IsEditingName = false;
                e.Handled     = true;
            }
            else if (e.Key == Key.Escape)
            {
                Text          = _oldText;
                IsEditingName = false;
                e.Handled     = true;
            }
        }

        #endregion Event Handlers
    }
}
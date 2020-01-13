////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the text box with watermark class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/5/2015   rcs     Initial Implementation
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

namespace Ide
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Interaction logic for TextBoxWithWaterMark.xaml
    ///     Source: http://www.dotnetspark.com/kb/1716-create-watermark-textbox-wpf-application.aspx.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class TextBoxWithWatermark : UserControl
    {
        #region Static Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Static constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static TextBoxWithWatermark()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (TextBoxWithWatermark),new FrameworkPropertyMetadata(typeof (TextBoxWithWatermark)));
        }

        #endregion

        #region fields

        ///// <summary>
        /////     The label text box property.
        ///// </summary>
        //private static readonly DependencyProperty LabelColorProperty = DependencyProperty.Register( "LabelColor",  typeof( Color ), typeof (TextBoxWithWatermark) );

        /// <summary>
        ///     The label text box property.
        /// </summary>
        private static readonly DependencyProperty LabelTextBoxProperty = DependencyProperty.Register("LabelTextBox",typeof (string), typeof (TextBoxWithWatermark));

        /// <summary>
        ///     The text property.
        /// </summary>
        private static readonly DependencyProperty TextProperty =TextBox.TextProperty.AddOwner(typeof (TextBoxWithWatermark));

        /// <summary>
        ///     The watermark property.
        /// </summary>
        private static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark",typeof (string), typeof (TextBoxWithWatermark));

        /// <summary>
        /// The accepts return property
        /// </summary>
        public static readonly DependencyProperty AcceptsReturnProperty = DependencyProperty.Register("AcceptsReturn", typeof (bool), typeof (TextBoxWithWatermark), new PropertyMetadata(default(bool)));

        #endregion fields

        #region properties


        ///// <summary>
        /////     Declare a label color dependency property.
        ///// </summary>
        //public Color LabelColor
        //{
        //    get { return (Color)GetValue( LabelColorProperty ); }
        //    set { SetValue( LabelColorProperty, value ); }
        //}

        /// <summary>
        ///     Declare a TextBox label dependency property.
        /// </summary>
        public string LabelTextBox
        {
            get { return (string) GetValue(LabelTextBoxProperty); }
            set { SetValue(LabelTextBoxProperty, value); }
        }

        /// <summary>
        ///     Declare a TextBox Text dependency property.
        /// </summary>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        ///     Declare a TextBox Watermark label dependency property.
        /// </summary>
        public string Watermark
        {
            get { return (string) GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [accepts return].
        /// </summary>
        public bool AcceptsReturn
        {
            get { return (bool) GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }

        #endregion properties
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the output item class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/31/2015   rcs     Initial Implementation
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
//  USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Output item. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class OutputItem : ViewModelBase
    {
        private Boolean _isEditable;
        private string  _text;
        private string  _input;
        private bool    _isFocus;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged( "Text" );
            }
        }

        /// <summary>
        /// Gets or sets the input text
        /// </summary>
        public string Input
        {
            get { return _input; }
            set { _input = value; RaisePropertyChanged("Input"); }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this object is editable.
        /// </summary>
        public Boolean IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                RaisePropertyChanged( "IsEditable" );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is focus.
        /// </summary>
        public bool IsFocus
        {
            get { return _isFocus; }
            set { _isFocus = value; RaisePropertyChanged( "IsFocus" ); }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text"> The text. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public OutputItem(string text)
        {
            _text      = text;
            _input     = "";
            IsEditable = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return _text;
        }
    }
}
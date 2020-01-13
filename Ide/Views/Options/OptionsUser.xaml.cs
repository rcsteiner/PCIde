////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the options general.xaml class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/14/2015   rcs     Initial Implementation
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

using System.Windows.Controls;

namespace  Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interaction logic for OptionsGeneral.xaml.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class OptionsUser : UserControl
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public OptionsUser()
        {
            InitializeComponent();
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  The Name Validator Class definition.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class NameValidator : ValidationRule
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Validate
        /// </summary>
        /// <param name="value">       The value.</param>
        /// <param name="cultureInfo"> The culture information.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var s = value.ToString();

            if (string.IsNullOrEmpty(s))
            {
                return new ValidationResult(false,"value cannot be empty.");
            }
            for (int i = 0; i < s.Length; ++i)
            {
                var c = s[i];
                if (char.IsLetter(c) || c=='.' || c==' ') continue;
                if (char.IsDigit(c)) return new ValidationResult(false,"Name cannot contain digits");
                if (char.IsPunctuation(c) ) return new ValidationResult(false, "Punctuation characters are not allowed");
                return  new ValidationResult(false, "Invalid Characters in the name, only letters and '.' allowed");

            }
            if (s.Length < 3)
            {
                return new ValidationResult(false, "Name must be moe than 2 characters long.");
            }
            return ValidationResult.ValidResult;
        }
    }
}

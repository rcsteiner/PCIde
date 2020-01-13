////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the string range validation rule class
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
//  Redistribution and use in source and binary forms, with or without   modification,
//  are permitted provided that the following conditions are met:
//
//  1. Redistributions of source code must retain the above copyright notice, this
//  list of conditions and the following disclaimer.
//
//  2. Redistributions in binary form must reproduce the above copyright notice,
//  this list of conditions and the following disclaimer in the documentation
//  and/or other materials provided with the distribution.
//
//  3. Neither the name of the copyright holder nor the names of its
//  contributors may be used to endorse or promote products derived from
//  this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
//  FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//  DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
//  CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//  OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Globalization;
using System.Windows.Controls;

namespace Ide.Validation
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// String range validation rule. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class StringRangeValidationRule : ValidationRule
    {
        /// <summary>
        /// Length of the maximum.
        /// </summary>
        private int _maximumLength = -1;

        /// <summary>
        /// Length of the minimum.
        /// </summary>
        private int _minimumLength = -1;

        /// <summary>
        /// Gets or sets the length of the minimum.
        /// </summary>
        public int MinimumLength
        {
            get { return _minimumLength; }
            set { _minimumLength = value; }
        }

        /// <summary>
        /// Gets or sets the length of the maximum.
        /// </summary>
        public int MaximumLength
        {
            get { return _maximumLength; }
            set { _maximumLength = value; }
        }

        /// <summary>
        /// Gets or sets a message describing the error.
        /// </summary>
        public string ErrorMessage { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// When overridden in a derived class, performs validation checks on a value.
        /// </summary>
        /// <param name="value">        The value from the binding target to check. </param>
        /// <param name="cultureInfo">  The culture to use in this rule. </param>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ValidationResult" /> object.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ValidationResult Validate(object value,CultureInfo cultureInfo)
        {
            var result      = new ValidationResult(true, null);
            var inputString = (value ?? string.Empty).ToString();
            if (inputString.Length < MinimumLength || (MaximumLength > 0 && inputString.Length > MaximumLength))
            {
                result = new ValidationResult(false, ErrorMessage);
            }
            return result;
        }
    }
}
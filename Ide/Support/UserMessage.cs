////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the user message class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/18/2015   rcs     Initial Implementation
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

using System;
using System.Collections.Generic;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace Ide
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// User message. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static class UserMessage
    {

        /// <summary>
        /// The error list log 
        /// </summary>
        public static List<string> ErrorList = new List<string>();

        /// <summary>
        /// Gets the error summary.
        /// </summary>
        public static string ErrorSummary { get { return string.Join("\r\n", ErrorList); } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the erorr.
        /// </summary>
        /// <param name="caption">  The caption. </param>
        /// <param name="message">  The message. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void ShowErorr(string message, string caption)
        {
           ErrorList.Add(string.Format("Error {0}: {1}", caption, message) );
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the question.
        /// </summary>
        /// <param name="caption">  The caption. </param>
        /// <param name="message">  The message. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool ShowQuestion(string message, string caption)
        {
           return MessageBox.Show(caption,message, MessageBoxButton.YesNo, MessageBoxImage.Question ) == MessageBoxResult.Yes;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the asterik.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="caption">  The caption. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void ShowAsterik(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Asterisk );
            ErrorList.Add( string.Format( "Problem {0}: {1}", caption, message ) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the exception.
        /// </summary>
        /// <param name="e">        The execption </param>
        /// <param name="caption">  The caption. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void ShowException(Exception e, string caption)
        {
            MessageBox.Show( e.Message, caption, MessageBoxButton.OK, MessageBoxImage.Asterisk );
            ErrorList.Add(string.Format("Exception {0}: {1}", caption, e.Message));
        }
    }
}
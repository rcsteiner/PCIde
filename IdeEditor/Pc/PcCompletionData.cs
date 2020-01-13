////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the pc completion data class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/26/2015   rcs     Initial Implementation
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
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Implements AvalonEdit ICompletionData interface to provide the entries in the completion drop down.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PcCompletionData : ICompletionData
    {
        /// <summary>
        /// Gets the image.
        /// </summary>
        public ImageSource Image {get { return null; }}

        /// <summary>
        /// Gets or sets the text. This property is used to filter the list of visible elements.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The displayed content. This can be the same as 'Text', or a WPF UIElement if you want to display rich content.
        /// </summary>
        public object Content {get { return Text; }}

        /// <summary>
        /// Gets the description.
        /// </summary>
        public object Description {get { return "Description for " + Text; }}

        /// <summary>
        /// Gets the priority. This property is used in the selection logic. You can use it to prefer selecting those items
        /// which the user is accessing most frequently.
        /// </summary>
        public double Priority {get { return 0; }}
   
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text"> The text. This property is used to filter the list of visible elements. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PcCompletionData(string text)
        {
            Text = text;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Perform the completion.
        /// </summary>
        /// <param name="textArea">                     The text area on which completion is performed. </param>
        /// <param name="completionSegment">            The text segment that was used by the completion window if the user
        ///                                             types (segment between CompletionWindow.StartOffset and
        ///                                             CompletionWindow.EndOffset). </param>
        /// <param name="insertionRequestEventArgs">    The EventArgs used for the insertion request. These can be
        ///                                             TextCompositionEventArgs, KeyEventArgs, MouseEventArgs, depending on how
        ///                                             the insertion was triggered. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }
}
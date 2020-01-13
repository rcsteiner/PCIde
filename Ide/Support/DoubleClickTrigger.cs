////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the double click trigger class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/13/2015   rcs     Initial Implementation
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
//  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.Reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Ide.Support
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// This is a Blend/VS.NET behavior which drives a DoubleClick event to some interactive action.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class DoubleClickTrigger : TriggerBase<UIElement>
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This handles the UIElement.LeftButtonDown event to test for a double-click event.
        /// </summary>
        /// <param name="sender">           UIElement. </param>
        /// <param name="mouseEventArgs">   EventArgs. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void HandlePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.ClickCount != 2)
            {
                return;
            }
            InvokeActions(mouseEventArgs);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        ///
        /// ### <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDown += HandlePreviewMouseLeftButtonDown;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        ///
        /// ### <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseDown -= HandlePreviewMouseLeftButtonDown;
        }
    }
}
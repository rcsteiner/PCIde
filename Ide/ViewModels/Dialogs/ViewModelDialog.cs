////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the view model Dialog
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
//  USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Windows.Input;
using Ide.Controller;
using Views;
using Views.Support;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Dialog for setting the view model. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelDialog : ViewModelBase
    {
        private RelayCommand _cancelCommand;
        private RelayCommand _okCommand;

        /// <summary>
        /// Gets or sets the title text.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the title text.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand {get{return _cancelCommand ??(_cancelCommand = new RelayCommand(() => CancelCommandExecute(), () => CancelCommandCanExecute()));}}

        /// <summary>
        /// Gets the ok command.
        /// </summary>
        public ICommand OkCommand {get{return _okCommand ?? (_okCommand = new RelayCommand(() => OkCommandExecute(), () => OkCommandCanExecute()));}}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        /// <param name="view">         The view. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelDialog(ControllerIde controller, IView view) : base(controller, view)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Queries if we cancel command execute.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool CancelCommandCanExecute()
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Cancel command can execute.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void CancelCommandExecute()
        {
            CloseViewModel( false );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Ok command execute.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual bool OkCommandCanExecute()
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Ok command can execute.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OkCommandExecute()
        {
            CloseViewModel( true );
        }
    }
}
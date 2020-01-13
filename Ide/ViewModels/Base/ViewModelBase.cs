////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the base view model class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/4/2015   rcs     Initial Implementation
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

using System.Collections.Generic;
using Ide.Controller;
using Ide.Support;
using Views;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A base class for all view models.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public abstract class ViewModelBase : ObservableObject
    {
        /// <summary>
        /// Keep a list of any children ViewModels so we can safely remove them when this ViewModel gets closed.
        /// </summary>
        private readonly List<ViewModelBase> childViewModels = new List<ViewModelBase>();

        /// <summary>
        /// If the ViewModel wants to do anything, it needs a controller.
        /// </summary>
        protected ControllerIde Controller { get; set; }

        /// <summary>
        /// Event queue for all listeners interested in ViewModelActivating events.
        /// </summary>
        public event ViewModelActivatingEventHandler ViewModelActivating;

        /// <summary>
        /// Event queue for all listeners interested in ViewModelClosing events.
        /// </summary>
        public event ViewModelClosingEventHandler ViewModelClosing;

        /// <summary>
        /// Gets the child view models.
        /// </summary>
        public List<ViewModelBase> ChildViewModels
        {
            get { return childViewModels; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dialog result].
        /// </summary>
        public bool DialogResult { get; set; }

        #region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parameterless Constructor required for support of DesignTime versions of View Models.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelBase()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// A view model needs a controller reference.
        /// </summary>
        /// <param name="controller">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelBase(ControllerIde controller)
        {
            Controller = controller;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Create the View Model with a Controller and a FrameworkElement (View) injected. Note that we don't keep a
        /// reference to the View - just set its data context and subscribe it to our Activating and Closing events... Of
        /// course, this means there are references - that must be removed when the view closes, which is handled in the
        /// BaseView.
        /// </summary>
        /// <param name="controller">  the controller. </param>
        /// <param name="view">        the view . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelBase(ControllerIde controller, IView view): this( controller )
        {
            BindView(controller,view);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Bind view.
        /// </summary>
        /// <param name="controller">  the controller. </param>
        /// <param name="view">        the view . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void BindView(ControllerIde controller,IView view)
        {
            Controller = controller;

            if (view != null)
            {
                view.DataContext = this;
                ViewModelClosing += view.ViewModelClosingHandler;
                ViewModelActivating += view.ViewModelActivatingHandler;
            }

            MessegerRegister();
        }

        #endregion

        #region public methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// De-Register the VM from the Messenger to avoid non-garbage collected VMs receiving messages Tell the View (via
        /// the ViewModelClosing event) that we're closing.
        /// </summary>
        /// <param name="dialogResult"> The dialog result to pass to all children </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CloseViewModel(bool dialogResult)
        {
            DialogResult = dialogResult;

            MessegerDeregister();
           
            if (ViewModelClosing != null)
            {
                ViewModelClosing(dialogResult);
            }
         
            foreach (var childViewModel in childViewModels)
            {
                childViewModel.CloseViewModel(dialogResult);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Activates a view model.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ActivateViewModel()
        {
            if (ViewModelActivating != null)
            {
                ViewModelActivating();
            }
        }



        #endregion


    }
}
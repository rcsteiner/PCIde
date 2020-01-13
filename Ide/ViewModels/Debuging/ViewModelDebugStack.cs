////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the error view model class
//
// Author:
//  Robert Steiner
//  
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/20/2015   rcs     Initial Implementation
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

using System.Collections.ObjectModel;
using Ide.Controller;
using ZCore;

namespace ViewModels
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// File stats view model.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelDebugStack : ViewModelTool
    {
        private Error _selectedItem;

        /// <summary>
        /// Occurs when [selected item changed].
        /// </summary>
        public event SelectedItemChangedHandler SelectedItemChanged;

        /// <summary>
       /// Gets or sets the error list.
       /// </summary>
       public ObservableCollection<string> CallStack { get { return Controller.CallStack; } }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public Error SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
                OnSelectedItemChanged(_selectedItem);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelDebugStack() : this(null)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       public ViewModelDebugStack(ControllerIde controller)
           : base( "Debug Stack", controller )
        {
           //Controller..ActiveDocumentChanged += OnActiveDocumentChanged;
            ContentId    = Constants.DEBUG_STACK_CONTENT_ID;
            IconSource   = IconUtil.GetImage("DebugStack");

          //  Add(new Error(ErrorLevel.INFO, BuildInfo.Product,1,1,0,"Initial loading of error list"));

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Clears this object to its blank/initial state.
        ///// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public void Clear()
        //{
        //    CallStack.Clear();
        //}


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Queries if we can error select execute 'error'.
        ///// </summary>
        ///// <param name="error">    The Error to add. </param>
        ///// <returns>
        ///// true if it succeeds, false if it fails.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private static bool CanErrorSelectExecute(Error error)
        //{
        //    return !string.IsNullOrEmpty( error.FilePath.FullPath );
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Error select execute.
        ///// </summary>
        ///// <param name="error">    The Error to add. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private static void ErrorSelectExecute(Error error)
        //{
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the selected item changed action.
        /// </summary>
        /// <param name="item"> The item. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void OnSelectedItemChanged(Error item)
        {
            var handler = SelectedItemChanged;
            if (handler != null && item != null)
            {
                handler(item);
            }
        }
    }
}
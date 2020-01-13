////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the view model solution options class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/9/2015   rcs     Initial Implementation
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

using Ide;
using Ide.Controller;
using Views;

namespace ViewModels
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     View model solution options.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelWorkspaceOptions : ViewModelDialog
    {
        /// <summary>
        ///     Gets or sets a value indicating whether [open last solution].
        /// </summary>
        public bool OpenLastWorkspace
        {
            get { return AppSettings.Default.OpenLastWorkspace; }
            set
            {
                AppSettings.Default.OpenLastWorkspace = value;
                RaisePropertyChanged("OpenLastWorkspace");
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to outline documents.
        /// </summary>
        public bool OutlineDocuments
        {
            get { return AppSettings.Default.OutlineDouments; }
            set
            {
                AppSettings.Default.OutlineDouments = value;
                RaisePropertyChanged( "OutlineDouments" );
            }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether [automatic reload file changed].
        /// </summary>
        public bool AutoReloadFileChanged
        {
            get { return AppSettings.Default.AutoLoadChangedFiles; }
            set
            {
                AppSettings.Default.AutoLoadChangedFiles = value;
                RaisePropertyChanged("AutoReloadFileChanged");
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [show output window floating on run].
        /// </summary>
        public bool ShowOutputWindowFLoatingOnRun
        {
            get { return AppSettings.Default.ShowOutputWindowFloatingOnRun; }
            set
            {
                AppSettings.Default.ShowOutputWindowFloatingOnRun = value;
                RaisePropertyChanged("ShowOutputWindowFLoatingOnRun");
            }
        }


        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string UserName
        {
            get { return AppSettings.Default.UserName; }
            set
            {
                AppSettings.Default.UserName = value;
                RaisePropertyChanged("UserName");
            }
        }

        /// <summary>
        /// Gets or sets the user class.
        /// </summary>
        public string UserClass
        {
            get { return AppSettings.Default.UserClass; }
            set
            {
                AppSettings.Default.UserClass = value;
                RaisePropertyChanged("UserClass");
            }
        }

        /// <summary>
        /// Gets or sets flag indicating to run after compile
        /// </summary>
        public bool AutoRun
        {
            get { return AppSettings.Default.AutoRun; }
            set
            {
                AppSettings.Default.AutoRun = value;
                RaisePropertyChanged("AutoRun");
            }
        }

        /// <summary>
        /// Gets or sets flag indicating to run after compile
        /// </summary>
        public bool NoSet
        {
            get { return AppSettings.Default.NoSet; }
            set
            {
                AppSettings.Default.NoSet = value;
                RaisePropertyChanged("NoSet");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelWorkspaceOptions() : base(null, null)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        /// <param name="view">         The view. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelWorkspaceOptions(ControllerIde controller, IView view) : base(controller, view)
        {
        }

 
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves the settings in system config for this app
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Save()
        {
            AppSettings.Default.Save();
        }
    }
}
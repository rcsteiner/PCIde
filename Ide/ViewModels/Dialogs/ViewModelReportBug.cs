////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the view model report bug class
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

using System.Windows;
using System.Windows.Media;
using Ide.Controller;
using Views;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// View model report bug. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelReportBug : ViewModelDialog
    {
        private string _description;
        private string _solutionPath;
        private ImageSource _screenShot;

        /// <summary>
        /// Gets or sets the screen shot.
        /// </summary>
        public ImageSource ScreenShot
        {
            get { return _screenShot; }
            set { _screenShot = value; RaisePropertyChanged("ScreenShot");}
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { if (_description != value){ _description = value; RaisePropertyChanged("Description");} }
        }

        /// <summary>
        /// Gets or sets the full pathname of the solution file.
        /// </summary>
        public string SolutionPath
        {
            get { return _solutionPath; }
            set { if (_solutionPath!=value) {_solutionPath = value; RaisePropertyChanged("SolutionPath");} }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelReportBug() : this(null,null,".\\", null)
        {
            _screenShot = IconUtil.GetImage("pc.ico");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        /// <param name="view">         The description. </param>
        /// <param name="solutionPath"> Full pathname of the solution file. </param>
        /// <param name="screenShot">   (Optional) The screen shot. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelReportBug(ControllerIde controller, IView view, string solutionPath, ImageSource screenShot=null) : base(controller,view)
        {
            Title = "Report a bug on Version " + Controller.AppVersion;
            if (screenShot == null)
            {
                screenShot = Clipboard.GetImage();
            }
            if (screenShot == null)
            {
                screenShot = controller.CreateScreenShotToClipboard();
            }
            _screenShot   = screenShot;
            _solutionPath = solutionPath??controller.VMWorkspace.CurrentSolution.FullPath;
        }
    }
}
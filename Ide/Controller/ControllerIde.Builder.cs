////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the controller ide. builder class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/10/2015   rcs     Initial Implementation
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
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Ide.Services;
using ViewModels;
using Views;
using ZCore;

namespace Ide.Controller
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Handles the compiler and code.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial  class   ControllerIde
    {
        private readonly   ServiceCompiler _serviceCompiler;
        private object                     _waitForBuild = new object();
        private Thread                     _buildThread;


        /// <summary>
        /// Gets or sets the error counter.
        /// </summary>
        public ErrorCounter ErrorCounter { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the compile ok.
        /// </summary>
        public bool CompileOk { get { return IsCompiled && ErrorCounter != null && ErrorCounter.ErrorCount < 1; } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is compiled.
        /// </summary>
        public bool IsCompiled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the compiling.
        /// </summary>
        public bool Compiling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [write list file].
        /// </summary>
        public bool WriteListFile { get; set; }

        /// <summary>
        /// Gets or sets the last vm compiled
        /// </summary>
        public ViewModelFileEdit LastVM { get; set; }

        /// <summary>
        /// Gets the Compiler service.
        /// </summary>
        public ServiceCompiler CompilerService { get { return _serviceCompiler; } }

      
        #region Build Support Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Builds the current selected project
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Build()
        {
            //TODO make this write list file a option in ui
            WriteListFile = true;
            Compiling     = true;
            _buildThread  = new Thread(TaskBuildStart);
            LastVM        = VMActiveDocument;
     
            AppCommand.SaveAll.Execute(null, null);
            ErrorCounter.Reset();
            VMActiveDocument.ClearMarkers();
            OutputClear();
       
            _buildThread.Start();
            _buildThread.Join(1000);
            Compiling = false;
            IsCompiled = ErrorCounter.Total == 0;

            if (AppSettings.Default.AutoRun && IsCompiled)
            {
                RunStart();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Execute Build on separate thread
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TaskBuildStart()
        {
            CompilerService.Compile(VMActiveDocument.FullPath, WriteListFile);
            _buildThread   = null;
            BuildRefresh();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Refresh after build to show markers
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void BuildRefresh()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,new Action(() => { SendRefresh(); }));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sent refresh to last VM
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SendRefresh()
        {
            if (LastVM != null)
            {
                LastVM.Send("refresh", null);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Builds all the projects in the solution
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void BuildAll()
        {
            Compiling = true;
            AppCommand.SaveAll.Execute(null, null);
            ErrorCounter.Reset();
          
            // clear all markers etc
            foreach (var vm in Files)
            {
                vm.ClearMarkers();
            }

            _buildThread = new Thread( BuildAllStart );
            _buildThread.Start();
           
            Compiling  = false;
            LastVM     = VMActiveDocument;
            BuildRefresh();
            IsCompiled = ErrorCounter.Total == 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Builds all start.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BuildAllStart()
        {
            foreach (var vm in Files)
            {
                if (vm.FilePath.Extension == Constants.PC_EXT)
                {
                    CompilerService.Compile(vm.FullPath, true);
                }
            }
            _buildThread = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Stops the current build.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void BuildStop()
        {
            Compiling  = false;
            IsCompiled = false;
            if (_buildThread != null)
            {
                _buildThread.Abort();
                _buildThread = null;
            }
        }

        #endregion
    }
}

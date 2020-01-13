////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the service pc class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/19/2015   rcs     Initial Implementation
//  =====================================================================================================
//
// Copyright:
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
using System.Windows;
using System.Windows.Threading;
using Ide.Controller;
using ZCore;
using pc;
using Views;

namespace Ide.Services
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Pseudo code compiler service provides both incremental and compile services. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ServiceCompiler : ServiceBase
    {
        private Compiler     _compiler;
        private PUserOutput  _userOutput;
        private PErrorOutput _errorOutput;
        private ServiceUser  _userInput;

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public PContext Context { get; set; }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ServiceCompiler(ControllerIde controller) : base(controller)
        {
            Context                = new PContext(controller.ErrorCounter);
            _userInput             = controller.User;
            _userOutput            = new PUserOutput(Context);
            _errorOutput           = new PErrorOutput( Context );
            _compiler              = new Compiler( Context );
            Context.Reporter      += Report;
            Context.NoSet          = ControllerIde.AppSettings.NoSet;
            Context.MainWindow     = controller.VMain;
            Context.ElevatorWindow = new ViewElevator();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Reports.
        /// </summary>
        /// <param name="level">    The level. </param>
        /// <param name="filepath"> The filepath. </param>
        /// <param name="linenum">  The linenum. </param>
        /// <param name="column">   The column. </param>
        /// <param name="offset">   The offset. </param>
        /// <param name="length">   The length. </param>
        /// <param name="line">     The line. </param>
        /// <param name="errormsg"> The errormsg. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Report(ErrorLevel level, string filepath, int linenum, int column, int offset, int length, string line, string errormsg)
        {
            var error = new Error(level,filepath,linenum,column,offset,errormsg,length);
            MarkError(error);
            Controller.ErrorCounter.Record(error);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compiles the given file.
        /// </summary>
        /// <param name="filePath">         Full pathname of the file. </param>
        /// <param name="writeListFile">    true to write list file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Compile(string filePath, bool writeListFile)
        {
            try
            {
                Context.ClearFiles();
                _compiler.Compile(filePath,writeListFile);
                Controller.VMOutput.WriteLine(string.Format( "Finished - '{0}'  {1}", FileUtil.GetFileNameAndExtension( filePath ), Context.ErrorCounter.SummaryString()));

            }
            catch (Exception e)
            {
                Context.Root.RuntimeFatal(0, 1, -1, "Compiler exception.  Please fill out bug report!" + e);
            }
        }


        #region Update Document Views with errors

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Send error messages to the view editors to mark up the document.  Call this method to clear old errors too.
        /// Send messages to the view to update/remove markers  
        /// 
        /// </summary>
        /// <param name="error">    The error. </param>
        /// <param name="remove">   (optional) true to remove. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MarkError(Error error, bool remove = false)
        {
            // check if on the UI Thread and dispatch if necessary

            Application.Current.Dispatcher.BeginInvoke( DispatcherPriority.ApplicationIdle, new Action( () =>
            {
                var vm = Controller.GetViewModelFileEdit(error.FilePath.FullPath);

                if (vm != null)
                {
                    vm.AddMarker(error);
                }
            }
            ));
            if (error.Level == ErrorLevel.FATAL)
            {
                Controller.VMOutput.WriteLine("\n\r!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Controller.VMOutput.WriteLine(string.Format("!!  Fatal Error line {0} \r\n!! {1}", error.LineNumber,error.Description));
                Controller.VMOutput.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }
        }

        #endregion
    }
}
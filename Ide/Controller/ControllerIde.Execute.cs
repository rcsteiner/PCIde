////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the controller ide class
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
using System.Windows.Input;
using ViewModels;
using ZCore;

namespace Ide.Controller
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Controller ide. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class ControllerIde : ObservableObject
    {

        #region File Command Support

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Export text to html can execute.
        /// </summary>
        /// <param name="s">    The s. </param>
        /// <param name="e">    Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileExportTextToHtmlCanExecute(object s, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VMActiveDocument != null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Export text to html execute.
        /// </summary>
        /// <param name="s">    The s. </param>
        /// <param name="e">    Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileExportTextToHtmlExecute(object s, ExecutedRoutedEventArgs e)
        {
            FileExportTextToHtml();
             e.Handled = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves all execute.
        /// </summary>
        /// <param name="s">    The s. </param>
        /// <param name="e">    Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileSaveAllExecute(object s, ExecutedRoutedEventArgs e)
        {
            FileSaveAll();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves a can execute.
        /// </summary>
        /// <param name="s">    The s. </param>
        /// <param name="e">    Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileSaveCanExecute(object s, CanExecuteRoutedEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;

                if (VMActiveDocument != null)
                {
                    e.CanExecute = VMActiveDocument.CanSave();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File save execute.
        /// </summary>
        /// <param name="s">    The s. </param>
        /// <param name="e">    Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileSaveExecute(object s, ExecutedRoutedEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;
            }
            FileSave();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves as can execute.
        /// </summary>
        /// <param name="s">    The s. </param>
        /// <param name="e">    Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileSaveAsCanExecute(object s, CanExecuteRoutedEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;
                e.CanExecute = (VMActiveDocument != null) && VMActiveDocument.CanSaveAs();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves as execute.
        /// </summary>
        /// <param name="s">    The s. </param>
        /// <param name="e">    Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileSaveAsExecute(object s, ExecutedRoutedEventArgs e)
        {
            try
            {
                e.Handled = true;
                FileSaveAs();
            }
            catch (Exception exp)
            {
                UserMessage.ShowException( exp, "Save" );
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Closes a file can execute.
        /// </summary>
        /// <param name="s">    The s. </param>
        /// <param name="e">    Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileCloseCanExecute(object s, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    e.Handled = true;
                    e.CanExecute = false;

                    ViewModelFileEdit f;

                    e.Handled = true;
                    f = e.Parameter as ViewModelFileEdit;

                    if (f != null)
                    {
                        e.CanExecute = f.CanClose();
                    }
                    else
                    {
                        if (VMActiveDocument != null)
                        {
                            e.CanExecute = VMActiveDocument.CanClose();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Closes a file execute.
        ///  Close Document command
        /// Closes the FileViewModel document supplied in e.parameter
        /// or the Active document
        /// </summary>
        /// <param name="s">    The s. </param>
        /// <param name="e">    Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileCloseExecute(object s, ExecutedRoutedEventArgs e)
        {
            try
            {
                ViewModelFileEdit f = null;

                if (e != null)
                {
                    e.Handled = true;
                    f = e.Parameter as ViewModelFileEdit;
                }

                if (f != null)
                {
                    FileClose( f );
                }
                else
                {
                    if (VMActiveDocument != null)
                    {
                        FileClose( VMActiveDocument );
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Queries if we can file open.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileOpenCanExecute(object sender, CanExecuteRoutedEventArgs canArgs)
        {
            canArgs.CanExecute = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the open action.
        /// </summary>
        /// <param name="sender">                   Source of the event. </param>
        /// <param name="executedRoutedEventArgs">  Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileOpenExecute(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            FileOpen();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the new action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileNewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (VMWorkspace != null)
            {
                e.CanExecute = VMWorkspace.CurrentFolder() != null;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the new action.
        /// </summary>
        /// <param name="sender">                   Source of the event. </param>
        /// <param name="executedRoutedEventArgs">  Executed routed event information. </param>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileNewExecute(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            FileNew();
        }


        #endregion

        #region Build Command Support

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Builds a can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BuildCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (VMActiveDocument != null)
            {
                e.CanExecute = !Compiling && VMActiveDocument.FilePath.Extension == Constants.PC_EXT;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Builds an execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BuildExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Build();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Builds all can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BuildAllCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Compiling && Files.Count > 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Builds all execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BuildAllExecute(object sender, ExecutedRoutedEventArgs e)
        {
            BuildAll();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Builds a stop can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BuildStopCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Compiling;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Builds a stop execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BuildStopExecute(object sender, ExecutedRoutedEventArgs e)
        {
            BuildStop();
        }

        #endregion

        #region Run Command Support

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the can execute operation.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RunCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CompileOk && !Running && !Debugging && VMActiveDocument == LastVM;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the execute operation.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RunExecute(object sender, ExecutedRoutedEventArgs e)
        {
            RunStart();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DebugStepInto go can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugRunCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CompileOk && Debugging ;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DebugStepInto go execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugRunExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DebugRun();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DebugStepInto can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugStepInfoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CompileOk && Debugging;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DebugStepInto execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugStepIntoExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DebugStepInto();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DebugStepInto return can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugStepReturnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =  Debugging;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DebugStepInto return execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugStepReturnExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DebugStepReturn();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DebugStepInto over execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugStepOverExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DebugStepOver();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DebugStepInto over can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugStepOverCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Debugging;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Stop execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RunStopExecute(object sender, ExecutedRoutedEventArgs e)
        {
            RunStop("Stop Requested by user");
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Stop can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RunStopCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Running || Debugging;
        }


        #endregion

        #region Debug Command Support

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug add execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugAddExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DebugAddBreakpoint();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug remove execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugRemoveExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DebugRemoveBreakpoint();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debu clear execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebuClearExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DebugClearBreakpoints();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug enable execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugEnableExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DebugEnableBreakpoint();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug disable execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugDisableExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DebugDisableBreakpoint();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug breakpoint can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugBreakpointCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            SetCurrentBookmark();
            e.CanExecute = VMActiveDocument != null && CurrentBookmark == null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debu breakpoint exist can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugBreakpointExistCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            SetCurrentBookmark();
            e.CanExecute = VMActiveDocument != null && CurrentBookmark != null;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug can execute.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CompileOk && !Running && VMActiveDocument != null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug pause can execute.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugPauseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Debugging;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DebugStart();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug pause execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugPauseExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DebugPause();
        }

        #endregion

        #region Workspace Command Support

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Solution close can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void WorkspaceCloseCanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VMWorkspace != null && VMWorkspace.WorkspaceIsLoaded;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Solution close execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WorkspaceCloseExecute(object sender, ExecutedRoutedEventArgs e)
        {
            WorkspaceClose();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File export solution to zip can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void WorkspaceExportToZipCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VMWorkspace != null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File export solution to zip execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void WorkspaceExportToZipExecute(object sender, ExecutedRoutedEventArgs e)
        {
            WorkspaceExportToZip();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the solution can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void WorkspaceShowCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VMWorkspace != null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Executes the show solution action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WorkspaceShowExecute(object sender, ExecutedRoutedEventArgs e)
        {
            WorkspaceShow();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Solution open execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WorkspaceOpenExecute(object sender, ExecutedRoutedEventArgs e)
        {
            WorkspaceOpen();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Solution new execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WorkspaceNewExecute(object sender, ExecutedRoutedEventArgs e)
        {
            WorkspaceNew();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Workspace refresh execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void WorkspaceRefreshExecute(object sender, ExecutedRoutedEventArgs e)
        {
            WorkspaceRefresh();
        }

        #endregion

        #region Project Commands

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Project new execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ProjectNewExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ProjectNew();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Project new can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ProjectNewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VMWorkspace != null && VMWorkspace.WorkspaceIsLoaded;
        }


        #endregion

        #region Folder Commands

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Folder new execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FolderNewExecute(object sender, ExecutedRoutedEventArgs e)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Folder new can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FolderNewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VMWorkspace != null && VMWorkspace.WorkspaceIsLoaded;
        }

        #endregion

        #region Tools Command

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clear output window execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ClearOutputExecute(object sender, ExecutedRoutedEventArgs e)
        {
            OutputClear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Application properties command executed.
        /// </summary>
        /// <param name="sender">                   Source of the event. </param>
        /// <param name="executedRoutedEventArgs">  Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WorkspaceOptionsExecute(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            executedRoutedEventArgs.Handled = ShowWorkspaceOptions();
        }


        #endregion

        #region Application Help Command


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Screen shot execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ScreenShotExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CreateScreenShotToClipboard();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Help report execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void HelpReportExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ReportBug();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Help contents execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void HelpContentsExecute(object sender, ExecutedRoutedEventArgs e)
        {
            HelpContents();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Application about command executed.
        /// </summary>
        /// <param name="sender">                   Source of the event. </param>
        /// <param name="executedRoutedEventArgs">  Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AppAboutExecute(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            HelpAbout();
            executedRoutedEventArgs.Handled = true;

        }


        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Browse url.
        /// </summary>
        /// <param name="sender">                   Source of the event. </param>
        /// <param name="executedRoutedEventArgs">  Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void BrowseURLExecute(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            BrowseURL( "http://google.com" );
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Edit comment block execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void EditCommentBlockExecute(object sender, ExecutedRoutedEventArgs e)
        {
            EditInsertCommentBlock();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Edit record and clear execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void EditRecordAndClearExecute(object sender, ExecutedRoutedEventArgs e)
        {
            EditRecordAndClear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Edit playback one line execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void EditPlaybackOneLineExecute(object sender, ExecutedRoutedEventArgs e)
        {
            EditPlaybackOneLine();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Have active document can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void HaveActiveDocumentCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VMActiveDocument != null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Resets a view execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ResetViewExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ResetView();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Exit can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ExitCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanShutdown();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Exit execute.
        /// </summary>
        /// <param name="sender">                   Source of the event. </param>
        /// <param name="executedRoutedEventArgs">  Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ExitExecute(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Shutdown();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Queries if we can shutdown.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CanShutdown()
        {
            // Check if conditions within the WorkspaceViewModel are suitable to close the application
            // eg.: Prompt to Cancel long running background tasks such as Search - Replace in Files (if any)

            return !_shutDownInProgress;
        }

        #region Application Settings Command

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Application program settings command executed.
        /// </summary>
        /// <param name="sender">                   Source of the event. </param>
        /// <param name="executedRoutedEventArgs">  Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AppProgramSettingsExecute(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            //TODO: add settings dialog for the application, let controller 
            //try
            //{
            //    // Initialize view model for editing settings
            //    ConfigViewModel dlgVM = new ConfigViewModel();
            //    dlgVM.LoadOptionsFromModel(this.mSettingsManager.SettingData);

            //    // Create dialog and attach viewmodel to view datacontext
            //    Window dlg = ViewSelector.GetDialogView(dlgVM, Application.Current.ViewMainWindow);

            //    dlg.ShowDialog();

            //    if (dlgVM.WindowCloseResult == true)
            //    {
            //        dlgVM.SaveOptionsToModel(this.mSettingsManager.SettingData);

            //        if (this.mSettingsManager.SettingData.IsDirty == true)
            //        {
            //            this.mSettingsManager.SaveOptions(this.mAppCore.DirFileAppSettingsData,
            //                this.mSettingsManager.SettingData);
            //        }
            //    }
            //}
            //catch (Exception exp)
            //{
            //}
        }

        #endregion
    }

}
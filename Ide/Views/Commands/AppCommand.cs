////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the application command class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/1/2015   rcs     Initial Implementation
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

using System.Windows.Input;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Application commands used in this App
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class AppCommand
    {
        #region Build Commands

        private static RoutedUICommand _build;
        private static RoutedUICommand _buildAll;
        private static RoutedUICommand _buildStop;
        
        /// <summary>
        /// Command for build.
        /// </summary>
        public static RoutedUICommand Build
        {
            get { return _build; }
        }

        /// <summary>
        /// Command for build.
        /// </summary>
        public static RoutedUICommand BuildAll
        {
            get { return _buildAll; }
        }

        /// <summary>
        /// Command for build.
        /// </summary>
        public static RoutedUICommand BuildStop
        {
            get { return _buildStop; }
        }

        #endregion

        #region Breakpoint Commands

        private static RoutedUICommand _debugAddBreakpoint;
        private static RoutedUICommand _debugRemoveBreakpoint;
        private static RoutedUICommand _debugDisableBreakpoint;
        private static RoutedUICommand _debugEnableBreakpoint;
        private static RoutedUICommand _debugClearBreakpoint;

        /// <summary>
        /// Command for debug clear all breakpoint
        /// </summary>
        public static RoutedUICommand DebugClearBreakpoint {get { return _debugClearBreakpoint; }}

        /// <summary>
        /// Command for debug add breakpoint to current line
        /// </summary>
        public static RoutedUICommand DebugAddBreakpoint {get { return _debugAddBreakpoint; }}

        /// <summary>
        /// Command for debug remove breakpoint
        /// </summary>
        public static RoutedUICommand DebugRemoveBreakpoint {get { return _debugRemoveBreakpoint; }}

        /// <summary>
        /// Command for debug.
        /// </summary>
        public static RoutedUICommand DebugEnableBreakpoint {get { return _debugEnableBreakpoint; }}

        /// <summary>
        /// Command for debug.
        /// </summary>
        public static RoutedUICommand DebugDisableBreakpoint {get { return _debugDisableBreakpoint; }}

        #endregion

        #region Debug Commands

        private static RoutedUICommand _debug;
        private static RoutedUICommand _debugPause;
        private static RoutedUICommand _debugStepInto;
        private static RoutedUICommand _debugStepOver;
        private static RoutedUICommand _debugStepReturn;
        private static RoutedUICommand _debugRun;

        private static RoutedUICommand _run;
        private static RoutedUICommand _stop;

        /// <summary>
        /// Command for Run.
        /// </summary>
        public static RoutedUICommand Run { get { return _run; } }

        /// <summary>
        /// Command for Stop.
        /// </summary>
        public static RoutedUICommand Stop { get { return _stop; } }

        /// <summary>
        /// Command for debug.
        /// </summary>
        public static RoutedUICommand Debug {get { return _debug; }}

        /// <summary>
        /// Command for debug pause running.
        /// </summary>
        public static RoutedUICommand DebugPause {get { return _debugPause; }}
    
        /// <summary>
        /// Command for debug step.
        /// </summary>
        public static RoutedUICommand DebugStepInto { get { return _debugStepInto; } }

        /// <summary>
        /// Command for debug step over.
        /// </summary>
        public static RoutedUICommand DebugStepOver { get { return _debugStepOver; } }

        /// <summary>
        /// Command for debug step return.
        /// </summary>
        public static RoutedUICommand DebugStepReturn { get { return _debugStepReturn; } }

        /// <summary>
        /// Command for debug step go (run).
        /// </summary>
        public static RoutedUICommand DebugRun { get { return _debugRun; } }

        #endregion

        #region Workspace Commands
        private static RoutedUICommand _workspaceClose;
        private static RoutedUICommand _workspaceNew;
        private static RoutedUICommand _workspaceOpen;
        private static RoutedUICommand _workspaceRefresh;
        private static RoutedUICommand _workspaceOptions;

        /// <summary>
        /// Gets the select solution file.
        /// </summary>
        public static RoutedUICommand WorkspaceOpen
        {
            get { return _workspaceOpen; }
        }


        /// <summary>
        /// Gets the select solution file.
        /// </summary>
        public static RoutedUICommand WorkspaceClose
        {
            get { return _workspaceClose; }
        }

        /// <summary>
        /// Crfeate a new solution.
        /// </summary>
        public static RoutedUICommand WorkspaceNew
        {
            get { return _workspaceNew; }
        }

        /// <summary>
        /// Crfeate a new solution.
        /// </summary>
        public static RoutedUICommand WorkspaceRefresh
        {
            get { return _workspaceRefresh; }
        }

        /// <summary>
        /// Crfeate a new solution.
        /// </summary>
        public static RoutedUICommand WorkspaceOptions
        {
            get { return _workspaceOptions; }
        }

        #endregion

        #region View Commands
        private static RoutedUICommand _showStartPage;
        private static RoutedUICommand _showWorkspace;
        private static RoutedUICommand _showErrorList;
        private static RoutedUICommand _showOutput;
        private static RoutedUICommand _showElevator;
        private static RoutedUICommand _showStats;
        private static RoutedUICommand _showChart;
        private static RoutedUICommand _showDebugVariables;
        private static RoutedUICommand _showDebugStack;
        private static RoutedUICommand _resetView;
        private static RoutedUICommand _clearOutput;

        /// <summary>
        /// Gets the show solution.
        /// </summary>
        public static RoutedUICommand ShowWorkspace {get { return _showWorkspace; }}

        /// <summary>
        /// Gets the show error list.
        /// </summary>
        public static RoutedUICommand ShowErrorList {get { return _showErrorList; }}

        /// <summary>
        /// Gets the show output page.
        /// </summary>
        public static RoutedUICommand ShowOutput {get { return _showOutput; }}

        /// <summary>
        /// Gets the show output page.
        /// </summary>
        public static RoutedUICommand ShowElevator { get { return _showElevator; } }
        /// <summary>
        /// Gets the show output page.
        /// </summary>
        public static RoutedUICommand ShowStats { get { return _showStats; } }
        /// <summary>
        /// Gets the show output page.
        /// </summary>
        public static RoutedUICommand ShowChart { get { return _showChart; } }

        /// <summary>
        /// Gets the show output page.
        /// </summary>
        public static RoutedUICommand ShowDebugVariables {get { return _showDebugVariables; }}

        /// <summary>
        /// Gets the show output page.
        /// </summary>
        public static RoutedUICommand ShowDebugStack {get { return _showDebugStack; }}

        /// <summary>
        /// Gets the show output page.
        /// </summary>
        public static RoutedUICommand ResetView {get { return _resetView; }}

        /// <summary>
        /// Gets the show start page.
        /// </summary>
        public static RoutedUICommand ShowStartPage {get { return _showStartPage; }}

        /// <summary>
        /// Clear the output window
        /// </summary>
        public static RoutedUICommand ClearOutput { get { return _clearOutput; } }

        #endregion

        #region Help commands

        private static RoutedUICommand _helpContents;
        private static RoutedUICommand _helpAbout;
        private static RoutedUICommand _helpReport;
        private static RoutedUICommand _screenShot;

        /// <summary>
        /// Create a screenshot
        /// </summary>
        public static RoutedUICommand ScreenShot { get { return _screenShot; } }

        /// <summary>
        /// Gets the about.
        /// </summary>
        public static RoutedUICommand HelpAbout
        {
            get { return _helpAbout; }
        }

        /// <summary>
        /// Gets the help contents.
        /// </summary>
        public static RoutedUICommand HelpContents
        {
            get { return _helpContents; }
        }

        /// <summary>
        /// Gets the help contents.
        /// </summary>
        public static RoutedUICommand HelpReport
        {
            get { return _helpReport; }
        }

        #endregion

        #region File Commands
        private static RoutedUICommand _exportTextToHtml;
        private static RoutedUICommand _exportWorkspaceToZip;

        private static RoutedUICommand _exit;
        private static RoutedUICommand _saveAll;
        private static RoutedUICommand _rename;
        private static RoutedUICommand _fileClose;
        private static RoutedUICommand _fileOpen;
        private static RoutedUICommand _fileNew;

        private static RoutedUICommand _projectNew;
        private static RoutedUICommand _projectDelete;
        private static RoutedUICommand _folderNew;


        /// <summary>
        /// Gets the rename
        /// </summary>
        public static RoutedUICommand Rename { get { return _rename; } }

        /// <summary>
        /// Exit command
        /// </summary>
        public static RoutedUICommand Exit { get { return _exit; } }

        /// <summary>
        ///     Execute a command to save all edited files and current program settings
        /// </summary>
        public static RoutedUICommand SaveAll {get { return _saveAll; }}

        /// <summary>
        ///     Execute a command to export the currently loaded and highlighted text (XML, C# ...)
        ///     into an HTML data format (*.htm, *.html ...)
        /// </summary>
        public static RoutedUICommand ExportTextToHTML {get { return _exportTextToHtml; }}

        /// <summary>
        ///     Export workspace to a zip file
        /// </summary>
        public static RoutedUICommand ExportWorkspaceToZip {get { return _exportWorkspaceToZip; }}


        /// <summary>
        /// Execute file open command (without user interaction)
        /// </summary>
        public static RoutedUICommand FileOpen {get { return _fileOpen; }}

        /// <summary>
        /// Gets the close file.
        /// </summary>
        public static RoutedUICommand FileClose {get { return _fileClose; }}

        /// <summary>
        /// Command for new folder.
        /// </summary>
        public static RoutedUICommand FolderNew { get { return _folderNew; } }


        /// <summary>
        /// Command for new file.
        /// </summary>
        public static RoutedUICommand FileNew { get { return _fileNew; } }

        /// <summary>
        /// Command for new project.
        /// </summary>
        public static RoutedUICommand ProjectNew { get { return _projectNew; } }


        #endregion

        #region Edit Commands
        private static RoutedUICommand _editRecordAndClear;
        private static RoutedUICommand _editPlayOneLine;
        private static RoutedUICommand _editSpellCheck;
        private static RoutedUICommand _editCommentBlock;

        /// <summary>
        /// Gets the edit record and clear.
        /// </summary>
        public static RoutedUICommand EditRecordAndClear
        {
            get { return _editRecordAndClear; }
        }
        /// <summary>
        /// Gets the edit play one line.
        /// </summary>
        public static RoutedUICommand EditPlayOneLine
        {
            get { return _editPlayOneLine; }
        }

        /// <summary>
        /// Execute spellcheck
        /// </summary>
        public static RoutedUICommand EditSpellCheck
        {
            get { return _editSpellCheck; }
        }

        /// <summary>
        /// Execute comment block
        /// </summary>
        public static RoutedUICommand EditCommentBlock
        {
            get { return _editCommentBlock; }
        }


        #endregion

        #region Options Commands
        private static RoutedUICommand _programSettings;
        private static RoutedUICommand _browseURL;

        /// <summary>
        /// Gets the program settings.
        /// </summary>
        public static RoutedUICommand ProgramSettings {get { return _programSettings; }}

        /// <summary>
        /// Gets URL of the browse.
        /// </summary>
        public static RoutedUICommand BrowseURL {get { return _browseURL; }}

        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Define custom commands and their key gestures.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static AppCommand()
        {
            _browseURL                 = new RoutedUICommand("Open URL ..."                        , "OpenURL", typeof (AppCommand));
            _showStartPage             = new RoutedUICommand("Show Start Page"                     , "ShowStartPage", typeof (AppCommand));
            _showWorkspace             = new RoutedUICommand("Show Workspace"                      , "ShowWorkspace", typeof (AppCommand));
            _showErrorList             = new RoutedUICommand("Show Error List"                     , "ShowErrorList", typeof (AppCommand));
            _showOutput                = new RoutedUICommand( "Show Output"                        , "ShowOutput", typeof( AppCommand ) );
            _showDebugVariables        = new RoutedUICommand( "Show Debug Variables"               , "ShowDebugVariables", typeof( AppCommand ) );
            _showDebugStack            = new RoutedUICommand( "Show Debug Stack"                   , "ShowDebugStack", typeof( AppCommand ) );
            _showElevator              = new RoutedUICommand( "Show Elevator simulator"            , "ShowElevator", typeof( AppCommand ) );
            _showStats                 = new RoutedUICommand( "Show Elevator Stats"                , "ShowStats", typeof( AppCommand ) );
            _showChart                 = new RoutedUICommand( "Show Chart"                         , "ShowChart", typeof( AppCommand ) );
            _resetView                 = new RoutedUICommand( "Reset View"                         , "Reset View", typeof( AppCommand ) );

            _fileOpen                  = new RoutedUICommand("Open ..."                            , "FileOpen", typeof (AppCommand));
            _fileNew                   = new RoutedUICommand( "Create a new  File"                 , "FileNew", typeof( AppCommand ) );


            _editSpellCheck            = new RoutedUICommand( "Spell Check"                        , "EditSpellCheck", typeof( AppCommand ) );

            _helpAbout                 = new RoutedUICommand("About this applicaation"             , "HelpAbout", typeof (AppCommand));
            _helpContents              = new RoutedUICommand( "Help Contents"                      , "HelpAContents", typeof( AppCommand ) );
            _helpReport                = new RoutedUICommand( "Help Report a problem"              , "HelpReport", typeof( AppCommand ) );
       
            _saveAll                   = new RoutedUICommand("Save All"                             , "SaveAll", typeof (AppCommand));
      
            _exportTextToHtml          = new RoutedUICommand("Export Current Document to HTML"     , "ExportTextToHTML",typeof (AppCommand));
            _exportWorkspaceToZip      = new RoutedUICommand("Export Solution to Zip"              , "ExportWorkspaceToZip",typeof (AppCommand));

            _folderNew                 = new RoutedUICommand( "Create a new  Folder"               , "FolderNew", typeof( AppCommand ) );
      
            _projectNew                = new RoutedUICommand( "Create a new  Project"              , "ProjectNew", typeof( AppCommand ) );
            _projectDelete             = new RoutedUICommand( "Delete the current Project"         , "ProjectDelete", typeof( AppCommand ) );

            _workspaceClose            = new RoutedUICommand( "Close Workspace"                     , "WorkspaceClose", typeof( AppCommand ) );
            _workspaceNew              = new RoutedUICommand( "Create a new  workspace"             , "WorkspaceNew", typeof( AppCommand ) );
            _workspaceOpen             = new RoutedUICommand( "Open an existing  workspace"         , "WorkspaceOpen", typeof( AppCommand ) );
            _workspaceRefresh          = new RoutedUICommand( "Refresh the current workspace"       , "WorkspaceRefresh", typeof( AppCommand ) );
            _workspaceOptions          = new RoutedUICommand( "Options for workspace"               , "WorkspaceOpitons", typeof( AppCommand ) );
            _programSettings           = new RoutedUICommand( "Edit or Review your program settings", "ProgramSettings", typeof( AppCommand ) );

 //           _errorSelected             = new RoutedUICommand( "Goto the error line in source code.", "ErrorSelected", typeof( AppCommand ) );
            _clearOutput               = new RoutedUICommand( "Clear Output Window"                , "ClearOutput", typeof( AppCommand ) );


            _debugAddBreakpoint     = new RoutedUICommand( "Add a breakpoint to the current line"       , "DebugAddBreakpoint", typeof( AppCommand ) );
            _debugClearBreakpoint   = new RoutedUICommand( "Clear all breakpoints"                      , "DebugClearBreakpoint", typeof( AppCommand ) );
            _debugRemoveBreakpoint  = new RoutedUICommand( "Remove a breakpoint from the current line"  , "DebugRemoveBreakpoint", typeof( AppCommand ) );
            _debugEnableBreakpoint  = new RoutedUICommand( "Enable this breakpoint on the current line" , "DebugEnabledBreakpoint", typeof( AppCommand ) );
            _debugDisableBreakpoint = new RoutedUICommand( "Disable this breakpoint on the current line", "DebugDisableBreakpoint", typeof( AppCommand ) );

            InputGestureCollection inputs;
           
         //   inputs                     = new InputGestureCollection { new KeyGesture( Key.G, ModifierKeys.Control, "Ctrl+G" ) };

            inputs                     = new InputGestureCollection { new KeyGesture( Key.F3, ModifierKeys.None, "F3" ) };
            _editCommentBlock          = new RoutedUICommand( "Comment Block", "EditCommentBlock", typeof( AppCommand ) , inputs);

            inputs                     = new InputGestureCollection { new KeyGesture( Key.R, ModifierKeys.Control, "Cntrl+R" ) };
            _editRecordAndClear        = new RoutedUICommand( "Record and Clear", "EdtiRecordAndClear", typeof( AppCommand ), inputs );
        
            inputs                     = new InputGestureCollection { new KeyGesture( Key.P, ModifierKeys.Control, "Cntrl+P" ) };
            _editPlayOneLine           = new RoutedUICommand( "Play one line", "EditPlayOneOne", typeof( AppCommand ), inputs );

            inputs                     = new InputGestureCollection {new KeyGesture(Key.F4, ModifierKeys.Alt, "Alt+F4")};
            _exit                      = new RoutedUICommand("Exit", "Exit", typeof (AppCommand), inputs);
           
            inputs                     = new InputGestureCollection {new KeyGesture(Key.F4, ModifierKeys.Control, "Ctrl+F4"),new KeyGesture(Key.W, ModifierKeys.Control, "Ctrl+W")};
            _fileClose                 = new RoutedUICommand("Close current document", "Close", typeof (AppCommand), inputs);

            inputs                     = new InputGestureCollection { new KeyGesture( Key.F2, ModifierKeys.None,"F2" ) };
            _rename                    = new RoutedUICommand( "Rename", "Rename", typeof( AppCommand ), inputs );

            inputs                     = new InputGestureCollection { new KeyGesture( Key.F5, ModifierKeys.Control, "Ctrl+F5" ) };
            _run                       = new RoutedUICommand( "Run the current program", "Run", typeof( AppCommand ), inputs );
            _stop                      = new RoutedUICommand( "Stop Execution", "Stop", typeof( AppCommand ));
            _debugPause                = new RoutedUICommand( "Debug pause Execution", "DebugPause", typeof( AppCommand ) );

            inputs                     = new InputGestureCollection { new KeyGesture( Key.F5, ModifierKeys.None, "F5" ) };
            _debug                     = new RoutedUICommand( "Debug start run", "Debug", typeof( AppCommand ), inputs );

            inputs                     = new InputGestureCollection { new KeyGesture( Key.F5, ModifierKeys.Shift, "Shift+F5" ) };
            _debugRun                  = new RoutedUICommand( "Debug to breakpoint", "DebugRun", typeof( AppCommand ), inputs );

            inputs                     = new InputGestureCollection { new KeyGesture( Key.F10, ModifierKeys.None, "F10" ) };
            _debugStepOver             = new RoutedUICommand( "Step over this statement", "DebugStepOver", typeof( AppCommand ), inputs );

            inputs                     = new InputGestureCollection { new KeyGesture( Key.F11, ModifierKeys.None, "F11" ) };
            _debugStepInto             = new RoutedUICommand( "Step Into into this statement", "DebugStepInto", typeof( AppCommand ), inputs );

            inputs                     = new InputGestureCollection { new KeyGesture( Key.F11, ModifierKeys.Shift, "Shift+F11" ) };
            _debugStepReturn           = new RoutedUICommand( "Return from this function", "DebugStepReturn", typeof( AppCommand ), inputs );


            inputs                     = new InputGestureCollection { new KeyGesture( Key.F6, ModifierKeys.None, "F6" ) };
            _build                     = new RoutedUICommand( "Build Current program", "Build", typeof( AppCommand ), inputs );

            inputs                     = new InputGestureCollection { new KeyGesture( Key.F6, ModifierKeys.Shift, "Shift+F6" ) };
            _buildAll                  = new RoutedUICommand( "Build All programs", "BuildAll", typeof( AppCommand ), inputs );

            _buildStop                 = new RoutedUICommand( "Build Stop compiling", "BuildStop", typeof( AppCommand ) );


            inputs                     = new InputGestureCollection { new KeyGesture( Key.F12, ModifierKeys.None, "F12" ) };
            _screenShot                = new RoutedUICommand( "Copy a screenshot to the clipboard", "ScreenShot", typeof( AppCommand ), inputs );

        }
    }

}
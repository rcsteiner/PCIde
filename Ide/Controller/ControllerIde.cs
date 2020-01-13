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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xaml.Schema;
using System.Xml.Schema;
using Help;
using Ide.Services;
using LineChartControl;
using LineCharts;
using ViewModels;
using Views;
using Views.About;
using Views.Editors;
using ZCore;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;

namespace Ide.Controller
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Controller ide.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class ControllerIde : ObservableObject
    {
        /// <summary>
        ///  Gets the application version.
        /// </summary>
        public string AppVersion { get { return BuildInfo.ConvertMSVersionToVersionInfo(Assembly.GetEntryAssembly().GetName().Version.ToString()); } }

        /// <summary>
        ///  Get application Version Short.
        /// </summary>
        public string AppVersionShort { get { return BuildInfo.ConvertMSVersionToVersionInfoShort(Assembly.GetEntryAssembly().GetName().Version.ToString()); } }

        /// <summary>
        ///  Gets or sets the default editor options.
        /// </summary>
        public EditorOptions DefaultEditorOptions { get; set; }

        /// <summary>
        ///  Gets or sets the error list.
        /// </summary>
        public ObservableCollection<Error> ErrorList { get; private set; }

        /// <summary>
        ///  The list of all the files that are being managed. This is bound to the document layout.
        /// </summary>
        public ObservableCollection<ViewModelFileEdit> Files
        {
            get { return _files; }
        }

        /// <summary>
        ///  Get/Set MRUWorkspaces.
        /// </summary>
        public MRUList MRUWorkspaces { get; set; }

        /// <summary>
        ///  Get/Set Output.
        /// </summary>
        public ObservableCollection<OutputItem> Output { get; private set; }

        /// <summary>
        ///  The current list of all tools views. Add a view to this list to get it added to the tools pane.
        /// </summary>
        public ObservableCollection<ViewModelTool> Tools
        {
            get { return _tools; }
        }

        /// <summary>
        ///  Gets or sets the active document.
        /// </summary>
        public ViewModelFileEdit VMActiveDocument
        {
            get { return _vmActiveDocument; }
            set
            {
                if (_vmActiveDocument != value)
                {
                    _vmActiveDocument = value;
                    RaisePropertyChanged("VMActiveDocument");
                    if (VMMain != null)
                    {
                        VMMain.ActiveDocumentChanged();
                    }
                    if (ActiveDocumentChanged != null)
                    {
                        ActiveDocumentChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        /// <summary>
        ///  Gets the Stats view model.
        /// </summary>
        public ViewModelStats VMStats
        {
            get { return _vmStats ?? (_vmStats = GetViewModelStats()); }
        }

        /// <summary>
        ///  Gets the Elevator view model.
        /// </summary>
        public ViewModelElevator VMElevator
        {
            get { return _vmElevator ?? (_vmElevator = GetViewModelElevator()); }
        }

        /// <summary>
        ///  Gets the chart view modle.
        /// </summary>
        public ViewModelChart VMChart
        {
            get { return _vmChart ?? (_vmChart = GetViewModelChart()); }
        }

        /// <summary>
        ///  The error list view.
        /// </summary>
        public ViewModelDebugStack VMDebugStack
        {
            get { return _vmDebugStack ?? (_vmDebugStack = GetViewModelDebugStack()); }
        }

        /// <summary>
        ///  The error list view.
        /// </summary>
        public ViewModelDebugVariables VMDebugVariables
        {
            get { return _vmDebugVariables ?? (_vmDebugVariables = GetViewModelDebugVariables()); }
        }

        /// <summary>
        ///  The error list view.
        /// </summary>
        public ViewModelErrorList VMErrorList
        {
            get { return _vmErrorList ?? (_vmErrorList = GetViewModelErrorList()); }
        }

        /// <summary>
        ///  Gets  main view model
        /// </summary>
        public ViewModelMain VMMain { get; private set; }

        /// <summary>
        ///  Gets the output.
        /// </summary>
        public ViewModelOutput VMOutput
        {
            get { return _vmOutput ?? (_vmOutput = GetViewModelOutput()); }
        }

        /// <summary>
        ///  Gets the vm workspace options.
        /// </summary>
        public ViewModelWorkspaceOptions VMWorkspaceOptions
        {
            get { return _vmWorkspaceOptions ?? (_vmWorkspaceOptions = new ViewModelWorkspaceOptions()); }
        }

        /// <summary>
        ///  The  edit Listeners.
        /// </summary>
        private readonly Dictionary<string, IMessageListener> _editListeners;

        /// <summary>
        ///  The  files.
        /// </summary>
        private ObservableCollection<ViewModelFileEdit> _files;

        /// <summary>
        ///  The  output Synch.
        /// </summary>
        private object _outputSynch = new object();

        /// <summary>
        ///  The  playback Buffer.
        /// </summary>
        private Stack<string> _playbackBuffer = new Stack<string>();

        /// <summary>
        ///  The  service User.
        /// </summary>
        private ServiceUser _serviceUser;

        /// <summary>
        ///  The  shut Down In Progress.
        /// </summary>
        private bool _shutDownInProgress;

        /// <summary>
        ///  The  tools.
        /// </summary>
        private ObservableCollection<ViewModelTool> _tools;

        /// <summary>
        ///  The  view model Active Document.
        /// </summary>
        private ViewModelFileEdit _vmActiveDocument;

        /// <summary>
        ///  The  v Main.
        /// </summary>
        public  ViewMain VMain;

        /// <summary>
        ///  The  view model Chart.
        /// </summary>
        private ViewModelChart _vmChart;
     
        /// <summary>
        ///  The  view model Chart.
        /// </summary>
        private ViewModelElevator _vmElevator;

        /// <summary>
        ///  The  view model Debug Stack.
        /// </summary>
        private ViewModelDebugStack _vmDebugStack;

        /// <summary>
        ///  The  view model Debug Variables.
        /// </summary>
        private ViewModelDebugVariables _vmDebugVariables;

        /// <summary>
        ///  The  view model Error List.
        /// </summary>
        private ViewModelErrorList _vmErrorList;

        /// <summary>
        ///  The  view model Output.
        /// </summary>
        private ViewModelOutput _vmOutput;

        /// <summary>
        ///  The  view model Stats.
        /// </summary>
        private ViewModelStats _vmStats;

        /// <summary>
        ///  The  view model Workspace Options.
        /// </summary>
        private ViewModelWorkspaceOptions _vmWorkspaceOptions;


        /// <summary>
        ///  Occurs when [active document changed].
        /// </summary>
        public event EventHandler ActiveDocumentChanged;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor.
        ///  Create the error list
        ///  create the output list
        ///  Register special extensions.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ControllerIde()
        {
            _serviceFile     = new ServiceFile(this);
            _serviceCompiler = new ServiceCompiler(this);
            _serviceUser     = new ServiceUser(this);
            _editListeners   = new Dictionary<string, IMessageListener>(20);
            _tools           = new ObservableCollection<ViewModelTool>();
            _files           = new ObservableCollection<ViewModelFileEdit>();
            Output           = new ObservableCollection<OutputItem>();
            ErrorCounter     = new ErrorCounter();
            VariableList     = new ObservableCollection<ViewModelModule>();
            CallStack        = new ObservableCollection<string>();
            ErrorList        = ErrorCounter.ErrorList;
            MRUWorkspaces    = new MRUList("Workspace", this);
            SetupEditorOptions();
            _serviceCompiler.Context.Stats = VMStats;  // pre-create the VM for stats.

            RegisterFileIcons();
            RegisterLanguages();
            SetupCompiler();

            // bind it for multithreading
            BindingOperations.EnableCollectionSynchronization(Output, _outputSynch);
            watchTimer = DateTime.Now;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Adds a listener.
        /// </summary>
        /// <param name="fullPath"> Full pathname of the full file.</param>
        /// <returns>
        ///  A list of.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IMessageListener AddListener(string fullPath)
        {
            IMessageListener v;
            return _editListeners.TryGetValue(fullPath, out v) ? v : FindListener(fullPath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Browse url.
        /// </summary>
        /// <param name="urlPath"> Full pathname of the url file.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void BrowseURL(string urlPath)
        {
            Process.Start(new ProcessStartInfo(urlPath));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Creates a bitmap from visual.
        /// </summary>
        /// <param name="target">   Target for the.</param>
        /// <param name="fileName"> Filename of the file.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void CreateBitmapFromVisual(Visual target, string fileName)
        {
            if (target == null || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((Int32)bounds.Width, (Int32)bounds.Height, 96, 96,
                PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(target);
                context.DrawRectangle(visualBrush, null, new Rect(new Point(), bounds.Size));
            }

            renderTarget.Render(visual);
            PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTarget));
            using (Stream stm = File.Create(fileName))
            {
                bitmapEncoder.Save(stm);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Creates the screen shot to clipboard.
        /// </summary>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public RenderTargetBitmap CreateScreenShotToClipboard()
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)VMain.ActualWidth, (int)VMain.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(VMain);
            Clipboard.SetImage(bmp);
            return bmp;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Edit insert comment block.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void EditInsertCommentBlock()
        {
            var text = "/////////////////////////////////////////////////////////////////////////////////\r\n";
            VMActiveDocument.Insert(text);
            VMActiveDocument.Insert("// Summary\r\n");
            VMActiveDocument.Insert("// Parameters:\r\n");
            VMActiveDocument.Insert(text);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Edit insert comment block.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void EditPlaybackOneLine()
        {
            if (_playbackBuffer.Count > 0)
            {
                var text = _playbackBuffer.Pop();
                VMActiveDocument.Insert(text);
                VMActiveDocument.Insert("\n");
            }
            PreviewPlayback();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Edit insert comment block.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void EditRecordAndClear()
        {
            var text = VMActiveDocument.Text.Split('\n');
            _playbackBuffer.Clear();
            for (int t = text.Length - 1; t >= 0; t--)
            {
                _playbackBuffer.Push(text[t]);
            }
            PreviewPlayback();

            //Execute( ApplicationCommands.SelectAll );

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Error clear.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ErrorClear()
        {
            ErrorList.Clear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Error list selected error changed.  Show the file if possible.
        /// </summary>
        /// <param name="error"> The error.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ErrorListSelectedErrorChanged(Error error)
        {
            // extract the file path
            var path = error.FilePath.FullPath;
            var modelTreeFile = VMWorkspace.CurrentSolution.FindSolutionFileVM(path);
            if (modelTreeFile != null)
            {
                FileOpen(modelTreeFile.ModelFile);
            }
            var ve = AddListener(path);
            if (ve != null)
            {
                SendTo(ve, "gotoerr", error);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Error write.
        /// </summary>
        /// <param name="error"> The error.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ErrorWrite(Error error)
        {
            ErrorList.Add(error);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Gets a main view.
        ///  Bind a window to some commands to be executed by the viewmodel.
        /// </summary>
        /// <returns>
        ///  The main view.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ViewMain GetMainView()
        {
            var v = new ViewMain();
            var vm = new ViewModelMain(this, v);
            VMMain = vm;
            VMain = v;

            v.CommandBindings.Add(new CommandBinding(AppCommand.Exit                  , ExitExecute, ExitCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.FileNew               , FileNewExecute, FileNewCanExecute));
            v.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open         , FileOpenExecute, FileOpenCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.FileClose             , FileCloseExecute, FileCloseCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.FileOpen              , FileOpenExecute));
            v.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save         , FileSaveExecute, FileSaveCanExecute));
            v.CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs       , FileSaveAsExecute, FileSaveAsCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.SaveAll               , FileSaveAllExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ExportTextToHTML      , FileExportTextToHtmlExecute, FileExportTextToHtmlCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ExportWorkspaceToZip  , WorkspaceExportToZipExecute, WorkspaceExportToZipCanExecute));

            v.CommandBindings.Add(new CommandBinding(AppCommand.BrowseURL             , BrowseURLExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.EditCommentBlock      , EditCommentBlockExecute, HaveActiveDocumentCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.EditRecordAndClear    , EditRecordAndClearExecute, HaveActiveDocumentCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.EditPlayOneLine       , EditPlaybackOneLineExecute, HaveActiveDocumentCanExecute));

            //todo fix these to all be local
            v.CommandBindings.Add(new CommandBinding(AppCommand.ShowStartPage         , vm.ShowStartPageExecuted));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ShowErrorList         , vm.ShowErrorListExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ShowOutput            , vm.ShowOutputExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ShowElevator          , vm.ShowElevatorExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ShowStats             , vm.ShowStatsExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ShowChart             , vm.ShowChartExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ShowDebugVariables    , vm.ShowDebugVariablesExecute, vm.ShowDebugCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ShowDebugStack        , vm.ShowDebugStacktExecute, vm.ShowDebugCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ShowWorkspace         , WorkspaceShowExecute, WorkspaceShowCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ClearOutput           , ClearOutputExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ResetView             , ResetViewExecute));



            v.CommandBindings.Add(new CommandBinding(AppCommand.HelpAbout             , AppAboutExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.HelpContents          , HelpContentsExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.HelpReport            , HelpReportExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ScreenShot            , ScreenShotExecute));

            v.CommandBindings.Add(new CommandBinding(AppCommand.ProgramSettings       , AppProgramSettingsExecute));

            v.CommandBindings.Add(new CommandBinding(AppCommand.WorkspaceClose        , WorkspaceCloseExecute, WorkspaceCloseCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.WorkspaceNew          , WorkspaceNewExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.WorkspaceOpen         , WorkspaceOpenExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.WorkspaceRefresh      , WorkspaceRefreshExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.WorkspaceOptions      , WorkspaceOptionsExecute));

            v.CommandBindings.Add(new CommandBinding(AppCommand.FolderNew             , FolderNewExecute, FolderNewCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.ProjectNew            , ProjectNewExecute, ProjectNewCanExecute));

            v.CommandBindings.Add(new CommandBinding(AppCommand.Build                 , BuildExecute, BuildCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.BuildAll              , BuildAllExecute, BuildAllCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.BuildStop             , BuildStopExecute, BuildStopCanExecute));

            v.CommandBindings.Add(new CommandBinding(AppCommand.Run                   , RunExecute, RunCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.Stop                  , RunStopExecute, RunStopCanExecute));

            v.CommandBindings.Add(new CommandBinding(AppCommand.Debug                 , DebugExecute, DebugCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.DebugStepInto         , DebugStepIntoExecute, DebugStepInfoCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.DebugStepReturn       , DebugStepReturnExecute, DebugStepReturnCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.DebugStepOver         , DebugStepOverExecute, DebugStepOverCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.DebugRun              , DebugRunExecute, DebugRunCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.DebugPause            , DebugPauseExecute, DebugPauseCanExecute));

            v.CommandBindings.Add(new CommandBinding(AppCommand.DebugAddBreakpoint    , DebugAddExecute, DebugBreakpointCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.DebugRemoveBreakpoint , DebugRemoveExecute, DebugBreakpointExistCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.DebugClearBreakpoint  , DebuClearExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.DebugEnableBreakpoint , DebugEnableExecute, DebugBreakpointExistCanExecute));
            v.CommandBindings.Add(new CommandBinding(AppCommand.DebugDisableBreakpoint, DebugDisableExecute, DebugBreakpointExistCanExecute));


            return v;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Help about.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void HelpAbout()
        {
            try
            {
                var vm = new ViewModelAbout(this);
                var v = new ViewAbout();
                v.DataContext = vm;
                v.ShowInWindow(true, "HelpAbout Pseudo Code IDE");
            }
            catch (Exception)
            {
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Help contents.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void HelpContents()
        {
            var win = new HelpWindow();
            win.Owner = VMain;
            win.Title = "Help for Pseudo Code IDE";
            win.Show();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Show the building for elevator
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OutputBuilding()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { ShowElevator(true); }));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Output the Chart
        /// </summary>
        /// <param name="data">    The data.</param>
        /// <param name="title">   The title.</param>
        /// <param name="xlabels"> The xlabels.</param>
        /// <param name="ylabels"> The ylabels.</param>
        /// <param name="xtitle">  The xtitle.</param>
        /// <param name="ytitle">  The ytitle.</param>
        /// <param name="gridx">   True if gridx.</param>
        /// <param name="gridy">   True if gridy.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OutputChart(double[] data, string title, string[] xlabels, string[] ylabels, string xtitle, string ytitle, bool gridx, bool gridy)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { ShowChart(data, title, xlabels, ylabels, xtitle, ytitle, gridx, gridy); }));

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Output clear.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OutputClear()
        {
            if (VMOutput != null)
            {
                VMOutput.Clear();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Output write.
        /// </summary>
        /// <param name="text"> The text.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OutputWrite(string text)
        {
            VMOutput.Write(text);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Output write line
        /// </summary>
        /// <param name="text"> The text.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OutputWriteLine(string text)
        {
            VMOutput.WriteLine(text);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Preview playback.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void PreviewPlayback()
        {
            var msg = (_playbackBuffer.Count > 0) ? _playbackBuffer.Peek() : "";
            VMMain.Message = msg;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Register file icons for special file types not registered with system.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RegisterFileIcons()
        {
            IconUtil.MapImage("pcsmall", Constants.PC_EXT);
            IconUtil.MapImage("pclist", Constants.PCLIST_EXT);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Register languages.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RegisterLanguages()
        {
            IdeEditor.RegisterHighlighters();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Removes the listener described by filepath.
        /// </summary>
        /// <param name="filepath"> The filepath.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveListener(string filepath)
        {
            if (filepath != null)
            {
                _editListeners.Remove(filepath);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Report a bug
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ReportBug()
        {
            try
            {
                var dlg = new DialogReportBug();

                var vm = new ViewModelReportBug(this, dlg, null);
                //  dlg.DataContext = vm;

                dlg.ShowInWindow(true, "Report a bug!");
                if (vm.DialogResult)
                {
                    var version = AppVersion;
                    var screenshot = vm.ScreenShot;
                    var solution = vm.SolutionPath;
                    var time = DateTime.Now.ToLocalTime().ToString("g");
                    var errors = UserMessage.ErrorSummary + "\r\n" + ErrorCounter.ErrorListSummary;
                    var description = string.Format("Date={0}\r\nVersion={1}\r\nUserName={2}\r\nUserClass={3}\r\nDescription={4}\r\nError List:\r\n{5}",
                                                    time, version, AppSettings.Default.UserName, AppSettings.Default.UserClass, vm.Description, errors);
                    var solutionFolder = FileUtil.GetFolder(solution);
                    var bugFolder = solutionFolder + "BugReports\\";
                    var errorFile = FileUtil.FileCreateUnique(bugFolder, "Bug.txt");
                    var imageFile = FileUtil.FileCreateUnique(bugFolder, "BugImage.png");

                    // write image to disk in solution root  BugReports Folder
                    SaveToPng((BitmapSource)screenshot, imageFile);


                    // write error report to disk
                    FileUtil.FileWriteText(errorFile, description);

                    // compress solution
                    var target = WorkspaceExportToZip();

                    //todo: send message somewhere with target file...

                }
            }
            catch (Exception)
            {
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Resets a view.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ResetView()
        {
            string ws = null;
            if (VMWorkspace != null)
            {
                ws = VMWorkspace.FullPath;
                WorkspaceClose();
            }
            Tools.Clear();
            // save workspace
            VMMain.LoadDefaultLayout();

            if (ws != null)
            {
                WorkspaceOpen(ws);
            }
            //ShowOutput(true);
            //ShowErrorList(true);
            //_vmMain.ShowWorkspace = true;
            //_vmMain.ShowOutput = true;
            //_vmMain.ShowErrorList = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Saves source bitmap to file
        /// </summary>
        /// <param name="visual">   The visual.</param>
        /// <param name="filePath"> Filename of the file.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SaveToPng(BitmapSource visual, string filePath)
        {
            var encoder = new PngBitmapEncoder();
            BitmapFrame frame = BitmapFrame.Create(visual);
            encoder.Frames.Add(frame);

            using (var stream = FileUtil.FileCreate(filePath))
            {
                encoder.Save(stream);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Sets up the compiler.
        ///  1. Link to the the ErorrList to watch for errors on this file
        ///  2. Listen to a document change and send error messages
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetupCompiler()
        {
            MessegerRegister();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Sets up the editor options.
        ///  Load from the configuration
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetupEditorOptions()
        {
            //TODO: load and save editor options in the configuration file of the editor
            DefaultEditorOptions = new EditorOptions();
            DefaultEditorOptions.WordWrap = false;
            DefaultEditorOptions.FontSize = 18;
            DefaultEditorOptions.FontFamily = "Consolas";
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Shows the statistics page.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowStats(bool showStats)
        {
            var viewModel = VMStats;
            ShowToolView(showStats, viewModel);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Show the Building
        /// </summary>
        /// <param name="showElevator"> True if show Elevator.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowElevator(bool showElevator)
        {
            try
            {
                var viewModel = VMElevator;
                var v = (ViewElevator)this._serviceCompiler.Context.ElevatorWindow;
                //     VMElevator.BindView(this, v);
                VMElevator.MyCanvas = v.MyCanvas;
                ShowToolView(showElevator, viewModel);
                ShowStats(showElevator);
                // v.ShowAsWindow("Elevator");

                //  ShowToolView(true,VMElevator);
                //((ViewBase) v).Refresh();
                //    v.ShowInWindow(false, "Elevator");
                return;
            }
            catch (Exception)
            {
            }
            return;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Show the Chart
        /// </summary>
        /// <param name="showChart"> True if show Chart.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowChart(bool showChart)
        {
            try
            {

                var v = new ViewLineChart();



                var vm = VMChart;
                var c = v.MyLineChart;

                c.Title = "Chart Only";
                c.IsXGrid = true;
                c.IsYGrid = true;
                c.XLabel = "x label";
                c.YLabel = "y label";

                var dataSet = c.DataSet;
                vm.Clear();
                double[] data = new double[] {10, 20, 5, 15, 30};
                AddDataSet(vm, c, dataSet,data, "testing");

                //  vm.BindView(this, v);

                 ShowToolView(showChart,VMChart);
                //   v.Refresh();
               // v.ShowInWindow(true, "Line Chart");
                return;
            }
            catch (Exception)
            {
            }
            return;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Show the Chart
        /// </summary>
        /// <param name="data">    The data.</param>
        /// <param name="title">   The title.</param>
        /// <param name="xlabels"> The xlabels.</param>
        /// <param name="ylabels"> The ylabels.</param>
        /// <param name="xtitle">  The xtitle.</param>
        /// <param name="ytitle">  The ytitle.</param>
        /// <param name="gridx">   True if gridx.</param>
        /// <param name="gridy">   True if gridy.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ShowChart(double[] data, string title, string[] xlabels, string[] ylabels, string xtitle, string ytitle, bool gridx, bool gridy)
        {
            try
            {
               // VMChart.Init(data, title, xlabels, ylabels, xtitle, ytitle, gridx, gridy);

                var v = new ViewLineChart();



                var vm = VMChart;
                var c = v.MyLineChart;

                c.Title = title;
                c.IsXGrid = gridx;
                c.IsYGrid = gridy;
                c.XLabel = xtitle;
                c.YLabel = ytitle;

                var dataSet = c.DataSet;
                vm.Clear();
                AddDataSet(vm,c, dataSet,data,xtitle);

              //  vm.BindView(this, v);

               ShowToolView(true,VMChart);
             //   v.Refresh();
              //  v.ShowInWindow(true, "Line Chart");
                return;
            }
            catch (Exception)
            {
            }
            return;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Add the Data Set
        /// </summary>
        /// <param name="vm">      The virtual memory.</param>
        /// <param name="chart">   The chart.</param>
        /// <param name="dataset"> The dataset.</param>
        /// <param name="data">    The data.</param>
        /// <param name="xtitle">  The xtitle.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddDataSet(ViewModelChart vm, LineChartControlLib chart, DataCollection dataset, double[] data, string xtitle)
        {
            // Draw Sine curve:
            DataSeries ds          = new DataSeries();
            ds.LineColor           = Brushes.Blue;
            ds.LineThickness       = 1;
            ds.SeriesName          = xtitle;
            ds.Symbols.SymbolType  = SymbolTypeEnum.Circle;
            ds.Symbols.BorderColor = ds.LineColor;
            var xmin               = chart.XMin;
            var xmax               = chart.XMax;
            var ymin               = chart.YMin;
            var ymax               = chart.YMax;

            for (int i = 0; i < data.Length; i++)
            {
                
                double x = i;
                double y = data[i];
                if (ymin > y) ymin = y;
                if (ymax < y) ymax = y;
                if( xmax < x) xmax = x;
                ds.LineSeries.Points.Add(new Point(x, y));
            }
            chart.XMin  = xmin;
            chart.XMax  = xmax;
            chart.YMin  = ymin;
            chart.YMax  = ymax;

            var xrange  = Math.Ceiling(chart.XMax - chart.XMin);
            var xt      = Math.Ceiling(xrange / 10.0);
            chart.XTick = xt>0?xt:1;

            var yrange  = Math.Ceiling(chart.YMax - chart.YMin);
            chart.YMax  = Math.Ceiling(yrange * 1.2);
            var yt      = Math.Ceiling(chart.YMax / 10.0);
            chart.YTick = yt>0?yt:1;

            dataset.DataList.Add(ds);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Shows the debug stack.
        /// </summary>
        /// <param name="showDebugStack"> Stack of show debugs.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowDebugStack(bool showDebugStack)
        {
            var viewModel = VMDebugStack;
            ShowToolView(showDebugStack, viewModel);

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Shows the debug variables.
        /// </summary>
        /// <param name="showDebugVariables"> true to show, false to hide the debug variables.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowDebugVariables(bool showDebugVariables)
        {
            var viewModel = VMDebugVariables;
            ShowToolView(showDebugVariables, viewModel);

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Shows the error list.
        /// </summary>
        /// <param name="showErrorList"> List of show errors.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowErrorList(bool showErrorList)
        {
            var viewModel = VMErrorList;
            ShowToolView(showErrorList, viewModel);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Shows the output list.
        /// </summary>
        /// <param name="showOutput"> List of show output.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowOutput(bool showOutput)
        {
            var viewModel = VMOutput;
            ShowToolView(showOutput, viewModel);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Shows the start page.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowStartPage()
        {
            //  var spage = GetStartPage();

            //if (spage != null)
            //{
            //    //TODO fix start page
            //    VMActiveDocument = spage;
            //}

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Shows the tool view.
        /// </summary>
        /// <param name="show">      true to show, false to hide.</param>
        /// <param name="viewModel"> The view model.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowToolView(bool show, ViewModelTool viewModel)
        {
            if (show)
            {
                if (!_tools.Contains(viewModel))
                {
                    // show it
                    _tools.Add(viewModel);
                    RaisePropertyChanged("Tools");
                }
                viewModel.IsVisible = true;
            }
            else
            {
                // hide it
                if (_tools.Contains(viewModel))
                {
                    viewModel.IsVisible = false;
                    _tools.Remove(viewModel);
                    RaisePropertyChanged("Tools");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Shows the workspace options.
        /// </summary>
        /// <param name="showUser"> [optional=false] True if show User.</param>
        /// <returns>
        ///  true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ShowWorkspaceOptions(bool showUser = false)
        {
            try
            {
                var v = new ViewWorkspaceOptions(showUser);
                var vm = VMWorkspaceOptions;
                vm.BindView(this, v);
                v.ShowInWindow(true, "Workspace Opitons");

                if (vm.DialogResult)
                {
                    vm.Save();
                }
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Shutdowns the application
        ///  Stops shutdown if cancelled on file save dialog.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Shutdown()
        {
            ShutdownSaveAndCloseAllFiles( false );
            MRUWorkspaces.Save();
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Close();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Shutdown save and close all files.
        /// </summary>
        /// <param name="showCancel"> (Optional)</param>
        /// <returns>
        ///  true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ShutdownSaveAndCloseAllFiles(bool showCancel = true)
        {
            foreach (var file in Files)
            {
                if (FileSaveQueyIfDirty(file, showCancel))
                {
                    _shutDownInProgress = false;
                    return true;
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Main entry point of the Controller. Called once (from App.xaml.cs) this will initialise the application.
        /// </summary>
        /// <param name="solution"> The solution.  or null if no start</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Start(string solution)
        {
            var v = GetMainView();
            v.Show();
            if (string.IsNullOrEmpty(AppSettings.Default.UserName.Trim()))
            {
                ShowWorkspaceOptions(true);
            }
            if (solution != null)
            {
                WorkspaceOpen(solution);
            }
        }
    }
}



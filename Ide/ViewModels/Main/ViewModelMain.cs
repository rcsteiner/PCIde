////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the view model main class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/5/2015   rcs     Initial Implementation
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Ide;
using Ide.Controller;
using Views;
using Views.Support;
using ZCore;
using ZCore.Util;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// View model main is basically the:
    /// 1) Main Menu  
    /// 2) Menu Bar  
    /// 3) Dock Control  
    /// 4) Status Bar
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class ViewModelMain : ViewModelBase
    {

        #region Fields
        
        private RelayCommand<object>    _toggleEditorOptionCommand;
        private bool                    _showWorkspace;
        private bool                    _showErrorList;
        private bool                    _showOutput;
        private bool                    _showElevator;
        private bool                    _showStats;
        private bool                    _showChart;
        private bool                    _showDebugVariables;
        private bool                    _showDebugStack;
        private DispatcherTimer         _dateTimeTimer;
        private string                  _textPosition;

        #endregion

        /// <summary>
        /// Gets or sets the active document.
        /// </summary>
        public ViewModelFileEdit VMActiveDocument {get { return IdeController.VMActiveDocument; } set{IdeController.VMActiveDocument = value;}}

        /// <summary>
        /// Gets the IDE controller.
        /// </summary>
        public ControllerIde IdeController {get { return (ControllerIde) Controller; }}

        /// <summary>
        /// Gets the tools.
        /// </summary>
        public ObservableCollection<ViewModelTool> Tools { get { return IdeController.Tools; } }

        /// <summary>
        /// Gets the files.
        /// </summary>
        public ObservableCollection<ViewModelFileEdit> Files { get { return IdeController.Files; } }


        /// <summary>
        /// Gets the MRU workspaces.
        /// </summary>
        public MRUList MRUWorkspaces { get { return IdeController.MRUWorkspaces; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show solution].
        /// </summary>
        public bool ShowWorkspace
        {
            get { return _showWorkspace; }
            set {
                if (value != _showWorkspace)
                {
                    _showWorkspace = value;
                    RaisePropertyChanged("ShowWorkspace");
                } }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show error list].
        /// </summary>
        public bool ShowErrorList
        {
            get { return _showErrorList; }
            set {
                if (value != _showErrorList)
                {
                    _showErrorList = value;
                    RaisePropertyChanged("ShowErrorList");
                } }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show output].
        /// </summary>
        public bool ShowOutput
        {
            get { return _showOutput; }
            set {
                if (value != _showOutput)
                {
                    _showOutput = value;
                    RaisePropertyChanged("ShowOutput");
                } }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show Elevator].
        /// </summary>
        public bool ShowElevator
        {
            get { return _showElevator; }
            set
            {
                if (value != _showElevator)
                {
                    _showElevator = value;
                    RaisePropertyChanged("ShowElevator");
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show Stats].
        /// </summary>
        public bool ShowStats
        {
            get { return _showStats; }
            set
            {
                if (value != _showStats)
                {
                    _showStats = value;
                    RaisePropertyChanged("ShowStats");
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show Chart].
        /// </summary>
        public bool ShowChart
        {
            get { return _showChart; }
            set
            {
                if (value != _showChart)
                {
                    _showChart = value;
                    RaisePropertyChanged("ShowChart");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show debug variable].
        /// </summary>
        public bool ShowDebugVariables
        {
            get { return _showDebugVariables; }
            set
            {
                if (value != _showDebugVariables)
                {
                    _showDebugVariables = value;
                    RaisePropertyChanged( "ShowDebugVariables" );
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show debug stack window].
        /// </summary>
        public bool ShowDebugStack
        {
            get { return _showDebugStack; }
            set
            {
                if (value != _showDebugStack)
                {
                    _showDebugStack = value;
                    RaisePropertyChanged( "ShowDebugStack" );
                }
            }
        }

        /// <summary>
        ///     Gets the company.
        /// </summary>
        private static string Company {get { return BuildInfo.Company; }}


        /// <summary>
        ///   Get a path to the directory where the application can persist/load user data on session exit and re-start.
        /// </summary>
        public static string DirAppData
        {
            get {return BuildInfo.DirAppData(); }
        }
        /// <summary>
        /// Sets the position
        /// </summary>
        public string TextPosition { get { return _textPosition; } set { _textPosition = value; RaisePropertyChanged( "TextPosition" ); } }

        /// <summary>
        ///Session timer
        /// </summary>
        public string Timer
        {
            get
            {
                var timeSpan = DateTime.Now.Truncate(TimeSpan.TicksPerSecond) - StartTime;
                return string.Format("[ {0:g} ]", timeSpan);
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get { return _message; }

            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }


        /// <summary>
        /// Gets the date time now.
        /// </summary>
        public string DateTimeNow { get { return DateTime.Now.ToString("F"); }  }

        /// <summary>
        /// Gets the application version
        /// </summary>
        public string AppVersion { get { return BuildInfo.Product + "  Version " + Controller.AppVersionShort; } }

        /// <summary>
        /// The start time
        /// </summary>
        public DateTime StartTime;

        /// <summary>
        ///     Gets the toggle editor option command.
        /// </summary>
        public ICommand ToggleEditorOptionCommand
        {
            get
            {
                if (_toggleEditorOptionCommand == null)
                {
                    _toggleEditorOptionCommand = new RelayCommand<object>( p => OnToggleEditorOption( p ),p => CanToggleEditorOption( p ) );
                }

                return _toggleEditorOptionCommand;
            }
        }

        private bool _showListing;

        /// <summary>
        /// Gets or sets a value indicating whether [show listing].
        /// </summary>
        public bool ShowListing
        {
            get { return _showListing; }
            set
            {
                _showListing = value;
                RaisePropertyChanged( "ShowListing" );
                FilterChangedExecute(".pclst");
                //              FilteredSolutions.Refresh();
            }
        }



        //private ICommand _filterChangedCommand;

        ///// <summary>
        /////     Gets the close command.
        ///// </summary>
        //public ICommand FilterChanged { get { return _filterChangedCommand ?? (_filterChangedCommand = new RelayCommand<string>( p => FilterChangedExecute( p ) )); } }

        /// <summary>
        ///     Gets the filename of the layout file.
        /// </summary>
        public static string LayoutFileName {get { return   BuildInfo.Title + ".config" ; }}


        /// <summary>
        /// Gets the MRU select command.
        /// </summary>
        public RelayCommand<object> MRUSelectCommand
        {
            get
            {
                return _mruSelectCommand ?? (_mruSelectCommand = new RelayCommand<object>(MRUExecute));
            }
        }

        private void MRUExecute(object o)
        {
              Controller.WorkspaceOpen(o.ToString());
        }

        #region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        /// <param name="viewMain">    The main view. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelMain(ControllerIde controller, ViewMain viewMain)
            : base(controller, viewMain)
        {
            DockManager = viewMain.DockingManager;
            SetupDateTimeTimer();
            SetupEditorOptions();

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets up the date time timer.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetupDateTimeTimer()
        {
            StartTime = DateTime.Now.Truncate(TimeSpan.TicksPerSecond);
            _dateTimeTimer = new DispatcherTimer(DispatcherPriority.Background);
            _dateTimeTimer.Interval = TimeSpan.FromMilliseconds(10000);
            _dateTimeTimer.Tick += (sender, args) => { RaisePropertyChanged("DateTimeNow"); RaisePropertyChanged("Timer");};
            _dateTimeTimer.Start();
        }

        #endregion

        #region Show Commands

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the start page executed.
        /// </summary>
        /// <param name="s">    The s. </param>
        /// <param name="e">    Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowStartPageExecuted(object s, ExecutedRoutedEventArgs e)
        {
            Controller.ShowStartPage();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Filter changed execute.
        /// </summary>
        /// <param name="extension">    The extension. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FilterChangedExecute(string extension)
        {
            var vm = Controller.VMWorkspace;
            vm.UpdateFilter( ShowListing, vm.CurrentSolution, extension );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Executes the show erorr list action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowErrorListExecute(object sender, ExecutedRoutedEventArgs e)
        {
            IdeController.ShowErrorList(ShowErrorList);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Executes the show output list action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowOutputExecute(object sender, ExecutedRoutedEventArgs e)
        {
            IdeController.ShowOutput(ShowOutput);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Executes the show Elevator list action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowElevatorExecute(object sender, ExecutedRoutedEventArgs e)
        {
            IdeController.ShowElevator(ShowElevator);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Executes the show output list action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowStatsExecute(object sender, ExecutedRoutedEventArgs e)
        {
            IdeController.ShowStats(ShowStats);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Executes the show Chart list action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowChartExecute(object sender, ExecutedRoutedEventArgs e)
        {
            IdeController.ShowChart(ShowChart);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the debug variables execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowDebugVariablesExecute(object sender, ExecutedRoutedEventArgs e)
        {
            IdeController.ShowDebugVariables(ShowDebugVariables);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the debug stackt execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowDebugStacktExecute(object sender, ExecutedRoutedEventArgs e)
        {
            IdeController.ShowDebugStack(ShowDebugStack);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the debug can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowDebugCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Controller.CompileOk;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        /////     Construct and add a new <seealso cref="ViewModelStartPage" /> to intenral list of documents, if none is already
        /////     present, otherwise return already present <seealso cref="ViewModelStartPage" /> from internal document collection.
        ///// </summary>
        ///// <returns>
        /////     The start page.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public ViewModelStartPage GetStartPage()
        //{
        //    var l = _files.OfType<ViewModelStartPage>().ToList();

        //    if (l.Count == 0)
        //    {
        //        var s = new ViewModelStartPage();
        //        _files.Add( s );
        //        return s;
        //    }

        //    return l[0];
        //}


        #endregion

        #region Editor Options Commands

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Queries if we can toggle editor option 'parameter'.
        /// </summary>
        /// <param name="parameter">    The parameter. </param>
        /// <returns>
        ///     true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool CanToggleEditorOption(object parameter)
        {
            if (VMActiveDocument != null)
            {
                return true;
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Executes the toggle editor option action.
        /// </summary>
        /// <param name="parameter">    The parameter. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnToggleEditorOption(object parameter)
        {

            if (parameter == null || (parameter is EditorOptionEnum) == false)
            {
                return;
            }

            var f = VMActiveDocument;
            if (f != null)
            {
                f.OptionsSet((EditorOptionEnum) parameter);
            }
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Active document changed.
        /// This is here since the controller is not bound to the main view and docking manager.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ActiveDocumentChanged()
        {
            RaisePropertyChanged( "VMActiveDocument" );
        }




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This procedure changes the current WPF Application Theme into another theme while the application is running (re-
        /// boot should not be required).
        /// </summary>
        /// <param name="s">    . </param>
        /// <param name="e">    . </param>
        /// <param name="disp"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ChangeThemeCmd_Executed(object s, ExecutedRoutedEventArgs e, Dispatcher disp)
        {
            //string oldTheme = ThemesManager.DefaultThemeName;

            //try
            //{
            //    if (e == null)
            //    {
            //        return;
            //    }

            //    if (e.Parameter == null)
            //    {
            //        return;
            //    }

            //    var newThemeName = e.Parameter as string;

            //    // Check if request is available
            //    if (newThemeName == null)
            //    {
            //        return;
            //    }

            //    oldTheme = this.mSettingsManager.SettingData.CurrentTheme;

            //    // The Work to perform on another thread
            //    ThreadStart start = delegate
            //    {
            //        // This works in the UI tread using the dispatcher with highest Priority
            //        disp.Invoke(DispatcherPriority.Send,
            //            (Action) (() =>
            //            {
            //                try
            //                {
            //                    if (this.mThemesManager.SetSelectedTheme(newThemeName) == true)
            //                    {
            //                        this.mSettingsManager.SettingData.CurrentTheme = newThemeName;
            //                        this.ResetTheme(); // Initialize theme in process
            //                    }
            //                }
            //                catch (Exception exp)
            //                {
            //                }
            //            }));
            //    };

            //    // Create the thread and kick it started!
            //    var thread = new Thread(start);

            //    thread.Start();
            //}
            //catch (Exception exp)
            //{
            //    this.mSettingsManager.SettingData.CurrentTheme = oldTheme;

            //}
        }


        //#region EditorCommands

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Set command bindings necessary to perform copy/cut/paste operations.
        ///// </summary>
        ///// <param name="win">  . </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public void InitEditCommandBinding(Window win)
        //{
        //}



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Queries if we can execute if active document is view model file edit.
        ///// </summary>
        ///// <returns>
        ///// true if it succeeds, false if it fails.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private bool CanExecuteIfActiveDocumentIsViewModelFileEdit()
        //{
        //    return VMActiveDocument != null;
        //}

        //#endregion EditorCommands

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Find a match in a given peace of string.
        ///// </summary>
        ///// <param name="selectionStart">   The selection start. </param>
        ///// <param name="selectionLength">  Length of the selection. </param>
        ///// <param name="invertLeftRight">  true to invert left right. </param>
        ///// <param name="text">             The text. </param>
        ///// <param name="f">                [in,out]. </param>
        ///// <param name="r">                [out] The r. </param>
        ///// <returns>
        ///// The found next match in text.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private Match FindNextMatchInText(int selectionStart,int selectionLength, bool invertLeftRight, string text, ref FindReplaceViewModel f,out Regex r)
        //{
        //    if (invertLeftRight)
        //    {
        //        f.SearchUp = !f.SearchUp;
        //        r = f.GetRegEx();
        //        f.SearchUp = !f.SearchUp;
        //    }
        //    else
        //    {
        //        r = f.GetRegEx();
        //    }

        //    return r.Match( text,
        //        r.Options.HasFlag( RegexOptions.RightToLeft ) ? selectionStart : selectionStart + selectionLength );
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Finds the next item.
        ///// </summary>
        ///// <param name="f">                . </param>
        ///// <param name="invertLeftRight">  (optional) true to invert left right. </param>
        ///// <returns>
        ///// true if it succeeds, false if it fails.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private bool FindNext(FindReplaceViewModel f,bool invertLeftRight = false)
        //{
        //    var CE = f.GetCurrentEditor();

        //    if (CE == null)
        //    {
        //        return false;
        //    }

        //    Regex r;
        //    var m = FindNextMatchInText( CE.SelectionStart, CE.SelectionLength,
        //        invertLeftRight, CE.Text, ref f, out r );

        //    if (m.Success)
        //    {
        //        CE.Select( m.Index, m.Length );

        //        return true;
        //    }
        //    if (f.SearchIn == SearchScope.CurrentDocument)
        //    {
        //        MessageBox.Show("No more matches" );

        //        return false;
        //    }

        //    // we have reached the end of the document
        //    // start again from the beginning/end,
        //    object OldEditor = f.CurrentEditor;
        //    do
        //    {
        //        if (f.SearchIn == SearchScope.AllDocuments)
        //        {
        //            CE = GetNextEditor( f, r.Options.HasFlag( RegexOptions.RightToLeft ) );

        //            if (CE == null)
        //            {
        //                return false;
        //            }

        //            f.CurrentEditor = CE;

        //            return true;
        //        }

        //        if (r.Options.HasFlag( RegexOptions.RightToLeft ))
        //        {
        //            m = r.Match( CE.Text, CE.Text.Length - 1 );
        //        }
        //        else
        //        {
        //            m = r.Match( CE.Text, 0 );
        //        }

        //        if (m.Success)
        //        {
        //            CE.Select( m.Index, m.Length );
        //            break;
        //        }
        //        MessageBox.Show( "No more items found","Find");
        //    } while (f.CurrentEditor != OldEditor);

        //    return false;
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Gets the next editor.
        ///// TODO delete
        ///// <param name="f">        . </param>
        ///// <param name="previous"> (optional) true to previous. </param>
        ///// <returns>
        ///// The next editor.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private IEditor GetNextEditor(FindReplaceViewModel f,bool previous = false)
        //{
        //    // There is no next open document if there is none or only one open
        //    if (Files.Count <= 1)
        //    {
        //        return f.GetCurrentEditor();
        //    }

        //    // There is no next open document If the user wants to search the current document only
        //    if (f.SearchIn == SearchScope.CurrentDocument)
        //    {
        //        return f.GetCurrentEditor();
        //    }

        //    var l = new List<object>( Files.Cast<object>() );

        //    var idxStart = l.IndexOf( f.CurrentEditor );
        //    var i = idxStart;

        //    if (i >= 0)
        //    {
        //        Match m = null;

        //        var textSearchSuccess = false;
        //        do
        //        {
        //            if (previous) // Get next/previous document
        //            {
        //                i = (i < 1 ? l.Count - 1 : i - 1);
        //            }
        //            else
        //            {
        //                i = (i >= l.Count - 1 ? 0 : i + 1);
        //            }

        //            //// i = (i + (previous ? l.Count - 1 : +1)) % l.Count;

        //            var fTmp = l[i] as ViewModelFileEdit; // Search text in document
        //            if (fTmp != null)
        //            {
        //                Regex r;
        //                m = FindNextMatchInText( 0, 0, false, fTmp.Text, ref f, out r );

        //                textSearchSuccess = m.Success;
        //            }
        //        } while (i != idxStart && textSearchSuccess != true);

        //        // Found a match so activate the corresponding document and select the text with scroll into view
        //        if (textSearchSuccess && m != null)
        //        {
        //            var doc = l[i] as ViewModelFileEdit;

        //            if (doc != null)
        //            {
        //                VMActiveDocument = doc;
        //            }

        //            // Ensure that no pending calls are in the dispatcher queue
        //            // This makes sure that we are blocked until bindings are re-established
        //            // Bindings are required to scroll a selection into view
        //            Dispatcher.CurrentDispatcher.BeginInvoke( DispatcherPriority.SystemIdle, (Action)delegate
        //            {
        //                if (this.VMActiveDocument != null && doc != null)
        //                {
        //                    //doc.TextEditorSelectionStart = m.Index;
        //                    //doc.TextEditorSelectionLength = m.Length;

        //                    //// Reset cursor position to make sure we search a document from its beginning
        //                    //doc.TxtControl.SelectText( m.Index, m.Length );

        //                    f.CurrentEditor = l[i] as IEditor;

        //                    var edi = f.GetCurrentEditor();

        //                    if (edi != null)
        //                    {
        //                        edi.Select( m.Index, m.Length );
        //                    }
        //                }
        //            } );

        //            return f.GetCurrentEditor();
        //        }
        //    }

        //    return null;
        //}

        #region Editor Options

        public List<CheckOption> EnumValues { get; private set; }
        public List<ExclusiveOption> EnumValues2 { get; private set; }

        private EditorOptionEnum     _selectedValue;
        private RelayCommand<object> _mruSelectCommand;
        private string               _message;

        public EditorOptionEnum SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                _selectedValue = value;
                RaisePropertyChanged("SelectedValue");
            }
        }


        public void SetupEditorOptions()
        {
            EnumValues = new List<CheckOption>();
            EnumValues2 = new List<ExclusiveOption>();

            foreach (object t in Enum.GetValues( typeof(     EditorOptionEnum) ))
            {
                EnumValues.Add( new CheckOption { Value = (EditorOptionEnum) t } );
                EnumValues2.Add( new ExclusiveOption { Value = (EditorOptionEnum) t  });
            }

            SelectedValue = EditorOptionEnum.ConvertTabsToSpaces;
            ExclusiveOption.Owner = this;
        }

        #endregion
    }


    // ViewModel for Test enumeration
    public class CheckOption
    {
        public EditorOptionEnum Value { get; set; }

        public ImageSource Icon { get { return IconUtil.GetImage("Show" + Value.ToString()); }  }

        public string Text
        {
            get { return Value.ToString().FromCamel(); }
        }

        public bool IsChecked { get; set; }
    }

    // ViewModel for Exclusive Test enumeration
    public class ExclusiveOption
    {
        public static ViewModelMain Owner { get; set; }
        public ImageSource Icon { get { return IconUtil.GetImage("Show" + Text); }  }

        public EditorOptionEnum Value { get; set; }
        public string Text { get { return Value.ToString(); } }
        public bool IsChecked
        {
            get { return Value == Owner.SelectedValue; }
            set
            {
                if (value)
                {
                    Owner.SelectedValue = Value;
                }
            }
        }
    }
}
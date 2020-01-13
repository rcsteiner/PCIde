////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the file view model class
//
// Author:
//  Robert C. Steiner
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

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Ide.Controller;
using Ide.Model;
using Microsoft.Win32;
using Views;
using Views.Editors;
using Views.Support;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     File view model used when a file is being edited.
    ///     TODO reconcile with the ViewModelTree
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelFileEdit : ViewModelPane
    {

        #region Fields

        private TextDocument             _document;
        private RelayCommand             _saveAsCommand;
        private RelayCommand             _closeCommand;
        private RelayCommand             _saveCommand;
        private RelayCommand             _openContainingFolderCommand;
        private RelayCommand             _copyFullPathtoClipboard;
        private ModelFilePath            _filePath;

        private int                     _columnRulerPosition = 120;
        private EditorOptions           _textEditorEditorOptions;
        private bool                    _showLineNumbers;
        private bool                    _wordWrap;
        private bool                    _showOutline;

        private string                  _isReadOnlyReason = string.Empty;
        private bool                    _isReadOnly;
        private bool                    _isDirty;
        private IHighlightingDefinition _highlightdef;
  
   
        private string                   _fontFamily="Consolas";
        private float                    _fontSize=18;
 //       private double                   _scaleValue;
   //     private int                      _lineNumber;
        private string                   _fullPath;
     //   private string                   _text;
     
        private TextLocation _textLocation = new TextLocation( 0, 0 );
        private bool         _showCurrentLineMarker;
        private bool         _showCurrentLineHighlighted;
        private int          _currentLine;

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether the current line highligted is shown.
        /// </summary>
        public bool ShowCurrentLineHighligted
        {
            get { return _showCurrentLineHighlighted; }
            set
            {
                _showCurrentLineHighlighted = value;
                RaisePropertyChanged("ShowCurrentLineHighligted");
            }
        }


        public Bookmark CurrentLineBookmark { get; set; }

        /// <summary>
        ///     Gets the filename of the file.
        /// </summary>
        public string FileName {get { return (FilePath==null || FilePath.FullPath == null) ? "NewFile" + (IsDirty ? "*" : "") : FilePath.Name + (IsDirty ? "*" : "");}}

        /// <summary>
        ///     Gets the text.
        /// </summary>
        public string Text { get { return Document == null ? string.Empty : Document.Text; } }

        /// <summary>
        ///     Title is the string that is usually displayed - with or without dirty mark '*' - in the docking environment.
        /// </summary>
        public override string Title {get { return FilePath.Name + (IsDirty ? "*" : string.Empty); }}

        #region Commands

        /// <summary>
        ///     Gets the close command.
        /// </summary>
        public virtual ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(OnClose, CanClose)); }
        }

        /// <summary>
        ///     Gets the save as command.
        /// </summary>
        public ICommand SaveAsCommand
        {
            get { return _saveAsCommand ?? (_saveAsCommand = new RelayCommand(OnSaveAs, CanSaveAs)); }
        }

        /// <summary>
        ///     Gets the save command.
        /// </summary>
        public virtual ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(OnSave, CanSave)); }
        }

        /// <summary>
        /// Get open containing folder command which will open
        /// the folder indicated by the path in windows explorer
        /// and select the file (if path points to one).
        /// </summary>
        public ICommand OpenContainingFolderCommand
        {
            get
            {
                return _openContainingFolderCommand ??
                       (_openContainingFolderCommand = new RelayCommand(OnOpenContainingFolderCommand));
            }
        }

        /// <summary>
        /// Get CopyFullPathtoClipboard command which will copy
        /// the path of the executable into the windows clipboard.
        /// </summary>
        public ICommand CopyFullPathtoClipboard
        {
            get
            {
                return _copyFullPathtoClipboard ??
                       (_copyFullPathtoClipboard = new RelayCommand(OnCopyFullPathtoClipboardCommand));
            }
        }

        #endregion

        #region IconBarManager

        private IconBarManager _iconBarManager;

        /// <summary>
        /// Gets or sets the text marker service.
        /// </summary>
        public IconBarManager IconBarManager
        {
            get { return _iconBarManager; }
            set
            {
                _iconBarManager = value;
                RaisePropertyChanged( "IconBarManager" );
            }
        }

        #endregion


        #region TextMarkerSerivce

        private ITextMarkerService _textMarkService;

        /// <summary>
        /// Gets or sets the text marker service.
        /// </summary>
        public ITextMarkerService TextMarkerService
        {
            get { return _textMarkService; }
            set
            {
                _textMarkService = value;
                RaisePropertyChanged("TextMarkerService");
            }
        }

        #endregion

        #region Options Properties
    
        /// <summary>
        ///     Gets or sets options for controlling the text.
        /// </summary>
        public EditorOptions Options
        {
            get { return _textEditorEditorOptions; }
            set
            {
                if (_textEditorEditorOptions != value)
                {
                    _textEditorEditorOptions = value;
                    RaisePropertyChanged("Options");
                }
            }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether the line numbers is shown.
        /// </summary>
        public bool ShowLineNumbers
        {
            get { return _showLineNumbers; }

            set
            {
                if (_showLineNumbers != value)
                {
                    _showLineNumbers = value;
                    RaisePropertyChanged("ShowLineNumbers");
                }
            }
        }
        /// <summary>
        ///     Gets or sets a value indicating whether the show outlining
        /// </summary>
        public bool ShowOutline
        {
            get { return _showOutline; }

            set
            {
                if (_showOutline != value)
                {
                    _showOutline = value;
                    RaisePropertyChanged( "ShowOutline" );
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the word wrap.
        /// </summary>
        public bool WordWrap
        {
            get { return _wordWrap; }

            set
            {
                if (_wordWrap != value)
                {
                    _wordWrap = value;
                    RaisePropertyChanged("WordWrap");
                }
            }
        }

        /// <summary>
        ///     Set the font family
        /// </summary>
        public string FontFamily
        {
            get { return _fontFamily; }

            set
            {
                if (_fontFamily != value)
                {
                    _fontFamily = value;
                    RaisePropertyChanged("FontFamily");
                }
            }
        }

        public float FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    RaisePropertyChanged("FontSize");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the highlight def.
        /// </summary>
        public IHighlightingDefinition HighlightDef
        {
            get { return _highlightdef; }
            set
            {
                if (_highlightdef != value)
                {
                    _highlightdef = value;
                    RaisePropertyChanged("HighlightDef");
                }
            }
        }

        #endregion

        #region Document properties
        ///// <summary>
        ///// Gets or sets the scale value.
        ///// </summary>
        //public double ScaleValue
        //{
        //    get { return _scaleValue; }
        //    set
        //    {
        //        if (value < .1) value = .1;
        //        if (value > 100) value = 100;
        //        if (_scaleValue != value )
        //        {
        //            _scaleValue = value;
        //            RaisePropertyChanged("ScaleValue");
        //        }
        //    }
        //}


        /// <summary>
        ///     Gets or sets a value indicating whether this object is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }

            set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    RaisePropertyChanged("IsReadOnly");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the is read only reason.
        /// </summary>
        public string IsReadOnlyReason
        {
            get { return _isReadOnlyReason; }

            protected set
            {
                if (_isReadOnlyReason != value)
                {
                    _isReadOnlyReason = value;
                    RaisePropertyChanged("IsReadOnlyReason");
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this object is dirty.
        /// </summary>
        public virtual bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    
                    _isDirty = value;
                    RaisePropertyChanged("IsDirty");
                    RaisePropertyChanged("Title");
                    RaisePropertyChanged("FileName");
                }
            }
        }


        /// <summary>
        ///     Gets or sets the document.
        /// </summary>
        public TextDocument Document
        {
            get { return _document; }
            set
            {
                if (_document != value)
                {
                    _document = value;
                    RaisePropertyChanged("Document");
                    IsDirty = false;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the full path.
        /// </summary>
        public string FullPath
        {
            get { return _fullPath; }
            set
            {
                if (_fullPath != value)
                {
                    _fullPath = value;
                    RaisePropertyChanged("FullPath");
                    RaisePropertyChanged( "Title" );
                    RaisePropertyChanged( "FileName" );
                }
            }
        }

                /// <summary>
        /// Gets or sets the full pathname of the file.
        /// </summary>
        public virtual ModelFilePath FilePath
        {
            get { return _filePath; }
            set
            {
                if (!_filePath.Equals( value ))
                {
                    _filePath = value;
                    RaisePropertyChanged( "FilePath" );
                    RaisePropertyChanged( "Title" );
                }
            }
        }

        /// <summary>
        /// Gets or sets the column ruler position.
        /// </summary>
        public int ColumnRulerPosition
        {
            get { return Options.ColumnRulerPosition; }
            set
            {
                if (_columnRulerPosition != value)
                {
                    _columnRulerPosition = value;
                    Options.ColumnRulerPosition = value;
                    RaisePropertyChanged("Options");
                }
            }
        }


        #endregion
        /// <summary>
        /// Hold the start of the currently selected text.
        /// </summary>
        private int _caretPosition = 0;
        public int CaretPosition
        {
            get { return _caretPosition; }
            set
            {
                _caretPosition = value;
                RaisePropertyChanged( "CaretPosition" );
            }
        }

        /// <summary>
        /// Hold the start of the currently selected text.
        /// </summary>
        private int _selectionStart = 0;
        public int SelectionStart
        {
            get { return _selectionStart; }
            set
            {
                _selectionStart = value;
                RaisePropertyChanged("SelectionStart" );
            }
        }

        /// <summary>
        /// Hold the selection length of the currently selected text.
        /// </summary>
        private int _selectionLength = 0;
        public int SelectionLength
        {
            get { return _selectionLength; }
            set
            {
                _selectionLength = value;
                UpdateStatusBar();
                RaisePropertyChanged("SelectionLength" );
            }
        }

        /// <summary>
        /// Gets or sets the TextLocation of the current editor control. If the 
        /// user is setting this value it will scroll the TextLocation into view.
        /// </summary>
        public TextLocation TextLocation
        {
            get { return _textLocation; }
            set
            {
                _textLocation = value;
                UpdateStatusBar();
                RaisePropertyChanged("TextLocation" );
            }
        }

        /// <summary>
        /// Gets or sets the current line. (TODO Could use textlocation instead)
        /// </summary>
        public int CurrentLine
        {
            get { return _currentLine; } 
            set
            {
                if (_currentLine != value)
                {
                    _currentLine = value;
                    RaisePropertyChanged("CurrentLine");
                }
            }
        }

        /// <summary>
        /// Gets the breakpoints.
        /// </summary>
        public IList<IBookmark> Breakpoints {get{return IconBarManager != null ? IconBarManager.Bookmarks : null;}}

        #region constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePath">     Full pathname of the file. </param>
        /// <param name="controller">   The controller. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelFileEdit(ModelFilePath filePath, ControllerIde controller) : base( controller )
        {
            _filePath = filePath;
            SetupDocument();
            MessegerRegister();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets up the document.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetupDocument()
        {
            if ( FilePath.FullPath == null) return;

            Title                    = FileName;
            IconSource               = IconUtil.GetImage( FilePath.Extension );
           
            //TODO: consider making text editor options part of the MainView instead of the file view
            _textEditorEditorOptions = new EditorOptions(Controller.DefaultEditorOptions);

            string text = "";

             _fullPath = FilePath.FullPath;
            if (FilePath.Path.Exists())
            {
                HighlightDef    = HighlightingManager.Instance.GetDefinitionByExtension(FilePath.Extension);
                ShowLineNumbers = FilePath.Extension == Ide.Constants.PC_EXT;
                WordWrap        = false;
                FontSize        = 18;
                FontFamily      =  "Consolas";
                ColumnRulerPosition = 120;
          
                _isDirty        = false;
                IsReadOnly      = false;

                // Check file attributes and set to read-only if file attributes indicate that
                if ((File.GetAttributes(_fullPath) & FileAttributes.ReadOnly) != 0)
                {
                    IsReadOnly = true;
                    IsReadOnlyReason =
                        "This file cannot be edit because another process is currently writing to it.\n" +
                        "Change the file access permissions or save the file in a different location if you want to edit it.";
                }
                text = Controller.FileService.FileLoad(_fullPath);
            }
            else
            {
                text = "// File: " + FilePath.Name;
            }
            _document            = new TextDocument( text );
            _document.FileName   = FullPath;
 //           Text                 = text;
            ContentId            = FullPath;


            // margin bar for breakpoints and info
            IconBarManager = new IconBarManager();

            var markerService = new TextMarkerService( _document );
            TextMarkerService = markerService;
            var services = (IServiceContainer)_document.ServiceProvider.GetService( typeof( IServiceContainer ) );
            if (services != null)
            {
                services.RemoveService(typeof(ITextMarkerService));
                services.AddService( typeof( ITextMarkerService ), TextMarkerService );

                services.RemoveService( typeof( IBookmarkMargin ) );
                services.AddService( typeof( IBookmarkMargin ) , IconBarManager);
            }


            // create the current line bookmarker

            CurrentLineBookmark = IconBarManager.AddCurrentLine( 1, "Current execution line" );
            CurrentLineBookmark.CanDragDrop = true;
            CurrentLineBookmark.DisplaysTooltip = true;
            CurrentLineBookmark.ZOrder = 3;
        }

        #region Bookmark management

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets a current line book mark.
        /// </summary>
        /// <param name="lineNumber">   The line number. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetCurrentLineBookMark(int lineNumber)
        {
            CurrentLineBookmark.LineNumber = lineNumber;
            if (lineNumber > 0)
            {
                TextLocation = new TextLocation(lineNumber,1);
            }
            IconBarManager.Redraw();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the current line marker
        /// </summary>
        /// <param name="showMarker">   true to show, false to hide the marker. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowCurrentLineMarker(bool showMarker)
        {
            IconBarManager.Bookmarks.Remove(CurrentLineBookmark);
            _showCurrentLineMarker = showMarker;
            if (showMarker)
            {
                IconBarManager.Bookmarks.Add(CurrentLineBookmark);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a breakpoint. 
        /// </summary>
        /// <param name="lineNumber">   The line number. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddBreakpoint(int lineNumber)
        {
            var b             = IconBarManager.AddBreakpoint( lineNumber, "Breakpoint" );
            b.CanDragDrop     = true;
            b.DisplaysTooltip = true;
            b.Enabled         = true;
            IconBarManager.Redraw();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a breakpoint. 
        /// </summary>
        /// <param name="lineNumber">   The line number. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveBreakpoint(int lineNumber)
        {
            IBookmark b = null;
            foreach (var bookmark in IconBarManager.Bookmarks)
            {
                if (bookmark != CurrentLineBookmark && bookmark.LineNumber == lineNumber)
                {
                    b = bookmark;
                    break;
                }

            }
            if (b!=null) IconBarManager.Bookmarks.Remove(b);
            IconBarManager.Redraw();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clears the bookmarks.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ClearBookmarks()
        {
            IconBarManager.Bookmarks.Clear();
            ShowCurrentLineMarker(_showCurrentLineMarker);
            IconBarManager.Redraw();
        }
        #endregion

        #endregion constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Options set.
        /// </summary>
        /// <param name="t">    The t. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OptionsSet( EditorOptionEnum t)
        {
            switch (t)
            {
                case EditorOptionEnum.ShowOutline:
                    ShowOutline = !ShowOutline;
                    break;
                case EditorOptionEnum.WordWrap:
                    WordWrap = !WordWrap;
                    break;

                case EditorOptionEnum.ShowLineNumber:
                    ShowLineNumbers = !ShowLineNumbers;
                    break;

                case EditorOptionEnum.ShowSpaces:
                    Options.ShowSpaces = !Options.ShowSpaces;
                    break;

                case EditorOptionEnum.ShowTabs:
                    Options.ShowTabs = !Options.ShowTabs;
                    break;

                case EditorOptionEnum.ShowEndOfLine:
                    Options.ShowEndOfLine = !Options.ShowEndOfLine;
                    break;

                case EditorOptionEnum.ShowColumnRuler:
                    Options.ShowColumnRuler = !Options.ShowColumnRuler;
                    break;

                case EditorOptionEnum.ConvertTabsToSpaces:
                    Options.ConvertTabsToSpaces = !Options.ConvertTabsToSpaces;
                    break;
            }
        }

        #region OkCommand
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Queries if we can save 'parameter'.
        /// </summary>
        /// <returns>
        ///     true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CanSave()
        {
            return IsDirty;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Executes the save action.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSave()
        {
           Controller.FileSave(this, false);
        }

        #endregion

        #region SaveAsCommand
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Queries if we can save as 'parameter'.
        /// </summary>
        /// <returns>
        ///     true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CanSaveAs()
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Executes the save as action.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSaveAs()
        {
            Controller.FileSave(this, true);
        }
        #endregion

        #region Close command
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Queries if we can close.
        /// </summary>
        /// <returns>
        ///     true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CanClose()
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Executes the close action.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void OnClose()
        {
           Controller.FileClose(this);
           MessegerDeregister();
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Updates the status bar.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateStatusBar()
        {
            Controller.VMMain.TextPosition = _textLocation.ToString();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Export the current content of the text editor as HTML.
        /// </summary>
        /// <param name="defaultFileName">          (Optional) </param>
        /// <param name="showLineNumbers">          (Optional) true to show, false to hide the line numbers. </param>
        /// <param name="alternateLineBackground">  (optional) true to alternate line background. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ExportToHtml(string defaultFileName = "", bool showLineNumbers = true, bool alternateLineBackground = true)
        {
            var ExportHTMLFileFilter = Ide.Constants.HTML_FILE_FILTER;

            // Create and configure SaveFileDialog.
            FileDialog dlg = new SaveFileDialog
            {
                ValidateNames = true,
                AddExtension  = true,
                Filter        = ExportHTMLFileFilter,
                FileName      = defaultFileName
            };

            // Show dialog; return if canceled.
            if (!dlg.ShowDialog( Application.Current.MainWindow ).GetValueOrDefault())
            {
                return;
            }

            defaultFileName           = dlg.FileName;
            HtmlWriter w              = new HtmlWriter();
            w.ShowLineNumbers         = showLineNumbers;
            w.AlternateLineBackground = alternateLineBackground;
            string html               = w.GenerateHtml( Text, HighlightDef );
            File.WriteAllText( defaultFileName, @"<html><body>" + html + @"</body></html>" );

            // view in browser
            Process.Start( defaultFileName );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the copy full pathto clipboard command action.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCopyFullPathtoClipboardCommand()
        {
            try
            {
                Clipboard.SetText( FilePath.FullPath );
            }
            catch
            {
            }
        }

     
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Opens the folder in which this document is stored in the Windows Explorer.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnOpenContainingFolderCommand()
        {
            try
            {
                if ( FilePath.Path.Exists() )
                {
                    // combine the arguments together it doesn't matter if there is a space after ','
                    Process.Start(Ide.Constants.EXPLORER_EXE, @"/select, " + FilePath );
                }
                else
                {
                    string parentDir = FilePath.Path.Folder;

                    if (!FileUtil.FolderExists(parentDir))
                    {
                        string argument = @"/select, " + parentDir;

                        Process.Start(Ide.Constants.EXPLORER_EXE, argument);
                    }
                    else
                    {
                        MessageBox.Show(string.Format(CultureInfo.CurrentCulture, "Folder '{0}' does not exist",parentDir),"Can't find File",MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show( string.Format( "{0}\n'{1}'.", ex.Message, FilePath.FullPath ), "Can't find File", MessageBoxButton.OK );
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Match identifier calls each registered listener, if the listener matches the identifier, then the listener
        /// returns true.
        /// Names are vm:fullpath
        /// </summary>
        /// <param name="identifier">   The identifier. </param>
        /// <returns>
        /// true if it matches the identifier.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool MatchIdentifier(string identifier)
        {
            var fullPath = FullPath;
            return fullPath != null && fullPath.StartsWith( "vm:" ) && identifier.Equals( fullPath.Substring( 3 ) );
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return FileName != null ? "ViewModel: "+FileName : "ViewModel: <new file>";
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Updates a language.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UpdateLanguage()
        {
            //       var textEditor = Document;
            //    if (textEditor.SyntaxHighlighting == null)
            //    {
            //        foldingStrategy = null;
            //    }
            //    else
            //    {
            //        switch (textEditor.SyntaxHighlighting.Name)
            //        {
            //            case "XML":
            //                foldingStrategy = new XmlFoldingStrategy();
            //                textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
            //                break;
            //            case "C#":
            //            case "C++":
            //            case "PHP":
            //            case "Java":
            //                textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy( textEditor.Options );
            //                foldingStrategy = new BraceFoldingStrategy();
            //                break;
            //            default:
            //                textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
            //                foldingStrategy = null;
            //                break;
            //        }
            //    }
            //    if (foldingStrategy != null)
            //    {
            //        if (foldingManager == null)
            //            foldingManager = FoldingManager.Install( textEditor.TextArea );
            //        UpdateFoldings();
            //    }
            //    else
            //    {
            //        if (foldingManager != null)
            //        {
            //            FoldingManager.Uninstall( foldingManager );
            //            foldingManager = null;
            //        }
            //    }
            //}

            //void UpdateFoldings()
            //{
            //    if (foldingStrategy is BraceFoldingStrategy)
            //    {
            //        ((BraceFoldingStrategy)foldingStrategy).UpdateFoldings( foldingManager, Document );
            //    }
            //    if (foldingStrategy is XmlFoldingStrategy)
            //    {
            //        ((XmlFoldingStrategy)foldingStrategy).UpdateFoldings( foldingManager, Document );
            //    }
        }

        #region Markers

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clears the markers.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ClearMarkers()
        {
            TextMarkerService.RemoveAll(m => true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a marker.
        /// </summary>
        /// <param name="selectionStart">   The selection start. </param>
        /// <param name="selectionLength">  Length of the selection. </param>
        /// <param name="error">            The error. </param>
        /// <param name="description">      (optional) the description. </param>
        /// <param name="title">            (Optional) Title is the string that is usually displayed - with or without dirty
        ///                                 mark '*' - in the docking environment. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddMarker(int selectionStart, int selectionLength, Error error, string description = "",string title = "")
        {

            var marker = TextMarkerService.Create(selectionStart, selectionLength,error);
            var level = error.Level;
            if (marker != null)
            {
                marker.MarkerTypes = (level >= ErrorLevel.WARNING
                    ? TextMarkerTypes.SquigglyUnderline
                    : TextMarkerTypes.DottedUnderline) | TextMarkerTypes.LineInScrollBar;
                marker.MarkerColor = ErrorLevelToColorConverter.ErrorToColor(level);
                marker.ToolTip     = description;
                marker.FontWeight  = FontWeights.Bold;
                marker.Tag         = title;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a marker.
        /// </summary>
        /// <param name="error">            The error. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddMarker(Error error)
        {
            AddMarker(error.Offset, error.Length, error, error.Description, error.Name.Capitalize());
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Inserts the text at current position.
        /// </summary>
        /// <param name="text"> The text. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Insert(string text)
        {
           Document.Insert(CaretPosition,text);
        }
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the ide editor.xaml class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/25/2015   rcs     Initial Implementation
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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Views.Dialogs;
using ZCore;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interaction logic for IdeEditor.xaml. This must be kept simple because the AvalonDock control calls the
    /// constructor and rebinds the new view on each tab or view change (when it gets hidden)
    /// 
    /// All markup must be done on the document and then it will be kept between tabs etc.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class IdeEditor : TextEditor, IMessageListener
    {
        /// <summary>
        /// The can scroll flag.
        /// </summary>
        private static bool canScroll = true;

        /// <summary>
        /// The change count, used for revision checking.
        /// </summary>
        private int _changeCount = 0;

        /// <summary>
        /// The completion window.
        /// </summary>
        private CompletionWindow _completionWindow=null;


        /// <summary>
        /// Gets or sets the current line highligher.
        /// </summary>
        public HighlightCurentLineRenderer CurrentLineHighligher { get; set; }



        /// <summary>
        /// The icon bar manager property
        /// </summary>
        public static readonly DependencyProperty IconBarManagerProperty = DependencyProperty.Register(
            "IconBarManager", typeof (IconBarManager), typeof (IdeEditor), new PropertyMetadata(default(IconBarManager)));

        /// <summary>
        /// Gets or sets the manager for icon bar.
        /// </summary>
        public IconBarManager IconBarManager
        {
            get { return (IconBarManager) GetValue(IconBarManagerProperty); }
            set { SetValue(IconBarManagerProperty, value); }
        }

        #region TextMarker Service

        /// <summary>
        /// The text marker service property.
        /// </summary>
        public static readonly DependencyProperty TextMarkerServiceProperty = DependencyProperty.Register("TextMarkerService", typeof (ITextMarkerService), typeof (IdeEditor), new PropertyMetadata(default(ITextMarkerService)));

        /// <summary>
        /// Gets or sets the text marker service.
        /// </summary>
        public ITextMarkerService TextMarkerService
        {
            get { return (ITextMarkerService) GetValue(TextMarkerServiceProperty); }
            set { SetValue(TextMarkerServiceProperty, value); }
        }

        #endregion

        #region FullPath binding

        // Using a DependencyProperty as the backing store for FilePath. 
        // This enables animation, styling, binding, etc...

        /// <summary>
        /// The full path property.
        /// </summary>
        public static readonly DependencyProperty FullPathProperty = DependencyProperty.Register("FullPath",typeof (string), typeof (IdeEditor), new PropertyMetadata(String.Empty,
            OnFullPathChanged));

        /// <summary>
        /// Gets or sets the full path.
        /// </summary>
        public string FullPath
        {
            get { return (string) GetValue(FullPathProperty); }
            set { SetValue(FullPathProperty, value); }
        }

        #endregion

        #region CaretPosition

        /// <summary>
        /// The caret position property
        /// </summary>
        public static readonly DependencyProperty CaretPositionProperty = DependencyProperty.Register( "CaretPosition", typeof( int ), typeof( IdeEditor ), new PropertyMetadata( 0) );

        /// <summary>
        /// Gets or sets the caret positoin
        /// </summary>
        public int CaretPosition
        {
            get { return (int)GetValue( CaretPositionProperty ); }
            set { SetValue( CaretPositionProperty, value ); }
        }


        #endregion

        #region ScaleFactor

        /// <summary>
        /// The scale factor property.
        /// </summary>
        public static readonly DependencyProperty ScaleFactorProperty = DependencyProperty.Register("ScaleFactor", typeof (double), typeof (IdeEditor), new PropertyMetadata(1.0));

        /// <summary>
        /// Gets or sets the scale value.
        /// </summary>
        public double ScaleFactor
        {
            get { return (double) GetValue(ScaleFactorProperty); }
            set { SetValue(ScaleFactorProperty, value); }
        }


        #endregion

        /// <summary>
        /// The document.
        /// </summary>
        public TextDocument doc;

        /// <summary>
        /// The folding update timer.
        /// </summary>
        private DispatcherTimer _foldingUpdateTimer;

        /// <summary>
        /// The show outline property.
        /// </summary>
        public static readonly DependencyProperty ShowOutlineProperty = DependencyProperty.Register("ShowOutline", typeof (bool), typeof (IdeEditor), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Gets or sets a value indicating whether [show outline].
        /// </summary>
        public bool ShowOutline
        {
            get { return (bool) GetValue(ShowOutlineProperty); }
            set { SetValue(ShowOutlineProperty, value); }
        }


        #region Caret Offset.

        /// <summary>
        /// DependencyProperty for the TextEditorCaretOffset binding.
        /// </summary>
        public static DependencyProperty CaretOffsetProperty =
            DependencyProperty.Register( "CaretOffset", typeof( int ), typeof( IdeEditor ),
            new PropertyMetadata( (obj, args) =>
            {
                IdeEditor target = (IdeEditor)obj;
                if (target.CaretOffset != (int)args.NewValue)
                    target.CaretOffset = (int)args.NewValue;
            } ) );

        /// <summary>
        /// Access to the SelectionStart property.
        /// </summary>
        public new int CaretOffset
        {
            get { return base.CaretOffset; }
            set { SetValue( CaretOffsetProperty, value ); }
        }
        #endregion // Caret Offset.

        #region Selection.

        /// <summary>
        /// DependencyProperty for the TextLocation. Setting this value will scroll the TextEditor to the desired
        /// TextLocation.
        /// </summary>
        public static readonly DependencyProperty TextLocationProperty =
             DependencyProperty.Register( "TextLocation", typeof( TextLocation ), typeof( IdeEditor ),
             new PropertyMetadata( (obj, args) =>
             {
                 IdeEditor target = (IdeEditor)obj;
                 TextLocation loc = (TextLocation)args.NewValue;
                 
                 if (canScroll)
                 {
                     target.ScrollTo( loc.Line, loc.Column );
                    // target.CaretPosition = target.Document.GetOffset(loc);
                     if (loc.Line <= target.LineCount && loc.Line > 0)
                     {
                         target.Select( target.Document.GetOffset( loc ),0);
                     }
                 }
             } ) );

        /// <summary>
        /// Get or set the TextLocation. Setting will scroll to that location.
        /// </summary>
        public TextLocation TextLocation
        {
            get {return Document != null ? Document.GetLocation( SelectionStart ) : new TextLocation(0,0);}
            set { SetValue( TextLocationProperty, value ); }
        }

        /// <summary>
        /// DependencyProperty for the TextEditor SelectionLength property.
        /// </summary>
        public static readonly DependencyProperty SelectionLengthProperty =
             DependencyProperty.Register( "SelectionLength", typeof( int ), typeof( IdeEditor ),
             new PropertyMetadata( (obj, args) =>
             {
                 IdeEditor target = (IdeEditor)obj;
                 var length = (int)args.NewValue;
                 int documentLength = target.Document != null ? target.Document.TextLength : 0;

                 if (target.SelectionLength != length && documentLength >= length + target.SelectionStart)
                 {
                     target.SelectionLength = length;
                     target.Select( target.SelectionStart, length );
                 }
             } ) );

        /// <summary>
        /// Access to the SelectionLength property.
        /// </summary>
        public new int SelectionLength
        {
            get { return base.SelectionLength; }
            set { SetValue( SelectionLengthProperty, value ); }
        }

        /// <summary>
        /// DependencyProperty for the TextEditor SelectionStart property.
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register( "SelectionStart", typeof( int ), typeof( IdeEditor ),
            new PropertyMetadata( (obj, args) =>
             {
                 IdeEditor target   = (IdeEditor)obj;
                 var start          = (int)args.NewValue;
                 int documentLength = target.Document != null ? target.Document.TextLength : 0;
                 if (start < 0 || start > documentLength) return;
                 if (target.SelectionStart != start)
                 {
                     target.SelectionStart = start;
                     target.Select( start, target.SelectionLength );
                 }
             } ) );

        /// <summary>
        /// Access to the SelectionStart property.
        /// </summary>
        public new int SelectionStart
        {
            get { return base.SelectionStart; }
            set { SetValue( SelectionStartProperty, value ); }
        }
        #endregion

        #region Current hight highlighted

        /// <summary>
        /// The show current line highligted property
        /// </summary>
        public static readonly DependencyProperty ShowCurrentLineHighligtedProperty = DependencyProperty.Register(
            "ShowCurrentLineHighligted", typeof( bool ), typeof( IdeEditor ), new PropertyMetadata( (obj, args) =>
            {
                IdeEditor target = (IdeEditor)obj;
                var isVisible = (bool)args.NewValue;
                target.CurrentLineHighligher.IsVisible = isVisible;
                target.TextArea.TextView.Redraw();
            } ) );


        /// <summary>
        /// Gets or sets a value indicating whether [show current line highligted].
        /// </summary>
        public bool ShowCurrentLineHighligted
        {
            get { return (bool) GetValue(ShowCurrentLineHighligtedProperty); }
            set { SetValue(ShowCurrentLineHighligtedProperty, value); }
        }

        #endregion

        #region Current Line number

        /// <summary>
        /// The current line property
        /// </summary>
        public static readonly DependencyProperty CurrentLineProperty = DependencyProperty.Register(
            "CurrentLine", typeof (int), typeof (IdeEditor), new PropertyMetadata((obj, args) =>
            {
                IdeEditor target = (IdeEditor) obj;
                var line = (int) args.NewValue;
                target.TextLocation = new TextLocation(line, 1);
            }));

        /// <summary>
        /// Gets or sets the current line.
        /// </summary>
        public int CurrentLine
        {
            get { return (int) GetValue(CurrentLineProperty); }
            set { SetValue(CurrentLineProperty, value); }
        }

        #endregion

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Static constructor.
        ///   Load our pc highlighting definition.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static IdeEditor()
        {

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Register highlighters.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void RegisterHighlighters()
        {
            const string PC_EXT = ".pc";
            const string PC_LST = ".pclst";
            const string PC_XSHD = "Pc.PcHighlighting.xshd";
            RegisterHighlighter(PC_XSHD, new[] {PC_EXT, PC_LST});
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Register highlighter. File must be in the the assembly relativePath  for this Folder for instance: Folder = '.pc'
        /// in relativePath = 'pc' path file name = pc/pchighighting.xshd.
        /// </summary>
        /// <param name="relativePath">         the relative path. </param>
        /// <param name="applyToExtensions">    The apply to extensions. array of valid extensions to use this on. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void RegisterHighlighter(string relativePath, string[] applyToExtensions)
        {
            //"IdeEditor.Pc.PcHighlighting.xshd"
            var fileName = string.Format("{0}.{1}", Assembly.GetExecutingAssembly().GetName().Name, relativePath);

            using (var s = typeof (IdeEditor).Assembly.GetManifestResourceStream(fileName))
            {
                if (s == null)
                {
                    Debug.Print("Could not find {0} embedded resource", fileName);
                    return;
                }
                using (XmlReader reader = new XmlTextReader(s))
                {
                    var highlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    HighlightingManager.Instance.RegisterHighlighting(highlighting.Name, applyToExtensions, highlighting);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IdeEditor()
        {

            TextArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Replace, ReplaceExecute,ReplaceCanExecute));
            TextArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, FindExecute, ReplaceCanExecute));
            TextArea.CommandBindings.Add(new CommandBinding(GotoLineDialog.GotoLine, GotoExecute));

            CurrentLineHighligher = new HighlightCurentLineRenderer( this, Colors.Yellow, Colors.Blue );

            SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);

            Loaded   += OnLoaded;
            Unloaded += OnUnloaded;

            OnDocumentChanged(null);

            //  FontFamily = new FontFamily("Consolas");

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the unloaded action. remove any listeners free up timer.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            TextArea.TextEntering          -= textEditor_TextArea_TextEntering;
            TextArea.TextEntered           -= textEditor_TextArea_TextEntered;
            TextArea.SelectionChanged      -= TextArea_SelectionChanged;
            TextArea.Caret.PositionChanged -= TextArea_CaretPositionChanged;

            if (_foldingUpdateTimer != null)
            {
                _foldingUpdateTimer.Stop();
            }

            _foldingUpdateTimer = null;
            Messenger.DeRegister( this );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called when [full path changed]. This is a bit of a hack.
        /// </summary>
        /// <param name="d">    The d. </param>
        /// <param name="e">    The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void OnFullPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (IdeEditor)d;
            var newValue = (string)e.NewValue;
            var oldValue = (string)e.OldValue;
            target.FullPath = string.IsNullOrEmpty( newValue ) ? oldValue : newValue;
  
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Raises the text changed event.
        /// </summary>
        /// <param name="e">    Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnTextChanged(EventArgs e)
        {
            ++_changeCount;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the loaded action. add listeners setup folding timer.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var vmarkers   = new TextMarkerView( this );
            if (IconBarManager != null)
            {
                var margin = new IconBarMargin(IconBarManager);
                TextArea.LeftMargins.Add( margin );
            }

            TextArea.TextEntering          += textEditor_TextArea_TextEntering;
            TextArea.TextEntered           += textEditor_TextArea_TextEntered;
            TextArea.SelectionChanged      += TextArea_SelectionChanged;
            TextArea.Caret.PositionChanged += TextArea_CaretPositionChanged;

            _foldingUpdateTimer          = new DispatcherTimer();
            _foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            _foldingUpdateTimer.Tick     += delegate { UpdateFoldings(); };
            _foldingUpdateTimer.Start();
            SetLanguage();

            Messenger.Register( this );
        }

        #endregion


        #region Markers

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Raises the <see cref="TextEditorWeakEventManager.DocumentChanged"/> event.
        /// </summary>
        /// <param name="e">    Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnDocumentChanged(EventArgs e)
        {
            base.OnDocumentChanged(e);
            //TODO: try and go back to simple binding
            var document = Document;
            if (document != null && document.FileName!=null)
            {
                doc = document;
                //var markerService = new TextMarkerService( this );
                //TextMarkerService = markerService;
                //InitializeTextMarkerService();
                //TextArea.TextView.BackgroundRenderers.Add( markerService );
                //TextArea.TextView.LineTransformers.Add( markerService );
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the text marker service.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void InitializeTextMarkerService()
        {
            if (doc != null)
            {
                var services = (IServiceContainer)doc.ServiceProvider.GetService( typeof( IServiceContainer ) );
                if (services != null)
                {
                    services.RemoveService(typeof(ITextMarkerService));
                    services.AddService( typeof( ITextMarkerService ), TextMarkerService );
                    TextMarkerService.Document = doc;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Query if 'marker' is selected.
        /// </summary>
        /// <param name="marker">   The marker. </param>
        /// <returns>
        /// true if selected, false if not.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool IsSelected(ITextMarker marker)
        {
            var selectionEndOffset = SelectionStart + SelectionLength;
            return marker.StartOffset >= SelectionStart && marker.StartOffset <= selectionEndOffset ||
                   marker.EndOffset   >= SelectionStart && marker.EndOffset   <= selectionEndOffset;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes the marker at offset described by offset.
        /// </summary>
        /// <param name="offset">   The offset. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RemoveMarkerAtOffset(int offset)
        {
            TextMarkerService.RemoveAll(m=> m.StartOffset >= offset && m.EndOffset < offset);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes all click.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveAllMarkers()
        {
            TextMarkerService.RemoveAll(m => true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes the selected marker.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RemoveSelectedMarker()
        {
            TextMarkerService.RemoveAll(IsSelected);
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Goto execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void GotoExecute(object sender, ExecutedRoutedEventArgs e)
        {
           var line = GotoLineDialog.ShowGotoDialog(doc.LineCount);

            if (line >= 0)
            {
                GotoLine(line);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Goto line.
        /// </summary>
        /// <param name="number">   Number of. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GotoLine(int number)
        {
            if (doc != null)
            {
                if (number < 0) number = 0;
                if (number > doc.LineCount) number = doc.LineCount;

                var l = doc.GetLineByNumber(number);

                Select(l.Offset,1);
           
                var loc = doc.GetLocation(l.Offset);
                ScrollTo(loc.Line, loc.Column);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Goto line.
        /// </summary>
        /// <param name="offset">   The offset. </param>
        /// <param name="length">   (optional) the length. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GotoOffset(int offset, int length=0)
        {
            if (doc != null)
            {
                var loc = doc.GetLocation(offset );
                ScrollToLine(loc.Line);
                ScrollTo( loc.Line, loc.Column );
                if (length == 0)
                {
                    SelectWordAtOffset(offset);
                }
                else
                {
                    Select(offset,length);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Select word at offset.
        /// </summary>
        /// <param name="offset">   The offset. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SelectWordAtOffset(int offset)
        {
            try
            {
                var start = TextUtilities.GetNextCaretPosition(doc, offset, LogicalDirection.Backward,CaretPositioningMode.WordStartOrSymbol);
                var end = TextUtilities.GetNextCaretPosition(doc, offset, LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol);
                Select(start, end - start);
            }
            catch (Exception )
            {
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Replace can execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Can execute routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ReplaceCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FindExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ShowFindReplaceDialog(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Replace execute.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Executed routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ReplaceExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ShowFindReplaceDialog(false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the find replace dialog.
        /// </summary>
        /// <param name="showFind"> (optional) true to show, false to hide the find. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ShowFindReplaceDialog(bool showFind = true)
        {
            FindReplaceDialog.ShowForReplace( this, showFind );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by textEditor_TextArea for text entered events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text composition event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                //// open code completion after the user has pressed dot:
                //_completionWindow = new CompletionWindow( TextArea );
                //// provide AvalonEdit with the data:
                //var data = _completionWindow.CompletionList.CompletionData;
                //data.Add( new PcCompletionData( "Item1" ) );
                //data.Add( new PcCompletionData( "Item2" ) );
                //data.Add( new PcCompletionData( "Item3" ) );
                //data.Add( new PcCompletionData( "Another item" ) );
                //_completionWindow.Show();
                //_completionWindow.Closed += delegate { _completionWindow = null; };
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by textEditor_TextArea for text entering events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text composition event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit( e.Text[0] ))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion( e );
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler to update properties based upon the selection changed event.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void TextArea_SelectionChanged(object sender, EventArgs e)
        {
            SelectionStart = SelectionStart;
            SelectionLength = SelectionLength;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event that handles when the caret changes.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TextArea_CaretPositionChanged(object sender, EventArgs e)
        {
            try
            {
                canScroll    = false;
               TextLocation  = TextLocation;
               CaretPosition = CaretOffset;

            }
            finally
            {
                canScroll = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseWheel" /> attached event reaches an
        /// element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="args"> The <see cref="T:System.Windows.Input.MouseWheelEventArgs" /> that contains the event data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs args)
        {
            base.OnPreviewMouseWheel( args );
            if (Keyboard.IsKeyDown( Key.LeftCtrl ) || Keyboard.IsKeyDown( Key.RightCtrl ))
            {
                ScaleFactor += (args.Delta > 0) ? 0.1 : -0.1;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseDown" /> attached routed event
        /// reaches an element in its route that is derived from this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="args"> The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data.
        ///                     The event data reports that one or more mouse buttons were pressed. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnPreviewMouseDown(MouseButtonEventArgs args)
        {
            base.OnPreviewMouseDown( args );
            if (Keyboard.IsKeyDown( Key.LeftCtrl ) || Keyboard.IsKeyDown( Key.RightCtrl ))
            {
                if (args.MiddleButton == MouseButtonState.Pressed)
                {
                    ScaleFactor = 1.0;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// New message.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="key">      The key. </param>
        /// <param name="payload">  The payload. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public MessageResult NewMessage(IMessageListener sender, string key, object payload)
        {
            switch (key)
            {
                case "gotoerr":
                    var error = (Error)payload;
                    GotoOffset(error.Offset,error.Length);
                    return MessageResult.HANDLED_STOP;
             
                case "goto":
                    GotoLine((int)payload);
                    return MessageResult.HANDLED_STOP;

                case "refresh":
                    TextArea.TextView.Redraw();
                    return MessageResult.HANDLED_STOP;
                    ;
                    //case "error":
                    //case "warining":
                    //case "remove":
                    //     AddMarker((Error)payload );
                    //    return MessageResult.HANDLED_STOP;
            }
            return MessageResult.IGNORED;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sends a message to all listeners.  The listeners provide the filtering.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="payload">  The payload. </param>
        /// <returns>
        /// true if it is handled by at least one listener, false if no listeners process it.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Send(string message, object payload)
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Match identifier calls each registered listener, if the listener matches the identifier, then the listener
        /// returns true. Names are v:fullpath.
        /// </summary>
        /// <param name="identifier">   The identifier. </param>
        /// <returns>
        /// true if it matches the identifier.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool MatchIdentifier(string identifier)
        {
            var fullPath = FullPath;
            return  fullPath!=null  && identifier.Equals(fullPath);
          //  return fullPath != null && fullPath.StartsWith( "v:" ) && identifier.Equals( fullPath.Substring( 2 ) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns the string representation of a <see cref="T:System.Windows.Controls.Control" /> object.
        /// </summary>
        /// <returns>
        /// A string that represents the control.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return ("View: " +( FullPath ?? "<no doc>"));
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Opens a file click.
        ///// </summary>
        ///// <param name="sender">   Source of the event. </param>
        ///// <param name="e">        Routed event information. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void OpenFileClick(object sender, RoutedEventArgs e)
        //{
        //    var dlg = new OpenFileDialog();
        //    dlg.CheckFileExists = true;
        //    if (dlg.ShowDialog() ?? false)
        //    {
        //        currentFileName = dlg.FileName;
        //        Load( currentFileName );
        //        SyntaxHighlighting =
        //            HighlightingManager.Instance.GetDefinitionByExtension( Path.GetExtension( currentFileName ) );
        //    }
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Property grid combo box selection changed.
        ///// </summary>
        ///// <param name="sender">   Source of the event. </param>
        ///// <param name="e">        Routed event information. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void PropertyGridComboBoxSelectionChanged(object sender, RoutedEventArgs e)
        //{
        //    //if (propertyGrid == null)
        //    //{
        //    //    return;
        //    //}
        //    //switch (propertyGridComboBox.SelectedIndex)
        //    //{
        //    //    case 0:
        //    //        propertyGrid.SelectedObject = textEditor;
        //    //        break;
        //    //    case 1:
        //    //        propertyGrid.SelectedObject = TextArea;
        //    //        break;
        //    //    case 2:
        //    //        propertyGrid.SelectedObject = Options;
        //    //        break;
        //    //}
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Process the errors.
        ///// </summary>
        ///// <param name="sender">   Source of the event. </param>
        ///// <param name="e">        Notify collection changed event information. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void ProcessErrors(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    RemoveAllMarkers();
        //    foreach (var error in ErrorList)
        //    {
        //        if (error.FilePath.FullPath == Document.FileName)
        //        {
        //            AddMarker( error );
        //        }
        //    }
        //}
    }
}
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
// Copyright:
//  Copyright Advanced Performance
//  All Rights Reserved THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Advanced Performance
//  The copyright notice above does not evidence any actual or intended publication of such source code.
//  Some third-party source code components may have been modified from their original versions
//  by Advanced Performance. The modifications are Copyright Advanced Performance., All Rights Reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using IDECore.Dialogs;
using Microsoft.Win32;

namespace IdeCore.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interaction logic for IdeEditor.xaml.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class IdeEditor : TextEditor
    {
        /// <summary>
        /// The completion window.
        /// </summary>
        private CompletionWindow completionWindow;

        /// <summary>
        /// Filename of the current file.
        /// </summary>
        private string currentFileName;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is invalid. </exception>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IdeEditor()
        {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (var s = typeof (IdeEditor).Assembly.GetManifestResourceStream("AvalonEdit.Sample.CustomHighlighting.xshd")){
                if (s == null)
                {
                    throw new InvalidOperationException("Could not find embedded resource");
                }
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new[] {".cool"}, customHighlighting);


          //  InitializeComponent();
            SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
         //   propertyGridComboBox.SelectedIndex = 2;

            //TextArea.SelectionBorder = null;

            //SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            //SyntaxHighlighting = customHighlighting;
            // initial highlighting now set by XAML

            TextArea.TextEntering += textEditor_TextArea_TextEntering;
            TextArea.TextEntered += textEditor_TextArea_TextEntered;

            var foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            foldingUpdateTimer.Start();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Opens a file click.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                currentFileName = dlg.FileName;
                Load(currentFileName);
                SyntaxHighlighting =
                    HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Property grid combo box selection changed.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void PropertyGridComboBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            //if (propertyGrid == null)
            //{
            //    return;
            //}
            //switch (propertyGridComboBox.SelectedIndex)
            //{
            //    case 0:
            //        propertyGrid.SelectedObject = textEditor;
            //        break;
            //    case 1:
            //        propertyGrid.SelectedObject = TextArea;
            //        break;
            //    case 2:
            //        propertyGrid.SelectedObject = Options;
            //        break;
            //}
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves a file click.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SaveFileClick(object sender, EventArgs e)
        {
            if (currentFileName == null)
            {
                var dlg = new SaveFileDialog();
                dlg.DefaultExt = ".txt";
                if (dlg.ShowDialog() ?? false)
                {
                    currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            Save(currentFileName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shows the find replace dialog.
        /// </summary>
        /// <param name="ShowFind"> (optional) true to show, false to hide the find. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ShowFindReplaceDialog(bool ShowFind = true)
        {
            // find the Editor
            FindReplaceDialog.ShowForReplace(this);

            // Window dlg = null;
            //try
            //
            //    if (FindReplaceVM == null)
            //    {
            //        FindReplaceVM = new FindReplaceViewModel(mSettingsManager);
            //    }

            //    FindReplaceVM.FindNext = FindNext;

            //    // determine whether Find or Find/Replace is to be executed
            //    FindReplaceVM.ShowAsFind = ShowFind;

            //    if (f.TxtControl != null) // Search by default for currently selected text (if any)
            //    {
            //        string textToFind;
            //        f.TxtControl.GetSelectedText(out textToFind);

            //        if (textToFind.Length > 0)
            //        {
            //            FindReplaceVM.TextToFind = textToFind;
            //        }
            //    }

            //    FindReplaceVM.CurrentEditor = f;

            //    dlg = ViewSelector.GetDialogView(FindReplaceVM, Application.Current.MainWindow);

            //    dlg.Closing += FindReplaceVM.OnClosing;

            //    dlg.ShowDialog();
            //}
            //catch (Exception exc)
            //{
            //    Msg.Show(exc, Strings.STR_MSG_FIND_UNEXPECTED_ERROR,
            //        MsgBoxButtons.OK, MsgBoxImage.Error);
            //}
            //finally
            //{
            //    if (dlg != null)
            //    {
            //        dlg.Closing -= FindReplaceVM.OnClosing;
            //        dlg.Close();
            //    }
            //}
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
                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(TextArea);
                // provide AvalonEdit with the data:
                var data = completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("Item1"));
                data.Add(new MyCompletionData("Item2"));
                data.Add(new MyCompletionData("Item3"));
                data.Add(new MyCompletionData("Another item"));
                completionWindow.Show();
                completionWindow.Closed += delegate { completionWindow = null; };
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
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        #region Folding

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Manager for folding.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private FoldingManager foldingManager;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The folding strategy.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private object foldingStrategy;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by HighlightingComboBox for selection changed events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Selection changed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {
                switch (SyntaxHighlighting.Name)
                {
                    case "XML":
                        foldingStrategy = new XmlFoldingStrategy();
                        TextArea.IndentationStrategy = new DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        TextArea.IndentationStrategy = new CSharpIndentationStrategy(Options);
                        foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        TextArea.IndentationStrategy = new DefaultIndentationStrategy();
                        foldingStrategy = null;
                        break;
                }
            }
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                {
                    foldingManager = FoldingManager.Install(TextArea);
                }
                UpdateFoldings();
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Updates the foldings.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateFoldings()
        {
            if (foldingStrategy is BraceFoldingStrategy)
            {
                ((BraceFoldingStrategy) foldingStrategy).UpdateFoldings(foldingManager, Document);
            }
            if (foldingStrategy is XmlFoldingStrategy)
            {
                ((XmlFoldingStrategy) foldingStrategy).UpdateFoldings(foldingManager, Document);
            }
        }

        #endregion
    }
}
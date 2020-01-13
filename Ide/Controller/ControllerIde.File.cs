////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the controller ide. file class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/8/2015   rcs     Initial Implementation
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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Ide.Model;
using Ide.Services;
using Microsoft.Win32;
using ViewModels;
using ZCore;
using ZCore.Util;

namespace Ide.Controller
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Controller ide. File operations
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class ControllerIde
    {
        private readonly ServiceFile _serviceFile;
    
        /// <summary>
        /// Gets or sets the file service.
        /// </summary>
        public ServiceFile FileService { get { return _serviceFile; } }


        /// <summary>
        ///  Get application Settings.
        /// </summary>
        public static AppSettings AppSettings {get { return AppSettings.Default; }}

        /// <summary>
        /// The _watcher
        /// </summary>
        private FileSystemWatcher _watcher;

        /// <summary>
        /// The watch timer, fixed bug in watcher firing multiple time
        /// </summary>
        private DateTime watchTimer;

        #region File Watcher

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a watcher. 
        /// </summary>
        /// <param name="basePath"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void AddWatcher(string basePath)
        {
            _watcher = FileService.AddFileWatcher( FileUtil.GetFolder( basePath ) );
            // Add event handlers.
            _watcher.Changed += FileChangedExternally;
            _watcher.Deleted += FileDeletedExternally;
            _watcher.Renamed += FileRenamedExternally;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File changed by external app
        /// Use dispatch to send to UI thread.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        File system event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FileChangedExternally(object sender, FileSystemEventArgs e)
        {
            try
            {
                var elapsedSeconds = watchTimer.ElapsedSeconds();
                Debug.Print("Watcher time = {0}",elapsedSeconds);
                if (elapsedSeconds <8)
                {
                    return;
                }
               
                watchTimer = DateTime.Now;

                _watcher.EnableRaisingEvents = false;
                var filePath                 = e.FullPath;
               
                var vm                       = GetViewModelFileEdit( filePath );
                if (vm != null)
                {
                    DateTime fileTime = File.GetLastWriteTime(filePath);
                    Debug.Print( "fileTime = {0}  VM file time={1}", fileTime,vm.FilePath.SavedTime );
                    if (vm.FilePath.SavedTime.AddSeconds(2) >= fileTime)
                    {
                        _watcher.EnableRaisingEvents = true;
                        return;
                    }


                    if (AppSettings.AutoLoadChangedFiles)
                    {
                        // auto reload
                        ExternalReloadFile( vm, filePath );
                        return;
                    }
                    // ask user now.
                    var result = UserMessage.ShowQuestion( "File: " + filePath, "Has Changed.  Reload?" );

                    if (result )
                    {
                        ExternalReloadFile(vm, filePath);
                    }
                }
            }
            catch (Exception )
            {
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// External reload file. on external thread.
        /// </summary>
        /// <param name="vm">       The virtual memory. </param>
        /// <param name="filePath"> Full pathname of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ExternalReloadFile(ViewModelFileEdit vm, string filePath)
        {
            // this must be on the UI thread so dispatch it.
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                //  VMActiveDocument = vm;
                var adoc = VMActiveDocument;
                FileClose( vm );
                FileOpen(new ModelFile(filePath));
                VMActiveDocument = adoc;
            } ) );

            // restore current document.
            if (!_watcher.EnableRaisingEvents)
            {
                _watcher.EnableRaisingEvents = true;
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File renamed.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Renamed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FileRenamedExternally(object sender, RenamedEventArgs e)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File deleted.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        File system event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FileDeletedExternally(object sender, FileSystemEventArgs e)
        {
        }

        #endregion

        #region File Operations

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Opens a file using a dialog box.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileOpen()
        {
            var dlg = new OpenFileDialog();
            //TODO: use solution specific file filter.
            dlg.Filter = Constants.OPEN_FILE_FILTER;
            dlg.FilterIndex = 0;
            dlg.Multiselect = true;

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                foreach (var fileName in dlg.FileNames)
                {
                    var fileViewModel = FileOpen( _serviceFile.FileOpen( fileName ) );
                    VMActiveDocument = fileViewModel;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File export text to html.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileExportTextToHtml()
        {
            if (VMActiveDocument != null)
            {
                VMActiveDocument.ExportToHtml( VMActiveDocument.FileName + ".html", true, false );
                //this.mSettingsManager.SettingData.TextToHTML_ShowLineNumbers,
                //this.mSettingsManager.SettingData.TextToHTML_AlternateLineBackground);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File save as.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileSaveAs()
        {
            if (VMActiveDocument != null)
            {
                ViewModelFileEdit doc = VMActiveDocument;
                if (FileSave( doc, true ))
                {
                    //this.mSettingsManager.SessionData.MruList.AddMRUEntry(VMActiveDocument.FilePath);
                    //this.mSettingsManager.SessionData.LastActiveFile = VMActiveDocument.FilePath;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves the given file.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FileSave()
        {
            try
            {
                if (_watcher != null)
                {
                    _watcher.EnableRaisingEvents = false;
                }
                if (VMActiveDocument != null)
                {
                    var doc = VMActiveDocument;
                    FileSave( doc );
                }
                if (_watcher != null)
                {
                    _watcher.EnableRaisingEvents = true;
                }
            }
            catch (Exception e)
            {
                UserMessage.ShowException(e,"File save failed");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File save all.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FileSaveAll()
        {
            try
            {
                if (_watcher != null)
                {
                    _watcher.EnableRaisingEvents = false;
                }
                // Save all edited documents
                if (Files != null) // Close all open files and make sure there are no unsaved edits
                {
                    // If there are any: Ask user if edits should be saved
                    var activeDoc = VMActiveDocument;

                    try
                    {
                        for (var i = 0; i < Files.Count; i++)
                        {
                            var f = Files[i];

                            if (f != null)
                            {
                                if (f.IsDirty && !f.IsReadOnly)
                                {
                                    VMActiveDocument = f;
                                    FileSave( f );
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        if (activeDoc != null)
                        {
                            VMActiveDocument = activeDoc;
                        }
                    }
                }

                // Save program settings
                // this.SaveConfigOnAppClosed();
                if (_watcher != null)
                {
                    _watcher.EnableRaisingEvents = true;
                }
            }
            catch
            {
            }

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File delete.
        /// </summary>
        /// <param name="vmFile">   The virtual memory file. </param>
        /// <param name="typeName"> Name of the type. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool FileDelete(ViewModelTreeFile vmFile, string typeName)
        {
            var vm = GetViewModelFileEdit( vmFile.FullPath );
            if (vm != null)
            {
                FileClose( vm );
            }

            if (!FileUtil.FileDelete( vmFile.FullPath ))
            {
                UserMessage.ShowAsterik( "Can't delete " + typeName, typeName + ": '" + vmFile.FullPath );
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Closes the given file.
        /// </summary>
        /// <param name="fileToClose">  The file to close. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileClose(ViewModelFileEdit fileToClose)
        {
            if (FileSaveQueyIfDirty(fileToClose)) return;

            _files.Remove(fileToClose);
            RemoveListener(fileToClose.FilePath.FullPath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File save quey if dirty.
        /// </summary>
        /// <param name="fileToClose">  The file to close. </param>
        /// <param name="showCancel">   (optional) true to show, false to hide the cancel. </param>
        /// <returns>
        /// true cancelled, else file is saved if requested or ignored if no.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool FileSaveQueyIfDirty(ViewModelFileEdit fileToClose, bool showCancel=true)
        {
            if (fileToClose.IsDirty)
            {
                var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", fileToClose.FileName),
                    " document", showCancel? MessageBoxButton.YesNoCancel:MessageBoxButton.YesNo);
                switch (res)
                {
                    case MessageBoxResult.Cancel:
                        return true;
                    case MessageBoxResult.Yes:
                        FileSave(fileToClose);
                        break;
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File new.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FileNew()
        {
            //TODO: this is coming from the tree selection so have a ViewModelFileEdit after adding to current location in the tree, add suffix?
            // show a dialog box of where to create it
            //  var modelFile       = new ViewModelFileEdit( new ModelFile( "newfile" ),this ) { Document = new TextDocument()};
            ViewModelProject folder = VMWorkspace.CurrentProject();
            var path                = folder.Folder.Folder;
            var ext                 = folder.Project.FileFilter[0];
            var fileName            = "NewFile";
            var baseName            = fileName + ext;
            string fullpath;


            var list = new TemplateList();
            list.LoadAllTemplates( ViewModels.Constants.TEMPLATE_BUILT_IN, ViewModels.Constants.TEMPLATE_ALTERATE, "File" );

            // get template if it exists
            // 
            var template = list.FirstOrDefault( x => x.Name.EqualsIgnoreCase( ext.TrimStart( '.' ) ) );
            if (template != null)
            {
                fileName = template.DefaultFile;
                baseName = template.DefaultName + ext;
                fullpath = FileUtil.FileCreateUnique(path, baseName );
                template.CreateFile(fileName, fullpath);
                var text = Substitue(FileUtil.FileReadText(fullpath), folder.Name );
                FileUtil.FileWriteText(fullpath, text);
            }
            else
            {
                fullpath = _serviceFile.FileCreate( path, baseName );
            }
            
           
   
            //var modelFile = new ViewModelFileEdit( new ModelFile( "newfile" ), this );

            WorkspaceRefresh();

            var vm   = VMWorkspace.CurrentSolution.FindSolutionFileVM(fullpath);
            FileOpen(vm.ModelFile);
        

            if( vm.RenameCanExecute())
            {
                vm.RenameExecute();
            }

            // setup any new text template for this file
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Substitues.
        /// </summary>
        /// <param name="mText">    The text. </param>
        /// <param name="folder">   Pathname of the folder. </param>
        /// <returns>
        /// text modified
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string Substitue(string mText, string folder )
        {
            mText = mText.Replace("$DATE", DateTime.Now.Date.ToShortDateString());
            mText = mText.Replace("$USER",VMWorkspaceOptions.UserName);
            mText = mText.Replace("$PROJNAME",folder);
            mText = mText.Replace( "$CLASS", VMWorkspaceOptions.UserClass);
            return mText;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Opens the given file.
        /// </summary>
        /// <param name="modelFile"> The modelFile. </param>
        /// <returns>
        ///     .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelFileEdit FileOpen(ModelFile modelFile)
        {
            var fileViewModel = _files.FirstOrDefault(fm => fm.FilePath.Equals(modelFile));
            if (fileViewModel != null)
            {
                VMActiveDocument = fileViewModel;
                return fileViewModel;
            }

            fileViewModel = new ViewModelFileEdit(modelFile, this);
            _files.Add(fileViewModel);
            VMActiveDocument = fileViewModel;

            return fileViewModel;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Saves the given file.
        /// </summary>
        /// <param name="doc">   The file to save. </param>
        /// <param name="saveAs">   (optional) true to save as flag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool FileSave(ViewModelFileEdit doc, bool saveAs = false)
        {
            if (doc == null || doc.IsReadOnly)
            {
                return false;
            }
            
            var path = doc.FilePath.Path;
            if (saveAs || (doc.FilePath != null && !path.Exists()))
            {

                var dlg = new SaveFileDialog();
                dlg.FileName = path.FileNameAndExtension;
                dlg.Filter = Constants.SAVE_FILE_FILTER;
                // dlg.InitialDirectory = this.GetDefaultPath();

                if (dlg.ShowDialog().GetValueOrDefault())
                {
                    doc.FilePath = new ModelFile(dlg.SafeFileName);
                }
                else
                {
                    return false;
                }
            }

            doc.FilePath.SavedTime = DateTime.Now;
            _serviceFile.FileSave( doc.FilePath, doc.Text );
           // don't change the is dirty flag so edits can pass over changes
            doc.IsDirty = false;
            doc.Document.UndoStack.MarkAsOriginalFile();
            return true;
        }

        #endregion
    }
}

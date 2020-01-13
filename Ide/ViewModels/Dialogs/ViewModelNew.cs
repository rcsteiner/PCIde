////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the view model new class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/1/2015   rcs     Initial Implementation
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
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using Ide;
using Ide.Controller;
using Views;
using Views.Support;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// View model new. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    internal class ViewModelNew : ViewModelDialog, IDataErrorInfo
    {
        #region Fields

        private Template     _template;
        private string       _textNewWhat;
        private string       _description;
        private string       _name;
        private string       _folder;
        private string       _folderLabel;
        private bool         _showFolder;
        private TemplateList _templateList;


        #endregion


        /// <summary>
        /// show the folder or not.
        /// </summary>
        public bool ShowFolder
        {
            get { return _showFolder; }
            set
            {
                if (value != _showFolder)
                {
                    _showFolder = value;
                    RaisePropertyChanged( "ShowFolder" );
                }
            }
        }

        /// <summary>
        /// Gets the text new folder.
        /// </summary>
        public string Folder { get { return _folder; } set
        {
            if (value != _folder)
            {
                _folder = value; 
                RaisePropertyChanged("Folder");
            }
        } }

        /// <summary>
        /// Gets the text new folder.
        /// </summary>
        public string FolderLabel { get { return _folderLabel; } set
        {
            if (value != _folderLabel)
            {
                _folderLabel = value; 
                RaisePropertyChanged("FolderLabel");
            }
        } }

        /// <summary>
        /// Gets the text new title.
        /// </summary>
        public string TextNewWhat {get { return _textNewWhat; }}

        /// <summary>
        /// Gets the new template.
        /// </summary>
        public Template NewTemplate
        {
            get { return _template; }
            set
            {
                if (_template != value)
                {
                    _template = value;
                    RaisePropertyChanged("NewTemplate");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string NameWatermark
        {
            get { return _nameWatermark; }
            set
            {
                if (value != _nameWatermark)
                {
                    _nameWatermark = value;
                    RaisePropertyChanged( "NameWatermark" );
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string DescriptionWatermark
        {
            get { return _descriptionWatermark; }
            set
            {
                if (value != _descriptionWatermark)
                {
                    _descriptionWatermark = value;
                    RaisePropertyChanged( "DescriptionWatermark" );
                }
            }
        }


        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Gets or sets the template list.
        /// </summary>
        public TemplateList TemplateList
        {
            get { return _templateList; }
            set 
            {
                _templateList = value;
                RaisePropertyChanged("TemplateList");
            }
        }

        private RelayCommand _browseFolderCommand;
        private string _nameWatermark;
        private string _descriptionWatermark;

        /// <summary>
        /// Gets the item activate command.
        /// </summary>
        public ICommand BrowseFolderCommand
        {
            get
            {
                return _browseFolderCommand ?? (_browseFolderCommand = new RelayCommand( BrowseFolderExecute, BrowseFolderCanExecute ));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Indexer to get items within this collection using array index syntax. Validation
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string this[string columnName]
        {
            get
            {
                string validationResult = null;
                switch (columnName)
                {
                    case "Folder":
                        validationResult = ValidateFolder();
                        break;
                    case "Name":
                        validationResult = ValidateName();
                        break;
                    case "NewTemplate":
                        validationResult = ValidateNewTemplate();
                        break;
                }
                CommandManager.InvalidateRequerySuggested();
                return validationResult;
            }
        }

        /// <summary>
        /// Gets or sets an error message indicating what is wrong with this object.
        /// </summary>
        public string Error { get; private set; }

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title">title is new.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="view">The view.</param>
        /// <param name="showFolder">if set to <c>true</c> [show folder].</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelNew(string title, ControllerIde controller=null, IView view=null,bool showFolder=true) : base(controller, view)
        {
             ShowFolder           = showFolder;
            Title                 = title;
            _textNewWhat          = "New " + Title;
            DescriptionWatermark  = "Description of " + Title;
           NameWatermark          = "Name of " + Title;
           FolderLabel            = "Folder for " + Title;
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Validate new template.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string ValidateNewTemplate()
        {
            return NewTemplate == null ? "You must select a template in the template list." : String.Empty;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Validate name.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string ValidateName()
        {
            return string.IsNullOrEmpty( Name ) ? "Name is required." : String.Empty;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Validate folder.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string ValidateFolder()
        {
            if (!ShowFolder) return "";
            return (FileUtil.FolderExists(Folder))
                ? String.Empty
                : "The folder does not exist.  Create the folder first with the browse button.";
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Browse folder can execute.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool BrowseFolderCanExecute()
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Browse folder execute.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BrowseFolderExecute()
        {
            var dlg = new FolderBrowserDialog();
            dlg.SelectedPath = Folder;
            dlg.Description = _folderLabel;
            dlg.ShowNewFolderButton = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Folder = dlg.SelectedPath;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Ok command execute.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override bool OkCommandCanExecute()
        {
            return NewTemplate != null && !string.IsNullOrEmpty( Name ) && (!ShowFolder || !string.IsNullOrEmpty( Folder ));
        }
    }
}
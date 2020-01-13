////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the tree view item view model class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/25/2015   rcs     Initial Implementation
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

using System.Windows.Input;
using System.Windows.Media;
using Ide.Controller;
using Views.Support;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Base class for all ViewModel classes displayed by TreeViewItems.
    ///     This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public abstract class ViewModelTreePath : ViewModelIconItem
    {

        protected string            oldPath;
     
        private ViewModelTreeFolder _parentFolder;
        private bool                _isEditingName;
        private RelayCommand        _deleteCommand;
        private RelayCommand        _renameCommand;

        /// <summary>
        /// The type name like folder, file, solution etc.
        /// </summary>
        public abstract string TypeName { get; }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        public virtual ImageSource Icon { get { return IconSource; } }

        /// <summary>
        /// Gets the ParentFolder.
        /// </summary>
        public ViewModelTreeFolder ParentFolder {get { return _parentFolder; }}
        
        /// <summary>
        /// Gets or sets the full pathname of the file.
        /// </summary>
        public abstract string FullPath { get; set; }

        /// <summary>
        /// True/False whether we are changing the name of the this node
        /// </summary>
        public bool IsEditingName
        {
            get { return _isEditingName; }
            set
            {
                if (value != _isEditingName)
                {
                    if (_isEditingName)
                    {
                        RenameExecute();
                    }
                    _isEditingName = value;
                    RaisePropertyChanged( "IsEditingName" );
                }
            }
        }


        /// <summary>
        /// Title on tree node shown in textbox
        /// </summary>
        public override string Title
        {
            get { return _title; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                    IsEditingName = false;
                }
            }
        }

        #region Commands and variables

        /// <summary>
        ///     Gets the save command.
        /// </summary>
        public  ICommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new RelayCommand( DeleteExecute, DeleteCanExecute )); }
        }

        /// <summary>
        ///     Gets the save command.
        /// </summary>
        public  ICommand RenameCommand
        {
            get { return _renameCommand ?? (_renameCommand = new RelayCommand( RenameExecute, RenameCanExecute )); }
        }

        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        /// <param name="parentFolder"> The ParentFolder. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected ViewModelTreePath(ControllerIde controller, ViewModelTreeFolder parentFolder) :base(controller)
        {
            _parentFolder = parentFolder;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Rename can execute.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool RenameCanExecute()
        {
            return !_isEditingName;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Rename execute.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void RenameExecute()
        {
            if (!_isEditingName)
            {
                oldPath       = FullPath;
                IsEditingName = true;
                return;
            }
            // editing so complete it
            _isEditingName = false;
          
            Rename();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Renames the file
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual bool Rename()
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Delete can execute.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual bool DeleteCanExecute()
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Delete execute.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void DeleteExecute()
        {
            Delete();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Deletes this object.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual bool Delete()
        {
            ParentFolder.Children.Remove(this);
            return true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return FullPath;
        }

    }
}
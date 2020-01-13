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
using Ide;
using Ide.Controller;
using Ide.Model;
using Views.Support;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Base class for all ViewModel classes displayed by TreeViewItems.
    ///     This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelTreeFile : ViewModelTreePath
    {

        /// <summary>
        /// The type name like folder, file, solution etc.
        /// </summary>
        public override string TypeName { get { return "Code File"; } }

        /// <summary>
        /// Full pathname of the file.
        /// </summary>
        public ModelFile ModelFile;

        #region ItemActivate

        private RelayCommand _itemActivateCommand;

        /// <summary>
        /// Gets the item activate command.
        /// </summary>
        public ICommand ItemActivateCommand
        {
            get
            {
                return _itemActivateCommand ??(_itemActivateCommand = new RelayCommand(ActivateExecute, ActivateCanExecute));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Activates the can execute described by o.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ActivateCanExecute()
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Activates the execute described by o.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ActivateExecute()
        {
            Controller.FileOpen(ModelFile);
        }

        #endregion

        #region IsExpanded
        private bool _isExpanded;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Gets or sets/sets whether the TreeViewItem associated with this object is expanded.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    RaisePropertyChanged( "IsExpanded" );
                }

                // Expand all the way up to the root.
                if (_isExpanded && ParentFolder != null)
                {
                    ParentFolder.IsExpanded = true;
                }
            }
        }
        #endregion // IsExpanded

        #region File Path

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the full pathname of the file.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string FullPath
        {
            get { return ModelFile.FullPath; }
            set
            {
                if (ModelFile.FullPath != value)
                {
                    ModelFile.FullPath = value;
                    OnChange();
                }
            }
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the title.
        /// We don't use Name here because that causes a rename each time the IsEditing text is changed.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string Title
        {
            get { return ModelFile.NameAndExtension; }
            set
            {
                if (!string.IsNullOrEmpty(value) && ModelFile.NameAndExtension != value)
                {
                    ModelFile.NameAndExtension = value;
                    OnChange();
                }
            }
        }


        #region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelTreeFile"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="modelFile">The model file.</param>
        /// <param name="parentFolder">The parent folder.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelTreeFile(ControllerIde controller, ModelFile modelFile, ViewModelTreeFolder parentFolder):base(controller, parentFolder )
        {
           ModelFile   = modelFile;
            IconSource = IconUtil.GetImage(ModelFile.Extension);
            if (modelFile.Extension == ".pclst") IsVisible = false;
        }

        #endregion // Constructors


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the change action.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnChange()
        {
            RaisePropertyChanged( "Title" );
            RaisePropertyChanged( "FullPath" );
        }


        #region Rename command
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Renames the file
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override bool Rename()
        {
            if (oldPath == FullPath) return true;

            var old = new ZPath(oldPath);
            var newFile = new ZPath(FullPath);
           

            if (string.IsNullOrEmpty(newFile.Extension))
            {
                newFile.Extension = old.Extension;
                FullPath = newFile.FullPath;
            }

            if (!FileUtil.FileMove( oldPath, FullPath ))
            {
                UserMessage.ShowAsterik( string.Format( "{0}: '{1}' failed to rename to: '{2}", TypeName, oldPath, FullPath ), "Can't rename" + TypeName );
                FullPath = oldPath;
                return false;
            }
            Controller.VMActiveDocument.FullPath = FullPath;
            return true;
        }

        #endregion



        #region Delete command

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Deletes this object.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override bool Delete()
        {
            if (!Controller.FileDelete(this,TypeName)) return false;
            return base.Delete();
        }


        #endregion


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
            return Title;
        }
    }
}
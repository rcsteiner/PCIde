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
    public class ViewModelTreeFolder : ViewModelTreePath
    {

        /// <summary>
        /// Gets or sets the icon source if selected
        /// </summary>
        public ImageSource IconSelected { get; protected set; }

        /// <summary>
        /// The type name like folder, file, solution etc.
        /// </summary>
        public override string TypeName {get { return "Folder"; }}

        /// <summary>
        /// Gets the icon to use
        /// </summary>
        public override ImageSource Icon { get { return IsSelected || IsExpanded? IconSelected : IconSource; } }


        /// <summary>
        /// Full pathname of the file.
        /// </summary>
        public ModelFolder Folder;


        /// <summary>
        ///     Returns the logical child items of this object.
        /// </summary>
        public ObservableCollectionEx<ViewModelTreePath> Children { get { return _children; } }

 
        /// <summary>
        ///     Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild {get { return Children.Count == 1 && Children[0] == DummyChild; }}

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public override string Title
        {
            get { return Folder.Name; }
            set
            {
                if (!string.IsNullOrEmpty( value ) && Folder.Name != value)
                {
                    Folder.Name = value;
                    RaisePropertyChanged( "Title" );
                }
            }
        }
   
        #region Fields

        /// <summary>
        ///     The dummy child.
        /// </summary>
        private static readonly ViewModelTreeFolder DummyChild = new ViewModelTreeFolder();

        /// <summary>
        ///     The children.
        /// </summary>
        private readonly ObservableCollectionEx<ViewModelTreePath> _children;

        /// <summary>
        ///     true if is expanded.
        /// </summary>
        private bool _isExpanded;


        #endregion

        #region IsExpanded

        /// <summary>
        ///     Gets or sets/sets whether the TreeViewItem associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    RaisePropertyChanged( "IsExpanded" );
                    RaisePropertyChanged( "Icon" );
                }

                // Expand all the way up to the root.
                if (_isExpanded && ParentFolder != null)
                {
                    ParentFolder.IsExpanded = true;
                }

                // Lazy load the child items, if necessary.
                if (HasDummyChild)
                {
                    Children.Remove( DummyChild );
                    LoadChildren();
                }
            }
        }

        #endregion // IsExpanded

        #region File Path

        /// <summary>
        /// Gets or sets the full pathname of the file.
        /// </summary>
        public override string FullPath
        {
            get { return Folder.FullPath; }
            set
            {
                if (Folder.FullPath != value)
                {
                    Folder.FullPath = value;
                    RaisePropertyChanged( "FullPath" );
                    RaisePropertyChanged( "Title" );
                }
            }
        }

        /// <summary>
        /// Gets or sets the name and extension (used by pseudo folders like solution or project).
        /// </summary>
        public virtual string NameAndExtension
        {
            get { return Folder.NameAndExtension; }
            set
            {
                if (Folder.NameAndExtension != value)
                {
                    Folder.NameAndExtension = value;
                    RaisePropertyChanged( "NameAndExtension" );
                }
            }
        }

        #endregion

        #region ItemActivate

        private RelayCommand _itemActivateCommand;

        /// <summary>
        /// Gets the item activate command.
        /// </summary>
        public ICommand ItemActivateCommand
        {
            get
            {
                return _itemActivateCommand ?? (_itemActivateCommand = new RelayCommand( ActivateExecute, ActivateCanExecute ));
            }
        }


        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">       The controller. </param>
        /// <param name="filepath">         Full pathname of the file. </param>
        /// <param name="parent">           (Optional) The ParentFolder. </param>
        /// <param name="lazyLoadChildren"> (Optional) true to lazy load children. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected ViewModelTreeFolder(ControllerIde controller, ModelFolder filepath, ViewModelTreeFolder parent = null, bool lazyLoadChildren = false)
            : this( controller, parent, lazyLoadChildren )
        {
            Folder = filepath;
            IconSelected = IconUtil.GetImage( "folderopen" );
            IconSource = IconUtil.GetImage( "folderclosed" );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">       The controller. </param>
        /// <param name="parentFolder">     Pathname of the parent folder. </param>
        /// <param name="lazyLoadChildren"> true to lazy load children. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected ViewModelTreeFolder(ControllerIde controller, ViewModelTreeFolder parentFolder, bool lazyLoadChildren)
            : base( controller, parentFolder )
        {
            _children = new ObservableCollectionEx<ViewModelTreePath>();

            if (lazyLoadChildren)
            {
                _children.Add( DummyChild );
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This is used to create the DummyChild instance.
        ///     Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ViewModelTreeFolder(): base( null, null )
        {
        }


        #endregion // Constructors


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Activates the can execute described by o.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual bool ActivateCanExecute()
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Activates the execute described by o.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void ActivateExecute()
        {
        }

        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first virtual memory.
        /// </summary>
        /// <param name="path"> The path. </param>
        /// <returns>
        /// The found virtual memory.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected ViewModelTreeFile FindVM(string path)
        {
            foreach (var child in Children)
            {
                var edit = child as ViewModelTreeFile;
                if (edit != null && Equals( edit.FullPath, path ))
                {
                    return edit;
                }
                
                var treeFolder = child as ViewModelTreeFolder;
                if (treeFolder != null)
                {
                    var vm = treeFolder.FindVM(path);
                    if (vm != null)
                    {
                        return vm;
                    }
                }
            }
            return null;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first virtual memory.
        /// </summary>
        /// <returns>
        /// The found virtual memory.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelTreePath FindSelectedVM()
        {
            foreach (var child in Children)
            {
                if (child.IsSelected) return child;
                var treeFolder = child as ViewModelTreeFolder;
                if (treeFolder != null)
                {
                    var vm = treeFolder.FindSelectedVM();
                    if (vm != null)
                    {
                        return vm;
                    }
                }
            }
            return null;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a folder.
        /// </summary>
        /// <param name="modelFolder"> The path. </param>
        /// <returns>
        /// the folder view
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelTreeFolder AddFolder(ModelFolder modelFolder)
        {
            var viewModelFolder = new ViewModelTreeFolder(Controller, modelFolder, this );
            Children.Insert(0, viewModelFolder );
            return viewModelFolder;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a file.
        /// </summary>
        /// <param name="path"> The path. </param>
        /// <returns>
        /// the file view
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelTreeFile AddFile(ModelFile path)
        {
            var viewModelFile = new ViewModelTreeFile(Controller,path,this );
            Children.Add( viewModelFile );
            return viewModelFile;
        }


        #region LoadChildren

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Invoked when the child items need to be loaded on demand. Subclasses can override this to populate the Children
        ///     collection.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void LoadChildren()
        {
        }

        #endregion // LoadChildren


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
            if (!FileUtil.FolderDelete(FileUtil.GetFolder(FullPath)))
            {
                UserMessage.ShowAsterik( "Can't delete " + TypeName, TypeName + ": '" + FullPath );
                return false;
            }
            return base.Delete();

        }

        #endregion
    
        
        #region Rename command



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Renames the file
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override bool Rename()
        {
            if (!FileUtil.FolderMove( oldPath, Folder.FullPath ))
            {
                UserMessage.ShowAsterik( "Can't rename" + TypeName, string.Format( "{0}: '{1}' failed to rename to: '{2}", TypeName, oldPath, FullPath ));
                Folder.FullPath = oldPath;
                return false;
            }
            return true;
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
            return Folder.FullPath;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Gets the currently selected node if any
        ///// </summary>
        ///// <returns>
        ///// The found node or else null
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //protected ViewModelTreePath SelectedNode()
        //{
        //    foreach (var child in Children)
        //    {
        //        if (child.IsSelected) return child;

        //        var treeFolder = child as ViewModelTreeFolder;
        //        if (treeFolder != null)
        //        {
        //            var vm = treeFolder.SelectedNode( );
        //            if (vm != null)
        //            {
        //                return vm;
        //            }
        //        }
        //    }
        //    return null;
        //}

    }
}
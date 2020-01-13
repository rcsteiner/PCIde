////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the view model works class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/7/2015   rcs     Initial Implementation
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


using System.ComponentModel;
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
    /// View model works. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelWorkspace : ViewModelTool
    {
        private RelayCommand _closeCommand;
        private ViewModelTreePath _selected;

        public ModelWorkspace Model { get; set; }
    
        /// <summary>
        ///     Gets the close command.
        /// </summary>
        public ICommand CloseCommand { get { return _closeCommand ?? (_closeCommand = new RelayCommand( OnClose, CanClose )); } }


        /// <summary>
        /// Gets or sets the projects.
        /// </summary>
        public ObservableCollectionEx<ViewModelSolution> Solutions { get; set; }

        public ICollectionView FilteredSolutions { get; set; }

        /// <summary>
        /// Gets the current solution.
        /// </summary>
        public ViewModelSolution CurrentSolution { get { return Solutions.Count > 0 ? Solutions[0] : null; } }

        /// <summary>
        /// Gets a value indicating whether [workspace is loaded].
        /// </summary>
        public bool WorkspaceIsLoaded { get { return CurrentSolution!=null; } }

        public ViewModelTreePath Selected
        {
            get { return _selected; }
            set { _selected = value; RaisePropertyChanged("Selected"); }
        }


        #region File Path

        /// <summary>
        /// Gets or sets the full pathname of the file.
        /// </summary>
        public string FullPath
        {
            get
            {
                return CurrentSolution != null ? CurrentSolution.FullPath : "";
            }
            set
            {
                if (CurrentSolution != null && CurrentSolution.FullPath != value)
                {
                    CurrentSolution.FullPath = value;
                    RaisePropertyChanged( "FullPath" );
                }
            }
        }

        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">        The model. </param>
        /// <param name="controller">   The controller. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelWorkspace(ModelWorkspace model, ControllerIde controller) : base( model.Name, controller )
        {
            Solutions = new ObservableCollectionEx<ViewModelSolution>();
            LoadWorkspace(model, controller);

            //FilteredSolutions = CollectionViewSource.GetDefaultView(Solutions);
            //FilteredSolutions.Filter = FilterOutListings;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Updates a filter.
        /// </summary>
        /// <param name="value">        the filter value. </param>
        /// <param name="folder">       Pathname of the folder. </param>
        /// <param name="extension">    The extension. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UpdateFilter(bool value,ViewModelTreeFolder folder, string extension)
        {
            foreach (var item in folder.Children)
            {
                var file = item as ViewModelTreeFile;
                if (file != null && file.ModelFile.Extension == extension)
                {
                    file.IsVisible = value;
                    continue;
                }
                var f = item as ViewModelTreeFolder;
                if (f != null)
                {
                    UpdateFilter(value,f, extension);
                }
            }
            
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Filter out listings.
        ///// </summary>
        ///// <param name="obj">  The object. </param>
        ///// <returns>
        ///// true if it succeeds, false if it fails.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public bool FilterOutListings(object obj)
        //{
        //    var item = obj as ViewModelTreeFile;
        //    if (!ShowListing && item != null)
        //    {
        //        return item.ModelFile.Extension != ".pclst";
        //    }
        //    return true;
        //}


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads a workspace.
        /// </summary>
        /// <param name="model">        The model. </param>
        /// <param name="controller">   The controller. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LoadWorkspace(ModelWorkspace model, ControllerIde controller)
        {
            IconSource            = IconUtil.GetImage( "Workspace" );
            Model                 = model;
            var vmSolution        = new ViewModelSolution(controller, model.Solution);
            vmSolution.IsExpanded = true;
            ContentId             = Constants.WORKSPACE_CONTENT_ID + vmSolution.FullPath;

            Solutions.Clear();
            Solutions.Add(vmSolution);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Current project selected
        /// </summary>
        /// <returns>
        /// project or null
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelProject CurrentProject()
        {
            return CurrentFolder() as ViewModelProject;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Current folder selected
        /// </summary>
        /// <returns>
        /// project or null
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelTreeFolder CurrentFolder()
        {
            //  return Selected as ViewModelTreeFolder;
          //  TODO: get selection to work or send a message

            if (CurrentSolution != null)
            {
                return  CurrentSolution.FindSelectedVM() as ViewModelTreeFolder;
            }
            return null;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Queries if we can close.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool CanClose()
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clears thie workspace to its blank/initial state.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Clear()
        {
            Solutions.Clear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the close action.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClose()
        {
            //THIS IS NOT WORKING
            // we don't shutdown since the shutdown flag is false, just close all files.
            Controller.ShutdownSaveAndCloseAllFiles(true);
        }

    }
}
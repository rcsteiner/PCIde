////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the view model solution class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/26/2015   rcs     Initial Implementation
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
using System.Linq;
using Ide.Controller;
using Ide.Model;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// View model solution. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelSolution : ViewModelTreeFolder
    {
        /// <summary>
        /// The type name like folder, file, solution etc.
        /// </summary>
        public override string TypeName { get { return "Solution"; } }

        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        public  string Name { get { return Folder.NameOnly; } set { Folder.NameOnly = value; } }

        /// <summary>
        /// Gets or sets the title.
        /// TODO: look at title name change
        /// </summary>
        public override string Title
        {
            get { return Name; }
            set
            {
                if (Name != value)
                {
                    Name = value;
                    RaisePropertyChanged( "Title" );
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        /// <param name="solution">     The solution. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelSolution(ControllerIde controller, ModelSolution solution): base( controller,solution )
        {
            IconSource = IconSelected = IconUtil.GetImage( TypeName );

            // convert the solution to a view model

            AddViewModels(solution,this);


            controller.ActiveDocumentChanged += SynchDocument;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Synch document.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SynchDocument(object sender, EventArgs e)
        {
            if (Controller.VMActiveDocument != null)
            {
                var path = Controller.VMActiveDocument.FullPath;

                // find it in the tree
                FindSolutionFileVM(path);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first edit virtual memory.
        /// </summary>
        /// <param name="path"> Full pathname of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelTreeFile FindSolutionFileVM(string path)
        {
            if (path != null)
            {
                ViewModelTreeFile vm = FindVM(path);
                if (vm != null)
                {
                    vm.IsSelected = true;
                    vm.ParentFolder.IsExpanded = true;
                    return vm;
                }
            }
            return null;
        }




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds  view models.
        /// </summary>
        /// <param name="folder">   Pathname of the folder. </param>
        /// <param name="parent">   The parent. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void AddViewModels(ModelFolder folder,ViewModelTreeFolder parent)
        {
           var sorted = folder.Children.OrderBy(x=>x.Name, new NaturalStringComparer());
            foreach (var path in  sorted)
            {
                var project = path as ModelProject;
                if (project != null)
                {
                    var viewModelProject = new ViewModelProject(Controller,project,this);
                    parent.Children.Add(viewModelProject);
                    AddViewModels(project,viewModelProject);
                }
                else
                {
                    var file = path as ModelFile;
                    if (file != null)
                    {
                        parent.Children.Add(new ViewModelTreeFile(Controller,file,parent));
                    }
                
                }
            }
          //  folder.SortFiles();
        }



        #region Rename command


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Renames the file
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override bool Rename()
        {
            // Close current solution
            Controller.WorkspaceClose();

            // name will have the solution XXXX on the front so strip it off if its there
            // rename solution file

            if (!FileUtil.FileMove( oldPath, Folder.FullPath ))
            {
                Folder.FullPath = oldPath;
                return false;
            }

            // rename solution folder
            var folder = Folder.Folder;
            var nameindex = folder.LastIndexOf( FileUtil.GetFileName(oldPath), StringComparison.CurrentCultureIgnoreCase ) - 1;
            if (nameindex > 0)
            {
                folder = folder.Substring( nameindex );

                if (FileUtil.FolderMove( oldPath, folder ))
                {
                    Folder.Folder = folder;
                    //reload solution
                    Controller.WorkspaceOpen(Folder.FullPath);
                    //Children.Clear();
                    //AddViewModels( (ModelSolution)Folder, this );
                    return true;
                }
            }
            return false;
        }

        #endregion

    }
}

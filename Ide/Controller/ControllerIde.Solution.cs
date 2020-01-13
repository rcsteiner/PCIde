////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the controller ide. solution class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/6/2015   rcs     Initial Implementation
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

using Ide.Model;
using Microsoft.Win32;
using ViewModels;
using Views;
using ZCore;

namespace Ide.Controller
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Controller ide. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class ControllerIde
    {
        #region Workspace Properties

        /// <summary>
        /// Gets or sets the workspace.
        /// </summary>
        public ViewModelWorkspace VMWorkspace { get; set; }

        #endregion


        #region Get View Model

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the workspace.
        /// The workspace is a folder
        /// In it is a .sln file that stores information about the solution.
        /// the structure is:
        ///   
        ///  [solution]                 solution folder 
        ///    solution.sln             solution settings file
        ///    [library]                folder for library modules
        ///         library.prj         settings for library
        ///         lib1.pc             file in library
        ///    [project1]               folder for project 1
        ///         project1.prj        Settings for project 1
        ///         file1.pc            file in project 1
        ///    [project2]
        ///    
        /// </summary>
        /// <param name="basePath"> Full pathname of the base file. </param>
        /// <returns>
        /// The workspace.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelWorkspace GetWorkspace(string basePath = null)
        {
            if (basePath == null)
            {
                // just show the solution if it has been loaded
                if (VMWorkspace != null)
                {
                    ShowToolView(true, VMWorkspace);
                    return VMWorkspace;
                }
            }
            
            WorkspaceClose();

            // load it
            var workspace = new ModelWorkspace();
            if (basePath == "")
            {
                // empty workspace initial load
                workspace.LoadEmpty();
            }
            else
            {
                MRUWorkspaces.AddFilePath(basePath);
                workspace.Load( basePath );
                AddWatcher( basePath );
            }
            if (VMWorkspace == null)
            {
                VMWorkspace = new ViewModelWorkspace(workspace, this);
            }
            else
            {
                VMWorkspace.LoadWorkspace(workspace,this);
            }
            VMWorkspace.Title = workspace.Name;
            ShowToolView( true, VMWorkspace );
            return VMWorkspace;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a view model file edit.
        /// </summary>
        /// <param name="path"> Full pathname of the file. </param>
        /// <returns>
        /// The view model file edit.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelFileEdit GetViewModelFileEdit(string path)
        {
            foreach (var file in Files)
            {
                if (file.FullPath != null && file.FullPath.Equals( path )) return file;
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the view model error list.
        /// </summary>
        /// <returns>
        /// The view model error list.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ViewModelErrorList GetViewModelErrorList()
        {
            var vm = new ViewModelErrorList(this);
            vm.SelectedItemChanged += ErrorListSelectedErrorChanged;
            return vm;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the view model debug variables.
        /// </summary>
        /// <returns>
        /// The view model debug variables
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ViewModelDebugVariables GetViewModelDebugVariables()
        {
            var vm = new ViewModelDebugVariables( this );
          //  vm.SelectedItemChanged += DebugVariableSelectedErrorChanged;
            return vm;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the view model stack
        /// </summary>
        /// <returns>
        /// The view model debug stack.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ViewModelDebugStack GetViewModelDebugStack()
        {
            var vm = new ViewModelDebugStack( this );
            //  vm.SelectedItemChanged += DebugStackSelectedErrorChanged;
            return vm;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the view model output.
        /// </summary>
        /// <returns>
        /// The view model output.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ViewModelOutput GetViewModelOutput()
        {
            return new ViewModelOutput(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the view model chart.
        /// </summary>
        /// <returns>
        /// The view model chart.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ViewModelChart GetViewModelChart()
        {
            return new ViewModelChart(this);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the view model Elevator.
        /// </summary>
        /// <returns>
        /// The view model Elevator.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ViewModelElevator GetViewModelElevator()
        {
            return new ViewModelElevator(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the view model Stats.
        /// </summary>
        /// <returns>
        /// The view model Stats.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ViewModelStats GetViewModelStats()
        {
            return new ViewModelStats(this);
        }

        #endregion

        #region Workspace Support Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Open a dialog and create a new Solution.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WorkspaceNew()
        {
            //TODO: fix this to use workspace
            var dlg = new DialogNew();
            var vm = new ViewModelNewWorkspace( "Workspace", this, dlg );

            //todo move to services
            var list = new TemplateList();

            list.LoadAllTemplates( ViewModels.Constants.TEMPLATE_BUILT_IN, ViewModels.Constants.TEMPLATE_ALTERATE,"Workspace" );

            vm.TemplateList = list;

            dlg.ShowInWindow( true, "New Workspace" );
            if (vm.DialogResult)
            {
                var path     = FileUtil.PathCombine( vm.Folder, vm.Name, Constants.SOLUTION_EXT );
                var solution = ModelSolution.Create( path, vm.NewTemplate );
                if (solution != null)
                {
                    WorkspaceClose();
                    WorkspaceOpen(solution.FullPath);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Solution show.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WorkspaceShow()
        {
            if (VMWorkspace != null)
            {
                VMWorkspace.IsVisible = true;
                ShowToolView(VMMain.ShowWorkspace, VMWorkspace);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Solution close.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WorkspaceClose()
        {
            if (VMMain.ShowWorkspace && _tools.Contains(VMWorkspace))
            {
                // hide it
               // _tools.Remove(VMWorkspace);
                // close all content
                _files.Clear();
                VMWorkspace.Clear();
                VMWorkspace.Title = "<no workspace>";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Opens a workspace using a dialog box.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WorkspaceOpen()
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = Constants.SOLUTION_FILE_FILTER;
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                var fileName = dlg.FileName;
                WorkspaceOpen( fileName );
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Opens a solution.
        /// </summary>
        /// <param name="fileName"> Filename of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WorkspaceOpen(string fileName)
        {
            VMMain.ShowWorkspace = true;
            GetWorkspace(fileName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Solution export to zip.
        /// </summary>
        /// <returns>
        /// the target file
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string  WorkspaceExportToZip()
        {
            // Get solution folder, zip it.
            var solutionPath = VMWorkspace.Solutions[0].Folder;
            var folder = solutionPath.Folder;

            var name = AppSettings.Default.UserName.Replace(" ","");
            if (string.IsNullOrEmpty(name)  || name.EqualsIgnoreCase("student"))
            {
              name = solutionPath.NameOnly;  
            } 

            VMOutput.WriteLine(string.Format("Saving solution as zip file in folder: '{0}'", folder));

            OutputWrite(_serviceFile.SaveFolderAsZip(folder, name));
           return FileUtil.PathCombine( folder, name, ".zip" );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Workspace refresh.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WorkspaceRefresh()
        {
            var path = VMWorkspace.FullPath;
            WorkspaceClose();
            GetWorkspace( path );
        }


        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Project new create.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ProjectNew()
        {
            var dlg = new DialogNew();

            //TODO disconnect VM from View in NewDialog VMs
            var vm = new ViewModelNewProject( "Project", this, dlg );

            //todo move to services
            var list = new TemplateList();

            list.LoadAllTemplates( ViewModels.Constants.TEMPLATE_BUILT_IN, ViewModels.Constants.TEMPLATE_ALTERATE,"Project" );

            vm.TemplateList = list;

            dlg.ShowInWindow( true, "New Project" );
            if (vm.DialogResult)
            {
                var path = FileUtil.PathCombine(VMWorkspace.CurrentSolution.Folder.Folder, vm.Name, Constants.PROJECT_EXT );
                ModelProject.Create( path, vm.NewTemplate );
                WorkspaceRefresh();
            }
        }

    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the model solution class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/14/2015   rcs     Initial Implementation
//  =====================================================================================================
//
//  BSD 3-Clause License
//  Copyright (c) 2020, Robert C. Steiner
//  All rights reserved.
//  Redistribution and use in source and binary forms, with or without   modification,
//  are permitted provided that the following conditions are met:
//
//  1. Redistributions of source code must retain the above copyright notice, this
//  list of conditions and the following disclaimer.
//
//  2. Redistributions in binary form must reproduce the above copyright notice,
//  this list of conditions and the following disclaimer in the documentation
//  and/or other materials provided with the distribution.
//
//  3. Neither the name of the copyright holder nor the names of its
//  contributors may be used to endorse or promote products derived from
//  this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
//  FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//  DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
//  CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//  OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Ide.Support;
using ZCore;

namespace Ide.Model
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Model solution. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ModelSolution : ModelFolder
    {
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public Settings Settings { get; set; }

        /// <summary>
        /// Gets or sets the file filter.Commma separated list of file extensions.
        /// </summary>
        public string[] FileFilter  { get { return Settings.FindStringList("FileFilter"); }set { Settings.AddStringList("FileFilter",value); }
        }

        /// <summary>
        /// Gets or sets the project  filter, comma separated list of project extensions.
        /// </summary>
        public string[] ProjectFilter {get { return Settings.FindStringList( "ProjectFilter" ); }set { Settings.AddStringList( "ProjectFilter", value ); }
        }

        /// <summary>
        /// Gets or sets the solution filter (extension).
        /// </summary>
        public string WorkspaceFilter { get { return Settings.FindString( "WorkspaceFilter" ); } set { Settings.AddString( "WorkspaceFilter", value ); } }


        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get { return Settings.FindString( "Description" ); } set { Settings.AddString("Description",value);} }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rootPath">     Full pathname of the root file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ModelSolution(string rootPath) : base( rootPath)
        {
            Settings = new Settings(rootPath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a child. This default adds files as ModelFile, override to add other types.
        /// Override this method to add types not solution or project
        /// </summary>
        /// <param name="fullPath"> The file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void AddFile(string fullPath)
        {
            var ext = FileUtil.GetFileExtension(fullPath);
            if (ext.IsOneOf(FileFilter))
            {
                base.AddFile(fullPath);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a folder. Override to add special folders or filter folders.
        /// </summary>
        /// <param name="folderName">   Pathname of the folder. </param>
        /// <returns>
        /// Model folder.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override ModelFolder AddFolder(string folderName)
        {
            // check if folder is a project name
            if (ProjectFilter != null && ProjectFilter.Length>0)
            {
                var projects= FileUtil.FolderGetAllFiles(folderName, "*"+ProjectFilter[0]);
                if (projects.Length > 0)
                {
                    var prj = new ModelProject(projects[0]);
                    prj.LoadSettings(prj.FullPath);

                    AddProject(prj);
                    return prj;
                }
            }
            return base.AddFolder(folderName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a project. 
        /// </summary>
        /// <param name="prj">  The prj. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void AddProject(ModelProject prj)
        {
            if (prj.Name.StartsWith( "Library" ))
            {
                Children.Insert( 0, prj );
                return;
            }
            for (int x = 0; x < Children.Count; ++x)
            {
                var folder = Children[x] as ModelFolder;
                if (folder != null)
                {
                    if (prj.Name.CompareToIgnoreCase( folder.Name ) < 0)
                    {
                        Children.Insert( x, prj );
                        return ;
                    }
                }
                else
                {
                    Children.Insert( x, prj );
                    return ;
                }
            }
            Children.Add( prj );
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Creates the given solution.
        ///  extract template from resources
        /// expand template into the target folder
        /// rename Solution.sln to new name
        /// (optional) create project folder and add project.prj
        ///
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <param name="template"> The template. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static ModelSolution  Create(string filePath, Template template)
        {
            // create the template

            var folder = FileUtil.GetFolder(filePath);
            var name   = FileUtil.GetFileName(filePath);
            var ext = FileUtil.GetFileExtension(filePath);
            string newPath=null;

            if (template.Create(folder))
            {
                var solutionFile = FileUtil.PathCombine( folder, template.Name, ext );

                if (FileUtil.FileExists( solutionFile ))
                {
                    newPath = FileUtil.PathCombine( folder, name, ext);
                    FileUtil.FileMove( solutionFile, newPath );
                }
            }
            return newPath != null ? new ModelSolution(newPath) : null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads the given solution in this folder and all the children
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Load(string filePath)
        {
            FullPath = filePath;

            // get the solution out of the file

            if (FileUtil.FileExists( FullPath ))
            {
                // load the file
                base.Load( true );

                // now the folders and projects need to get reconciled.
                return true;
            }
            return false;
        }

    }
}
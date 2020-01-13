////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the model project class
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
    /// Model project. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ModelProject : ModelFolder
    {
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public Settings Settings { get; set; }


        /// <summary>
        /// Gets or sets the file filter.Commma separated list of file extensions.
        /// </summary>
        public string[] FileFilter
        {
            get { return Settings.FindStringList( "FileFilter" ); }
            set { Settings.AddStringList( "FileFilter", value ); }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rootPath">     Full pathname of the root file. </param>
        /// <param name="loadChildren"> (optional) true to load children. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ModelProject(string rootPath, bool loadChildren = false) : base(rootPath)
        {
            Settings = new Settings(rootPath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <param name="fullPath"> The file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LoadSettings(string fullPath)
        {
             Settings.Load(fullPath);
       }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a child. This default adds files as ModelFile, override to add other types.
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
        /// Creates this object.
        /// </summary>
        /// <param name="filePath">         Full pathname of the file. </param>
        /// <param name="template">  The new template. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string  Create(string filePath, Template template)
        {
            // create the template

            var folder     = FileUtil.GetFolder( filePath );
            var name       = FileUtil.GetFileName( filePath );
            var ext        = FileUtil.GetFileExtension( filePath );
            string newPath = null;

            if (template.Create(folder))
            {
                var path = folder + template.Name;
                var projectPath = FileUtil.PathCombine(path, template.Name, ext);

                if (FileUtil.FileExists(projectPath))
                {
                    newPath = FileUtil.PathCombine(path, name, ext);
                    FileUtil.FileMove(projectPath, newPath);
                    folder = FileUtil.GetFolder(path) + name;
                    FileUtil.FolderMove(path, folder);
                }
            }
            else
            {
                UserMessage.ShowErorr(string.Format("Can't create project folder '{0}",folder),"Failed to create project");
            }
            return newPath;
        }
    }
}
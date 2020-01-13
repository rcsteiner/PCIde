////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the folder class
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

using System.IO;
using ZCore;

namespace Ide.Model
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Model of file system Folder. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ModelFolder : ModelFilePath
    {
        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        public ObservableCollectionEx<ModelFilePath> Children { get ; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has sub-folders or sub-files.
        /// </summary>
        public bool HasChildren { get { return _hasChildren || Children.Count > 0; }  }

        ///// <summary>
        ///// Gets or sets the folder name and extension.
        ///// </summary>
        //public override string Name { get { return Folder.LastElement(FileUtil.SeparatorChar); } set {ChangeFolderName(value); }}


        /// <summary>
        /// flag set if not loading children and this folder has sub-folders or sub-files
        /// </summary>
        private bool _hasChildren;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rootPath">Full pathname of the root file.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ModelFolder(string rootPath) : base(rootPath)
        {
            Children = new ObservableCollectionEx<ModelFilePath>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Renames the file
        /// </summary>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void Rename(string value)
        {
            var oldPath = FullPath;
           NameAndExtension = value;
            if (!FileUtil.FolderMove( oldPath, FullPath ))
            {
                FullPath = oldPath;
                // reload folder (all files)
                Children.Clear();
                Load(true);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Load this folders children (only).
        /// </summary>
        /// <param name="loadChildren"> (optional) the bool to load. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Load(bool loadChildren = false)
        {
            if (loadChildren && !string.IsNullOrEmpty(FullPath) && FileUtil.FolderExists(Folder))
            {
                // we want to load the files first, then the folders
                LoadFiles();
                var folders = FileUtil.GetAllFolders(Folder);
                foreach (var folder in folders)
                {
                    LoadFolder(folder, true);
                }
            }
            else
            {
                PopulateWithDrives(loadChildren);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads a folder.
        /// </summary>
        /// <param name="folder">       Pathname of the folder. </param>
        /// <param name="loadChildren"> the bool to load. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void LoadFolder(string folder,bool loadChildren)
        {
            var zFolder = AddFolder(folder);
            {
                if (!loadChildren)
                {
                    _hasChildren = FileUtil.GetAllFolders(folder).Length > 0 || FileUtil.FolderGetAllFiles(folder, "*.*").Length > 0;
                    return;
                }
                if (zFolder != null)
                {
                    zFolder.Load(true);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads the files.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void LoadFiles()
        {
            var files = FileUtil.FolderGetAllFiles(Folder,"*.*");
            foreach (var file in files)
            {
                AddFile(file);
            }
            SortFiles();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sort files.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void SortFiles()
        {
            Children.Sort( NameSelector);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Name selector.
        /// </summary>
        /// <param name="x">    The x coordinate. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string NameSelector(ModelFilePath x)
        {
            return x.Name.ToLower();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a folder. Override to add special folders or filter folders
        /// </summary>
        /// <param name="folderName">   Pathname of the folder. </param>
        /// <returns>
        /// Model folder
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual ModelFolder AddFolder(string folderName)
        {
            var folder = new ModelFolder(FileUtil.AppendSeparator(folderName));
            Children.Add(folder);
            return folder;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a child. This default adds files as ModelFile, override to add other types.
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void AddFile(string filePath)
        {
            Children.Add(new ModelFile(filePath));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Populate with drives.
        /// </summary>
        /// <param name="loadChildren"> the bool to load. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void PopulateWithDrives(bool loadChildren)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                // string name = drive.Name;
                //try
                //{
                //    name = drive.VolumeLabel;
                //}
                //catch 
                //{
                //}
                LoadFolder(drive.RootDirectory.FullName,loadChildren);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Change folder name.
        /// </summary>
        /// <param name="name"> The name. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ChangeFolderName(string name)
        {
           FullPath =  Folder.RemoveSuffix(Name + FileUtil.SeparatorChar);
        }

    }
}
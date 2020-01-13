////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the folder tree view class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/15/2015   rcs     Initial Implementation
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

using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Views.Support;

namespace IDECore.Tree
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Folder tree view. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class TreeViewFolder : TreeView
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TreeViewFolder()
        {
            RefreshTree();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rootPath"> Full pathname of the root file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TreeViewFolder(string rootPath)
        {
            RefreshTree(rootPath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Refresh tree.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RefreshTree(string rootPath=null)
        {
            BeginInit();
            Items.Clear();
           if (!string.IsNullOrEmpty(rootPath) &&  Directory.Exists(rootPath))
            {
                // load root path
                var root             = new DirectoryInfo(rootPath);
                var item             = new TreeViewItemFolder(root);
                item.Text            = rootPath;// Vector.GetFileNameWithoutExtension( rootPath );
                item.UnselectedImage = item.SelectedImage = IconUtil.GetImage( "folderopen" ); ;
                Items.Add(item);
                item.Populate();
            }
            else
            {
                PopulateWithDrives();
            }
            EndInit();

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Populate with drives.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void PopulateWithDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                 string name = drive.Name;
                try
                {
                    name = drive.VolumeLabel;
                }
                catch 
                {
                }
                var item = new TreeViewItemFolder(drive.RootDirectory);
                item.Text = name;
                ImageSource image;
                image = GetDriveImage(drive);
                item.UnselectedImage = item.SelectedImage = image;
                Items.Add(item);
                item.Populate();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a drive image.
        /// </summary>
        /// <param name="drive">    The drive. </param>
        /// <returns>
        /// The drive image.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static ImageSource GetDriveImage(DriveInfo drive)
        {
            ImageSource image;
            switch (drive.DriveType)
            {
                default:
                case DriveType.NoRootDirectory:
                case DriveType.Unknown:
                case DriveType.Fixed:
                    image = IconUtil.GetImage("DriveFixed");
                    break;
                case DriveType.Removable:
                    image = IconUtil.GetImage("DriveUSB");
                    break;
                case DriveType.Network:
                    image = IconUtil.GetImage("DriveNetwork");
                    break;
                case DriveType.CDRom:
                    image = IconUtil.GetImage("DriveCDROM");
                    break;
                case DriveType.Ram:
                    image = IconUtil.GetImage( "DriveRam" );
                    break;
            }
            return image;
        }
    }
}
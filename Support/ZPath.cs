////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the path class
//
// Author:
//  Robert C. Steiner
//
// History:
//  ===================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------
//   9/12/2013   rcs     Initial implementation.
//  ===================================================================================
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Defines a class that wraps a file path string and provides accessors to the data in the path along with constrution.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public struct ZPath
    {
        /// <summary>
        /// Gets or sets the full path expanded to include current path if required.
        /// </summary>
        public string FullPath { get {  return FileUtil.GetFullPath( _fullpath); } set {_fullpath=value;} }


        /// <summary>
        /// Gets or sets the folder and file and extension.
        /// </summary>
        public string FolderAndFileAndExtension { get { return _fullpath; } set { _fullpath=value; } }

        /// <summary>
        /// Gets or sets the extension.  The extension starts with a . unless its empty
        /// </summary>;
        public string Extension
        {
            get { return FileUtil.GetFileExtension(_fullpath); }
            set { _fullpath = FileUtil.PathCombine(FileUtil.GetFolder(_fullpath), FileUtil.GetFileName(_fullpath), value); }
        }

        /// <summary>
        /// Gets or sets the name of the file. WITHOUT the extension
        /// </summary>
        public string FileName
        {
            get { return FileUtil.GetFileName(_fullpath); }
            set { _fullpath = FileUtil.PathCombine( FileUtil.GetFolder( _fullpath ),value, FileUtil.GetFileExtension( _fullpath )); }
        }

        /// <summary>
        /// Gets or sets the name of the file. WITHOUT the extension
        /// </summary>
        public string FileNameAndExtension
        {
            get { return FileUtil.GetFileNameAndExtension(_fullpath); }
            set
            {
                _fullpath = FileUtil.PathCombine( FileUtil.GetFolder( _fullpath ), FileUtil.GetFileName( value ),FileUtil.GetFileExtension( value ));
            }
        }

        /// <summary>
        /// Gets or sets the folder text, can start with . or .. or a \ and ends in the \
        /// </summary>
        public string Folder
        {
            get { return FileUtil.GetFolder(_fullpath); }
            set
            {
                _fullpath = FileUtil.PathCombine( value, FileUtil.GetFileName( _fullpath ),FileUtil.GetFileExtension( _fullpath ));
            }
        }

  
        /// <summary>
        /// Gets a value indicating whether this is a relative file path, Root is empty and folder starts with .
        /// </summary>
        public bool IsRelative
        {
            get { return _fullpath.StartsWith(FileUtil.ThisFolderPrefix) || _fullpath.StartsWith(FileUtil.ParentFolderPrefix); }
        }

        #region Backing fields

        /// <summary>
        /// The fullpath storage of this file path.
        /// </summary>
        private string _fullpath;

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ZPath casting operator from string.
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator ZPath(string filePath)
        {
            return new ZPath(filePath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ZPath(string filePath) : this()
        {
            _fullpath = filePath;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="folder">       Pathname of the folder. </param>
        /// <param name="fileName">     Filename of the file. </param>
        /// <param name="extension">    The extension. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ZPath(string folder, string fileName, string extension) : this()
        {
            _fullpath = FileUtil.PathCombine(folder, fileName, extension);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets a relative to another path by subtracting the current folder from the basePath.
        /// </summary>
        /// <param name="mainPath"> Full pathname of the base file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetRelativeTo(string mainPath)
        {
            _fullpath = FileUtil.PathRelative(mainPath, _fullpath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Check if this file actually exits.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Exists()
        {
            return FileUtil.FileExists(_fullpath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Reads all text and returns it as a string.
        /// </summary>
        /// <returns>
        /// all text or empty string if file does not exist.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ReadAllText()
        {
            string file = FullPath;

            if (FileUtil.FileExists(file))
            {
                return FileUtil.FileReadText(file);
            }
            return string.Empty;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Tests if this ZPath is considered equal to another.
        /// </summary>
        /// <param name="other">   the other path to compare to. </param>
        /// <returns>
        /// true if the objects are considered equal, false if they are not.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Equals(ZPath other)
        {
            return _fullpath.EqualsIgnoreCase(other._fullpath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert this object into a string representation.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return _fullpath;
        }
    }
}
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
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using ZCore;

namespace Ide.Model
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// File base. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public abstract class ModelFilePath
    {
 
        /// <summary>
        /// Full pathname of the file.
        /// </summary>
        private ZPath _path;

        /// <summary>
        /// Gets or sets the full ZPath of the file or folder.
        /// </summary>
        public ZPath Path {get { return _path; }set { _path = value; }}

        /// <summary>
        /// Gets or sets the pathname of the folder.
        /// </summary>
        public string Folder { get { return _path.Folder; } set { _path.Folder = value; } }


        /// <summary>
        /// Gets or sets the full pathname of the full file.
        /// </summary>
        public string FullPath { get { return _path.FullPath; } set { _path.FullPath = value; } }

        /// <summary>
        /// Gets or sets the folder name and extension.
        /// </summary>
        public string NameAndExtension { get { return _path.FileNameAndExtension; } set { _path.FileNameAndExtension = value; } }
        
        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        public string Extension { get { return _path.Extension; } set { _path.Extension = value; } }

        /// <summary>
        /// Gets or sets the file name and extension.
        /// </summary>
        public virtual string Name { get { return _path.FileNameAndExtension; } set { Rename(value); }}

        /// <summary>
        /// Gets or sets the folder name and extension.
        /// </summary>
        public string NameOnly { get { return _path.FileName; } set { _path.FileName = value; } }


        /// <summary>
        /// Gets or sets the filename of the file.
        /// </summary>
        public string FileName { get { return _path.FileName; } set { _path.FileName = value; } }

        /// <summary>
        /// Gets or sets the saved time.
        /// </summary>
        public DateTime SavedTime { get; set; }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Renames the file
        /// </summary>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Rename(string value)
        {
            var oldPath = _path.FullPath;
            _path.FileNameAndExtension = value;
            if (!FileUtil.FileMove(oldPath, _path.FullPath))
            {
                _path.FullPath = oldPath;
            }
            SavedTime = DateTime.Now;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fullPath"> The full pathname of the full file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected ModelFilePath(string fullPath)
        {
            FullPath = fullPath;
            SavedTime = DateTime.Now;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">  The object to compare with the current object. </param>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Equals(object obj)
        {
            return Equals ((ModelFilePath)obj);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="other">    The model file path to compare to this object. </param>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected bool Equals(ModelFilePath other)
        {
            return _path.Equals( other._path );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override int GetHashCode()
        {
            return _path.FullPath.GetHashCode();
        }


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
            return _path.FullPath;
        }

    }
}
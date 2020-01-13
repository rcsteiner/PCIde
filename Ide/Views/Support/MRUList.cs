////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the mru list class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/12/2015   rcs     Initial Implementation
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

using System;
using System.Collections.ObjectModel;
using Ide.Controller;
using ZCore;

namespace Ide
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Provides basic MRU operations for a string type object.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class MRUList : ObservableCollection<ViewModelMRU>
    {
        private ControllerIde _controller;

        /// <summary>
        /// The category
        /// </summary>
        public string Category;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MRUList"/> class and load the list.
        /// </summary>
        /// <param name="category">     A category, the data is stored under. </param>
        /// <param name="controller">   The controller. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public MRUList(string category, ControllerIde controller)
        {
            _controller = controller;
			Load(category);
		}


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds the specified a string.
        /// </summary>
        /// <param name="filePath">  A string. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddFilePath(string filePath)
        {
            RemoveFilePath(filePath);

            if (Count > AppSettings.Default.MaxMRUFiles)
            {
                RemoveAt(Items.Count-1);
            }

            Insert(0,new ViewModelMRU(_controller,filePath));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes the specified a string.
        /// </summary>
        /// <param name="filePath">  A string. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveFilePath(string filePath)
        {
            ViewModelMRU w = null;
            foreach (var ws in this)
            {
                if (ws.Path.Equals( filePath, StringComparison.CurrentCultureIgnoreCase ))
                {
                    w = ws ;
                    break;
                }
            }
            if (w != null)
            {
                Remove(w);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Checks if the item loaded is still Valid.
        /// </summary>
        /// <param name="path">    item to check. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual  bool Valid(string path)
        {
            return FileUtil.FileExists(path);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads the specified a category.
        /// </summary>
        /// <param name="category"> A category, the data is stored under. </param>
        /// <returns>
        /// the number loaded
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int Load(string category)
        {
            Clear();
            Category = category;

            var mru = AppSettings.Default[category];
            string oMru = mru!=null? mru.ToString():"" ;

            string[] oItems = oMru.Split( ';' );


            foreach (string item in oItems)
            {
                if (!string.IsNullOrEmpty( item ) && Valid( item ))
                {
                    AddFilePath( item );
                }
            }

            return Count;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves this list.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Save()
		{
			string oMru = String.Join(";", Items);
			AppSettings.Default[Category] =  oMru;
            AppSettings.Default.Save();
		}
    }
}
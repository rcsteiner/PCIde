//  ---------------------------------------------------------------------------------------
//    Description:
//   
//   Provides extended file operations using core fileUtil methods.
//   
//   Author: Robert C. Steiner
//   
//   ======================== History ======================
//   
//   Date        Who      What
//  ----------- ------   ---------------------------
//   09/21/09    rcs     Initial implementation
//   09/03/13    rcs     Reworked, simplified and moved to ZCore
//  
//   ======================================================
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
//  ---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZCore
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     File service, provides access and manipulation of the file system using only FileUtil
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static partial class FileUtil
    {
        /// <summary>
        /// The empty strings array
        /// </summary>
        public static readonly string[] EMPTY_STRINGS = new string[0];



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Validates the full path and computes the file path to use to access the file.  If the folder doesn't
        /// exist, it creates it. 
        /// </summary>
        /// <param name="path">     The path. </param>
        /// <param name="filename"> The filename. </param>
        /// <returns>   
        /// the full file path or null if it cannot be created.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string FolderValidate(string path, string filename)
        {
            string localFilename = PathCombine( path, filename );

            //make sure the folder exits, if not create it.
            return FolderCreate( path ) ? localFilename : null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Create a new name unique folder with this base name.
        /// </summary>
        /// <param name="folder">      A folder. </param>
        /// <param name="folderName">  Key of a folder. </param>
        /// <returns>
        ///     a new unique folder name or null if cannot be created.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string FolderCreateUnique(string folder, string folderName)
        {
            int folderId = 0;
            folder       = AppendSeparator(folder);

            if (!FolderExists(folder))
            {
                FolderCreate(folder);
            }
            string newFolderName = folderName;
            do
            {
                string path = folder + newFolderName + SeparatorChar;
                if (!FolderExists(path))
                {
                    FolderCreate(path);
                    return newFolderName;
                }
                ++folderId;
                newFolderName = folderName + folderId;
            } while (true);
        }

        #region File methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Create a new unique to folder with this base name.
        /// </summary>
        /// <param name="folder">  A folder. </param>
        /// <param name="baseName">
        ///     Key of a base. This is the first part of the file name containing baseNameId.
        ///     Where "Id" is an integer value [1, 2,...]
        ///     For example, if baseName="x", and this file "x" already exists, then a new file called "x1"
        /// </param>
        /// <returns>
        ///     a new unique name in the format of "baseNameId" (e.g baseName="x", Id=1, return new unique name="x1")
        ///     or a null value if
        ///     -baseName is null or empty
        ///     -can't create a folder
        ///     -?
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string FileCreateUnique(string folder, string baseName)
        {
            if (String.IsNullOrEmpty(baseName))
            {
                return null;
            }
            int id = 0;
            string extension = GetFileExtension(baseName);
            baseName = GetFileName(baseName);

            if (!FolderCreate(folder))
            {
                return null;
            }

            string fileName = baseName;
            do
            {
                string path = PathCombine(folder, fileName, extension);
                if (!FileExists(path))
                {
                    return path;
                }
                ++id;
                fileName = baseName + id;
            } while (true);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Writes the application's byte data to disk, checks the path to see if it exits, delete's the old file
        ///     and writes the new using FileWriteBinary.
        /// </summary>
        /// <param name="path">     Full pathname of the file to write data to. </param>
        /// <param name="byteData"> byte data to write. </param>
        /// <param name="start">    [optional start at 0]The start offset in data. </param>
        /// <param name="length">   [optional the remaining length] The length of the data to write. </param>
        /// <returns>
        ///     true if successful.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FileSave(string path, byte[] byteData, int start = 0, int length = -1)
        {
            string localFilename = GetFullPath(path);

            if (String.IsNullOrEmpty(localFilename))
            {
                return false;
            }
            if (FileExists(localFilename))
            {
                FileDelete(localFilename);
            }
            return FileWriteBinary(localFilename, byteData, start, length);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Writes a csv record.
        /// </summary>
        /// <param name="writer">   The writer. </param>
        /// <param name="fields">   A variable-length parameters list containing fields. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FileWriteCSVRecord(StreamWriter writer, params object[] fields)
        {
            if (writer == null || fields == null)
            {
                return false;
            }

            for (int i = 0; i < fields.Length; i++)
            {
                if (i > 0)
                {
                    writer.Write(",");
                }
                writer.Write(fields[i].ToString());
            }
            writer.WriteLine();
            return true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Loads the specified filepath as bytes. Checks the filepath first.
        /// </summary>
        /// <param name="filepath"> The filepath. </param>
        /// <returns>
        ///     byte array or null.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] FileLoad(string filepath)
        {
            string path = GetFullPath(filepath);
            return FileExists(path) ? FileReadBinary(path) : null;
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Gets all files with optional wild cards and returns all the files (full path) in a string list. 
        /// </summary>
        /// <param name="fileNames">        List of names of the files. </param>
        /// <param name="searchSubFolders"> true to search sub folders. </param>
        /// <returns>   
        /// all files. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<string> ExpandAllFiles(IEnumerable<string> fileNames, bool searchSubFolders = false)
        {
            var list = new List<string>();

            // for each instance, grab actual set of files

            if (fileNames != null)
            {
                foreach (var file in fileNames)
                {
                    ExpandFile( list, file, searchSubFolders );
                }
            }
            return list;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Expand file and . with optional wild cards and returns all the files (full path) and adds to string list.
        /// </summary>
        /// <param name="list">             The list. </param>
        /// <param name="file">             The file. </param>
        /// <param name="searchSubFolders"> true to search sub folders. </param>
        /// <returns>
        /// true - if a file expansion occurred.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool ExpandFile(List<string> list, string file, bool searchSubFolders = false)
        {
            if (list == null || String.IsNullOrEmpty( file ))
            {
                return false;
            }

            // prepend '\' if this is just a file name
            // separate each file into the path and filename

            int origCount        = list.Count;
            string filePath      = file.IndexOf( SeparatorChar ) < 0 ? ThisFolderPrefix + file : file;
            string folder        = GetFolder( filePath );
            string searchPattern = GetFileNameAndExtension( file );
            var lst              = FolderGetAllFiles( folder, searchPattern, searchSubFolders );
            foreach (var fullfile in lst)
            {
                if (!list.Contains( fullfile ))
                {
                    list.Add( fullfile );
                }
            }
            return (list.Count - origCount) > 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets all folders.
        /// </summary>
        /// <param name="path"> The path. </param>
        /// <returns>
        /// all folders.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string[] GetAllFolders( string path)
        {
            string filePath = path.IndexOf( SeparatorChar ) < 0 ? ThisFolderPrefix + path : path;
            string folder   = GetFolder( filePath );
            return (folder != null && Directory.Exists(folder)) ? Directory.GetDirectories( folder ) : EMPTY_STRINGS;
        }


        #region Path Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Combines an array of strings into a path
        /// </summary>
        /// <param name="paths">    A variable-length parameters list containing paths. </param>
        /// <returns>
        ///     All the paths combined with folder separator characters.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string PathCombineLiteral(params string[] paths)
        {
            if (paths == null || paths.Length <= 0)
            {
                return null;
            }

            var b = new StringBuilder();

            for (int i = 0; i < paths.Length - 1; i++)
            {
                b.Append(paths[i]);
                if (!paths[i].EndsWith(Separator))
                {
                    b.Append(Separator);
                }
            }

            b.Append(paths[paths.Length - 1]);

            return b.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Combine a folder, file and extension with all the possible combinations to get a file path. If Extension
        /// is given The it can replace the existing extension on the filename.
        /// </summary>
        /// <param name="path">         Path to start or null to start at the application folder, can include file
        ///                             name. </param>
        /// <param name="relativePath"> A relative path (relative to "path" parameter) or null to adjust the path
        ///                             parameter. </param>
        /// <param name="fileName">     File name with or without an extension,  if null then assumes the path
        ///                             contains the filename. </param>
        /// <param name="ext">          If the extension does not begin with a '.' it is added. </param>
        /// <returns>
        /// Full expanded path or null if there is an error.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string PathCombineRelative(string path, string relativePath, string fileName = null,string ext = null)
        {
            if (!String.IsNullOrEmpty(relativePath))
            {
                if (path != null)
                {
                    path = AppendSeparator(path);
                    path = relativePath.StartsWith(".") ? GetFullPath(path + relativePath) : relativePath;
                }
            }
            return (!String.IsNullOrEmpty(fileName)) ? PathCombine(path, fileName, ext) : path;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Compute the relative path given a Main path and a Secondpath.
        ///     NOTE: does not check for AbsolutePaths
        /// </summary>
        /// <param name="mainPath">     Full pathname of the main file. </param>
        /// <param name="secondPath">   Full pathname of the second file. </param>
        /// <returns>
        /// partial relative path if possible.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string PathRelative(string mainPath, string secondPath)
        {
            if (String.IsNullOrEmpty(mainPath))
            {
                return secondPath;
            }

            if (String.IsNullOrEmpty(secondPath))
            {
                return mainPath;
            }

            mainPath             = AppendSeparator(mainPath);
            string[] mainParts   = mainPath.Trim(SeparatorChar).Split(SeparatorChar);
            string[] secondParts = secondPath.Trim(SeparatorChar).Split(SeparatorChar);

            int len = mainParts.Length;
            if (len > secondParts.Length) len = secondParts.Length;

            int count;
            for (count = 0; count < len; count++)
            {
                if (!mainParts[count].EqualsIgnoreCase(secondParts[count]))
                {
                    break;
                }
            }

            if (count == 0)
            {
                return secondPath;
            }

            StringBuilder newPath = new StringBuilder();
       
            for (int i = count; i < mainParts.Length; i++)
            {
                if (i > count)
                {
                    newPath.Append( SeparatorChar);
                }
                newPath.Append("..");
            }
            if (newPath.Length == 0)
            {
                newPath.Append( ".");
            }
            for (int i = count; i < secondParts.Length; i++)
            {
                newPath.Append(SeparatorChar);
                newPath.Append(secondParts[i]);
            }
            return newPath.ToString();
        }

        #endregion


        #region Path Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Combine a folder, file and extension with all the possible combinations to get a file path. If Extension
        /// is given then replace the existing extension on the filename.
        /// </summary>
        /// <param name="path">     Path to start or null to start at the application directory, can include file
        ///                         name. </param>
        /// <param name="fileName"> File name with or without an extension,  if null then assumes the path contains
        ///                         the filename. </param>
        /// <param name="ext">      If the extension does not begin with a '.' it is added. </param>
        /// <returns>
        /// path or null if can't form a path.
        /// </returns>
        /// <remarks>
        ///     path        fileName        ext     return
        ///     =========   ========        =====   ========
        ///     ""          ""              "ext"   null
        ///     "hello"     ""              "ext"   hello\.ext
        ///     ""          "world"         "ext"   "world.ext"
        ///     "hello"     "world"         "ext"   hello\world.ext
        ///     "c:\foo"    "world"         "ext"   "c:\foo\world.ext"  
        ///     "c:\foo\"   "world"         "ext"   "c:\foo\world.ext"
        ///     "c:"        "world"         "ext"   "c:\world.ext"
        /// </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string PathCombine(string path, string fileName = null, string ext = null)
        {
            // validate the path
            string fullPath = null;
            bool fileNameIsOk = !String.IsNullOrEmpty( fileName );
            bool folderIsOk = !String.IsNullOrEmpty( path );
            if (!folderIsOk && !fileNameIsOk)
            {
                // both are null, not valid
                return null;
            }

            if (folderIsOk && fileNameIsOk)
            {
                // have a folder and filename
                fullPath = AppendSeparator( path ) + fileName;
            }
            else if (fileNameIsOk)
            {
                // valid filename but no path
                fullPath = fileName;
            }
            else
            {
                // only path, no filename
                fullPath = AppendSeparator( path );
            }

            // handle the extension now
            if (!String.IsNullOrEmpty( ext ))
            {
                if (ext[0] != '.')
                {
                    ext = "." + ext;
                }
                for (int p = fullPath.Length - 1; p > 0; --p)
                {
                    var c = fullPath[p];
                    if (c == '.')
                    {
                        return fullPath.Substring( 0, p ) + ext;
                    }
                    if (c == SeparatorChar || c==AlternateSeparatorChar || c==VolumeChar)
                    {
                        break;
                    }
                }

                fullPath = fullPath + ext;
            }
            return fullPath;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>   
        /// Appends a seperator character. 
        /// </summary>
        /// <param name="path">                    Path to start or null to start at the application directory, can
        ///                                        include file name. </param>
        /// <returns>   
        /// Folder fixed with separator.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string AppendSeparator(string path)
        {
            return (String.IsNullOrEmpty( path ) || path[path.Length - 1] == SeparatorChar) ? path : path + SeparatorChar;
        }


        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Gets the parent folder associated with a path
        /// </summary>
        /// <param name="filepath"> The filepath. </param>
        /// <returns>   
        /// The folder or null if no folder.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetParentFolder(string filepath)
        {
            return GetFolder(GetFolder(filepath).TrimEnd(SeparatorChar, AlternateSeparatorChar));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Gets the folder asscoaited with a path and terminates it with a separator character.
        /// </summary>
        /// <param name="filepath"> The filepath. </param>
        /// <returns>   
        /// The folder or null if no folder.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetFolder(string filepath)
        {
            if (filepath != null)
            {
                for (int p = filepath.Length - 1; p > 0; --p)
                {
                    char c = filepath[p];
                    if (c == SeparatorChar || c == AlternateSeparatorChar || c == VolumeChar)
                    {
                        return AppendSeparator( filepath.Substring( 0, p+1 ).TrimEnd() );
                    }
                }
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Gets the folder asscoaited with a path and terminates it with a separator character.
        /// </summary>
        /// <param name="filepath"> The filepath. </param>
        /// <returns>   
        /// The folder or null if no folder.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetFirstFolder(string filepath)
        {
            int last = 0;
            if (filepath != null)
            {
                for (int p = filepath.Length - 1; p > 0; --p)
                {
                    char c = filepath[p];
                    if (c == SeparatorChar || c == AlternateSeparatorChar || c == VolumeChar)
                    {
                        last = p;
                    }
                }

                if (last > 0)
                {
                    return filepath.Substring(0, last) + SeparatorChar;
                }
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Gets the file name and extension of the specified path string. 
        /// </summary>
        /// <param name="filepath"> The filepath. </param>
        /// <returns>   
        /// The file name. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetFileNameAndExtension(string filepath)
        {
            if (filepath != null)
            {
                for (int p = filepath.Length - 1; p > 0; --p)
                {
                    char c = filepath[p];
                    if (c == SeparatorChar || c == AlternateSeparatorChar || c == VolumeChar)
                    {
                        return filepath.Substring( p+1 ).TrimEnd();
                    }
                }
            }
            return filepath;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Gets the file name of the specified path string without the extension. 
        /// </summary>
        /// <param name="filepath"> The filepath. </param>
        /// <returns>   
        /// The file name without extension or null if no path
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetFileName(string filepath)
        {

            if (filepath != null)
            {
                int end = filepath.Length;
                for (int p = end - 1; p > 0; --p)
                {
                    char c = filepath[p];
                    if (c == '.')
                    {
                        end = p;
                    }

                    if (c == SeparatorChar || c == AlternateSeparatorChar || c == VolumeChar)
                    {
                        return filepath.Substring( p + 1, end - p-1 ).TrimEnd();
                    }
                }
               return filepath.Substring(0,end);
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Gets the extension of the specified path string. 
        /// </summary>
        /// <param name="filepath"> The filepath. </param>
        /// <returns>   
        /// The file extension or empty string if not found.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetFileExtension(string filepath)
        {
            if (filepath != null)
            {
                for (int p = filepath.Length - 1; p > 0; --p)
                {
                    char c = filepath[p];
                    if (c == '.')
                    {
                        return filepath.Substring( p ).TrimEnd();
                    }
                    if (c == SeparatorChar || c == AlternateSeparatorChar || c == VolumeChar)
                    {
                        break;
                    }
                }
            }
            return string.Empty;
        }

    }
}
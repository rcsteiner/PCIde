////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the file util class
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   
    /// File service, provides access and manipulation of the file system. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static partial class FileUtil
    {
        #region Static character and string separators

        /// <summary>   
        /// Gets the folder separator char. 
        /// </summary>
        public static readonly char SeparatorChar= Path.DirectorySeparatorChar; 

        /// <summary>
        /// Gets the separator.
        /// </summary>
        public static readonly string Separator = Path.DirectorySeparatorChar.ToString();
     
        /// <summary>
        /// Gets the alternate separator char.
        /// </summary>
        public static readonly char AlternateSeparatorChar = Path.AltDirectorySeparatorChar; 

        /// <summary>
        /// Gets the volumn char.
        /// </summary>
        public static readonly char VolumeChar = Path.VolumeSeparatorChar; 

        /// <summary>
        /// Gets the this folder prefix.
        /// </summary>
        public static readonly string ThisFolderPrefix = "." + Separator; 

        /// <summary>
        /// Gets the parent folder prefix.
        /// </summary>
        public static readonly string ParentFolderPrefix =".." + Separator; 
   
        public static readonly char[] FolderSeparators = new char[]{SeparatorChar,AlternateSeparatorChar,VolumeChar};
        #endregion

        #region Special Folder access

        /// <summary>
        /// Gets the application folder.
        /// </summary>
        /// <value>The application folder.</value>
        public static string ApplicationFolder
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
              //  Assembly asm = Assembly.GetEntryAssembly();
              //  return asm == null ? CurrentFolder : GetFolder(asm.Location);
            }
        }

        /// <summary>   
        /// Gets the current directory. 
        /// </summary>
        public static string CurrentFolder
        {
            get { return AppendSeparator(Directory.GetCurrentDirectory()); }
        }

        /// <summary>
        /// Gets my documents folder
        /// </summary>
        /// <value>My documents.</value>
        public static string MyDocumentsFolder
        {
            get { return  AppendSeparator(Environment.SpecialFolder.MyDocuments.ToString()); }
        }

        /// <summary>
        /// Gets recent folder
        /// </summary>
        /// <value>recent folder.</value>
        public static string RecentFilesFolder
        {
            get { return  AppendSeparator(Environment.SpecialFolder.Recent.ToString()); }
        }

        /// <summary>
        /// Gets the application data folder.
        /// </summary>
        public static string ApplicationDataFolder
        {
            get { return AppendSeparator(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)); }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the names of files (including their paths) that match the specified search pattern in the specified
        /// folder, using a value to determine whether to search subfolders.
        /// </summary>
        /// <param name="folder">           Pathname of the directory. </param>
        /// <param name="searchPattern">    The search pattern. </param>
        /// <param name="searchSubFolders"> (optional) the search sub folders. </param>
        /// <returns>
        /// array of all files in a folder. Possibly an empty array.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string[] FolderGetAllFiles(string folder, string searchPattern, bool searchSubFolders = false)
        {
            try
            {
                if (folder != null && !String.IsNullOrEmpty( searchPattern ) && FolderExists( (folder) ))
                {
                    return Directory.GetFiles( folder, searchPattern, searchSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );
                }
            }
            catch
            {
            }
            return new string[0];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Creates the directory if it does not exist. 
        /// </summary>
        /// <param name="path"> A path. </param>
        /// <returns>>
        ///     true if directory created, false if not
        ///     
        ///     path        returns
        ///     =========   ========
        ///     null or ""  false
        ///     "c:\foo"    false
        ///     "c:\foo\"   true    - creates this folder if "c:\foo" doesn't already exist
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FolderCreate(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return false;
            }
            string folder =AppendSeparator(path);
            try
            {
                if (!FolderExists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                return true;
            }
            catch
            {
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Check if a folder exists. 
        /// </summary>
        /// <param name="path"> Path to the folder to check. </param>
        /// <returns>   
        /// True if the folder exists. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FolderExists(string path)
        {
            if (String.IsNullOrEmpty(path)) return false;
            return Directory.Exists(path);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Deletes the specified a full Directory path.  Checks if it exists first. 
        /// </summary>
        /// <param name="fullFolderPath">    A full Directory path. </param>
        /// <returns>   
        /// true if it succeeds, false if it fails. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FolderDelete(string fullFolderPath)
        {
            if ( !FolderExists(fullFolderPath))
            {
                return false;
            }
            try
            {
                Directory.Delete(fullFolderPath, true);
                return true;
            }
            catch
            {
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Moves a Directory. 
        /// </summary>
        /// <param name="oldPath">  The old path. </param>
        /// <param name="newPath">  A new path. </param>
        /// <returns>   
        /// false if Directory exists in new location or can't move it. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FolderMove(string oldPath, string newPath)
        {
            if (FolderExists(newPath))
            {
                return false;
            }

            try
            {
                if (!oldPath.EqualsIgnoreCase(newPath))
                {
                    Directory.Move(oldPath, newPath);
                }
                return true;
            }
            catch
            {
            }
            return false;
        }

        #endregion

        #region File methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Get a file length. 
        /// </summary>
        /// <param name="filePath"> A file path. </param>
        /// <returns>   
        /// the length in bytes of the file.
        /// -1 if filePath is null or empty
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static long FileLength(string filePath)
        {
            if (String.IsNullOrEmpty(filePath) || !FileExists(filePath))
            {
                return -1;
            }
            var info = new FileInfo(filePath);
            return info.Length;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File read all all text. 
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <returns>
        /// text string or null if not readable.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string FileReadText(string filePath )
        {
            try
            {

                return File.ReadAllText( filePath, Encoding.Default );
            }
            catch 
            {
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Read a file in the form of     Name = Value
        /// 
        /// If the next line starts with '=' then it is joined to the previous value. The separator can be replace.
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <param name="sep">      (optional) the sep. </param>
        /// <returns>
        /// dictionary of pairs, keys are not case sensitive.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Dictionary<string, string> FileReadPNameValuePairs(string filePath, char sep = '=')
        {
            var lines = FileReadLines(filePath);

            var dict = new Dictionary<string, string>(lines.Length, StringComparer.OrdinalIgnoreCase);

            string value = "";
            string key = null;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                var index = line.IndexOf(sep);
                if (index < 0) continue;
                if (index == 0)
                {
                    // extension value
                    value = value + "\r\n" + line.Substring(index + 1).Trim();
                    continue;
                }
                if (key != null && !dict.ContainsKey(key))
                {
                    dict.Add(key, value);
                }
                key   = line.Substring(0, index).Trim();
                value = line.Substring(index + 1).Trim();
            }
            if (key != null && !dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
            return dict;

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File read all the lines of text.
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <returns>
        /// string[] of all lines or empty array if no file.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string[] FileReadLines(string filePath)
        {
            try
            {
                return File.ReadAllLines( filePath, Encoding.Default );
            }
            catch
            {
            }
            return new string[0];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Write a file as a single string.
        /// </summary>
        /// <param name="fullPath"> Full pathname of the full file. </param>
        /// <param name="text">     The text to write. </param>
        /// <returns>
        /// the string file context.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FileWriteText(string fullPath, string text )
        {
            try
            {
                FolderCreate(GetFolder(fullPath));
                File.WriteAllText(fullPath, text, Encoding.ASCII );
                return true;
            }
            catch 
            {
            }
             return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Write a file as a string[] in utf8
        /// </summary>
        /// <param name="fullPath"> Full pathname of the full file. </param>
        /// <param name="text">     The text to write. </param>
        /// <returns>
        /// the string file context.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FileWriteLines(string fullPath, string[] text)
        {
            try
            {
                FolderCreate( GetFolder( fullPath ) );
                File.WriteAllLines( fullPath, text, Encoding.UTF8 );
                return true;
            }
            catch
            {
            }
            return false;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Open a file for reading. Checks for file existence.
        /// </summary>
        /// <param name="path"> Path to the file to open. </param>
        /// <returns>   
        /// Stream of opened file or null if not opened. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static StreamReader FileOpenRead(string path)
        {
            if (String.IsNullOrEmpty(path) || !FileExists(path) ) return null;
            try
            {
                return new StreamReader( new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),Encoding.UTF8,true);
            }
            catch 
            {
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Open a file for writing. 
        /// </summary>
        /// <param name="path"> path to the file to write. </param>
        /// <param name="destroyExisting"> removes an existing file before opening.</param>
        /// <returns>   
        /// stream of opened file to write to or null if failed. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static StreamWriter FileOpenWrite(string path, bool destroyExisting = true)
        {
            if (String.IsNullOrWhiteSpace( GetFileNameAndExtension( path ) ))
            {
                return null;
            }

            try
            {
                FolderCreate(GetFolder(path));
                FileMode mode = FileExists(path)
                    ? ((destroyExisting) ? FileMode.Truncate : FileMode.Append)
                    : FileMode.Create;
                return new StreamWriter(File.Open(path, mode, FileAccess.Write, FileShare.ReadWrite),Encoding.UTF8);
            }
            catch
            {
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Open a file for writing. 
        /// </summary>
        /// <param name="path"> path to the file to write. </param>
        /// <returns>   
        /// stream of opened file to write to or null if failed. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static FileStream FileCreate(string path)
        {
            return File.Create(path);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Check if a file exists in a path. 
        /// </summary>
        /// <param name="path"> Path to file to check for. </param>
        /// <returns>   
        /// True if the file exists. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FileExists(string path)
        {
            return  !String.IsNullOrEmpty(path) && File.Exists(path);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Deletes the specified a full file path.  Checks if it exists first. 
        /// </summary>
        /// <param name="fullFilePath"> A full file path. </param>
        /// <returns>   
        /// true if it succeeds, false if it fails. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FileDelete(string fullFilePath)
        {
            if (String.IsNullOrEmpty(fullFilePath) || !FileExists(fullFilePath))
            {
                return false;
            }
            try
            {
                File.Delete(fullFilePath);
                return true;
            }
            catch 
            {
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File copy. Can't copy over self, and can't copy if destination exists.
        /// </summary>
        /// <param name="sourcePath">       Full pathname of the source file. </param>
        /// <param name="destinationPath">  Full pathname of the destination file. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FileCopy(string sourcePath, string destinationPath)
        {
            if ( FileExists( destinationPath )  || !FileExists(sourcePath))
            {
                return false;
            }

            try
            {
                File.Copy( sourcePath, destinationPath );
                return true;
            }
            catch
            {
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Moves a file. 
        /// </summary>
        /// <param name="oldPath">  The old path. </param>
        /// <param name="newPath">  A new path. </param>
        /// <returns>   
        /// false if file exists in new location or can't move it. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FileMove(string oldPath, string newPath)
        {
            if (FileExists( newPath ) || !FileExists( oldPath ))
            {
                return false;
            }

            try
            {
                FolderCreate(GetFolder(newPath));
                File.Move(oldPath, newPath);
                return true;
            }
            catch (Exception){}
        
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Locate a file by starting at the PathName (or application directory if PathName is null)
        /// and searching up the tree for the file. 
        /// </summary>
        /// <param name="pathName"> Path to start or null to start at the application directory. </param>
        /// <param name="fileName"> File name with or without an extension,  if null then assumes the path contains
        ///                         the filename. </param>
        /// <param name="ext">      If the extension does not begin with a '.' it is added. If null then ignores it</param>
        /// <returns>   
        /// . 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string FileExitsPath(string pathName, string fileName, string ext = null)
        {
            string oPath = PathCombine(pathName, fileName, ext);
            return File.Exists(oPath) ? oPath : null;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File write binary. No path check.
        /// </summary>
        /// <param name="fullFilePath"> Full pathname of the file to write data to. </param>
        /// <param name="byteData">     byte data to write. </param>
        /// <param name="start">        [optional start at 0]The start offset in data. </param>
        /// <param name="length">       [optional the remaining length] The length of the data to write. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FileWriteBinary(string fullFilePath, byte[] byteData, int start = 0, int length = -1)
        {
            if (byteData == null)
            {
                return false;
            }
            try
            {
                if (length < 0)
                {
                    length = byteData.Length - start;
                }
                // make sure folder is there first.
                FolderCreate(GetFolder(fullFilePath));
                var stream = File.OpenWrite( fullFilePath );
                stream.Write( byteData, start, length );
                stream.Close();
                return true;
            }
            catch
            {
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Reads binary data from the specified filepath as bytes. No checks.
        /// </summary>
        /// <param name="filePath"> The file path. (not checked) </param>
        /// <returns>   
        /// byte array or null. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] FileReadBinary(string filePath)
        {
            try
            {
                return File.ReadAllBytes( filePath );
            }
            catch
            {
            }
            return null;
        }

        #endregion
   
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Gets the absolute path for the specified path string. 
        /// </summary>
        /// <param name="filepath"> The filepath. </param>
        /// <returns>   
        /// The full path or null if not a valid path.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetFullPath(string filepath)
        {
            try
            {
                return String.IsNullOrEmpty( filepath ) ? null : Path.GetFullPath( filepath );
            }
            catch
            {
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File write value pairs.
        /// </summary>
        /// <param name="filePath"> A file path. </param>
        /// <param name="pairs">    The pairs. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void FileWriteValuePairs(string filePath, Dictionary<string, string> pairs)
        {
            var f = FileOpenWrite(filePath);
            if (f == null) return;

            foreach (var pair in pairs)
            {
                string s=pair.Value;
                s= s.Replace("\r\n", "\r\n  = ");
                f.Write("{0} = {1}\r\n",pair.Key,s);
            }
            f.Close();
        }
    }
}
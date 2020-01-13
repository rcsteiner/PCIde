////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the service ide class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/4/2015   rcs     Initial Implementation
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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Ide.Controller;
using Ide.Model;
using ZCore;

namespace Ide.Services
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Service file.  Provides loads and saves of files and settings.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ServiceFile : ServiceBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ServiceFile(ControllerIde controller) : base(controller)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a file watcher.
        /// </summary>
        /// <param name="path"> Full pathname of the file. </param>
        /// <returns>
        /// File watcher
        /// </returns>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FileSystemWatcher AddFileWatcher(string path)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = true;
            // Begin watching.
            watcher.EnableRaisingEvents = true;

            return watcher;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a solution given a filepath to the solution file.
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <returns>
        /// The solution.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ModelSolution GetSolution(string filePath)
        {
            var solution = new ModelSolution( filePath );

            //TODO: remove this temp code for defalut solution
            //if (basePath == null)
            //{
            //    // empty solution
            //    // open solution dialog box and get folder and name for soultion
            //    // create file and set up solution
            //    var newDlg = new DialogNew();

            //    var r = newDlg.ShowDialog();

            //    if (r != null && (bool) r)
            //    {
            //        // extract template from resources
            //        // expand template into the target folder
            //        // rename Solution.sln to new name
            //        // create project folder and add project.prj
            //    }
            //}
            ////TODO: remove
            // mSolution = new ModelSolution(basePath);

            // load the solution from the base path
            solution.Load( true );
            return  solution;

        }

        private Dictionary<string, string> fileIncludePatterns = new Dictionary<string, string>( 30, StringComparer.CurrentCultureIgnoreCase );
        private Dictionary<string, string> fileExcludePatterns = new Dictionary<string, string>( 30, StringComparer.CurrentCultureIgnoreCase );
        private string[] Excludes = new[] { ".zip", ".obj", ".out", ".mf", ".xml", ".class", ".doc",".docx" ,".dll",".exe",".config",".html",".pclst",".jar",".txt"};
        private string[] Includes = new[] { Constants.ZIP_SEARCH_PATTERNS };


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves a folder as zip. First it checks if there is a zip file there first, it deletes it and then compresses a
        /// new one.
        /// </summary>
        /// <param name="folder">   Pathname of the folder. </param>
        /// <param name="name">     The name. </param>
        /// <returns>
        /// path used to write and message.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string SaveFolderAsZip(string folder, string name)
        {
            ZipFilePatterns(Includes,Excludes);
            return ZipIt( FileUtil.PathCombine( folder, name, ".zip" ), folder );
            //if (FileUtil.FolderExists(folder))
            //{
            //    try
            //    {
            //       var target = FileUtil.PathCombine(folder, name, ".zip");
            //        if (FileUtil.FileExists(target))
            //        {
            //            FileUtil.FileDelete(target);
            //        }

            //       var files = FileUtil.FolderGetAllFiles(folder, Constants.ZIP_SEARCH_PATTERNS, true);

            //       using (ZipArchive zipFile = ZipFile.Open(target, ZipArchiveMode.Create))
            //       {
            //           foreach (string file in files)
            //           {
            //               //Adds the file to the archive
            //               if (FileUtil.GetFileExtension(file).IsOneOf(Excludes) || file.StartsWith(".")) continue;
            //               zipFile.CreateEntryFromFile( file,  file , CompressionLevel.Optimal );
            //           }
            //           zipFile.Dispose();
            //       }
            //        return string.Format("Created Zip file: '{0}'", target);
            //    }
            //    catch (Exception e)
            //    {
            //        return "Failed: " + e;
            //    }
            //}
            //return string.Format("Folder does not exist: '{0}'", folder);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Main entry-point for this application.
        /// </summary>
        /// <param name="includePatterns"> Array of include patterns </param>
        /// <param name="excludePatterns">  The exclude patterns. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ZipFilePatterns(string[] includePatterns, string[] excludePatterns)
        {
            fileExcludePatterns.Clear();
            fileIncludePatterns.Clear();

            for (int i = 2; i < includePatterns.Length; i++)
            {
                var ext = includePatterns[i].TrimStart( '*' );

                fileIncludePatterns.Add( ext, ext );
            }
            for (int i = 2; i < excludePatterns.Length; i++)
            {
                var ext = excludePatterns[i].TrimStart( '*' );

                fileExcludePatterns.Add( ext, ext );
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Zip iterator.
        /// </summary>
        /// <param name="targetPath">   Full pathname of the target file. </param>
        /// <param name="sourceFolder"> Pathname of the source folder. </param>
        /// <returns>
        /// text result of what happened.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private  string ZipIt(string targetPath, string sourceFolder)
        {
            //TODO cleanup zip code
            // 
            Controller.Output.Clear();
            // prep target path
            string targetFolder = null;
            string targetName   = null;
            string target       = null;
            int last            = 0;
            try
            {
                targetFolder = Path.GetDirectoryName( targetPath );
                targetName   = Path.GetFileNameWithoutExtension( targetPath );
                target       = string.Format( "{0}\\{1}{2}", targetFolder, targetName, ".zip" );

                if (String.IsNullOrEmpty( targetFolder ) || !FileUtil.FolderExists( targetFolder ))
                {
                    return string.Format( "Zip Target Folder does not exist: '{0}'", targetFolder );
                }

                var separatorChar    = FileUtil.SeparatorChar;
                var sourceFolderName = sourceFolder.TrimEnd( separatorChar );
                last                 = sourceFolderName.LastIndexOf( separatorChar ) + 1;
                if (last > 1)
                {
                    sourceFolderName = sourceFolderName.Substring( last );
                }

                if (sourceFolder[sourceFolder.Length - 1] != separatorChar)
                {
                    sourceFolder += separatorChar;
                }

                // prep source folder

                if (String.IsNullOrEmpty( sourceFolder ) || !FileUtil.FolderExists( sourceFolder ))
                {
                    return string.Format( "Zip Source Folder does not exist: '{0}'", sourceFolder );
                }

                // remove old zip if exits

                if (FileUtil.FileExists( target ))
                {
                    FileUtil.FileDelete(target);
                }
            }
            catch (Exception e)
            {
                return "Zip file Error: "+ e;
            }

            // open archive and write files
            try
            {

                var files = FolderGetAllFiles( sourceFolder, "*.*", true );

                using (ZipArchive zipFile = ZipFile.Open( target, ZipArchiveMode.Create ))
                {
                    foreach (string file in files)
                    {
                        //Adds the file to the archive

                        var extension = Path.GetExtension( file );
                        if (extension == null ||  fileExcludePatterns.ContainsKey(extension) || file.StartsWith("."))
                        {
                            continue;
                        }

                        // Adjust the filepath for the entry to bas based on the source Folder

                        var sourceName = file.Substring( last );
                        zipFile.CreateEntryFromFile( file, sourceName, CompressionLevel.Optimal );
                        Controller.OutputWrite(string.Format( "{0}\r\n", sourceName ));
                    }

                }
                return string.Format( "Created Zip file: '{0}'\r\n", target );
            }
            catch (Exception e)
            {
                return string.Format( "Zip creation Failed: " + e );
            }
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
                if (!String.IsNullOrEmpty( searchPattern ) && !String.IsNullOrEmpty( folder ) && Directory.Exists( folder ))
                {
                    return Directory.GetFiles( folder, searchPattern, searchSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );
                }
            }
            catch
            {
            }
            return new string[0];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Queries if a given file open.
        /// </summary>
        /// <param name="filename"> Filename of the file. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ModelFile FileOpen(string filename)
        {
            return new ModelFile( filename );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File load.
        /// </summary>
        /// <param name="fullPath"> Full pathname of the full file. </param>
        /// <returns>
        /// the text read
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string FileLoad(string fullPath)
        {
            var reader = FileUtil.FileOpenRead(fullPath);
            var text =  reader.ReadToEnd();
            reader.Close();
            return text;

            //using (var fs = new FileStream( fullPath, FileMode.Open, FileAccess.Read, FileShare.Read ))
            //{

            //    using (var reader =   FileReader.OpenStream( fs, Encoding.UTF8 ))
            //    {
            //        text = reader.ReadToEnd();
            //    }
            //}
            // return text;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File save the text in path.
        /// </summary>
        /// <param name="file"> The file. </param>
        /// <param name="text"> The text. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool FileSave(ModelFilePath file, string text)
        {
            return FileUtil.FileWriteText( file.FullPath, text );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// File create.
        /// </summary>
        /// <param name="folder">   Pathname of the folder. </param>
        /// <param name="baseName"> Name of the base. </param>
        /// <returns>
        /// the execution path
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string  FileCreate(string folder, string baseName)
        {
            var path = FileUtil.FileCreateUnique(folder, baseName);
            FileUtil.FileCreate(path).Close();
            return path;
        }
    }
}

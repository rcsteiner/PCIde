////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Description: Templates
// 
//  Author:      Robert C Steiner
// 
//=====================================================[ History ]=====================================================
//  Date        Who      What
//  ----------- ------   ----------------------------------------------------------------------------------------------
//  1/13/2020   RCS      Initial Code.
//====================================================[ Copyright ]====================================================
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
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using ZCore;
using System.IO.Compression;

namespace Ide
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Template definition 
    /// The template is stored in a separate folder, one for each template
    /// in the folder is two files:
    ///     Name.template
    ///     Name.zip
    /// 
    /// The Name.template folder has the following format
    /// Name: name of template (same as file normally)
    /// Group: group this belongs to
    /// Descripton: short description of this file (to end of file)
    /// </summary>
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class Template : Named
    {
        /// <summary>
        /// Gets or sets the default name.  The name will have be like:  Name{0}.pc  , where the {0} is substituted as a number.
        /// </summary>
        public string DefaultName { get; set; }


        /// <summary>
        /// Gets or sets the default file.  Used as template
        /// </summary>
        public string DefaultFile { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        public string ImageName { get; set;}

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets the zip file path.
        /// </summary>
        public string TemplatePath { get; set; }


        /// <summary>
        /// Gets or sets the tool set.
        /// </summary>
        public string ToolSet { get; set; }


        /// <summary>
        /// Gets the zip path.
        /// </summary>
        private string ZipPath {get { return FileUtil.PathCombine( FileUtil.GetFolder( TemplatePath ), FileUtil.GetFileName( TemplatePath ), ".zip" ); }}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class.
        /// </summary>
        /// <param name="templatePath"> The zip file path. </param>
        /// <param name="name">         The name. </param>
        /// <param name="type">         The type. </param>
        /// <param name="extension">    The extension. </param>
        /// <param name="imageName">    The name of the image. </param>
        /// <param name="group">        The group. </param>
        /// <param name="description">  The description. </param>
        /// <param name="defname">      . </param>
        /// <param name="toolset">      The tool set. </param>
        /// <param name="defFile">      The def file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Template(string templatePath, string name, string type, string extension, string imageName, string @group, string description, string defname, string toolset,string defFile) : base(name, description)
        {
            TemplatePath = templatePath;
            ImageName    = imageName;
            Group        = group;
            Extension    = extension;
            Type         = type;
            DefaultName  = defname;
            ToolSet      = toolset;
            DefaultFile  = defFile;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Creates this object.
        /// </summary>
        /// <param name="folder">                   Pathname of the folder. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Create(string folder)
        {
            try
            {
                if (FileUtil.FolderCreate(folder))
                {
                    var archive = ZipPath;

                    folder = folder.Replace('\\', '/');
                    if (archive != null)
                    {
                        using (ZipArchive zip = ZipFile.Open( archive, ZipArchiveMode.Read ))
                        {
                            foreach (var entry in zip.Entries)
                            {
                                var path = entry.FullName;

                                int n = path.IndexOf("/", 1, StringComparison.Ordinal);
                                if (n > 1)
                                {
                                    path = path.Substring(n + 1);
                                }

                                var destinationFileName = folder+path;

                                if (path.EndsWith("/"))
                                {
                                    FileUtil.FolderCreate(destinationFileName);
                                }
                                else
                                {

                                    try
                                    {
                                        entry.ExtractToFile( destinationFileName );
                                    }
                                    catch (Exception )
                                    {
                                    }
                                }
                            }
                        } 

                        
                    //    ZipFile.ExtractToDirectory(archive,folder);
                        return true;
                    }
                }
                else
                {
                    UserMessage.ShowErorr(string.Format("Could not create folder '{0}",folder),"Folder creation failed");
                }

            }
            catch (Exception e)
            {
                UserMessage.ShowException(e,"Failed creating "+Name);
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Creates a file from the file format at path.
        /// </summary>
        /// <param name="templateName"> Pathname of the folder. </param>
        /// <param name="filePath">     Full pathname of the file. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CreateFile(string templateName, string filePath)
        {
            try
            {
                {
                    var archive = ZipPath;

                    if (archive != null)
                    {
                        using (ZipArchive zip = ZipFile.Open( archive, ZipArchiveMode.Read ))
                        {
                            foreach (var entry in zip.Entries)
                            {
                                var path = new ZPath( entry.FullName);
                                if (path.FileNameAndExtension.Equals(templateName))
                                {
                                    FileUtil.FileDelete(filePath);
                                    entry.ExtractToFile( filePath );
                                    break;
                                }
                            }
                        }

                        //    ZipFile.ExtractToDirectory(archive,folder);
                        return true;
                    }
                }

            }
            catch (Exception e)
            {
                UserMessage.ShowException( e, "Failed creating " + filePath );
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads a template.
        /// </summary>
        /// <param name="templateFile"> The template file. </param>
        /// <param name="filtertype">   (optional) the filtertype. </param>
        /// <returns>
        /// The template.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Template LoadTemplate(string templateFile,string filtertype=null)
        {
            var dict       = FileUtil.FileReadPNameValuePairs(templateFile);
            string name    = StringUtil.ReadValue(dict,  "name");
            string desc    = StringUtil.ReadValue( dict, "Description");
            string group   = StringUtil.ReadValue(dict,  "Group");
            string type    = StringUtil.ReadValue( dict, "Type" );
            string defname = StringUtil.ReadValue(dict, "DefaultName", "Name{0}");
            string toolset = StringUtil.ReadValue( dict, "ToolSet", "PC" );
            string defFile = StringUtil.ReadValue(dict, "DefaultFile");

            if (filtertype != null && !filtertype.EqualsIgnoreCase(type))
            {
                return null;
            }
            var t = new Template( templateFile, name, type, FileUtil.GetFileExtension( templateFile ), ".zip", group, desc, defname ,toolset,defFile);
            return t;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Gets the file path of template. Look in application folder first. Then look in user documents.
        ///// </summary>
        ///// <param name="templatePath"> Path to builtin templates. </param>
        ///// <param name="alternate">    (optional) the alternate. </param>
        ///// <returns>
        ///// The file path of template.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public string GetFilePathOfTemplate(string templatePath, string alternate = null)
        //{
        //    var folder = TemplatePath;

        //    var filePath = FileUtil.PathCombine( folder, Name, Extension );
        //    if (FileUtil.FileExists( filePath ))
        //    {
        //        return filePath;
        //    }
        //    folder = alternate;

        //    filePath = FileUtil.PathCombine( folder, Name, Extension );
        //    if (FileUtil.FileExists( filePath ))
        //    {
        //        return filePath;
        //    }
        //    return null;
        //}

    }
}
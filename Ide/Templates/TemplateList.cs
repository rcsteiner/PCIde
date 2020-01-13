////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the template list class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/28/2015   rcs     Initial Implementation
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

using System.Collections.ObjectModel;
using ZCore;

namespace Ide
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A list of templates.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class TemplateList : ObservableCollection<Template>
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Adds the specified name.
        ///// </summary>
        ///// <param name="zipPath">      Full pathname of the zip file. </param>
        ///// <param name="name">         The name. </param>
        ///// <param name="type">         The type. </param>
        ///// <param name="extension">    The extension. </param>
        ///// <param name="imageName">    Name of the image. </param>
        ///// <param name="group">        The group. </param>
        ///// <param name="description">  The description. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public void Add(string zipPath,string name,string type, string extension, string imageName, string group, string description)
        //{
        //    Add( new Template(zipPath, name, type, extension, imageName, @group,description));
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a template files.
        ///  in the folder is two files:
        ///     Name.template Name.zip
        /// 
        /// The Name.template folder has the following format Name: name of template (same as file normally)
        /// Group: group this belongs to Descripton: short description of this file (to end of file)
        /// </summary>
        /// <param name="path"> Full pathname of the file. </param>
        /// <param name="type"> the type. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddTemplateFiles(string path, string type)
        {
            var templateFiles = FileUtil.FolderGetAllFiles( path, Constants.TEMPLATE_EXTENSION_SEARCH, true );
            foreach (var templateFile in templateFiles)
            {
                var template = Template.LoadTemplate(templateFile,type);
                if (template != null)
                {
                    Add(template);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads all templates.
        /// </summary>
        /// <param name="templatePath">             Full pathname of the template file. </param>
        /// <param name="alternateTemplatePath">    Full pathname of the alternate template file. </param>
        /// <param name="type">                     (optional) the type. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LoadAllTemplates(string templatePath, string alternateTemplatePath, string type=null)
        {
           AddTemplateFiles(templatePath,type);
           AddTemplateFiles( alternateTemplatePath,type);
        }
    }
}
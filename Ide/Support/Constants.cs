////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the constants class
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

using ZCore;

namespace Ide
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Constants used in ide core
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static class Constants
    {

        //TODO: merge with other constants!!!!
        // 

        public const string STD_SOLUTION_NAME         = "Solution";
        public const string SOLUTION_EXT              = ".pcsln";
        public const string PROJECT_EXT               = ".pcprj";
        public const string PC_EXT                    = ".pc";
        public const string PCLIST_EXT                = ".pclst";
        public const string JAVA_EXT                  = ".java";
        public const string PROJECT_FILE_PATTERN      = "*" + PROJECT_EXT;
        public const string TEMPLATE_EXTENSION        = ".template";
        public const string TEMPLATE_EXTENSION_SEARCH = "*"+TEMPLATE_EXTENSION;

        /// <summary>
        /// Identifier for the workspace content.
        /// </summary>
        public const string WORKSPACE_CONTENT_ID = "Solution:";

        /// <summary>
        /// Identifier for the tool content.
        /// </summary>
        public const string ERROR_LIST_CONTENT_ID = "Errors:";


        /// <summary>
        /// The start  page  content identifier
        /// </summary>
        public const string START_PAGE_CONTENT_ID = "StartPage:";


        /// <summary>
        /// The template built in folder
        /// </summary>
        public static readonly string TEMPLATE_BUILT_IN = FileUtil.ApplicationFolder + @"Templates\";


        /// <summary>
        /// The template alternate (user) folder
        /// </summary>
        public static readonly string TEMPLATE_ALTERATE =   FileUtil.MyDocumentsFolder + BuildInfo.Product + FileUtil.Separator + @"Templates\";

        public const string EXPLORER_EXE = "explorer.exe";

        public const string ZIP_SEARCH_PATTERNS = "*.*";

        /// <summary>
        /// The solution filter
        /// </summary>
        public const string SOLUTION_FILE_FILTER = "Solutions|*" + SOLUTION_EXT;

        /// <summary>
        /// The save file filter
        /// </summary>
        public const string SAVE_FILE_FILTER = "Pseudo Code |*"+ PC_EXT;

        /// <summary>
        /// The save file filter
        /// </summary>
        public const string OPEN_FILE_FILTER = "Pseudo Code (*.pc)|*" + PC_EXT + "|Pseudo Code Listing (*.pclst)|*" + PCLIST_EXT;

        /// <summary>
        /// The project file filter
        /// </summary>
        public const string PROJECT_FILE_FILTER = "Pseudo Code Project|*" + PROJECT_EXT;

        /// <summary>
        /// The HTMl file filter
        /// </summary>
        public const string HTML_FILE_FILTER = "HTML Files (*.html, *.htm)|*.html;*.htm|All Files (*.*)|*.*";

        
        /// <summary>
        /// The template file filter
        /// </summary>
        public const string TEMPLATE_FILE_FILTER = "Template Files (*.template)|*" + TEMPLATE_EXTENSION;
    }
}
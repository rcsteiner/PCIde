////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the build information class
//
// Author:
//  Robert C. Steiner
//
// History:
//  ===================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------
//   10/10/2013   rcs     Initial re-implementation.
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
using System.Reflection;

namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   
    /// Assembly attributes. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static class BuildInfo
    {
        /// <summary>
        /// The title
        /// </summary>
        public static readonly string Title;

        /// <summary>
        /// The company
        /// </summary>
        public static readonly string Company;

        /// <summary>
        /// The product
        /// </summary>
        public static readonly string Product;

        /// <summary>
        /// The file version
        /// </summary>
        public static readonly string FileVersion;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Constructor. 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static BuildInfo()
        {
            // initialize each parameter to ""
            Title             = "";
            Company           = "PerformanceSystems";
            Product           = "";
            FileVersion       = "0.0.0.0";
            Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            // walk through the attributes and collect the detail info
            foreach (Attribute attr in Attribute.GetCustomAttributes(assembly))
            {
                var attribute = attr as AssemblyTitleAttribute;
                if (attribute != null)
                {
                    Title = attribute.Title;
                    continue;
                }
                var companyAttribute = attr as AssemblyCompanyAttribute;
                if (companyAttribute != null)
                {
                    Company = companyAttribute.Company;
                    continue;
                }
                var productAttribute = attr as AssemblyProductAttribute;
                if (productAttribute != null)
                {
                    Product = productAttribute.Product;
                    continue;
                }
                var versionAttribute = attr as AssemblyFileVersionAttribute;
                if (versionAttribute != null)
                {
                    FileVersion = versionAttribute.Version;
                    continue;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert milliseconds version to version information includes build date and time
        /// </summary>
        /// <param name="versionInfo">  The version information, with the date and time adjusted on the build and revision. </param>
        /// <returns>
        /// versoin and date/time for revision
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string ConvertMSVersionToVersionInfo(string versionInfo)
        {
            // split off the build/revision
            var f = versionInfo.Split('.');
            var date = new DateTime(2000, 1, 1).AddDays(int.Parse(f[2])).AddSeconds(int.Parse(f[3])*2);
            return string.Format("{0}.{1}  Built: {2:F}", f[0], f[1], date);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert milliseconds version to version information just version no date.
        /// </summary>
        /// <param name="versionInfo">  The version information, with the date and time adjusted on the build and revision. </param>
        /// <returns>
        /// versoin and date/time for revision
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string ConvertMSVersionToVersionInfoShort(string versionInfo)
        {
            // split off the build/revision
            var f = versionInfo.Split( '.' );
            return string.Format( "{0}.{1}", f[0], f[1] );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the folder with app data.
        /// </summary>
        /// <returns>
        /// The folder for application data.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string DirAppData()
        {
            var dirPath = FileUtil.AppendSeparator( FileUtil.ApplicationDataFolder + Company);
            FileUtil.FolderCreate( dirPath );
            return dirPath;
        }
    }
}

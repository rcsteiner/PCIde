////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the settings class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/19/2015   rcs     Initial Implementation
//  =====================================================================================================
//
//  BSD 3-Clause License
//  Copyright (c) 2020, Robert C. Steiner
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
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
using Ide.Support;
using ZCore;

namespace Ide
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Application settings. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class AppSettings : Settings
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        private static readonly AppSettings DefaultInstance = new AppSettings();


        /// <summary>
        /// Gets the default.
        /// </summary>
        public static AppSettings Default {get { return DefaultInstance; }}

        /// <summary>
        /// Gets the settings file path.
        /// </summary>
        public static string SettingsFilePath { get { return BuildInfo.DirAppData() +  BuildInfo.Title + ".settings"; } }

   
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the maximum mru files.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int MaxMRUFiles
        {
            get { return FindInteger("MaxMRUFiles",10); }
            set { this["MaxMRUFiles"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether the last workspace should be opened.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OpenLastWorkspace
        {
            get { return FindBool("OpenLastWorkspace"); }
            set { this["OpenLastWorkspace"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether the generate list files.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GenerateListFiles
        {
            get { return FindBool("GenerateListFiles"); }
            set { this["GenerateListFiles"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the window top.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double WindowTop
        {
            get { return FindDouble("WindowTop"); }
            set { this["WindowTop"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the window left.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double WindowLeft
        {
            get { return FindDouble("WindowLeft"); }
            set { this["WindowLeft"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double WindowHeight
        {
            get { return FindDouble("WindowHeight"); }
            set { this["WindowHeight"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double WindowWidth
        {
            get { return FindDouble("WindowWidth"); }
            set { this["WindowWidth"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether the window maximize.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool WindowMaximize
        {
            get { return FindBool("WindowMaximize"); }
            set { this["WindowMaximize"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UserName
        {
            get { return FindString("UserName"); }
            set { this["UserName"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the user class.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UserClass
        {
            get { return FindString("UserClass"); }
            set { this["UserClass"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether the automatic load changed files.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool AutoLoadChangedFiles
        {
            get { return FindBool("AutoLoadChangedFiles"); }
            set { this["AutoLoadChangedFiles"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether the output window floating on run is shown.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ShowOutputWindowFloatingOnRun
        {
            get { return FindBool("ShowOutputWindowFloatingOnRun"); }
            set { this["ShowOutputWindowFloatingOnRun"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether the outline douments.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OutlineDouments
        {
            get { return FindBool("OutlineDouments"); }
            set { this["OutlineDouments"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether the automatic run after compile.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool AutoRun
        {
            get { return FindBool("AutoRun"); }
            set { this["AutoRun"] = value; }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether the automatic run after compile.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool NoSet
        {
            get { return FindBool("NoSet"); }
            set { this["NoSet"] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public AppSettings() : base(SettingsFilePath)
        {
            if (Count <2)
            {
                // must be at least some
                SetToDefaults();
                Save();
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets to defaults.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetToDefaults()
        {
            OpenLastWorkspace             = true;
            GenerateListFiles             = true;
            ShowOutputWindowFloatingOnRun = false;
            OutlineDouments               = false;
            AutoLoadChangedFiles          = true;
            WindowTop                     = 100;
            WindowLeft                    = 100;
            WindowWidth                   = 1200;
            WindowHeight                  = 800;
            WindowMaximize                = false;
            UserName                      = "";
            UserClass                     = "CSC119";
            AutoRun                       = false;
            NoSet                         = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads the settings from a file.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Load()
        {
            base.Load(SettingsFilePath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Save()
        {
            base.Save(SettingsFilePath);
        }
    }
}
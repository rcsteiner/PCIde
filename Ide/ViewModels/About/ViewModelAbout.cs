////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the about view model class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/15/2015   rcs     Initial Implementation
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
using System.Reflection;
using Ide.Controller;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Organize the viewmodel for an about program information presentation (e.g. HelpAbout dialog)
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelAbout : ViewModelBase
    {

        #region properties

        /// <summary>
        /// Get the title string of the view - to be displayed in the associated view (e.g. as dialog title)
        /// </summary>
        public string WindowTitle
        {
            get { return "HelpAbout This IDE"; }
        }

        /// <summary>
        /// Get title of application for display in HelpAbout view.
        /// </summary>
        public string AppTitle
        {
            get { return Assembly.GetEntryAssembly().GetName().Name; }
        }

        /// <summary>
        /// Gets the sub title.
        /// </summary>
        public string SubTitle
        {
            get { return "Pseudo code IDE"; }
        }

        /// <summary>
        /// Gets the assembly copyright.
        /// </summary>
        ///
        /// ### <value>
        /// The assembly copyright.
        /// </value>
        public string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                var attributes = Assembly.GetEntryAssembly()
                    .GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);

                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute) attributes[0]).Copyright;
            }
        }


        /// <summary>
        /// Gets the application URL display string.
        /// </summary>
        public string AppUrlDisplayString
        {
            get { return "CSC 119 Front Range Community College"; }
        }


        /// <summary>
        /// Gets the application URL.
        /// </summary>
        public string AppUrl
        {
            get { return "https://frcc.desire2learn.com/d2l/home/1314470"; }
        }


        /// <summary>
        /// Get application version for display in HelpAbout view.
        /// </summary>
        public string AppVersion
        {
            get { return Controller.AppVersion; }
        }

        /// <summary>
        /// Get version of runtime for display in HelpAbout view.
        /// </summary>
        public string RunTimeVersion
        {
            get { return BuildInfo.ConvertMSVersionToVersionInfo( Assembly.GetEntryAssembly().ImageRuntimeVersion); }
        }

        /// <summary>
        /// Get list of modules (referenced from EntryAssembly) and their version for display in HelpAbout view.
        /// </summary>
        public SortedList<string, string> Modules
        {
            get
            {
                var l = new SortedList<string, string>();

                var name = Assembly.GetEntryAssembly().FullName;

                foreach (var assembly in Assembly.GetEntryAssembly().GetReferencedAssemblies())
                {
                    try
                    {
                        string val;

                        if (l.TryGetValue(assembly.Name, out val) == false)
                        {
                            l.Add(assembly.Name, string.Format("{0}, {1}={2}", assembly.Name,"Version",assembly.Version));
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Gets the share URI.
        /// </summary>
        public string ShareUri { get {return "https://drive.google.com/folderview?id=0B_bjqo5PLsQeSWxLSWc1Vjk0TnM&usp=sharing";} }

        #endregion properties

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelAbout()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelAbout(ControllerIde controller) : base(controller)
        {
        }
    }
}
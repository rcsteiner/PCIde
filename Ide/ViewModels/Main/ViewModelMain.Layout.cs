////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the avalon dock layout view model class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/20/2015   rcs     Initial Implementation
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
//  USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.IO;
using System.Reflection;
using System.Windows.Input;
using Ide.Model;
using Views.Support;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Avalon dock layout view model.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public  partial class ViewModelMain
    {
        #region fields
        private RelayCommand<DockingManager> _loadLayoutCommand;
        private RelayCommand<string>         _saveLayoutCommand;
        #endregion fields


        /// <summary>
        /// Gets the docking manager.  Set during construction.
        /// </summary>
        public DockingManager DockManager;

        private XmlLayoutSerializer layoutSerializer;

        #region command properties

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Implement a command to load the layout of an AvalonDock-DockingManager instance. This layout defines the position
        /// and shape of each document and tool window displayed in the application. Parameter: The command expects a
        /// reference to a <seealso cref="DockManager" /> instance to work correctly. Not supplying that reference results
        /// in not loading a layout (silent return).
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ICommand LoadLayoutCommand
        {
            get
            {
                if (_loadLayoutCommand == null)
                {
                    //TODO: drop the parameter to simplify this
                    _loadLayoutCommand = new RelayCommand<DockingManager>(p =>
                    {
                        DockManager = p as DockingManager;

                        if (DockManager == null)
                        {
                            return;
                        }

                        LoadDockingManagerLayout(DockManager);
                    });
                }

                return _loadLayoutCommand;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Implements a command to save the layout of an AvalonDock-DockingManager instance. This layout defines the
        /// position and shape of each document and tool window displayed in the application. Parameter: The command expects
        /// a reference to a <seealso cref="string" /> instance to work correctly. The string is supposed to contain the XML
        /// layout persisted from the DockingManager instance. Not supplying that reference to the string results in not
        /// saving a layout (silent return).
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ICommand SaveLayoutCommand
        {
            get
            {
                if (_saveLayoutCommand == null)
                {
                    _saveLayoutCommand = new RelayCommand<string>(xmlLayout =>
                    {
                        if (xmlLayout == null)
                        {
                            return;
                        }

                        SaveDockingManagerLayout(xmlLayout);
                    });
                }

                return _saveLayoutCommand;
            }
        }

        #endregion command properties

        #region LoadLayout

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads the layout of a particular docking manager instance from persistence and checks whether a file should
        /// really be reloaded (some files may no longer be available).
        /// </summary>
        /// <param name="docManager">   Manager for document. </param>
        /// <param name="forceReset">   (optional) true to force reset. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LoadDockingManagerLayout(DockingManager docManager, bool forceReset=false)
        {
             layoutSerializer = new XmlLayoutSerializer(docManager);

            layoutSerializer.LayoutSerializationCallback += (s, args) =>
            {
                // This can happen if the previous session was loading a file
                // but was unable to initialize the view ...
                if (args.Model.ContentId == null)
                {
                    args.Cancel = true;
                    return;
                }

                ReloadContentOnStartUp(args);
            };

           var layoutFileName = Path.Combine(DirAppData, LayoutFileName);

          //  string[] names = this.GetType().Assembly.GetManifestResourceNames();

            if (forceReset || !FileUtil.FileExists(layoutFileName))
            {
                // load default layout file.

                LoadDefaultLayout();
                return;
            }

            layoutSerializer.Deserialize(layoutFileName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads the default layout.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public  void LoadDefaultLayout()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("Ide.Resources.DefaultLayout.xml");
            if (layoutSerializer != null)
            {
                layoutSerializer.Deserialize(stream);
            }
            return;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Reload content on start up.
        /// </summary>
        /// <param name="args"> Layout serialization callback event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private  void ReloadContentOnStartUp(LayoutSerializationCallbackEventArgs args)
        {
            var contentId = args.Model.ContentId;

            // Empty Ids are invalid but possible if aaplication is closed with File>New without edits.
            if (string.IsNullOrWhiteSpace(contentId))
            {
                args.Cancel = true;
                return;
            }

            //Note: this is where the non-document windows are built.
            // 
            if (contentId.StartsWith(Constants.WORKSPACE_CONTENT_ID))
            {
                // get the workspace path too and recreate / load workspace here use content id to identify the path.
                args.Content= IdeController.GetWorkspace(contentId.Substring(Constants.WORKSPACE_CONTENT_ID.Length));
                IdeController.ShowToolView(true, IdeController.VMWorkspace );
                ShowWorkspace = true;
            }
            else if (contentId == Constants.ERROR_LIST_CONTENT_ID)
            {
                args.Content  = IdeController.VMErrorList;
                IdeController.ShowToolView(true, IdeController.VMErrorList );
                ShowErrorList = true;
            }
            else if (contentId == Constants.OUTPUT_CONTENT_ID)
            {
                args.Content = IdeController.VMOutput;
                IdeController.ShowToolView(true, IdeController.VMOutput );
                ShowOutput = true;
            }
            else if (contentId == Constants.DEBUG_VARIABLES_CONTENT_ID)
            {
                args.Content = IdeController.VMDebugVariables;
                IdeController.ShowToolView( true, IdeController.VMDebugVariables );
                ShowOutput = true;
            }
            else if (contentId == Constants.DEBUG_STACK_CONTENT_ID)
            {
                args.Content = IdeController.VMDebugStack;
                IdeController.ShowToolView( true, IdeController.VMDebugStack );
                ShowOutput = true;
            }
            else if (contentId == Constants.ELEVATOR_CONTENT_ID)
            {
                args.Content = IdeController.VMElevator;
                IdeController.ShowToolView(true, IdeController.VMElevator);
                ShowOutput = true;
            }
            else if (contentId == Constants.STATS_CONTENT_ID)
            {
                args.Content = IdeController.VMStats;
                IdeController.ShowToolView(true, IdeController.VMStats);
                ShowOutput = true;
            }

            //else if (contentId == Constants.CHART_CONTENT_ID)
            //{
            //    args.Content = IdeController.VMChart;
            //    IdeController.ShowToolView(true, IdeController.VMChart);
            //    ShowOutput = true;
            //}
            else
            {
                args.Content = ReloadDocument(contentId);

                if (args.Content == null)
                {
                    args.Cancel = true;
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Reload document.
        /// </summary>
        /// <param name="path"> Full pathname of the file. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private  object ReloadDocument(string path)
        {
            object ret = null;

            if (!string.IsNullOrWhiteSpace(path))
            {
                switch (path)
                {
                    case Constants.START_PAGE_CONTENT_ID: 
                        // Re-create start page content
                        //  ret =ViewModelMain.This.GetStartPage();
                        //if (ViewModelMain.This.GetStartPage(false) == null)
                        //{
                        //  ret =Controller.GetStartPage(true);
                        //}
                        break;
                   
                    // TODO: Add view model calls to support other kinds of wihdow

                    default:
                        // Re-create text document
                        ret =IdeController.FileOpen(new ModelFile(path));
                        break;
                }
            }

            return ret;
        }

        #endregion LoadLayout

        #region SaveLayout

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves a docking manager layout.
        /// </summary>
        /// <param name="xmlLayout">    The xml layout. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SaveDockingManagerLayout(string xmlLayout)
        {
            // Create XML Layout file on close application (for re-load on application re-start)
            if (xmlLayout == null)
            {
                return;
            }

            var fileName = Path.Combine(DirAppData, LayoutFileName);

            FileUtil.FileWriteText(fileName, xmlLayout);
        }

        #endregion SaveLayout
    }
}
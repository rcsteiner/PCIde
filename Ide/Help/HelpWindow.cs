////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the help Windows Form
//  Based on Application+Code Charles Petzold's book.
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/7/2015   rcs     Initial Implementation
//  =====================================================================================================
//
// Copyright:
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml;

namespace Help
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Form for viewing the help. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class HelpWindow
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public HelpWindow()
        {
            InitializeComponent();
            treevue.Focus();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="strTopic"> The string topic. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public HelpWindow(string strTopic) : this()
        {
            if (strTopic != null)
            {
                frame.Source = new Uri(strTopic, UriKind.Relative);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Search through items in TreeView to select one.
        /// </summary>
        /// <param name="ctrl">         The control. </param>
        /// <param name="strSource">    The string source. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool FindItemToSelect(ItemsControl ctrl, string strSource)
        {
            foreach (var obj in ctrl.Items)
            {
                var xml          = obj as XmlElement;
                var strAttribute = xml.GetAttribute("Source");
                var item         = (TreeViewItem) ctrl.ItemContainerGenerator.ContainerFromItem(obj);

                // If the TreeViewItem matches the Frame URI, select the item.
                if (!string.IsNullOrEmpty(strAttribute) && strSource.EndsWith(strAttribute))
                {
                    if (item != null && !item.IsSelected)
                    {
                        item.IsSelected = true;
                    }

                    return true;
                }

                // Expand the item to search nested items.
                if (item != null)
                {
                    var isExpanded = item.IsExpanded;
                    item.IsExpanded = true;

                    if (item.HasItems && FindItemToSelect(item, strSource))
                    {
                        return true;
                    }

                    item.IsExpanded = isExpanded;
                }
            }
            return false;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  When the Frame has navigated to a new source, synchronize TreeView.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="args">     Navigation event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FrameOnNavigated(object sender, NavigationEventArgs args)
        {
            if (args.Uri != null && !string.IsNullOrEmpty( args.Uri.OriginalString))
            {
                FindItemToSelect(treevue, args.Uri.OriginalString);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  When TreeView selected item changes, set the source of the Frame,
        ///     new Uri("pack://siteoforigin:,,,/myFile.htm", UriKind.RelativeOrAbsolute)

        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="args">     The arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TreeViewOnSelectedItemChanged(object sender,RoutedPropertyChangedEventArgs<object> args)
        {
            if (treevue.SelectedValue != null)
            {
                var uriString = treevue.SelectedValue as string;
                var uri = uriString.StartsWith("pack")
                    ? new Uri(uriString, UriKind.Absolute)
                    : new Uri(uriString, UriKind.Relative);
                frame.Source = uri;
            }
        }
    }
}
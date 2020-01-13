﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the layout initializer class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/24/2015   rcs     Initial Implementation
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
//  USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.Reserved.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Linq;
using Xceed.Wpf.AvalonDock.Layout;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Layout initializer. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    internal class LayoutInitializer : ILayoutUpdateStrategy
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Before insert anchorable.
        /// </summary>
        /// <param name="layout">               The layout. </param>
        /// <param name="anchorableToShow">     The anchorable to show. </param>
        /// <param name="destinationContainer"> Destination container. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow,ILayoutContainer destinationContainer)
        {
            //AD wants to add the anchorable into destinationContainer
            //just for test provide a new anchorablepane 
            //if the pane is floating let the manager go ahead
            var destPane = destinationContainer as LayoutAnchorablePane;
        
            if (destinationContainer != null && destinationContainer.FindParent<LayoutFloatingWindow>() != null)
            {
                return false;
            }

            var toolsPane =  layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == "ToolsPane");
            if (toolsPane != null)
            {
                toolsPane.Children.Add(anchorableToShow);
                return true;
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// After insert anchorable.
        /// </summary>
        /// <param name="layout">           The layout. </param>
        /// <param name="anchorableShown">  The anchorable shown. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Before insert document.
        /// </summary>
        /// <param name="layout">               The layout. </param>
        /// <param name="anchorableToShow">     The anchorable to show. </param>
        /// <param name="destinationContainer"> Destination container. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow,ILayoutContainer destinationContainer)
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// After insert document.
        /// </summary>
        /// <param name="layout">           The layout. </param>
        /// <param name="anchorableShown">  The anchorable shown. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
        }
    }
}
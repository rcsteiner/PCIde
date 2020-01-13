////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the panes template selector class
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

using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Panes template selector.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PanesTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        ///     Gets or sets the file view template.
        /// </summary>
        public DataTemplate FileTemplate { get; set; }

        /// <summary>
        ///     Gets or sets the chart view template.
        /// </summary>
        public DataTemplate ChartTemplate { get; set; }

        /// <summary>
        ///     Gets or sets the stats view template.
        /// </summary>
        public DataTemplate StatsTemplate { get; set; }

        /// <summary>
        ///     Gets or sets the building view template.
        /// </summary>
        public DataTemplate BuildingTemplate { get; set; }

        /// <summary>
        ///     Gets or sets the file error view template.
        /// </summary>
        public DataTemplate ErrorListTemplate { get; set; }

        /// <summary>
        /// Gets or sets the solution template.
        /// </summary>
        public DataTemplate WorkspaceTemplate { get; set; }

        /// <summary>
        /// Gets or sets the output template.
        /// </summary>
        public DataTemplate OutputTemplate { get; set; }


        /// <summary>
        /// Gets or sets the variables template.
        /// </summary>
        public DataTemplate DebugVariablesTemplate { get; set; }

        /// <summary>
        /// Gets or sets the stack template.
        /// </summary>
        public DataTemplate DebugStackTemplate { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate" /> based on custom logic.
        /// </summary>
        /// <param name="item">         The data object for which to select the template. </param>
        /// <param name="container">    The data-bound object. </param>
        /// <returns>
        ///     Returns a <see cref="T:System.Windows.DataTemplate" /> or null. The default value is null.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            var typeName = item.GetType().Name;
            switch (typeName)
            {
                case "ViewModelStats":
                    return StatsTemplate;

                case "ViewModelChart":
                    return ChartTemplate;

                case "ViewModelElevator":
                    return BuildingTemplate;

                case "ViewModelOutput":
                 return OutputTemplate;

                case "ViewModelDebugStack":
                 return DebugStackTemplate;

                case "ViewModelDebugVariables":
                 return DebugVariablesTemplate;

                case "ViewModelErrorList":
                 return ErrorListTemplate;

                case "ViewModelFileEdit":
                 return FileTemplate;

                case "ViewModelWorkspace":
                 return WorkspaceTemplate;
            }
         
           return base.SelectTemplate(item, container);
        }
    }
}
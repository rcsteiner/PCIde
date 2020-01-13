////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the panes style selector class
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
using ViewModels;

namespace Views

{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Panes style selector. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    internal class PanesStyleSelector : StyleSelector
    {
        /// <summary>
        /// Gets or sets the tool style.
        /// </summary>
        public Style ToolStyle { get; set; }

        /// <summary>
        /// Gets or sets the file style.
        /// </summary>
        public Style FileStyle { get; set; }


        /// <summary>
        /// Gets or sets the solution style.
        /// </summary>
        public Style SolutionStyle { get; set; }

        /// <summary>
        /// Gets or sets the solution style.
        /// </summary>
        public Style WorkspaceStyle { get; set; }

        /// <summary>
        /// Gets or sets the output style.
        /// </summary>
        public Style OutputStyle { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.Style" /> based on custom logic.
        /// </summary>
        /// <param name="item">         The content. </param>
        /// <param name="container">    The element to which the style will be applied. </param>
        /// <returns>
        /// Returns an application-specific style to apply; otherwise, null.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ViewModelWorkspace)
            {
                return WorkspaceStyle;
            }


            if (item is ViewModelFileEdit)
            {
                return FileStyle;
            }

            if (item is ViewModelSolution)
            {
                return SolutionStyle;
            }

            // if (item is ViewModelOutput)
            //{
            //    return OutputStyle;
            //}
       
            if (item is ViewModelTool)
            {
                return ToolStyle;
            }

           return base.SelectStyle(item, container);
        }
    }
}
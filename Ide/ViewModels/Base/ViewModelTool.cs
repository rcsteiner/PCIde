////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the tool view model class
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

using Ide.Controller;
using Views;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Tool view model. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelTool : ViewModelPane
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// true to show, false to hide the is.
        /// </summary>
        private bool _isVisible = true;

        /// <summary>
        /// Gets or sets a value indicating whether this object is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged("IsVisible");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">         The name. </param>
        /// <param name="controller">   The controller. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelTool(string name, ControllerIde controller)
            : this( controller )
        {
            Name   = name;
            _title = name;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelTool(ControllerIde controller)
            : base( controller )
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        /// <param name="view">         The view. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelTool(ControllerIde controller, IView view)
            : base( controller, view )
        {
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return Title;
        }

    }
}
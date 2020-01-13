////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the error view model class
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

using System;
using System.Collections.ObjectModel;
using System.Security.RightsManagement;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ide.Controller;
using pc;
using Views;
using Views.Support;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  File stats view model.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelElevator : ViewModelTool
    {

        /// <summary>
        ///  Get/Set Canvas.
        /// </summary>
        public Canvas MyCanvas { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor:
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelElevator() : base("Elevator", null)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default constructor.
        /// </summary>
        /// <param name="controller"> The controller.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelElevator(ControllerIde controller): base("Elevator",controller)
        {
            ContentId = Constants.ELEVATOR_CONTENT_ID;
            IconSource = IconUtil.GetImage("elevator");
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  New message is received, filter and decide if we are interesed in it.
        ///  Default is to ignore it.
        ///  we look for the user input complete message from the view and capture the data.
        /// </summary>
        /// <param name="sender">  The sender.</param>
        /// <param name="key">     The key.</param>
        /// <param name="payload"> The payload.</param>
        /// <returns>
        ///  the message result, what to do.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override MessageResult NewMessage(IMessageListener sender, string key, object payload)
        {
            //if (key == "UserInput")
            //{
            //    return MessageResult.HANDLED_STOP;
            //}
            return MessageResult.IGNORED;
        }
    }
}






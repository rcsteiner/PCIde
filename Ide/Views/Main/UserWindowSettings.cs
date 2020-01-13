////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the user window settings class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/14/2015   rcs     Initial Implementation
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
using Ide;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// User window settings. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class UserWindowSettings
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the window top.
        /// </summary>
        public double WindowTop { get; set; }

        /// <summary>
        /// Gets or sets the window left.
        /// </summary>
        public double WindowLeft { get; set; }

        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        public double WindowHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        public double WindowWidth { get; set; }

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        public WindowState WindowState { get; set; }

        #endregion //Public Properties

        #region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public UserWindowSettings()
        {
            //Load the settings
            Load();

            //Size it to fit the current screen
            SizeToFit();

            //Move the window at least partially into view
            MoveIntoView();
        }

        #endregion 

        #region Functions

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// If the saved window dimensions are larger than the current screen shrink the window to fit.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SizeToFit()
        {
            if (WindowHeight > SystemParameters.VirtualScreenHeight)
            {
                WindowHeight = SystemParameters.VirtualScreenHeight;
            }

            if (WindowWidth > SystemParameters.VirtualScreenWidth)
            {
                WindowWidth = SystemParameters.VirtualScreenWidth;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// If the window is more than half off of the screen move it up and to the left so half the height and half the
        /// width are visible.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MoveIntoView()
        {
            if (WindowTop + WindowHeight / 2 > SystemParameters.VirtualScreenHeight)
            {
                WindowTop = SystemParameters.VirtualScreenHeight - WindowHeight;
            }

            if (WindowLeft + WindowWidth / 2 > SystemParameters.VirtualScreenWidth)
            {
                WindowLeft = SystemParameters.VirtualScreenWidth - WindowWidth;
            }

            if (WindowTop < 0)
            {
                WindowTop = 0;
            }

            if (WindowLeft < 0)
            {
                WindowLeft = 0;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads this object.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Load()
        {
            WindowTop    = AppSettings.Default.WindowTop;
            WindowLeft   = AppSettings.Default.WindowLeft;
            WindowHeight = AppSettings.Default.WindowHeight;
            WindowWidth  = AppSettings.Default.WindowWidth;
            WindowState  = AppSettings.Default.WindowMaximize ? WindowState.Maximized : WindowState.Normal;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves this object.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Save()
        {
            if (WindowState != WindowState.Minimized)
            {
                AppSettings.Default.WindowTop      = WindowTop;
                AppSettings.Default.WindowLeft     = WindowLeft;
                AppSettings.Default.WindowHeight   = WindowHeight;
                AppSettings.Default.WindowWidth    = WindowWidth;
                AppSettings.Default.WindowMaximize = WindowState == WindowState.Maximized;

                AppSettings.Default.Save();
            }
        }

        #endregion //Functions
    }
}
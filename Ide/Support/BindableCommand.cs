////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the bindable command class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/13/2015   rcs     Initial Implementation
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
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Input;

namespace Ide.Support
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     This class can be used to create a bound ICommand in resources
    ///     which may then be assigned to the Command property of Key/Mouse gestures.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class BindableCommand : Freezable, ICommand
    {
        /// <summary>
        ///     ICommand implementation to bind to the input type.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",typeof (ICommand), typeof (BindableCommand));

        /// <summary>
        ///     Parameter for the ICommand.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =DependencyProperty.Register("CommandParameter", typeof (object), typeof (BindableCommand));

        /// <summary>
        ///     Gets or sets and sets the Command.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        ///     Gets or sets and sets the CommandParameter.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        ///     This is used to determine if the command validity has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { Command.CanExecuteChanged += value; }
            remove { Command.CanExecuteChanged -= value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Returns whether the command is valid.
        /// </summary>
        /// <param name="parameter">    . </param>
        /// <returns>
        ///     true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CanExecute(object parameter)
        {
            return Command.CanExecute(CommandParameter);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <param name="parameter">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Execute(object parameter)
        {
            Command.Execute(CommandParameter);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Creates an instance of the object.
        /// </summary>
        /// <returns>
        ///     .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override Freezable CreateInstanceCore()
        {
            return new BindableCommand();
        }
    }
}
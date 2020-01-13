////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the relay command class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/4/2015   rcs     Initial Implementation
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

using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Views.Support
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default
    ///     return value for the CanExecute method is 'true'.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        private readonly Action<T>    _execute;
        private readonly Predicate<T> _canExecute;

        #endregion 

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Creates a new command.
        /// </summary>
        /// <param name="execute">  The execution logic. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Creates a new command.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="execute">      The execution logic. </param>
        /// <param name="canExecute">   The execution status logic. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute    = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this
        ///     object can be set to null.
        /// </param>
        /// <returns>
        ///     true if this command can be executed; otherwise, false.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T) parameter);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this
        ///     object can be set to null.
        /// </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Execute(object parameter)
        {
            _execute((T) parameter);
        }

        #endregion // ICommand Members

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default
    ///     return value for the CanExecute method is 'true'.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class RelayCommand : ICommand
    {

        #region Fields

        private readonly Action     _execute;
        private readonly Func<bool> _canExecute;

        #endregion // Fields

        #region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Creates a new command.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="execute">      The execution logic. </param>
        /// <param name="canExecute">   The execution status logic. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public RelayCommand(Action execute, Func<bool> canExecute=null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");}

            _execute    = execute;
            _canExecute = canExecute;
        }

        #endregion 

        #region ICommand Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this
        ///     object can be set to null.
        /// </param>
        /// <returns>
        ///     true if this command can be executed; otherwise, false.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this
        ///     object can be set to null.
        /// </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Execute(object parameter)
        {
            _execute();
        }

        #endregion 
    }
}
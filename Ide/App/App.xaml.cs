////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the app.xaml class
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
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Ide.Controller;
using ZCore;

namespace Ide
{
        public class DebugDummyConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                Debugger.Break();
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                Debugger.Break();
                return value;
            }
        }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Startup logic.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class App
    {
        private ControllerIde _controller;
        private int _unhandledErrorCount=0;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Application startup.  the default controller is created and it starts the application.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Startup event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void AppStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var solution = e.Args.Length > 0 ? e.Args[0] : null;

            IconUtil.Load( Assembly.GetExecutingAssembly(), "views/images");
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
           
            _controller = new ControllerIde();
            _controller.Start(solution);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the dispatcher unhandled exception action.
        /// </summary>
        /// <param name="sender">                                   Source of the event. </param>
        /// <param name="args">    Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDispatcherUnhandledException(object sender,DispatcherUnhandledExceptionEventArgs args)
        {
            string errorMessage = string.Format( "An unhandled exception occurred: {0}", args.Exception.Message );
            MessageBox.Show( errorMessage, "WPF Error - please file a error report and include this message.", MessageBoxButton.OK, MessageBoxImage.Error );
            if (_unhandledErrorCount++ > 1)
            {
                OnExit(null);
            }
            args.Handled = true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit" /> event.
        /// </summary>
        /// <param name="e">    An <see cref="T:System.Windows.ExitEventArgs" /> that contains the event data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _controller.Shutdown();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Application dispatcher unhandled exception.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Dispatcher unhandled exception event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //Handling the exception within the UnhandledException handler.
            UserMessage.ShowException(e.Exception, "Exception Caught");
            e.Handled = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by CurrentDomain for unhandled exception events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Unhandled exception event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UserMessage.ShowException( e.ExceptionObject as Exception, "Uncaught Thread Exception" );
        }
    }
}

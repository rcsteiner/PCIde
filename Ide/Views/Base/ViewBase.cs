////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the view base class
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZCore;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     This is the basis of all views. It cannot be Abstract because of design time issues when it tries to instantiate
    ///     this class. Note that this 'view' doesn't have any Xaml (because you can't inherit Xaml)
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewBase : UserControl, IDisposable, IView, IMessageListener
    {

        /// <summary>
        ///     The on window closed.
        /// </summary>
        private OnWindowClose _onWindowClosed;

        /// <summary>
        ///     If shown on a window, the window used
        /// </summary>
        private ViewWindow _viewWindow;

        #region ScaleFactor
        /// <summary>
        /// The scale factor property
        /// </summary>
        public static readonly DependencyProperty ScaleFactorProperty = DependencyProperty.Register( "ScaleFactor", typeof( double ), typeof( ViewBase ), new PropertyMetadata( 1.0 ) );

        /// <summary>
        /// Gets or sets the scale value.
        /// </summary>
        public double ScaleFactor
        {
            get { return (double)GetValue( ScaleFactorProperty ); }
            set { SetValue( ScaleFactorProperty, value ); }
        }

        #endregion

   
        #region Window

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     The Window on which the View is displayed (if it is displayed on a Window)
        ///     The Window will be created by the View on demand (if required) or may be supplied by the application.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ViewWindow ViewWindow
        {
            get
            {
                if (_viewWindow == null)
                {
                    _viewWindow         = new ViewWindow();
                    _viewWindow.Closed += ViewsWindow_Closed;
                }
                return _viewWindow;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Disconnect
        /// </summary>
        /// <param name="child"> The child.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void Disconnect(Control child)
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return;

            var parentAsPanel = parent as Panel;
            if (parentAsPanel != null)
            {
                parentAsPanel.Children.Remove(child);
            }
            var parentAsContentControl = parent as ContentControl;
            if (parentAsContentControl != null)
            {
                parentAsContentControl.Content = null;
            }
            var parentAsDecorator = parent as Decorator;
            if (parentAsDecorator != null)
            {
                parentAsDecorator.Child = null;
            }
        }
        #endregion

        #region IDisposable Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Dispose of this object, cleaning up any resources it uses.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void IDisposable.Dispose()
        {
            // Remove any events from our window to prevent any memory leakage.
            if (_viewWindow != null)
            {
                _viewWindow.Closed -= ViewsWindow_Closed;
            }
        }

        #endregion

        #region Closing

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     The view is closing, so clean up references.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ViewClosed()
        {
            // In order to handle the case where the user closes the window (rather than us controlling the close via a ViewModel)
            // we need to check that the DataContext isn't null (which would mean this ViewClosed has already been done)
            if (DataContext != null)
            {
                //((BaseViewModel) DataContext).ViewModelClosing -= ViewModelClosingHandler;
                //((BaseViewModel) DataContext).ViewModelActivating -= ViewModelActivatingHandler;
                DataContext = null; // Make sure we don't have a reference to the VM any more.
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Handle the Window Closed event.
        /// </summary>
        /// <param name="sender">   . </param>
        /// <param name="e">        . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ViewsWindow_Closed(object sender, EventArgs e)
        {
            if (_onWindowClosed != null)
            {
                _onWindowClosed(sender, e);
            }
            //((BaseViewModel) DataContext).CloseViewModel(false);
        }

        #endregion

        #region IView implementations

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Tell the View to close itself. Handle the case where we're in a window and the window needs closing.
        /// </summary>
        /// <param name="dialogResult"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ViewModelClosingHandler(bool dialogResult)
        {
            if (_viewWindow == null)
            {
                var panel = Parent as Panel;
                if (panel != null)
                {
                    panel.Children.Remove(this);
                }
            }
            else
            {
                _viewWindow.Closed -= ViewsWindow_Closed;

                if (_viewWindow.IsDialogWindow)
                {
                    // If the window is a Dialog and is not actiuve it must be in the process of being closed
                    if (_viewWindow.IsActive)
                    {
                        _viewWindow.DialogResult = dialogResult;
                    }
                }
                else
                {
                    _viewWindow.Close();
                }
                _viewWindow = null;
            }
            // Process the ViewClosed method to cater for if this has been instigated by the user closing a window, rather than by
            // the close being instigated by a ViewModel
            ViewClosed();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     View model activating handler.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ViewModelActivatingHandler()
        {
            if (_viewWindow != null)
            {
                _viewWindow.Activate();
            }
        }

        #endregion

        #region Showing methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Show this control in a window, sized to fit, with this title.
        /// </summary>
        /// <param name="windowTitle">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowAsWindow( string windowTitle)
        {
            _viewWindow       = ViewWindow;
            _viewWindow.Title = windowTitle;

            DockPanel.SetDock( this, Dock.Top );
            _viewWindow.WindowDockPanel.Children.Add( this );
            _viewWindow.SizeToContent = SizeToContent.Manual;
            //_viewWindow.Width = windowWidth;
            //_viewWindow.Height = windowHeight;
   
            _viewWindow.Show();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Show this control in a window, sized to fit, with this title.
        /// </summary>
        /// <param name="modal">        true to modal. </param>
        /// <param name="windowTitle">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowInWindow(bool modal, string windowTitle)
        {
            ShowInWindow(modal, windowTitle, 900,600, Dock.Top, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Show this control in an existing window, by default docked top.
        /// </summary>
        /// <param name="modal">    true to modal. </param>
        /// <param name="window">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowInWindow(bool modal, ViewWindow window)
        {
            ShowInWindow(modal, window, window.Title, window.Width, window.Height, Dock.Top, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Maximum Flexibility of Window Definition version of Show In Window.
        /// </summary>
        /// <param name="modal">            true to modal. </param>
        /// <param name="window">           THe Window in which to show this View. </param>
        /// <param name="windowTitle">      A Title for the Window. </param>
        /// <param name="windowWidth">      The Width of the Window. </param>
        /// <param name="windowHeight">     The Height of the Window. </param>
        /// <param name="dock">             How should the View be Docked. </param>
        /// <param name="onWindowClose">    Event handler for when the window is closed. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowInWindow(bool modal, ViewWindow window, string windowTitle, double windowWidth,double windowHeight, Dock dock, OnWindowClose onWindowClose)
        {
            _onWindowClosed   = onWindowClose;
            _viewWindow       = window;
            _viewWindow.Title = windowTitle;

            DockPanel.SetDock(this, dock);
            // The viewWindow must have a dockPanel called WindowDockPanel. If you want to change this to use some other container on the window, then
            // the below code should be the only place it needs to be changed.
            _viewWindow.WindowDockPanel.Children.Add(this);

            if (windowWidth == 0 && windowHeight == 0)
            {
                _viewWindow.SizeToContent = SizeToContent.WidthAndHeight;
            }
            else
            {
                _viewWindow.SizeToContent = SizeToContent.Manual;
                _viewWindow.Width         = windowWidth;
                _viewWindow.Height        = windowHeight;
            }

            if (modal)
            {
                _viewWindow.ShowDialog();
            }
            else
            {
                _viewWindow.Show();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Show the View in a New Window.
        /// </summary>
        /// <param name="modal">            true to modal. </param>
        /// <param name="windowTitle">      Give the Window a Title. </param>
        /// <param name="windowWidth">      Set the Window's Width. </param>
        /// <param name="windowHeight">     Set the Window's Height. </param>
        /// <param name="dock">             How to Dock the View in the Window. </param>
        /// <param name="onWindowClose">    Event handler for when the Window closes. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowInWindow(bool modal, string windowTitle, double windowWidth, double windowHeight, Dock dock,OnWindowClose onWindowClose)
        {
            ShowInWindow(modal, ViewWindow, windowTitle, windowWidth, windowHeight, dock, onWindowClose);
        }

        #endregion

        #region Zoom mouse wheel support.

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseWheel" /> attached event reaches an
        /// element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="args"> The <see cref="T:System.Windows.Input.MouseWheelEventArgs" /> that contains the event data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs args)
        {
            base.OnPreviewMouseWheel(args);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                ScaleFactor += (args.Delta > 0) ? 0.1 : -0.1;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseDown" /> attached routed event
        /// reaches an element in its route that is derived from this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="args"> The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data.
        ///                     The event data reports that one or more mouse buttons were pressed. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnPreviewMouseDown(MouseButtonEventArgs args)
        {
            base.OnPreviewMouseDown(args);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (args.MiddleButton == MouseButtonState.Pressed)
                {
                    ScaleFactor = 1.0;
                }
            }
        }


        #endregion

        #region IMessage Listener

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// New message is received.  Check it to see if we are interested in it.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="key">      The key. </param>
        /// <param name="payload">  The payload. </param>
        /// <returns>
        /// the message result, what to do.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public MessageResult NewMessage(IMessageListener sender, string key, object payload)
        {
            return MessageResult.IGNORED;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sends a message to all listeners.  The listeners provide the filtering.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="payload">  The payload. </param>
        /// <returns>
        /// true if it is handled by at least one listener, false if no listeners process it.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Send(string message, object payload)
        {
            return Messenger.Send(this, message, payload);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Match identifier calls each registered listener, if the listener matches the identifier, then the listener
        /// returns true.
        /// </summary>
        /// <param name="identifier">   The identifier. </param>
        /// <returns>
        /// true if it matches the identifier.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool MatchIdentifier(string identifier)
        {
            return false;
        }

        #endregion
    }
}
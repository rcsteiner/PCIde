////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the view error list.xaml class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/19/2015   rcs     Initial Implementation
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
using System.Windows.Input;
using ZCore;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interaction logic for ViewErrorList.xaml.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class ViewOutput : ViewBase
    {
       // #region ScaleFactor
       // /// <summary>
       // /// The scale factor property
       // /// </summary>
       // public static readonly DependencyProperty ScaleFactorProperty = DependencyProperty.Register( "ScaleFactor", typeof( double ), typeof( ViewOutput ), new PropertyMetadata( 1.0 ) );


       // /// <summary>
       // /// Gets or sets the scale value.
       // /// </summary>
       // public double ScaleFactor
       // {
       //     get { return (double)GetValue( ScaleFactorProperty ); }
       //     set { SetValue( ScaleFactorProperty, value ); }
       // }

       //#endregion
        #region Is Focus Attached

        public static DependencyProperty IsFocusProperty = DependencyProperty.RegisterAttached("IsFocus", typeof (bool),
            typeof (ViewOutput), new UIPropertyMetadata(false, OnIsFocusedChanged));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the is focus.
        /// </summary>
        /// <param name="dependencyObject"> The dependency object. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool GetIsFocus(DependencyObject dependencyObject)
        {
            return (bool) dependencyObject.GetValue(IsFocusProperty);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the is focus.
        /// </summary>
        /// <param name="dependencyObject"> The dependency object. </param>
        /// <param name="value">            true to value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetIsFocus(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsFocusProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called when [is focused changed].
        /// </summary>
        /// <param name="dependencyObject">                     The dependency object. </param>
        /// <param name="dependencyPropertyChangedEventArgs">   The <see cref="DependencyPropertyChangedEventArgs"/> instance
        ///                                                     containing the event data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void OnIsFocusedChanged(DependencyObject dependencyObject,DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            TextBox textBox = dependencyObject as TextBox;
            bool newValue = (bool) dependencyPropertyChangedEventArgs.NewValue;
            bool oldValue = (bool) dependencyPropertyChangedEventArgs.OldValue;
            if (newValue && !oldValue && !textBox.IsFocused) textBox.Focus();
        }


        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewOutput()
        {
            InitializeComponent();
            Messenger.Register(this);

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Text input on lost focus.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TextInputOnLostFocus(object sender, RoutedEventArgs e)
        {
            // SIGNAL listener with message
            var t = (TextBox) sender;

            Send( "UserInput", t.Text);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// New message is received.  Check it to see if we are interested in it.
        ///// </summary>
        ///// <param name="sender">   The sender. </param>
        ///// <param name="key">      The key. </param>
        ///// <param name="payload">  The payload. </param>
        ///// <returns>
        ///// the message result, what to do.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public MessageResult NewMessage(IMessageListener sender, string key, object payload)
        //{
        //    return MessageResult.IGNORED;
        //}


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseWheel" /> attached event reaches an
        ///// element in its route that is derived from this class. Implement this method to add class handling for this event.
        ///// </summary>
        ///// <param name="args"> The <see cref="T:System.Windows.Input.MouseWheelEventArgs" /> that contains the event data. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //protected override void OnPreviewMouseWheel(MouseWheelEventArgs args)
        //{
        //    base.OnPreviewMouseWheel( args );
        //    if (Keyboard.IsKeyDown( Key.LeftCtrl ) || Keyboard.IsKeyDown( Key.RightCtrl ))
        //    {
        //        ScaleFactor += (args.Delta > 0) ? 0.1 : -0.1;
        //    }
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseDown" /> attached routed event
        ///// reaches an element in its route that is derived from this class. Implement this method to add class handling for
        ///// this event.
        ///// </summary>
        ///// <param name="args"> The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data.
        /////                     The event data reports that one or more mouse buttons were pressed. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //protected override void OnPreviewMouseDown(MouseButtonEventArgs args)
        //{
        //    base.OnPreviewMouseDown( args );
        //    if (Keyboard.IsKeyDown( Key.LeftCtrl ) || Keyboard.IsKeyDown( Key.RightCtrl ))
        //    {
        //        if (args.MiddleButton == MouseButtonState.Pressed)
        //        {
        //            ScaleFactor = 1.0;
        //        }
        //    }
        //}



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Sends a message to all listeners.  The listeners provide the filtering.
        ///// </summary>
        ///// <param name="message">  The message. </param>
        ///// <param name="payload">  The payload. </param>
        ///// <returns>
        ///// true if it is handled by at least one listener, false if no listeners process it.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public bool Send(string message, object payload)
        //{
        //    return Messenger.Send(this, message, payload);
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Match identifier calls each registered listener, if the listener matches the identifier, then the listener
        ///// returns true.
        ///// </summary>
        ///// <param name="identifier">   The identifier. </param>
        ///// <returns>
        ///// true if it matches the identifier.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public bool MatchIdentifier(string identifier)
        //{
        //    return false;
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by ScrollViewer for preview key down events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Key event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox s = e.Source as TextBox;
                if (s != null)
                {
                    s.MoveFocus( new TraversalRequest( FocusNavigationDirection.Next ) );
                }

                e.Handled = true;
            }
        }
    }
}

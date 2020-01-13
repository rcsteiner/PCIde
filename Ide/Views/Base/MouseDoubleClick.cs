////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the mouse double click class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/8/2015   rcs     Initial Implementation
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Mouse double click. 
    /// usage: on treeview item style.
    ///                 <Setter Property="views:MouseDoubleClick.Command"          Value="{Binding ItemActivateCommand}"/>
    ///                <Setter Property="views:MouseDoubleClick.CommandParameter" Value="{Binding}"/>
    ///  on treeview
    ///               views:MouseDoubleClick.Command="{Binding YourCommand}"
    ///              views:MouseDoubleClick.CommandParameter="{Binding }"
    ///
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class MouseDoubleClick
    {
        /// <summary>
        /// The command property.
        /// </summary>
        public static DependencyProperty CommandProperty =DependencyProperty.RegisterAttached("Command",typeof(ICommand),typeof(MouseDoubleClick),new UIPropertyMetadata(CommandChanged));

        /// <summary>
        /// The command parameter property.
        /// </summary>
        public static DependencyProperty CommandParameterProperty =DependencyProperty.RegisterAttached("CommandParameter",typeof(object),typeof(MouseDoubleClick),new UIPropertyMetadata(null));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets a command.
        /// </summary>
        /// <param name="target">   Target for the. </param>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets a command parameter.
        /// </summary>
        /// <param name="target">   Target for the. </param>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a command parameter.
        /// </summary>
        /// <param name="target">   Target for the. </param>
        /// <returns>
        /// The command parameter.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Command changed.
        /// </summary>
        /// <param name="target">   Target for the. </param>
        /// <param name="e">        Dependency property changed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Control control = target as Control;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.MouseDoubleClick += OnMouseDoubleClick;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.MouseDoubleClick -= OnMouseDoubleClick;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the mouse double click action.  Filters items in tree view to be treeviewitem only.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {

            Control control         = sender as Control;
            ICommand command        = (ICommand)control.GetValue( CommandProperty );
            object commandParameter = control.GetValue( CommandParameterProperty );

            var item = sender as TreeViewItem;
            if (item != null && !item.IsSelected)
            {
                return;
            }

            if (command.CanExecute( commandParameter ))
            {
                command.Execute( commandParameter );
            }
        }
    }
}
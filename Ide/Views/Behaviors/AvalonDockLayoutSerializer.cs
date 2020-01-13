////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the avalon dock layout serializer class
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

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Views.Behavior
{
    //TODO: simplify this load/save layout to just be commands on the ViewModel
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class implements an attached behavior to load/save a layout for AvalonDock manager. This layout defines the
    /// position and shape of each document and tool window displayed in the application. Load/Save is triggered through
    /// command binding On application start (AvalonDock.Load event results in LoadLayoutCommand) and application
    /// shutdown (AvalonDock.Unload event results in SaveLayoutCommand). This implementation of layout save/load is MVVM
    /// compliant, robust, and simple to use. Just add the following code into your XAML: xmlns:AVBehav="clr-
    /// namespaceGrammarWorkshop.Behavior"
    /// ...
    ///  avalonDock:DockingManager AnchorablesSource="{Binding Tools}"
    ///                           DocumentsSource="{Binding Files}"
    ///                           ActiveContent="{Binding VMActiveDocument, Mode=TwoWay, Converter={StaticResource ActiveDocumentConverter}}"
    ///                           Grid.Row="3"
    ///                           SnapsToDevicePixels="True"
    ///                AVBehav:AvalonDockLayoutSerializer.LoadLayoutCommand="{Binding LoadLayoutCommand}"
    ///                AVBehav:AvalonDockLayoutSerializer.SaveLayoutCommand="{Binding SaveLayoutCommand}"
    ///
    /// The LoadLayoutCommand passes a reference of the AvalonDock Manager instance to load the XML layout.
    /// The SaveLayoutCommand passes a string of the XML Layout which can be persisted by the viewmodel/model.
    ///
    /// Both command bindings work with RoutedCommands or delegate commands (RelayCommand).
    ///
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static class AvalonDockLayoutSerializer
    {
        #region fields

        /// <summary>
        /// Backing store for LoadLayoutCommand dependency property.
        /// </summary>
        private static readonly DependencyProperty LoadLayoutCommandProperty =DependencyProperty.RegisterAttached("LoadLayoutCommand",typeof (ICommand),typeof (AvalonDockLayoutSerializer),new PropertyMetadata(null, OnLoadLayoutCommandChanged));

        /// <summary>
        /// Backing store for SaveLayoutCommand dependency property.
        /// </summary>
        private static readonly DependencyProperty SaveLayoutCommandProperty =DependencyProperty.RegisterAttached("SaveLayoutCommand",typeof (ICommand),typeof (AvalonDockLayoutSerializer),new PropertyMetadata(null, OnSaveLayoutCommandChanged));

        #endregion fields

        #region methods

        #region Load Layout

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Standard get method of <seealso cref="LoadLayoutCommandProperty" /> dependency property.
        /// </summary>
        /// <param name="obj">  . </param>
        /// <returns>
        /// The load layout command.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static ICommand GetLoadLayoutCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(LoadLayoutCommandProperty);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Standard set method of <seealso cref="LoadLayoutCommandProperty" /> dependency property.
        /// </summary>
        /// <param name="obj">      . </param>
        /// <param name="value">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetLoadLayoutCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadLayoutCommandProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This method is executed if a <seealso cref="LoadLayoutCommandProperty" /> dependency property is about to change
        /// its value (eg: The framewark assigns bindings).
        /// </summary>
        /// <param name="d">    . </param>
        /// <param name="e">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void OnLoadLayoutCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var framworkElement = d as FrameworkElement; // Remove the handler if it exist to avoid memory leaks
            if (framworkElement != null)
            {
                framworkElement.Loaded -= OnFrameworkElement_Loaded;

                var command = e.NewValue as ICommand;
                if (command != null)
                {
                    // the property is attached so we attach the Drop event handler
                    framworkElement.Loaded += OnFrameworkElement_Loaded;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This method is executed when a AvalonDock <seealso cref="DockingManager" /> instance fires the Load standard
        /// (FrameworkElement) event.
        /// </summary>
        /// <param name="sender">   . </param>
        /// <param name="e">        . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void OnFrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var frameworkElement = sender as FrameworkElement;

                // Sanity check just in case this was somehow send by something else
                if (frameworkElement == null)
                {
                    return;
                }

                var loadLayoutCommand = GetLoadLayoutCommand(frameworkElement);

                // There may not be a command bound to this after all
                if (loadLayoutCommand == null)
                {
                    return;
                }

                // Check whether this attached behaviour is bound to a RoutedCommand
                if (loadLayoutCommand is RoutedCommand)
                {
                    // Execute the routed command
                    (loadLayoutCommand as RoutedCommand).Execute(frameworkElement, frameworkElement);
                }
                else
                {
                    // Execute the Command as bound delegate
                    // Todo: this is the load command 
                    loadLayoutCommand.Execute(frameworkElement);
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion Load Layout

        #region Save Layout

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Standard get method of <seealso cref="SaveLayoutCommandProperty" /> dependency property.
        /// </summary>
        /// <param name="obj">  . </param>
        /// <returns>
        /// The save layout command.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static ICommand GetSaveLayoutCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(SaveLayoutCommandProperty);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Standard get method of <seealso cref="SaveLayoutCommandProperty" /> dependency property.
        /// </summary>
        /// <param name="obj">      . </param>
        /// <param name="value">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetSaveLayoutCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(SaveLayoutCommandProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This method is executed if a <seealso cref="SaveLayoutCommandProperty" /> dependency property is about to change
        /// its value (eg: The framewark assigns bindings).
        /// </summary>
        /// <param name="d">    . </param>
        /// <param name="e">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void OnSaveLayoutCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var framworkElement = d as FrameworkElement; // Remove the handler if it exist to avoid memory leaks
            if (framworkElement != null)
            {
                framworkElement.Unloaded -= OnFrameworkElement_Saved;

                var command = e.NewValue as ICommand;
                if (command != null)
                {
                    // the property is attached so we attach the Drop event handler
                    framworkElement.Unloaded += OnFrameworkElement_Saved;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This method is executed when a AvalonDock <seealso cref="DockingManager" /> instance fires the Unload standard
        /// (FrameworkElement) event.
        /// </summary>
        /// <param name="sender">   . </param>
        /// <param name="e">        . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void OnFrameworkElement_Saved(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as DockingManager;

            // Sanity check just in case this was somehow send by something else
            if (frameworkElement == null)
            {
                return;
            }

            var saveLayoutCommand = GetSaveLayoutCommand(frameworkElement);

            // There may not be a command bound to this after all
            if (saveLayoutCommand == null)
            {
                return;
            }

            string xmlLayoutString;

            using (var fs = new StringWriter())
            {
                var xmlLayout = new XmlLayoutSerializer(frameworkElement);

                xmlLayout.Serialize(fs);

                xmlLayoutString = fs.ToString();
            }

            // Check whether this attached behaviour is bound to a RoutedCommand
            if (saveLayoutCommand is RoutedCommand)
            {
                // Execute the routed command
                (saveLayoutCommand as RoutedCommand).Execute(xmlLayoutString, frameworkElement);
            }
            else
            {
                // Execute the Command as bound delegate
                saveLayoutCommand.Execute(xmlLayoutString);
            }
        }

        #endregion Save Layout

        #endregion methods
    }
}
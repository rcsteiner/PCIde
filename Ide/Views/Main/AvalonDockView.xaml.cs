////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the avalon dock view.xaml class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/20/2015   rcs     Initial Implementation
//  =====================================================================================================
//
// Copyright:
//  Copyright Advanced Performance
//  All Rights Reserved THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Advanced Performance
//  The copyright notice above does not evidence any actual or intended publication of such source code.
//  Some third-party source code components may have been modified from their original versions
//  by Advanced Performance. The modifications are Copyright Advanced Performance., All Rights Reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Interaction logic for AvalonDockView.xaml.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [TemplatePart(Name = "PART_DockView", Type = typeof (DockingManager))]
    public class AvalonDockView : UserControl
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyDown" /> attached event reaches an
        ///     element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">    The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        #region fields


        /// <summary>
        ///     Manager for dock.
        /// </summary>
        private DockingManager mDockManager;

        /// <summary>
        ///     The layout item template selector.
        /// </summary>
        private DataTemplateSelector mLayoutItemTemplateSelector;

        /// <summary>
        ///     The document header template.
        /// </summary>
        private DataTemplate mDocumentHeaderTemplate;

        /// <summary>
        ///     The layout item container style selector.
        /// </summary>
        private StyleSelector mLayoutItemContainerStyleSelector;

        /// <summary>
        ///     The layout update strategy.
        /// </summary>
        private ILayoutUpdateStrategy mLayoutUpdateStrategy;

        /// <summary>
        ///     The on load xml layout.
        /// </summary>
        private string mOnLoadXmlLayout;

        #endregion fields

        #region constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Static constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static AvalonDockView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (AvalonDockView),new FrameworkPropertyMetadata(typeof (AvalonDockView)));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Class Constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public AvalonDockView()
        {
            LayoutID = Guid.NewGuid();
        }

        #endregion constructor

        #region properties

        /// <summary>
        ///     Gets or sets/Sets the LayoutId of the AvalonDocking Manager layout used to manage the positions and layout of
        ///     documents and tool windows within the AvalonDock view.
        /// </summary>
        public Guid LayoutID { get; private set; }

        /// <summary>
        ///     Gets the current AvalonDockManager Xml layout and returns it as a string.
        /// </summary>
        public string CurrentADLayout
        {
            get
            {
                if (mDockManager == null)
                {
                    return string.Empty;
                }

                var xmlLayoutString = string.Empty;
                try
                {
                    using (var fs = new StringWriter())
                    {
                        var xmlLayout = new XmlLayoutSerializer(mDockManager);

                        xmlLayout.Serialize(fs);

                        xmlLayoutString = fs.ToString();
                    }
                }
                catch
                {
                }

                return xmlLayoutString;
            }
        }

        #endregion properties

        #region methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Standard method is executed when control template is applied to lookless control.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mDockManager = Template.FindName("PART_DockView", this) as DockingManager;

            SetCustomLayoutItems();
            ////this.LoadXmlLayout(this.mOnLoadXmlLayout);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Class Constructor.
        /// </summary>
        /// <param name="paneSel">                  . </param>
        /// <param name="documentHeaderTemplate">   . </param>
        /// <param name="panesStyleSelector">       . </param>
        /// <param name="layoutInitializer">        . </param>
        /// <param name="layoutID">                 . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetTemplates(DataTemplateSelector paneSel,
            DataTemplate documentHeaderTemplate,
            StyleSelector panesStyleSelector,
            ILayoutUpdateStrategy layoutInitializer,
            Guid layoutID
            )
        {
            mLayoutItemTemplateSelector       = paneSel;
            mDocumentHeaderTemplate           = documentHeaderTemplate;
            mLayoutItemContainerStyleSelector = panesStyleSelector;
            mLayoutUpdateStrategy             = layoutInitializer;
            LayoutID                          = layoutID;

            if (mDockManager == null)
            {
                return;
            }

            SetCustomLayoutItems();
        }

        #region Workspace Layout Management

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Is executed when PRISM sends an Xml layout string notification via a sender which could be a viewmodel that wants
        ///     to receive the load the <seealso cref="LoadLayoutEvent" />.
        ///     Save layout is triggered by the containing window onClosed event.
        /// </summary>
        /// <param name="args"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OnLoadLayout(LoadLayoutEventArgs args)
        {
            if (args == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(args.XmlLayout))
            {
                return;
            }

            mOnLoadXmlLayout = args.XmlLayout;

            if (mDockManager == null)
            {
                return;
            }

            LoadXmlLayout(mOnLoadXmlLayout);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Loads an xml layout.
        /// </summary>
        /// <param name="xmlLayout">    The xml layout. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void LoadXmlLayout(string xmlLayout)
        {
            try
            {
                var sr = new StringReader(xmlLayout);

                XmlLayoutSerializer layoutSerializer = null;

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        layoutSerializer = new XmlLayoutSerializer(mDockManager);
                        layoutSerializer.LayoutSerializationCallback += UpdateLayout;
                        layoutSerializer.Deserialize(sr);
                    }
                    catch (Exception exp)
                    {
                        Debug.Print("Error Loading Layout: {0}\n\n{1}", exp.Message, xmlLayout);
                    }
                }), DispatcherPriority.Background);
            }
            catch (Exception exp)
            {
                Debug.Print("Error Loading Layout: {0}\n\n{1}", exp.Message, xmlLayout);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Convert a Avalondock ContentId into a viewmodel instance that represents a document or tool window. The re-load
        ///     of this component is cancelled if the Id cannot be resolved.
        ///     The result is (viewmodel Id or Cancel) is returned in <paramref name="args" />.
        /// </summary>
        /// <param name="sender">   . </param>
        /// <param name="args">     . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateLayout(object sender, LayoutSerializationCallbackEventArgs args)
        {
            try
            {
                Edi.Core.Interfaces.IViewModelResolver resolver = null;

                resolver = DataContext as Edi.Core.Interfaces.IViewModelResolver;

                if (resolver == null)
                {
                    return;
                }

                // Get a matching viewmodel for a view through DataContext of this view
                var content_view_model = resolver.ContentViewModelFromID(args.Model.ContentId);

                if (content_view_model == null)
                {
                    args.Cancel = true;
                }

                // found a match - return it
                args.Content = content_view_model;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }

        #endregion Workspace Layout Management

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Assigns the currently assigned custom layout controls to the AvalonDock DockingManager.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetCustomLayoutItems()
        {
            if (mDockManager == null)
            {
                return;
            }

            if (mLayoutItemTemplateSelector != null)
            {
                mDockManager.LayoutItemTemplateSelector = mLayoutItemTemplateSelector;
            }

            if (mDocumentHeaderTemplate != null)
            {
                mDockManager.DocumentHeaderTemplate = mDocumentHeaderTemplate;
            }

            if (mLayoutItemContainerStyleSelector != null)
            {
                mDockManager.LayoutItemContainerStyleSelector = mLayoutItemContainerStyleSelector;
            }

            if (mLayoutUpdateStrategy != null)
            {
                mDockManager.LayoutUpdateStrategy = mLayoutUpdateStrategy;
            }
        }

        #endregion methods
    }
}
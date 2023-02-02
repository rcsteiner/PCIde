////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   Main window
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/18/2018  RCS       Initial code.
//  
// 
//=====================================================[ Copyright ]=====================================================
// 
//  Copyright Advanced Performance
//  All Rights Reserved THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Advanced Performance
//  The copyright notice above does not evidence any actual or intended publication of such source code.
//  Some third-party source code components may have been modified from their original versions
//  by Advanced Performance. The modifications are Copyright Advanced Performance., All Rights Reserved.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.ComponentModel;
using System.Windows;
using Lift.Properties;
using Lift.View;

namespace Lift
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Interaction logic for MainWindow.xaml,  contains the menu, canvas and basic background.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class MainWindow : Window
    {

        /// <summary>
        ///  Get/Set Options.
        /// </summary>
        public Options Options { get; set; }

        /// <summary>
        ///  The  world Controller.
        /// </summary>
        private WorldController _worldController;

        /// <summary>
        ///  The  stats View.
        /// </summary>
        private StatisticalPanel _statsView;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor:
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public MainWindow()
        {
            InitializeComponent();

            var s = Settings.Default;
            Options = new Options(s.FloorCount, s.ElevatorCount,s.MaxPassangers,s.SpawnRate, s.MaxTime);

            Top    = s.Top;
            Left   = s.Left;
            Height = s.Height;
            Width  = s.Width;
            // Very quick and dirty - but it does the job
            if (s.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
            _statsView = new StatisticalPanel();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Initialize the World, by creating the world controller.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void InitializeWorld()
        {
            _worldController = new WorldController(Options, MyCanvas, _statsView);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Run Click, start the simulation.  callback hooked to run button.
        ///   Places statView on screen
        ///   Initialize the world
        ///   Initialize the world controller
        ///   starts simulation
        /// </summary>
        /// <param name="sender"> The event sender. (not used)</param>
        /// <param name="e">      The event arguments (not used).</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RunClick(object sender, RoutedEventArgs e)
        {
            _statsView.Owner = this;
            _statsView.Top   = Settings.Default.StatsTop;
            _statsView.Left  = Settings.Default.StatsLeft;

            InitializeWorld();

            // Start the animation
            _worldController.Init();
            _worldController.Start();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Options click, shows the options dialog, and saves it if OK is pressed.
        /// </summary>
        /// <param name="sender"> The event sender. (not used)</param>
        /// <param name="e">      The event arguments (not used).</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OptionsClick(object sender, RoutedEventArgs e)
        {
            // change the options
            // show options page
            var opt = new OptionsDialog(Options);
            if ((bool) opt.ShowDialog())
            {
                // handle it
                Settings.Default.FloorCount    = Options.FloorCount;
                Settings.Default.ElevatorCount = Options.ElevatorCount;
                Settings.Default.SpawnRate     = Options.SpawnRate;
                Settings.Default.MaxPassangers = Options.MaxPassangers;
                Settings.Default.MaxTime       = Options.MaxTime;
                Settings.Default.Save();

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Pause button Clicked, toggle the pause state of the simulator.
        /// </summary>
        /// <param name="sender"> The event sender. (not used)</param>
        /// <param name="e">      The event arguments (not used).</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void PauseClick(object sender, RoutedEventArgs e)
        {
            _worldController.SetPaused(cmdPause.IsChecked);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Main window is closing so clean up/ save the state of any properties that should persist.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e">      The e.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void xMain_Closing(object sender, CancelEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Settings.Default.Top       = RestoreBounds.Top;
                Settings.Default.Left      = RestoreBounds.Left;
                Settings.Default.Height    = RestoreBounds.Height;
                Settings.Default.Width     = RestoreBounds.Width;
                Settings.Default.Maximized = true;
            }
            else
            {
                Settings.Default.Top       = Top;
                Settings.Default.Left      = Left;
                Settings.Default.Height    = Height;
                Settings.Default.Width     = Width;
                Settings.Default.Maximized = false;
            }

           Settings.Default.StatsTop = _statsView.Top;
           Settings.Default.StatsLeft= _statsView.Left;

            Settings.Default.Save();
        }
    }
}


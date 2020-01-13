////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   Highest level building view
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/25/2018  RCS       Initial code.
//  
// 
//=====================================================[ Copyright ]=====================================================
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
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ide.Properties;
using Lift;
using Lift.View;
using LiftLib.View;


namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Interaction logic for Building, this owns the canvas and the stats view.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class BuildingControl : UserControl, IBuilding
    {
        /// <summary>
        ///  The  world Controller.
        /// </summary>
        public WorldController Controller { get;  set; }

        /// <summary>
        ///  Get Decider.
        /// </summary>
        public DecideNextFloorHandler Decider { get; set; }
        /// <summary>
        ///  Get/Set Options.
        /// </summary>
        public Options Options { get; set; }

        /// <summary>
        ///  Get World.
        /// </summary>
        public World World { get { return Controller._world; } }

        /// <summary>
        ///  Get/Set Stats.
        /// </summary>
        public IStats Stats {get { return Controller?.Stats;} set {Controller.Stats=value;} }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor:
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public BuildingControl()
        {
            InitializeComponent();

            //_statsView = new StatisticalPanel();
            //Options    = new Options(4, 1, 4, .5, 100);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Initialize to the User code
        /// </summary>
        /// <param name="floorCount">    The floor Count.</param>
        /// <param name="elevatorCount"> The elevator Count.</param>
        /// <param name="maxPassengers"> The maximum Passengers.</param>
        /// <param name="spawnRate">     The spawn Rate.</param>
        /// <param name="maxTime">       The maximum Time.</param>
        /// <param name="stats"></param>
        /// <param name="decider">       [optional=null] The decider.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public WorldController Init(int floorCount, int elevatorCount, int maxPassengers, double spawnRate,double maxTime, IStats stats, DecideNextFloorHandler decider = null)
        {
            Options = new Options(floorCount, elevatorCount, maxPassengers, spawnRate, maxTime);
            Decider = decider;
            InitializeComponent();
            return Controller = new WorldController(Options, MyCanvas, stats);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Initialize the World, by creating the world controller.
        /// </summary>
        /// <param name="showToolbar"> [optional=true] True if show Toolbar.</param>
        /// <returns>
        ///  The world controller.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public WorldController InitializeWorld(bool showToolbar = true)
        {
         //   ToolbarMenu.Visibility = showToolbar ? Visibility.Visible : Visibility.Hidden;
            return Controller;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Show
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Show()
        {
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
            if ((bool)opt.ShowDialog())
            {
                //// handle it
                //Settings.Default.FloorCount = Options.FloorCount;
                //Settings.Default.ElevatorCount = Options.ElevatorCount;
                //Settings.Default.SpawnRate = Options.SpawnRate;
                //Settings.Default.MaxPassengers = Options.MaxPassengers;
                //Settings.Default.MaxTime = Options.MaxTime;
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
      //      Controller.SetPaused(cmdPause.IsChecked);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Run Click, start the simulation.  callback hooked to run button.
        ///  Places statView on screen
        ///  Initialize the world
        ///  Initialize the world controller
        ///  starts simulation
        /// </summary>
        /// <param name="sender"> The event sender. (not used)</param>
        /// <param name="e">      The event arguments (not used).</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RunClick(object sender, RoutedEventArgs e)
        {
            Start();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Start the simulation.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Start()
        {
           // _statsView.Owner ;
            //_statsView.Top = 20;

            //_statsView.Left = MyCanvas.ActualWidth - _statsView.Width;
            //_statsView.Show();

            InitializeWorld();

            // Start the animation
            Controller.Init(Decider);
            Controller.Start();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Shutdown
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Shutdown()
        {
            Controller.Stop();
        }
    }
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   Core controller manages the interface from main to world
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/8/2018   RCS       Initial code.
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lift
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  The World Controller Class definition.  Povides high level interface to control the simulation.
    ///  It stores the canvas, viewStats, challenge flags, and options.  and starts/stops the simulation.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class WorldController
    {

        /// <summary>
        ///  The Wait For coompletion flag.
        /// </summary>
        public AutoResetEvent WaitForCompletion = new AutoResetEvent(false);

        /// <summary>
        ///  The World Controller Class definition.
        /// </summary>
        public Canvas _myCanvas;

        /// <summary>
        ///  The  stats View.
        /// </summary>
        public IStats Stats { get { return _world?.Stats; } set {if (_world != null){_world.Stats = value;}} }

        /// <summary>
        ///  The challenge Ended flag.
        /// </summary>
        public bool ChallengeEnded { get { return _world.ChallengeEnded; } set { _world.ChallengeEnded = value; } }

        ///  Get/Set Is Paused flag
        /// </summary>
        public bool IsPaused { get { return _world.IsPaused; } set { _world.IsPaused = value; } }

        /// <summary>
        ///  Get/Set Time Scale.
        /// </summary>
        public double TimeScale { get { return _world.TimeScale; } set { _world.TimeScale = value; } }

        /// <summary>
        ///  The first Update.
        /// </summary>
        private bool _firstUpdate;

        /// <summary>
        ///  The last time the animator ran.
        /// </summary>
        private double _lastTime = -1;

        /// <summary>
        ///  The  options.
        /// </summary>
        public Options Options { get; set; }

        /// <summary>
        ///  The world.
        /// </summary>
        public World _world;

        /// <summary>
        ///  flag inicating we are trying to close (unwind).
        /// </summary>
        private bool _closing;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor:  Set the options, canvas and statistics viewer to passenger.
        /// </summary>
        /// <param name="options">   The options.</param>
        /// <param name="myCanvas">  The Canvas to use.</param>
        /// <param name="stats"> The statistics to use.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public WorldController(Options options, Canvas myCanvas, IStats stats)
        {
            Options    = options;
            _myCanvas  = myCanvas;
            _world = new World(this, Options, _myCanvas, stats, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor:
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public WorldController()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: initialize the world (create a new one) and set challenge ended to false.
        /// </summary>
        /// <param name="decider"> [optional=null] The decider.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Init(DecideNextFloorHandler decider = null)
        {
            _world.Decider      = decider;
            ChallengeEnded      = false;
            _closing            = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: On the Render, draws a frame.  Callback used by the WPF animation system to indicate: draw a frame
        ///  Gets the time from the event.
        ///  checks for end of challenge.
        /// and then updates the state of all the elements using the current time passed from animation system.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e">      The event args of type RenderingEventArgs (must be cast).</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnRender(object sender, EventArgs e)
        {
            var r =(( RenderingEventArgs)e).RenderingTime.TotalSeconds;
            if (ChallengeEnded )
            {
                if (!_closing)
                {
                    _world.UnWind();
                    _closing = true;
                }
                if (_world.IsDone())
                {
                    CompositionTarget.Rendering -= OnRender;
                }

                WaitForCompletion.Set();
            }
            Update(r);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: set the Paused flag (or unset it), pausing the animation system, freezing time.
        /// </summary>
        /// <param name="paused"> True if paused.  False to unpause.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetPaused(bool paused)
        {
            IsPaused = paused;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: set the Time Scale (adjust speed), 1.0 is run normal speed,  2.0 would be run at twice speed.
        /// </summary>
        /// <param name="timeScale"> The time Scale multiplier to use.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetTimeScale(double timeScale)
        {
            TimeScale = timeScale;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: start Running the simulation, clears initial flags, hooks the animation system and canvas size change.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Start()
        {
            IsPaused      = false;
            _lastTime     = -1;
            _firstUpdate  = true;

            CompositionTarget.Rendering += OnRender;
            _myCanvas.SizeChanged       += MyCanvasOnSizeChanged;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: My the Canvas On Size Changed, force a redraw and a restart of system.
        /// </summary>
        /// <param name="sender">               The sender.</param>
        /// <param name="sizeChangedEventArgs"> The size Changed Event arguments.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MyCanvasOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            _world.CreateBuilding();

            //_statsView.Top = 20;
            //_statsView.Left = _myCanvas.ActualWidth - _statsView.Width;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: updater, updates the world, that has all the components to be displayed.
        /// </summary>
        /// <param name="currentTime"> The time Value.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Update(double currentTime)
        {

            if (!IsPaused && !ChallengeEnded)
            {
                if (_firstUpdate)
                {
                    _firstUpdate = false;
                    _lastTime    = currentTime;
                   _world.CreateBuilding();
                   _world.Init();
                }

                // check if time / challenge expired

                if (_world.ElapsedTime >= Options.MaxTime)
                {
                    ChallengeEnded = true;
                }
            }
            var deltaTime        = currentTime - _lastTime;
             var scaledDeltaTime = deltaTime   * TimeScale;
            _world.RenderFrame(scaledDeltaTime);
            _world.UpdateDisplayPositions();

            _lastTime = currentTime;
        }

        public void Stop()
        {
            ChallengeEnded = true;
        }
    }
}

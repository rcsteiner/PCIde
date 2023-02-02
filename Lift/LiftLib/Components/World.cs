////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   The world (simulation environment)
//                 This program is inspired by https://play.elevatorsaga.com/ and the basic structure
//                 without the events is used.
// 
// 
//  Author:        Robert C Steiner
// 
//====================================[ History ]====================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/8/2018   RCS       Initial code.
//  
// 
//====================================[ Copyright ]====================================
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
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Lift.View;

namespace Lift
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  The World Class definition.  This class contains all the components that make up the world.
    ///   Major arrays Includes:
    ///      Elevators[]
    ///      Floors[]
    ///      Users[]
    ///      ButtonStats[]
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class World
    {
        /// <summary>
        ///  Get/Set Up Buttons that are on each floor (0 based)
        /// </summary>
        public bool[] floorButtonPressedUp { get; set; }

        /// <summary>
        ///  Get/Set down Buttons that are on each floor (0 based)
        /// </summary>
        public bool[] floorButtonPressedDown { get; set; }


        /// <summary>
        ///  Get/Set Statistics used to measure performance.
        /// </summary>
        public IStats Stats { get; set; }

        /// <summary>
        ///  Flag indicating the challenge Ended. (times up)
        /// </summary>
        public bool ChallengeEnded { get; set; }

        /// <summary>
        ///  The elapsed Time, the simulator has been running, (accesses Stats)
        /// </summary>
        public double ElapsedTime
        {
            get { return Stats != null ? Stats.ElapsedTime : 0; }
            set {if (Stats != null) {Stats.ElapsedTime = value;}}
        }

        /// <summary>
        ///  The elevators array, stores all active elevators.
        /// </summary>
        public Elevator[] Elevators { get; set; }

        /// <summary>
        ///  The floors array, stores the floors used.
        /// </summary>
        public Floor[] Floors { get; set; }

        
        /// <summary>
        ///  The is Paused flag indicating to momentary stop in running. Can be toggled.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        ///  The Canvas used for all animation drawing.  Shared with components to draw themselves.
        /// </summary>
        public Canvas MyCanvas { get; set; }

        /// <summary>
        ///  Get Decider for this elevator system.
        /// </summary>
        public DecideNextFloorHandler Decider { get; set; }

        /// <summary>
        ///  The time Scale multiplier, default is 1, 
        /// todo consider moving to options.
        /// </summary>
        public double TimeScale { get; set; }

        /// <summary>
        ///  Get/Set the options structure to use (saved on exit)
        /// </summary>
        private Options Options { get; set; }

        /// <summary>
        ///  The passengers array (stored in list to make it easier to add/remove)
        /// </summary>
        public List<Passenger> Passengers { get; set; }

        /// <summary>
        ///  Get/Set Passenger height multiplier computed based on the scaling of the floor height.
        /// </summary>
        public double UserScale { get; set; }

        /// <summary>
        ///  The transported Counter wrapper.  Accesses Stats
        /// </summary>
        public int TransportedCounter
        {
            get { return Stats != null ? Stats.Transported : 0; }
            set { if (Stats != null) {Stats.Transported = value; }}

        }

        /// <summary>
        ///  The  black brush, cached and frozen for performance.
        /// </summary>
        private SolidColorBrush _black;

        /// <summary>
        ///  The world controller, used to manage the world and provide access control from outside.
        /// </summary>
        private readonly WorldController _controller;

        /// <summary>
        ///  The elapsed time since last Spawn.  Used to decide how many passengers to add on each animation frame.
        /// </summary>
        private double _elapsedSinceSpawn;

        /// <summary>
        ///  The floor Count - number of floors (0 based)
        /// </summary>
        private int _floorCount;

        /// <summary>
        ///  The  gradient Blue brush, cached and frozen for performance. Used for floor background.
        /// </summary>
        private readonly LinearGradientBrush _gradientBlue;

        /// <summary>
        ///  The  gradient Gray brush, cached and frozen for performance.  Used for doors and set semi-transparent.
        /// </summary>
        private readonly LinearGradientBrush _gradientGray;

        /// <summary>
        ///  The  random number generator used for animation.
        /// </summary>
        private static Random _random = new Random();

        /// <summary>
        ///  The  last Update for stats time, used to minimize the number of updates of slower changing stats values.
        /// </summary>
        private double _lastUpdate;

        /// <summary>
        ///  The Passenger HEIGHT (set by Icon size being used)
        ///  TODO this needs a better way to handle this
        /// </summary>
        private const int Passenger_HEIGHT = 64;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor:  Initialize the World with its controller, options and canvas to draw on, along with the stats
        ///  view
        ///  to update during the run.
        ///  Brushes are created,
        ///  parameters stored, but no action taken yet.
        /// </summary>
        /// <param name="controller"> The controller.</param>
        /// <param name="options">    The options.</param>
        /// <param name="myCanvas">   The my Canvas.</param>
        /// <param name="statsView">  The stats View.</param>
        /// <param name="decider">    [optional=null] The decider.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public World(WorldController controller, Options options, Canvas myCanvas, IStats statsView,DecideNextFloorHandler decider = null)
        {
            MyCanvas           = myCanvas;
            Decider            = decider;
            Options            = options;
            TimeScale          = 1;
            Passengers         = new List<Passenger>();
            _controller        = controller;
            Stats              = statsView;

            _gradientBlue = new LinearGradientBrush(Colors.CornflowerBlue, Colors.Blue, 0.0);
            _gradientGray = new LinearGradientBrush(Color.FromArgb(118, 240, 248, 255), Color.FromArgb(126, 112, 128, 144), 0.0);

            _gradientGray.Freeze();
            _gradientBlue.Freeze();
            _black = Brushes.Black;
            _black.Freeze();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Check the Destination Queues for each elevator.  Used as a simple mechanism to goto the head of the
        ///  destination queues.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CheckDestinationQueues()
        {
            // Checking the floor queue of the elevators triggers the idle event here
            for (var i = 0; i < Elevators.Length; ++i)
            {
                Elevators[i].CheckDestinationQueue();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Create the Building, Compute the size of all the elements, then draw the elements on the screen
        ///  Floors first
        ///  Floor buttons and indicators
        ///  Elevators and doors
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CreateBuilding()
        {
            MyCanvas.Children.Clear();
            Stats?.Clear();
            
            var w                = MyCanvas.ActualWidth;
            var h                = MyCanvas.ActualHeight;

            var maxUsers         = Options.MaxPassengers;

            // compute the height of the floor + thickness

            _floorCount          = Options.FloorCount;
            var floorHeightTotal = h / _floorCount;
            var floorThick       =  floorHeightTotal/8;
            var floorHeight      = floorHeightTotal - floorThick;
            var floorThickAdjust = floorThick / 2.0;

            UserScale            = floorHeight / Passenger_HEIGHT * .4;
            var statusHeight     = floorHeight * .15;

            var elevatorCount    = Options.ElevatorCount;
            var elevatorWidth    = floorHeight;
            var elevatorSpacing  = elevatorWidth * 1.5;
            var elevatorHieght   = floorHeight - statusHeight;
            var elevatorXStart   = elevatorSpacing * .5;



            var doorWidth        = elevatorWidth * .5;
            var doorHeight       = elevatorHieght * .95;

            var xRightElevators  = 1.5 * ((elevatorCount - 1) * elevatorWidth) + elevatorXStart + elevatorWidth;
            var xSpawn           = xRightElevators + (w - xRightElevators) * .1;

            var buttonLeft       = xRightElevators + w * .02;
            var buttonBottom     = elevatorHieght * .4;

            floorButtonPressedDown  = new bool[_floorCount];
            floorButtonPressedUp    = new bool[_floorCount];


            // create the floors now

            var floors           = new Floor[_floorCount];
            Floors               = floors;

            var y                = 0.0;

            for (var floorNum = 0; floorNum < floors.Length; floorNum++)
            {
                var floorYPos           = floorNum * floorHeightTotal + floorThickAdjust;
                var floor               = new Floor(this, w, floorNum, xSpawn, floorYPos);

                //    var elapsedSinceSpawn   = 1.001 / Options.SpawnRate;

                CreateRectangle(y, floorHeightTotal, 0, w, _gradientBlue, floorThickAdjust);

                var txt                 = new Label();
                txt.Content             = (floorNum + 1).ToString();
                txt.FontFamily          = new FontFamily("Arial Bold");
                txt.FontSize            = 15;
                txt.Foreground          = Brushes.White;

                MyCanvas.Children.Add(txt);
                Canvas.SetBottom(txt, y + floorHeight / 2.0);
                Canvas.SetLeft(txt, 20);


                // but buttons on right side of all elevators on each floor
                var buttons             = new Indicator();
                buttons.LayoutTransform = new ScaleTransform(elevatorWidth * .05 / buttons.Width, statusHeight / buttons.Height);
                MyCanvas.Children.Add(buttons);
                Canvas.SetBottom(buttons, buttonBottom + y);
                Canvas.SetLeft(buttons, buttonLeft);
                floor.DisplayButtons    = buttons;
                floors[floorNum]        = floor;
                y += floorHeightTotal;
            }


            // create the elevators, Position them off the bottom so they move into place

            var elevators = new Elevator[elevatorCount];
            Elevators     = elevators;
            var x         = elevatorXStart;
            double scaleX;

            for (int e    = 0; e < elevators.Length; ++e)
            {
                var elevator             = new Elevator(MyCanvas, 2.6, _floorCount, floorHeightTotal, floorThick, maxUsers, this, Decider);
                elevator.Doors           = new Doors[_floorCount];
                elevator.XOff            = x;
                elevator.YOff            = -floorHeightTotal  - 5;
                scaleX                   = elevatorWidth / elevator.Width;
                elevator.LayoutTransform = new ScaleTransform(scaleX, elevatorHieght / elevator.Height);

                for (int f = 0; f < _floorCount; ++f)
                {
                    elevator.Doors[f] = new Doors(MyCanvas, elevator, floors[f], doorWidth, floorHeight);
                }
                elevator.UpdateDisplayPosition();
                elevator.GoToFloor(0);

                MyCanvas.Children.Add(elevator);
                Canvas.SetBottom(elevator, elevator.YOff);
                Canvas.SetLeft(elevator, x);

                y = floorThickAdjust;

                for (int floorNum = 0; floorNum < Floors.Length; ++floorNum)
                {
                    var door              = elevator.Doors[floorNum];
                    door.Left             = CreateRectangle(y, doorHeight, x, doorWidth, _gradientGray, 1);
                    door.Right            = CreateRectangle(y, doorHeight, x + doorWidth, doorWidth, _gradientGray, 1);
                    var ys                = y + doorHeight + 2;

                    // status floor number on top of elevator
                    var panel             = new StatusFloors();
                    var xp                = x + doorWidth - panel.Width / 2;
                    door.StatusOnFloor    = panel;
                    panel.LayoutTransform = new ScaleTransform(scaleX, statusHeight / panel.Height);
                    MyCanvas.Children.Add(panel);
                    Canvas.SetBottom(panel, ys);
                    Canvas.SetLeft(panel, xp);
              //      Floors[floorNum].StatusFloorNumber = panel;

                    y += floorHeightTotal;
                }

                elevators[e] = elevator;
                x += elevatorSpacing;
            }

    //        Stats.DataContext = Stats;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: create the Random Passenger,  Computes their weight, the display type and then returns.
        /// </summary>
        /// <returns>
        ///  The new passenger, the passenger is not added to passenger array yet, only initialized with values.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Passenger CreateRandomPassenger()
        {
            var weight = _random.Next(55, 200);
            var dtype  = (_random.Next(0, 101) < 40) ? DisplayType.CHILD : (DisplayType)_random.Next((int)DisplayType.MAN, (int)DisplayType.WOMEN);
            var passenger   = new Passenger(MyCanvas, weight, dtype);
            return passenger;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> 
        ///  Method: Create a Rectangle and add it to the canvas.  Used to draw simple rectanglar shapes, with fill and outline thickness.
        /// </summary>
        /// <param name="yBottom">         The y coordinate Bottom.</param>
        /// <param name="yHeight">         The y coordinate Height.</param>
        /// <param name="xLeft">           The x coordinate Left.</param>
        /// <param name="xWidth">          The x coordinate Width.</param>
        /// <param name="fill">            The fill.</param>
        /// <param name="strokeThickness"> The stroke Thickness.</param>
        /// <returns>
        ///  Rectangle that has been added to the canvas at the xLeft, yBottom position.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Rectangle CreateRectangle(double yBottom, double yHeight, double xLeft, double xWidth, Brush fill, double strokeThickness)
        {
            var rect = new Rectangle();
            MyCanvas.Children.Add(rect);

            rect.Fill            = fill;
            rect.Stroke          = _black;
            rect.Width           = xWidth;
            rect.Height          = yHeight;
            rect.StrokeThickness = strokeThickness;
            Canvas.SetBottom(rect, yBottom);
            Canvas.SetLeft(rect, xLeft);
            return rect;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: initialize all the elevators, giving them something to do.  (check destination queue)
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Init()
        {
            CheckDestinationQueues();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: recalculate the Stats, for this run, normally called after passenger exits or elevators move.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void RecalculateStats()
        {
            if (!ChallengeEnded &&  ElapsedTime >= _lastUpdate+1  )
            {
                 Stats?.Update();
                _lastUpdate = ElapsedTime;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: register the Passenger in the world.
        ///  Add passenger to list of passengers, and sets position and spawn time.
        /// </summary>
        /// <param name="passenger"> The passenger.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void RegisterUser(Passenger passenger)
        {
            MyCanvas.Children.Add(passenger);

            Passengers.Add(passenger);
            passenger.UpdateDisplayPosition();
            passenger.SpawnTimestamp = ElapsedTime;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: update, the world, this is the main loop called by the animation system on each tic.
        /// </summary>
        /// <param name="deltaTime"> The dt.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RenderFrame(double deltaTime)
        {
            if (IsPaused) return;

            if (ChallengeEnded)
            {
                Update(deltaTime);
                return;
            }

            ElapsedTime        += deltaTime;
            _elapsedSinceSpawn += deltaTime;

            while (_elapsedSinceSpawn > 1.0 / Options.SpawnRate)
            {
                _elapsedSinceSpawn -= 1.0 / Options.SpawnRate;
                RegisterUser(SpawnUserRandomly());
            }

            Update(deltaTime);
            RecalculateStats();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Update the status (movement etc of elements), doesn't update the positions just the velocity and
        ///  other time depedent values.
        /// </summary>
        /// <param name="deltaTime"> The delta Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Update(double deltaTime)
        {
            // update the elevators
            foreach (var elevator in Elevators)
            {
                elevator.Update(deltaTime);
            }

            // update the passengers (mostly moving)
            foreach (var t in Passengers)
            {
                t.Update(deltaTime);
            }

            // clean up any passengers that are finished
            // todo add to cleanup list instead
            for (var i = Passengers.Count - 1; i >= 0; i--)
            {
                var u = Passengers[i];
                if (u.RemoveMe)
                {
                    Passengers.RemoveAt(i);
                    _controller._myCanvas.Children.Remove(u);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: spawn a Passenger Randomly, computes the floor to spawn on assumeng 50% spawn on 1st floor (floor 0)
        /// </summary>
        /// <returns>
        ///  The new passenger, with starting floor and destination floor set.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Passenger SpawnUserRandomly()
        {
            var passenger = CreateRandomPassenger();

            var spawnFloorNum  = _random.Next(2) == 0 ? 0 : _random.Next(_floorCount);
            passenger.CurrentFloor = spawnFloorNum;

            int destinationFloor;
            if (spawnFloorNum == 0)
            {
                // Definitely going up
                destinationFloor = _random.Next(1, _floorCount);
            }
            else
            {
                // Usually going down, but sometimes not
                if (_random.Next(5) == 0)
                {
                    destinationFloor = (spawnFloorNum + _random.Next(1, _floorCount)) % _floorCount;
                }
                else
                {
                    destinationFloor = 0;
                }
            }

            passenger.StartNewUserOnFloor(Floors[spawnFloorNum], destinationFloor, UserScale);
            return passenger;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Update stats when Passenger Exited Elevator.
        /// </summary>
        /// <param name="passenger"> The passenger.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StatsUserExitedElevator(Passenger passenger)
        {
            TransportedCounter++;
            Stats.AddWaitTime( ElapsedTime - passenger.SpawnTimestamp);
            RecalculateStats();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: unwind the current state, move passengers off, park elevators
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UnWind()
        {
            ElapsedTime = Options.MaxTime;
            RecalculateStats();

            foreach (var passenger in Passengers)
            {
                passenger.Off();
            }

            foreach (var elevator in Elevators)
            {
                elevator.Off();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Done - waiting to all passengers to disappear, then clear the floors.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsDone()
        {
            if (Passengers.Count == 0)
            {
                for (var index = 0; index < Floors.Length; index++)
                {
                    var floor = Floors[index];
                    floor.Off();
                    Floors[index] = null;
                }
                return true;
            }
            return false;
        }

     ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Method: update the Display Position of the elevators and the passengers of world.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void UpdateDisplayPositions()
        {
            for (var i = 0; i < Elevators.Length; ++i)
            {
                Elevators[i].UpdateDisplayPosition();
            }
            for (var i = 0; i < Passengers.Count; ++i)
            {
                Passengers[i].UpdateDisplayPosition();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Get a string representation of Destination Queue
        /// </summary>
        /// <returns>
        ///  The string representation, space between floor numbers
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string ArrayToString(bool[] array)
        {
            StringBuilder b = new StringBuilder(100);
            for (var index = 0; index < array.Length; index++)
            {
                if (array[index]) b.Append($@" {index + 1}");
            }
            return b.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Notify the Passenger Arrived
        /// </summary>
        /// <param name="floorNum"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void NotifyPassengerArrived(int floorNum)
        {
            foreach (var elevator in Elevators)
            {
                if (!elevator.IsMoving)
                {
                    elevator.NeedDecision =true;
                    if (elevator.CurrentFloorNumber == floorNum)
                    {
                        elevator.StartDoorOpen();
                    }
                }
            }

        }
    }

}


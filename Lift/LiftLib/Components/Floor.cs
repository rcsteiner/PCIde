////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:  Defines a floor
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/3/2018   RCS       Initial code.
//  
// 
//=====================================================[ Copyright ]=====================================================
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
using Lift.View;

namespace Lift
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  The Floor Class definition, stores the floor information, and can reference the world Button states associated
    ///  with this floor.  It maintains a Waiting Queue of passengers waiting for the elevator.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Floor
    {
        /// <summary>
        ///  Gets the floor number assigned to this floor.
        /// </summary>
        public int FloorNum { get; private set; }
   
        /// <summary>
        ///  Get/Set Down button Pressed, indicating desire of a passenger to move to lower floor.
        /// </summary>
        public bool DownPressed {get { return _world.floorButtonPressedDown[FloorNum]; }set { _world.floorButtonPressedDown[FloorNum] = value; }}

        /// <summary>
        ///  Get/Set Up button Pressed, indicating the desire of a passenger to move to a higher floor.
        /// </summary>
        public bool UpPressed { get { return _world.floorButtonPressedUp[FloorNum]; } set { _world.floorButtonPressedUp[FloorNum] = value; } }

        /// <summary>
        ///  Get/Set Floor Width. Used to compute where a passenger can appear
        /// </summary>
        public double FloorWidth { get; set; }

        /// <summary>
        ///  Get/Set X position for spawning on this floor.
        /// </summary>
        public double XSpawn { get; set; }

        /// <summary>
        ///  Get/Set Y Position for spawning, floor Y position (above border)
        /// </summary>
        public double YSpawnPosition { get; set; }

        /// <summary>
        ///  The Waiting Queue on this floor.  Holds all passengers waiting for the elevator.
        /// </summary>
        public List<Passenger> WaitingQueue = new List<Passenger>(30);

        /// <summary>
        ///  Get/Set Buttons used to indicate desired direction.
        /// </summary>
        public Indicator DisplayButtons { get; set; }

        /// <summary>
        ///  The  random number generator, internal for generation of positions etc.
        /// </summary>
        private static Random _random = new Random();

        /// <summary>
        ///  The  world we live in.  Used to access global ButtonState arrays etc.
        /// </summary>
        private readonly World _world;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor:  Iniital values setup.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="floorWidth">     The floor Width.</param>
        /// <param name="floorNum">       The floor number.</param>
        /// <param name="xSpawn">         The x spawn position.</param>
        /// <param name="ySpawnPosition"> The y spawn position.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Floor(World world, double floorWidth, int floorNum, double xSpawn, double ySpawnPosition)
        {
            _world               = world;
            FloorWidth           = floorWidth;
            FloorNum             = floorNum;
            YSpawnPosition       = ySpawnPosition;
            XSpawn               = xSpawn;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Clear the up/down button based on direction
        /// </summary>
        /// <param name="dir"> The direction.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ClearButton(Direction dir)
        {
            if (dir == Direction.UP)
            {
                UpPressed = false;
                DisplayButtons.SetDirectionOff(Direction.UP);
            }
            else if (dir == Direction.DOWN)
            {
                DownPressed = false;
                DisplayButtons.SetDirectionOff(Direction.DOWN);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Count the Waiting in queue for a direction.
        /// </summary>
        /// <param name="dir"> The direction to count for.</param>
        /// <returns>
        ///  Number waiting on this floor.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int CountWaiting(Direction dir)
        {
            int n = 0;
            foreach (var passenger in WaitingQueue)
            {
                if (passenger.Direction == dir) ++n;
            }

            return n;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Elevator is Available, handle button reset, and any action when the elevator arrives at this floor.
        /// </summary>
        /// <param name="elevator"> The elevator.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ElevatorAvailable(Elevator elevator)
        {

            if (elevator.GoingUpIndicator && UpPressed)
            {
                UpPressed = false;
                DisplayButtons.SetDirectionOff(Direction.UP);
            }
            if (elevator.GoingDownIndicator && DownPressed)
            {
                DownPressed = false;
                DisplayButtons.SetDirectionOff(Direction.DOWN);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Get the Spawn position x coordinate, and adds the passenger to the waiting to the queue
        /// </summary>
        /// <param name="passenger"> The passenger.</param>
        /// <returns>
        ///  the x position to place this passenger on the floor.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double GetSpawnPosX(Passenger passenger)
        {
            WaitingQueue.Add(passenger);
            var w = FloorWidth;
            var range = w - XSpawn - w * .05;

            var x = ((WaitingQueue.Count - 1) * passenger.Width + _random.Next((int)(passenger.Width / 2))) % range;
            return XSpawn + x;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: get the Spawn position y coordinate
        /// </summary>
        /// <returns>
        ///  The y coordinate the passenger will appear at.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double GetSpawnPosY()
        {
            return YSpawnPosition;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: turn off the floor and release resources.  Turns buttons off.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Off()
        {
            DisplayButtons.SetDirectionOff(Direction.STOPPED);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Press the Down Button,  pressed by passenger requesting to go down.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void PressDownButton()
        {
            DisplayButtons.SetDirectionOn(Direction.DOWN);
            DownPressed = true;
            _world.NotifyPassengerArrived(FloorNum);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Press the Up Button.  pressed by passenger requesting to go up.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void PressUpButton()
        {
            DisplayButtons.SetDirectionOn(Direction.UP);
            UpPressed = true;
            _world.NotifyPassengerArrived(this.FloorNum);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Repress the Buttons, happens after elevator leaves and passengers are still waiting.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RepressButtons()
        {
            DisplayButtons.SetDirectionOff(Direction.STOPPED);
            DownPressed = false;
            UpPressed = false;
            foreach (var passenger in WaitingQueue)
            {
                if (!UpPressed && passenger.Direction == Direction.UP)
                {
                    PressUpButton();
                }
                if (!DownPressed && passenger.Direction == Direction.DOWN)
                {
                    PressDownButton();
                }
            }
        }
    }
}


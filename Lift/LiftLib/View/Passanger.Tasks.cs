////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:  Users of the system.
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/17/2018  RCS       Initial code.
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
using System.Windows.Media.Imaging;

namespace Lift.View
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Interaction logic for Passenger.xaml, defines a icon for the passenger 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class Passenger
    {
        /// <summary>
        ///  The elevator assigned to this passenger.
        /// </summary>
        private Elevator _elevator;

        /// <summary>
        ///  The  passenger Recycler.
        /// </summary>
        private static Queue<Passenger>  _passengerRecycler = new Queue<Passenger>(100);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: appear the On Floor, puts this passenger on the floor number and moves him to to Position
        ///  and presses the floor button (floor to go to)
        /// </summary>
        /// <param name="floor">               The floor the passenger is on.</param>
        /// <param name="destinationFloorNum"> The destination Floor number.</param>
        /// <param name="width">               The width.</param>
        /// <param name="height">              The height.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StartNewUserOnFloor(Floor floor, int destinationFloorNum, double UserScale)
        {
          //  LayoutTransform = new ScaleTransform(width / 30, height / 30);

            CurrentFloor     = floor.FloorNum;
            DestinationFloor = destinationFloorNum;
            myImage.Source   = new BitmapImage(UserImage);
            Width            = myImage.Source.Width * UserScale;
            Height           = myImage.Source.Height *UserScale;
            var floorPosY    = floor.GetSpawnPosY();
            var floorPosX    = floor.GetSpawnPosX(this);
            MoveTo(floorPosX, floorPosY);
            PressFloorButton(floor);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Start the Loading into elevator,  sets the passenger to move onto the elevator.
        /// </summary>
        /// <param name="elevator"> The elevator.</param>
        /// <param name="xOff">     The x coordinate Off.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StartLoad(Elevator elevator, int xOff)
        {
            SetParent(elevator);
            _elevator = elevator;
            MoveToOverTime(xOff, YOff, 1, null, StartOnBoard);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Start the On Board, press the destination button on the elevator.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void StartOnBoard()
        {
            // adjust the position of everyone on the platform
            _elevator.PressFloorButton(DestinationFloor);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Start the Off Board sequence, passenger moves off the elevator.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StartOffBoard()
        {
            CurrentFloor = DestinationFloor;
            var xOff     = -10;
            SetParent(null);
            MoveToOverTime(xOff - Width,YOff, 1.5, null, Bye);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Remove this passenger and cleanup refernences.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Bye()
        {
            _elevator.UserOff(this);
            _elevator    = null;
            RemoveMe     = true;
            _currentTask = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: turn off passenger.
        ///  Take off elevator immediately.
        ///   Move off the screen and free resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Off()
        {
             SetParent(null);
            MoveToOverTime(MyCanvas.ActualWidth + Width, YOff, 3, null, End);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Terminate the passenger, free resource, make for deletion.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void End()
        {
            _elevator    = null;
            RemoveMe     = true;
            _currentTask = null;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        /////  Method: elevator the Available
        ///// </summary>
        ///// <param name="elevator"> The elevator.</param>
        ///// <param name="floor">    The floor.</param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public void ElevatorAvailable(ViewElevator elevator, Floor floor)
        //{
        //    // if in the elevator, parent  or busy moving or done
        //    if (_done || MyParent != null || IsBusy())
        //    {
        //        return;
        //    }

        //    if (!elevator.IsSuitableForTravelBetween(CurrentFloor, DestinationFloor))
        //    {
        //        // Not suitable for travel - don't use this elevator
        //        return;
        //    }

        //    var slotAvailable = elevator.UserEntering(this, out pos);
        //    if (slotAvailable)
        //    {
        //        // Success
        //        SetParent(elevator);
        //        EnteredElevator?.Invoke(this, elevator, floor.FloorNum);
        //        MoveToOverTime(pos.X, pos.Y, 1, null, StartOnBoard);
        //        floor.WaitingQueue.Remove(this);
        //        elevator.ExitAvailable += StartOffBoard;                   // add event
        //    }
        //    else
        //    {
        //        PressFloorButton(floor);
        //    }
        //}

    }
}

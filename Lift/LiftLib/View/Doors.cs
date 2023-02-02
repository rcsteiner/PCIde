////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   The elevator doors 
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
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Lift.View
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  The Doors Class definition.  The doors are draw semi-transparent on each floor, one door for each elevator/floor.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class Doors : Movable
    {

        /// <summary>
        ///  Get Elevator associated with.
        /// </summary>
        public Elevator Elevator { get; }

        /// <summary>
        ///  Get/Set Floor these doors are on.
        /// </summary>
        public Floor Floor { get; set; }

        /// <summary>
        ///  Get/Set Left Door rectangle.
        /// </summary>
        public Rectangle Left { get; set; }

        /// <summary>
        ///  Get/Set Right Door rectangle.
        /// </summary>
        public Rectangle Right { get; set; }

        /// <summary>
        ///  Get/Set State of the Door (see DoorState)
        /// </summary>
        public DoorState State { get; set; }

        /// <summary>
        ///  Get/Set The status indicator showing the floor number to the passengers (above the elevator)
        /// </summary>
        public StatusFloors StatusOnFloor { get; set; }

        /// <summary>
        ///  Get Waiting Queue associated with this floor.
        /// </summary>
        public List<Passenger> WaitingQueue { get { return Floor.WaitingQueue; } }

        /// <summary>
        ///  Get XPosition of the doors
        /// </summary>
        public double XPos { get { return Elevator.XOff; } }

        /// <summary>
        ///  Get YPosition of the door from the floor the door is on.
        /// </summary>
        public double YPos { get { return Floor.YSpawnPosition; } }

        /// <summary>
        ///  The  fix Width.
        /// </summary>
        private double _fixWidth;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  The Door State Enumeration definition.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public enum DoorState
        {
            /// <summary>
            ///  DoorState CLOSED.
            /// </summary>
            CLOSED,

            /// <summary>
            ///  DoorState CLOSING.
            /// </summary>
            CLOSING,

            /// <summary>
            ///  DoorState OPEN.
            /// </summary>
            OPEN,

            /// <summary>
            ///  DoorState OPENING.
            /// </summary>
            OPENING,
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor:  Setup the canvas, elevator, floor and size of the door.
        /// </summary>
        /// <param name="canvas">   The canvas.</param>
        /// <param name="elevator"> The elevator.</param>
        /// <param name="floor">    The floor.</param>
        /// <param name="width">    The width.</param>
        /// <param name="height">   The height.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Doors(Canvas canvas, Elevator elevator, Floor floor, double width, double height) : base(canvas)
        {
            Elevator  = elevator;
            Width     = width;
            _fixWidth = width;
            Height    = height;
            Floor     = floor;
            State     = DoorState.CLOSED;
        }
    }
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:  The Passenger object
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
using System.Windows.Controls;


namespace Lift.View
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Interaction logic for Passenger.xaml, defines aspects of passenger.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class Passenger : Movable
    {
        /// <summary>
        ///  Get Display Type.
        /// </summary>
        public DisplayType DisplayType { get; }

        /// <summary>
        ///  Get/Set Spawn Timestamp, time passenger was created.
        /// </summary>
        public double SpawnTimestamp { get; set; }

        /// <summary>
        ///  Get/Set Passenger Image (icon)
        /// </summary>
        public Uri UserImage { get; set; }

        /// <summary>
        ///  Get/Set Weight of the passenger.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        ///  Get Direction the passenger wishes to travel 
        /// </summary>
        public Direction Direction {get { return DestinationFloor > CurrentFloor ? Direction.UP : Direction.DOWN; }}

        /// <summary>
        ///  The current Floor the passenger is on.
        /// </summary>
        public int CurrentFloor;

        /// <summary>
        ///  The destination Floor the passenger wants to go to.
        /// </summary>
        public int DestinationFloor;

        /// <summary>
        ///  The remove Me flag, if true this passenger will be deleted during update.
        /// </summary>
        public bool RemoveMe;

        /// <summary>
        ///  The FAMILY Icon.
        /// </summary>
        public static Uri FAMILY = new Uri(@"../Images/White/x64/family.png", UriKind.Relative);

        /// <summary>
        ///  The MAN Icon.
        /// </summary>
        public static Uri MAN = new Uri(@"../Images/White/x64/man.png", UriKind.Relative);

        /// <summary>
        ///  The WOMAN Icon.
        /// </summary>
        public static Uri WOMAN = new Uri(@"../Images/White/x64/woman.png", UriKind.Relative);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  The State Enumeration definition.  Not used right now.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public enum State
        {

            /// <summary>
            ///  State ENTERING ELEVATOR.
            /// </summary>
            ENTERING_ELEVATOR,

            /// <summary>
            ///  State EXITING ELEVATOR.
            /// </summary>
            EXITING_ELEVATOR,

            /// <summary>
            ///  State EXITING FLOOR.
            /// </summary>
            EXITING_FLOOR,

            /// <summary>
            ///  State GONE.
            /// </summary>
            GONE,

            /// <summary>
            ///  State PRESSING ELEVATOR BUTTON.
            /// </summary>
            PRESSING_ELEVATOR_BUTTON,

            /// <summary>
            ///  State PRESSING FLOOR BUTTON.
            /// </summary>
            PRESSING_FLOOR_BUTTON,

            /// <summary>
            ///  State SPAWN.
            /// </summary>
            SPAWN,

            /// <summary>
            ///  State WAITING FOR EVEVATOR.
            /// </summary>
            WAITING_FOR_EVEVATOR,

            /// <summary>
            ///  State WAITING IN ELEVATOR.
            /// </summary>
            WAITING_IN_ELEVATOR,

            /// <summary>
            ///  State WALKING TO ELEVATOR.
            /// </summary>
            WALKING_TO_ELEVATOR
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor: (design time initialization)
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Passenger()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor:  Set the canvas, weight and type of passenger.
        /// </summary>
        /// <param name="canvas">      The canvas to draw on.</param>
        /// <param name="weight">      The weight of the passenger in pounds.</param>
        /// <param name="displayType"> The display Type.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Passenger(Canvas canvas,int weight, DisplayType displayType) : base(canvas)
        {
            InitializeComponent();
            Weight      = weight;
            DisplayType = displayType;
            switch (displayType)
            {
                case DisplayType.CHILD:
                    UserImage = FAMILY;
                    break;

                case DisplayType.MAN:
                    UserImage = MAN;
                    break;

                case DisplayType.WOMEN:
                    UserImage = WOMAN;
                    break;

            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: press the Floor Button, to call an elevator, based on my floor and destination floor.
        /// </summary>
        /// <param name="floor"> The floor I am on.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void PressFloorButton(Floor floor)
        {
            if (DestinationFloor < CurrentFloor)
            {
                floor.PressDownButton();
            }
            else
            {
                floor.PressUpButton();
            }

        }
    }
}


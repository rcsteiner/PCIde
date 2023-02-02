////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   Manages the elevator view object.
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/14/2018  RCS       Initial code.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace Lift.View
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Interaction logic for Elevator.xaml, The elevator is the central component and primary stimulus for changes.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class Elevator : Movable
    {

        /// <summary>
        ///  Get/Set Current Direction the elevator is traveling.
        /// </summary>
        public Direction CurrentDirection { get; set; }

        /// <summary>
        ///  Gets the floor number that the elevator currently is on.
        /// </summary>
        public int CurrentFloorNumber
        {
            get { return _currentFloorNumber; }
            private set {if (value >= floorStops.Length) _currentFloorNumber = floorStops.Length - 1; else _currentFloorNumber = value; }
        }

        /// <summary>
        ///  The current destination queue, meaning the floor numbers the elevator is scheduled to go to. Can be
        ///  modified and emptied if desired. Note that you need to call checkDestinationQueue() for the change to take
        ///  effect immediately.
        /// </summary>
        public bool[] floorStops { get; set; }

        /// <summary>
        ///  Get/Set Doors array (one door per floor) associated with this elevator.
        /// </summary>
        public Doors[] Doors { get; set; }

        /// <summary>
        ///  Get/Set Going Down Indicator which will affect passenger behaviour when stopping at floors.
        /// </summary>
        public bool GoingDownIndicator
        {
            get { return _goingDownIndicator; }
            set
            {
                _goingDownIndicator = value;
                OnIndicatorChange();
            }
        }

        /// <summary>
        ///  Get/Set Going Up Indicator which will affect passenger behaviour when stopping at floors.
        /// </summary>
        public bool GoingUpIndicator
        {
            get { return _goingUpIndicator; }
            set
            {
                _goingUpIndicator = value;
                OnIndicatorChange();
            }
        }

        /// <summary>
        ///  The Is Moving flag
        /// </summary>
        public bool IsMoving { get; set; }

        /// <summary>
        ///  Get/Set Load Factor  of the elevator. 0 means empty, 1 means full. Varies with passenger weights, which
        ///  vary - not an exact measure.
        /// </summary>
        public int LoadFactor { get; private set; }

        /// <summary>
        ///  Get/Set maximum Passenger Count that can occupy the elevator at the same time.
        /// </summary>
        public int MaxPassengers { get; private set; }

        /// <summary>
        ///  Get/Set Slots Available in the elevator.  Currently each passenger gets a single slot
        ///  it can be possible that a passenger could take multiple slots.  In that case, the SlotsAvailable
        ///  count should be adjusted to reflect "bigger" passengers.
        /// </summary>
        public int SlotsAvailable { get; set; }
        /// <summary>
        ///  Get Current Floor the elevator is on.
        /// </summary>
        private Floor CurrentFloor { get { return Doors[CurrentFloorNumber].Floor; } }

        /// <summary>
        ///  The  acceleration.
        /// </summary>
        private double _acceleration;

    
        /// <summary>
        ///  The  deceleration.
        /// </summary>
        private double _deceleration;

        /// <summary>
        ///  The  destination y coordinate.
        /// </summary>
        private double _destinationY;

        /// <summary>
        ///  The  floor Count.
        /// </summary>
        private readonly int _floorCount;

        /// <summary>
        ///  The  floor Height.
        /// </summary>
        private readonly double _floorHeight;

        /// <summary>
        ///  The  floor Thick.
        /// </summary>
        private readonly double _floorThick;

        /// <summary>
        ///  The  going Down Indicator.
        /// </summary>
        private bool _goingDownIndicator;

        /// <summary>
        ///  The  going Up Indicator.
        /// </summary>
        private bool _goingUpIndicator;

        /// <summary>
        ///  The  maxspeed allowed
        /// </summary>
        private double _maxspeed;

        /// <summary>
        ///  The  speed Floors Per security.
        /// </summary>
        private readonly double _speedFloorsPerSec;

        /// <summary>
        ///  The  passenger Slots in the elevator.
        /// </summary>
        private Passenger[] _passengerSlots;

        /// <summary>
        ///  The  velocity y coordinate.
        /// </summary>
        private double _velocityY;

        /// <summary>
        ///  The  current Floor Number.
        /// </summary>
        private int _currentFloorNumber;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor:
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Elevator()
        {
            InitializeComponent();
            SetDirection(Direction.STOPPED);
            DecideNextFloorMethod = DecideNextFloor;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor:
        /// </summary>
        /// <param name="canvas">            The canvas.</param>
        /// <param name="speedFloorsPerSec"> The speed Floors Per security.</param>
        /// <param name="floorCount">        The floor Count.</param>
        /// <param name="floorHeight">       The floor Height.</param>
        /// <param name="floorThick">        The floor Thick.</param>
        /// <param name="maxPassengers">     The maximum Users.</param>
        /// <param name="world">             The world.</param>
        /// <param name="decider">           [optional=null use built in] The decider.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Elevator(Canvas canvas, double speedFloorsPerSec, int floorCount, double floorHeight, double floorThick, int maxPassengers, World world, DecideNextFloorHandler decider=null) : base(canvas)
        {
            InitializeComponent();

            _world             = world;

            _floorCount        = floorCount;
            DecideNextFloorMethod = decider?? DecideNextFloor;
            _floorHeight       = floorHeight;
            _floorThick        = floorThick;
            floorStops         = new  bool[floorCount];

            _speedFloorsPerSec = speedFloorsPerSec;
            _acceleration      = floorHeight * 2.1;
            _deceleration      = floorHeight * 2.6;
            _maxspeed          = floorHeight * speedFloorsPerSec;
            _destinationY      = 0.0;
            _velocityY         = 0.0;

            IsMoving           = false;
            GoingUpIndicator   = true;
            GoingDownIndicator = false;

            CurrentDirection   = Direction.STOPPED;
            CurrentFloorNumber = 0;
            _destinationY      = GetYPosOfFloor(CurrentFloorNumber);

            _passengerSlots    = new Passenger[maxPassengers];
            SlotsAvailable     = maxPassengers;
            MaxPassengers      = maxPassengers;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: acceleration the Needed To Achieve Change Distance
        /// </summary>
        /// <param name="currentSpeed"> The current Speed.</param>
        /// <param name="targetSpeed">  The target Speed.</param>
        /// <param name="distance">     The distance.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double AccelerationNeededToAchieveChangeDistance(double currentSpeed, double targetSpeed, double distance)
        {
            // v² = u² + 2a * d
            var requiredAcceleration = 0.5 * ((Math.Pow(targetSpeed, 2) - Math.Pow(currentSpeed, 2)) / distance);
            return requiredAcceleration;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: clear an array to all false.
        /// </summary>
        /// <param name="array"> The array.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void clear(bool[] array)
        {

            for (int floor = 0;  floor < _world.Floors.Length; ++floor)
            {
                array[floor]=false;

            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: find the Next floor that is set.
        /// </summary>
        /// <param name="array">     The array.</param>
        /// <param name="direction"> The direction.</param>
        /// <returns>
        ///  The value.
        /// </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int findNext(bool[] array, Direction direction)
        {

            if (direction == Direction.UP)
            {
                for (int floor = CurrentFloorNumber + 1; floor < _world.Floors.Length; ++floor)
                {
                    if (array[floor]) return floor;

                }
                return CurrentFloorNumber;
            }

            for (int floor = CurrentFloorNumber -1; floor >= 0 ; --floor)
            {
                if (array[floor]) return floor;

            }
            return CurrentFloorNumber;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Check the Destination Queue
        ///  Checks the destination queue for any new destinations to go to
        ///  Note that you only need to call this if you
        ///  modify the destination queue explicitly.
        /// </summary>
        /// <returns>
        ///  True if successful
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CheckDestinationQueue()
        {
   //         var next = findNext(floorStops,CurrentDirection);
  //          if ( next != CurrentFloorNumber)
            //{
            //    GoToFloor(next);
            //    return true;
            //}

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: distance the Needed To Achieve Speed
        /// </summary>
        /// <param name="currentSpeed"> The current Speed.</param>
        /// <param name="targetSpeed">  The target Speed.</param>
        /// <param name="acceleration"> The acceleration.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double DistanceNeededToAchieveSpeed(double currentSpeed, double targetSpeed, double acceleration)
        {
            // v² = u² + 2a * d
            var requiredDistance = (Math.Pow(targetSpeed, 2) - Math.Pow(currentSpeed, 2)) / (2 * acceleration);
            return requiredDistance;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Epsilon the Equals
        /// </summary>
        /// <param name="a"> The a.</param>
        /// <param name="b"> The b.</param>
        /// <returns>
        ///  True if successful
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool EpsilonEquals(double a, double b)
        {
            return Math.Abs(a - b) < 0.00000001;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: get the Destination Floor
        /// </summary>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double GetDestinationFloor()
        {
            return GetExactFloorOfYPos(_destinationY);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: get the Exact Current Floor
        /// </summary>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double GetExactCurrentFloor()
        {
            return GetExactFloorOfYPos(YOff);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: get the Exact Floor Of YPos
        /// </summary>
        /// <param name="y"> The y coordinate.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double GetExactFloorOfYPos(double y)
        {
            return (int)(y / _floorHeight);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: get the Exact Future Floor If Stopped
        /// </summary>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double GetExactFutureFloorIfStopped()
        {
            var distanceNeededToStop = DistanceNeededToAchieveSpeed(_velocityY, 0.0, _deceleration);
            return GetExactFloorOfYPos(YOff - Math.Sign(_velocityY) * distanceNeededToStop);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: get the Load Factor
        /// </summary>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double GetLoadFactor()
        {
            double load = 0;
            foreach (var passenger in _passengerSlots)
            {
                load += passenger.Weight;
            }

            return load / (MaxPassengers * 100);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: get the Rounded Current Floor
        /// </summary>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int GetRoundedCurrentFloor()
        {
            return (int)Math.Round(GetExactCurrentFloor());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: get the YPos Of Floor
        /// </summary>
        /// <param name="floorNum"> The floor number.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private double GetYPosOfFloor(int floorNum)
        {
            return floorNum * _floorHeight + _floorThick/2 -2;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Go the To Floor
        ///  Queue the elevator to go to specified floor number. If you specify true as second argument, the elevator
        ///  will go to that floor directly, and then go to any other queued floors
        /// </summary>
        /// <param name="floorNum"> The floor number.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GoToFloor(int floorNum)
        {
            IsMoving = true;
            _destinationY = GetYPosOfFloor(floorNum) + 1;  // add 1 to keep from trying to be exactly on
            CurrentDirection=floorNum > CurrentFloorNumber ? Direction.UP : floorNum < CurrentFloorNumber ? Direction.DOWN : Direction.STOPPED;
            StartMove();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: is the Approaching Floor
        /// </summary>
        /// <param name="floorNum"> The floor number.</param>
        /// <returns>
        ///  True if successful
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsApproachingFloor(int floorNum)
        {
            var floorYPos = GetYPosOfFloor(floorNum);
            var elevToFloor = floorYPos - YOff;
            return _velocityY != 0.0 && (Math.Sign(_velocityY) == Math.Sign(elevToFloor));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: is the Empty
        /// </summary>
        /// <returns>
        ///  True if successful
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsEmpty()
        {
            for (var i = 0; i < _passengerSlots.Length; i++)
            {
                if (_passengerSlots[i] != null)
                {
                    return false;
                }
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: is the Full
        /// </summary>
        /// <returns>
        ///  True if successful
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsFull()
        {
            for (var i = 0; i < _passengerSlots.Length; i++)
            {
                if (_passengerSlots[i] == null)
                {
                    return false;
                }
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: is the On AFloor
        /// </summary>
        /// <returns>
        ///  True if successful
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsOnAFloor()
        {
            return EpsilonEquals(GetExactCurrentFloor(), GetRoundedCurrentFloor());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: is the Suitable For Travel Between
        /// </summary>
        /// <param name="fromFloorNum"> The from Floor number.</param>
        /// <param name="toFloorNum">   The to Floor number.</param>
        /// <returns>
        ///  True if successful
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsSuitableForTravelBetween(int fromFloorNum, int toFloorNum)
        {
            if (fromFloorNum > toFloorNum)
            {
                return GoingDownIndicator;
            }

            if (fromFloorNum < toFloorNum)
            {
                return GoingUpIndicator;
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Lock the To Floor
        /// </summary>
        /// <param name="floor"> The floor.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LockToCurrentFloor(int floor)
        {
            var destination = GetYPosOfFloor(floor);
            CurrentFloorNumber = floor;
            MoveToY(destination);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Turn elevator off and free resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Off()
        {
            for (int i = 0; i < _passengerSlots.Length; i++)
            {
                _passengerSlots[i] = null;
            }
            clear(floorStops);
            GoToFloor(-1);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: On the Indicator Change
        ///  Sets current direction and handles event.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnIndicatorChange()
        {
            CurrentDirection = GoingDownIndicator ? Direction.DOWN : GoingUpIndicator ? Direction.UP : Direction.STOPPED;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: press the Floor Button, if first time the notify listeners. Decrement slot counter
        /// </summary>
        /// <param name="floorNumber"> The floor Number.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void PressFloorButton(int floorNumber)
        {
            --_slotChange;
            var prev = floorStops[floorNumber];
            floorStops[floorNumber] = true;
            if (!prev)
            {
                floorStops[floorNumber]=true;
                SetElevatorStops();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Set the Current Floor Display
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetCurrentFloorDisplay()
        {
            foreach (var door in Doors)
            {
                door.StatusOnFloor.SetElevatorFloor(CurrentFloorNumber);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Set the Direction  clear departing buttons.
        /// </summary>
        /// <param name="dir"> The dir.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetDirection(Direction dir)
        {
            xIndicator.SetDirectionOn(dir);
            CurrentFloor.ClearButton(dir);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Set the Elevator Stops
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetElevatorStops()
        {
            xIndicator.SetElevatorStops(floorStops);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Stop
        ///  Clear the destination queue and stop the elevator if it is moving. Note that you normally don't need to
        ///  stop elevators - it is intended for advanced solutions with in-transit rescheduling logic. Also, note that
        ///  the elevator will probably not stop at a floor, so passengers will not get out.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Stop()
        {
            clear(floorStops);
            if (!IsBusy())
            {
                GoToFloor((int)GetExactFutureFloorIfStopped());
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Update the Elevator information
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UpdateElevatorInfo()
        {
            // Recalculate the floor number etc
            var currentFloor = GetRoundedCurrentFloor();

            if (currentFloor != CurrentFloorNumber)
            {
                if (_world.Stats != null)
                {
                    _world.Stats.Moves++;
                }

                CurrentFloorNumber = currentFloor;
                SetCurrentFloorDisplay();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Passenger is Off,  decrement slot changes expected, update the move stats.
        /// </summary>
        /// <param name="passenger"> The passenger.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UserOff(Passenger passenger)
        {
            --_slotChange;
            _world.StatsUserExitedElevator(passenger);
        }
    }
}


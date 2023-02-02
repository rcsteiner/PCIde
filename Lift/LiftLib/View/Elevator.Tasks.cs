////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   Elevator tasks
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/16/2018  RCS       Initial code.
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
using System.Text;

namespace Lift.View
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  The View Elevator Class definition.
    ///  Each task is introduced by a method:
    ///  Start...  sets up conditions,
    ///  Task...   does it over time, and calls Start... next thing to start when finished with task
    ///  Tasks are:
    ///  OpenDoor
    ///  Load
    ///  CloseDoor
    ///  Move
    ///  Unload
    ///  Idle
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class Elevator
    {

        /// <summary>
        ///  Get Reverse Direction.
        /// </summary>
        public Direction ReverseDirection { get { return CurrentDirection == Direction.UP ? Direction.DOWN : Direction.UP; } }

        /// <summary>
        ///  The  slot Changing count used to signal all aboard or all off.
        /// </summary>
        private int _slotChange;

        /// <summary>
        ///  The  template type Passenger.
        /// </summary>
        private List<Passenger> _tPassengers = new List<Passenger>(20);

        /// <summary>
        ///  The  world.
        /// </summary>
        private World _world;

        /// <summary>
        ///  The  Decide Next Floor method
        /// </summary>
        public DecideNextFloorHandler DecideNextFloorMethod { get; set; }

        /// <summary>
        ///  Get/Set Need Decision.
        /// </summary>
        public bool NeedDecision { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Find the Closest Down Floor with button pressed.
        /// </summary>
        /// <returns>
        ///  The floor number or -1 if not found.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int ClosestDownFloor()
        {
            for (int f = CurrentFloorNumber - 1; f >= 0; --f)
            {
                // empty, so wait for door press or see if needed on another floor
                if (Doors[f].WaitingQueue.Count > 0)
                {
                    return f;
                }
            }
            return -1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Find Closest Up Floor with button pushed.
        /// </summary>
        /// <returns>
        ///  The floor number of -1 if not found.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int ClosestUpFloor()
        {
            for (int f = CurrentFloorNumber + 1; f < _floorCount; ++f)
            {
                // empty, so wait for door press or see if needed on another floor
                if (Doors[f].WaitingQueue.Count > 0)
                {
                    return f;
                }
            }
            return -1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method:  Setup the Decide task what to do when the doors close.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StartDecision()
        {
            _currentTask = TaskDecision;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Start the Door Close task.  Clears button states
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void StartDoorClose()
        {
            floorStops[CurrentFloorNumber] = false;

            var viewDoors = Doors[CurrentFloorNumber];
            _currentTask  = WaitDoorClose;
            _nextStep     = StartDecision;
            viewDoors.StartDoorClose();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Open the doors, next off board passengers.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StartDoorOpen()
        {
            if (IsMoving || _currentTask==WaitDoorClose || _currentTask==WaitDoorClose) return;

            floorStops[CurrentFloorNumber] = false;
            var floor = Doors[CurrentFloorNumber].Floor;
            floor.ElevatorAvailable(this);

            Doors[CurrentFloorNumber].StatusOnFloor.SetElevatorFloor(CurrentFloorNumber);

            var viewDoors = Doors[CurrentFloorNumber];
            _currentTask = WaitDoorOpen;
            _nextStep = StartUnload;
            viewDoors.StartDoorOpen();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Start the Loading of passengers
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void StartLoad()
        {
            _slotChange      = 0;
            var doors        = Doors[CurrentFloorNumber];
            var waitingQueue = doors.WaitingQueue;


            // decide which direction we will go, if direction is stopped

            if (waitingQueue.Count > 0)
            {
                if (CurrentDirection == Direction.STOPPED || SlotsAvailable == MaxPassengers)
                {
                    CurrentDirection = waitingQueue[0].Direction;
                }

                doors.Floor.DisplayButtons.SetDirectionOff(CurrentDirection);

                // add any passengers
                foreach (var passenger in waitingQueue)
                {
                    if (SlotsAvailable == 0) break;
                    if (passenger.Direction == CurrentDirection)
                    {
                        var randomOffset = Random.Next(_passengerSlots.Length);
                        for (var i = 0; i < _passengerSlots.Length; i++)
                        {
                            int n = (randomOffset + i) % _passengerSlots.Length;
                            if (_passengerSlots[n] == null)
                            {
                                _tPassengers.Add(passenger);
                                _passengerSlots[n] = passenger;
                                int x = 2 + (n * 15);
                                passenger.StartLoad(this, x);
                                --SlotsAvailable;
                                ++_slotChange;
                                break;
                            }
                        }
                    }
                }

                // now clear from waiting queue
                foreach (var passenger in _tPassengers)
                {
                    waitingQueue.Remove(passenger);
                }

                _tPassengers.Clear();
            }

            _currentTask = TaskLoad;
            _nextStep = StartDoorClose;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Start the Elevator moving by setting current task to TaskMove
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StartMove()
        {
            // setup direction etc
            SetDirection(Direction.STOPPED);

            SetDirection(CurrentDirection);
            CurrentFloor.RepressButtons();
            _currentTask = TaskMove;
            _nextStep    = StartDoorOpen;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Start the Unload of passengers, if any.
        ///  Sets the _slotChange counter for the number that gets, off, when zero all passengers are off.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void StartUnload()
        {
            _currentTask   = TaskUnload;
            _nextStep      = StartLoad;
            _slotChange    = 0;
            for (var index = 0; index < _passengerSlots.Length; index++)
            {
                var u = _passengerSlots[index];
                if (u != null && u.DestinationFloor == CurrentFloorNumber)
                {
                    ++_slotChange;
                    _passengerSlots[index] = null;
                    ++SlotsAvailable;
                    u.StartOffBoard();
                }
            }

            // clear destination queue for this floor only
            floorStops[ CurrentFloorNumber]=false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Task Making the Decision of where to move next.
        /// </summary>
        /// <param name="deltaTime"> The delta Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void TaskDecision(double deltaTime)
        {
            // let control decide what to do
            // decision is in the destination queue for stops
            MakeDecision();

            // final order to use
            SetElevatorStops();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Make the Decision
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MakeDecision()
        {
            Decide(CurrentFloorNumber,(int) CurrentDirection, floorStops, _world.floorButtonPressedUp,
                _world.floorButtonPressedDown);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Decide the With Users, elevator has passengers so make a decision on next floor.
        /// </summary>
        /// <param name="currentFloorNumber"> The current Floor Number.</param>
        /// <param name="currentDirection">   The current Direction.</param>
        /// <param name="floorSelection">     The destination Queue (floors pressed).</param>
        /// <param name="upfloorButtons">     The upfloor Buttons.</param>
        /// <param name="downFloorButtons">   The down Floor Buttons.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Decide(int currentFloorNumber, int currentDirection, bool[] floorSelection, bool[] upfloorButtons, bool[] downFloorButtons)
        {
            int floor =DecideNextFloorMethod(currentFloorNumber, currentDirection, floorSelection, upfloorButtons, downFloorButtons);

            if (floor == currentFloorNumber)
            {
                CurrentDirection = Direction.STOPPED;
            }
            else
            {
                GoToFloor(floor);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: decide the Next Floor
        /// </summary>
        /// <param name="currentFloorNumber"> The current Floor Number.</param>
        /// <param name="currentDirection">   The current Direction.</param>
        /// <param name="floorSelection">     The floor Selection.</param>
        /// <param name="upFloorButtons">     The upfloor Buttons.</param>
        /// <param name="downFloorButtons">   The down Floor Buttons.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public  int DecideNextFloor(int currentFloorNumber, int currentDirection, bool[] floorSelection,bool[] upFloorButtons, bool[] downFloorButtons)
        {
            int floor = findNext(floorSelection, CurrentDirection);
            if (floor == currentFloorNumber)
            {
                floor = nextFloor((Direction)currentDirection, upFloorButtons, downFloorButtons);
            }

            if (floor == currentFloorNumber)
            {
                floor = nextFloor(ReverseDirection, upFloorButtons, downFloorButtons);
            }

            return floor;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: get the next Floor in up/down buttons.
        /// </summary>
        /// <param name="dir">              The dir.</param>
        /// <param name="upfloorButtons">   The upfloor Buttons.</param>
        /// <param name="downFloorButtons"> The down Floor Buttons.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int nextFloor(Direction dir, bool[] upfloorButtons, bool[] downFloorButtons)
        {
            int floor;
            if (dir == Direction.UP)
            {
                floor = findNext(upfloorButtons, Direction.UP);
            }
            else
            {
                floor = findNext(downFloorButtons, dir);
            }

            if (CurrentFloorNumber == floor)
            {
                DecideEmptyCar(CurrentFloorNumber,CurrentDirection);
            }
            return floor;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Handle the Empty Car
        ///  empty, so wait for door press or see if needed on another floor
        /// </summary>
        /// <param name="currentFloorNumber"> The current Floor Number.</param>
        /// <param name="currentDirection">   The current Direction.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DecideEmptyCar(int currentFloorNumber, Direction currentDirection)
        {
            if (Doors[currentFloorNumber].WaitingQueue.Count > 0)
            {
                StartDoorOpen();
            }

            int f;
            switch (currentDirection)
            {
                case Direction.DOWN:
                    f = ClosestDownFloor();
                    if (f == -1)
                    {
                        CurrentDirection = Direction.UP;
                        f = ClosestUpFloor();
                        if (f == -1)
                        {
                            CurrentDirection = Direction.STOPPED;
                            break;
                        }
                    }

                    GoToFloor(f);

                    return;
                case Direction.STOPPED:
                    CurrentDirection = Direction.UP;
                    f = ClosestUpFloor();
                    if (f == -1)
                    {
                        CurrentDirection = Direction.DOWN;
                        f = ClosestDownFloor();
                        if (f == -1)
                        {
                            CurrentDirection = Direction.STOPPED;
                            break;
                        }
                    }

                    GoToFloor(f);
                    break;
                case Direction.UP:
                    f = ClosestUpFloor();
                    if (f == -1)
                    {
                        CurrentDirection = Direction.DOWN;
                        f = ClosestDownFloor();
                        if (f == -1)
                        {
                            CurrentDirection = Direction.STOPPED;
                            break;
                        }
                    }

                    GoToFloor(f);
                    return;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Task the Load
        /// </summary>
        /// <param name="deltatime"> The deltatime.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TaskLoad(double deltatime)
        {
            if (_slotChange == 0)
            {
                StartDoorClose();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: update the Elevator Movement
        ///  When complete calls open door task.
        /// </summary>
        /// <param name="timeChanged"> The time Changed.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void TaskMove(double timeChanged)
        {
            // Make sure we're not speeding
            _velocityY = LimitNumber(_velocityY, -_maxspeed, _maxspeed);

            // Move elevator
            MoveToY(YOff + _velocityY * timeChanged);

            var destinationDiff = _destinationY - YOff;
            var directionSign = Math.Sign(destinationDiff);
            var velocitySign = Math.Sign(_velocityY);
            var acceleration = 0.0;
            if (destinationDiff != 0.0)
            {
                if (directionSign == velocitySign)
                {
                    // Moving in correct direction
                    var distanceNeededToStop = DistanceNeededToAchieveSpeed(_velocityY, 0.0, _deceleration);
                    if (distanceNeededToStop * 1.05 < -Math.Abs(destinationDiff))
                    {
                        // Slow down
                        // Allow a certain factor of extra breaking, to enable a smooth breaking movement after detecting overshoot
                        var requiredDeceleration = AccelerationNeededToAchieveChangeDistance(_velocityY, 0.0, destinationDiff);
                        var deceleration = Math.Min(_deceleration * 1.1, Math.Abs(requiredDeceleration));
                        _velocityY -= directionSign * deceleration * timeChanged;
                    }
                    else
                    {
                        // Speed up (or keep max speed...)
                        acceleration = Math.Min(Math.Abs(destinationDiff * 5), _acceleration);
                        _velocityY += directionSign * acceleration * timeChanged;
                    }
                }
                else if (velocitySign == 0)
                {
                    // Standing still - should accelerate
                    acceleration = Math.Min(Math.Abs(destinationDiff * 5), _acceleration);
                    _velocityY += directionSign * acceleration * timeChanged;
                }
                else
                {
                    // Moving in wrong direction - decelerate as much as possible
                    _velocityY -= velocitySign * _deceleration * timeChanged;
                    // Make sure we don't change direction within this time step - let standstill logic handle it
                    if (Math.Sign(_velocityY) != velocitySign)
                    {
                        _velocityY = 0.0;
                    }
                }
            }

            UpdateElevatorInfo();
            if (IsMoving && Math.Abs(destinationDiff) < 0.3 && Math.Abs(_velocityY) < 3)
            {
                // Snap to destination and stop
                LockToCurrentFloor(this.GetRoundedCurrentFloor());
                //  MoveToY(_destinationY);
                _velocityY = 0.0;
                IsMoving = false;
                _nextStep?.Invoke();
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Task the Unload
        /// </summary>
        /// <param name="deltatime"> The deltatime.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TaskUnload(double deltatime)
        {
            if (_slotChange == 0)
            {
                StartLoad();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Wait the Door Close
        /// </summary>
        /// <param name="deltaTime"> The delta Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void WaitDoorClose(double deltaTime)
        {
            var doors = Doors[CurrentFloorNumber];
            doors.Update(deltaTime);
            if (doors.State == View.Doors.DoorState.CLOSED)
            {
                _currentTask = null;
                _nextStep?.Invoke();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Wait the Door Open
        /// </summary>
        /// <param name="deltaTime"> The delta Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void WaitDoorOpen(double deltaTime)
        {
            var doors = Doors[CurrentFloorNumber];
            doors.Update(deltaTime);
            if (doors.State == View.Doors.DoorState.OPEN)
            {
                _currentTask = null;
                _nextStep?.Invoke();
            }
        }


    }
}



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   elevator extension
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
using System.Windows.Threading;
using Lift;
using LiftLib.View;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  The PCElevator Control Class definition.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PCElevatorControl : IPCElevator
    {

        /// <summary>
        ///  Get Context.
        /// </summary>
        private PContext Context { get; }

        /// <summary>
        ///  Get/Set Current Floor Direction.
        /// </summary>
        private PField CurrentFloorDirection { get; set; }

        /// <summary>
        ///  Get/Set Current Floor Number.
        /// </summary>
        private PField CurrentFloorNumber { get; set; }

        /// <summary>
        ///  Get/Set Down Floor Buttons.
        /// </summary>
        private PField DownFloorButtons { get; set; }

        /// <summary>
        ///  Get/Set Floor Selection.
        /// </summary>
        private PField FloorSelection { get; set; }

        /// <summary>
        ///  Get/Set Up Floor Buttons.
        /// </summary>
        private PField UpFloorButtons { get; set; }
        /// <summary>
        ///  Get/Set  building.
        /// </summary>
        private IBuilding _building { get; set; }

        /// <summary>
        ///  Get/Set  world Controller.
        /// </summary>
        private WorldController _worldController { get; set; }

        /// <summary>
        ///  The down Floor Buttons.
        /// </summary>
        private PCBArray _downFloorButtons = new PCBArray();

        /// <summary>
        ///  The  elevator user object in pseudo code.
        /// </summary>
        private PObject _elevatorUser;

        /// <summary>
        ///  The  elevator callback function.
        /// </summary>
        private static PFunction _elevatorFunction;

        /// <summary>
        ///  The  floor Count.
        /// </summary>
        private int _floorCount;

        /// <summary>
        ///  The floor Selection.
        /// </summary>
        private PCBArray _floorSelection = new PCBArray();

        /// <summary>
        ///  The  is First.
        /// </summary>
        private bool _isFirst;

        /// <summary>
        ///  The up Floor Buttons.
        /// </summary>
        private PCBArray _upFloorButtons = new PCBArray();

       // private PExpressionReference _call;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor:
        /// </summary>
        /// <param name="context"> The context.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PCElevatorControl(PContext context)
        {
            Context = context;
            Context.Elevator = this;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Set the Fields
        /// </summary>
        /// <param name="floorStops">             The floor Stops.</param>
        /// <param name="floorButtonPressedDown"> The floor Button Pressed Down.</param>
        /// <param name="floorButtonPressedUp">   The floor Button Pressed Up.</param>
        /// <param name="currentFloor">           The current Floor.</param>
        /// <param name="currentDirection">       The current Direction.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetFields(PField floorStops, PField floorButtonPressedDown, PField floorButtonPressedUp,PField currentFloor, PField currentDirection)
        {
            FloorSelection        = floorStops;
            DownFloorButtons      = floorButtonPressedDown;
            UpFloorButtons        = floorButtonPressedUp;
            CurrentFloorNumber    = currentFloor;
            CurrentFloorDirection = currentDirection;

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: initialise
        /// </summary>
        /// <param name="floorCount">    The floor Count.</param>
        /// <param name="elevatorCount"> The elevator Count.</param>
        /// <param name="maxPassengers"> The maximum Passengers.</param>
        /// <param name="spawnRate">     The spawn Rate.</param>
        /// <param name="maxTime">       The maximum Time.</param>
        /// <param name="function">      The function.</param>
        /// <param name="elevator">      The elevator object.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Init(int floorCount, int elevatorCount, int maxPassengers, double spawnRate, double maxTime, PFunction function, PObject elevator)
        {
            _elevatorFunction = function;
           var token = Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { Setup(floorCount, elevatorCount, maxPassengers, spawnRate, maxTime, elevator); }));
            token.Wait();

            //_call = new PExpressionReference(Context,function.Name);

            //_call.Function  = function.Function;
            //_call.Module    = function;
            //_call.Name      = function.Name;
            //_call.Args      = new PExpressionList(Context);
            //_call.Args.Expressions.Add(new PExpressionReference(Context, "this"));
            //var instance    = new PVariable(Context, false);
            //instance.Value  = new Accumulator(elevator);
            //_call.Variable  = instance;
            //_call.IsCall    = true;


            // setup data for fields (override to set)
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Is the Running
        /// </summary>
        /// <returns>
        ///  True if successful
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsRunning()
        {
            return !_worldController.ChallengeEnded;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: my the Function
        /// </summary>
        /// <param name="currentfloornumber"> The currentfloornumber.</param>
        /// <param name="currentdirection">   The currentdirection.</param>
        /// <param name="floorSelection">     The floor Selection.</param>
        /// <param name="upfloorButtons">     The upfloor Buttons.</param>
        /// <param name="downFloorButtons">   The downfloorbuttons.</param>
        /// <returns>
        ///  The next floor.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int myFunction(int currentfloornumber, int currentdirection, bool[] floorSelection, bool[] upfloorButtons, bool[] downFloorButtons)
        {
            // setup arrays on stack
            if (_elevatorUser != null)
            {
                var fields = _elevatorUser.Fields;
                if (_isFirst)
                {
                    _isFirst = false;

                    _downFloorButtons.Values = downFloorButtons;
                    _upFloorButtons.Values = upfloorButtons;
                    _floorSelection.Values = floorSelection;

                    {
                        fields[FloorSelection.Variable.ValueOffset].Value   = new Accumulator(_floorSelection);
                        fields[UpFloorButtons.Variable.ValueOffset].Value   = new Accumulator(_upFloorButtons);
                        fields[DownFloorButtons.Variable.ValueOffset].Value = new Accumulator(_downFloorButtons);
                    }
                }
                fields[CurrentFloorDirection.Variable.ValueOffset].Value = new Accumulator(currentdirection);
                fields[CurrentFloorNumber.Variable.ValueOffset].Value    = new Accumulator(currentfloornumber);
            }


            var floor = CallIndirect();
            return floor < _floorCount ? floor : _floorCount - 1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Call Indirect to the pc engine
        /// </summary>
        /// <returns>
        ///  The floor number to move to.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int CallIndirect()
        {
            // setup call

            var old = Context.RunStack.Count;
            Context.RunStack.ObjectRef = _elevatorUser;
            Context.RunStack.Push(_elevatorFunction);
            if (_elevatorFunction != null)
            {
                _elevatorFunction.Execute();
                // get answer

                int floor = Context.RunStack.Pop().IValue;

                // cleanup call
                Context.RunStack.ObjectRef = null;
                while (Context.RunStack.Count > old)
                {
                    Context.RunStack.Pop();
                }

                if (Context.RunStack.CallStack.Count > 0)
                {
                    Context.RunStack.CallStack.Pop();
                }
                return floor;
            }

            return 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Setup the building and world controller
        /// </summary>
        /// <param name="floorCount">    The floor Count.</param>
        /// <param name="elevatorCount"> The elevator Count.</param>
        /// <param name="maxPassengers"> The maximum Passengers.</param>
        /// <param name="spawnRate">     The spawn Rate.</param>
        /// <param name="maxTime">       The maximum Time.</param>
        /// <param name="elevator">      The elevator.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Setup(int floorCount, int elevatorCount, int maxPassengers, double spawnRate, double maxTime, PObject elevator)
        {
            _isFirst         = true;
            _floorCount      = floorCount;
            _elevatorUser    = elevator;
            _building        = Context.ElevatorWindow;
           _worldController  =  _building.Init(floorCount, elevatorCount, maxPassengers, spawnRate, maxTime,Context.Stats, myFunction);
            //     _building.Owner  = Context.MainWindow;
            // show window now
            Context.UserOutput.OutputBuilding();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Start
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Start()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => { _building.Start(); }));
                // wait for end
                 _worldController.WaitForCompletion.WaitOne(500 * 1000);

               
            }
            catch (Exception e)
            {
                Stop();
                Console.WriteLine(e);
                throw;
            }
            

            // force elevator to stop (this could have timed out or user hits stop)
            Stop();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Stop, lets the elevator thread (on wpf thread, know to stop and unhook simulator)
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Stop()
        {
            _worldController.ChallengeEnded = true;
            _worldController.IsPaused = false;
        }
    }
}


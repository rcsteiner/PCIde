////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:  Tasks for doors
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

using System.Windows.Controls;

namespace Lift.View
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  The Doors Class definition.  Partial, defines the tasks for doors.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class Doors
    {
        /// <summary>
        ///  The  actual width
        /// </summary>
        private double _dwidth;

        /// <summary>
        ///  The minimum WIDTH of door.
        /// </summary>
        private const int MIN_WIDTH = 2;

        /// <summary>
        ///  The TIME TO OPEN in seconds.  Change to make door open/close speed faster/slower 
        /// </summary>
        private const double TIME_TO_OPEN = .75;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Start the Door Close operation.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StartDoorClose()
        {
            State        = DoorState.CLOSING;
            _currentTask = TaskMove;
            TimeSpent   = 0;
            TimeToSpend = TIME_TO_OPEN;
            _nextStep    = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Start the Door Open operation.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StartDoorOpen()
        {
            State        = DoorState.OPENING;
            _currentTask = TaskMove;
            TimeSpent   = 0;
            TimeToSpend = TIME_TO_OPEN;
            _nextStep    = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: update the door movement based on current state.
        ///  next is done.
        /// </summary>
        /// <param name="deltaTime"> The time Changed.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void TaskMove(double deltaTime)
        {
            bool finished = UpdateTime(deltaTime);
            var factor    = TimeSpent / TimeToSpend;

            //   Debug.Print($"{_timeSpent:F3} secs {factor:F2}");
            switch (State)
            {
                case DoorState.CLOSED:
                    // time to move elevator
                    _dwidth = _fixWidth;
                    break;

                case DoorState.OPENING:
                    if (finished)
                    {
                        _dwidth = MIN_WIDTH;
                        State = DoorState.OPEN;
                        break;
                    }
                    _dwidth = LinearInterpolate(_fixWidth, MIN_WIDTH, factor);
                    break;

                case DoorState.OPEN:
                    _dwidth = MIN_WIDTH;
                    // elevator has arrived
                    break;

                case DoorState.CLOSING:
                    if (finished)
                    {
                        _dwidth = _fixWidth;
                        State = DoorState.CLOSED;
                        break;
                    }
                    _dwidth = LinearInterpolate(MIN_WIDTH, _fixWidth, factor);
                    break;
            }

            Left.Width  = _dwidth;
            Right.Width = _dwidth;
            Canvas.SetLeft(Right, XPos + _fixWidth * 2 - _dwidth);
            if (finished)
            {
                _currentTask = null;
                _nextStep?.Invoke();
            }
        }
    }
}



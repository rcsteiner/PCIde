////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   The options structure
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
namespace Lift
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  The Options Class definition.  Stores the run-time options.
    ///  Options are saved in the settings under:  C:\Users\USERID\AppData\Local\WPFElevator
    ///  Where USERID is your login id on this computer.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Options
    {

        /// <summary>
        ///  The elevator Count to use.  Default 1 elevator.
        /// </summary>
        public int ElevatorCount { get; set; }

        /// <summary>
        ///  The floor Count to use.  Default 4 floors.
        /// </summary>
        public int FloorCount { get; set; }

        /// <summary>
        ///  The spawn Rate, seconds between passengers appearing.  default .5 seconds.
        /// </summary>
        public double SpawnRate { get; set; }

        /// <summary>
        ///  The maximum Users allowed on the elevator, default is 4 passengers.
        /// </summary>
        public int MaxPassengers { get; set; }

        /// <summary>
        ///  The maximum Time (in seconds to run).  Default is 100 seconds.
        /// </summary>
        public double MaxTime { get; set; }




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor:  Load value from passenger saved settings.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Options(int floorCount, int elevatorCount,int maxPassengers, double spawnRate, double maxTime)
        {
            FloorCount    = floorCount;
            ElevatorCount = elevatorCount;
            MaxPassengers = maxPassengers;
            SpawnRate     = spawnRate;
            MaxTime       = maxTime;
        }
    }
}

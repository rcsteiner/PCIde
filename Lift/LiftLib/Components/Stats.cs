////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:  The statistics of this run
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/19/2018  RCS       Initial code.
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
namespace Lift
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  The Stats Class definition.  Stores statistics used to measure the performance of the elevator system.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Stats : IStats
    {

        /// <summary>
        ///  Get average Wait Time in seconds.
        /// </summary>
        public double AvgWaitTime { get { return _totalWaitTime / ((Transported > 0) ? Transported : 1); } set { } }

        /// <summary>
        ///  Get/Set Elapsed Time of the run.
        /// </summary>
        public double ElapsedTime { get; set; }

        /// <summary>
        ///  Get/Set maximum Wait Time of this run in seconds.
        /// </summary>
        public double MaxWaitTime { get; set; }

        /// <summary>
        ///  Get/Set The number of moves for all elevators.
        /// </summary>
        public int Moves { get; set; }

        /// <summary>
        ///  Get/Set The number of passengers Transported by the elevators.
        /// </summary>
        public int Transported { get; set; }

        /// <summary>
        ///  Get The number of passengers Transported per second.
        /// </summary>
        public double TransportedPerSec { get { return Transported / (ElapsedTime > 0 ? ElapsedTime : 1);  } set { }}

        /// <summary>
        ///  The  total Wait Time, used to compute average.
        /// </summary>
        private double _totalWaitTime;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor: Initializes to 0.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Stats()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Clear the stats. to zero.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Clear()
        {
            Transported    = 0;
            _totalWaitTime = 0;
            ElapsedTime    = 0;
            MaxWaitTime    = 0;
            Moves          = 0;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Update, force update in view
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Update()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Add the Wait Time, set value updates. 
        ///   Transported is incremented
        ///   _totalWaitTime is adjusted
        ///   MaxWaitTime is computed.
        /// </summary>
        /// <param name="waitTime"> The wait Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddWaitTime(double waitTime)
        {
            _totalWaitTime += waitTime;
            if (waitTime > MaxWaitTime)
            {
                MaxWaitTime = waitTime;
            }
            ++Transported;
        }
    }
}

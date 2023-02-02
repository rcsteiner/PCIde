////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   Stats interface
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/27/2018  RCS       Initial code.
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
    ///  The IStats Interface definition.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface IStats
    {
        /// <summary>
        ///  Get average Wait Time in seconds.
        /// </summary>
        double AvgWaitTime { get; set; }

        /// <summary>
        ///  Get/Set Elapsed Time of the run.
        /// </summary>
        double ElapsedTime { get; set; }

        /// <summary>
        ///  Get/Set maximum Wait Time of this run in seconds.
        /// </summary>
        double MaxWaitTime { get; set; }

        /// <summary>
        ///  Get/Set The number of moves for all elevators.
        /// </summary>
        int Moves { get; set; }

        /// <summary>
        ///  Get/Set The number of passengers Transported by the elevators.
        /// </summary>
        int Transported { get; set; }

        /// <summary>
        ///  Get The number of passengers Transported per second.
        /// </summary>
        double TransportedPerSec { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Add the Wait Time, set value updates.
        ///  Transported is incremented
        ///  _totalWaitTime is adjusted
        ///  MaxWaitTime is computed.
        /// </summary>
        /// <param name="waitTime"> The wait Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void AddWaitTime(double waitTime);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Clear the stats. to zero.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Clear();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Update, force update in view 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update();
    }
}

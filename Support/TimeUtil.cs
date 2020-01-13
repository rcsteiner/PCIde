////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the time util class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/25/2015   rcs     Initial Implementation
//  =====================================================================================================
//
// Copyright:
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;

namespace ZCore.Util
{
   ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   /// <summary>
   /// Time util. 
   /// </summary>
   ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   public static class TimeUtil
    {

        /// <summary>
        /// The ticks to seconds conversion.
        /// </summary>
        private const double TICKS_TO_SECONDS = 1E-7;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Elapsed seconds since the time passed in.
        /// </summary>
        /// <param name="time"> The time. </param>
        /// <returns>
        /// The elapsed time since the parameter time.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static double ElapsedSeconds(this DateTime time)
        {
            return (DateTime.Now.Ticks - time.Ticks) * TICKS_TO_SECONDS;
        }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Truncates.
    /// </summary>
    /// <param name="date">         The date. </param>
    /// <param name="resolution">   The resolution. </param>
    /// <returns>
    /// .
    /// </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static DateTime Truncate(this DateTime date, long resolution)
    {
        return new DateTime(date.Ticks - (date.Ticks % resolution), date.Kind);
    }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the natural string comparer class
//
// Author:
//  Rcst
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   10/21/2015   rcs     Initial Implementation
//  =====================================================================================================
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Natural string comparer. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class NaturalStringComparer : IComparer<string>
    {
        /// <summary>
        /// The reqular expression to separate parts.
        /// </summary>
        private static readonly Regex _re = new Regex(@"(?<=\D)(?=\d)|(?<=\d)(?=\D)", RegexOptions.Compiled);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compares two string objects to determine their relative ordering.
        /// </summary>
        /// <param name="x">    String to be compared. </param>
        /// <param name="y">    String to be compared. </param>
        /// <returns>
        /// Negative if 'x' is less than 'y', 0 if they are equal, or positive if it is greater.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int Compare(string x, string y)
        {
            x = x.ToLower();
            y = y.ToLower();
            if (string.Compare(x, 0, y, 0, Math.Min(x.Length, y.Length)) == 0)
            {
                if (x.Length == y.Length)
                {
                    return 0;
                }
                return x.Length < y.Length ? -1 : 1;
            }
            var a = _re.Split(x);
            var b = _re.Split(y);
            int i = 0;
            while (true)
            {
                int r = PartCompare(a[i], b[i]);
                if (r != 0)
                {
                    return r;
                }
                ++i;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Part compare.
        /// </summary>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static int PartCompare(string x, string y)
        {
            int a, b;
            if (int.TryParse(x, out a) && int.TryParse(y, out b))
            {
                return a.CompareTo(b);
            }
            return x.CompareTo(y);
        }
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:  Bool backup array
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
using System.Collections.Generic;

namespace pc
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  The PCBArray Class definition.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PCBArray : IPCArray
    {
        private bool[] _values;

        /// <summary>
        ///  The dimension array.
        /// </summary>
        public List<int> Dimensions { get; set; }

        /// <summary>
        ///  Get Items array
        /// </summary>
        public Accumulator[] Items {get{return null;}}

        /// <summary>
        ///  Get Length.
        /// </summary>
        public int Length {get { return Values.Length; }}

        /// <summary>
        ///  Gets the rank.
        /// </summary>
        public int Rank { get { return Dimensions.Count; }}

        /// <summary>
        ///  Get/Set this.
        /// </summary>
        public Accumulator this[int index] { get { return new Accumulator(Values[index]); } set { Values[index] = value.ConvertToBool(); }}

        /// <summary>
        ///  Get/Set Values.
        /// </summary>
        public bool[] Values
        {
            get { return _values; }
            set { _values = value; Dimensions.Add(_values.Length); }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor:
        /// </summary>
        /// <param name="dimensions"> [optional=1] The dimensions.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PCBArray(int dimensions=1)
        {
            Dimensions = new List<int>(1);
        }
    }
}

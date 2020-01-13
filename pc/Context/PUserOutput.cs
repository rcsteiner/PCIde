////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the error class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/26/2015   rcs     Initial Implementation
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

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  UserOutput.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PUserOutput : IUserOutput
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="context"> The context.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PUserOutput(PContext context)
        {
            context.UserOutput = this;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Outputs any text to the output system.
        /// </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="args">   A variable-length parameters list containing arguments.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Output(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Output the Building
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OutputBuilding()
        {
            Console.WriteLine("Elevator Building: ");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Output a Chart
        /// </summary>
        /// <param name="data">    The data.</param>
        /// <param name="title">   The title.</param>
        /// <param name="xlabels"> The xlabels.</param>
        /// <param name="ylabels"> The ylabels.</param>
        /// <param name="xtitle">  The xtitle.</param>
        /// <param name="ytitle">  The ytitle.</param>
        /// <param name="gridx">   True if gridx.</param>
        /// <param name="gridy">   True if gridy.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OutputChart(double[] data, string title, string[] xlabels, string[] ylabels, string xtitle, string ytitle,bool gridx, bool gridy)
        {
            Console.WriteLine("Chart: "+title);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Outputs any text to the output system.
        /// </summary>
        /// <param name="text"> The text.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OutputLine(string text)
        {
            Console.WriteLine(text);
        }
    }
}

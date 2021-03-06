﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the service user class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/31/2015   rcs     Initial Implementation
//  =====================================================================================================
//
//  BSD 3-Clause License
//  Copyright (c) 2020, Robert C. Steiner
//  All rights reserved.
//  Redistribution and use in source and binary forms, with or without   modification,
//  are permitted provided that the following conditions are met:
//
//  1. Redistributions of source code must retain the above copyright notice, this
//  list of conditions and the following disclaimer.
//
//  2. Redistributions in binary form must reproduce the above copyright notice,
//  this list of conditions and the following disclaimer in the documentation
//  and/or other materials provided with the distribution.
//
//  3. Neither the name of the copyright holder nor the names of its
//  contributors may be used to endorse or promote products derived from
//  this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
//  FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//  DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
//  CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//  OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Ide.Controller;
using pc;

namespace Ide.Services
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Service user.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ServiceUser : ServiceBase, IUserOutput, IUserInput
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="controller"> The controller.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ServiceUser(ControllerIde controller) : base(controller)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Input integer.
        /// </summary>
        /// <returns>
        ///  integer or zero
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public char InputChar()
        {
            var s = InputString();
            return s.Length > 0 ? s[0] : '\0';
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Input double.
        /// </summary>
        /// <returns>
        ///  double or zero.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double InputDouble()
        {
            var t = InputString();
            double d;

            return double.TryParse(t, out d) ? d : 0.0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Input integer.
        /// </summary>
        /// <returns>
        ///  integer or zero.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public long InputInteger()
        {
            var t = InputString();
            long d;

            return long.TryParse(t, out d) ? d : 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Input string.
        /// </summary>
        /// <returns>
        ///  reads till CR.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string InputString()
        {
            return Controller.VMOutput.InputString();
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
            Controller.OutputWrite(string.Format(format, args));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Output the Chart
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
            Controller.OutputChart(data, title, xlabels, ylabels, xtitle, ytitle, gridx, gridy);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Output the Building
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OutputBuilding()
        { 
            Controller.OutputBuilding();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Outputs any text to the output system.
        /// </summary>
        /// <param name="text"> The text.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OutputLine(string text)
        {
            Controller.OutputWriteLine(text);
        }
    }
}


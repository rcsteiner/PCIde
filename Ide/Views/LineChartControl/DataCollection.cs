////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the data collection class
//
// Author:
//  Robert C. Steiner
//
// History:
//  ===================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------
//   2/4/2014   rcs     Initial implementation.
//  ===================================================================================
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;

namespace LineCharts
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Collection of data. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class DataCollection
    {
        /// <summary>
        /// Gets or sets a list of data.
        /// </summary>
        public List<DataSeries> DataList { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public DataCollection()
        {
            DataList = new List<DataSeries>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a lines. 
        /// </summary>
        /// <param name="cs">   The create struct. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddLines(ChartStyle cs)
        {
            int j = 0;
            foreach (DataSeries ds in DataList)
            {
                if (ds.SeriesName == "Default Name")
                {
                    ds.SeriesName = "DataSeries" + j;
                }
                ds.AddLinePattern();
                for (int i = 0; i < ds.LineSeries.Points.Count; i++)
                {
                    ds.LineSeries.Points[i] = cs.NormalizePoint(ds.LineSeries.Points[i]);
                    ds.Symbols.AddSymbol(cs.ChartCanvas, ds.LineSeries.Points[i]);
                }
                cs.ChartCanvas.Children.Add(ds.LineSeries);
                j++;
            }
        }
    }
}
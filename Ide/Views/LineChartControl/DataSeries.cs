////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the data series class
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
using System.Windows.Media;
using System.Windows.Shapes;

namespace LineCharts
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Data series. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class DataSeries
    {
        /// <summary>
        /// Gets or sets the symbols.
        /// </summary>
        public Symbols Symbols { get; set; }

        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        public Brush LineColor { get; set; }

        /// <summary>
        /// Gets or sets the line series.
        /// </summary>
        public Polyline LineSeries { get; set; }

        /// <summary>
        /// Gets or sets the line thickness.
        /// </summary>
        public double LineThickness { get; set; }

        /// <summary>
        /// Gets or sets the line pattern.
        /// </summary>
        public LinePatternEnum LinePattern { get; set; }

        /// <summary>
        /// Gets or sets the name of the series.
        /// </summary>
        public string SeriesName { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public DataSeries()
        {
            SeriesName    = "Default Name";
            LineThickness = 1;
            LineSeries    = new Polyline();
            LineColor     = Brushes.Black;
            Symbols       = new Symbols();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds  line pattern.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddLinePattern()
        {
            LineSeries.Stroke          = LineColor;
            LineSeries.StrokeThickness = LineThickness;

            switch (LinePattern)
            {
                case LinePatternEnum.Dash:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[2] {4, 3});
                    break;
                case LinePatternEnum.Dot:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[2] {1, 2});
                    break;
                case LinePatternEnum.DashDot:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[4] {4, 2, 1, 2});
                    break;
                case LinePatternEnum.None:
                    LineSeries.Stroke = Brushes.Transparent;
                    break;
            }
        }
    }
}
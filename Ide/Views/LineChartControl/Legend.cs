////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the legend class
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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LineCharts
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Legend.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Legend
    {
        /// <summary>
        ///     Gets or sets the legend position.
        /// </summary>
        public LegendPositionEnum LegendPosition { get; set; }

        /// <summary>
        ///     Gets or sets the legend canvas.
        /// </summary>
        public Canvas LegendCanvas { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the legend enable.
        /// </summary>
        public bool LegendEnable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the border is shown.
        /// </summary>
        public bool ShowBorder { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Legend()
        {
            LegendEnable     = false;
            ShowBorder       = true;
            LegendPosition = LegendPositionEnum.NorthEast;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Adds a legend to 'dc'.
        /// </summary>
        /// <param name="canvas">   The canvas. </param>
        /// <param name="dc">       The device-context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddLegend(Canvas canvas, DataCollection dc)
        {
            if (dc.DataList.Count < 1 || !LegendEnable)
            {
                return;
            }

            var tb = new TextBlock();
            int n = 0;
            var legendLabels = new string[dc.DataList.Count];
            foreach (DataSeries ds in dc.DataList)
            {
                legendLabels[n] = ds.SeriesName;
                n++;
            }

            double legendWidth = 0;
            var size = new Size(0, 0);
            for (int i = 0; i < legendLabels.Length; i++)
            {
                tb.Text = legendLabels[i];
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;
                if (legendWidth < size.Width)
                {
                    legendWidth = size.Width;
                }
            }

            legendWidth += 50;
            LegendCanvas.Width  = legendWidth + 5;
            double legendHeight = 17*dc.DataList.Count;
            double sx           = 6;
            double sy           = 0;
            double textHeight   = size.Height;
            double lineLength   = 34;
            var legendRect      = new Rectangle();
            legendRect.Stroke   = Brushes.Black;
            legendRect.Fill     = Brushes.White;
            legendRect.Width    = legendWidth;
            legendRect.Height   = legendHeight;

            if (ShowBorder)
            {
                LegendCanvas.Children.Add(legendRect);
            }
            Panel.SetZIndex(LegendCanvas, 10);

            n = 1;
            foreach (DataSeries ds in dc.DataList)
            {
               // double xSymbol = sx + lineLength/2;
                double xText = 2*sx + lineLength;
                double yText = n*sy + (2*n - 1)*textHeight/2;
                var line = new Line();
                AddLinePattern(line, ds);
                line.X1 = sx;
                line.Y1 = yText;
                line.X2 = sx + lineLength;
                line.Y2 = yText;
                LegendCanvas.Children.Add(line);
                ds.Symbols.AddSymbol(LegendCanvas,
                    new Point(0.5*(line.X2 - line.X1 + ds.Symbols.SymbolSize) + 1, line.Y1));

                tb = new TextBlock();
                tb.Text = ds.SeriesName;
                LegendCanvas.Children.Add(tb);
                Canvas.SetTop(tb, yText - size.Height/2);
                Canvas.SetLeft(tb, xText);
                n++;
            }
            LegendCanvas.Width = legendRect.Width;
            LegendCanvas.Height = legendRect.Height;

            double offSet = 7.0;
            switch (LegendPosition)
            {
                case LegendPositionEnum.East:
                    Canvas.SetRight(LegendCanvas, offSet);
                    Canvas.SetTop(LegendCanvas, canvas.Height/2 - legendRect.Height/2);
                    break;
                case LegendPositionEnum.NorthEast:
                    Canvas.SetTop(LegendCanvas, offSet);
                    Canvas.SetRight(LegendCanvas, offSet);
                    break;
                case LegendPositionEnum.North:
                    Canvas.SetTop(LegendCanvas, offSet);
                    Canvas.SetLeft(LegendCanvas, canvas.Width/2 - legendRect.Width/2);
                    break;
                case LegendPositionEnum.NorthWest:
                    Canvas.SetTop(LegendCanvas, offSet);
                    Canvas.SetLeft(LegendCanvas, offSet);
                    break;
                case LegendPositionEnum.West:
                    Canvas.SetTop(LegendCanvas, canvas.Height/2 - legendRect.Height/2);
                    Canvas.SetLeft(LegendCanvas, offSet);
                    break;
                case LegendPositionEnum.SouthWest:
                    Canvas.SetBottom(LegendCanvas, offSet);
                    Canvas.SetLeft(LegendCanvas, offSet);
                    break;
                case LegendPositionEnum.South:
                    Canvas.SetBottom(LegendCanvas, offSet);
                    Canvas.SetLeft(LegendCanvas, canvas.Width/2 - legendRect.Width/2);
                    break;
                case LegendPositionEnum.SouthEast:
                    Canvas.SetBottom(LegendCanvas, offSet);
                    Canvas.SetRight(LegendCanvas, offSet);
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Adds a line pattern to 'ds'.
        /// </summary>
        /// <param name="line"> The line. </param>
        /// <param name="ds">   The ds. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void AddLinePattern(Line line, DataSeries ds)
        {
            line.Stroke = ds.LineColor;
            line.StrokeThickness = ds.LineThickness;

            switch (ds.LinePattern)
            {
                case LinePatternEnum.Dash:
                    line.StrokeDashArray = new DoubleCollection(new double[2] {4, 3});
                    break;
                case LinePatternEnum.Dot:
                    line.StrokeDashArray = new DoubleCollection(new double[2] {1, 2});
                    break;
                case LinePatternEnum.DashDot:
                    line.StrokeDashArray = new DoubleCollection(new double[4] {4, 2, 1, 2});
                    break;
                case LinePatternEnum.None:
                    line.Stroke = Brushes.Transparent;
                    break;
            }
        }
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the chart style gridlines class
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
    /// Chart style gridlines. 
    /// </summary>
    /// <seealso cref="T:LineCharts.ChartStyle"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ChartStyleGridlines : ChartStyle
    {
        /// <summary>
        /// The bottom offset.
        /// </summary>
        private double bottomOffset = 15;

        /// <summary>
        /// The gridline.
        /// </summary>
        private Line gridline = new Line();

        /// <summary>
        /// The left offset.
        /// </summary>
        private double leftOffset = 20;

        /// <summary>
        /// The right offset.
        /// </summary>
        private double rightOffset = 10;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string XLabel { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string YLabel { get; set; }

        /// <summary>
        /// Gets or sets the gridline pattern.
        /// </summary>
        public GridlinePatternEnum GridlinePattern { get; set; }

        /// <summary>
        /// Gets or sets the tick.
        /// </summary>
        public double XTick { get; set; }

        /// <summary>
        /// Gets or sets the tick.
        /// </summary>
        public double YTick { get; set; }

        /// <summary>
        /// Gets or sets the color of the gridline.
        /// </summary>
        public Brush GridlineColor { get; set; }

        /// <summary>
        /// Gets or sets the text canvas.
        /// </summary>
        public Canvas TextCanvas { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object is x coordinate grid.
        /// </summary>
        public bool IsXGrid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object is y coordinate grid.
        /// </summary>
        public bool IsYGrid { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ChartStyleGridlines()
        {
            IsYGrid       = true;
            IsXGrid       = true;
            GridlineColor = Brushes.LightGray;
            YTick         = 2;
            XTick         = 2;
            Title         = "Title";
            XLabel        = "X Axis";
            YLabel        = "Y Axis";
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a chart style.
        /// </summary>
        /// <param name="tbTitle">  The tb title. </param>
        /// <param name="tbXLabel"> The tb x coordinate label. </param>
        /// <param name="tbYLabel"> The tb y coordinate label. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddChartStyle(TextBlock tbTitle, TextBlock tbXLabel, TextBlock tbYLabel)
        {
            var pt = new Point();
            var tick = new Line();
            double offset = 0;
            double dx, dy;
            var tb = new TextBlock();

            //  determine right offset:
            tb.Text = Xmax.ToString();
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Size size = tb.DesiredSize;
            rightOffset = size.Width/2 + 2;

            // Determine left offset:
            for (dy = Ymin; dy <= Ymax; dy += YTick)
            {
                pt = NormalizePoint(new Point(Xmin, dy));
                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.TextAlignment = TextAlignment.Right;
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;
                if (offset < size.Width)
                    offset = size.Width;
            }
            leftOffset = offset + 5;

            Canvas.SetLeft(ChartCanvas, leftOffset);
            Canvas.SetBottom(ChartCanvas, bottomOffset);
            ChartCanvas.Width  = Math.Abs(TextCanvas.Width - leftOffset - rightOffset);
            ChartCanvas.Height = Math.Abs(TextCanvas.Height - bottomOffset - size.Height/2);
            var chartRect      = new Rectangle();
            chartRect.Stroke   = Brushes.Black;
            chartRect.Width    = ChartCanvas.Width;
            chartRect.Height   = ChartCanvas.Height;
            ChartCanvas.Children.Add(chartRect);

            // Create vertical gridlines:
            if (IsYGrid)
            {
                for (dx = Xmin + XTick; dx < Xmax; dx += XTick)
                {
                    gridline = new Line();
                    AddLinePattern();
                    gridline.X1 = NormalizePoint(new Point(dx, Ymin)).X;
                    gridline.Y1 = NormalizePoint(new Point(dx, Ymin)).Y;
                    gridline.X2 = NormalizePoint(new Point(dx, Ymax)).X;
                    gridline.Y2 = NormalizePoint(new Point(dx, Ymax)).Y;
                    ChartCanvas.Children.Add(gridline);
                }
            }

            // Create horizontal gridlines:
            if (IsXGrid)
            {
                for (dy = Ymin + YTick; dy < Ymax; dy += YTick)
                {
                    gridline = new Line();
                    AddLinePattern();
                    gridline.X1 = NormalizePoint(new Point(Xmin, dy)).X;
                    gridline.Y1 = NormalizePoint(new Point(Xmin, dy)).Y;
                    gridline.X2 = NormalizePoint(new Point(Xmax, dy)).X;
                    gridline.Y2 = NormalizePoint(new Point(Xmax, dy)).Y;
                    ChartCanvas.Children.Add(gridline);
                }
            }

            // Create x-axis tick marks:
            for (dx = Xmin; dx <= Xmax; dx += XTick)
            {
                pt = NormalizePoint(new Point(dx, Ymin));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X;
                tick.Y1 = pt.Y;
                tick.X2 = pt.X;
                tick.Y2 = pt.Y - 5;
                ChartCanvas.Children.Add(tick);

                tb = new TextBlock();
                tb.Text = dx.ToString();
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;
                TextCanvas.Children.Add(tb);
                Canvas.SetLeft(tb, leftOffset + pt.X - size.Width/2);
                Canvas.SetTop(tb, pt.Y + 2 + size.Height/2);
            }

            // Create y-axis tick marks:
            for (dy = Ymin; dy <= Ymax; dy += YTick)
            {
                pt = NormalizePoint(new Point(Xmin, dy));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X;
                tick.Y1 = pt.Y;
                tick.X2 = pt.X + 5;
                tick.Y2 = pt.Y;
                ChartCanvas.Children.Add(tick);

                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;
                TextCanvas.Children.Add(tb);
                Canvas.SetRight(tb, ChartCanvas.Width + rightOffset + 3);
                Canvas.SetTop(tb, pt.Y);
            }

            // Add title and labels:
            tbTitle.Text = Title;
            tbXLabel.Text = XLabel;
            tbYLabel.Text = YLabel;
            tbXLabel.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
            tbTitle.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds  line pattern.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddLinePattern()
        {
            gridline.Stroke = GridlineColor;
            gridline.StrokeThickness = 1;

            switch (GridlinePattern)
            {
                case GridlinePatternEnum.Dash:
                    gridline.StrokeDashArray = new DoubleCollection(new double[2] {4, 3});
                    break;
                case GridlinePatternEnum.Dot:
                    gridline.StrokeDashArray = new DoubleCollection(new double[2] {1, 2});
                    break;
                case GridlinePatternEnum.DashDot:
                    gridline.StrokeDashArray = new DoubleCollection(new double[4] {4, 2, 1, 2});
                    break;
            }
        }
    }
}
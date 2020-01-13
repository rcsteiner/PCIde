////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the automatic ticks.xaml class
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
using System.Windows.Media;

namespace LineCharts
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interaction logic for AutomaticTicks.xaml.
    /// </summary>
    /// <seealso cref="T:System.Windows.Window"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class AutomaticTicks : Window
    {
        /// <summary>
        /// The create struct.
        /// </summary>
        private ChartStyleGridlines cs;

        /// <summary>
        /// The device-context.
        /// </summary>
        private DataCollection dc;

        /// <summary>
        /// The ds.
        /// </summary>
        private DataSeries ds;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public AutomaticTicks()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds  chart.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void AddChart()
        {
            cs = new ChartStyleGridlines();
            dc = new DataCollection();
            ds = new DataSeries();

            cs.ChartCanvas     = chartCanvas;
            cs.TextCanvas      = textCanvas;
            cs.Title           = "Sine and Cosine Chart";
            cs.Xmin            = 0;
            cs.Xmax            = 7;
            cs.Ymin            = -1.1;
            cs.Ymax            = 1.1;
            cs.GridlinePattern = GridlinePatternEnum.Dot;
            cs.GridlineColor   = Brushes.Black;
            cs.AddChartStyle(tbTitle, tbXLabel, tbYLabel);

            // Draw Sine curve:
            ds.LineColor = Brushes.Blue;
            ds.LineThickness = 2;
            double dx = (cs.Xmax - cs.Xmin)/100;

            for (double x = cs.Xmin; x <= cs.Xmax + dx; x += dx)
            {
                double y = Math.Exp(-0.3*Math.Abs(x))*Math.Sin(x);
                ds.LineSeries.Points.Add(new Point(x, y));
            }
            dc.DataList.Add(ds);

            // Draw cosine curve:
            ds = new DataSeries();
            ds.LineColor = Brushes.Red;
            ds.LinePattern = LinePatternEnum.DashDot;
            ds.LineThickness = 2;

            for (double x = cs.Xmin; x <= cs.Xmax + dx; x += dx)
            {
                double y = Math.Exp(-0.3*Math.Abs(x))*Math.Cos(x);
                ds.LineSeries.Points.Add(new Point(x, y));
            }
            dc.DataList.Add(ds);
            dc.AddLines(cs);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by chartGrid for size changed events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Size changed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void chartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            tbDate.Text = DateTime.Now.ToShortDateString();
            textCanvas.Width = chartGrid.ActualWidth;
            textCanvas.Height = chartGrid.ActualHeight;
            chartCanvas.Children.Clear();
            textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
            AddChart();
        }
    }
}
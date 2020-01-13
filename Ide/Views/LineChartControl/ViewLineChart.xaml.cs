////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the line chart.xaml class
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
using LineChartControl;
using Views;
using ViewBase = Views.ViewBase;

namespace LineCharts
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Interaction logic for XYChart.xaml.
    /// </summary>
    /// <seealso cref="T:System.Windows.Window" />
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class ViewLineChart : ViewBase
    {
        /// <summary>
        ///  Interaction logic for Building, this owns the canvas and the stats view.
        /// </summary>
        private static LineChartControlLib _lineChart = new LineChartControlLib();

        /// <summary>
        ///  Get My Line Chart.
        /// </summary>
        public LineChartControlLib MyLineChart {get { return _lineChart; }}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewLineChart()
        {
            InitializeComponent();
            Disconnect(_lineChart);
            RootGrid.Children.Insert(0, _lineChart);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Event handler. Called by rootGrid for size changed events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Size changed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _lineChart.Width = RootGrid.ActualWidth;
            _lineChart.Height = RootGrid.ActualHeight;
            MyLineChart.Refresh();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Refresh()
        {
            MyLineChart.Width = RootGrid.ActualWidth;
            MyLineChart.Height = RootGrid.ActualHeight;
            MyLineChart.AddChart();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Adds  data.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddData()
        {
            //MyLineChart.DataSet.DataList.Clear();

            //// Draw Sine curve:
            //DataSeries ds          = new DataSeries();
            //ds.LineColor           = Brushes.Blue;
            //ds.LineThickness       = 1;
            //ds.SeriesName          = "Sine";
            //ds.Symbols.SymbolType  = SymbolTypeEnum.Circle;
            //ds.Symbols.BorderColor = ds.LineColor;
            //for (int i             = 0; i < 170; i++)
            //{
            //    double x = i/5.0;
            //    double y = Math.Sin(x);
            //    ds.LineSeries.Points.Add(new Point(x, y));
            //}
            //MyLineChart.DataSet.DataList.Add(ds);

            //// Draw cosine curve:
            //ds                     = new DataSeries();
            //ds.LineColor           = Brushes.Red;
            //ds.SeriesName          = "Cosine";
            //ds.Symbols.SymbolType  = SymbolTypeEnum.OpenDiamond;
            //ds.Symbols.BorderColor = ds.LineColor;
            //ds.LinePattern         = LinePatternEnum.DashDot;
            //ds.LineThickness       = 2;
            //for (int i             = 0; i < 70; i++)
            //{
            //    double x = i/5.0;
            //    double y = Math.Cos(x);
            //    ds.LineSeries.Points.Add(new Point(x, y));
            //}
            //MyLineChart.DataSet.DataList.Add(ds);

            //MyLineChart.LegendEnable = true;
            //MyLineChart.LegendPosition = LegendPositionEnum.NorthEast;
        }

    }
}
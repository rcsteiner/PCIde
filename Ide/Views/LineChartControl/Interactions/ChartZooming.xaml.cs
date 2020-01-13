////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the chart zooming.xaml class
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LineCharts
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interaction logic for ChartZooming.xaml.
    /// </summary>
    /// <seealso cref="T:System.Windows.Window"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class ChartZooming : Window
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

        /// <summary>
        /// The end point.
        /// </summary>
        private Point endPoint;

        /// <summary>
        /// The rubber band.
        /// </summary>
        private Shape rubberBand;

        /// <summary>
        /// The start point.
        /// </summary>
        private Point startPoint;

        /// <summary>
        /// The xmax 0.
        /// </summary>
        private double xmax0 = 7;

        /// <summary>
        /// The xmin 0.
        /// </summary>
        private double xmin0 = 0;

        /// <summary>
        /// The ymax 0.
        /// </summary>
        private double ymax0 = 1.5;

        /// <summary>
        /// The ymin 0.
        /// </summary>
        private double ymin0 = -1.5;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ChartZooming()
        {
            InitializeComponent();
            cs = new ChartStyleGridlines();
            cs.Xmin = xmin0;
            cs.Xmax = xmax0;
            cs.Ymin = ymin0;
            cs.Ymax = ymax0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a chart.
        /// </summary>
        /// <param name="xmin"> The xmin. </param>
        /// <param name="xmax"> The xmax. </param>
        /// <param name="ymin"> The ymin. </param>
        /// <param name="ymax"> The ymax. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void AddChart(double xmin, double xmax, double ymin, double ymax)
        {
            dc = new DataCollection();
            ds = new DataSeries();
            cs = new ChartStyleGridlines();

            cs.ChartCanvas = chartCanvas;
            cs.TextCanvas = textCanvas;
            cs.Title = "Sine and Cosine Chart";
            cs.Xmin = xmin;
            cs.Xmax = xmax;
            cs.Ymin = ymin;
            cs.Ymax = ymax;
            cs.GridlinePattern = GridlinePatternEnum.Dot;
            cs.GridlineColor = Brushes.Black;
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
            AddChart(cs.Xmin, cs.Xmax, cs.Ymin, cs.Ymax);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the mouse left button down action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!chartCanvas.IsMouseCaptured)
            {
                startPoint = e.GetPosition(chartCanvas);
                chartCanvas.CaptureMouse();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the mouse move action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (chartCanvas.IsMouseCaptured)
            {
                endPoint = e.GetPosition(chartCanvas);
                if (rubberBand == null)
                {
                    rubberBand = new Rectangle();
                    rubberBand.Stroke = Brushes.Red;
                    chartCanvas.Children.Add(rubberBand);
                }
                rubberBand.Width = Math.Abs(startPoint.X - endPoint.X);
                rubberBand.Height = Math.Abs(startPoint.Y - endPoint.Y);
                double left = Math.Min(startPoint.X, endPoint.X);
                double top = Math.Min(startPoint.Y, endPoint.Y);
                Canvas.SetLeft(rubberBand, left);
                Canvas.SetTop(rubberBand, top);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the mouse left button up action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            double x0 = 0;
            double x1 = 1;
            double y0 = 0;
            double y1 = 1;
            endPoint = e.GetPosition(chartCanvas);

            if (endPoint.X > startPoint.X)
            {
                x0 = cs.Xmin + (cs.Xmax - cs.Xmin)*startPoint.X/chartCanvas.Width;
                x1 = cs.Xmin + (cs.Xmax - cs.Xmin)*endPoint.X/chartCanvas.Width;
            }
            else if (endPoint.X < startPoint.X)
            {
                x1 = cs.Xmin + (cs.Xmax - cs.Xmin)*startPoint.X/chartCanvas.Width;
                x0 = cs.Xmin + (cs.Xmax - cs.Xmin)*endPoint.X/chartCanvas.Width;
            }

            if (endPoint.Y < startPoint.Y)
            {
                y0 = cs.Ymin + (cs.Ymax - cs.Ymin)*(chartCanvas.Height - startPoint.Y)/chartCanvas.Height;
                y1 = cs.Ymin + (cs.Ymax - cs.Ymin)*(chartCanvas.Height - endPoint.Y)/chartCanvas.Height;
            }
            else if (endPoint.Y > startPoint.Y)
            {
                y1 = cs.Ymin + (cs.Ymax - cs.Ymin)*(chartCanvas.Height - startPoint.Y)/chartCanvas.Height;
                y0 = cs.Ymin + (cs.Ymax - cs.Ymin)*(chartCanvas.Height - endPoint.Y)/chartCanvas.Height;
            }

            chartCanvas.Children.Clear();
            textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
            AddChart(x0, x1, y0, y1);

            if (rubberBand != null)
            {
                rubberBand = null;
                chartCanvas.ReleaseMouseCapture();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the mouse right button down action.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            chartCanvas.Children.Clear();
            textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
            AddChart(xmin0, xmax0, ymin0, ymax0);
        }
    }
}
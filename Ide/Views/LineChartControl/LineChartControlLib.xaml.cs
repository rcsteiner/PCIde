////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the line chart control lib.xaml class
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
using LineCharts;

namespace LineChartControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Interaction logic for LineChartControl.xaml.
    /// </summary>
    /// 
    /// <seealso cref="T:System.Windows.Controls.UserControl"/>  
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class LineChartControlLib : UserControl
    {

        /// <summary>
        ///  Gets or sets the chart style.
        /// </summary>
        public ChartStyleGridlines ChartStyle { get; set; }

        /// <summary>
        ///  Gets or sets the data series.
        /// </summary>
        public DataSeries DataSeries   { get; set; }


        ///// <summary>
        /////  Gets or sets the data collection.
        ///// </summary>
        //public DataCollection DataSet
        //{
        //    get { return (DataCollection)GetValue(DataSetProperty); }
        //    set
        //    {
        //        SetValue(DataSetProperty, value);
        //        _dataSet = value;
        //    }
        //}

         public DataCollection DataSet { get { return _dataSet; } set { _dataSet = value; } }

        /// <summary>
        ///  Gets or sets the color of the gridline.
        /// </summary>
        public Brush GridlineColor
        {
            get { return (Brush)GetValue(GridlineColorProperty); }
            set
            {
                SetValue(GridlineColorProperty, value);
                ChartStyle.GridlineColor = value;
            }
        }

        /// <summary>
        ///  Gets or sets the gridline pattern.
        /// </summary>
        public GridlinePatternEnum GridlinePattern
        {
            get { return (GridlinePatternEnum)GetValue(GridlinePatternProperty); }
            set
            {
                SetValue(GridlinePatternProperty, value);
                ChartStyle.GridlinePattern = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether this object is x coordinate grid.
        /// </summary>
        public bool IsXGrid
        {
            get { return (bool)GetValue(IsXGridProperty); }
            set
            {
                SetValue(IsXGridProperty, value);
                ChartStyle.IsXGrid = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether this object is y coordinate grid.
        /// </summary>
        public bool IsYGrid
        {
            get { return (bool)GetValue(IsYGridProperty); }
            set
            {
                SetValue(IsYGridProperty, value);
                ChartStyle.IsYGrid = value;
            }
        }

        /// <summary>
        ///  Gets or sets the legend.
        /// </summary>
        public Legend Legend { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether this object is legend.
        /// </summary>
        public bool LegendEnable
        {
            get { return (bool)GetValue(LegendEnableProperty); }
            set
            {
                SetValue(LegendEnableProperty, value);
                Legend.LegendEnable = value;
            }
        }

        /// <summary>
        ///  Gets or sets the legend position.
        /// </summary>
        public LegendPositionEnum LegendPosition
        {
            get { return (LegendPositionEnum)GetValue(LegendPositionProperty); }
            set
            {
                SetValue(LegendPositionProperty, value);
                Legend.LegendPosition = value;
            }
        }

        /// <summary>
        ///  Gets or sets the title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
                ChartStyle.Title = value;
            }
        }

        /// <summary>
        ///  Gets or sets the label.
        /// </summary>
        public string XLabel
        {
            get { return (string)GetValue(XLabelProperty); }
            set
            {
                SetValue(XLabelProperty, value);
                ChartStyle.XLabel = value;
            }
        }

        /// <summary>
        ///  Gets or sets the xmax.
        /// </summary>
        public double XMax
        {
            get { return (double)GetValue(XMaxProperty); }
            set
            {
                SetValue(XMaxProperty, value);
                ChartStyle.Xmax = value;
            }
        }

        /// <summary>
        ///  Gets or sets the xmin.
        /// </summary>
        public double XMin
        {
            get { return (double)GetValue(XMinProperty); }
            set
            {
                SetValue(XMinProperty, value);
                ChartStyle.Xmin = value;
            }
        }

        /// <summary>
        ///  Gets or sets the tick.
        /// </summary>
        public double XTick
        {
            get { return (double)GetValue(XTickProperty); }
            set
            {
                SetValue(XTickProperty, value);
                ChartStyle.XTick = value;
            }
        }

        /// <summary>
        ///  Gets or sets the label.
        /// </summary>
        public string YLabel
        {
            get { return (string)GetValue(YLabelProperty); }
            set
            {
                SetValue(YLabelProperty, value);
                ChartStyle.YLabel = value;
            }
        }

        /// <summary>
        ///  Gets or sets the ymax.
        /// </summary>
        public double YMax
        {
            get { return (double)GetValue(YMaxProperty); }
            set
            {
                SetValue(YMaxProperty, value);
                ChartStyle.Ymax = value;
            }
        }

        /// <summary>
        ///  Gets or sets the ymin.
        /// </summary>
        public double YMin
        {
            get { return (double)GetValue(YMinProperty); }
            set
            {
                SetValue(YMinProperty, value);
                ChartStyle.Ymin = value;
            }
        }

        /// <summary>
        ///  Gets or sets the tick.
        /// </summary>
        public double YTick
        {
            get { return (double)GetValue(YTickProperty); }
            set
            {
                SetValue(YTickProperty, value);
                ChartStyle.YTick = value;
            }
        }

        /// <summary>
        ///  The legend position property.
        /// </summary>
        public static DependencyProperty DataSetProperty = DependencyProperty.Register("DataSet", typeof(DataCollection),
                    typeof(LineChartControlLib), new FrameworkPropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        ///  The gridline color property.
        /// </summary>
        public static DependencyProperty GridlineColorProperty = DependencyProperty.Register("GridlineColor",
                    typeof(Brush), typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata(Brushes.Gray, OnPropertyChanged));

        /// <summary>
        ///  The gridline pattern property.
        /// </summary>
        public static DependencyProperty GridlinePatternProperty = DependencyProperty.Register("GridlinePattern",
                    typeof(GridlinePatternEnum), typeof(LineChartControlLib), new FrameworkPropertyMetadata(GridlinePatternEnum.Solid,
                        OnPropertyChanged));

        /// <summary>
        ///  The is x coordinate grid property.
        /// </summary>
        public static DependencyProperty IsXGridProperty = DependencyProperty.Register("IsXGrid", typeof(bool),
                    typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata(true, OnPropertyChanged));

        /// <summary>
        ///  The is y coordinate grid property.
        /// </summary>
        public static DependencyProperty IsYGridProperty = DependencyProperty.Register("IsYGrid", typeof(bool),
                    typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata(true, OnPropertyChanged));

        /// <summary>
        ///  The is legend property.
        /// </summary>
        public static DependencyProperty LegendEnableProperty = DependencyProperty.Register("LegendEnable", typeof(bool),
                    typeof(LineChartControlLib), new FrameworkPropertyMetadata(false, OnPropertyChanged));

        /// <summary>
        ///  The legend position property.
        /// </summary>
        public static DependencyProperty LegendPositionProperty = DependencyProperty.Register("LegendPosition",
                    typeof(LegendPositionEnum), typeof(LineChartControlLib), new FrameworkPropertyMetadata(LegendPositionEnum.NorthEast,
                        OnPropertyChanged));

        /// <summary>
        ///  The title property.
        /// </summary>
        public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string),
                    typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata("Title", OnPropertyChanged));

        /// <summary>
        ///  The label property.
        /// </summary>
        public static DependencyProperty XLabelProperty = DependencyProperty.Register("XLabel", typeof(string),
                    typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata("X Axis", OnPropertyChanged));

        /// <summary>
        ///  The xmax property.
        /// </summary>
        public static DependencyProperty XMaxProperty = DependencyProperty.Register("XMax", typeof(double),
                    typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata(10.0, OnPropertyChanged));
        /// <summary>
        ///  The xmin property.
        /// </summary>
        public static DependencyProperty XMinProperty = DependencyProperty.Register("XMin", typeof(double),
                    typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        ///  The tick property.
        /// </summary>
        public static DependencyProperty XTickProperty = DependencyProperty.Register("XTick", typeof(double),
                    typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata(2.0, OnPropertyChanged));

        /// <summary>
        ///  The label property.
        /// </summary>
        public static DependencyProperty YLabelProperty = DependencyProperty.Register("YLabel", typeof(string),
                    typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata("Y Axis", OnPropertyChanged));

        /// <summary>
        ///  The ymax property.
        /// </summary>
        public static DependencyProperty YMaxProperty = DependencyProperty.Register("YMax", typeof(double),
                    typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata(10.0, OnPropertyChanged));

        /// <summary>
        ///  The ymin property.
        /// </summary>
        public static DependencyProperty YMinProperty = DependencyProperty.Register("YMin", typeof(double),
                    typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        ///  The tick property.
        /// </summary>
        public static DependencyProperty YTickProperty = DependencyProperty.Register("YTick", typeof(double),
                    typeof(LineChartControlLib),
                    new FrameworkPropertyMetadata(2.0, OnPropertyChanged));

        /// <summary>
        ///  The end point.
        /// </summary>
        private Point endPoint;

        /// <summary>
        ///  The rubber band.
        /// </summary>
        private Shape rubberBand;

        /// <summary>
        ///  The start point.
        /// </summary>
        private Point startPoint;

        /// <summary>
        ///  The xmax 0.
        /// </summary>
 //       private double xmax0 = 7;

        /// <summary>
        ///  The xmin 0.
        /// </summary>
   //     private double xmin0 = 0;

        /// <summary>
        ///  The ymax 0.
        /// </summary>
   //     private double ymax0 = 1.5;

        /// <summary>
        ///  The ymin 0.
        /// </summary>
   //     private double ymin0 = -1.5;

        /// <summary>
        ///  The  data Set.
        /// </summary>
        private DataCollection _dataSet;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public LineChartControlLib()
        {
            InitializeComponent();
            this.ChartStyle        = new ChartStyleGridlines();
            this.DataSet           = new DataCollection();
            this.DataSeries        = new DataSeries();
            this.Legend            = new Legend();
            ChartStyle.TextCanvas  = textCanvas;
            ChartStyle.ChartCanvas = chartCanvas;
            Legend.LegendCanvas    = legendCanvas;
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Adds  chart.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddChart()
        {
            ChartStyle.AddChartStyle(tbTitle, tbXLabel, tbYLabel);
            if (_dataSet.DataList.Count != 0)
            {
                _dataSet.AddLines(ChartStyle);
                Legend.AddLegend(chartCanvas, _dataSet);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        /////  Adds  chart.
        ///// </summary>
        ///// <param name="xmin"> The xmin.</param>
        ///// <param name="xmax"> The xmax.</param>
        ///// <param name="ymin"> The ymin.</param>
        ///// <param name="ymax"> The ymax.</param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void AddChart(double xmin, double xmax, double ymin, double ymax)
        //{
        //    _dataSet    = new DataCollection();
        //    DataSeries = new DataSeries();
        //    ChartStyle = new ChartStyleGridlines();

        //    ChartStyle.ChartCanvas     = chartCanvas;
        //    ChartStyle.TextCanvas      = textCanvas;
        //    ChartStyle.Title           = "My 2D Chart";
        //    ChartStyle.Xmin            = xmin;
        //    ChartStyle.Xmax            = xmax;
        //    ChartStyle.Ymin            = ymin;
        //    ChartStyle.Ymax            = ymax;
        //    ChartStyle.GridlinePattern = GridlinePatternEnum.Dot;
        //    ChartStyle.GridlineColor   = Brushes.Black;
        //    ChartStyle.AddChartStyle(tbTitle, tbXLabel, tbYLabel);

        //    // Draw Sine curve:
        //    DataSeries.LineColor     = Brushes.Blue;
        //    DataSeries.LineThickness = 2;
        //    double dx                = (ChartStyle.Xmax - ChartStyle.Xmin) / 100;

        //    for (double x = ChartStyle.Xmin; x <= ChartStyle.Xmax + dx; x += dx)
        //    {
        //        double y = Math.Exp(-0.3 * Math.Abs(x)) * Math.Sin(x);
        //        DataSeries.LineSeries.Points.Add(new Point(x, y));
        //    }
        //    _dataSet.DataList.Add(DataSeries);

        //    // Draw cosine curve:
        //    DataSeries               = new DataSeries();
        //    DataSeries.LineColor     = Brushes.Red;
        //    DataSeries.LinePattern   = LinePatternEnum.DashDot;
        //    DataSeries.LineThickness = 2;

        //    for (double x = ChartStyle.Xmin; x <= ChartStyle.Xmax + dx; x += dx)
        //    {
        //        double y = Math.Exp(-0.3 * Math.Abs(x)) * Math.Cos(x);
        //        DataSeries.LineSeries.Points.Add(new Point(x, y));
        //    }
        //    _dataSet.DataList.Add(DataSeries);
        //    _dataSet.AddLines(ChartStyle);
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Event handler. Called by chartGrid for size changed events.
        /// </summary>
        /// <param name="sender"> Source of the event.</param>
        /// <param name="e">      Size changed event information.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Chart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // tbDate.Text = DateTime.Now.ToShortDateString();
            textCanvas.Width = chartGrid.ActualWidth;
            textCanvas.Height = chartGrid.ActualHeight;
            chartCanvas.Children.Clear();
            textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
            AddChart();

            //AddChart(ChartStyle.Xmin, ChartStyle.Xmax, ChartStyle.Ymin, ChartStyle.Ymax);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Event handler. Called by chartGrid for size changed events.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e">      Size changed event information.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void chartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Refresh();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Refresh
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Refresh()
        {
            textCanvas.Width = chartGrid.ActualWidth;
            textCanvas.Height = chartGrid.ActualHeight;
            legendCanvas.Children.Clear();
            chartCanvas.Children.Clear();
//            chartCanvas.Children.RemoveRange(1, chartCanvas.Children.Count - 1);
            textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
            AddChart();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Executes the mouse left button down action.
        /// </summary>
        /// <param name="sender"> Source of the event.</param>
        /// <param name="e">      Event information to send to registered event handlers.</param>
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
        ///  Executes the mouse left button up action.
        /// </summary>
        /// <param name="sender"> Source of the event.</param>
        /// <param name="e">      Event information to send to registered event handlers.</param>
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
                x0 = ChartStyle.Xmin + (ChartStyle.Xmax - ChartStyle.Xmin) * startPoint.X / chartCanvas.Width;
                x1 = ChartStyle.Xmin + (ChartStyle.Xmax - ChartStyle.Xmin) * endPoint.X / chartCanvas.Width;
            }
            else if (endPoint.X < startPoint.X)
            {
                x1 = ChartStyle.Xmin + (ChartStyle.Xmax - ChartStyle.Xmin) * startPoint.X / chartCanvas.Width;
                x0 = ChartStyle.Xmin + (ChartStyle.Xmax - ChartStyle.Xmin) * endPoint.X / chartCanvas.Width;
            }

            if (endPoint.Y < startPoint.Y)
            {
                y0 = ChartStyle.Ymin + (ChartStyle.Ymax - ChartStyle.Ymin) * (chartCanvas.Height - startPoint.Y) / chartCanvas.Height;
                y1 = ChartStyle.Ymin + (ChartStyle.Ymax - ChartStyle.Ymin) * (chartCanvas.Height - endPoint.Y) / chartCanvas.Height;
            }
            else if (endPoint.Y > startPoint.Y)
            {
                y1 = ChartStyle.Ymin + (ChartStyle.Ymax - ChartStyle.Ymin) * (chartCanvas.Height - startPoint.Y) / chartCanvas.Height;
                y0 = ChartStyle.Ymin + (ChartStyle.Ymax - ChartStyle.Ymin) * (chartCanvas.Height - endPoint.Y) / chartCanvas.Height;
            }

            chartCanvas.Children.Clear();
            textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
            AddChart();
         //   AddChart(x0, x1, y0, y1);

            if (rubberBand != null)
            {
                rubberBand = null;
                chartCanvas.ReleaseMouseCapture();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Executes the mouse move action.
        /// </summary>
        /// <param name="sender"> Source of the event.</param>
        /// <param name="e">      Event information to send to registered event handlers.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (chartCanvas.IsMouseCaptured)
            {
                endPoint = e.GetPosition(chartCanvas);
                if (rubberBand == null)
                {
                    rubberBand        = new Rectangle();
                    rubberBand.Stroke = Brushes.Red;
                    chartCanvas.Children.Add(rubberBand);
                }
                rubberBand.Width  = Math.Abs(startPoint.X - endPoint.X);
                rubberBand.Height = Math.Abs(startPoint.Y - endPoint.Y);
                double left       = Math.Min(startPoint.X, endPoint.X);
                double top        = Math.Min(startPoint.Y, endPoint.Y);
                Canvas.SetLeft(rubberBand, left);
                Canvas.SetTop(rubberBand, top);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Executes the mouse right button down action.
        /// </summary>
        /// <param name="sender"> Source of the event.</param>
        /// <param name="e">      Event information to send to registered event handlers.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            chartCanvas.Children.Clear();
            textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
            AddChart();
          //  AddChart(xmin0, xmax0, ymin0, ymax0);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Executes the property changed action.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e">      Event information to send to registered event handlers.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            LineChartControlLib lcc = sender as LineChartControlLib;

            if (lcc == null) return;

            if (e.Property == XMinProperty)
            {
                lcc.XMin = (double)e.NewValue;
            }
            else if (e.Property == XMaxProperty)
            {
                lcc.XMax = (double)e.NewValue;
            }
            else if (e.Property == YMinProperty)
            {
                lcc.YMin = (double)e.NewValue;
            }
            else if (e.Property == YMaxProperty)
            {
                lcc.YMax = (double)e.NewValue;
            }
            else if (e.Property == XTickProperty)
            {
                lcc.XTick = (double)e.NewValue;
            }
            else if (e.Property == YTickProperty)
            {
                lcc.YTick = (double)e.NewValue;
            }
            else if (e.Property == GridlinePatternProperty)
            {
                lcc.GridlinePattern = (GridlinePatternEnum)e.NewValue;
            }
            else if (e.Property == GridlineColorProperty)
            {
                lcc.GridlineColor = (Brush)e.NewValue;
            }
            else if (e.Property == TitleProperty)
            {
                lcc.Title = (string)e.NewValue;
            }
            else if (e.Property == XLabelProperty)
            {
                lcc.XLabel = (string)e.NewValue;
            }
            else if (e.Property == YLabelProperty)
            {
                lcc.YLabel = (string)e.NewValue;
            }
            else if (e.Property == IsXGridProperty)
            {
                lcc.IsXGrid = (bool)e.NewValue;
            }
            else if (e.Property == IsYGridProperty)
            {
                lcc.IsYGrid = (bool)e.NewValue;
            }
            else if (e.Property == LegendEnableProperty)
            {
                lcc.LegendEnable = (bool)e.NewValue;
            }
            else if (e.Property == LegendPositionProperty)
            {
                lcc.LegendPosition = (LegendPositionEnum)e.NewValue;
            }
            else if (e.Property == DataSetProperty)
            {
                lcc._dataSet = (DataCollection)e.NewValue;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: User the Control Size Changed
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e">      The e.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // tbDate.Text = DateTime.Now.ToShortDateString();
            //textCanvas.Width = ActualWidth;
            //textCanvas.Height = ActualHeight;
            //chartCanvas.Children.Clear();
            //textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
            //AddChart();

            //AddChart(ChartStyle.Xmin, ChartStyle.Xmax, ChartStyle.Ymin, ChartStyle.Ymax);

        }
    }
}

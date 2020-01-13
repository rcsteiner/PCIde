////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the error view model class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/20/2015   rcs     Initial Implementation
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
//  USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Ide.Controller;
using LineCharts;
using pc;
using Views.Support;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  File stats view model.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelChart : ViewModelTool
    {
        /// <summary>
        ///  Get/Set Data.
        /// </summary>
        public DataCollection Data { get;  set; }

        /// <summary>
        ///  Get/Set gridx.
        /// </summary>
        public bool GridX { get; private set; }

        /// <summary>
        ///  Get/Set gridy.
        /// </summary>
        public bool GridY { get; private set; }

        /// <summary>
        ///  Get/Set xlabels.
        /// </summary>
        public string[] LabelsX { get; private set; }

        /// <summary>
        ///  Get/Set ylabels.
        /// </summary>
        public string[] LabelsY { get; private set; }

        /// <summary>
        ///  Get/Set Legend Enable.
        /// </summary>
        public bool LegendEnable { get; private set; }

        /// <summary>
        ///  Get/Set Legend Position.
        /// </summary>
        public LegendPositionEnum LegendPosition { get; private set; }

        /// <summary>
        ///  Get/Set XMax.
        /// </summary>
        public double XMax { get; set; }

        /// <summary>
        ///  Get/Set XMin.
        /// </summary>
        public double XMin { get; set; }

        /// <summary>
        ///  Get/Set XTick.
        /// </summary>
        public double XTick { get; set; }

        /// <summary>
        ///  Get/Set xtitle.
        /// </summary>
        public string XTitle { get; private set; }

        /// <summary>
        ///  Get/Set YMax.
        /// </summary>
        public double YMax { get; set; }

        /// <summary>
        ///  Get/Set YMin.
        /// </summary>
        public double YMin { get; set; }

        /// <summary>
        ///  Get/Set YTick.
        /// </summary>
        public double YTick { get; set; }

        /// <summary>
        ///  Get/Set ytitle.
        /// </summary>
        public string YTitle { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor:
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelChart() : base("Chart", null)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default constructor.
        /// </summary>
        /// <param name="controller"> The controller.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelChart(ControllerIde controller): base("Chart", controller)
        {
            MessegerRegister();
            ContentId = Constants.CHART_CONTENT_ID;
            IconSource = IconUtil.GetImage("Graph");
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Clears this object to its blank/initial state.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Clear()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Clear the Data Set
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ClearDataSet()
        {
            Data.DataList.Clear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Intialize the Chart model
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
        public void Init(double[] data, string title, string[] xlabels, string[] ylabels, string xtitle, string ytitle, bool gridx, bool gridy)
        {
            Title = title;
            LabelsX = xlabels;
            LabelsY = ylabels;
            XTitle = xtitle;
            YTitle = ytitle;
            GridX = gridx;
            GridY = gridy;
            Data = new DataCollection();
            ClearDataSet();


            //// Draw cosine curve:
            //ds = new DataSeries();
            //ds.LineColor = Brushes.Red;
            //ds.SeriesName = "Cosine";
            //ds.Symbols.SymbolType = SymbolTypeEnum.OpenDiamond;
            //ds.Symbols.BorderColor = ds.LineColor;
            //ds.LinePattern = LinePatternEnum.DashDot;
            //ds.LineThickness = 2;
            //for (int i = 0; i < 70; i++)
            //{
            //    double x = i / 5.0;
            //    double y = Math.Cos(x);
            //    ds.LineSeries.Points.Add(new Point(x, y));
            //}
            // Data.DataList.Add(ds);

            LegendEnable = true;
            LegendPosition = LegendPositionEnum.NorthEast;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  New message is received, filter and decide if we are interesed in it.
        ///  Default is to ignore it.
        ///  we look for the user input complete message from the view and capture the data.
        /// </summary>
        /// <param name="sender">  The sender.</param>
        /// <param name="key">     The key.</param>
        /// <param name="payload"> The payload.</param>
        /// <returns>
        ///  the message result, what to do.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override MessageResult NewMessage(IMessageListener sender, string key, object payload)
        {
            //if (key == "UserInput")
            //{
            //    return MessageResult.HANDLED_STOP;
            //}
            return MessageResult.IGNORED;
        }
    }
}






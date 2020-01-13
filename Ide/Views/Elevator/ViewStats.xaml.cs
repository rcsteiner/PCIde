////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   Highest level building view
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/25/2018  RCS       Initial code.
//  
// 
//=====================================================[ Copyright ]=====================================================
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
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ide.Properties;
using Lift;
using Lift.View;
using LiftLib.View;
using ViewModels;


namespace Views
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  The IUpdate Interface definition.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface IUpdate
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update();
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Interaction logic for Building, this owns the canvas and the stats view.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class ViewStats : UserControl, IUpdate, IStats
    {

        /// <summary>
        ///  Get/Set average Wait Time.
        /// </summary>
        public double AvgWaitTime
        {
            get { return _avgWaitTime ;}
            set {_avgWaitTime = value; txtAvgWaitTiem.Text = value.ToString("F1"); }
        }

        /// <summary>
        ///  Get/Set Elapsed Time.
        /// </summary>
        public double ElapsedTime
        {
            get { return _elapsedTime; }
            set { _elapsedTime = value; txtElapsedTime.Text = _elapsedTime.ToString("F0"); }
        }

        /// <summary>
        ///  Get/Set maximum Wait Time.
        /// </summary>
        public double MaxWaitTime
        {
            get { return _maxWaitTime; }
            set { _maxWaitTime = value; txMaxgWaitTiem.Text = _maxWaitTime.ToString("F1"); }
        }

        /// <summary>
        ///  Get/Set Moves.
        /// </summary>
        public int Moves
        {
            get { return _moves; }
            set { _moves = value; txtMoves.Text = _moves.ToString("F1"); }
        }

        /// <summary>
        ///  Get/Set Transported.
        /// </summary>
        public int Transported
        {
            get { return _transported; }
            set { _transported = value; txtTransported.Text = value.ToString("F1"); }
        }

        /// <summary>
        ///  Get/Set Transported Per security.
        /// </summary>
        public double TransportedPerSec
        {
            get { return _transportedPerSec; }
            set { _transportedPerSec = value; txtTransportSec.Text = value.ToString("F1"); }
        }


        /// <summary>
        ///  The  elapsed Time.
        /// </summary>
        private double _elapsedTime;

        /// <summary>
        ///  The  maximum Wait Time.
        /// </summary>
        private double _maxWaitTime;

        /// <summary>
        ///  The  moves.
        /// </summary>
        private int _moves;

        /// <summary>
        ///  The  total Wait Time, used to compute average.
        /// </summary>
        private double _totalWaitTime;

        /// <summary>
        ///  The  transported.
        /// </summary>
        private int _transported;

        /// <summary>
        ///  The  transported Per security.
        /// </summary>
        private double _transportedPerSec;

        private double _avgWaitTime;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor:
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewStats()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Add the Wait Time
        /// </summary>
        /// <param name="waitTime"> The wait Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddWaitTime(double waitTime)
        {
            _totalWaitTime += waitTime;
            if (waitTime > MaxWaitTime)
            {
                MaxWaitTime = waitTime;
            }
            ++Transported;

            TransportedPerSec = Transported / (ElapsedTime > 0 ? ElapsedTime : 1);
      }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Method: Clear
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void Clear()
        {
            Transported    = 0;
            _totalWaitTime = 0;
            ElapsedTime    = 0;
            MaxWaitTime    = 0;
            Moves          = 0;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Update()
        {
            //var old = DataContext;
            //DataContext = null;
            //DataContext = old;
            IStats s = DataContext as IStats;
            if (s == null) return;
            AvgWaitTime = s.AvgWaitTime;
            ElapsedTime = s.ElapsedTime;
            MaxWaitTime = s.MaxWaitTime;
            Transported = s.Transported;
            TransportedPerSec = s.TransportedPerSec;
            Moves = s.Moves;
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
            var x = DataContext;
            ((ViewModelStats)DataContext).View = this;
        }
    }
}



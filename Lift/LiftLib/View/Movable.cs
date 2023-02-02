////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:  Base file for all user objects
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/14/2018  RCS       Initial code.
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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lift.View
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Interaction logic for Movable.xaml,  Movable provides a base control for movable objects.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Movable : UserControl
    {
        /// <summary>
        ///  The  x coordinate, offset (relative to parent)
        /// </summary>
        public double XOff;

        /// <summary>
        ///  The  y coordinate. , offset (relative to parent)
        /// </summary>
        public double YOff;

        /// <summary>
        ///  The  call Back for the next step after current task completes.
        /// </summary>
        protected NotifyHandler _nextStep;

        /// <summary>
        ///  The  current Task, executed on each frame of animation.
        /// </summary>
        protected TaskFunHandler _currentTask;

        /// <summary>
        ///  The  default Interpolator.
        /// </summary>
        private static Interpolator _defaultInterpolator;

        /// <summary>
        ///  The  interpolator to use.
        /// </summary>
        private Interpolator _interpolator;

        /// <summary>
        ///  The  new x coordinate.
        /// </summary>
        protected double _newX;

        /// <summary>
        ///  The  new y coordinate.
        /// </summary>
        private double _newY;

        /// <summary>
        ///  The  original x coordinate.
        /// </summary>
        private double _origX;

        /// <summary>
        ///  The  original y coordinate.
        /// </summary>
        private double _origY;

        /// <summary>
        ///  The time Spent.
        /// </summary>
        protected double TimeSpent;

        /// <summary>
        ///  The  time To Spend.
        /// </summary>
        protected double TimeToSpend;

        /// <summary>
        ///  The  world x coordinate.  What the canvas uses
        /// </summary>
        private double _worldX;

        /// <summary>
        ///  The  world y coordinate. What the canvas uses
        /// </summary>
        private double _worldY;

        /// <summary>
        ///  The  random generator used by all children
        /// </summary>
        protected static Random Random = new Random();

        /// <summary>
        ///  Get/Set My Parent, in the movable hiearchy.
        /// </summary>
        public Movable MyParent { get; set; }

        /// <summary>
        ///  Get/Set Canvas used for drawing.
        /// </summary>
        public Canvas MyCanvas { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor:  Sets up initial position with no tasks.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Movable()
        {
            _defaultInterpolator = CoolInterpolate;
            XOff                 = 0.0;
            YOff                 = 0.0;
            _worldX              = 0.0;
            _worldY              = 0.0;
            _currentTask         = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor:  Set the canvas and default construction.
        /// </summary>
        /// <param name="myCanvas"> The my Canvas.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Movable(Canvas myCanvas) : this()
        {
            MyCanvas = myCanvas;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Remove this from the canvas
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Remove()
        {
            MyCanvas.Children.Remove(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Cool Interpolate using power.
        /// </summary>
        /// <param name="value0"> The value0.</param>
        /// <param name="value1"> The value1.</param>
        /// <param name="x">      The x coordinate.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private double CoolInterpolate(double value0, double value1, double x)
        {
            return PowInterpolate(value0, value1, x, 1.3);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Create Rectangle and add to canvas.
        /// </summary>
        /// <param name="yBottom">         The y coordinate Bottom.</param>
        /// <param name="yHeight">         The y coordinate Height.</param>
        /// <param name="xLeft">           The x coordinate Left.</param>
        /// <param name="xWidth">          The x coordinate Width.</param>
        /// <param name="fill">            The fill.</param>
        /// <param name="strokeThickness"> The stroke Thickness.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public  Rectangle CreateRectangle(double yBottom, double yHeight, double xLeft, double xWidth, Brush fill, int strokeThickness)
        {
            var rect             = new Rectangle();
            MyCanvas.Children.Add(rect);
            rect.Fill            = fill;
            rect.Stroke          = Brushes.Black;
            rect.Width           = xWidth;
            rect.Height          = yHeight;
            rect.StrokeThickness = strokeThickness;
            Canvas.SetBottom(rect, yBottom);
            Canvas.SetLeft(rect, xLeft);
            return rect;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: get the World Position, by adding it to any relative to parent postion.
        /// </summary>
        /// <param name="x"> [ref] The x coordinate.</param>
        /// <param name="y"> [ref] The y coordinate.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GetWorldPosition(out double x, out double y)
        {
            var resultX       = XOff;
            var resultY       = YOff;
            var currentParent = MyParent;
            while (currentParent != null)
            {
                resultX      += currentParent.XOff;
                resultY      += currentParent.YOff;
                currentParent = currentParent.MyParent;
            }

            x = resultX;
            y = resultY;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Check if busy executing a task (i.e. current task is not null)
        /// </summary>
        /// <returns>
        ///  True if busy else not busy (no current task)
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsBusy()
        {
            return _currentTask != null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Linear Interpolate between two numbers value0 and value1
        /// </summary>
        /// <param name="value0"> The value0.</param>
        /// <param name="value1"> The value1.</param>
        /// <param name="factor">  factor 0-1 (percent)</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double LinearInterpolate(double value0, double value1, double factor)
        {
            return value0 + (value1 - value0) * factor;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: move the To x,y
        /// </summary>
        /// <param name="newX"> The new x offset coordinate.</param>
        /// <param name="newY"> The new y offset coordinate.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MoveTo(double newX, double newY)
        {
            XOff = newX;
            YOff = newY;
            UpdateDisplayPosition();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: move the location Over Time Task, calls the callback and clears the task when at destination.
        /// </summary>
        /// <param name="deltaTime"> The delta Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MoveToOverTimeTask(double deltaTime)
        {
            TimeSpent = Math.Min(TimeToSpend, TimeSpent + deltaTime);

            if (TimeSpent >= TimeToSpend)
            {
                MoveTo(_newX, _newY);
                _currentTask = null;
                _nextStep?.Invoke();
            }
            else
            {
                var factor = TimeSpent / TimeToSpend;
                MoveTo(_defaultInterpolator(_origX, _newX, factor), _defaultInterpolator(_origY, _newY, factor));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: move the To Over Time, sets up a task, and next task.
        /// </summary>
        /// <param name="newX">         The new x coordinate.</param>
        /// <param name="newY">         The new y coordinate.</param>
        /// <param name="timeToSpend">  The time To Spend.</param>
        /// <param name="interpolator"> The interpolator.</param>
        /// <param name="nextTask">     The callback Completed.</param>
        /// <param name="execute">      [optional=null] The execute.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void MoveToOverTime(double newX, double newY, double timeToSpend, Interpolator interpolator, NotifyHandler nextTask, TaskFunHandler execute=null)
        {
            _newX         = newX;
            _newY         = newY;
            TimeToSpend   = timeToSpend;
            _interpolator = interpolator;
            _origX        = XOff;
            _origY        = YOff;
            TimeSpent     = 0.0;
            _currentTask  = execute?? MoveToOverTimeTask;
            _nextStep     = nextTask;

            if (_interpolator == null)
            {
                _interpolator = _defaultInterpolator;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: move the To Y
        /// </summary>
        /// <param name="newY"> The new y coordinate.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MoveToY(double newY)
        {
            YOff = newY;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Pow the Interpolate
        /// </summary>
        /// <param name="value0"> The value0.</param>
        /// <param name="value1"> The value1.</param>
        /// <param name="factor"> The factor (0-1).</param>
        /// <param name="power">  The power.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private double PowInterpolate(double value0, double value1, double factor, double power)
        {
            return value0 + (value1 - value0) * Math.Pow(factor, power) / (Math.Pow(factor, power) + Math.Pow(1 - factor, power));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: set the Parent
        /// </summary>
        /// <param name="movableParent"> The movable Parent.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetParent(Movable movableParent)
        {
           double x, y;
           if (movableParent == null)
            {
                if (MyParent != null)
                {
                    GetWorldPosition(out x, out y);
                    MyParent = null;
                    MoveTo(x, y);
                }
            }
            else
            {
                // Parent is being set a non-null movable
                GetWorldPosition(out x, out y);
                double px , py;
                movableParent.GetWorldPosition(out px, out py);
                MyParent = movableParent;
                MoveTo(x - px, y - py);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: update, called to update the task operation like position, if it exits.
        /// </summary>
        /// <param name="deltaTime"> The delta Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Update(double deltaTime)
        {
            _currentTask?.Invoke(deltaTime);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: update the Display Position of this object.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UpdateDisplayPosition()
        {
            GetWorldPosition(out _worldX, out _worldY);
            Canvas.SetLeft(this, _worldX);
            Canvas.SetBottom(this, _worldY);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: update the time spend and test if more than allowed.
        /// </summary>
        /// <param name="deltaTime"> The delta Time.</param>
        /// <returns>
        ///  True time is up, else false more time allowed.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool UpdateTime(double deltaTime)
        {
            TimeSpent = Math.Min(TimeToSpend, TimeSpent + deltaTime);
            return (TimeSpent >= TimeToSpend);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: limit a Number to a high and low value.
        /// </summary>
        /// <param name="value"> The value.</param>
        /// <param name="low">   The low.</param>
        /// <param name="high">  The high.</param>
        /// <returns>
        ///  The value in this range.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static double LimitNumber(double value, double low, double high)
        {
            if (value < low)
            {
                value = low;
            }
            else if (value > high)
            {
                value = high;
            }

            return value;
        }
    }
}


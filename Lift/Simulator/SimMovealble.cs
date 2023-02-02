////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:  An object that can move.
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/15/2018  RCS       Initial code.
//  
// 
//=====================================================[ Copyright ]=====================================================
// 
//  Copyright Advanced Performance
//  All Rights Reserved THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Advanced Performance
//  The copyright notice above does not evidence any actual or intended publication of such source code.
//  Some third-party source code components may have been modified from their original versions
//  by Advanced Performance. The modifications are Copyright Advanced Performance., All Rights Reserved.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFElevator.View;

namespace WPFElevator
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  The Sim Movealble Class definition.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class SimMovable : UIElement
    {

        /// <summary>
        ///  Get/Set My Canvas.
        /// </summary>
        public Canvas MyCanvas { get; set; }

        /// <summary>
        ///  Get/Set My Parent.
        /// </summary>
        public Movable MyParent { get; set; }

        /// <summary>
        ///  The  call Back.
        /// </summary>
        private NotifyHandler _callBack;

        /// <summary>
        ///  The  current Task.
        /// </summary>
        private TaskFunHandler _currentTask;

        /// <summary>
        ///  The  default Interpolator.
        /// </summary>
        private static Interpolator _defaultInterpolator;

        /// <summary>
        ///  The  interpolator.
        /// </summary>
        private Interpolator _interpolator;

        /// <summary>
        ///  The  new x coordinate.
        /// </summary>
        private double _newX;

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
        ///  The  millis.
        /// </summary>
        private double _time;

        /// <summary>
        ///  The time Spent.
        /// </summary>
        protected double _timeSpent;
        /// <summary>
        ///  The  x coordinate.
        /// </summary>
        public double X;

        /// <summary>
        ///  The  y coordinate.
        /// </summary>
        public double Y;

        /// <summary>
        ///  The  time To Spend.
        /// </summary>
        protected double _timeToSpend;

        /// <summary>
        ///  The  world x coordinate.
        /// </summary>
        private double _worldX;

        /// <summary>
        ///  The  world y coordinate.
        /// </summary>
        private double _worldY;

        /// <summary>
        ///  The  random generator used by all children
        /// </summary>
        protected static Random Random = new Random();

        /// <summary>
        ///  The New Display State Event definition.
        /// </summary>
        public event MovableTaskHandler NewDisplayState;

        /// <summary>
        ///  The New State Event definition.
        /// </summary>
        public event MovableTaskHandler NewState;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Cool Interpolate using power.
        /// </summary>
        /// <param name="value0"> The value0.</param>
        /// <param name="value1"> The value1.</param>
        /// <param name="x">      The x coordinate.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private double CoolInterpolate(double value0, double value1, double x)
        {
            return PowInterpolate(value0, value1, x, 1.3);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Create the Rectangle
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
        public Rectangle CreateRectangle(double yBottom, double yHeight, double xLeft, double xWidth, Brush fill, int strokeThickness)
        {
            var rect = new Rectangle();
            MyCanvas.Children.Add(rect);
            rect.Fill = fill;
            rect.Stroke = Brushes.Black;
            rect.Width = xWidth;
            rect.Height = yHeight;
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
        public void GetWorldPosition(ref double x, ref double y)
        {
            var resultX = X;
            var resultY = Y;
            var currentParent = MyParent;
            while (currentParent != null)
            {
                resultX += currentParent.X;
                resultY += currentParent.Y;
                currentParent = currentParent.MyParent;
            }

            x = resultX;
            y = resultY;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: is the Busy
        /// </summary>
        /// <returns>
        ///  True if successful
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsBusy()
        {
            return _currentTask != null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Linear the Interpolate
        /// </summary>
        /// <param name="value0"> The value0.</param>
        /// <param name="value1"> The value1.</param>
        /// <param name="x">      The x coordinate.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double LinearInterpolate(double value0, double value1, double x)
        {
            return value0 + (value1 - value0) * x;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: make the Sure Not Busy
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MakeSureNotBusy()
        {
            if (IsBusy())
            {
                Console.Write(@"Attempt to use movable while it was busy");
                throw new Exception("Object is busy - you should use callback");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor:
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public SimMovable()
        {
            _defaultInterpolator = CoolInterpolate;
            X = 0.0;
            Y = 0.0;
            _worldX = 0.0;
            _worldY = 0.0;
            _currentTask = null;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor:
        /// </summary>
        /// <param name="myCanvas"> The my Canvas.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public SimMovable(Canvas myCanvas) : this()
        {
            MyCanvas = myCanvas;
            myCanvas.Children.Add(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: move the To x,y
        /// </summary>
        /// <param name="newX"> The new x coordinate.</param>
        /// <param name="newY"> The new y coordinate.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MoveTo(double newX, double newY)
        {
            X = newX;
            Y = newY;

            Canvas.SetLeft(this, X);
            Canvas.SetBottom(this, Y);
            //   NewState?.Invoke(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: move the To Over Time, sets up a task
        /// </summary>
        /// <param name="newX">              The new x coordinate.</param>
        /// <param name="newY">              The new y coordinate.</param>
        /// <param name="timeToSpend">       The time To Spend.</param>
        /// <param name="interpolator">      The interpolator.</param>
        /// <param name="callbackCompleted"> The callback Completed.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MoveToOverTime(double newX, double newY, double timeToSpend, Interpolator interpolator, NotifyHandler callbackCompleted)
        {
            MakeSureNotBusy();

            _newX = newX;
            _newY = newY;
            _timeToSpend = timeToSpend;
            _interpolator = interpolator;
            _origX = X;
            _origY = Y;
            _timeSpent = 0.0;
            _currentTask = MoveToOverTimeTask;
            _callBack = callbackCompleted;

            if (_interpolator == null)
            {
                _interpolator = _defaultInterpolator;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: move the location Over Time Task, calls the callback and clears the task when at destination.
        /// </summary>
        /// <param name="deltaTime"> The delta Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MoveToOverTimeTask(double deltaTime)
        {
            _timeSpent = Math.Min(_timeToSpend, _timeSpent + deltaTime);

            if (_timeSpent >= _timeToSpend)
            {
                // Epsilon issues possibly?
                MoveTo(_newX, _newY);
                _currentTask = null;
                _callBack?.Invoke();
            }
            else
            {
                var factor = _timeSpent / _timeToSpend;
                MoveTo(_defaultInterpolator(_origX, _newX, factor), _defaultInterpolator(_origY, _newY, factor));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: move the To X
        /// </summary>
        /// <param name="newX"> The new x coordinate.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MoveToX(double newX)
        {
            X = newX;
            Canvas.SetLeft(this, X);
            //   NewState?.Invoke(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: move the To Y
        /// </summary>
        /// <param name="newY"> The new y coordinate.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MoveToY(double newY)
        {
            Y = newY;
            Canvas.SetBottom(this, Y);
            //   NewState?.Invoke(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Pow the Interpolate
        /// </summary>
        /// <param name="value0"> The value0.</param>
        /// <param name="value1"> The value1.</param>
        /// <param name="x">      The x coordinate.</param>
        /// <param name="a">      The a.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private double PowInterpolate(double value0, double value1, double x, double a)
        {
            return value0 + (value1 - value0) * Math.Pow(x, a) / (Math.Pow(x, a) + Math.Pow(1 - x, a));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Remove myself from the canvas
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Remove()
        {
            MyCanvas.Children.Remove(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: set the Parent, moves the position stack relative to the new parent (object doesn't move on screen)
        /// </summary>
        /// <param name="movableParent"> The movable Parent.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetParent(Movable movableParent)
        {
            double x=0, y=0;
            if (movableParent == null)
            {
                if (MyParent != null)
                {
                    GetWorldPosition(ref x, ref y);
                    MyParent = null;
                    MoveTo(x, y);
                }
            }
            else
            {
                // Parent is being set a non-null movable
                var oldParent = MyParent;
                 MyParent = movableParent;
                GetWorldPosition(ref x, ref y);
                MoveTo(x - oldParent.X, y -oldParent.Y);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: update behavior.
        /// </summary>
        /// <param name="deltaTime"> The delta Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Update(double deltaTime)
        {
            _currentTask?.Invoke(deltaTime);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: update the Display Position
        /// </summary>
        /// <param name="forceTrigger"> True if force Trigger.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UpdateDisplayPosition(bool forceTrigger = false)
        {
            double x = 0, y = 0;

            GetWorldPosition(ref x, ref y);
            var oldX = _worldX;
            var oldY = _worldY;
            _worldX  = x;
            _worldY  = y;

            if (oldX != _worldX || oldY != _worldY || forceTrigger)
            {
                   NewDisplayState?.Invoke(this);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: update
        /// </summary>
        /// <param name="deltaTime"> The delta Time.</param>
        /// <returns>
        ///  True if successful
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool UpdateTime(double deltaTime)
        {
            _timeSpent = Math.Min(_timeToSpend, _timeSpent + deltaTime);
            return (_timeSpent >= _timeToSpend);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: wait, schedules a wait task, that if times out, calls callBack notifier.
        /// </summary>
        /// <param name="seconds">  The millis.</param>
        /// <param name="callBack"> The call Back.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Wait(double seconds, NotifyHandler callBack)
        {
            _time = seconds;
            MakeSureNotBusy();
            _timeSpent = 0.0;
            _currentTask = WaitTask;
            _callBack = callBack;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: wait the Task
        /// </summary>
        /// <param name="deltaTime"> The delta Time.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void WaitTask(double deltaTime)
        {
            _timeSpent += deltaTime;
            if (_timeSpent > _time)
            {
                _currentTask = null;
                _callBack?.Invoke();
            }
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFElevator
{
    public class Simulator
    {

        /// <summary>
        ///  The  random generator used by all children
        /// </summary>
        protected static Random Random = new Random();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Linear the Interpolate
        /// </summary>
        /// <param name="value0"> The value0.</param>
        /// <param name="value1"> The value1.</param>
        /// <param name="x">      The x coordinate.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double LinearInterpolate(double value0, double value1, double x)
        {
            return value0 + (value1 - value0) * x;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: move the To x,y
        /// </summary>
        /// <param name="newX"> The new x coordinate.</param>
        /// <param name="newY"> The new y coordinate.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MoveTo(UIElement element, double x, double y)
        {
            Canvas.SetLeft(element, x);
            Canvas.SetBottom(element, y);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: move the To Over Time, sets up a task
        /// </summary>
        /// <param name="newX">              The new x coordinate.</param>
        /// <param name="newY">              The new y coordinate.</param>
        /// <param name="timeToSpend">       The time To Spend.</param>
        /// <param name="interpolator">      The interpolator.</param>
        /// <param name="callbackCompleted"> The callback Completed.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
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



    }
}
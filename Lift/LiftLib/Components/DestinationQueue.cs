////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:  The Destination queue
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/3/2018   RCS       Initial code.
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Lift
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  The Destination Queue of floors to visit in order, when a passanger enters the elevator, the floor selected is added
    ///  to the end of the queue.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class DestinationQueue : List<int>
    {
        /// <summary>
        ///  Get/Set Head of the queue (next floor to go to)
        /// </summary>
        public int Head
        {
            get { return Count > 0 ? this[0] : -1; }
            set { Insert(0, value); }
        }

        /// <summary>
        ///  Get/Set Tail, the last floor in queue (most recently added)
        /// </summary>
        public int Tail
        {
            get { return Count > 0 ? this[Count - 1] : -1; }
            set { Add(value); }
        }

        /// <summary>
        ///  The  direction to sort
        /// </summary>
        private Direction _sortDir;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Constructor: Creates the queue empty.
        /// </summary>
        /// <param name="capacity"> [optional=20] The capacity.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public DestinationQueue(int capacity = 20) : base(capacity)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: compare two entries in the queue for sorting.
        /// </summary>
        /// <param name="a"> The first value.</param>
        /// <param name="b"> The second value.</param>
        /// <returns>
        ///   neg if a LT b  , zero if a  EQ b,  pos if a GT b
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int cmp(int a, int b)
        {
            return _sortDir == Direction.DOWN ? b - a : a - b;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Dequeue, removes the head of queue and returns it.
        /// </summary>
        /// <returns>
        ///  The head or -1 if the queue is empty.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int Dequeue()
        {
            if (Count == 0) return -1;
            int head = this[0];
            RemoveAt(0);
            return head;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Enqueue a floor number, only if not already in queue.
        /// </summary>
        /// <param name="floorNum"> floor number to enqueue.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Enqueue(int floorNum)
        {
            foreach (var v in this)
            {
                if (v == floorNum) return;
            }
            Tail = floorNum;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Sort the floors by direction starting with the floornum at the head (or next one if available)
        ///             53612   at 4  UP  56321
        /// </summary>
        /// <param name="dir">           The direction to sort (UP/DOWN).</param>
        /// <param name="floorNumStart"> The floor number Start.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SortBidirectional(Direction dir, int floorNumStart)
        {
            if (Count < 2) return;

            Sort(dir);      // ordered the way we want
            for (int i = 0; i < Count; i++)
            {
                var floor = this[i];
                if (   floor == floorNumStart)
                {
                    // found starting point
                    while (--i >= 0)
                    {
                        var f =this[i]; // put it on the end, in reverse order
                        Add(f);
                        RemoveAt(i);
                    }

                    return;
                }
                else if (   (dir == Direction.DOWN && floor < floorNumStart) || (dir == Direction.UP   && floor > floorNumStart))
                {
                    // found starting point
                    --i;
                    while (i-- > 0)
                    {
                        var f =this[i]; // put it on the end, in reverse order
                        Add(f);
                        RemoveAt(i);
                        
                    }

                    return;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Sort the array in a specific direction.
        /// </summary>
        /// <param name="dir"> The direction to sort UP/DOWN.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Sort(Direction dir)
        {
            _sortDir = dir;
            base.Sort(cmp);
        }




    }
}



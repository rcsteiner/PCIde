////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the runtime stack class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/10/2015   rcs     Initial Implementation
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
//  USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.Reserved.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Text;
using ZCore;

namespace pc.Context
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Request run line callback to controller.
    /// </summary>
    /// <param name="lineNumber">   The line number. </param>
    /// <param name="element">      The element. </param>
    /// <returns>
    /// the state to goto after run this statement.
    /// </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public delegate ReturnState RequestRunLine(int lineNumber,  PElement element);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Stack used duing runtime
    /// Stores all variables and local variables in an array
    /// Statics and globals are accessed via index
    /// Locals are index+base ptr
    /// Stack frames can be adjusted by Enter and Leave
    /// </summary>
   ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class RuntimeStack
    {
        /// <summary>
        /// Occurs when [request run line].
        /// </summary>
        public event RequestRunLine RequestRunLine;

        /// <summary>
        /// The stack.
        /// </summary>
        private List<Accumulator> _stack;

        /// <summary>
        /// Number of elements in stack. (top of stack+1)
        /// </summary>
        private int _count;

        /// <summary>
        /// The context.
        /// </summary>
        private PContext _context;

        /// <summary>
        /// The base pointer.
        /// </summary>
        public int _basePtr;

        /// <summary>
        /// The static count (can't pop below this)
        /// </summary>
        private int _staticCount;

        ///// <summary>
        ///// The empty accumalator for errors
        ///// </summary>
        //private Accumulator Empty;

        public string Debug { get { return ToString(); } }
        /// <summary>
        /// Gets or sets a value indicating whether [in call] passing.
        /// </summary>
        public bool InCall { get; set; }


        /// <summary>
        /// Gets or sets the called module. used for passing and correcting arguments.
        /// </summary>
        public PModule CalledModule { get; set; }


        /// <summary>
        /// Gets or sets the current argument being processed in a call. used to make sure conversions are correct.
        /// </summary>
        public PVariable CurrentArg { get; set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count { get { return _count; } }

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        public Accumulator this[int index]
        {
            get {return _stack[index]; }
            set { _stack[index] = value; }
        }

        /// <summary>
        /// The call stack
        /// </summary>
        public ObservableStack<PModule> CallStack = new ObservableStack<PModule>();

        /// <summary>
        /// Gets or sets the current line.
        /// </summary>
        public int CurrentLine { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [single step].
        /// </summary>
        public bool SingleStep { get; set; }

        /// <summary>
        /// Gets or sets the object reference.
        /// Set to the This pointer on an member call 
        /// </summary>
        public PObject ObjectRef { get; set;   }

        ///// <summary>
        ///// The object stack
        ///// </summary>
        //public Stack<PObject> ObjectStack = new Stack<PObject>();
   
        /// <summary> 
        /// Gets or sets a value indicating whether [error expected].
        /// </summary>
        public bool ErrorExpected { get; set; }

        /// <summary>
        /// Gets or sets the error found.
        /// </summary>
        public int ErrorFound { get; set; }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">      The context. </param>
        /// <param name="count">  Number of elements in initial stack. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public RuntimeStack(PContext context, int count)
        {
            _stack       = new List<Accumulator>(count+10);
            _context     = context;
            Initialize(count,null);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets a current line.
        /// Checks if it should stop for breakpoint!
        /// </summary>
        /// <param name="lineNumber">   The line number. </param>
        /// <param name="element"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetCurrentLine(int lineNumber, PElement element)
        {
            CurrentLine = lineNumber;
            if (RequestRunLine != null)
            {
                if (RequestRunLine(lineNumber,element) == ReturnState.STOP)
                {
                    throw new RuntimeError("Stop requested by user");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes this object.
        /// </summary>
        /// <param name="staticCount">  Number of statics. </param>
        /// <param name="program">      The program. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Initialize(int staticCount, PProgram program)
        {
            Clear();
            _staticCount = staticCount;
            for (int i = 0; i <= _count; ++i)
            {
                _stack.Add(new Accumulator());
            }
            _count = staticCount;
            // set the root Program BP at zero
           // SetValue(new Accumulator(0), 0,true );
            Push(0);
            CallStack.Push(program);
   }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Clear()
        {
            _stack.Clear();
            _basePtr = 0;
            CallStack.Clear();
            ObjectRef = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes and returns the top-of-stack object.
        /// </summary>
        /// <returns>
        /// The previous top-of-stack object.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Accumulator Pop()
        {
            if (_stack.Count > _count - 1)
            {
                return _stack[--_count];
            }
            throw new RuntimeError("Error, stack underflow.");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Pushes an object onto this stack.
        /// </summary>
        /// <param name="value">    The object to push. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Push(Accumulator value)
        {
            MakeRoom(_count);
            _stack[_count++] = value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Makes a room.
        /// </summary>
        /// <param name="newSize">  Size of the new. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MakeRoom(int newSize)
        {
            while (newSize + 1 > _stack.Count)
            {
                _stack.Add(new Accumulator());
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets a value.
        /// +---------------------------------+
        /// |         old values              |
        /// +---------------------------------+
        /// |           param 2               |  basePtr-1
        /// +---------------------------------+
        /// |           param 1               | basePtr-1
        /// +---------------------------------+
        /// |         old baseptr             |  basePtr-0
        /// +---------------------------------+
        /// |           local 1               | basePtr+1
        /// +---------------------------------+
        /// |           local 2               | basePtr+2
        /// +---------------------------------+
        /// |           stack top             |  _count-1
        /// +---------------------------------+.
        /// </summary>
        /// <param name="value">    The object to push. </param>
        /// <param name="index">    one-based index of the. </param>
        /// <param name="isStatic"> (optional) true if is static. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetValue(Accumulator value, int index, bool isStatic = false)
        {
            var i = isStatic ? index : (index + _basePtr);
            MakeRoom(i);
            _stack[i] = value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a value.
        /// +---------------------------------+
        /// |         old values              |
        /// +---------------------------------+
        /// |           param 2               |  basePtr-1
        /// +---------------------------------+
        /// |           param 1               | basePtr-1
        /// +---------------------------------+
        /// |         old baseptr             |  basePtr-0
        /// +---------------------------------+
        /// |           local 1               | basePtr+1
        /// +---------------------------------+
        /// |           local 2               | basePtr+2
        /// +---------------------------------+
        /// |           stack top             |  _count-1
        /// +---------------------------------+.
        /// </summary>
        /// <param name="index">   one-based index of the. </param>
        /// <param name="isStatic"> (optional) true if is static. </param>
        /// <returns>
        /// The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Accumulator GetValue(int index, bool isStatic = false)
        {
            var i = isStatic ? index : (index + _basePtr);

            System.Diagnostics.Debug.Assert( i>=0 && i<_stack.Count,"Stack index invalid." );
        
            return  i>=0 && i<_stack.Count ? _stack[i]: new Accumulator();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Pushes an object onto this stack.
        /// </summary>
        /// <param name="value">    The object to push. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Push(long value)
        {
            Push(new Accumulator(value));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Pushes an object onto this stack.
        /// </summary>
        /// <param name="index">    one-based index of the. </param>
        /// <param name="isStatic"> true if is static. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Push(int index, bool isStatic)
        {
            Push(GetValue(index,isStatic));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes and returns the top-of-stack object.
        /// </summary>
        /// <param name="index">    one-based index of the. </param>
        /// <param name="isStatic"> true if is static. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Pop(int index, bool isStatic)
        {
           SetValue(Pop(), index, isStatic );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Pushes an object onto this stack.
        /// </summary>
        /// <param name="value">    The object to push. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Push(object value)
        {
            Push( new Accumulator( value ) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Call enter.  Parameters are NOT on the stack yet.  
        /// +---------------------------------+
        /// |         lastptr                 | 
        /// +---------------------------------+
        /// |           param 2               |  basePtr-1
        /// +---------------------------------+
        /// |           param 1               | basePtr-1
        /// +---------------------------------+
        /// |         old baseptr             |  basePtr-0
        /// +---------------------------------+
        /// |           local 1               | basePtr+1
        /// +---------------------------------+
        /// |           local 2               | basePtr+2
        /// +---------------------------------+
        /// |           stack top             |  _count-1
        /// +---------------------------------+.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CallEnter(int frameSize)
        {
            PushBasePtr();
            MakeRoom(_count+frameSize+10);
            _basePtr = _count - 1;
            _count  += frameSize;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Pushes the base pointer.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void PushBasePtr()
        {
            var b = new Accumulator( _basePtr );
            b.IsBasePtr = true;
            Push( b );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Call leave. Adjusts stack to clean up locals.
        /// </summary>
        /// <param name="count">    Number of arguments to remove. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CallLeave(int count)
        {
            _count = _basePtr - count;
            _basePtr = this[_basePtr].BasePtr;
            CallStack.Pop();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Call return. Adjusts stack to clean up locals, leaves the last element on the stack.
        /// </summary>
        /// <param name="count">    Number of arguments to remove. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CallReturn(int count)
        {
            var last = Pop();
            CallLeave(count);
            Push(last);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes the arguments described by count.
        /// </summary>
        /// <param name="count">    Number of elements in initial stack. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveArguments(int count)
        {
            _count -= count;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            StringBuilder b=new StringBuilder(100);

            int bp = _basePtr;
            int pbp = (int) _stack[_basePtr].LValue;
            int i = _count - 1;

            foreach (var module in CallStack)
            {
                foreach (var variable in ((PBlock) module).Scope.Variables.Values)
                {
                    i = variable.IsStatic ? variable.ValueOffset : variable.ValueOffset +bp;
                    if (i < _stack.Count)
                    {
                        b.AppendFormat("{0} [{1}] = {2}\r\n", variable.Name, i, _stack[i]);
                    }
                    else
                    {
                        b.AppendFormat( "{0} [{1}] = {2}\r\n", variable.Name, i,"?" );
                    }
                    
                    //{
                    //    // is it base ptr relative
                    //    if (i < _staticCount)
                    //    {

                    //        b.AppendFormat("global[{0}]", i - bp);
                    //    }
                    //    else if (i > bp)
                    //    {
                    //        b.AppendFormat(" local[{0}]", i - bp);
                    //    }
                    //    else if (i < bp && i > pbp)
                    //    {
                    //        b.AppendFormat("  arg[{0}]", i - bp);
                    //    }
                    //    else
                    //    {
                    //        b.Append("BasePtr");
                    //    }
                    //    b.AppendFormat(" = {0}\r\n", _stack[i].ToString());
                    //}
                }
            }
            return b.ToString();
        }
    }
}

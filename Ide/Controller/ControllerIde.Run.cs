////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the controller ide. builder class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/10/2015   rcs     Initial Implementation
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

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Ide.Services;
using pc;
using pc.Context;
using ViewModels;
using Views.Editors;

namespace Ide.Controller
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Handles the compiler and code.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial  class   ControllerIde
    {
        private Thread       _runThread;
        private object       _waitForGo    = new object();
        private PStatement   _currentStatement;
        private int          _currentLineNumber;
        private int          _lineIncrement;

        public RuntimeStack RunStack { get { return CompilerService.Context.RunStack; }}

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public ServiceUser User { get { return _serviceUser; } set { _serviceUser = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ControllerIde"/> is running.
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the debugging.
        /// </summary>
        public bool Debugging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the stepping.
        /// </summary>
        public StepEnum Stepping { get; set; }

        /// <summary>
       /// Gets or sets the current bookmark.
       /// </summary>
       IBookmark CurrentBookmark { get; set; }

        /// <summary>
        /// The call stack, used for getting variables etc. Copied from the runstack
        /// </summary>
        public ObservableCollection<string> CallStack { get; set; }

        private bool _executeNext;
        private StringBuilder _vBuilder = new StringBuilder( 50 );

        /// <summary>
        /// Gets or sets the error list.
        /// </summary>
        public ObservableCollection<ViewModelModule> VariableList { get; set; }

        #region Run Support Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the current bookmark.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetCurrentBookmark()
        {
            CurrentBookmark = VMActiveDocument.IconBarManager.HasBookMarkAtCurrent( VMActiveDocument.TextLocation.Line );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the current line.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateDebugState()
        {
            // this should be executed on the UI thread.
            var runtimeStack = RunStack;
            VMActiveDocument.SetCurrentLineBookMark( runtimeStack.CurrentLine );

            // Update the call stack

            UpdateCallStack(runtimeStack);

            UpdateVariables();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Updates the variables.
        /// Expects the call stack to be updated.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateVariables()
        {
            bool newLine = _currentLineNumber != RunStack.CurrentLine;
            if (newLine)
            {
                // simple counter for line number changes
                ++_lineIncrement;
            }

            _currentLineNumber = RunStack.CurrentLine;

            for (int index = 0; index < CallStack.Count; index++)
            {
                var module = CallStack[index];
                if (VariableList.Count > index && module.IndexOf( VariableList[index].Module.Name+"(" )>0)
                {
                    VariableList[index].Refresh(_lineIncrement);
                    continue;
                }
                if (RunStack.CallStack.Count > index)
                {
                    VariableList.Add( new ViewModelModule(RunStack.CallStack[index] ) );
                }
            }

            while (VariableList.Count > CallStack.Count)
            {
                VariableList.RemoveAt(VariableList.Count - 1);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Updates the call stack described by runtimeStack.
        /// </summary>
        /// <param name="runtimeStack"> Stack of runtimes. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateCallStack(RuntimeStack runtimeStack)
        {
            CallStack.Clear();

            // TODO fix this so it doesn't need to redo the whole stack
            // 
            for (int i = 0; i < runtimeStack.CallStack.Count; i++)
            {
                var module = runtimeStack.CallStack[i];
                _vBuilder.Clear();
                _vBuilder.AppendFormat("{0}  {1}( ", module.ModuleType, module.Name);
                for (int index = 0; index < module.Parameters.Count; index++)
                {
                    var parameter = module.Parameters[index];
                    if (index > 0)
                    {
                        _vBuilder.Append(", ");
                    }
                    if (parameter.IsArray)
                    {
                        _vBuilder.AppendFormat("{0}[]", parameter.Name);
                    }
                    else
                    {
                        _vBuilder.AppendFormat("{0}={1}", parameter.Name, parameter.Value.StringValue());
                    }
                }
                _vBuilder.Append(" )");

                CallStack.Add(_vBuilder.ToString());
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Steps the execution thread to run to the next stop
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Step()
        {
            lock (_waitForGo)
            {
                Monitor.Pulse( _waitForGo );
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the stack on request run line operation.
        /// Run thread calls back on this method on each line
        /// If we want to pause the thread we can do it here
        /// 
        /// We pause on breakpoints or if single step, each step.
        /// </summary>
        /// <param name="lineNumber">   The line number. </param>
        /// <param name="element"></param>
        /// <returns>
        /// the state stop if exit.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState RunStackOnRequestRunLine(int lineNumber, PElement element)
        {
            if (lineNumber == 0)
            {
                // this is the Program so no code to exeucute.
                return ReturnState.NEXT;
            }
            // update all the state using a dispath
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,new Action(() => { UpdateDebugState(); }));


            // decide if we should stop and wait for next command
            if (!Debugging) return ReturnState.NEXT;

            if (Stepping == StepEnum.GO)
            {
                if (!CheckForBreakPoint())
                {
                    return ReturnState.NEXT;
                }
            }

            if (_executeNext)
            {
                _executeNext = false;
            }
            else
            {
                switch (Stepping)
                {
                    case StepEnum.STOP:
                        break;

                    case StepEnum.INTO:
                        var m = element as PModule;
                        if (m != null)
                        {
                            break;
                        }
                        goto case StepEnum.STATEMENT;

                    case StepEnum.STATEMENT:
                        var s = element as PStatement;
                        if (s != null)
                        {
                            if (s != _currentStatement)
                            {
                                if (s is PModule)
                                {
                                    Stepping = StepEnum.RETURN;
                                    return ReturnState.NEXT;
                                }
                                _currentStatement = s;
                                break;
                            }
                        }
                        return ReturnState.NEXT;

                    case StepEnum.RETURN:
                        if (!(element is PReturn)) return ReturnState.NEXT;
                        _executeNext = true;
                        Stepping = StepEnum.STOP;
                        return ReturnState.NEXT;

                    case StepEnum.GO:
                        if (CheckForBreakPoint())
                        {
                            break;
                        }
                        return ReturnState.NEXT;
                }
            }


            // wait for next pulse and new command Stepping
            lock (_waitForGo)
            {
                Monitor.Wait(_waitForGo);
            }
            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Check for break point.
        /// </summary>
        /// <returns>
        /// true if stop on the breakpoint.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool CheckForBreakPoint()
        {
            if (LastVM != null)
            {
                foreach (var bp in LastVM.Breakpoints )
                {
                    if (bp != LastVM.CurrentLineBookmark && bp.Enabled && bp.LineNumber == RunStack.CurrentLine)
                    {
                        Stepping = StepEnum.STATEMENT;
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Execution Start/Stop

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Stop execution assumes running thread is terminated.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void StopExecution()
        {
            _lineIncrement = 0;
            // update all the state using a dispath
            if (Application.Current.Dispatcher != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ForceStop()));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Force stop.  Run only on main thread.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ForceStop()
        {
            Running = false;
            IsCompiled = false;
            Stepping = StepEnum.STOP;

            if (_runThread != null && _runThread.IsAlive)
            {
                _runThread.Abort();
                // let it finish
                Thread.Sleep( 100 );
                _runThread = null;
           }
            if (Debugging)
            {
                _executeNext = false;
                RunStack.RequestRunLine -= RunStackOnRequestRunLine;
                LastVM.ShowCurrentLineHighligted = false;
                LastVM.ShowCurrentLineMarker(false);
                Debugging = false;
                Stepping = StepEnum.STOP;
                // cleanup views
                VariableList.Clear();
                CallStack.Clear();
            }
            SendRefresh();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the run operation in a separate thread, it communicates through the User object.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TaskRunStart()
        {
            _currentStatement                 = null;
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Name         = "PseudoRun";
            var context                       = CompilerService.Context;
            context.UserOutput                = User;
            context.UserInput                 = User;
            context.RunStack.Initialize(context.StaticPtr, context.Root);
            try
            {
                context.Root.Execute();
            }
            catch (ThreadAbortException)
            {
                // ok,, its a stop
                //context.UserOutput.Output("\r\n\r\nTerminated by user");
                StopExecution();
            }
            catch (RuntimeError e)
            {
                context.Root.RuntimeError(0, 1, context.RunStack.CurrentLine, "Runtime exception. " + e.Description);
                StopExecution();
            }
            catch (Exception e)
            {
                context.Root.RuntimeFatal(0, 1, context.RunStack.CurrentLine, "Runtime exception.  Please fill out bug report!" + e);
                StopExecution();
            }
            StopExecution();
        }

        #endregion

        #region Run Stop/Stop

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Begins execution of current project or startup project.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RunStart()
        {
           VariableList.Clear();
            Std.Stopwatch.Reset();
            Std.Stopwatch.Start();
            var context = CompilerService.Context;
            if (context.ErrorCounter.ErrorCount == 0 && context.Root != null)
            {
                Running = true;
                UpdateDebugState();
                ShowOutput(true);
                OutputClear();
                _runThread = new Thread(TaskRunStart);
                _runThread.Start();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the stop operation.
        /// </summary>
        /// <param name="reason">   The reason. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RunStop(string reason)
        {
             OutputWriteLine("\r\n"+ reason);

            ForceStop();
        }

        #endregion

        #region Debug

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debugs this object.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DebugStart()
        {
            VMActiveDocument.SetCurrentLineBookMark(1);
            VMActiveDocument.ShowCurrentLineHighligted = true;
            VMActiveDocument.ShowCurrentLineMarker(true);
            RunStack.RequestRunLine += RunStackOnRequestRunLine;
            DebugPause();
            Step();
            RunStart();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug pause.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DebugPause()
        {
            Debugging = true;
            Stepping  = StepEnum.STOP;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the step operation, executes one complete statement.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DebugRun()
        {
            Running   = true;
            Stepping  = StepEnum.GO;
            Debugging = true;
            UpdateDebugState();
            Step();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the step into operation.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DebugStepInto()
        {
            Stepping = StepEnum.INTO;
            UpdateDebugState();
            Step();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the step over operation.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DebugStepOver()
        {
            Stepping = StepEnum.STATEMENT;
            UpdateDebugState();
            Step();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the step return operation.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DebugStepReturn()
        {
            Stepping = StepEnum.RETURN;
            UpdateDebugState();
            Step();
        }

        #endregion

        #region Breakpoints

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug add breakpoint.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugAddBreakpoint()
        {
            if (VMActiveDocument != null)
            {
                VMActiveDocument.AddBreakpoint(VMActiveDocument.TextLocation.Line);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug remove breakpoint.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugRemoveBreakpoint()
        {
            VMActiveDocument.RemoveBreakpoint(VMActiveDocument.TextLocation.Line);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug clear breakpoints.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugClearBreakpoints()
        {
            VMActiveDocument.ClearBookmarks();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug enable breakpoint.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugEnableBreakpoint()
        {
            SetCurrentBookmark();
            if (CurrentBookmark != null)
            {
                CurrentBookmark.Image = BookmarkBase.DefaultBookmarkImage;
                CurrentBookmark.Enabled = true;
            }
            VMActiveDocument.IconBarManager.Redraw();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Debug disable breakpoint.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DebugDisableBreakpoint()
        {
            SetCurrentBookmark();

            if (CurrentBookmark != null)
            {
                CurrentBookmark.Image = BookmarkBase.DefaultBookmarkImageDisable;
                CurrentBookmark.Enabled = false;
            }
            VMActiveDocument.IconBarManager.Redraw();
        }

        #endregion
    }
}

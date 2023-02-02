////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the context class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/25/2015   rcs     Initial Implementation
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
using System.Windows;
using Lift;
using LiftLib.View;
using pc.Context;
using ZCore;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Context. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PContext
    {
        /// <summary>
        /// my source.
        /// </summary>
        public  PseudoSource Source;

        /// <summary>
        /// my output + errors.
        /// </summary>
        public  IUserOutput UserOutput;

        /// <summary>
        /// my output + errors.
        /// </summary>
        public IUserInput UserInput;

        /// <summary>
        /// my output + errors.
        /// </summary>
        public IErrorOutput ErrorOutput;

        /// <summary>
        ///  The Elevator controller 
        /// </summary>
        public IPCElevator Elevator;

        /// <summary>
        ///  The Statistics interface to use
        /// </summary>
        public IStats Stats;

        /// <summary>
        /// my Parser.
        /// </summary>
        public PseudoParser Parser;

        /// <summary>
        /// The scope stack, for keeping track of variables
        /// </summary>
        public Stack<PBlock>  ScopeStack = new Stack<PBlock>();

        /// <summary>
        /// The file paths to compilation units
        /// </summary>
        public List<string> FilePaths;

        /// <summary>
        /// The unresolved calls
        /// </summary>
        public List<PCall> UnresolvedCalls  = new List<PCall>();

        /// <summary>
        /// The unresolved functions
        /// </summary>
        public List<PExpressionReference> UnresolvedFunctions  = new List<PExpressionReference>();


        /// <summary>
        /// The unresolved references
        /// </summary>
        public List<PExpressionReference> UnresolvedReferences = new List<PExpressionReference>(); 
      
        /// <summary>
        /// The classes that are defined.
        /// </summary>
        public List<PClass> Classes  = new List<PClass>(); 

        /// <summary>
        /// The root program
        /// </summary>
        public PProgram Root;

        /// <summary>
        /// The do level, indicates if >0 that a While is a match to a Do
        /// </summary>
        public int DoLevel;

        /// <summary>
        /// Occurs when [error is being reported from the compiler]
        /// </summary>
        public event ReportError Reporter;

        /// <summary>
        /// The error counter
        /// </summary>
        public ErrorCounter ErrorCounter;

        /// <summary>
        /// The run stack
        /// </summary>
        public RuntimeStack RunStack;

        ///// <summary>
        ///// The return call stack
        ///// </summary>
        //public Stack<int> ReturnStack = new Stack<int>(50);

        /// <summary>
        /// The base PTR used for call stack (also used during compile to set variable indexes)
        /// </summary>
        public int BasePtr = 0;

        /// <summary>
        /// The static PTR used keeps track of static and program level global variables.
        /// </summary>
        public int StaticPtr = 0;


        /// <summary>
        /// True if currently in method, parsing, this could be the top of scope stack too.
        /// </summary>
        public PModule CurrentMethod
        {
            get
            {
                var m = ScopeStack.Count>0? ScopeStack.Peek():null;
                return m as PModule;
            }
        }

        /// <summary>
        /// Gets or sets the extensions.
        public List<PModule> Extensions { get; set; }

        /// <summary>
        /// Gets or sets the current class.
        /// </summary>
        public PClass CurrentClass { get; set; }

        /// <summary>
        ///  Get/Set No Set required on assignment statements.
        /// </summary>
        public bool NoSet { get; set; }

        /// <summary>
        ///  Get/Set Main Window.
        /// </summary>
        public Window MainWindow { get; set; }

        /// <summary>
        ///  Get/Set Elevator Window.
        /// </summary>
        public IBuilding ElevatorWindow { get; set; }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Constructor.
        ///// </summary>
        ///// <param name="source">   Source for the. </param>
        ///// <param name="error">    The error. </param>
        ///// <param name="parser">   The Parser. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public PContext(PseudoSource source, IUserOutput error, PseudoParser parser)
        //{
        //    Source = source;
        //    UserOutput  = error;
        //    Parser = parser;
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="errorCounter"> (Optional) The error counter. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PContext(ErrorCounter errorCounter=null)
        {
            FilePaths    = new List<string>();
            ErrorCounter = errorCounter?? new ErrorCounter();
            RunStack     = new RuntimeStack(this,400);
            Extensions   = new List<PModule>(Std.Methods.Count);

            // save context for extensions.
            Std.Context = this;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds  built ins.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddBuiltIns()
        {
            foreach (var builtIn in Std.Methods)
            {
                var name = builtIn.Key;
                var method = builtIn.Value;
                Extensions.Add( new PFunction( this, name, method ) );
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clears the files.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ClearFiles()
        {
            FilePaths.Clear();
            ErrorCounter.Reset();
            UnresolvedFunctions.Clear();
            UnresolvedCalls.Clear();
            UnresolvedReferences.Clear();
            ScopeStack.Clear();
            Classes.Clear();
            StaticPtr = 0;
            DoLevel = 0;
            ClearRuntime();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clears a runtime environment.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ClearRuntime()
        {
            BasePtr = 0;
            RunStack.Clear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clears all brealpoints.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ClearAllBrealpoints()
        {
            Root?.ClearAllBreakpoints();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first variable block.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// The found variable block.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PBlock FindVariableBlock(string name)
        {
            foreach (var block in ScopeStack)
            {
                PVariable variable = block.Scope.Find(name);
                if (variable != null)
                {
                    return block;
                }
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first variable.
        /// </summary>
        /// <param name="name">         The name. </param>
        /// <param name="lineNumber">   The line number. </param>
        /// <returns>
        /// The found variable.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PVariable FindVariable(string name, int lineNumber)
        {
            foreach (var block in ScopeStack)
            {
                PVariable variable = block.Scope.Find( name );
                if (variable != null)
                {
                    if (!variable.References.Contains(lineNumber))
                    {
                        variable.References.Add(lineNumber);
                    }
                    return variable;
                }
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Resolve module.
        /// </summary>
        /// <param name="name">         The name. </param>
        /// <param name="lineNumber">   The line number. </param>
        /// <returns>
        /// The found module.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PModule FindModule(string name,int lineNumber)
        {
            foreach (var module in Root.Modules.Values)
            {
                if (module.Name.Equals(name))
                {
                    if (!module.References.Contains(lineNumber))
                    {
                        module.References.Add(lineNumber);
                    }
                    
                    return module;
                }
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first class.
        /// </summary>
        /// <param name="name">         The name. </param>
        /// <param name="lineNumber">   The line number. -1 don't check line number or add it to references. </param>
        /// <returns>
        /// The found class.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PClass FindClass(string name, int lineNumber)
        {
            foreach (var cls in Classes)
            {
                if (cls.Name != null && cls.Name.Equals( name ))
                {
                    if (cls.LineNumber != -1)
                    {
                        if (!cls.References.Contains(lineNumber))
                        {
                            cls.References.Add(lineNumber);
                        }

                    }
                 return cls;
                }
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Reports.
        /// </summary>
        /// <param name="level">    The do level, indicates if >0 that a While is a match to a Do. </param>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <param name="lineNum">  The line number. </param>
        /// <param name="column">   The column. </param>
        /// <param name="offset">   The offset. </param>
        /// <param name="length">   The length. </param>
        /// <param name="line">     The line. </param>
        /// <param name="errorMsg"> Message describing the error. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Report(ErrorLevel level, string filePath, int lineNum, int column, int offset, int length, string line, string errorMsg)
        {
            if (Reporter != null)
            {
                Reporter(level, filePath, lineNum, column, offset, length, line, errorMsg);
            }
            ErrorOutput.Report(level, filePath, lineNum, column, offset, length, line, errorMsg);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Enter scope of a variable
        /// </summary>
        /// <param name="variable"> The variable. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int EnterScope(PExpressionReference reference)
        {
            int n = ScopeStack.Count;

            var scope = reference.Scope;

            EnterScope(scope);
            return n;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Enter scope.
        /// </summary>
        /// <param name="scope">    The scope. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void EnterScope(PBlock scope)
        {
            if (scope.OuterBlock != null)
            {
                EnterScope(scope.OuterBlock);
            }
            ScopeStack.Push(scope);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Leave scope up to a level.
        /// </summary>
        /// <param name="n">    The n. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LeaveScope(int n)
        {
            while (ScopeStack.Count > n)
            {
                ScopeStack.Pop();
            }
        }
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the tree class
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Program. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PProgram : PModule
    {
        /// <summary>
        /// The title of the program.
        /// </summary>
        public string Title { get { return Name; } set { Name = value; } }

        /// <summary>
        /// The modules in the program
        /// </summary>
        public Dictionary<string,PModule> Modules;

        /// <summary>
        /// Gets or sets a value indicating whether this instance has program statement.
        /// </summary>
        public bool HasProgramStatement { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        /// <param name="title">    The title. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PProgram(PContext context, string title="") : base(context)
        {
            Title      = title;
            ModuleType = "Program";
            Statements = new List<PStatement>();
            Modules    = new Dictionary<string, PModule>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Clear()
        {
            Modules.Clear();
            Statements.Clear();
            Title = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack. The stack has the parameters set set them up into the local frame.
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            Context.RunStack.SetCurrentLine( LineNumber, this );

          //  Context.RunStack.CallEnter(VariableFrameSize);
            var ret = ExecuteBody();
            // now find any modules and look for main
            if (ret == ReturnState.STOP) return ret;
            PModule main = null;
            if (!Modules.TryGetValue("main", out main))
            {
                if (Modules.TryGetValue("Main", out main))
                {
                   return  RuntimeError(main.Offset, 4, 0, "Module Main()  should be spelled as main() .");
                }
            }
            if (main != null)
            {
                return main.Execute();
            }

            if (Modules.Count > 0)
            {
                return RuntimeFatal(0, 1, -1, "Did not find Module main() to start running the program.");
            }
            return ret;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse program statement.
        /// Program text EOL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ParseProgramStatement()
        {
            // program has been recognized so get the text
            Title = Source.CurrentLine.Substring(Source.Position-Source.LineStart).Trim();
            Source.MoveNextLine();
            HasProgramStatement = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parses this element.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            again:
            EnterScope();
            if (!ParseStatements())
            {
                return false;
            }
            LeaveScope();


            if (Source.Token == "End")
            {

                if (Source.ParseToken() == TokenTypeEnum.RESERVED && Source.Reserved == ReservedEnum.MODULE)
                {
                    SyntaxError(-1,-1,"Expected the end of the program but found an End Module? Continuing." );
                    goto again;
                }
                if (HasProgramStatement && Source.MatchTokenCheckCase("Program", "Expected an 'End Program' to be the final statement.",Source.Start))
                {
                    HasProgramStatement = false;
                }
                // end of program
               // return true;
                while (!Source.EOF &&  Source.ParseToken() != TokenTypeEnum.RESERVED )
                {
                    Source.MoveNext();
                }
            }

            if (Source.ExpectedEnd.Count > 0)
            {
                foreach (var type in Source.ExpectedEnd)
                {
                    SyntaxError(Source.Position - 1, 1, "Missing closing:  End {0}",type);

                }
            }
            if (HasProgramStatement) 
            {
                SyntaxError( Source.Position-1, 1,  "Expected an 'End Program' to be the final statement." );
            }


            ResolveClasses();
            ResolveReferences();
            ResolveModules();
            return true;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Resolve references.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ResolveReferences()
        {
            foreach (var reference in Context.UnresolvedReferences)
            {
                int n = Context.EnterScope(reference);
                reference.BindToVariable(reference.Offset,true,true);
                Context.LeaveScope(n);
            }
               
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Resolve classes.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ResolveClasses()
        {
            foreach (var cls in Context.Classes)
            {
                if (cls.LineNumber == -1)
                {
                    var n = cls.Name;
                    SemanticError(cls.Offset, n != null ?n.Length:1, cls.References[0], "Unresolved class {0}:  Class declaration not found.", cls.Name??"?" );
                }
                else
                {
                    // fixup fields by moving fields of super into this one
                    cls.SetFinalFields();
                }
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Resolve modules.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ResolveModules()
        {
            // handle  calls to classes first
            foreach (var call in Context.UnresolvedCalls)
            {
                if (call.ObjClass == null) continue;

                var m = call.ObjClass.RefModule( call.Name );
                if (m == null)
                {
                    m = call.ObjClass.RefFunction( call.Name );
                    if (m == null) continue;

                }
                call.Module = m;
                FixupRefArgs( m, call );
            }

            // handle non class calls

            foreach (var call in Context.UnresolvedCalls)
            {
                if (call.ObjClass != null) continue;

                // it could be a class member so check containing class first.

                if (call.MyClass != null)
                {
                    // check here first.
                    foreach (var module in call.MyClass.Modules)
                    {
                        if (module.Name == call.MethodName)
                        {
                            call.Module = module;
                            call.Arguments.Expressions.Insert(0,new PExpressionReference(Context, "this"));

                            FixupRefArgs( module, call );
                        }
                    }
                    
                }
                
                foreach (var module in Modules.Values)
                {
                    if ( module.FullName == call.MethodName)
                    {
                        call.Module = module;
                        FixupRefArgs(module, call);
                    }
                }
            }
      
            foreach (var call in Context.UnresolvedCalls)
            {
                if (call.Module == null)
                {
                    var callName = call.Name;
                    if (callName == null)
                    {
                        SemanticError(call.ObjectRef.Offset+call.ObjectRef.Length+1, 1, call.LineNumber, "A module name should be here.");

                    }
                    else
                    {
                        SemanticError(call.Offset, callName.Length, call.LineNumber, "Unresolved module {0}.", callName);
                    }
                }
            }

            // Resolve functions

            foreach (var functionRef in Context.UnresolvedFunctions)
            {
                // it could be a class member so check containing class first.

                if (functionRef.MyClass != null)
                {
                    // check here first.
                    var module = functionRef.MyClass.RefFunction(functionRef.Name);
                   // foreach (var module in functionRef.MyClass.Functions)
                   if (module !=null)
                    {
                        if (module.Name == functionRef.Name )
                        {
                            functionRef.Module = module;
                           if ( functionRef.Args.Count>0)
                           {
                               var first = functionRef.Args[0] as PExpressionReference;
                               if (first != null && first.Name == "this") continue;
                           }

                            functionRef.Args.Expressions.Insert(0, new PExpressionReference(Context, "this"));
                            FixupRefArgs( module, functionRef );
                            continue;
                        }
                    }

                }

                foreach (var module in Modules.Values)
                {
                    if (module.Name.Equals(functionRef.Name))
                    {
                        functionRef.Module = module;
                        // fix up ref paramters and handle the return type 
                        FixupRefArgs(module,functionRef);
                    }
                    else if (module.Name.Equals( functionRef.Name,StringComparison.InvariantCultureIgnoreCase ))
                    {
                        SemanticError(module.Offset, module.Name.Length, functionRef.LineNumber,"Function name capitalization error:  Function {0} Called using {1}", module.Name,
                            functionRef.Name);
                        functionRef.Module = module;
                    }
                }
            }

            foreach (var functionRef in Context.UnresolvedFunctions)
            {
                if (functionRef.Module == null && functionRef.ObjectName==null)
                {
                    var cls = Context.Classes.Find(x => x.Name == functionRef.Name );
                    if (cls != null && functionRef.IsConstructor)
                    {
                        // constructor!
                        // bind to classes constructor if it has one
                        var mod = cls.Modules.Find(x => x.Name == functionRef.Name);
                        if (mod != null)
                        {
                            // TODO allow for multiple constructors?
                            functionRef.Module = mod;
                        }
                        else
                        {
                            // default constructor
                            return;
                        }
                    }


                    SemanticError(functionRef.Offset, functionRef.Name.Length, functionRef.LineNumber, "Unresolved Function '{0}':  function not found.", functionRef.Name,functionRef.LineNumber );
                }
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Fixup reference arguments.
        /// </summary>
        /// <param name="module">   The module. </param>
        /// <param name="call">     The call. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FixupRefArgs(PModule module, PCall call)
        {
           module.FixupRefArgs(call);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Fixup reference arguments.
        /// </summary>
        /// <param name="module">   The module. </param>
        /// <param name="call">     The call. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FixupRefArgs(PModule module, PExpressionReference call)
        {
            if (module.Parameters.Count != call.Args.Count)
            {
                SyntaxError(call.Offset, call.Length, "{0} has {1} arguments but Expected {2} on Call {3}\r\nFor example: {4}", call.ToString(), call.Args.Count,module.Parameters.Count, module.Header, module.CallStringExample());
            }

            for (int index = 0; index < module.Parameters.Count; index++)
            {
                var parameter = module.Parameters[index];
                if (parameter.IsRef)
                {
                    var arg = call.Args[index];
                    var eref = arg as PExpressionReference;
                    if (eref == null)
                    {
                        SemanticError( call.Offset, call.Length, call.LineNumber,
                            "Missing argument.  The call to {0} Parameter {1} must be a variable to be passed by reference.\r\nfor instance: {2}",
                            module.Name,
                            parameter.Name,module.CallStringExample());
                    }
                    else
                    {
                        eref.IsRef = true;
                    }
                }
            }
            //TODO mark the return type as being used in an expression or to discard the result
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a module. 
        /// </summary>
        /// <param name="module">   The module. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddModule(PModule module)
        {
            if (!Modules.ContainsKey(module.FullName))
            {
                Modules.Add(module.FullName, module);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.Append("\r\nProgram ");
            builder.AppendLine(Title);
            ToStringBody( builder );
            builder.Indent();
            builder.AppendLine( "End Program" );
        }

    }
}
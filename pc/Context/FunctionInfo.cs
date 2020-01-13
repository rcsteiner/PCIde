////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the function information class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   10/5/2015   rcs     Initial Implementation
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

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Information about the function. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public struct FunctionInfo
    {
        /// <summary>
        /// The name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets the number of arguments.
        /// </summary>
        public int ArgCount { get { return ArgType.Length; } }

        /// <summary>
        /// Type of the argument.
        /// </summary>
        public StoreEnum[] ArgType;

        /// <summary>
        /// The function.
        /// </summary>
        public Std.BuiltIn Function;

        /// <summary>
        /// The return type
        /// </summary>
        public StoreEnum ReturnType;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">     The name. </param>
        /// <param name="function"> The function. </param>
        /// <param name="retType">  Type of the ret. </param>
        /// <param name="argType">  Type of the argument. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FunctionInfo(string name, Std.BuiltIn function, StoreEnum retType, params StoreEnum[] argType)
        {
            Name       = name;
            ArgType    = argType;
            Function   = function;
            ReturnType = retType;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the fun operation.
        /// </summary>
        /// <param name="args">         The arguments. </param>
        /// <param name="elem">         The element. </param>
        /// <param name="this"> Information describing the function. </param>
        /// <returns>
        /// the next state
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ReturnState ExecuteFun(PExpressionList args, PElement elem)
        {
            var Context = elem.Context;
            try
            {
                Accumulator[] carg = new Accumulator[args.Count];

                // check the arg types before going in.
                if (ArgType.Length > 0 && ArgType[0] != StoreEnum.LIST)
                {
                    if (args.Count != ArgCount)
                    {
                        return elem.RuntimeFatal( elem.Offset, elem.Length,elem.LineNumber, "Wrong number of arguments to function: {0}  Found {1}, Expected {2}", Name,
                            args.Count, ArgCount);
                    }
                    ;
                    for (int index = 0; index < args.Count; ++index)
                    {
                        var arg = Context.RunStack.Pop();
                        arg.ConvertTo( ArgType[index] );
                        carg[index] = arg;
                        if (arg.Store != ArgType[index])
                        {
                            var x = args[index];
                            return elem.RuntimeFatal( x.Offset, x.Length,x.LineNumber, "Wrong argument Type to function: {0}  Found {1}, Expected {2}",
                                Name, carg[index].Store, ArgType[index]);
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < args.Count; ++index)
                    {
                        carg[index] = Context.RunStack.Pop();
                    }
                }
                var ret = Function( carg );

                Context.RunStack.Push( ret );
                return ReturnState.NEXT;
            }
            catch (ArgumentException e)
            {
                return elem.RuntimeFatal( elem.Offset, elem.Length, elem.LineNumber, e.Message);
            }
            catch (Exception e)
            {
                return elem.RuntimeFatal( elem.Offset, elem.Length,elem.LineNumber, e.Message);
            }
        }

    }
}
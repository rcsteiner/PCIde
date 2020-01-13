////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the pseudo code compiler class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/24/2015   rcs     Initial Implementation
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
using System.IO;
using ZCore;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Pseudo compiler.
    /// This includes the compiler/intepretor context and the AST Program.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Compiler
    {
        /// <summary>
        /// The context used to store all the parts of the compiler
        /// </summary>
        public PContext Context;

        /// <summary>
        /// The AST tree root
        /// </summary>
        public PProgram Program { get { return Context.Root; } set { Context.Root = value; } }


        /// <summary>
        /// Gets the debug view of the program
        /// </summary>
        public string Debug {get { return Program.ToString(); }}



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Compiler(PContext context)
        {
            Context = context;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compiles the given file.
        /// 1. Sets up the source manager  
        /// 2. Create the parser  
        /// 3. Create the root program  
        /// 4. Read in the source  
        /// 5. Parse the program  
        /// 6. UserOutput success or failure.
        /// 
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <param name="writeListFile"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Compile(string filePath, bool writeListFile = false)
        {
            Context.FilePaths.Add(filePath);
            var _parser    = new PseudoParser(Context);
            var _source    = new PseudoSource(Context, filePath);
            Context.Root   = new PProgram( Context, "" );


            Program.Title  = "Program_"+ Path.GetFileNameWithoutExtension(filePath);
         
            if (!_source.ReadText())
            {
                return;
            }

            //if (Source.Token.Equals( "Program" ))
            //{
            //    // we need to restart and build a new program....
            //    Source.ParseToken();
            //    // TODO: add another program.
            //}

            if (Program.Parse())
            {
                if (_source.Token == "End")
                {
                    _source.ParseToken();
                    if (_source.Token == "Program")
                    {
                        _source.ParseToken();
                    }
                }

                var pos = _source.Position;
                // must be the end of program.
                _source.FlushToToken();

                if (_source.Type != TokenTypeEnum.EOF)
                {
                    Context.Parser.SyntaxError(pos,1, "Something is wrong just before here.  Compiler lost, stoppped processing.  Please send file and report this to improve error messages." );
                }

                Console.WriteLine("Finished - '{0}'  {1}",FileUtil.GetFileNameAndExtension(_source.FilePath), Context.ErrorCounter.SummaryString() );
            }

            if (writeListFile)
            {
                Context.ErrorOutput.WriteListFile(filePath);
            }
            //else
            //{
            //    // dump errors
            //    Context.UserOutput.ListErrors();
            //}
        }
    }
}
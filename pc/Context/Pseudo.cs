////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements a very simple pseudo language interpreter.
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/8/2015   rcs     Initial Implementation
//   4/13/2015  rcs     Fixed [] and ForEach
//   
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
using System.IO;
using System.Reflection;
using ZCore;

namespace pc
{


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// pseudo code compiler/interpreter. 
    /// 
    /// Handles the following types of statements
    /// 
    /// comment
    /// display
    /// declare
    /// set
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Pseudo 
    {

        /// <summary>
        /// Gets the version.
        /// </summary>
        public static string Version { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString();  } }

        /// <summary>
        /// The compiler for pseudo
        /// </summary>
        private static Compiler           _compiler;

        /// <summary>
        /// The output and error tracking module
        /// </summary>
        private static PUserOutput        _output;

        /// <summary>
        /// The error output
        /// </summary>
        private static PErrorOutput _errout;

        /// <summary>
        /// The input module, for user interaction
        /// </summary>
        private static PUserInput         _input;

        /// <summary>
        /// The context of the system
        /// </summary>
        private static PContext           _context;

        /// <summary>
        /// The file paths (all files to parse as a program)
        /// </summary>
        private static List<string>       _filePaths;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Main entry-point for the pseudo compiler.
        /// </summary>
        /// <param name="args"> Array of command-line argument strings. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Main(string[] args)
        {
            try
            {
                if (ProcessCommandLine(args))
                {
                    Compile();
                    Output();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Setup the context to run.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void Setup()
        {
            _context   = new PContext(null);
            _output    = new PUserOutput( _context);
            _errout    = new PErrorOutput( _context );
            _input     = new PUserInput( _context );
            _compiler  = new Compiler(_context);
            _context.Reporter += _errout.Report;
            AddBuiltIns();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds  built ins.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void AddBuiltIns()
        {
            foreach (var builtIn in Std.Methods)
            {
                var name = builtIn.Key.Uncapitalize();
                var method = builtIn.Value;
              //  _context.Root.AddModule( new PFunction( _context, name, "Integer",method ) );

            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compiles the source file[s].
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void Compile()
        {
            foreach (var filePath in _filePaths)
            {
                Setup();
                _compiler.Compile(filePath);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Process the Command line.
        /// </summary>
        /// <param name="args"> Array of command-line argument strings. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private  static bool  ProcessCommandLine(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Pseudo compiler version {0}\r\nUsage: pc FileName(s)",Version);
                return false;
            }
          
            _filePaths = new List<string>();
           
            foreach (var filePath in args)
            {
                if (filePath.Contains("*"))
                {
                    var files = GetFiles(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), false);
                    foreach (var file in files)
                    {
                        CheckFile(file);
                    }
                }
                else
                {
                    CheckFile(filePath);
                }
            }
            return _filePaths.Count > 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Check file exists and report problem.
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static bool CheckFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                _filePaths.Add(filePath);
                return true;
            }
            Console.WriteLine("File: {0} does not exits.", filePath);
            return false;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Gets the names of files (including their paths) that match the specified search pattern in the specified
        /// directory, using a value to determine whether to search subdirectories. 
        /// </summary>
        /// <param name="path">             A path. </param>
        /// <param name="searchPattern">    The search pattern. </param>
        /// <param name="searchSubFolders"> true to search sub folders. </param>
        /// <returns>   
        /// The files. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string[] GetFiles(string path, string searchPattern, bool searchSubFolders = false)
        {
            SearchOption searchOpt = searchSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return Directory.GetFiles( path, searchPattern, searchOpt );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Outputs all errors or listings requested.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void Output()
        {
          //  _context.UserOutput.ListFiles(true);
           
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Runs the entire program.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void Run()
        {
            if (_context.ErrorCounter.ErrorCount == 0 && _context.Root != null)
            {
                _context.Root.Execute();
            }
        }

    }
}

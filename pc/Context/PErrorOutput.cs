////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the error output class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/31/2015   rcs     Initial Implementation
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
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using ZCore;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Error output. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PErrorOutput : IErrorOutput
    {
        private const char LINE_START      = '|';
        private const char LINE_CHAR       = '-';
        private const char ARROW_CHAR      = '^';
        private const string LEFTPAD       = "     |";
        private const string LEFTERR       = "->   |";
        private const string SYMBOL_FORMAT = "{0,8}  {1,9}  {2,27}  {3,8}  {4}";

        /// <summary>
        /// The context
        /// </summary>
        public PContext Context;

        /// <summary>
        /// The listing file
        /// </summary>
        private StreamWriter _listFile;


        /// <summary>
        /// Gets a list of errors.
        /// </summary>
        public ObservableCollection<Error> ErrorList {get { return Context.ErrorCounter.ErrorList; }}

        /// <summary>
        /// Gets the error summary.
        /// </summary>
        public string ErrorSummary {get {return Context.ErrorCounter.SummaryString();}}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PErrorOutput(PContext context)
        {
            Context = context;
            Context.ErrorOutput = this;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Reports the given file.
        /// </summary>
        /// <param name="level">    The level. </param>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <param name="lineNum">  The line number. </param>
        /// <param name="column">   The column. </param>
        /// <param name="offset">   The offset. </param>
        /// <param name="length">   The length. </param>
        /// <param name="line">     The line. </param>
        /// <param name="errorMsg"> Message describing the error. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public  void Report(ErrorLevel level, string filePath, int lineNum, int column, int offset, int length, string line, string errorMsg)
        {
            var record = new ErrorRecord(level, filePath, lineNum, column, offset, length, line, errorMsg);
            Context.ErrorCounter.Record(record);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Dumps the errors out.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ListErrors()
        {
            OutputErrorCount();

            foreach (var record in ErrorList)
            {
                OutputInfo(record.ToString());
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UserOutput error count.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OutputErrorCount()
        {
            OutputInfo(ErrorSummary);
            OutputInfo( "\r\n\r\n" );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// List files out with errors.
        /// </summary>
        /// <param name="writeListFile">    true to write list file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ListFiles(bool writeListFile)
        {
            foreach (string filePath in Context.FilePaths)
            {
                WriteListFile( filePath);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Writes a list file.
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WriteListFile(string filePath )
        {
            try
            {
                StringBuilder builder  = new StringBuilder(1000);
                _listFile             =FileUtil.FileOpenWrite( MakeListFile( filePath ) );
                List<int> items        = new List<int>();
                string[] lines         = File.ReadAllLines(filePath);
                builder.Length         = 0;

                OutputCommentBlock(builder,string.Format("Listing for file: {0}   ( {1} ) Pseudo Compiler V{2}", Path.GetFileNameWithoutExtension(filePath),ErrorSummary,Pseudo.Version));
                OutputInfo(builder.ToString());
                builder.Length        = 0;

                for (int lineNum = 0; lineNum < lines.Length; lineNum++)
                {
                    ErrorEntry(filePath, lineNum + 1, items);
                    string line = lines[lineNum];
                    if (items.Count > 0)
                    {
                        builder.AppendLine(LEFTPAD);
                        FormatLine(builder, lineNum + 1, line);
                        foreach (var index in items)
                        {
                            ListError(builder, index);
                        }
                    }
                    else
                    {
                        FormatLine(builder, lineNum + 1, line);
                    }

                }
                // pick up any after file errors
                for (int i = 0; i < 4; ++i)
                {
                    if( ErrorEntry(filePath, i+lines.Length, items))
                    {
                        foreach (var index in items)
                        {
                            ListError(builder, index);
                        }
                        OutputInfo( builder.ToString() );
                        builder.Length = 0;

                    }
                }


                OutputAST();

                OutputSymbols();

                _listFile.Flush();
                _listFile.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UserOutput symbols.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OutputSymbols()
        {
            StringBuilder b = new StringBuilder();
            OutputSeparator(b,"Symbol Table");
            b.AppendFormat( SYMBOL_FORMAT, "Scope", "Type ", "Name      ", " Defined", "       Used In" );
            b.AppendLine();
            b.AppendFormat( SYMBOL_FORMAT, "--------", "-------", "---------------------------", "--------", "---------------------------------" );
            b.AppendLine();
            OutputInfo(b.ToString());
            b.Length = 0;
            foreach (var variable in ((PBlock) Context.Root).Scope.Variables.Values)
            {
                OutputSymbol(b, "Global", variable.Type, variable.Name, variable.LineNumber, variable.References);
            }
            foreach (var module in Context.Root.Modules.Values)
            {
                var function = module as PFunction;
                var type = (function != null) ? function.ReturnType : "";
                OutputSymbol( b, function != null ? "Function":"Module",type, module.Name, module.LineNumber, module.References );

                foreach (var variable in ((PBlock) module).Scope.Variables.Values)
                {
                    OutputSymbol( b, "Local", variable.Type, variable.Name, variable.LineNumber, variable.References );
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UserOutput symbol.
        /// </summary>
        /// <param name="builder">      The text. </param>
        /// <param name="scope">        The scope. </param>
        /// <param name="type">         The type. </param>
        /// <param name="name">         The name. </param>
        /// <param name="definedIn">    The defined in. </param>
        /// <param name="referencedIn"> The referenced in. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OutputSymbol(StringBuilder builder, string scope, string type, string name, int definedIn, List<int> referencedIn )
        {
            builder.AppendFormat(SYMBOL_FORMAT,scope,type,name,definedIn,"");
            for (int index = 0; index < referencedIn.Count; index++)
            {
                if (index > 0) builder.Append(',');
                builder.Append(referencedIn[index]);
            }
            builder.AppendLine();
            OutputInfo(builder.ToString());
            builder.Length = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UserOutput a st.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OutputAST()
        {
            StringBuilder builder = new StringBuilder();
            OutputSeparator(builder, string.Format("Pseudo Compiler version {0} Rendering of Program AST: ",Pseudo.Version));
            OutputInfo(Context.Root.ToString());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Makes a list file.
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string MakeListFile(string filePath)
        {
            return Path.ChangeExtension(filePath,"pclst");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UserOutput text to the list or console.
        /// </summary>
        /// <param name="text">  The text. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private  void OutputInfo(string text)
        {
            if (_listFile != null)
            {
                _listFile.Write(text);
            }
            else
            {
                Console.Write(text);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UserOutput comment block.
        /// </summary>
        /// <param name="builder">  The text. </param>
        /// <param name="text">     The text comment </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void OutputCommentBlock(StringBuilder builder, string text)
        {
            builder.AppendLine();
            builder.Append("//");
            builder.Append('=', 100);
            builder.AppendLine();
            builder.AppendLine("//  ");
            builder.Append( "//  " );
            builder.AppendLine( text );
            builder.AppendLine( "//  " );
            builder.Append("//");
            builder.Append('=', 100);
            builder.AppendLine();
            builder.AppendLine();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UserOutput separator.
        /// </summary>
        /// <param name="builder">  The text. </param>
        /// <param name="title">    The title. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OutputSeparator(StringBuilder builder, string title)
        {
            builder.AppendLine();
            builder.Append( '=', 100 );
            builder.AppendLine();
            builder.Append(title);
            builder.AppendLine();
            builder.Append( '=', 100 );
            builder.AppendLine();
            OutputInfo( builder.ToString() );
            builder.Length = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Find the Error entry for this line if any.
        /// </summary>
        /// <param name="filePath">     Full pathname of the file. </param>
        /// <param name="lineNumber">   The line number. </param>
        /// <returns>
        /// the index or -1 if none.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ErrorEntry(string filePath, int lineNumber, List<int>  items)
        {
            items.Clear();
            
            for (int index = 0; index < ErrorList.Count; index++)
            {
                var record = ErrorList[index];
                if (record.FilePath.Equals(filePath) && record.LineNumber == lineNumber)
                {
                    items.Add( index);
                }
            }
            return items.Count>0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns the string representation of the error (Line and position marker)
        /// </summary>
        /// <param name="builder">  The text. </param>
        /// <param name="index">   The error record index. </param>
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ListError(StringBuilder builder, int index)
        {
            builder.Append( LEFTERR);
            builder.Append( LINE_CHAR, ErrorList[index].Column +1);
            builder.Append( ARROW_CHAR );
            builder.AppendLine();
            builder.Append( LEFTPAD );
            builder.Append(' ');
            builder.AppendLine( ErrorList[index].Description.Replace("\r\n","  ") );
            builder.AppendLine(LEFTPAD);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Format a program text line.
        /// </summary>
        /// <param name="builder">      The text. </param>
        /// <param name="lineNumber">   The line number. </param>
        /// <param name="line">         The line. </param>
        /// <returns>
        /// indent for the line number.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int FormatLine(StringBuilder builder, int lineNumber, string line)
        {
            builder.AppendFormat( "{0,4} | ", lineNumber );
            int n = builder.Length;
            builder.AppendLine( line );
            return n;
        }
    }
}
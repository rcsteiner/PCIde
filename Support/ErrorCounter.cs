////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the error counter class
//
// Author:
//  Robert C. Steiner
//
// History:
//  ===================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------
//   9/12/2013   rcs     Initial implementation.
//  ===================================================================================
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Data;

namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Error counter structure.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ErrorCounter
    {

        /// <summary>   
        /// Gets or sets the maximum error level found. 
        /// </summary>
        public ErrorLevel MaxErrorLevelFound { get; set; }
    
        /// <summary>
        /// Gets the total errors, fatals and exceptions.
        /// </summary>
        public int Total { get { return ErrorCount + FatalCount + ExceptionCount; } }

        /// <summary>
        /// Gets or sets the note count
        /// </summary>
        public int InfoCount { get; set; }
      
        /// <summary>
        /// Gets or sets the warning count
        /// </summary>
        public int WarningCount { get; set; }
      
        /// <summary>
        /// Gets or sets the error count
        /// </summary>
        public int ErrorCount { get; set; }
     
        /// <summary>
        /// Gets or sets the fatal count
        /// </summary>
        public int FatalCount { get; set; }

        /// <summary>
        /// Gets or sets the exceptions count
        /// </summary>
        public int ExceptionCount { get; set; }

        /// <summary>
        /// Gets or sets the error list.
        /// </summary>
        public ObservableCollection<Error> ErrorList { get; set; }
     
        /// <summary>
        /// Gets the total errors and warnings.
        ///  note: Used by incremental compiler
        /// </summary>
        public int TotalErrorsAndWarnings { get { return WarningCount + ErrorCount + FatalCount + ExceptionCount; } }

        /// <summary>
        /// Gets the dump the erro list out for debug only.
        /// </summary>
        public string Dump { get { return ToString(); } }

        /// <summary>
        /// The_error list lock used for synchronization.
        /// </summary>
        private object _errorListLock = new object();


        /// <summary>
        /// Gets the error summary.
        /// </summary>
        public  string ErrorListSummary { get { return string.Join( "\r\n", ErrorList ); } }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Default constructor. 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ErrorCounter()
        {
            ErrorList = new ObservableCollection<Error>();
            BindingOperations.EnableCollectionSynchronization(ErrorList,_errorListLock );
 
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Resets this instance. 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Reset()
        {
            InfoCount  = WarningCount = ErrorCount = FatalCount = ExceptionCount = 0;
            MaxErrorLevelFound = ErrorLevel.OUTPUT;
            ErrorList.Clear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Logs the specified  Error.
        /// If the RecordFilter delegate is set and it returns false, then the error is not recorded.
        /// </summary>
        /// <param name="error">    The error. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Record(Error error)
        {
            var level = error.Level;

            if (level >= ErrorLevel.WARNING && level > MaxErrorLevelFound)
            {
                MaxErrorLevelFound = level;
            }

            switch (level)
            {
                case ErrorLevel.INFO:
                    ++InfoCount;
                    break;
                case ErrorLevel.WARNING:
                    ++WarningCount;
                    break;
                case ErrorLevel.ERROR:
                    ++ErrorCount;
                    break;
                case ErrorLevel.FATAL:
                    ++FatalCount;
                    break;
                case ErrorLevel.EXCEPTION:
                    ++ExceptionCount;
                    break;
                case ErrorLevel.OUTPUT:
                    return;
            }

            ErrorList.Add(error);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Get a Summary string. 
        /// </summary>
        /// <returns>   
        /// The summary string with counts.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string SummaryString()
        {
            if (Total==0)
            {
                return "  -  No Errors" +
                    (WarningCount > 0 ? string.Format(", {0} Warning{1}", WarningCount, WarningCount>1?'s':' ') : "" ) +
                    (InfoCount    > 0 ? string.Format(", {0} Note{1}",    InfoCount, InfoCount>1?'s':' '):"");
            }
            StringBuilder b = new StringBuilder(50);
          //  b.AppendFormat("Max Error Level = {0}:  ", MaxErrorLevelFound);
            Output(b,ExceptionCount,"Exception");
            Output(b, FatalCount, "Fatal");
            Output(b, ErrorCount, "Error");
            Output( b, WarningCount, "Warning" );
            return b.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Output info with conditional formatting. 
        /// </summary>
        /// <param name="builder">  The builder. </param>
        /// <param name="count">    The count. </param>
        /// <param name="name">     The name. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void Output(StringBuilder builder, int count, string name)
        {
            if (count > 0)
            {
                builder.AppendFormat( "{0} {1}{2} ", count, name, count == 1 ? "" : "s" );
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Members to string. 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(1000);
            int len = ErrorList.Count;
            for (int index = 0; index < len; index++)
            {
                Error error = ErrorList[index];
                builder.AppendLine(error.ToString());
            }
            return builder.ToString();
        }
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the error record class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/30/2015   rcs     Initial Implementation
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
using System.Text;
using ZCore;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Error record for recording errors.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ErrorRecord : Error
    {
        public string Line;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor for an error record.
        /// </summary>
        /// <param name="level">        The level. </param>
        /// <param name="filePath">     Full pathname of the file. </param>
        /// <param name="lineNumber">   The line number. </param>
        /// <param name="column">       The column. </param>
        /// <param name="offset">       The offset. </param>
        /// <param name="length">       The length. </param>
        /// <param name="line">         The line. </param>
        /// <param name="description">  The description. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ErrorRecord(ErrorLevel level, string filePath, int lineNumber, int column, int offset, int length,
            string line, string description) : base(level, filePath, lineNumber, column, offset, description, length)
        {
            LineNumber  = lineNumber;
            Column      = (short) (column > 0 ? column : 0);
            Description = description;
            Line        = line.Replace('\t', ' ');
            FilePath    = filePath;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns the string representation of the error (Line and position marker)
        /// </summary>
        /// <returns>
        /// 3 line representation of the error
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder( 100 );
            var n = FormatLine(builder,LineNumber,Line);
            builder.Append(' ', n);
            builder.Append( '-', Column  );
            builder.Append('^');
            builder.AppendLine();
            builder.Append( ' ', n );
            builder.AppendLine( Description );
            builder.AppendLine();

            return builder.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Format line.
        /// </summary>
        /// <param name="builder">      The builder. </param>
        /// <param name="lineNumber">   The line number. </param>
        /// <param name="line">         The line. </param>
        /// <returns>
        /// The formatted line.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int FormatLine(StringBuilder builder,int lineNumber, string line)
        {
            builder.AppendFormat("{0,4} ", lineNumber);
            int n = builder.Length;
            builder.AppendLine(line);
            return n;
        }
    }
}
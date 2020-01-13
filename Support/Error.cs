////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the error class
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
using System;

namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   
    /// Error description for storage in a list of errors.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
	public class Error 
	{
        private static readonly string[] NAMES = new[] { "output", "trace", "info", "warning", "error" ,"fatal","exception"};

        /// <summary>
        /// Source of error
        /// </summary>
        public  ZPath FilePath {get; protected set;}


        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string FileName { get { return FilePath.FileNameAndExtension; } }
     
        /// <summary>
        /// Description of error.
        /// </summary>
        public  string Description {get; protected set;}

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Offset into an the What if it is a file or source.
        /// </summary>
        public  int Offset {get;private set;}


        /// <summary>
        /// Length of the offending error code.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        public short Column { get; protected set; }

        /// <summary>
        /// level of error.
        /// </summary>
        public ErrorLevel Level { get; private set; }

        /// <summary>
        /// Gets the lower case name for this level.
        /// </summary>
        public string Name { get { return NAMES[(int) Level]; } }

        /// <summary>
        /// Gets a value indicating whether this instance has position.
        /// </summary>
        public bool HasPosition { get { return Column != 0 && LineNumber != 0 && FilePath.FileName!=null; } }
	
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Error with description.
        /// </summary>
        /// <param name="level">        Level of error. </param>
        /// <param name="filePath">     The file path or what. </param>
        /// <param name="lineNumber">   The line number. </param>
        /// <param name="column">       The column. </param>
        /// <param name="offset">       The offset. </param>
        /// <param name="desc">         What happenned. </param>
        /// <param name="length">       (Optional) Length of the offending error code. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Error(ErrorLevel level, ZPath filePath, int lineNumber, int column, int offset, string desc, int length=1) 
		{
			FilePath              = filePath;
			Level                 = level;
			Description           = desc;
            Offset                = offset;
            LineNumber            = lineNumber;
            Column                = (short) column;
            Length                = length;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Error with description.
        /// </summary>
        /// <param name="level">    Level of error. </param>
        /// <param name="desc">     What happenned. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Error(ErrorLevel level, string desc)
        {
            Level       = level;
            Description = desc;
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Returns a <see cref="System.String"/> that represents this instance. 
        /// </summary>
        /// <returns>   
        /// A <see cref="System.String"/> that represents this instance. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            string pos = "";
            if (HasPosition)
            {
                pos = string.Format( "{0} ({1},{2})",FilePath.FileNameAndExtension, LineNumber, Column );
            }
            return string.Format(  "{0,-8}: {1,-30} : {2}", Level, pos,Description);
       }
	}
}

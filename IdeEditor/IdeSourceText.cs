////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the source based on a AvalonEditor document.  Used for parsing and processing document .
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

using System.Text;
using ZCore;
using ICSharpCode.AvalonEdit.Document;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Source character array.  Array with option length and start.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class IdeSourceText : ISourceText
    {
        /// <summary>
        /// Gets or sets the editors document.
        /// </summary>
        private ITextSource _document;

        /// <summary>
        /// Gets the current position stored as an integer. it may be encoded into line/offset or offset.
        /// </summary>
        public int Position
        {
            get { return _position; }
            set
            {
                MoveLineStart( value );
                SetCurrentCharacter();
            }
        }

        /// <summary>
        /// Gets a value indicating whether at end.
        /// </summary>
        public bool AtEnd {get { return _position >= _end; }}

        /// <summary>
        /// Gets or sets the current char.
        /// </summary>
        public char CurrentChar { get; set; }

        /// <summary>
        /// Gets the next char. 0 if at end.
        /// </summary>
        public char NextChar { get { return (_position < _end - 1) ? _document.GetCharAt(_position + 1) : '\0'; } }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Gets the line start.
        /// </summary>
        public int LineStart { get; private set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public ZPath FilePath { get; set; }

        #region Private data


        /// <summary>
        /// start in buffer, default is 0.
        /// </summary>
        private readonly int _start;

        /// <summary>
        /// current position in the buffer
        /// </summary>
        private int _position;

        /// <summary>
        /// end of buffer, can be a sub-string in text. default is length of _text + _start.
        /// </summary>
        private readonly int _end;


        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor, intializes a new string text soruce with optional length and start.
        /// </summary>
        /// <param name="document"> Gets or sets the editors document. </param>
        /// <param name="position"> (Optional) The position. Defaults to 0. </param>
        /// <param name="length">   (Optional) The length. Defaults to the length of the text. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IdeSourceText(ITextSource document, int position = 0, int length = -1)
        {
            _document = document;
            _start = position;
            _end   = length == -1 ? _document.TextLength + _start : length + _start;
            Reset();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Sets the current character. 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetCurrentCharacter()
        {
            CurrentChar = (_document != null && _position < _end) ? _document.GetCharAt(_position) : '\0';
        }

        #region Implementation of ISourceText

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Move next character and sets the current character to new character. 
        /// </summary>
        /// <returns>   
        /// the next character or '\0' if is at the end of document. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public char MoveNext()
        {
            if (_position < _end)
            {
                ++_position;
                if (CurrentChar == '\n')
                {
                    LineStart = _position;
                    ++LineNumber;
                }
                SetCurrentCharacter();
            }
            return CurrentChar;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Move line start relative to current position and adjusts column/row.
        /// </summary>
        /// <param name="newPosition">    Zero-based position to move to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MoveLineStart(int newPosition)
        {
            if (newPosition < _start)
            {
                Reset();
                return;
            }

            if (newPosition > _end)
            {
                newPosition = _end;
            }

            if (newPosition < LineStart)
            {
                // move backwards
                while (LineStart > _start && LineStart > newPosition)
                {
                    MovePrevLine();
                }
            }
            else
            {
                // Move forwards
                for (; _position < _end && _position < newPosition; ++_position)
                {
                    if (_document.GetCharAt(_position) == '\n')
                    {
                        LineStart = _position + 1;
                        ++LineNumber;
                    }
                }
            }
            _position = newPosition;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Move previous line.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MovePrevLine()
        {
            for (int p = LineStart - 2; p > _start; --p)
            {
                if (_document.GetCharAt(p) == '\n')
                {
                    LineStart = p + 1;
                    --LineNumber;
                    return;
                }
            }
            Reset();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Resets the parser back to starting position.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Reset()
        {
            _position = _start;
            LineStart = _start;
            LineNumber = 0;
            SetCurrentCharacter();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the current line.
        /// </summary>
        /// <returns>
        /// The current line.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetCurrentLine()
        {
            if (AtEnd) return null;

            int start = _position;
            while (start >= 0 && _document.GetCharAt(start) != '\n')
            {
                --start;
            }

            ++start;

            int end = _position;
            var b = new StringBuilder(100);
            char c;
            while (end <_document.TextLength && (c= _document.GetCharAt(end)) != '\n')
            {
                ++end;
                b.Append(c);
            }
            --end;
            // trim off CR/LF
            while (_document.GetCharAt(end) < 32)
            {
                --end;
            }

            return b.ToString(0,end-start+1);
        }

        #endregion
    }
}
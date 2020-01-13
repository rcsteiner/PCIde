////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the source class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/23/2015   rcs     Initial Implementation
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
using System.Globalization;
using System.IO;
using System.Text;
using ZCore;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Source. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PseudoSource
    {
        /// <summary>
        /// The line comment text string
        /// </summary>
// public static string COMMENT                 = "//";
        private const string WHITE_SPACE = " \t\r";
        private const string WHITE_SPACE_EOL = " \t\r\n";

        private const string PUNCTUATION = "()[],;{}";
        private const string OPERATOR = "+-*=/><!";
        private const string LETTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string DIGITS = "0123456789";
        private const string LETTERS_DIGITS = LETTERS + DIGITS + "_";

        /// <summary>
        /// The tokens in the current line
        /// </summary>
        public string Token;

        /// <summary>
        /// The current token type
        /// </summary>
        public TokenTypeEnum Type;


        /// <summary>
        /// The start of the token in file offset
        /// </summary>
        public int Start;

        /// <summary>
        /// The token index of the current character
        /// </summary>
        public int Position { get { return _position; } set { _position = value; SetCurrentChar(); } }

        /// <summary>
        /// The token index of the current character
        /// </summary>
        private int _position;

        /// <summary>
        /// The text of the program as an array of lines
        /// </summary>
        public string Text;

        /// <summary>
        /// The current line number.
        /// </summary>
        public int LineNumber;

        /// <summary>
        /// The current reserved word the parser found.
        /// </summary>
        public ReservedEnum Reserved = new ReservedEnum();

        /// <summary>
        /// The current operator the parser found.
        /// </summary>
        public OperatorEnum Operator;

        /// <summary>
        /// Gets or sets the current file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the current character the parser is at.
        /// </summary>
        public char CurrentChar { get; set; }

        /// <summary>
        /// Gets  (peeks) at the next character.
        /// </summary>
        public char NextChar { get { return PeekChar(); } }
      
        /// <summary>
        /// Gets or sets the line start index in the current file
        /// </summary>
        public int LineStart { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current position is End of file.
        /// </summary>
        public bool EOF { get { return _position >= Text.Length; } }

        /// <summary>
        /// Gets a value indicating whethe the current character is punctuation start.
        /// </summary>
        public bool IsPunctuationStart { get { return PUNCTUATION.IndexOf( CurrentChar ) >= 0; } }

        /// <summary>
        /// Gets a value indicating whethe the current character is operator start.
        /// </summary>
        public bool IsOperatorStart { get { return OPERATOR.IndexOf( CurrentChar ) >= 0; } }

        /// <summary>
        /// Gets a value indicating whethe the current character is number start.
        /// </summary>
        public bool IsNumberStart { get { return DIGITS.IndexOf( CurrentChar ) >= 0 || CurrentChar == '.'; } }

        /// <summary>
        /// Gets a value indicating whethe the current character is name start.
        /// </summary>
        public bool IsNameStart { get { return LETTERS.IndexOf( CurrentChar ) >= 0; } }

        /// <summary>
        /// Gets a value indicating whethe the current character is string start.
        /// </summary>
        private bool IsStringStart {get { return CurrentChar == '"'; }}

      
        /// <summary>
        /// Gets or sets the current punctuation character parsed.
        /// </summary>
        public char Punctuation { get; set; }
      
        /// <summary>
        /// Gets the current line being processed
        /// </summary>
        public string CurrentLine { get {  return Text!=null? Text.Substring(LineStart, LineLength(LineStart)).TrimEnd():""; }}


        /// <summary>
        /// Gets the length of the token by subtracting Start from current position
        /// </summary>
        public int TokenLength {get { return _position - Start; }}

        /// <summary>
        /// The output manager for errors and output data
        /// </summary>
        private PseudoParser _output;

        private string AlterQuotes = "‘“”’“";
        private bool _smartQuotes;
        public ReservedEnum PossibleType { get; set; }

        /// <summary>
        ///  The Expected End.
        /// </summary>
        public Stack<string> ExpectedEnd = new Stack<string>(8);


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compute the Line length.
        /// </summary>
        /// <param name="lineStart">    The line start. </param>
        /// <returns>
        /// the computed line length to the LF character.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int LineLength(int lineStart)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                int end = Text.IndexOf('\n', LineStart);
                if (end < 0) end = Text.Length;
                return end-lineStart;
            }
            return 0;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filePath">     Full pathname of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PseudoSource(PContext context, string filePath)
        {
            FilePath       = filePath;
            _output         = context.Parser;
            context.Source = this;
            ExpectedEnd.Clear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Reads all the text of the file into a single string.
        /// </summary>
        /// <returns>
        /// true if it read it.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ReadText()
        {
            LineNumber = 1;
            _position   = 0;

            if (!File.Exists(FilePath))
            {
                _output.SyntaxError(0,1, "File: '{0}' does not exist.",FilePath);
                Text        = "";
                CurrentChar = '\0';
                return false;
            }

            Text = File.ReadAllText( FilePath );
            SetCurrentChar();
            return true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Move next character and return it. We leave the LF as the last character in a line, and on move next, increment
        /// the line number. 
        /// </summary>
        /// <returns>
        /// Character or '\0' if no characters left. (EOF)
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public char MoveNext()
        {
            if (CurrentChar == '\n')
            {
                ++LineNumber;
                LineStart = _position + 1;
            }

            if (_position < Text.Length)
            {
                ++_position;
                if (_position < Text.Length)
                {
                     SetCurrentChar();
                    if  ( AlterQuotes.IndexOf(CurrentChar)>=0)
                    {
                        if (!_smartQuotes)
                        {
                            _output.Warning(_position, 1, "Wrong quotes {0} on a string where found, replacing with straight quotes",CurrentChar);
                            _smartQuotes = true;
                        }
                        CurrentChar = '"';
                    }
                    return CurrentChar;
                }
            }
            return  CurrentChar ='\0';
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the current char.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetCurrentChar()
        {
            CurrentChar = Text.Length > _position ? Text[_position] : '\0';
            if (CurrentChar > 0x7f)
            {
                _output.Warning(_position,1, "This character '{0}' is a unicoded character with a value of {1}", CurrentChar,(int) CurrentChar);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Move to the next line or termnating char.  This is normally used for recovery.
        /// </summary>
        /// <param name="terminateChar">    (Optional) terminating character </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MoveNextLine(char terminateChar='\0')
        {
            while (!EOF )
            {
                if (CurrentChar == terminateChar) break;
                if (CurrentChar == '\n' )
                {
                    MoveNext();
                    return;
                }
                MoveNext();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Move next character and return it. We leave the LF as the last character in a line, and on move next, increment
        /// the line number. 
        /// </summary>
        /// <returns>
        /// Character or '\0' if no characters left. (EOF)
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public char PeekChar(int n=1)
        {
            if (_position+n < Text.Length)
            {
                return  Text[_position+n];
            }
            return '\0';
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Flushes all whitespace.  Stops at LF and end of file.
        /// </summary>
        /// <returns>
        /// Then non-whitespace character or '\0' if at EOF.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public char FlushWhitespace()
        {
            while (WHITE_SPACE.IndexOf(CurrentChar)>=0)
            {
                MoveNext();
            }
            return CurrentChar;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Flushes all whitespace and EOL.  Stops at end of file.
        /// </summary>
        /// <returns>
        /// Then non-whitespace character or '\0' if at EOF.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public char FlushWhitespaceEol()
        {
            while (WHITE_SPACE_EOL.IndexOf( CurrentChar ) >= 0)
            {
                MoveNext();
            }
            return CurrentChar;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Matches character after flushing whitespace
        /// </summary>
        /// <param name="matchChar">   The match char. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool MatchFlush(char matchChar)
        {
            FlushWhitespace();
            return Match( matchChar );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Matches any text sequence and advances position.
        /// </summary>
        /// <param name="character">    The character. </param>
        /// <returns>
        /// true if it succeeds, false if it fails and the position is not changed.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Match(char character)
        {
            Start = _position;
            if (CurrentChar == character)
            {
                MoveNext();
                Token = Text.Substring( Start, 1 );
                Type = TokenTypeEnum.NAME;
                return true;
            }
            return false;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Matches any text sequence and advances position. 
        /// </summary>
        /// <param name="text"> The text of the program as an array of lines. </param>
        /// <returns>
        /// true if it succeeds, false if it fails and the position is not changed.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Match(string text)
        {
            FlushWhitespace();
            int pos = _position;
            for (int i = 0; i < text.Length; i++)
            {
                if (CurrentChar != text[i])
                {
                    Position = pos;
                    return false;
                }
                MoveNext();
            }
            Token = text;
            Type = Reserved.Parse( Token ) ? TokenTypeEnum.RESERVED : TokenTypeEnum.NAME;
            Start = pos;
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Matches any text sequence ignoring case and advances position. 
        /// </summary>
        /// <param name="text"> The text of the program as an array of lines. </param>
        /// <returns>
        /// true if it succeeds, false if it fails and the position is not changed.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool MatchNoCase(string text)
        {
            FlushWhitespace();
            int pos = _position;
            for (int i = 0; i < text.Length; i++)
            {
                if ( Char.ToLower(CurrentChar) != char.ToLower( text[i]))
                {
                    Position = pos;
                    return false;
                }
                MoveNext();
            }
            Token = text;
            Type = TokenTypeEnum.NAME;
            Start = pos;
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Match check case.
        /// </summary>
        /// <param name="text">     The text of the program as an array of lines. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool MatchCheckCase(string text)
        {
            if (Match(text)) return true;
            if (MatchNoCase(text))
            {
                _output.SyntaxError(-1, -1, "Incorrect Case.  Use: {0} correcting...",  text);
                Token = text;
                return true;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Match one of the given matches.
        /// </summary>
        /// <param name="matches">  A variable-length parameters list containing matches. </param>
        /// <returns>
        /// the index or -1 if not found.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int MatchOneOf(params string[] matches)
        {
            FlushWhitespaceEol();
            for (int i = 0; i < matches.Length; i++)
            {
                if (Match(matches[i]))
                {
                    return i;
                    
                }
            }
            return -1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Match end statement of a type. checks for case sensitivity too.
        /// </summary>
        /// <param name="typeName"> Name of the type. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool MatchEndStatement(string typeName)
        {
            if (MatchTokenCheckCase("End", "Expected a {0} found {1} "))
            {
                ParseToken();

                var top = ExpectedEnd.Pop();

                if (MatchTokenCheckCase(typeName, "Expected an End {0} statement here.  Found End {1}"))
                {
                    return true;
                }
                if (Token == typeName)
                {
                    return true;
                }
               _output.SyntaxError( -1, -1, "Expected an End {0} statement here. Found End {1}", typeName, Token );
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Match token check case.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <param name="formatMessage">Message describing the format. FIRST parameter is match word, SECOND parameter is word found</param>
        /// <param name="pos">The position to report.</param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool MatchTokenCheckCase(string match, string formatMessage,int pos=-1)
        {
            if (Token == match)
            {
                return true;
            }

            if (Token.Equals( match, StringComparison.InvariantCultureIgnoreCase ))
            {
                _output.Warning(pos,-1, "Case is wrong. "+ formatMessage+" Fixing.",match, Token );
                Token = match;
                return true;

            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse while.
        /// </summary>
        /// <param name="validChars">   The valid chars. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ParseWhile(string validChars)
        {
            Start = _position;
            while (validChars.IndexOf(CurrentChar ) >= 0)
            {
                MoveNext();
            }
            Token = Text.Substring( Start, _position - Start );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse token but skip end of lines, returns the next token set up.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ParseTokenSkipEol()
        {
            ParseToken();
            while (Type == TokenTypeEnum.EOL)
            {
                ParseToken();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse token in the current line at the current token index. and put it into  token
        /// Look specifically for comments and for string delimiters, then whitespace.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TokenTypeEnum ParseToken(bool skipEol=true)
        {
            while(!EOF)
            {
                FlushWhitespace();
                // check if comment

                if (CurrentChar == '/' && PeekChar() == '/')
                {

                    MoveNext();
                    MoveNext();
                    if (skipEol)
                    {
                        ParseComment();
                        continue;
                    }
                    return ParseComment();
                }

                // find start of a token
                for (; !EOF; MoveNext())
                {
                    if (CurrentChar == '\n')
                    {
                        MoveNext();
                        if (skipEol)
                        {
                            goto cont;
                        }
                        Token = "\n";
                        return  Type=TokenTypeEnum.EOL;
                    }

                    if (IsStringStart)
                    {
                        return ParseString();
                    }
               
                    if (IsNameStart)
                    {
                        return ParseName();
                    }
                 
                    if (IsNumberStart)
                    {
                       return  ParseNumber();
                    }
                 
                    if (IsOperatorStart)
                    {
                        return ParseOperator();
                    }
                  
                    if (IsPunctuationStart)
                    {
                        return ParsePunctuation();
                    }
                    if (CurrentChar == '\'')
                    {
                        return ParseChar();
                    }
                    
                }
            cont: ;
            }
            Token = "";
            return Type = TokenTypeEnum.EOF;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Flushes to token.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void FlushToToken()
        {
            while (ParseToken() == TokenTypeEnum.EOL || Type == TokenTypeEnum.COMMENT)
            {
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse system type.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CheckSystemType()
        {

            if (Type == TokenTypeEnum.RESERVED)
            {
                switch (Reserved.Value)
                {
                    case ReservedEnum.REAL:
                    case ReservedEnum.INTEGER:
                    case ReservedEnum.STRING:
                    case ReservedEnum.BOOLEAN:
                    case ReservedEnum.CHAR:
                    case ReservedEnum.OBJECT:
                        return true;
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse punctuation.
        /// </summary>
        /// <returns>
        /// the type of token
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TokenTypeEnum ParsePunctuation()
        {
            Type = TokenTypeEnum.PUNCTUATION;
            Start = _position;
            MoveNext();
            Token = Text.Substring( Start, _position - Start );
            return Type;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse comment. Assume start has already been processed.
        /// </summary>
        /// <returns>
        /// the type of token COMMENT
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TokenTypeEnum ParseComment()
        {
            // adjust to start of comment
            Start = _position -2;
            while (!EOF && MoveNext() != '\n')
            {
            }
            Token = Text.Substring( Start, _position - Start );
            Type = TokenTypeEnum.COMMENT;
            return Type;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse operator. Get the longest string of operators we can.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TokenTypeEnum ParseOperator()
        {
            Type = TokenTypeEnum.OPERATOR;
            ParseWhile( OPERATOR );
            return Type;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse number.  1st digit is already parsed, could be a decimal point too.
        /// </summary>
        /// <returns>
        /// type of token, REAL or INTEGER
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TokenTypeEnum ParseNumber()
        {
            Type = TokenTypeEnum.INTEGER;
            Start = _position;

            // can start with . or digits so look if digits

            if (DIGITS.IndexOf(CurrentChar) >= 0)
            {
                while (DIGITS.IndexOf( MoveNext() ) >= 0)
                {
                }
            }
            if (CurrentChar == '.')
            {
                Type = TokenTypeEnum.REAL;
                while (DIGITS.IndexOf(MoveNext()) >= 0)
                {
                }
            }
            Token = Text.Substring( Start, _position - Start );
            return Type;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse name.  First letter already parsed, but not moved Next, allow digits and _ too.
        /// </summary>
        /// <returns>
        /// the type NAME or RESERVED, if RESERVED then Reserved is set to the Match enum value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TokenTypeEnum ParseName()
        {
            ParseWhile( LETTERS_DIGITS );
            Type = Reserved.Parse(Token) ? TokenTypeEnum.RESERVED : TokenTypeEnum.NAME;

            if (Type == TokenTypeEnum.NAME)
            {
                // check for possible key word case issue
                PossibleType = Reserved.ParseNoCase(Token);
            }
            return Type;
       }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse literal character.  Anything can be in the '' block.
        /// </summary>
        /// <returns>
        /// Token is the string with quotes.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TokenTypeEnum ParseChar()
        {

            Start = _position;
            while (!EOF && MoveNext() != '\'' && CurrentChar != '\n')
            {
            }
            if (CurrentChar == '\'')
            {
                MoveNext();
            }
            else
            {
                _output.SyntaxError(Start, TokenLength, "Missing closing quote. " );
            }
            Token = Text.Substring( Start, TokenLength );
            Type = TokenTypeEnum.CHAR;
            return Type;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse string.  Anything can be in the "" block.
        /// Allow for embedded "" using \"
        /// </summary>
        /// <returns>
        /// Token is the string with quotes.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TokenTypeEnum ParseString()
        {
            Start = _position;

            StringBuilder b = new StringBuilder();
            b.Append(CurrentChar);

            while (!EOF && MoveNext() != '"' && CurrentChar != '\n')
            {
                if (CurrentChar == '\\')
                {
                    switch (MoveNext())
                    {
                        case 'r':
                            b.Append('\r');

                            break;
                        case 'n':
                            b.Append('\n');
                            break;

                        case 'f':
                            b.Append('\f');
                            break;

                        case '"':
                            b.Append('"');
                            break;

                        case '\\':
                            b.Append('\\');
                            break;

                        case 'u':
                            MoveNext();
                            int len = b.Length;
                            for (int i = 0; i < 4; i++)
                            {
                                b.Append(CurrentChar);
                                if (i < 3) MoveNext();
                            }
                            int c;
                            if (int.TryParse(b.ToString(len, 4), NumberStyles.HexNumber, null, out c))
                            {
                                b.Length = len;
                                b.Append((char)c);
                            }
                            break;
                    }

                }
                else
                {
                    b.Append(CurrentChar);
                }
            }

            if (CurrentChar == '"')
            {
                MoveNext();
            }
            else
            {
                _output.SyntaxError(Start, TokenLength, "Missing closing quote. ");
            }
            b.Append('"'); // always add this

            Token = b.ToString();
            Type = TokenTypeEnum.STRING;
            return Type;

            //Start = _position;
            //while (!EOF && MoveNext() != '"' && CurrentChar != '\n')
            //{
            //}
            //if (CurrentChar == '"')
            //{
            //    MoveNext();
            //}
            //else
            //{
            //    _output.SyntaxError(Start, TokenLength, "Missing closing quote. ");
            //}
            //Token = Text.Substring(Start, TokenLength);
            //Type = TokenTypeEnum.STRING;
            //return Type;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Query if 'character' is char.
        /// </summary>
        /// <param name="character">    The character. </param>
        /// <returns>
        /// true if char, false if not.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsChar(char character)
        {
            return CurrentChar == character;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return Text.Substring(Position, 20);
        }


    }
}
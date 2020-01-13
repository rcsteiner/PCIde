// Copyright (c) 2009 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Allows producing foldings from a document based on Blocks of Pseudo code.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PcFoldingStrategy
    {

        /// <summary>
        /// Gets or sets/Sets the opening brace. The default value is '{'.
        /// </summary>
        public string[] Open { get; set; }

        public string[] Closing { get; set; }

        public readonly string[] Modifiers = new[] { "Public", "Protected", "Private" };

        /// <summary>
        /// Creates a new BraceFoldingStrategy.
        /// </summary>
        public PcFoldingStrategy()
        {
            Open    = new[] {"Function", "Class", "Module", "For", "If", "While", "ExpectFail", "Select"};
            Closing = new[] {"End Function", "End Class", "End Module", "End For", "End If", "End While", "End ExpectFail","End Select"};
        }

        private const string SCAN_CHARS = "abcdefghijklmnopqrstufwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";

       // private TextParser _parser=new TextParser();


       private List<NewFolding> newFoldings = new List<NewFolding>(8);
       private Stack<Match> startOffsets = new Stack<Match>(8);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Match keeper
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private struct Match
        {
            public int Offset;
            public int MatchId;

            public Match(int offset, int matchId)
            {
                Offset = offset;
                MatchId = matchId;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Create <see cref="NewFolding" />s for the specified document.
        /// </summary>
        /// <param name="document">         The document. </param>
        /// <param name="firstErrorOffset"> [out] The first error offset. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Create <see cref="NewFolding" />s for the specified document.
        /// </summary>
        /// <param name="document"> The document. </param>
        /// <returns>
        /// new foldings.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            // only execute if document changed
            
            var lastNewLineOffset     = 0;
            var text                  = document.Text;
      
           
            newFoldings.Clear();
            startOffsets.Clear();

            // Get line
            var textLength = document.TextLength;
            for (var i = 0; i < textLength; i++)
            {

                var c = document.GetCharAt( i );

                // look for eol
                if (c == '\n')
                {
                    // end of line
                    lastNewLineOffset = i + 1;
                    continue;
                }
                if (c <= ' ')
                {
                    continue;
                }

                // look for comment
                if (c == '/')
                {
                    if (i + 1 < textLength && text[i + 1] == '/')
                    {
                        // comment - scan to EOL
                        // in any case it's not going to match.
                    }
                    while (i < textLength - 1 && text[i] != '\n') ++i;
                    lastNewLineOffset = i + 1;
                    continue;
                }

                // look for name start
                if (Char.IsLetter(c))
                {
                    int start = i;
                    int n;
                    // scan for name that matches one of the key words in open list
                    if ((n=MatchOneOf(text, ref i, Modifiers)) >= 0)
                    {
                        i += Modifiers[n].Length;
                       continue;
                    }
                    if ((n=MatchOneOf(text, ref i, Open)) >= 0)
                    {
                        // If Starts with a Start String, then push
                        startOffsets.Push(new Match(start,n)); ;
                    }
                    else if (startOffsets.Count > 0 && (n=MatchOneOf( text, ref i, Closing )) >= 0)
                    {
                        // if Starts with an End string , then pop
                        var startOffset = startOffsets.Pop();
                        // don't fold if opening and closing brace are on the same line
                        if (startOffset.Offset < lastNewLineOffset && n==startOffset.MatchId)
                        {
                            newFoldings.Add( new NewFolding( startOffset.Offset, start ) );
                        }
                    }
                    // scan to EOL
                    while (i < textLength - 1 && text[i] != '\n') ++i;
                    lastNewLineOffset = i + 1;
                }

            }

            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Match one of.
        /// </summary>
        /// <param name="text">         The text. </param>
        /// <param name="index">        [in,out] Zero-based index of the. </param>
        /// <param name="matchList">    List of matches. </param>
        /// <returns>
        /// -1 if no match.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int MatchOneOf(string text, ref int index, string[] matchList)
        {
            for (int i = 0; i < matchList.Length; i++)
            {
                var match = matchList[i];
                if (index + match.Length > text.Length)
                {
                    continue;
                }
               
                for (int p = 0; p < match.Length; ++p)
                {
                    if (text[index + p] != match[p]) goto next;
                }
                return i;
       
            next:;
            }
            return -1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Updates the foldings.
        /// </summary>
        /// <param name="manager">  The manager. </param>
        /// <param name="document"> The document. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            int firstErrorOffset;
            var newFoldings = CreateNewFoldings(document, out firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }
    }

}
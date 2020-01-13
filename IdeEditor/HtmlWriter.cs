﻿// SharpDevelop samples
// Copyright (c) 2010, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Views
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Implements a text to HTML converter that can be used to save highlighted text as HTML. Source:
    /// https://github.com/xiaochuwang/SharpDevelop-master/blob/master/samples/HtmlSyntaxColorizer/HtmlWriter.cs Usage:
    /// IHighlightingDefinition highlightDefinition = HighlightingManager.Instance.GetDefinition("C#");
    /// HtmlWriter w = new HtmlWriter();
    /// w.ShowLineNumbers = true;
    /// w.AlternateLineBackground = true;
    /// string code = File.ReadAllText("../../Main.cs");
    /// string html = w.GenerateHtml(code, highlightDefinition);
    /// File.WriteAllText("output.html", "
    /// <html>
    ///     <body>" + html + "</body>  </html> ");
    /// Process.Start("output.html"); // view in browser.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class HtmlWriter
    {

        #region fields
        /// <summary>
        /// true to alternate line background.
        /// </summary>
        public bool AlternateLineBackground;

        /// <summary>
        /// true to show, false to hide the line numbers.
        /// </summary>
        public bool ShowLineNumbers;

        /// <summary>
        /// The main style.
        /// </summary>
        public string MainStyle = "font-size: small; font-family: Consolas, \"Courier New\", Courier, Monospace;";

        /// <summary>
        /// The line style.
        /// </summary>
        public string LineStyle = "margin: 0em;";

        /// <summary>
        /// The alternate line style.
        /// </summary>
        public string AlternateLineStyle = "margin: 0em; width: 100%; background-color: #f0f0f0;";

        #region Stylesheet cache

        /// <summary>
        /// Specifies whether a CSS stylesheet should be created to reduce the size of the created code. The default value is
        /// true.
        /// </summary>
        public bool CreateStylesheet = true;

        /// <summary>
        /// Specifies the prefix to use in front of generate style class names.
        /// </summary>
        public string StyleClassPrefix = "code";

        /// <summary>
        /// The stylesheet cache.
        /// </summary>
        private readonly Dictionary<string, string> stylesheetCache = new Dictionary<string, string>();

        /// <summary>
        /// The stylesheet.
        /// </summary>
        private StringBuilder stylesheet = new StringBuilder();
        #endregion

        #endregion fields

        #region Stylesheet cache

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Resets the CSS stylesheet cache. Stylesheet classes will be cached on GenerateHtml calls. If you want to reuse
        /// the HtmlWriter for multiple .html files.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ResetStylesheetCache()
        {
            stylesheetCache.Clear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the class.
        /// </summary>
        /// <param name="style">    The style. </param>
        /// <returns>
        /// The class.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string GetClass(string style)
        {
            string className;
            if (!stylesheetCache.TryGetValue(style, out className))
            {
                className = StyleClassPrefix + stylesheetCache.Count;
                stylesheet.Append('.');
                stylesheet.Append(className);
                stylesheet.Append(" { ");
                stylesheet.Append(style);
                stylesheet.AppendLine(" }");
                stylesheetCache[style] = className;
            }
            return className;
        }

        #endregion Stylesheet cache

        #region methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Generates a html.
        /// </summary>
        /// <param name="code">                 The code. </param>
        /// <param name="highlightDefinition">  The highlight definition. </param>
        /// <returns>
        /// The html.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GenerateHtml(string code, IHighlightingDefinition highlightDefinition)
        {
            return GenerateHtml(new TextDocument(code), highlightDefinition);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Generates a html.
        /// </summary>
        /// <param name="document">             The document. </param>
        /// <param name="highlightDefinition">  The highlight definition. </param>
        /// <returns>
        /// The html.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GenerateHtml(TextDocument document, IHighlightingDefinition highlightDefinition)
        {
            DocumentHighlighter documentHighlighter = null;

            if (highlightDefinition != null)
            {
                documentHighlighter = new DocumentHighlighter(document, highlightDefinition);
            }

            return GenerateHtml(document, documentHighlighter);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Generates a html.
        /// </summary>
        /// <param name="document">     The document. </param>
        /// <param name="highlighter">  The highlighter. </param>
        /// <returns>
        /// The html.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GenerateHtml(TextDocument document, IHighlighter highlighter)
        {
            var myMainStyle = MainStyle;
            var LineNumberStyle = "color: #606060;";

            DocumentLine docline = null;
            string textLine = null;

            if (highlighter == null)
            {
                docline = document.GetLineByNumber(1);
            }

            var output = new StringWriter();
            if (ShowLineNumbers || AlternateLineBackground)
            {
                output.Write("<div");
                WriteStyle(output, myMainStyle);
                output.WriteLine(">");

                var longestNumberLength = 1 + (int) Math.Log10(document.LineCount);

                for (var lineNumber = 1; lineNumber <= document.LineCount; lineNumber++)
                {
                    HighlightedLine line = null;

                    if (highlighter != null)
                    {
                        line = highlighter.HighlightLine(lineNumber);
                    }
                    else
                    {
                        textLine = document.GetText(docline);
                        docline = docline.NextLine;
                    }

                    output.Write("<pre");
                    if (AlternateLineBackground && (lineNumber%2) == 0)
                    {
                        WriteStyle(output, AlternateLineStyle);
                    }
                    else
                    {
                        WriteStyle(output, LineStyle);
                    }
                    output.Write(">");

                    if (ShowLineNumbers)
                    {
                        output.Write("<span");
                        WriteStyle(output, LineNumberStyle);
                        output.Write('>');
                        output.Write(lineNumber.ToString().PadLeft(longestNumberLength));
                        output.Write(":  ");
                        output.Write("</span>");
                    }

                    PrintWords(output, line, textLine);
                    output.WriteLine("</pre>");
                }
                output.WriteLine("</div>");
            }
            else
            {
                output.Write("<pre");
                WriteStyle(output, myMainStyle + LineStyle);
                output.WriteLine(">");
                for (var lineNumber = 1; lineNumber <= document.LineCount; lineNumber++)
                {
                    var line = highlighter.HighlightLine(lineNumber);
                    PrintWords(output, line, textLine);
                    output.WriteLine();
                }
                output.WriteLine("</pre>");
            }
            if (CreateStylesheet && stylesheet.Length > 0)
            {
                var result = "<style type=\"text/css\">" + stylesheet + "</style>" + output;
                stylesheet = new StringBuilder();
                return result;
            }
            return output.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Print words.
        /// </summary>
        /// <param name="writer">   The writer. </param>
        /// <param name="line">     The line. </param>
        /// <param name="text">     The text. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void PrintWords(TextWriter writer, HighlightedLine line, string text)
        {
            if (line != null)
            {
                writer.Write(line.ToHtml(new MyHtmlOptions(this)));
            }
            else
            {
                writer.Write(text);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Writes a style.
        /// </summary>
        /// <param name="writer">   The writer. </param>
        /// <param name="style">    The style. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void WriteStyle(TextWriter writer, string style)
        {
            if (CreateStylesheet)
            {
                writer.Write(" class=\"");
                writer.Write(GetClass(style));
                writer.Write('"');
            }
            else
            {
                writer.Write(" style='");
                writer.Write(style);
                writer.Write("'");
            }
        }

        #endregion methods

        #region private class

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// My html options.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private class MyHtmlOptions : HtmlOptions
        {
            /// <summary>
            /// The html writer.
            /// </summary>
            private readonly HtmlWriter htmlWriter;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="htmlWriter">   The html writer. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            internal MyHtmlOptions(HtmlWriter htmlWriter)
            {
                this.htmlWriter = htmlWriter;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Writes the HTML attribute for the style to the text writer.
            /// </summary>
            /// <param name="writer">   The writer. </param>
            /// <param name="color">    The color. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public override void WriteStyleAttributeForColor(TextWriter writer, HighlightingColor color)
            {
                htmlWriter.WriteStyle( writer, color.ToCss() );
            }
        }

        #endregion private class

    }
}
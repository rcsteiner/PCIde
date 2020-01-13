////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the display class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/25/2015   rcs     Initial Implementation
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
using System.Text;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Display. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PChart : PStatement
    {
        /// <summary>
        /// The expressions.
        /// </summary>
        public PExpression Data;

        public bool GridX;
        public bool GridY;
        public PExpression Title;
        public PExpression XLables;
        public PExpression YLables;
        public PExpression YTitle;
        public PExpression XTitle;

  

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PChart(PContext context) : base(context)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Displays the elements of a comma separated list of tokens.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            if (Source.ParseToken() != TokenTypeEnum.NAME)
            {
                SyntaxError(Source.Position, Source.Token.Length,"Expected the name of an array after Chart\r\nFor Example:Declare int x[10]  ... Chart x");
                Source.MoveNextLine();
                return false;
            }
            Data = new PExpressionReference(Context, Source.Token);

            Data.Parse();

            while (Source.FlushWhitespace() == ',')
            {
                Source.MoveNext();
                Source.ParseToken();
                var command = Source.Reserved.Value;
                switch (command)
                {
                    case ReservedEnum.TITLE:
                     //   Source.MoveNext();
                        if (!ParseEqual("Expected an = after XLables\r\nFor Example: XLables=myLabels")) break;
                        Title=ParseExpression("Expected string name after Title\r\nFor Example: Title=\"My Chart\"");
                        break;

                    case ReservedEnum.XLABLES:
                     //   Source.MoveNext();
                        if (!ParseEqual("Expected an = after XLables\r\nFor Example: XLables=myLabels")) break;
                        XLables =  ParseExpression("Expected the name of an array after XLables\r\nFor Example:Declare string xlabel[10]  ... XLables=xlable");
                        break;

                    case ReservedEnum.YLABLES:
                    //    Source.MoveNext();
                        if (!ParseEqual("Expected an = after YLables\r\nFor Example: YLables=myLabels")) break;
                        YLables = ParseExpression("Expected the name of an array after YLables\r\nFor Example:Declare string ylabel[10]  ... YLables=ylable");
                        break;

                    case ReservedEnum.XTITLE:
                    //    Source.MoveNext();
                        if (!ParseEqual("Expected an = after XTitle\r\nFor Example: XTitle=\"My X values\"")) break;
                        XTitle = ParseExpression("Expected a string expression after XTitle\r\nFor Example:  XTitle=\"My X Title\"");
                        break;

                    case ReservedEnum.YTITLE:
                   //     Source.MoveNext();
                        if (!ParseEqual("Expected an = after YTitle\r\nFor Example: YTitle=\"My Y Title\"")) break;
                        YTitle = ParseExpression("Expected a string expression after YTitle\r\nFor Example:  YTitle=\"My Y Title\"");
                        break;

                    case ReservedEnum.GRID:
                   //     Source.MoveNext();
                        if (!ParseEqual("Expected a '=' after the Grid\r\nFor Example: Grid=X")) break;
                        Source.ParseToken();
                        switch (Source.Reserved.Value)
                        {
                            case ReservedEnum.X:
                                Source.MoveNext();
                                GridX = true;
                                break;

                            case ReservedEnum.Y:
                                Source.MoveNext();
                                GridY = true;
                                break;

                            case ReservedEnum.XY:
                                Source.MoveNext();
                                GridX = true;
                                GridY = true;
                                break;
                            default:
                                SyntaxError(Source.Position, 1,
                                    "Expected an Axis type X, Y or XY after the Grid '=' , for instance Grid=XY");
                                break;
                        }
                        break;
                    default:
                        SyntaxError(Source.Position, Source.TokenLength, "Unknown chart attribute.\r\n The possibilities are: Title, XTitle, YTitle, XLabels, YLabels, Grid ");
                        Source.MoveNextLine();
                        return false;
                }
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Parse the  Expression
        /// </summary>
        /// <param name="errorMessage"> The error Message.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private PExpression ParseExpression(string errorMessage)
        {
            Source.ParseToken();
            var expr = Parser.ParseExpression();
            if (expr == null)
            {
                SyntaxError(Source.Position, Source.TokenLength, errorMessage);
            }
            return expr;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Parse the Equal
        /// </summary>
        /// <param name="errorMessage"> The error Message.</param>
        /// <returns>
        ///  True if successful
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseEqual(string errorMessage)
        {
            if (!Source.Match("="))
            {
                SyntaxError(Source.Position, 1, errorMessage);
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.Append("Chart ");
            if (Data != null)
            {
                builder.Append(Data);
            }

            if (Title != null)
            {
                builder.Append(" , Title=");
                builder.Append(Title);

            }
            if (XLables != null)
            {
                builder.Append(" , XLables=");
                builder.Append(XLables);
            }
            if (YLables != null)
            {
                builder.Append(" , YLables=");
                builder.Append(YLables);
            }
            if (XTitle != null)
            {
                builder.Append(" , XTitle=");
                builder.Append(XTitle);
            }
            if (YTitle != null)
            {
                builder.Append(" , YTitle=");
                builder.Append(YTitle);
            }

            if (GridX || GridY)
            {
                builder.Append(" , Grid=");

                if (GridX) builder.Append('X');
                if (GridY) builder.Append('Y');
            }

            base.ToString( builder );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            base.Execute(); // sets line number

            double[] data    =null;
            string title     = null;
            string xtitle    =null;
            string ytitle    =null;
            string[] xlabels = null;
            string[] ylabels = null;

            // create a chart and populate it

            if (Data.Execute() == ReturnState.STOP)
            {
                return ReturnState.STOP;
            }
            // data array pushed on the stack
            var expr = Context.RunStack.Pop();

            // convert the data to double
            data = new double[expr.Array.Length];
            for (var index = 0; index < expr.Array.Length; index++)
            {
                data[index] = expr.Array[index].DValue;
            }


            if (Title!=null)
            {
                if ( Title.Execute() == ReturnState.STOP)
                {
                    return ReturnState.STOP;
                }
                expr = Context.RunStack.Pop();
                title = expr.SValue;
            }

            if (XTitle != null)
            {
                if (XTitle.Execute() == ReturnState.STOP)
                {
                    return ReturnState.STOP;
                }
                expr = Context.RunStack.Pop();
                xtitle = expr.SValue;
            }

            if (YTitle != null)
            {
                if (YTitle.Execute() == ReturnState.STOP)
                {
                    return ReturnState.STOP;
                }
                expr = Context.RunStack.Pop();
                ytitle = expr.SValue;
            }

            if (XLables != null)
            {
                if (XLables.Execute() == ReturnState.STOP)
                {
                    return ReturnState.STOP;
                }
                expr = Context.RunStack.Pop();
                xlabels = new string[expr.Array.Length];
                for (var index = 0; index < expr.Array.Length; index++)
                {
                    xlabels[index] = expr.Array[index].SValue;
                }
            }

            if (YLables != null)
            {
                if (YLables.Execute() == ReturnState.STOP)
                {
                    return ReturnState.STOP;
                }
                expr = Context.RunStack.Pop();
                ylabels = new string[expr.Array.Length];
                for (var index = 0; index < expr.Array.Length; index++)
                {
                    ylabels[index] = expr.Array[index].SValue;
                }
            }
           
            CreateChart(data, title, xlabels, ylabels, xtitle, ytitle, GridX, GridY);
            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Create the Chart
        /// </summary>
        /// <param name="data">    The data.</param>
        /// <param name="title">   The title.</param>
        /// <param name="xlabels"> The xlabels.</param>
        /// <param name="ylabels"> The ylabels.</param>
        /// <param name="xtitle">  The xtitle.</param>
        /// <param name="ytitle">  The ytitle.</param>
        /// <param name="gridx">   True if gridx.</param>
        /// <param name="gridy">   True if gridy.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void CreateChart(double[] data, string title, string[] xlabels, string[] ylabels, string xtitle, string ytitle, bool gridx, bool gridy)
        {
            UserOutput.OutputChart(data, title, xlabels, ylabels, xtitle, ytitle, gridx, gridy);
        }

    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the symbols class
//
// Author:
//  Robert C. Steiner
//
// History:
//  ===================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------
//   2/4/2014   rcs     Initial implementation.
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LineCharts
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Symbols. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Symbols
    {

        /// <summary>
        /// Gets or sets the border thickness.
        /// </summary>
        public double BorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        public Brush BorderColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the fill.
        /// </summary>
        public Brush FillColor { get; set; }

        /// <summary>
        /// Gets or sets the size of the symbol.
        /// </summary>
        public double SymbolSize { get; set; }

        /// <summary>
        /// Gets or sets the type of the symbol.
        /// </summary>
        public SymbolTypeEnum SymbolType { get; set; }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Symbols()
        {
            SymbolType      = SymbolTypeEnum.None;
            SymbolSize      = 8.0;
            BorderColor     = Brushes.Black;
            FillColor       = Brushes.Black;
            BorderThickness = 1.0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a symbol to 'pt'.
        /// </summary>
        /// <param name="canvas">   The canvas. </param>
        /// <param name="pt">       The point. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddSymbol(Canvas canvas, Point pt)
        {
            var plg = new Polygon();
            plg.Stroke = BorderColor;
            plg.StrokeThickness = BorderThickness;
            var ellipse = new Ellipse();
            ellipse.Stroke = BorderColor;
            ellipse.StrokeThickness = BorderThickness;
            var line = new Line();
            double halfSize = 0.5*SymbolSize;

            Panel.SetZIndex(plg, 5);
            Panel.SetZIndex(ellipse, 5);

            switch (SymbolType)
            {
                case SymbolTypeEnum.Square:
                    plg.Fill = Brushes.White;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.OpenDiamond:
                    plg.Fill = Brushes.White;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.Circle:
                    ellipse.Fill = Brushes.White;
                    ellipse.Width = SymbolSize;
                    ellipse.Height = SymbolSize;
                    Canvas.SetLeft(ellipse, pt.X - halfSize);
                    Canvas.SetTop(ellipse, pt.Y - halfSize);
                    canvas.Children.Add(ellipse);
                    break;
                case SymbolTypeEnum.OpenTriangle:
                    plg.Fill = Brushes.White;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.None:
                    break;
                case SymbolTypeEnum.Cross:
                    line = new Line();
                    Panel.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y + halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y - halfSize;
                    canvas.Children.Add(line);
                    line = new Line();
                    Panel.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    Panel.SetZIndex(line, 5);
                    break;
                case SymbolTypeEnum.Star:
                    line = new Line();
                    Panel.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y + halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y - halfSize;
                    canvas.Children.Add(line);
                    line = new Line();
                    Panel.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    line = new Line();
                    Panel.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y;
                    canvas.Children.Add(line);
                    line = new Line();
                    Panel.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    break;
                case SymbolTypeEnum.OpenInvertedTriangle:
                    plg.Fill = Brushes.White;
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.Plus:
                    line = new Line();
                    Panel.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y;
                    canvas.Children.Add(line);
                    line = new Line();
                    Panel.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    break;
                case SymbolTypeEnum.Dot:
                    ellipse.Fill = FillColor;
                    ellipse.Width = SymbolSize;
                    ellipse.Height = SymbolSize;
                    Canvas.SetLeft(ellipse, pt.X - halfSize);
                    Canvas.SetTop(ellipse, pt.Y - halfSize);
                    canvas.Children.Add(ellipse);
                    break;
                case SymbolTypeEnum.Box:
                    plg.Fill = FillColor;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.Diamond:
                    plg.Fill = FillColor;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.InvertedTriangle:
                    plg.Fill = FillColor;
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.Triangle:
                    plg.Fill = FillColor;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
            }
        }
    }
}
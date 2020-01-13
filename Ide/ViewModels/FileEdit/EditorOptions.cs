////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the editor options class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/15/2015   rcs     Initial Implementation
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
//  USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Reflection;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Editor options. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class EditorOptions : TextEditorOptions
    {
        private string                  _fontFamily;
        private float                   _fontSize;
        private IHighlightingDefinition _highlightDef;
        private bool                    _wordWrap;
        private bool                    _showOutline;

        /// <summary>
        ///     Set the font family
        /// </summary>
        public string FontFamily
        {
            get { return _fontFamily; }
            set { _fontFamily = value; }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        public float FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        /// <summary>
        ///     Gets or sets the highlight def.
        /// </summary>
        public IHighlightingDefinition HighlightDef
        {
            get { return _highlightDef; }
            set { _highlightDef = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [word wrap].
        /// </summary>
        public bool WordWrap
        {
            get { return _wordWrap; }
            set { _wordWrap = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether show outline.
        /// </summary>
        public bool ShowOutline
        {
            get { return _showOutline; }
            set { _showOutline = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public EditorOptions()
        {
            SetDefaults();
        }
      
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Copies this editor options.
        /// </summary>
        /// <returns>
        /// a new copy of this option set.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public EditorOptions(EditorOptions options)
        {
            FieldInfo[] fields = typeof( TextEditorOptions ).GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );

            // copy each value over to 'this'
            foreach (FieldInfo fi in fields)
            {
                if (!fi.IsNotSerialized)
                {
                    fi.SetValue( this, fi.GetValue( options ) );
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the defaults.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetDefaults()
        {
            ConvertTabsToSpaces         = true;
            IndentationSize             = 4;
            EnableHyperlinks            = true;
            EnableTextDragDrop          = true;
            EnableRectangularSelection  = true;
            EnableEmailHyperlinks       = true;
            ShowColumnRuler             = true;
            ShowBoxForControlCharacters = true;
            FontSize                    = 18;
            FontFamily                  = "Consolas";
            WordWrap                    = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets a highlighting.
        /// </summary>
        /// <param name="filePath"> Full pathname of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetHighlighting(string filePath)
        {
            HighlightDef = HighlightingManager.Instance.GetDefinitionByExtension( FileUtil.GetFileExtension(filePath));

        }

    }
}
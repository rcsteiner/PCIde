////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the ide editor.xaml class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   7/25/2015   rcs     Initial Implementation
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

using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Indentation.CSharp;

namespace Views.Editors
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interaction logic for IdeEditor.xaml.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class IdeEditor 
    {
        /// <summary>
        /// Manager for folding.
        /// </summary>
        private FoldingManager foldingManager;

        /// <summary>
        /// The folding strategy.
        /// </summary>
        private object foldingStrategy;

        /// <summary>
        /// The fold count revision checker
        /// </summary>
        private int _foldCount;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event handler. Called by HighlightingComboBox for selection changed events.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetLanguage()
        {
            if (SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {
                switch (SyntaxHighlighting.Name)
                {
                    case "PC":
                        foldingStrategy = new PcFoldingStrategy();
                        break;

                    case "XML":
                        foldingStrategy = new XmlFoldingStrategy();
                        TextArea.IndentationStrategy = new DefaultIndentationStrategy();
                        break;

                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        TextArea.IndentationStrategy = new CSharpIndentationStrategy( Options );
                        foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        TextArea.IndentationStrategy = new DefaultIndentationStrategy();
                        foldingStrategy = null;
                        break;
                }
            }
            if (foldingStrategy != null)
            {
                InstallFoldingManager();
            }
            else
            {
                UninstallFoldingManager();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Installs a folding manager.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void InstallFoldingManager()
        {
            if (foldingManager == null)
            {
                foldingManager = FoldingManager.Install(TextArea);
            }
            UpdateFoldings();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Uninstall folding manager.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void UninstallFoldingManager()
        {
            if (foldingManager != null)
            {
                FoldingManager.Uninstall(foldingManager);
                foldingManager = null;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Updates the foldings.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateFoldings()
        {
            if (foldingStrategy != null && !ShowOutline)
            {
                UninstallFoldingManager();
                return;
            }
            if (!ShowOutline) return;
        
            if (foldingManager == null)
            {
                SetLanguage();
                _changeCount = -1;
            }

            if (_foldCount == _changeCount ) return;

         
          
            _foldCount = _changeCount;
          
            //TODO: make this virtual 
            if (foldingStrategy is PcFoldingStrategy)
            {
                ((PcFoldingStrategy)foldingStrategy).UpdateFoldings( foldingManager, doc );
            }
            else if (foldingStrategy is BraceFoldingStrategy)
            {
                ((BraceFoldingStrategy)foldingStrategy).UpdateFoldings( foldingManager, doc );
            }
            else if (foldingStrategy is XmlFoldingStrategy)
            {
                ((XmlFoldingStrategy)foldingStrategy).UpdateFoldings( foldingManager, doc );
            }
        }
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the error view model class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/20/2015   rcs     Initial Implementation
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
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using Ide.Controller;
using Views.Support;
using ZCore;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// File stats view model. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelOutput : ViewModelTool
    {
        private string           _input;
        private ManualResetEvent _wait;
        private OutputItem       _lastOutputItem;
        private RelayCommand _enterCommand;
        private bool RepeatLine;

        /// <summary>
        /// Gets or sets the output list.
        /// </summary>
       public ObservableCollection<OutputItem> Output { get { return Controller.Output; }}

       /// <summary>
       /// Gets or sets the input.
       /// </summary>
        public string Input
        {
            get { return _input; }
            set { _input = value; RaisePropertyChanged("Input"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [input on new line].
        /// </summary>
        public bool InputOnNewLine { get; set; }


        /// <summary>
        /// Gets the enter command to terminate the input
        /// </summary>
        public RelayCommand EnterCommand { get { return _enterCommand ??(_enterCommand= new RelayCommand(EnterExecute)); } }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="controller">   The controller. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelOutput(ControllerIde controller): base( "Output", controller )
        {
            MessegerRegister();
           //Controller..ActiveDocumentChanged += OnActiveDocumentChanged;
            ContentId    = Constants.OUTPUT_CONTENT_ID;
            IconSource   = IconUtil.GetImage("Info");
            _wait        = new ManualResetEvent(false);

            
            WriteLine("Ok");
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Clear()
        {
            Output.Clear();
            _lastOutputItem = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Text input on lost focus.
        /// </summary>
        /// <param name="sender">           Source of the event. </param>
        /// <param name="routedEventArgs">  Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TextInputOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            _wait.Set();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Enter execute.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void EnterExecute()
        {
            _wait.Set();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// New message is received, filter and decide if we are interesed in it.  
        /// Default is to ignore it.
        /// we look for the user input complete message from the view and capture the data.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="key">      The key. </param>
        /// <param name="payload">  The payload. </param>
        /// <returns>
        /// the message result, what to do.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override MessageResult NewMessage(IMessageListener sender, string key, object payload)
        {
            if (key == "UserInput")
            {
                Input = (string) payload;
                _wait.Set();
                return MessageResult.HANDLED_STOP;
            }
            return MessageResult.IGNORED;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Input string by creating a text box and adding it to the output list, and setting focus.
        /// On exit of the text box set the input variable. 
        /// The calling thread is blocked until it is processed.
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string InputString()
        {
            _wait.Reset();
            // turn on editable
            if (_lastOutputItem == null)
            {
                Write("? ");
            }
            _lastOutputItem.IsEditable = true;
            _lastOutputItem.IsFocus = true;
            if (!_wait.WaitOne(30000))
            {
                Controller.RunStop("Waited too long for input");
            }

            _lastOutputItem.IsFocus = false;
    //        _lastOutputItem.IsEditable = false;
            WriteLine();
            return Input;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds text that can be composited on the output line. (last line)
        /// </summary>
        /// <param name="text"> The string to add. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Write(string text)
        {
            if (RepeatLine)
            {
                RepeatLine = false;

                if (Output.Count > 0)
                {
                    Output.RemoveAt(Output.Count-1);
                }
                _lastOutputItem = new OutputItem( "" );
                Output.Add(_lastOutputItem);
            }

            if (text.EndsWith( "\\r" ))
            {
                // this is a rewrite of the current line on the next write, set flag
                RepeatLine = true;
            }

            var lines = text.Split('\n');
            // split it if line feed, else add to last line

            int n = 0;

            if (_lastOutputItem != null)
            {
                int len = _lastOutputItem.Text.Length;
                int tabs = 8 - (len % 8);
                var line = lines[0].TrimEnd('\r');
                if (line == "\\t")
                {
                    // expand tab
                    line = new string( ' ',tabs );
                }
                else if (line == "\f")
                {
                    Output.Clear();
                }

                _lastOutputItem.Text += line;
                n = 1;
            }

            for ( ; n < lines.Length; n++)
            {
                var line = lines[n].TrimEnd('\r');
                if (line == "\\t")
                {
                    // expand tab
                    line = new string( ' ',8);
                }
                _lastOutputItem = new OutputItem( line );
                Output.Add(_lastOutputItem);
            }

            if (Output.Count > 300)
            {
                Output.RemoveAt(0);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds text..
        /// </summary>
        /// <param name="text"> The string to add. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WriteLine(string text="")
        {

            if (text.EndsWith( "\\r" ))
            {
                // this is a rewrite of the current line on the next write, set flag
                RepeatLine = true;
            }
            else
            {
                Write( text + "\r\n" );
            }
        }

    }
}
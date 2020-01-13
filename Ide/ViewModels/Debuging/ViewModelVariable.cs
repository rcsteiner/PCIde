////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the variable state class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/25/2015   rcs     Initial Implementation
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

using pc;
using Views.Support;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Variable state. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelVariable 
    {
        private static Accumulator EmptyAccumaltor     = new Accumulator();
        private RelayCommand      _itemActivateCommand = new RelayCommand(ItemActivated);

        /// <summary>
        /// The variable.
        /// </summary>
        public PVariable Variable { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public Accumulator Value { get { return Variable != null ? Variable.Value : EmptyAccumaltor; }}

        /// <summary>
        /// Gets the value text.
        /// </summary>
        public string ValueText { get {return Value.StringValue(); } }
      
        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        public string Scope {get { return Variable != null && Variable.Module != null ? Variable.Module.Name : ""; }}

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get {return Variable != null ? Variable.Name : "";}}

        /// <summary>
        /// Gets or sets the index of the run.
        /// </summary>
        public int RunIndex { get { return Variable==null?0: Variable.RunIndex; }}

        /// <summary>
        /// Gets the kind.
        /// </summary>
        public string Kind {get { return Variable==null?"?": Variable.IsArgument ? "Arg " : (Variable.IsConstant) ? "Const" : "Var "; }}

        /// <summary>
        /// Gets the type.
        /// </summary>
        public string Type { get {return (Variable!=null)?Variable.Type:"?"; } }
     
        /// <summary>
        /// Gets the item activate command.
        /// </summary>
        public RelayCommand ItemActivateCommand { get { return _itemActivateCommand; } }


        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        public Accumulator OldValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ViewModelVariable"/> is changed. since
        /// the last sweep, counter compared against sweep count
        /// </summary>
        public int ChangeCount { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ViewModelVariable"/> is changed.
        /// </summary>
        public bool Changed { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="variable"> The variable. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelVariable(PVariable variable)
        {
            Variable = variable;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Item activated.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void ItemActivated()
        {
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
            return string.Format( "{0} = {1}", Variable.Name,Variable.Value);
        }
    }
}
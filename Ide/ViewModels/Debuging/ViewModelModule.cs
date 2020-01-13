////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the view model module class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   9/26/2015   rcs     Initial Implementation
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using pc;

namespace ViewModels
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// View model module. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModelModule
    {
        /// <summary>
        /// Gets or sets the module.
        /// </summary>
        public PModule Module { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get { return Module.Name; } }

        /// <summary>
        /// Gets or sets the variables.
        /// </summary>
        public ObservableCollection<ViewModelVariable> Variables { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="module">   The module. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModelModule(PModule module)
        {
            Module = module;
            Variables = new ObservableCollection<ViewModelVariable>();
            foreach (var variable in ((PBlock) module).Scope.Variables.Values)
            {
                ViewModelVariable item;
                if (variable.IsArray)
                {
                    item = new ViewModelVariableArray(variable);
                }
                else if (variable.IsObject)
                {
                    item = new ViewModelVariableObject( variable );
                }
                else
                {
                    item = new ViewModelVariable(variable);
                }
                Variables.Add(  item);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Refreshes this object.
        /// </summary>
        /// <param name="lineIncrement"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Refresh(int lineIncrement)
        {
            var old = new List<ViewModelVariable>(Variables);
            Variables.Clear();
            foreach (var v in old)
            {
                Variables.Add( v );

                Refresh(lineIncrement, v);
                var obj = v as ViewModelVariableObject;
                if (obj != null)
                {
                    var fields = obj.Fields;
                    var oldm = new List<ViewModelVariable>( fields );
                    fields.Clear();
                    obj.Refresh(  );
                    for (int index   = 0; index < oldm.Count; index++)
                    {
                        if (obj.Fields.Count > index)
                        {
                            var vx = obj.Fields[index];
                            Refresh(lineIncrement, vx);
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Refreshes this object.
        /// </summary>
        /// <param name="lineIncrement">    . </param>
        /// <param name="v">                The v. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Refresh(int lineIncrement,  ViewModelVariable v)
        {
            var Changed = !v.OldValue.Equals(v.Value);
            if (Changed && v.ChangeCount == 0)
            {
                // first change 
                v.ChangeCount = lineIncrement;
                v.Changed = true;
            }
            if (Changed && lineIncrement > (v.ChangeCount + 2))
            {
                v.OldValue = v.Value;
                v.Changed = false;
            }
        }
    }
}
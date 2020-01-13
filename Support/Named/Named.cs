////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the named class
//   Defines a Named object.  
//   and refactoring.
//   
// Author:
//  Robert C. Steiner
//
// History:
//  ===================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------
//   1/31/2014   rcs     Initial implementation.
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
using System;
using System.ComponentModel;

namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Defines a basic object that is documented 
    /// with a name and description.  These objects
    /// can also be part of a hiearchy (NamedTree) and have 
    /// a family and a full name.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public abstract class Named : INamed
    {
        /// <summary>
        /// the name of this object.
        /// </summary>
        [Category( "General" ), Description( "Name of this object" )]
        public string Name { get; set; }
      
        /// <summary>
        /// Get the description of this object
        /// </summary>
        [Category( "General" ), Description( "Description of this object" )]
        public string Description {get { return _description??""; }set { _description = value; }}
        /// <summary>
        /// The description
        /// </summary>
        private string _description;


        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the <see cref="Named"/> class.
        /// </summary>
        /// <param name="name">The name of this object</param>
        /// <param name="description">The description of this object</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Named(string name="New", string description=null)
        {
            Name            = name;
            _description    = description;
        }

        #endregion

        #region INamed Members

        #endregion

        #region IRefactor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Renames this object.
        /// </summary>
        /// <param name="newName">the new name.</param>
        /// <returns>true if successful</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Rename(string newName)
        {
            Name = newName;
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Renames this object.
        /// </summary>
        /// <param name="refObject">The ref object.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">the new name.</param>
        /// <returns>true if successful</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Rename(object refObject, string oldName, string newName)
        {
            if (refObject == this)
            {
                Name = newName;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This instance is about to be deleted.
        /// Cleans up any associations and data.
        /// </summary>
        /// <returns>true if it is ok to delete this object</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Remove(INamed named)
        {
            return true;
        }


        #endregion

        #region Object members

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare two INamed objects
        /// </summary>
        /// <param name="to">Compare to object</param>
        /// <returns> less than zero if less tha,greater than zero if greater than or 0 equal</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int CompareTo(object to)
        {
            return String.Compare(Name, ((Named) to).Name, StringComparison.OrdinalIgnoreCase);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert to name of object as string representation
        /// </summary>
        /// <returns>the string rep</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return Name;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compute the hash code for the name. Case sensitive.
        /// </summary>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare to names of objects, ignoring case
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns> true if equal</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Equals(object obj)
        {
            INamed named = obj as INamed;
            return named!=null && String.Equals(Name,named.Name, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
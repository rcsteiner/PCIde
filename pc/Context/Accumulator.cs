////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the accumalator class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/27/2015   rcs     Initial Implementation
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
using System.Diagnostics;
using ZCore;

namespace pc
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Accumulator. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public struct Accumulator
    {
        /// <summary>
        /// The value stored in the accumulator
        /// </summary>
        public object Value;

        /// <summary>
        /// Gets or sets the object value.
        /// </summary>
        public PObject OValue { get { return Value as PObject;} set { Value = value; } }

        /// <summary>
        /// The double value.
        /// </summary>
        public double DValue { get { return Convert.ToDouble(Value); } set { Value = value; } }

        /// <summary>
        /// The integer value.
        /// </summary>
        public long LValue { get { return Convert.ToInt64(Value); } set { Value = value; } }

        /// <summary>
        /// The integer value.
        /// </summary>
        public int IValue { get { return Convert.ToInt32( Value ); } set { Value = value; } }

        /// <summary>
        /// The string value.
        /// </summary>
        public string SValue { get { return Convert.ToString(Value); } set { Value = value; } }

        /// <summary>
        /// the boolean value.
        /// </summary>
        public bool BValue { get { return (LValue) != 0; } set { LValue = (value ? 1 : 0); } }

        /// <summary>
        /// The character value
        /// </summary>
        public char CValue { get { return (char)LValue; } set { LValue = value; } }

        /// <summary>
        /// Gets or sets the array.
        /// </summary>
        public IPCArray Array { get { return IsArray? (IPCArray)Value: null; } set { Value = value; } }

        /// <summary>
        /// Gets a value indicating whether this instance is array.
        /// </summary>
        public bool IsArray { get { return (Value as IPCArray) != null; } }

        /// <summary>
        ///  Get Function (call back)
        /// </summary>
        public PFunction Function { get { return Value as PFunction;} }


        /// <summary>
        /// The is base PTR
        /// </summary>
        public bool IsBasePtr;

        /// <summary>
        /// Gets or sets the <see cref="Accumulator"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="Accumulator"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Accumulator this[int index]
        {
            get { return (Accumulator)(IsArray ? Array[index] : Value); }
            set
            {
                if (IsArray) Array[index] = value;
                else
                {
                    Value = value;
                }
            }
        }

        /// <summary>
        /// Gets the base PTR.
        /// </summary>
        /// <value>
        /// The base PTR.
        /// </value>
        public int BasePtr
        {
            get
            {
                Debug.Assert(IsBasePtr);
                return IsBasePtr ? (int) LValue : 0;
            }
            set { IsBasePtr = true;
                LValue = value;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public string Type { get { return Store.ToString().ToLower().Capitalize(); } }

        /// <summary>
        /// Gets the value text.
        /// </summary>
        public string ValueText { get { return StringValue(); } }

        /// <summary>
        /// The store type
        /// </summary>
        public StoreEnum Store;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">    The string value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Accumulator(string value)
            : this()
        {
            SValue = value;
            Store = StoreEnum.STRING;
            IsBasePtr = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">    The char value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Accumulator(char value)
            : this()
        {
            CValue = value;
            Store = StoreEnum.CHAR;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">    The string value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Accumulator(long value)
            : this()
        {
            LValue = value;
            Store = StoreEnum.INTEGER;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">    The string value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Accumulator(double value)
            : this()
        {
            DValue = value;
            Store = StoreEnum.REAL;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">    The string value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Accumulator(bool value)
            : this()
        {
            BValue = value;
            Store = StoreEnum.BOOL;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">    The string value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Accumulator(object value)
            : this()
        {
            Value = value;
            Store = StoreEnum.OBJECT;
        }

        #region Converters

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert to double.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ConvertToDouble()
        {
            switch (Store)
            {
                case StoreEnum.BOOL:
                case StoreEnum.NULL:
                    break;

                case StoreEnum.REAL:
                    return true;

                case StoreEnum.STRING:
                    double d;
                    if (Double.TryParse( SValue, out d ))
                    {
                        Store = StoreEnum.REAL;
                        Value = d;
                        return true;
                    }
                    break;
                case StoreEnum.CHAR:
                case StoreEnum.INTEGER:
                    Store = StoreEnum.REAL;
                    DValue = LValue;
                    return true;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert to integer.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ConvertToInteger()
        {
            switch (Store)
            {
                case StoreEnum.BOOL:
                    Store = StoreEnum.INTEGER;
                    LValue = LValue == 0 ? 0 : 1;
                    return true;

                case StoreEnum.NULL:
                    break;

                case StoreEnum.REAL:
                    LValue = (long)DValue;
                    Store = StoreEnum.INTEGER;
                    break;
                case StoreEnum.STRING:
                    long l;
                    if (long.TryParse( SValue, out l ))
                    {
                        LValue = l;
                        Store = StoreEnum.INTEGER;
                        return true;
                    }
                    break;

                case StoreEnum.INTEGER:
                    return true;

                case StoreEnum.CHAR:
                    Store = StoreEnum.INTEGER;
                    return true;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert to bool.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ConvertToBool()
        {
            switch (Store)
            {
                case StoreEnum.BOOL:
                    return true;

                case StoreEnum.NULL:
                    return false;

                case StoreEnum.REAL:
                    BValue = (long)DValue != 0;
                    return true;

                case StoreEnum.STRING:
                    bool b;
                    if (bool.TryParse( SValue, out b ))
                    {
                        BValue = b;
                        Store = StoreEnum.BOOL;
                        return true;
                    }
                    break;
                case StoreEnum.INTEGER:
                    BValue = LValue != 0;
                    Store = StoreEnum.BOOL;
                    return true;

                case StoreEnum.CHAR:
                    BValue = CValue != 0;
                    Store = StoreEnum.BOOL;
                    return true;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert to string.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ConvertToString()
        {
            switch (Store)
            {
                case StoreEnum.BOOL:
                    SValue = BValue ? "true" : "false";
                    Store = StoreEnum.STRING;
                    return true;

                case StoreEnum.NULL:
                    break;

                case StoreEnum.REAL:
                    SValue = DValue.ToString();
                    Store = StoreEnum.STRING;
                    return true;

                case StoreEnum.STRING:
                    return true;

                case StoreEnum.INTEGER:
                    SValue = LValue.ToString();
                    Store = StoreEnum.STRING;
                    return true;

                case StoreEnum.CHAR:
                    SValue = CValue.ToString();
                    Store = StoreEnum.STRING;
                    return true;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert to char.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ConvertToChar()
        {
            switch (Store)
            {
                case StoreEnum.BOOL:
                case StoreEnum.NULL:
                    break;

                case StoreEnum.REAL:
                    CValue = (char)DValue;
                    Store = StoreEnum.CHAR;
                    return true;

                case StoreEnum.STRING:
                    if (SValue.Length == 1)
                    {
                        CValue = SValue[0];
                        Store = StoreEnum.CHAR;
                        return true;
                    }

                    break;
                case StoreEnum.INTEGER:
                    CValue = (char)LValue;
                    Store = StoreEnum.CHAR;
                    return true;

                case StoreEnum.CHAR:
                    return true;
            }

            return false;
        }

        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets a store.
        /// </summary>
        /// <param name="typeName"> Type of the decl. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetStore(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "real":
                    Store = StoreEnum.REAL;
                    break;
                case "integer":
                    Store = StoreEnum.INTEGER;
                    break;
                case "string":
                    Store = StoreEnum.STRING;
                    break;
                case "boolean":
                    Store = StoreEnum.BOOL;
                    break;
                case "char":
                    Store = StoreEnum.CHAR;
                    break;
                default:
                    Store = StoreEnum.OBJECT;
                    break;
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert to.
        /// </summary>
        /// <param name="typeName"> Type of the decl. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Accumulator ConvertTo(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "real":
                    ConvertToDouble();
                    break;
                case "integer":
                    ConvertToInteger();
                    break;
                case "string":
                    ConvertToString();
                    break;
                case "boolean":
                    ConvertToBool();
                    break;
                case "char":
                    ConvertToChar();
                    break;
                default:
                    ConvertToObject();
                    break;
            }
            return this;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert to.
        /// </summary>
        /// <param name="typeName"> Type of the decl. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Accumulator ConvertTo(StoreEnum store)
        {
            switch (store)
            {
                case StoreEnum.NULL:
                    break;
                case StoreEnum.REAL:
                    ConvertToDouble();
                    break;
                case StoreEnum.BOOL:
                    ConvertToBool();
                    break;
                case StoreEnum.STRING:
                    ConvertToString();
                    break;
                case StoreEnum.INTEGER:
                    ConvertToInteger();
                    break;
                case StoreEnum.CHAR:
                    ConvertToChar();
                    break;
                case StoreEnum.OBJECT:
                    ConvertToObject();
                    break;
            }
            return this;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert to object.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ConvertToObject()
        {
            Store = StoreEnum.OBJECT;
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Tests if this Accumulator is considered equal to another.
        /// </summary>
        /// <param name="obj">  The accumulator to compare to this object. </param>
        /// <returns>
        /// true if the objects are considered equal, false if they are not.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Equals(Accumulator obj)
        {
            return obj.Value == Value;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// String value.
        /// </summary>
        /// <returns>
        /// The text
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string StringValue()
        {
            string s;
            string f;
            if (IsBasePtr)
            {
                {
                    s = string.Format("BasePtr={0}", LValue);
                    return s;
                }
            }
            f = "EMPTY";
            if (IsArray)
            {
                f = string.Format("Array[{0}]", Array.Length);
                return f;
            }
            switch (Store)
            {
                case StoreEnum.NULL:
                    f = "Null";
                    break;
                case StoreEnum.REAL:
                    f = DValue.ToString("N4");
                    break;
                case StoreEnum.BOOL:
                    f = BValue.ToString();
                    break;
                case StoreEnum.STRING:
                    f = string.Format("\"{0}\"", SValue);
                    break;
                case StoreEnum.INTEGER:
                    f = LValue.ToString("N0");
                    break;
                case StoreEnum.CHAR:
                    f = string.Format("'{0}'", CValue);
                    break;
                case StoreEnum.OBJECT:
                    f = Value.ToString();
                    break;
            }
            return f;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return string.Format("{0} : {1}", Store, StringValue());
        }
    }
}
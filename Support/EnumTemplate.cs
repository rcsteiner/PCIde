////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the EnumTemplate class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/24/2015   rcs     Initial Implementation
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

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ENUM  EnumTemplate definition.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public struct EnumTemplate : IEnum
    {
        /// <summary>   
        /// The value               
        /// </summary>              
        public byte Value { get; set; }

        //------------------------------------------------------
        // the value table of constants
        //------------------------------------------------------
        public const byte INVALID  = 0;

        // TODO: add values here
        // 
        public const byte END      = 8;
        public const byte MAX_ENUM = END;


        //------------------------------------------------------
        /// <summary>
        /// The  string names in lowercase. Index matches Month enum values
        /// NOTE: this can be internationalized.
        /// </summary>
        //------------------------------------------------------
        public static readonly string[] Names =
        {
            "Invalid",

            // TODO: add strings here
            
            "End"
        };

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor given the numeric value .
        /// </summary>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public EnumTemplate(byte value): this()
        {
            Value = value > MAX_ENUM ? INVALID : value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor for EnumTemplate given a numeric value.
        /// </summary>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public EnumTemplate(int value) : this( (byte)value )
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// case the name to an enum.
        /// </summary>
        /// <param name="name"> the text name to convert to an enum. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator EnumTemplate(string name)
        {
            var v = new EnumTemplate( INVALID );
            v.Parse( name );
            return v;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// cast a value to the enum casting operator. 
        /// </summary>
        /// <param name="value"> value as an integer to convert</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator EnumTemplate(int value)
        {
            return new EnumTemplate( (byte)value );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// enumValue casting operator.
        /// </summary>
        /// <param name="value">    The value as a byte to convert </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator EnumTemplate(byte value)
        {
            return new EnumTemplate( value );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// cast enum to a byte casting operator. 
        /// </summary>
        /// <param name="enumValue">   The operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator byte(EnumTemplate enumValue)
        {
            return enumValue.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// cast enum to an int casting operator. 
        /// </summary>
        /// <param name="enumValue">   The operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator int(EnumTemplate enumValue)
        {
            return enumValue.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Match an enum name to its value and return if found, setting value.
        /// </summary>
        /// <returns>   
        /// int value,defaults to 0 - INVALID. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Parse(string name)
        {
            for (int i = 1; i < Names.Length; ++i)
            {
                if (name.Equals(Names[i] ) )
                {
                    Value = (byte) i;
                    return true;
                }
            }
            Value = INVALID;
            return false;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Tests if this EnumTemplate is considered equal to another.
        /// </summary>
        /// <param name="other"> the other enum value. </param>
        /// <returns>
        /// true if the enum values are equal are considered equal, false if they are not.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Equals(EnumTemplate other)
        {
            return Value == other.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert this enum into a string representation.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return Names[Value];
        }
    }
}


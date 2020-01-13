////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the reserved words class
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

using System;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// reserved word  enum containing the reserved words + enumeration of words.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public struct ReservedEnum : IEnum
    {
        /// <summary>   
        /// The value               
        /// </summary>              
        public byte Value { get; set; }

        //------------------------------------------------------
        // the value table of constants
        //------------------------------------------------------
        public const byte INVALID     = 0;
        public const byte DECLARE     = 1;
        public const byte SET         = 2;
        public const byte CONSTANT    = 3;
        public const byte REAL        = 4;
        public const byte INTEGER     = 5;
        public const byte STRING      = 6;
        public const byte MODULE      = 7;
        public const byte END         = 8;
        public const byte DISPLAY     = 9;
        public const byte INPUT       = 10;
        public const byte REF         = 11;
        public const byte CASE        = 12;
        public const byte SELECT      = 13;
        public const byte SWITCH      = 14;
        public const byte IF          = 15;
        public const byte ELSE        = 16;
        public const byte FUNCTION    = 17;
        public const byte RETURN      = 18;
        public const byte AND         = 19;
        public const byte OR          = 20;
        public const byte NOT         = 21;
        public const byte DEFAULT     = 22;
        public const byte TRUE        = 23;
        public const byte FALSE       = 24;
        public const byte BOOLEAN     = 25;
        public const byte PROGRAM     = 26;
        public const byte WHILE       = 27;
        public const byte UNTIL       = 28;
        public const byte DO          = 29;
        public const byte FOR         = 30;
        public const byte FOREACH     = 31;
        public const byte OBJECT      = 32;
        public const byte CHAR        = 33;
        public const byte CLASS       = 34;
        public const byte PUBLIC      = 35;
        public const byte PROTECTED   = 36;
        public const byte PRIVATE     = 37;
        public const byte CALL        = 38;
        public const byte ASSERT      = 39;
        public const byte EXPECTFAIL  = 40;
        public const byte INCLUDE     = 41;

        public const byte CHART       = 42;
        public const byte TITLE       = 43;
        public const byte XLABLES     = 44;
        public const byte YLABLES     = 45;
        public const byte XTITLE      = 46;
        public const byte YTITLE      = 47;
        public const byte GRID        = 48;
        public const byte X           = 49;
        public const byte Y           = 50;
        public const byte XY          = 51;


        public const byte MAX_ENUM  = XY;


        //------------------------------------------------------
        /// <summary>
        /// The  string names in lowercase. Index matches Month enum values
        /// NOTE: this can be internationalized.
        /// </summary>
        //------------------------------------------------------
        public static readonly string[] Names =
        {
            "Invalid",
            "Declare",
            "Set",
            "Constant",
            "Real", 
            "Integer",
            "String",
            "Module",
            "End",
            "Display",
            "Input",
            "Ref",
            "Case",
            "Select",
            "Switch",
            "If",
            "Else",
            "Function",
            "Return",
            "AND",
            "OR",
            "NOT",
            "Default",
            "True",
            "False",
            "Boolean",
            "Program",
            "While",
            "Until",
            "Do",
            "For",
            "ForEach",
            "Object",
            "Char",
            "Class",
            "Public",
            "Protected",
            "Private",
            "Call",
            "Assert",
            "ExpectFail",
            "Include",

            "Chart",
            "Title",
            "XLabels",
            "YLabels",
            "XTitle",
            "YTitle",
            "Grid",
            "X",
            "Y",
            "XY"
        };



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor from byte.
        /// </summary>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ReservedEnum(byte value) : this()
        {
            Value = value > MAX_ENUM ? MAX_ENUM : value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor from integer.
        /// </summary>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ReservedEnum(int value) : this( (byte)value )
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// reserved word from string name casting operator.
        /// </summary>
        /// <param name="name"> The name of a weekday. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator ReservedEnum(string name)
        {
            var v = new ReservedEnum( INVALID );
            v.Parse( name );
            return v;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// reserved word from int casting operator.
        /// </summary>
        /// <param name="value"> value as an integer to convert</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator ReservedEnum(int value)
        {
            return new ReservedEnum( (byte)value );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// byte reserved word  casting operator.
        /// </summary>
        /// <param name="value">    The value as a byte to convert </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator ReservedEnum(byte value)
        {
            return new ReservedEnum( value );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// bytereserved word  casting operator. 
        /// </summary>
        /// <param name="enumValue">   The operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator byte(ReservedEnum enumValue)
        {
            return enumValue.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// int casting operator. 
        /// </summary>
        /// <param name="enumValue">   The operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator int(ReservedEnum enumValue)
        {
            return enumValue.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Match an enum name to its value and return if found, setting value.
        /// </summary>
        /// <returns>   
        /// int value,defaults to 0. 
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
        /// Tests if this reserved word is considered equal to another.
        /// </summary>
        /// <param name="other">    The day of week to compare to this object. </param>
        /// <returns>
        /// true if the objects are considered equal, false if they are not.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Equals(ReservedEnum other)
        {
            return Value == other.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert this reserved word  into a string representation.
        /// </summary>
        /// <returns>
        /// A string representation of this reserved word .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return Names[Value];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse no case.
        /// </summary>
        /// <param name="name"> The name of a weekday. </param>
        /// <returns>
        /// word if found.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ReservedEnum ParseNoCase(string name)
        {
            for (int i = 1; i < Names.Length; ++i)
            {
                if ( String.Compare(name, Names[i],true )==0)
                {
                    return (byte)i;
                }
            }

            return  INVALID;
        }
    }
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the Operator class
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
    /// ENUM  Operator definition.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public struct OperatorEnum : IEnum
    {
        /// <summary>   
        /// The value               
        /// </summary>              
        public byte Value { get; set; }

        public bool IsLogical { get
        {
            return Value == LAND   || Value == LOR   || Value == NOT    || Value == CMPAE || Value == CMPE ||
                   Value == CMPGT  ||Value == CMPGTE || Value == CMPLT || Value == CMPLTE || Value == CMPNE;
        }}

        // sorted and longest to shortest in each first character

        public const byte INVALID = 0;      //   ""
        public const byte COLON   = 1;      //   ":"
        public const byte CMPNE   = 2;      //   "!=" 
        public const byte NOT     = 3;      //   "!"  
        public const byte EQUMOD  = 4;      //   "%=" 
        public const byte MOD     = 5;      //   "%"  
        public const byte LAND    = 6;      //   "&&" 
        public const byte EQUAND  = 7;      //   "&=" 
        public const byte AND     = 8;      //   "&"  
        public const byte EQUMUL  = 9;      //   "*=" 
        public const byte MUL     = 10;     //   "*"  
        public const byte INC     = 11;     //   "++" 
        public const byte EQUPLS  = 12;     //   "+=" 
        public const byte ADD     = 13;     //   "+"  
        public const byte DEC     = 14;     //   "--" 
        public const byte EQUMIN  = 15;     //   "-=" 
        public const byte PTR     = 16;     //   "->" 
        public const byte SUB     = 17;     //   "-"  
        public const byte RANGE   = 18;     //   ".." 
        public const byte EQUDIV  = 19;     //   "/=" 
        public const byte DIV     = 20;     //   "/"  
        public const byte EQUSHL  = 21;     //   "<<="
        public const byte SHL     = 22;     //   "<<" 
        public const byte CMPLTE  = 23;     //   "<=" 
        public const byte CMPLT   = 24;     //   "<"  
        public const byte CMPE    = 25;     //   "==" 
        public const byte LAMDA   = 26;     //   "=>" 
        public const byte EQU     = 27;     //   "="  
        public const byte EQUSHR  = 28;     //   ">>="
        public const byte CMPGTE  = 29;     //   ">=" 
        public const byte SHR     = 30;     //   ">>" 
        public const byte CMPGT   = 31;     //   ">"  
        public const byte NCOP    = 32;     //   "??" 
        public const byte COND    = 33;     //    "?"             
        public const byte EQUXOR  = 34;     //   "^=" 
        public const byte POW     = 35;     //   "^"  
        public const byte EQUOR   = 36;     //   "|=" 
        public const byte LOR     = 37;     //   "||" 
        public const byte OR      = 38;     //   "|"  
        public const byte CMPAE   = 39;     //   "~=" 
        public const byte BINV    = 40;     //   "~"  

        public const byte POSTINC = 41;
        public const byte POSTDEC = 42;

        public const byte TAN    = 43;
        public const byte COS    = 44;
        public const byte SIN    = 45;
        public const byte ATAN   = 46;
        public const byte ACOS   = 47;
        public const byte ASIN   = 48;
        public const byte ROUND  = 49;
        public const byte RANDOM = 50;
        public const byte ZERO   = 51;
        public const byte ONE    = 52;
        public const byte PI     = 53;
        public const byte NEG    = 54;
        public const byte LOG    = 55;
        public const byte LOG10  = 56;
        public const byte SQRT   = 57;
        public const byte ABS    = 58;
        public const byte XOR    = 59;

        /// <summary>
        /// Defines string representation of enumerations.
        /// </summary>
        public static string[] Names =
            {
                ""          ,     // Invalid 
				":"         ,     // Colon
                "!="        ,     //  CmpNE        
                "!"         ,     // ! not         
                "%="        ,     //  EquMod       
                "MOD"       ,     //  Mod          
                "AND"       ,     //  LAnd         
                "&="        ,     //  EquAnd       
                "&"         ,     //  And          
                "*="        ,     //  EquMul       
                "*"         ,     //  Mul          
                "++"        ,     //  increment    
                "+="        ,     //  EquPls       
                "+"         ,     //  Add          
                "--"        ,     //  decrement    
                "-="        ,     //  EquMin       
                "->"        ,     //  Ptr          
                "-"         ,     //  Sub   
                ".."        ,     //  Range
                "/="        ,     //  EquDiv       
                "/"         ,     //  Div          
                "<<="       ,     //  EquShl       
                "<<"        ,     //  Shl          
                "<="        ,     //  CmpLTE       
                "<"         ,     //  CmpLT        
                "=="        ,     //  CmpE         
                "=>"        ,     //  Lamda        
                "="         ,     //  Equ          
                ">>="       ,     //  EquShr       
                ">="        ,     //  CmpGTE       
                ">>"        ,     //  Shr          
                ">"         ,     //  CmpGT        
                "??"        ,     //  NCOp         
                "?"         ,     //  Conditional         
                "^="        ,     //  EquXor       
                "^"         ,     //  Xor          
                "|="        ,     //  EquOr        
                "OR"        ,     //  LOr          
                "|"         ,     //  Or           
                "~="        ,     //  CmpAE        
                "~"         ,     //  bit invert   
                "++"        ,     //  post increment    
                "--"  ,           //  post increment    
                "tan",
                "cos",
                "sin",
                "atan",
                "acos",
                "asin",
                "round",
                "random",
                "zero",
                "one",
                "pi",
                "neg",
                "log",
                "log10",
                "sqrt",
                "abs",
                "XOR"

            };


        public const byte MAX_ENUM = POSTDEC;



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor given the numeric value .
        /// </summary>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public OperatorEnum(byte value): this()
        {
            Value = value > MAX_ENUM ? INVALID : value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor for Operator given a numeric value.
        /// </summary>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public OperatorEnum(int value) : this( (byte)value )
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// case the name to an enum.
        /// </summary>
        /// <param name="name"> The name of a weekday. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator OperatorEnum(string name)
        {
            var v = new OperatorEnum( INVALID );
            v.Parse( name );
            return v;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// cast a value to the enum casting operator. 
        /// </summary>
        /// <param name="value"> value as an integer to convert</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator OperatorEnum(int value)
        {
            return new OperatorEnum( (byte)value );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// enumValue casting operator.
        /// </summary>
        /// <param name="value">    The value as a byte to convert </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator OperatorEnum(byte value)
        {
            return new OperatorEnum( value );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// cast enum to a byte casting operator. 
        /// </summary>
        /// <param name="enumValue">   The operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator byte(OperatorEnum enumValue)
        {
            return enumValue.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// cast enum to an int casting operator. 
        /// </summary>
        /// <param name="enumValue">   The operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static implicit operator int(OperatorEnum enumValue)
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
        /// Tests if this Operator is considered equal to another.
        /// </summary>
        /// <param name="other"> the other enum value. </param>
        /// <returns>
        /// true if the enum values are equal are considered equal, false if they are not.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Equals(OperatorEnum other)
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


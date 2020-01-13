////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Description: <File description>
// 
//  Author:      Robert C Steiner
// 
//=====================================================[ History ]=====================================================
//  Date        Who      What
//  ----------- ------   ----------------------------------------------------------------------------------------------
//  1/13/2020   RCS      Initial Code.
//====================================================[ Copyright ]====================================================
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace ZCore
{
    public static class StringUtil
    {
        /// <summary>
        ///     defines whitespace characters NOT LF. + BOM characters
        /// </summary>
        public const string WHITESPACE = " \t\r\xfffe\xfeff";

        /// <summary>
        ///     defines whitespace characters (including end of line LF). + BOM characters
        /// </summary>
        public const string WHITESPACE_LF = WHITESPACE + "\n";

        /// <summary>
        ///     Key start characters
        /// </summary>
        public const string NAME_START = "$_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";


        /// <summary>
        ///     Key continuation characters
        /// </summary>
        public const string NAME_CONTINUE = "$_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// From camel case to expanded. Default expands with spaces.  From NameTestX to "Key Test X"
        /// </summary>
        /// <param name="str">          The string to capitalize. </param>
        /// <param name="spaceChar">    The space char. Defaults to ' ' </param>
        /// <returns>   
        /// new string 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string FromCamel(this String str, char spaceChar = ' ')
        {
            StringBuilder builder = new StringBuilder();
            bool lastLower = false;
            char lastChar = '\0';
            if (str != null)
            {
                int len = str.Length;
                for (int index = 0; index < len; index++)
                {
                    char c = str[index];
                    if (Char.IsUpper( c ))
                    {
                        if (lastLower)
                        {
                            if (lastChar != spaceChar)
                            {
                                builder.Append( spaceChar );
                            }
                            lastLower = false;
                        }
                    }
                    else
                    {
                        lastChar = c;
                        lastLower = true;
                    }
                    builder.Append( c );
                }
            }
            return builder.ToString();
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Query if 'name' is one of the names in the names list (array) Case insenstive.
        /// </summary>
        /// <param name="name">     The name. </param>
        /// <param name="names">    A variable-length parameters list containing names. </param>
        /// <returns>   
        /// true if one of, false if not. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsOneOf(this String name, params string[] names)
        {
            if ((name == null) || (names == null))
            {
                return false;
            }

            int len = names.Length;
            for (int index = 0; index < len; index++)
            {
                string s = names[index];
                if (name.Equals( s, StringComparison.CurrentCultureIgnoreCase ))
                {
                    return true;
                }
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Flush whitespace and LF characters.  
        /// </summary>
        /// <returns>true if not at end</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FlushWhitespace(this string text, ref int position)
        {
            if (String.IsNullOrEmpty( text ))
            {
                return false;
            }

            while ((position < text.Length) && WHITESPACE_LF.IndexOf( text[position] ) >= 0)
            {
                position++;
            }
            return position < text.Length;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// First non whitespace index. 
        /// </summary>
        /// <param name="text">     The text. </param>
        /// <param name="position">   The offset. </param>
        /// <returns>   
        /// the next index or the length of the string if no characters found.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static int FirstNonWhitespaceIndex(this String text, int position = 0)
        {
            if (String.IsNullOrEmpty( text ))
            {
                return 0;
            }
            return FlushWhitespace( text, ref position ) ? position : text.Length;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Removes the suffix on text.
        /// </summary>
        /// <param name="text"> The text. </param>
        /// <returns>   
        /// the string without the suffix.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string RemoveSuffix(this String text, string suffix)
        {
            if (string.IsNullOrEmpty( text ) || string.IsNullOrEmpty( suffix ))
            {
                return text;
            }
            if (text.EndsWith( suffix ))
            {
                return text.Substring( 0, text.Length - suffix.Length );
            }
            return text;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Equal to ignore case.
        /// </summary>
        /// <param name="key">      The key. </param>
        /// <param name="value">    The value. </param>
        /// <returns>
        /// true if same
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool EqualsIgnoreCase(this String key, string value)
        {
            if (key == null)
            {
                return value == null;
            }
            return value != null && String.Equals( key, value, StringComparison.OrdinalIgnoreCase );
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare to ignore case.
        /// </summary>
        /// <param name="key">      The key. </param>
        /// <param name="value">    The value. </param>
        /// <returns>
        /// -1,0,1
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static int CompareToIgnoreCase(this string key, string value)
        {
            if (key == null)
            {
                return value == null ? 0 : -1;
            }
            return value != null ? String.Compare( key, value, StringComparison.OrdinalIgnoreCase ) : 1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parses name  by testing first the name start character and continues to parse while name continue
        /// characters.
        /// </summary>
        /// <param name="text">     The text. </param>
        /// <param name="position">   [in,out] The offset. </param>
        /// <returns>
        /// text string of name or null if not a name.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string ParseName(this string text, ref int position)
        {
            position = text.FirstNonWhitespaceIndex( position );
            if (String.IsNullOrEmpty( text ) || position >= text.Length)
            {
                return null;
            }
            int start = position;
            if (NAME_START.IndexOf( text[position] ) >= 0)
            {
                while (++position < text.Length && NAME_CONTINUE.IndexOf( text[position] ) >= 0)
                {

                }
                return text.Substring( start, position - start );
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Capitalizes the specified string (only the first character is capitalized).
        /// </summary>
        /// <param name="str">The string to capitalize.</param>
        /// <returns>The capitalized string</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string Capitalize(this String str)
        {
            return String.IsNullOrEmpty( str ) ? "" : Char.ToUpper( str[0] ) + str.Substring( 1 );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Uncapitalize the specified string (only the first character is made lower case).
        /// </summary>
        /// <param name="str">The string to capitalize.</param>
        /// <returns>The capitalized string</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string Uncapitalize(this String str)
        {
            return String.IsNullOrEmpty( str ) ? "" : Char.ToLower( str[0] ) + str.Substring( 1 );
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        /// Get the first element of a stringIdentifier. Like X.y.z  LastElement('.') returns X. 
        /// </summary>
        /// <param name="stringIdentifier"> The string identifier. </param>
        /// <param name="separator">        The separator. </param>
        /// <returns>   
        /// the first element if there is one or the string if none. 
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string FirstElement(this String stringIdentifier, char separator)
        {
            if (stringIdentifier != null)
            {
                int i = stringIdentifier.IndexOf( separator );
                return (i >= 0) ? stringIdentifier.Substring( 0, i ) : stringIdentifier;
            }

            return "";
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Reads a value.
        /// </summary>
        /// <param name="dict">         The dictionary. </param>
        /// <param name="key">          The key. </param>
        /// <param name="defaultValue"> (optional) default value. </param>
        /// <returns>
        /// The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string ReadValue(Dictionary<string, string> dict, string key, string defaultValue = "")
        {
            string value;
            return dict.TryGetValue( key, out value ) ? value : defaultValue;
        }

    }

}

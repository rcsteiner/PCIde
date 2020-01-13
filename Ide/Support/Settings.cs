////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the settings class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   8/4/2015   rcs     Initial Implementation
//  =====================================================================================================
//
//  BSD 3-Clause License
//  Copyright (c) 2020, Robert C. Steiner
//  All rights reserved.
//  Redistribution and use in source and binary forms, with or without   modification,
//  are permitted provided that the following conditions are met:
//
//  1. Redistributions of source code must retain the above copyright notice, this
//  list of conditions and the following disclaimer.
//
//  2. Redistributions in binary form must reproduce the above copyright notice,
//  this list of conditions and the following disclaimer in the documentation
//  and/or other materials provided with the distribution.
//
//  3. Neither the name of the copyright holder nor the names of its
//  contributors may be used to endorse or promote products derived from
//  this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
//  FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//  DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
//  CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//  OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using ZCore;

namespace Ide.Support
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Settings. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Settings
    {
        /// <summary>
        /// The pairs.
        /// </summary>
        private Dictionary<string, string> Pairs;

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count { get { return Pairs.Count; } }

        /// <summary>
        /// Gets a value indicating whether this instance is loaded.
        /// </summary>
        public bool IsLoaded { get { return Pairs != null;} }

        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        public object this[string key]
        {
            get { return FindString(key);}
            set { AddString(key, value.ToString());}
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="count">    (optional) number of. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Settings(int count = 10)
        {
            Pairs = new Dictionary<string, string>( 10 );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pairs">    The pairs. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Settings(Dictionary<string, string> pairs)
        {
            Pairs = pairs;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fullPath"> The string to load. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Settings(string fullPath)
        {
            if (!string.IsNullOrEmpty(fullPath))
            {
                Pairs = FileUtil.FileReadPNameValuePairs( fullPath );
            }
            else
            {
                Pairs = new Dictionary<string, string>();
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a string to 'value'.
        /// </summary>
        /// <param name="key">      The key. </param>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddString(string key, string value)
        {
            Remove(key);
            Pairs.Add(key, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a string list set by adding string with comma separated members
        /// </summary>
        /// <param name="key">      The key. </param>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddStringList(string key, string[] value)
        {
            Remove(key);
            Pairs.Add(key,string.Join(",",value));
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a integer to 'value'.
        /// </summary>
        /// <param name="key">      The key. </param>
        /// <param name="value">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddInteger(string key, int value)
        {
            Remove(key);
            Pairs.Add(key, value.ToString());
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Finds the string value of key
        /// </summary>
        /// <param name="key">  The key. </param>
        /// <returns>
        /// The value or null
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string FindString(string key)
        {
            string s;
            return Pairs.TryGetValue(key, out s) ? s : null;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Finds the string[] , comma separated value of key
        /// </summary>
        /// <param name="key">  The key. </param>
        /// <returns>
        /// The value empty list
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string[] FindStringList(string key)
        {
            string s;
            if (Pairs.TryGetValue(key, out s))
            {
                return s.Split(',');
            }
            return new string[0];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Finds the integer value of key or 0.
        /// </summary>
        /// <param name="key">          The key. </param>
        /// <param name="defaultValue"> Default value. </param>
        /// <returns>
        /// The value or null.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int FindInteger(string key, int defaultValue=0)
        {
            string s = FindString(key);
            int    i;
            return (s != null) ? (int.TryParse(s, out i) ? i : defaultValue) :defaultValue;
        }
    
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Finds the double value of key or int.MinValue
        /// </summary>
        /// <param name="key">  The key. </param>
        /// <returns>
        /// The value or null
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public double FindDouble(string key)
        {
            string s = FindString( key );
            double i;
            return (s != null) ? (double.TryParse( s, out i ) ? i : 0) : 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Finds the bool value of key or false
        /// </summary>
        /// <param name="key">  The key. </param>
        /// <returns>
        /// The value or null
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool FindBool(string key)
        {
            string s = FindString( key );
            bool i;
            return (s != null) ? (bool.TryParse( s, out i ) ? i :false) : false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes the given key if it is in the table.
        /// </summary>
        /// <param name="key">  The key. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Remove(string key)
        {
            if (Pairs.ContainsKey(key))
            {
                Pairs.Remove(key);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads the settings from a file.
        /// </summary>
        /// <param name="fullPath"> The string to load. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Load(string fullPath)
        {
            Pairs = FileUtil.FileReadPNameValuePairs(fullPath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves.
        /// </summary>
        /// <param name="fullPath"> The string to load. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Save(string fullPath)
        {
            FileUtil.FileWriteValuePairs(fullPath,Pairs);
        }

    }
}
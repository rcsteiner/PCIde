////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Declares the IMessageListener interface
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


namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interface for a message receiver.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface IMessageListener
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// New message is received.  Check it to see if we are interested in it.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="key">      The key. </param>
        /// <param name="payload">  The payload. </param>
        /// <returns>
        /// the message result, what to do.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        MessageResult NewMessage(IMessageListener sender, string key, object payload);


        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sends a message to all listeners.  The listeners provide the filtering.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="payload">  The payload. </param>
        /// <param name="sender">   The sender. </param>
        /// <returns>
        /// true if it is handled by at least one listener, false if no listeners process it.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool Send(string message, object payload);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Match identifier calls each registered listener, if the listener matches the identifier, then the listener returns
        /// true.
        /// </summary>
        /// <param name="identifier">   The identifier. </param>
        /// <returns>
        /// true if it matches the identifier.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool MatchIdentifier(string identifier);
    }
}
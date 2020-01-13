////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the messenger class
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

using System.Collections.Generic;

namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Ultra light weight messeger.  
    /// Messenger service singlton used to publish and subscribe to specific messages.
    /// This is a singleton.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static class Messenger
    {
        private static List<IMessageListener>  Listeners = new List<IMessageListener>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first listener.
        /// </summary>
        /// <param name="sender">       The sender. </param>
        /// <param name="identifier">   The identifier. </param>
        /// <returns>
        /// The found listener.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static IMessageListener FindListener(IMessageListener sender, string identifier)
        {
            foreach (var listener in Listeners)
            {
                if (listener == sender) continue;

                if (listener.MatchIdentifier(identifier))
                {
                    return listener;
                }
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// De-registers a listener if it has been registered.
        /// </summary>
        /// <param name="listener">The listener.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DeRegister(IMessageListener listener)
        {
            if (Listeners.Contains(listener))
            {
                Listeners.Remove(listener);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Registers a listener to receive messages.
        /// </summary>
        /// <param name="listener"> The listener. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Register(IMessageListener listener)
        {
            if (!Listeners.Contains( listener ))
            {
                Listeners.Add( listener );
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sends a message to all listeners.  The listeners provide the filtering.
        /// Won't send the message to itself.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="message">  The message. </param>
        /// <param name="payload">  The payload. </param>
        /// <returns>
        /// true if it is handled by at least one listener, false if no listeners process it.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool Send(IMessageListener sender, string message, object payload)
        {
            bool handled = false;
            foreach (var listener in Listeners)
            {
                if (listener == sender) continue;

                switch (listener.NewMessage(sender, message, payload))
                {
                    case MessageResult.IGNORED:
                        continue;
                  
                    case MessageResult.HANDLED_STOP:
                        handled=true;
                        return handled;

                    case MessageResult.HANDLED_CONTINUE:
                        handled=true;
                        continue;
                }
            }
            return handled;
        }
    }
}
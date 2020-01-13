////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the observable object class
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

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace ZCore
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     This is the abstract base class for any object that provides property change notifications. From the MVVMProject.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public abstract class ObservableObject : INotifyPropertyChanged,  IMessageListener
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Raised when a property on this object has a new value.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public event PropertyChangedEventHandler PropertyChanged;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName"> The property that has a new value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RaisePropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            if (PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, e);
            }
        }


        #region Messaging

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// New message is received, filter and decide if we are interesed in it.  
        /// Default is to ignore it.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="key">      The key. </param>
        /// <param name="payload">  The payload. </param>
        /// <returns>
        /// the message result, what to do.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual MessageResult NewMessage(IMessageListener sender, string key, object payload)
        {
            return MessageResult.IGNORED;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sends a message to all listeners.  The listeners provide the filtering.
        /// This provides a convenient way for an model view to send a message.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="payload">  The payload. </param>
        /// <returns>
        /// true if it is handled by at least one listener, false if no listeners process it.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Send(string message, object payload)
        {
            return Messenger.Send(this, message, payload);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Match identifier calls each registered listener, if the listener matches the identifier, then the listener
        /// returns true.
        /// This default behavior is to return false.
        /// </summary>
        /// <param name="identifier">   The identifier. </param>
        /// <returns>
        /// true if it matches the identifier.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool MatchIdentifier(string identifier)
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches for the first listener to match identifier.
        /// </summary>
        /// <param name="identiier">    The identiier. </param>
        /// <returns>
        /// The found listener.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IMessageListener FindListener(string identiier)
        {
           return Messenger.FindListener( this, identiier );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sends a message to only one target.  The listener provide the filtering.
        ///  This provides a convenient way for an
        /// model view to send a message to one target like a view model.
        /// </summary>
        /// <param name="target">   Target for the. </param>
        /// <param name="message">  The message. </param>
        /// <param name="payload">  The payload. </param>
        /// <returns>
        /// true if it is handled by at least one listener, false if no listeners process it.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SendTo(IMessageListener target, string message, object payload)
        {
            return target != null && target.NewMessage(this, message, payload) != MessageResult.IGNORED;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Messeger register this listener
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MessegerRegister()
        {
            Messenger.Register(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Messeger deregister this listener.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MessegerDeregister()
        {
            Messenger.DeRegister(this);
        }

        #endregion


        #region Debugging Aides

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Warns the developer if this object does not have a public property with the specified name. This method does not
        ///     exist in a Release build.
        /// </summary>
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or illegal values. </exception>
        /// <param name="propertyName"> The property that has a new value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // If you raise PropertyChanged and do not specify a property name,
            // all properties on the object are considered to be changed by the binding system.
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                var msg = "Invalid property name: " + propertyName;

                if (ThrowOnInvalidPropertyName)
                {
                    throw new ArgumentException(msg);
                }
                Debug.Fail(msg);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Returns whether an exception is thrown, or if a Debug.Fail() is used when an invalid property name is passed to
        ///     the VerifyPropertyName method. The default value is false, but subclasses used by unit tests might override this
        ///     property's getter to return true.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides
   
    }
}
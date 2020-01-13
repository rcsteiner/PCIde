////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the expression new class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   10/6/2015   rcs     Initial Implementation
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

using System.Text;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Expression new. to create new objects.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PExpressionNew : PExpression
    {
        /// <summary>
        /// The reference expresson for the constructor.
        /// </summary>
        public PExpressionReference refExpr;

        #region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        /// <param name="expr">     The expression. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpressionNew(PContext context, PExpression expr = null) : base(context)
        {
            refExpr = expr as PExpressionReference;
            if (refExpr == null || !refExpr.IsCall)
            {
                SyntaxError(expr.Offset, expr.Length, "New expression requires a constructor.  found {0}", expr);
                return;
            }
            var thisRef    = new PExpressionReference(Context, "this");
            var args       = refExpr.Args;
            refExpr.IsConstructor = true;
         
            //if (args.Count == 0)
            //{
            //    args.Add(thisRef);
            //}
            //else
            {
                args.Expressions.Insert(0,thisRef);
            }

            if (refExpr.Module == null)
            {
                refExpr.Module = Context.FindModule(refExpr.Name, Source.LineNumber);


                //if (refExpr.Module == null)
                //{
                //    Context.UnresolvedReferences.Add(refExpr);
                //}
            }

        }

        #endregion

        #region Execute

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// It alway returns an object
        /// If the constructor is default then take a built in action.
        /// </summary>
        /// <returns>
        /// Next if continue else condition.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            var name = refExpr.Name;
            var cls = Context.Classes.Find(n => n.Name == name);
            if (cls == null)
            {
                return RuntimeFatal(refExpr.Offset, refExpr.Length, LineNumber, "Can't construct object of type {0}", name);
            }
            var obj = new PObject(cls);
  
            var old = Context.RunStack.ObjectRef;
            Context.RunStack.ObjectRef = obj;

            ExecuteConstructor(obj);

            Context.RunStack.ObjectRef = old;
            // it's removed from stack since it doesn't return like normal
            // put on stack for expression result.
            Context.RunStack.Push(obj);

            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the constructor operation.
        /// </summary>
        /// <param name="obj">  The object. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ExecuteConstructor(PObject obj)
        {
            base.Execute();
            var m = refExpr.Module;
            if (m == null && obj.ClassRef != null)
            {
                // handle forward reference issues
                m = obj.ClassRef.Modules.Find( x => x.Name == refExpr.Name );
                refExpr.Module = m;
            }
         
            if (refExpr.Args.Count == 1)
            {
                // this is a default constructor 
                // create a empty object of the right type
                // if the module exits and the right args use it. else its default
                if (m != null && m.Parameters.Count == 1)
                {
                    //put on stack for call
                    Context.RunStack.Push(obj);

                    // execute the default since it exists
                    refExpr.Execute();
                }
            }
            else
            {
                // call constructor normally, 1st parameter is the object
                // put on stack for call
               // Context.RunStack.Push(obj);

                refExpr.Execute();
            }
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.Append(" New ");
            if (refExpr != null)
            {
                refExpr.ToString(builder);
            }
        }
    }
}
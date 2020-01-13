////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the expression binary class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/25/2015   rcs     Initial Implementation
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
using System.Text;
using ZCore;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Expression binary. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PExpressionBinary : PExpression
    {
        /// <summary>
        /// The left.
        /// </summary>
        public PExpression Left;

        /// <summary>
        /// The right.
        /// </summary>
        public PExpression Right;

        /// <summary>
        /// The operator
        /// </summary>
        public OperatorEnum Operator;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpressionBinary(PContext context)
            : base( context )
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        /// <param name="left">     The left. </param>
        /// <param name="right">    The right. </param>
        /// <param name="op">       The operation. </param>
        /// <param name="pos">      The position. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpressionBinary(PContext context, PExpression left, PExpression right, byte op, int pos) : base(context)
        {
            Left     = left;
            Right    = right;
            Operator = op;
            Offset   = pos;
            if (right != null)
            {
                Length   = right.Offset+right.Length-pos;
            }
            LineNumber = Source.LineNumber;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            base.Execute(); // sets line number

            if (Left.Execute() == ReturnState.STOP)
            {
                return ReturnState.STOP;
            }
            var l = Context.RunStack.Pop();

            if (Operator.Value == OperatorEnum.LAND)
            {
                // short circuit AND
                if (!l.BValue)
                {
                    Context.RunStack.Push(l);
                    return ReturnState.NEXT;
                }
            }
            else if (Operator.Value == OperatorEnum.LOR)
            {
                // short circuit OR
                if (l.BValue)
                {
                    Context.RunStack.Push( l );
                    return ReturnState.NEXT;
                }
            }


            if (Right.Execute() == ReturnState.STOP)
            {
                return ReturnState.STOP;
            }
            if (!ExecuteBinary(l)) return ReturnState.STOP;

            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the binary operation.
        /// </summary>
        /// <param name="left">    The left. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ExecuteBinary(Accumulator left)
        {
            var right = Context.RunStack.Pop();
            byte op   = Operator.Value;

            ConvertToSame(ref left, ref right);

            Accumulator a = left;
            if (a.Store== StoreEnum.INTEGER || a.Store == StoreEnum.CHAR)
            {
                switch (op)
                {
                    case OperatorEnum.CMPNE: //   "!=" 
                    case OperatorEnum.CMPLTE: //   "<=" 
                    case OperatorEnum.CMPLT: //   "<"  
                    case OperatorEnum.CMPE: //   "==" 
                    case OperatorEnum.CMPGT: //   ">"  
                    case OperatorEnum.CMPGTE: //   ">=" 
                        Compare(left, right, Operator);
                        return true;


                    case OperatorEnum.MOD: //   "MOD"  
                        a.LValue %= right.LValue;
                        break;
                    case OperatorEnum.MUL: //   "*"  
                        a.LValue *= right.LValue;
                        break;
                    case OperatorEnum.ADD: //   "+"  
                        a.LValue += right.LValue;
                        break;
                    case OperatorEnum.SUB: //   "-"  
                        a.LValue -= right.LValue;
                        break;
                    case OperatorEnum.DIV: //   "/"  
                        a.LValue /= right.LValue;
                        break;
                    case OperatorEnum.POW: //   "^"  
                        a.LValue = (long)Math.Pow( a.LValue, right.LValue );
                        break;

                    case OperatorEnum.AND: //   "&"  
                        a.LValue &= right.LValue;
                        break;
                    case OperatorEnum.OR:  //   "|"  
                        a.LValue |= right.LValue;
                        break;

                    case OperatorEnum.SHL: //   "<<" 
                        a.LValue <<= (int) right.LValue;
                        break;
                    case OperatorEnum.SHR: //   ">>" 
                        a.LValue <<= (int) right.LValue;
                        break;

                default:
                        RuntimeFatal(Offset, Length, LineNumber, "operator {0} Cannot be used on Integer or Char type.", Operator.ToString());
                        return false;
                }
            }
            else if (a.Store == StoreEnum.REAL)
            {
                switch (op)
                {
                    case OperatorEnum.CMPNE:  //   "!=" 
                    case OperatorEnum.CMPLTE: //   "<=" 
                    case OperatorEnum.CMPLT:  //   "<"  
                    case OperatorEnum.CMPE:   //   "==" 
                    case OperatorEnum.CMPGT:  //   ">"  
                    case OperatorEnum.CMPGTE: //   ">=" 
                        Compare( left, right, Operator );
                        return true;

                    case OperatorEnum.MUL: //   "*"  
                        a.DValue *= right.DValue;
                        break;
                    case OperatorEnum.ADD: //   "+"  
                        a.DValue += right.DValue;
                        break;
                    case OperatorEnum.SUB: //   "-"  
                        a.DValue -= right.DValue;
                        break;
                    case OperatorEnum.DIV: //   "/"  
                        a.DValue /= right.DValue;
                        break;
                    case OperatorEnum.POW: //   "^"  
                        a.DValue = Math.Pow( a.DValue, right.DValue );
                        break;
                    default:
                        RuntimeFatal( Offset, Length, LineNumber, "operator {0} cannot be used of Real type.", Operator.ToString());
                        return false;
                }

            }
            else if (a.Store == StoreEnum.BOOL)
            {
                switch (op)
                {
                    case OperatorEnum.CMPNE: //   "!=" 
                    case OperatorEnum.CMPE: //   "==" 
                        Compare(left, right, Operator);
                        return true;

                    case OperatorEnum.LOR: //   "||" 
                        a.BValue = left.BValue || right.BValue;
                        a.Store = StoreEnum.BOOL;
                        break;
                    case OperatorEnum.LAND: //   "&&" 
                        a.BValue = left.BValue && right.BValue;
                        a.Store = StoreEnum.BOOL;
                        break;
                    default:
                        RuntimeFatal( Offset, Length, LineNumber, "Operator {0} cannot be used on Boolean type", Operator.ToString());
                        return false;
                }
            }
            else if (a.Store == StoreEnum.STRING)
            {
                switch (op)
                {
                    case OperatorEnum.CMPNE: //   "!=" 
                    case OperatorEnum.CMPLTE: //   "<=" 
                    case OperatorEnum.CMPLT: //   "<"  
                    case OperatorEnum.CMPE: //   "==" 
                    case OperatorEnum.CMPGT: //   ">"  
                    case OperatorEnum.CMPGTE: //   ">=" 
                        Compare(left, right, Operator);
                        return true;

                    case OperatorEnum.NCOP: //   "??" 
                    case OperatorEnum.COND: //    "?"             

                default:
                        RuntimeFatal(Offset, Length, LineNumber, "operator {0} Cannot be used on String type.", Operator.ToString());
                        return false;
                }
            }
            else if (a.Store == StoreEnum.OBJECT)
            {
                switch (op)
                {
                    case OperatorEnum.CMPNE: //   "!=" 
                    case OperatorEnum.CMPLTE: //   "<=" 
                    case OperatorEnum.CMPLT: //   "<"  
                    case OperatorEnum.CMPE: //   "==" 
                    case OperatorEnum.CMPGT: //   ">"  
                    case OperatorEnum.CMPGTE: //   ">=" 
                        Compare( left, right, Operator );
                        return true;

                    case OperatorEnum.NCOP: //   "??" 
                    case OperatorEnum.COND: //    "?"             

                    default:
                        RuntimeFatal( Offset, Length, LineNumber, "operator {0} Cannot be used on Integer or Object type.", Operator.ToString());
                        return false;
                }
            }

            Context.RunStack.Push(a);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert to same.
        /// </summary>
        /// <param name="left">     The left. </param>
        /// <param name="right">    The right. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private  bool ConvertToSame(ref Accumulator  left,ref Accumulator right)
        {
            if (left.Store == right.Store)
            {
                return true;
            }
            switch (left.Store)
            {
                case StoreEnum.NULL:
                    break;
                case StoreEnum.REAL:
                    return right.ConvertToDouble();
                case StoreEnum.BOOL:
                    return right.ConvertToBool();
                case StoreEnum.STRING:
                    if (right.Store != StoreEnum.STRING)
                    {
                      RuntimeError(Offset,Length, 0,"Cannot covert a {0} to a String.", right.Store.ToString().Capitalize());
                        return false;
                    }
                    break;
                case StoreEnum.INTEGER:
                    if (right.Store == StoreEnum.STRING)
                    {
                        RuntimeError( Offset, Length, 0, "Cannot covert a String to a Integer.");
                        return false;
                    }
                    if (right.Store == StoreEnum.REAL)
                    {
                        return left.ConvertToDouble();
                    }
                    return right.ConvertToInteger();
                case StoreEnum.CHAR:
                    break;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compares objects.
        /// uses Negative if 'left' is less than 'right', 0 if they are equal, or positive if it is greater.
        /// </summary>
        /// <param name="left">     The left. </param>
        /// <param name="right">    The right. </param>
        /// <param name="op">       The operation. </param>
        /// <returns>
        /// true if valid else runtime error
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool Compare(Accumulator left, Accumulator right, OperatorEnum op)
        {
            int comp=0;
            if (ConvertToSame(ref left,ref right))
            {
                switch (left.Store)
                {
                    case StoreEnum.NULL:
                        break;
                    case StoreEnum.REAL:
                        comp = (left.DValue > right.DValue) ? 1 : ((left.DValue == right.DValue) ? 0 : -1);
                        break;
                    case StoreEnum.BOOL:
                        if (op == OperatorEnum.CMPE)
                        {
                            comp = left.BValue == right.BValue ? 0 : 1;
                        }
                        else if (op == OperatorEnum.CMPNE)
                        {
                            comp = left.BValue != right.BValue ? 1 : 0;
                        }
                        else
                        {
                            RuntimeError(Offset,Length, 0,"Can't apply {0} to boolean values.", op.ToString());
                            return false;
                        }
                        break;
                    case StoreEnum.STRING:
                        comp = String.Compare(left.SValue, right.SValue, StringComparison.Ordinal);
                        break;
                    case StoreEnum.CHAR:
                    case StoreEnum.INTEGER:
                        comp = (left.LValue > right.LValue) ? 1 : ((left.LValue == right.LValue) ? 0 : -1);
                        break;
                }

                Accumulator b = new Accumulator(false);

                switch (op.Value)
                {
                    case OperatorEnum.CMPNE:  //   "!=" 
                        b.BValue = comp != 0;
                        break;
                    case OperatorEnum.CMPLTE: //   "<=" 
                        b.BValue = comp <= 0;
                        break;
                    case OperatorEnum.CMPLT:  //   "<"  
                        b.BValue = comp < 0;
                        break;
                    case OperatorEnum.CMPE:   //   "==" 
                        b.BValue = comp == 0;
                        break;
                    case OperatorEnum.CMPGT:  //   ">"  
                        b.BValue = comp > 0;
                        break;
                    case OperatorEnum.CMPGTE: //   ">=" 
                        b.BValue = comp >= 0;
                        break;
                  
                }
                Context.RunStack.Push(b);
                return true;
               
            }
            // conveted to same?
            RuntimeError(Offset, Length, 0, "Failed to convert both sides to the same type in expression '{0}", ToString());
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the bool the expression and returns the bool result only. Stack is clear of result.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ExecuteBool()
        {
            Execute();
            var b = Context.RunStack.Pop();
            if (b.ConvertToBool())
            {
                return  b.BValue;
            }
            RuntimeError(Offset, Length, 0, "Cannot Convert expression '{0}' to a boolean expression", ToString());
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.Append('(');
            Left.ToString(builder);
            builder.AppendFormat(" {0} ", Operator.ToString());
            Right.ToString(builder);
            builder.Append(')');
        }

    }
}
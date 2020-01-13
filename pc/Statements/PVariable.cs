////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the variable class
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

using System.Collections.Generic;
using System.Text;
using ZCore;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Variable. 
    /// Variables are arranged on an array at runtime.  
    /// 
    /// static variables occupy the first slots in the array.  Once execution starts, 
    /// each module uses relative slots in the array.
    /// 
    /// Addressing is   base+offset.
    /// static base = 0;
    /// module base = basePtr
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PVariable : PElement
    {
        private const string TYPE_ERROR_FORMAT = "{1}s require a type (Real | Integer | String | Boolean  | Object | Char | User Object Type) found {0}";

        public const int ARRAY_MARKER = 0x1000000;

        /// <summary>
        /// The expression or intial value of declaration or null if none.
        /// </summary>
        public PExpression InitialExpression;

        /// <summary>
        /// The name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The type string.
        /// </summary>
        public string Type;

        /// <summary>
        /// true if is constant.
        /// </summary>
        public bool IsConstant;

        /// <summary>
        /// Gets or sets a value indicating whether this variable is reference.
        /// </summary>
        public bool IsRef { get; set; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public PBlock Scope { get; set; }

      
        /// <summary>
        /// The scope of this variable (named scope)
        /// </summary>
        public PModule Module;

        /// <summary>
        /// This is an object so if the initial expression is set, then New must be used.
        /// </summary>
        public bool IsObject;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is member. 
        /// </summary>
        public bool IsMember { get; set; }

        /// <summary>
        /// The class reference if any
        /// </summary>
        public PClass ClassRef;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is argument.
        /// </summary>
        public bool IsArgument { get; set; }

        /// <summary>
        /// Gets the type of the declaration
        /// </summary>
        private string DeclType { get {return IsArgument ? "Argument" : (IsObject )?"Field": "Variable"; } }

        /// <summary>
        /// Gets or sets the references.
        /// </summary>
        public List<int> References { get; set; }

        /// <summary>
        /// Gets or sets the size of the dimension.
        /// </summary>
        public List<int> DimensionSize { get; set; } 

        /// <summary>
        /// The dimensions
        /// </summary>
        public List<PExpression> Dimensions;

        /// <summary>
        /// The dimension rank
        /// </summary>
        public int DimensionRank { get;set; }

        /// <summary>
        /// Gets a value indicating whether this instance is array.
        /// </summary>
        public bool IsArray { get { return Dimensions != null; } }

        /// <summary>
        /// Gets or sets the store.
        /// </summary>
        public StoreEnum Store { get; set; }


        #region Value Property

        /// <summary>
        /// Gets or sets a value indicating whether this instance is static.
        /// </summary>
        public bool IsStatic { get; set; }


        /// <summary>
        /// Gets or sets the base pointer + offset when the variable is declared
        /// </summary>
        public int StackIndex { get; set; }

        /// <summary>
        /// Gets the slots.
        /// </summary>
        public int Slots {get { return (ClassRef != null) ? ClassRef.Scope.Variables.Count : 1; }}

        /// <summary>
        /// The value of this variable valid  only if top of stack!
        /// </summary>
        public Accumulator Value {get {return GetValue();} set {SetValue(value);}}

        private Accumulator _fieldValue;

        /// <summary>
        /// Gets or sets the offset declared.  This is relative to basePtr or relative to 0 if static.
        /// </summary>
        public int ValueOffset { get; set; }

        public int RunIndex { get { return StackIndex; } }

//       public int RunIndex { get { return IsStatic ? ValueOffset : ValueOffset + Context.RunStack._basePtr; } }
        #endregion

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor. for copy
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PVariable() : base(null)
        {
            Scope = Context.ScopeStack.Count > 0 ? Context.ScopeStack.Peek() : null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">      The context. </param>
        /// <param name="isArgument">   true if is argument else it's a variable. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PVariable(PContext context, bool isArgument, bool isMember=false) : base(context)
        {
            IsArgument = isArgument;
            References = new List<int>();
            LineNumber = context.Source.LineNumber;
            Scope      = Context.ScopeStack.Count > 0 ? Context.ScopeStack.Peek() : null;
            Name       = "<UnknownW>";
            var method = Context.CurrentMethod;
            IsMember   = isMember;
            Module     = method;
            if (method != null && !(method is PProgram))
            {
                if (isArgument)
                {
                    ValueOffset = -(method.Parameters.Count + 1);
                    IsStatic = false;
                }
                else
                {
                    ValueOffset = ((PBlock) method).Scope.Variables.Count-method.Parameters.Count + 1;
                }
            }
            else if (!isMember)
            {
                ValueOffset = Context.StaticPtr++;
                IsStatic = true;
            }
            // member ignore value offset first.
        }

        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the default.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetDefault()
        {
            if (IsArray)
            {
                ExecuteArray();
                return;
            }
            switch (Type.ToLower())
            {
                case "real":
                    Value = new Accumulator(0.0);
                    break;
                case "integer":
                    Value = new Accumulator(0);
                    break;
                case "string":
                    Value = new Accumulator("");
                    break;
                case "boolean":
                    Value = new Accumulator(false);
                    break;
                case "char":
                    Value = new Accumulator('\0');
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets a value.
        /// </summary>
        /// <param name="value">    The value of this variable valid  only if top of stack! </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetValue(Accumulator value)
        {
            if (IsMember)
            {
                _fieldValue = value;
                return;
            }

            //Context.RunStack[StackIndex] = value;
            //SetValue(Context.RunStack._basePtr,value);
            if (IsRef)
            {
                var v = Context.RunStack[RunIndex];
                int n = (int)v.LValue;
                Context.RunStack[n] = value;
                // Context.RunStack.SetValue( value, n, true );
            }
            else
            {
                Context.RunStack[StackIndex] = value;

                //  Context.RunStack.SetValue( value, ValueOffset, IsStatic );
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>
        /// The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Accumulator GetValue()
        {
            if (IsMember)
            {
                return _fieldValue;
            }
            //return Context.RunStack[StackIndex];
            // return GetValue(Context.RunStack._basePtr);
            if (IsRef && !IsArray)
            {
                int n = Context.RunStack[RunIndex].IValue & (ARRAY_MARKER - 1);

                return Context.RunStack[n];
            }
            return Context.RunStack[StackIndex];
        }



        #region Parse

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Declares a variable of some type Syntax: 
        ///  [Declare|Constant] (Real | Integer | String |Char | Object) Identifier [ = Expression].
        ///  Must be on the Type
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            Source.FlushWhitespace();
            LineNumber = Source.LineNumber;
            int pos    = Source.Position;
            Offset     = pos;

           
        
            if (Source.ParseToken() == TokenTypeEnum.NAME)
            {
                ParseCheckType();
            }
            // common error of Declare on parameters
            if (Source.Token.EqualsIgnoreCase("Declare"))
            {
                SyntaxError(-1,-1,"'Declare' is not needed here.  Remove it.");
                if (Source.ParseToken() == TokenTypeEnum.NAME)
                {
                    ParseCheckType();
                }
            }

            if (IsArgument && (Source.Token.EqualsIgnoreCase("Ref")))
            {
                ParseFixRef();
            }

            if (Source.CheckSystemType())
            {
                return ParseSystemType();
            }

            if (Source.Type != TokenTypeEnum.NAME)
            {
                SyntaxError(Source.Position, 2, TYPE_ERROR_FORMAT, Source.Token, DeclType);
                return true;
            }

            // current token is the name
            Type = Source.Token;
            var p = Source.Position;
            Source.ParseToken();
            if (Source.Type != TokenTypeEnum.NAME)
            {
                // missing declaration
                SyntaxError( pos, Type.Length, TYPE_ERROR_FORMAT + "  You must have a type for '{2}' For isntance:\nInteger {2}.", Source.Token, DeclType,Type );
                Source.Position = p;
                return true;
            }
            Name = Source.Token;

            return ParseNamedType(pos);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse fix a possible bad type.
        ///  look to see if its a case sensitive issue
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ParseCheckType()
        {
            if (Source.PossibleType != ReservedEnum.INVALID)
            {
                // assume a case issue and fix it
                string token = Source.PossibleType.ToString();
                SyntaxError(-1, -1, "The case of '{0}' is wrong, it should be {1}, fixing and attempting to continue.",
                    Source.Token, token);

                Source.Reserved = Source.PossibleType;
                Source.Type = TokenTypeEnum.RESERVED;
                Source.Token = token;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse reference. Ref
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ParseFixRef()
        {
            SyntaxError(-1, -1, "Found a Ref without a type. Assuming a type.");
            Source.Reserved = ReservedEnum.INTEGER;
            Source.Type = TokenTypeEnum.RESERVED;
            Source.Token = "Integer";
            IsRef = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse named type.
        /// it's not a system type so it could be a class name or an unknown type
        /// we need to look ahead to see if there is another name following this on.
        /// make sure it's a name, if so look ahead, else it's just an error.
        /// </summary>
        /// <param name="pos">      The position. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseNamedType(int pos)
        {
            var typPos = Source.Start;
            if (Name == null)
            {
                SyntaxError(pos, Type.Length,
                    "{1}s require a type  (Real | Integer | String | Boolean | Object | Char), assuming missing type.\r\nExample:  Integer {0}",
                    Type, DeclType);
                Name = Type;
                Type = "Integer";
            }
            else
            {
                // it might be a class so add it as a reference and
                // check at the end if it was found.
                var cls = Context.FindClass(Type, Source.LineNumber);
                IsObject = true;
                if (cls == null)
                {
                    cls = new PClass(Context, Type, typPos);
                    cls.References.Add(Source.LineNumber);
                    cls.LineNumber = -1; // mark it as undefined
                    Context.Classes.Add(cls);
                }
                ClassRef = cls;
            }

            return ParseDimensionsAndValue();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse system type.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseSystemType()
        {
            Type = Source.Token;
            if (IsArgument && Source.Match("Ref"))
            {
                IsRef = true;
            }
            else if (IsArgument && Source.MatchNoCase("ref"))
            {
                SyntaxError(-1, -1, "The case of '{0}' is wrong, it should be {1}, fixing and attempting to continue.",
                    Source.Token, "Ref");
                IsRef = true;
            }

            return ParseNameAndValue();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse name and value.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ParseNameAndValue()
        {
            int pos;
            pos = Source.Position;

            Source.FlushWhitespace();

            Name = Parser.ParseName();
            if (Name == null)
            {
                SyntaxError(pos, 2, "Expected {0} name.", DeclType);
                Name = "<Missing>";
                return true;
            }

            return ParseDimensionsAndValue();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse dimensions and value.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseDimensionsAndValue()
        {
            Offset = Source.Start;
            Length = Name.Length;
            // add it to the context now
            var blk = Context.ScopeStack.Peek();
            if (!blk.Scope.Add(this))
            {
                SyntaxError(Offset, Length, "Variable '{0}' is already defined.", Name);
            }

            ParseDimensions();
            Source.FlushWhitespace();
            return ParseInitialValue();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse dimensions.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ParseDimensions()
        {
            PExpression dim;
            DimensionRank = 0;
            DimensionSize = new List<int>();
            while (Source.Match('['))
            {
                ++DimensionRank;
                if (Dimensions == null)
                {
                    Dimensions = new List<PExpression>();
                }
                // array declaration
                if (!IsArgument)
                {
                    Source.ParseToken();
                    var pos = Source.Position;

                    if ((dim = Parser.ParseExpression()) == null)
                    {
                        SyntaxError(pos, Source.Position - pos,
                            "Expected a expression for the array dimension like {0}[EXPRESSION].", Name);
                        dim = new PLiteralInteger(Context, "20");
                    }

                    Dimensions.Add(dim);
                }
                if (!Source.Match(']'))
                {
                    SyntaxError(Source.Position, 1, "Expected a closing ']' after the dimension of the array. ");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse initial value.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ParseInitialValue()
        {
            bool isNew = false;

            if (Source.Match("=="))
            {
                SyntaxError(-1, -1, "Used the wrong operator for assignment '==' should be '='");
            }

            else if (!Source.MatchFlush('='))
            {
                if (IsConstant)
                {
                    // we may still have an error so look if there is a literal here
                    var p = Source.Position;
                    Source.ParseToken();
                    var value = (Source.Type >= TokenTypeEnum.PUNCTUATION) ? "value" : Source.Token;
                    Source.Position = p;
                    SyntaxError(-1, 1,
                        "Missing '=' Expected a Constant to be initialized.  For Example Constant {0} {1} = {2}", Type,
                        Name, value);
                    Source.MoveNextLine();
                }
                return true;
            }

            if (IsObject && Source.ParseToken() != TokenTypeEnum.RESERVED ||
                Source.MatchTokenCheckCase("New", "Expected {0} but found {1}, Correcting."))
            {
                // assignment is a object.
                isNew = true;

            }

            PExpression expr;
            if (IsArray)
            {
                var list = new PExpressionList(Context);
                list.Parse();
                DimensionSize.Add( list.Count);
                expr = list;
            }
            else
            {
                expr = ParseInitialExpression(isNew);
            }

            InitialExpression = expr; //= list.Count == 1 ? list[0] : list;
            return expr!=null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse initial expression.
        /// </summary>
        /// <param name="isNew">    true if is new. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private PExpression ParseInitialExpression( bool isNew)
        {
            var pos = Source.Position;
            Source.ParseToken();
            var expr = Parser.ParseExpression();

            if (expr == null)
            {
                SyntaxError(pos, 2, "Expected a expression to set {0} to.", Name);
                {
                    return null;
                }
            }
            if (isNew)
            {
                // adjust to be a new expression
                expr = new PExpressionNew(Context, expr);
            }

            var lit = expr as PLiteral;

            if (lit!=null)
            switch(Type)
            {
                case "String":
                    if (lit.Store != StoreEnum.STRING)
                    {
                        SyntaxError(lit.Offset,lit.Length,"Expected a string literal here.  But found {0}",lit.ToString());
                    }
                    break;

                case "Real":
                    if (lit.Store != StoreEnum.REAL && lit.Store != StoreEnum.INTEGER)
                    {
                        SyntaxError( lit.Offset, lit.Length, "Expected a number literal here.  But found {0}", lit.ToString() );
                    }
                    break;
                case "Integer":
                    if (lit.Store != StoreEnum.INTEGER)
                    {
                        SyntaxError( lit.Offset, lit.Length, "Expected a integer literal here.  But found {0}", lit.ToString() );
                    }
                    break;
                case "Boolean":
                    if (lit.Store != StoreEnum.BOOL)
                    {
                        SyntaxError( lit.Offset, lit.Length, "Expected a string boolean here.  But found {0}", lit.ToString() );
                    }
                    break;
                case "Char":
                     if (lit.Store != StoreEnum.CHAR)
                    {
                        SyntaxError(lit.Offset,lit.Length,"Expected a character literal here.  But found {0}",lit.ToString());
                    }
                   break;
            }

            
            return expr;
        }

        #endregion

        #region Execute
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Gets a value relative to an active basepointer
        ///// </summary>
        ///// <param name="basePtr">  The base pointer. </param>
        ///// <returns>
        ///// The value.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public Accumulator GetValue(int basePtr)
        //{
        //    var runIndex = IsStatic ? ValueOffset : ValueOffset + basePtr;

        //    if (IsRef)
        //    {
        //        int n = Context.RunStack[runIndex].IValue;
        //        return Context.RunStack.GetValue( n, true );
        //    }
        //    return Context.RunStack.GetValue( runIndex, true );
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Sets a value relative to an active base pointer.
        ///// </summary>
        ///// <param name="basePtr">  The base pointer. </param>
        ///// <param name="value">    The value of this variable. </param>
        ///// <returns>
        ///// The value.
        ///// </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public void SetValue(int basePtr, Accumulator value)
        //{
        //    var runIndex = IsStatic ? ValueOffset : ValueOffset + basePtr;

        //    if (IsRef)
        //    {
        //        int n = Context.RunStack[runIndex].IValue;
        //        Context.RunStack.SetValue( value, n, true );
        //    }
        //    else
        //    {
        //        Context.RunStack.SetValue( value, runIndex, true );
        //    }
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets an array index by computing the runtime value.
        /// </summary>
        /// <param name="args"> The argument expressions to compute the index from. </param>
        /// <returns>
        /// The array index.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int GetArrayIndex(PExpressionList args)
        {
            if (Type == "String" && DimensionRank == 0)
            {
                args[0].Execute();
                var value = Context.RunStack.Pop().IValue;
                if (value < 0)
                {
                    RuntimeFatal( Offset, Length,LineNumber, "Index is negative, it must always be positive.");
                    value = 0;
                }
                return value;
            }

            var rank =  DimensionRank;
            int index = 0;
            int nbase = 1;

            if (args == null)
            {
                // pushing the array, return -1 as the index
                return -1;
            }
            for (int i = rank - 1; i >= 0; --i)
            {
                args[i].Execute();
                var value = Context.RunStack.Pop().IValue;
                if (value < 0)
                {
                    RuntimeFatal(Offset, Length, LineNumber, "Index is negative, it must always be positive.");
                    index = 0;
                }
                index += value * nbase;
                if (IsArgument)
                {
                    var v = Context.RunStack[RunIndex].Array;
                    nbase *= v.Dimensions[i];
                }
                else
                {
                    nbase *= DimensionSize[i];
                }
            }
            return index;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the stack index.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetStackIndex()
        {
            StackIndex = IsStatic ? ValueOffset : ValueOffset + Context.RunStack._basePtr;
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
            // this is the declaration of this variable, so we can set the StackIndex

            SetStackIndex();

            base.Execute(); // sets line number

            if (InitialExpression != null && !IsArray) 
            {
                if (InitialExpression.Execute() == ReturnState.STOP) return ReturnState.STOP;
                Value = Context.RunStack.Pop().ConvertTo( Type );
            }
            else if (IsArray)
            {
                return ExecuteArray();
            }
            else if (IsObject)
            {
                return ExecuteObject();
            }
            else
            {
                var v = new Accumulator(0);
                v.Store = StoreEnum.NULL;
                Value = v;
            }
            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the object operation.
        /// We compute the number of slots needed and allocate them on the stack
        /// 
        /// </summary>
        /// <returns>
        /// the next state
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState ExecuteObject()
        {
            // do nothing, the slot is left null until it's assigned a value.
            //int slots = Slots;

            //Value = new Accumulator( new PObject(ClassRef ));

            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes the array operation by computing the size and setting the array as the value.
        /// </summary>
        /// <returns>
        /// the next state
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ReturnState ExecuteArray()
        {
            var v = new Accumulator();
            long size = 1;
            foreach (var dimension in Dimensions)
            {
                if (dimension.Execute() == ReturnState.STOP)
                {
                    return  ReturnState.STOP;
                }
                var value = Context.RunStack.Pop().IValue;
                DimensionSize.Add(value);
                size *= value;
            }

            v.Array = new PCArray(DimensionSize, new Accumulator[size]);
            v.SetStore(Type);
            Value = v;
            var list = InitialExpression as PExpressionList;

            if (list != null)
            {
                for (int index   = 0; index < list.Expressions.Count; index++)
                {
                    var exp = list.Expressions[index];
                    exp.Execute();
                    v.Array[index]=Context.RunStack.Pop();
                }
            }
            return ReturnState.NEXT;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Makes a deep copy of this object.
        /// </summary>
        /// <returns>
        /// A copy of this object.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PVariable Clone()
        {
            return (PVariable) MemberwiseClone();
        }

        #endregion

        #region To String

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {

            builder.AppendFormat(IsRef ? "{0} Ref {1}" : "{0} {1}", Type, Name);
            if (Dimensions != null)
            {
                foreach (var dimension in Dimensions)
                {
                    builder.Append("[");
                    dimension.ToString(builder);
                    builder.Append(']');
                }
            }
            else if (DimensionRank > 0)
            {
                for (int i = 0; i < DimensionRank; i++)
                {
                    builder.Append("[]");
                }
            }
            if (InitialExpression != null)
            {
                builder.Append(" = ");
                if (IsObject)
                {
                    builder.Append("New ");
                }
                InitialExpression.ToString(builder);
            }
        }


        #endregion
    }
}
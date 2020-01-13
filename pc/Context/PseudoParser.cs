////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the parser class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   2/23/2015   rcs     Initial Implementation
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
using System.Collections.Generic;
using System.Configuration;
using ZCore;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Pseudo Parser. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PseudoParser
    {

        /// <summary>
        /// Gets the source.
        /// </summary>
        private PseudoSource Source {get { return Context.Source; }}

        /// <summary>
        /// Gets the error.
        /// </summary>
        public IUserOutput UserOutput {get { return Context.UserOutput; }}

        /// <summary>
        /// The context
        /// </summary>
        public PContext Context;

        /// <summary>
        /// The expression stack
        /// </summary>
        public Stack<PExpression> ExpressionStack = new Stack<PExpression>();

        private Stack<PseudoSource> _sourceStack = new Stack<PseudoSource>();


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="context"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PseudoParser(PContext context)
        {
            Context        = context;
            Context.Parser = this;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads a source.
        /// </summary>
        /// <param name="statements"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ParseStatements(List<PStatement> statements)
        {
            while (Source.ParseToken() != TokenTypeEnum.EOF)
            {
                if (!ParseStatement(statements)) break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Statements parser, must begin with a keyword.
        /// </summary>
        /// <param name="statements">   . </param>
        /// <returns>
        /// true if it succeeds, false if it fails to recognize the statement type. Reports the error if it does not
        /// recognize the statement keyword.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ParseStatement(List<PStatement> statements )
        {
            top:
            // look to see if its a case sensitive issue
            if (Source.Type == TokenTypeEnum.NAME)
            {
                if (Source.PossibleType != ReservedEnum.INVALID)
                {
                    // assume a case issue and fix it
                    string token = Source.PossibleType.ToString();
                    SyntaxError(-1,-1, "The case of '{0}' is wrong, it should be {1}, fixing and attempting to continue.", Source.Token, token );
                    Source.Reserved = Source.PossibleType;
                    Source.Type     = TokenTypeEnum.RESERVED;
                    Source.Token    = token;
                }
            }

            switch (Source.Type)
            {
                case TokenTypeEnum.NAME:
                    int start = Source.Start;
                    string name = Source.Token;
                    if (Source.ParseToken() == TokenTypeEnum.RESERVED)
                    {
                        if (Source.Reserved == ReservedEnum.MODULE)
                        {
                            SyntaxError(-1,-1, "This is incorrect format for a Module.  Repairing.  Correct format is\nModule '{0}'().", name );
                            var module = new PModule(Context);
                            module.ParseArgsAndBody(name);
                            break;
                        }
                    }
                    Source.Position = start;
                    Source.ParseToken();
                    // have a name so it could be a myObject.Function() call or myData = x  
                    ParseNameStatement(start, name, statements);
                    break;

                case TokenTypeEnum.OPERATOR:
                case TokenTypeEnum.PUNCTUATION:
                    if (Source.Token.Equals( ";" ))
                    {
                        SyntaxError( -1, -1, "Using an ';' here is not valid.\nAre you confusing psuedo-code with java or c++?" );
                        Source.ParseToken();
                        goto top;
                    }

                    if (Source.Token.Equals( "{" ) || Source.Token.Equals( "}" ))
                    {
                        SyntaxError( -1, -1, "Using an '{0}' here is not valid.\nAre you confusing psuedo-code with java or c++?",Source.Token );
                        Source.ParseToken();
                        goto top;
                    }
                    if (Source.Token.Equals( "," ))
                    {
                        SyntaxError( -1, -1, "Using an '{0}' here is not valid.  Remove it.",Source.Token );
                        Source.ParseToken();
                        goto top;
                    }
                    return false;
             
                case TokenTypeEnum.REAL:
                case TokenTypeEnum.INTEGER:
                case TokenTypeEnum.STRING:
                    string n;
                    string sx;
                    var s = PossibleStatement(statements, out n, out sx);
                    SyntaxError( -1, -1, "A Literal '{0}' here is not valid." + (s is PDisplay ? "\nPossible missing comma in Display Statement  like:\n{1} , {0}" : "\nPossible missing operator like:\n{1} + {0}."), Source.Token ,sx);
                    Source.ParseToken();
                    goto top;

                case TokenTypeEnum.RESERVED:
                    return Statement(statements);

                case TokenTypeEnum.COMMENT:
                case TokenTypeEnum.EOL:
                    break;

                case TokenTypeEnum.EOF:
                    return false;


                default:
                    SyntaxError(-1,-1, "Lines must start with (Display | Set | Declare)  found: {0}",Source.LineNumber);
                    return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse name statement.
        ///  look for name = value
        ///  look for name.Method()
        ///  look for name++     info advanced
        ///  look for ++name     info
        ///  look for Method()   Warning requires Call
        /// </summary>
        /// <param name="pos">          The position of this name. </param>
        /// <param name="name">         The name. </param>
        /// <param name="statements">   statement list</param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseNameStatement(int pos, string name, List<PStatement> statements)
        {
            // save the name position for reporting
          
            // look for '=' to see if its really a set statement
            if (Source.ParseToken() ==  TokenTypeEnum.OPERATOR && Source.Token == "=")
            {
                Source.Position = pos;
                // assume meant a set statement
                if (!Context.NoSet)
                {
                    Warning( pos, name.Length, "Setting a variable to a value requires a 'Set' before the assignment.\nFor Example:\n Set {0} = value", name );
                }
                return ParseStatement( statements, new PSet( Context ) );

            }
            while (Source.Token == ".")
            {
                // object method or object property
                var t = Source.ParseToken();
                if (t == TokenTypeEnum.NAME)
                {
                    if (Source.FlushWhitespace() == '(')
                    {
                       // member call
                        Source.Position = pos;
                        return ParseStatement( statements, new PCall( Context ) );
                    }
                    if (Source.CurrentChar!='.' && Source.CurrentChar != '[')
                    {
                        // it's a member access.
                        Source.Position = pos;
                        return ParseStatement( statements, new PSet(Context));
                    }
                    // member property or more complex call, goto next token
                }
            }

            // look for Method()
           
            if (Source.Token=="(")
            {
                // ok its a method without a call, report, backup and do a call
                Warning( pos, name.Length, "a call to a Module or Function  at the start of a line requires the keyword Call.\r\nFor Example:  Call {0}()", name );
                Source.Position = pos;
                return ParseStatement( statements, new PCall( Context ) );
            }
            if (Source.Token == "[")
            {
                // ok its a method without a call, report, backup and do a call
                Source.Position = pos;
                return ParseStatement( statements, new PSet( Context ) );
            }
         
            if (Source.Token == "++")
            {
                // ok its a method without a call, report, backup and do a call
                Warning( pos, name.Length, "Increment is an advanced feature of Set statements.\r\nFor Example:  Set {0} = {0}++", name );
                Source.Position = pos;
                return ParseStatement( statements, new PSet( Context ) );
            }
            
            // MatchTokenCheckCase()
            string n;
            string sx;
            var s = PossibleStatement(statements, out n, out sx);
            SyntaxError( pos, name.Length, " '{0}' here is not valid." + (s != null ? "\nPossible missing comma in {1} Statement like: \n{2} , {0}" : " This is not a statement, maybe a missing comma\n Should it be: , {0}  ?."), name,n,sx);
            Source.Position = pos;
            Source.ParseToken();

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Possible statement.
        /// </summary>
        /// <param name="statements">   . </param>
        /// <param name="n">            [out] The n. </param>
        /// <param name="sx">           [out] The sx. </param>
        /// <returns>
        /// the statement if it exists
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static PStatement PossibleStatement(List<PStatement> statements, out string n, out string sx)
        {
            var s = (statements.Count > 0) ? statements[statements.Count - 1] : null;
            n = s != null ? s.GetType().Name.Substring(1) : "";
            sx = s != null ? s.ToString().Trim() : "";
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Statements this object.
        /// </summary>
        /// <param name="statements"></param>
        /// <returns>
        /// the statement or null
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Statement(List<PStatement> statements)
        {
            switch (Source.Reserved.Value)
            {
                case ReservedEnum.CHART:
                    return ParseStatement(statements, new PChart(Context));

                case ReservedEnum.INCLUDE:
                    return ParseInclude();

                case ReservedEnum.PROGRAM:
                    Context.Root.ParseProgramStatement();
                    return true;
               
                case ReservedEnum.ASSERT:
                    return ParseStatement( statements, new PAssert( Context ) );
              
                case ReservedEnum.EXPECTFAIL:
                    return ParseStatement( statements, new PExpectFail( Context ) );

                case ReservedEnum.CALL:
                    return ParseStatement(statements, new PCall(Context));

                case ReservedEnum.INPUT:
                    return ParseStatement(statements, new PInput(Context));

                case ReservedEnum.DISPLAY:
                    return ParseStatement(statements, new PDisplay(Context));
  
                case ReservedEnum.DECLARE:
                    return ParseStatementMultiple(statements,new PDeclare(Context, false));

                case ReservedEnum.CLASS:
                    return ParseClass(statements);

                case ReservedEnum.CONSTANT:
                    return ParseStatementMultiple(statements, new PDeclare(Context, true));

                case ReservedEnum.SET:
                    return ParseStatement(statements,  new PSet(Context));

                case ReservedEnum.MODULE:
                    return ParseStatement(statements,  new PModule(Context));

                case ReservedEnum.SWITCH:
                    return ParseStatement(statements, new PSwitch(Context));

                case ReservedEnum.SELECT:
                    return ParseStatement( statements, new PSwitch(Context) );

                case ReservedEnum.IF:
                    return ParseStatement( statements, new PIf( Context ) );
              
                case ReservedEnum.FUNCTION:
                    return ParseStatement( statements, new PFunction( Context ) );

                case ReservedEnum.RETURN:
                    return ParseStatement(statements, new PReturn(Context));

                case ReservedEnum.WHILE:
                    if (Context.DoLevel > 0)
                    {
                        // possible part of Do - While
                        return false;
                    }
                    return ParseStatement( statements, new PWhileLoop( Context ) );

                case ReservedEnum.DO:
                    return ParseStatement( statements, new PDoLoop( Context ) );

                case ReservedEnum.FOR:
                    if (Source.Match("Each"))
                    {
                        return ParseStatement(statements, new PForEachLoop(Context));
                    }
                    return ParseStatement( statements, new PForLoop( Context ) );
        
                case ReservedEnum.FOREACH:
                    return ParseStatement( statements, new PForEachLoop( Context ) );

                case ReservedEnum.ELSE:
                case ReservedEnum.END:
                    
                case ReservedEnum.CASE:
                case ReservedEnum.DEFAULT:
                case ReservedEnum.UNTIL:
                    // end some level, let caller determine if this is the right token.
                    return false;

                case ReservedEnum.OBJECT:
                case ReservedEnum.CHAR:
                case ReservedEnum.REAL:
                case ReservedEnum.INTEGER:
                case ReservedEnum.STRING:
                case ReservedEnum.BOOLEAN:
                    var pos = Source.Start;
                    var t = Source.Token;
                    var n = Source.ParseToken();
                    if (n == TokenTypeEnum.NAME)
                    {
                        SyntaxError( pos,Source.Start-pos+Source.Token.Length, "Missing 'Declare' or 'Constant'\r\nAssuming you are trying to declare a variable:\r\n Declare {0} {1}", t ,Source.Token);
                        Source.Position = pos;
                        return ParseStatementMultiple( statements, new PDeclare( Context, false ) );
                    }
                        goto default;
                default:
                    SyntaxError(-1,-1, "Unknown statement starting with {0}. ",Source.Token);
                    Source.MoveNext();
                    return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse include "filepath".
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseInclude()
        {
            var pos = Source.Position;
            if (Source.ParseToken() != TokenTypeEnum.STRING)
            {
                SyntaxError(Source.Start,Source.TokenLength,"Expected a string file path to include.");
                return false;
            }
            var filePath = Source.Token.Trim('"', '\'');

            // Recursive parse the file path.
            _sourceStack.Push(Source);
            var current = new ZPath(Source.FilePath);
            var fp = FileUtil.PathCombine( current.Folder, filePath );

            Context.Source = new PseudoSource(Context,fp);
            if (!Context.Source.ReadText())
            {
                Context.Source = _sourceStack.Pop();
                SyntaxError( pos, Context.Source.Token.Length, "Can't read file " + filePath );
                return false;
            }
            Context.Root.Parse();
            Context.Source = _sourceStack.Pop();
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse class.
        /// </summary>
        /// <param name="statements">   . </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseClass(List<PStatement> statements)
        {
            Source.FlushWhitespace();
            var pos = Source.Position;

            string name = ParseName();

            if (name == null)
            {
                SyntaxError(pos,2, "Class declaration must have a name like:  Class MyClass" );
                name = "<UnknownClass>";
            }

            PClass cls = Context.FindClass( name, Source.LineNumber );
            if (cls == null)
            {
                cls = new PClass(Context,name, pos);
                Context.Classes.Add( cls );
            }
            else if (cls.LineNumber != -1)
            { 
                SyntaxError(pos,name.Length, "Class {0} defined more than once.", name );
            }

            cls.LineNumber = Source.LineNumber;
            cls.Offset     = pos;


            return ParseStatement( statements, cls);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse statement with mulitple initialization multiple.
        /// </summary>
        /// <param name="pDeclare"> The declare. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ParseStatementMultiple(List<PStatement> statements,PDeclare pDeclare)
        {
            if (!ParseStatement(statements,pDeclare)) return false;
            // Look for short hand declaration like Declare Real a,b,c   or Declare Real a=3, b=7
            while (Source.Match( ',' ))
            {
                var decl = new PDeclare( Context, pDeclare.IsConstant );
                decl.Variable.Type = pDeclare.Variable.Type;
                decl.Variable.ParseNameAndValue();
                statements.Add(decl);
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Statements parser, must begin with a keyword.
        /// </summary>
        /// <param name="statements">   Final list of statements to add this one to </param>
        /// <param name="statement">    The statement. </param>
        /// <returns>
        /// true if it succeeds, false if it fails to recognize the statement type. Reports the error if it does not
        /// recognize the statement keyword.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public  bool ParseStatement(List<PStatement> statements, PStatement statement)
        {
            if (statement.Parse())
            {
                statements.Add(statement);
            }
            return true;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse name of a variable
        /// </summary>
        /// <returns>
        /// the name or null if not found.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ParseName()
        {
            if (ParseVariable())
            {
                return Source.Token;
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse variable.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ParseVariable()
        {
            Source.FlushWhitespace();
            int pos = Source.Position;
            if (Source.ParseToken() != TokenTypeEnum.NAME)
            {
                Source.Position = pos;
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse assignment.
        /// </summary>
        /// <param name="isRequired">   true if is required. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ParseAssignment(bool isRequired)
        {
            var pos = Source.Position;

            if (Source.ParseToken() == TokenTypeEnum.OPERATOR && Source.Operator == OperatorEnum.EQU)
            {
                Source.ParseToken();
                return ParseExpression() != null;
            }
            if (isRequired)
            {
                SyntaxError(pos,2, "Expected an assignment expression.");
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse bool expression.
        /// </summary>
        /// <returns>
        /// the new expression.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpression ParseBoolExpression()
        {
            int lineNumber = Source.LineNumber;
            int start = Source.Position;
            var expr = Expression() ? ExpressionStack.Pop() : null;
            var len = Source.Position - start;
            var binary = expr as PExpressionBinary;
            if (binary != null)
            {
                if (!binary.Operator.IsLogical)
                {
                    SemanticError(start,len,lineNumber, "Expected a boolean expression but found: {0}", binary.ToString());
                }
                return expr;
            }
            var unary = expr as PExpressionUnary;
            if (unary != null)
            {
                if (!unary.Operator.IsLogical)
                {
                    SemanticError(start,len,lineNumber , "Expected a boolean expression but found: {0}", unary.ToString() );
                }
                return expr;
            }
            if (!(expr is PLiteralBool))
            {
                var rexpr = expr as PExpressionReference;
                if (expr != null)
                {
                    if (rexpr.Variable != null && rexpr.Variable.Type != "Boolean" )
                    {
                        SemanticError(start,len, lineNumber, "Expected a boolean constant expression found: {0}", expr != null ? expr.ToString() : "" );
                    }
                }
            }
            return expr;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse expression.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpression ParseExpression()
        {
           return Expression() ? ExpressionStack.Pop() : null;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    Expression =>   Conditional 
        ///                 | Assignment 
        ///                 .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Expression()
        {
            while (Source.Type == TokenTypeEnum.EOL)
            {
                Source.ParseToken();
            }

            return LogicalOr();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    LogicalOr => LogicalAnd ( '||' LogicalAnd )* .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool LogicalOr()
        {
            if (!LogicalAnd())
            {
                return false;
            }

            while (Source.Match( "||" ) || Source.MatchCheckCase( "OR" ))
            {
                string op = Source.Token;
                var pos   = Source.Start;

                Source.ParseTokenSkipEol();

                if (!LogicalAnd())
                {
                    SyntaxError(pos,Source.Position=pos, "Expression after logical operator '{0}'.",op);
                    return false;
                }

                //------------------------------------------------------------
                //build the stack up correctly  Right op Left
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                CheckForLogical( right, "Right Side", "OR" );
                var left = ExpressionStack.Pop();
                CheckForLogical( right, "Right Side", "OR" );
                ExpressionStack.Push( new PExpressionBinary( Context, left, right, OperatorEnum.LOR,pos) );
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    LogicalAnd =>  InclusiveOr ( '&&' InclusiveOr )* .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool LogicalAnd()
        {
            if (!InclusiveOr())
            {
                return false;
            }

            while (Source.Match( "&&" ) || Source.MatchCheckCase( "AND" ))
            {
                var pos = Source.Start;
                string op = Source.Token;

                Source.ParseTokenSkipEol();
           
                if (!InclusiveOr())
                {
                    SyntaxError(pos,Source.Position-pos, "Expected an expression after logical operator '{0}'.", op );
                    return false;
                }
                //------------------------------------------------------------
                //build the stack up correctly  Right op Left
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                CheckForLogical(right,"Right Side","AND");
                var left = ExpressionStack.Pop();
                CheckForLogical( right, "Right Side", "AND" );
                ExpressionStack.Push( new PExpressionBinary( Context, left, right, OperatorEnum.LAND,pos ) );
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Check for not logical.
        /// </summary>
        /// <param name="expr"> The expression. </param>
        /// <param name="side"> The side. </param>
        /// <param name="op">   The operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void CheckForLogical(PExpression expr, string side, string op)
        {
            var pos = Source.Position;
            var bexpr = expr as PExpressionBinary;
            if (bexpr != null)
            {
                if (!bexpr.Operator.IsLogical)
                {
                    SyntaxError(pos,Source.Position-pos, "This Expression '{0}' cannot be the {1} of a {2}. ", expr, side,op );
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    InclusiveOr =>  ExclusiveOr ( '|' ExclusiveOr )* .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool InclusiveOr()
        {
            if (!ExclusiveOr())
            {
                return false;
            }
            while (Source.Match("|"))
            {
                var pos = Source.Start;
                Source.ParseTokenSkipEol();

                if (!ExclusiveOr())
                {
                    SyntaxError(pos, Source.Position - pos, "Expression after '|'.");
                    return false;
                }

                //------------------------------------------------------------
                //build the stack up correctly  Right op Left
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                var left = ExpressionStack.Pop();
                ExpressionStack.Push(new PExpressionBinary(Context, left, right, OperatorEnum.OR,pos));
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    ExclusiveOr =>  InclusiveAnd ( '^' InclusiveAnd )* .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ExclusiveOr()
        {
            if (!InclusiveAnd())
            {
                return false;
            }
            while (Source.Match("XOR"))
            {
                var pos = Source.Start;
                Source.ParseTokenSkipEol();


                if (!InclusiveAnd())
                {
                    SyntaxError(pos, Source.Position - pos, "Expected Expression after '^'.");
                    return false;
                }

                //------------------------------------------------------------
                //build the stack up correctly  Right op Left
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                var left = ExpressionStack.Pop();
                ExpressionStack.Push(new PExpressionBinary(Context, left, right, OperatorEnum.XOR,pos));
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    InclusiveAnd =>   Equality ( '&' Equality )* .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool InclusiveAnd()
        {
            if (!Equality())
            {
                return false;
            }
            while (Source.Match("&"))
            {
                var pos = Source.Start;
                Source.ParseTokenSkipEol();

                if (!Equality())
                {
                    SyntaxError(pos, Source.Position - pos, "Expected an expression after '&'.");
                    return false;
                }

                //------------------------------------------------------------
                //build the stack up correctly  Right op Left
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                var left = ExpressionStack.Pop();
                ExpressionStack.Push(new PExpressionBinary(Context, left, right, OperatorEnum.AND,pos));
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    Equality =>   Relational ( '!=' Relational  | '==' Relational   )* .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool Equality()
        {
            if (!Relational())
            {
                return false;
            }
            //make sure the ordering of this match is the same as OpCompare
            if ((Source.MatchOneOf("==", "!=", "=")) >= 0)
            {
                if (Source.Token == "=")
                {
                    SyntaxError(-1, -1, "Used '=' instead of '==' for an exact comparison.  Substituting and continuing.");
                    Source.Token = "==";
                }
                OperatorEnum opBin = Source.Token;
                var pos = Source.Start;
                Source.ParseTokenSkipEol();

                if (!Relational())
                {
                    SyntaxError(pos,Source.Position-pos, "Expected an expression after {0} operator.", opBin.ToString());
                    return false;
                }

                //------------------------------------------------------------
                //build the stack up correctly  Right op Left
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                var left = ExpressionStack.Pop();
                ExpressionStack.Push(new PExpressionBinary(Context, left, right, opBin, pos));
            }
            return true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    Relational =>   Shift [ 'lt' Shift   | '>' Shift   | '>=' Shift   | 'lte' Shift   ] .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool Relational()
        {
            if (!Shift())
            {
                return false;
            }
            //if (!Shift())
            //{
            //    return false;
            //}
            //make sure the ordering of this match is the same as OpCompare, match double char's first
            if ((Source.MatchOneOf("<=", ">=", "<", ">")) >= 0)
            {
                OperatorEnum opBin = Source.Token;
                var pos = Source.Start;
                Source.ParseTokenSkipEol();

                if (!Shift())
                {
                    SyntaxError(pos,Source.Position-pos, "Expected an expression after {0} operator.", opBin.ToString());
                    return false;
                }
            // skip shift
                //if (!Shift())
                //{
                //    SyntaxError(-1,-1, "Expected an expression after {0} operator.", opBin.ToString() );
                //    return false;
                //}
                //------------------------------------------------------------
                //build the stack up correctly  Right op Left
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                CheckForNotLogical(right,"Right side");
                var left = ExpressionStack.Pop();
                CheckForNotLogical( left, "Left side" );
                ExpressionStack.Push( new PExpressionBinary( Context, left, right, opBin, pos) );
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Check for not logical.
        /// </summary>
        /// <param name="expr"> The expression. </param>
        /// <param name="side"> The side. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void CheckForNotLogical(PExpression expr,string side)
        {
            var pos = Source.Position;
            var bexpr = expr as PExpressionBinary;
            if (bexpr != null)
            {
                if (bexpr.Operator.IsLogical)
                {
                    SyntaxError(pos,Source.Position-pos, "This Expression '{0}' cannot be the {1} of a comparison.  Are you missing an AND or an OR", expr,side );
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    Shift =>  Additive ( 'shl' Additive   | '>>' Additive   )* .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool Shift()
        {
            if (!Additive())
            {
                return false;
            }
            while ((Source.MatchOneOf("<<", ">>")) >= 0)
            {
                OperatorEnum opBin = Source.Token;
                var pos = Source.Position;
                Source.ParseTokenSkipEol();

                if (!Additive())
                {
                    SyntaxError(pos, Source.Position - pos, "Expected an expression after << or >> operator.");
                    return false;
                }
                //------------------------------------------------------------
                //build the stack up correctly  Right op Left
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                var left = ExpressionStack.Pop();
                ExpressionStack.Push(new PExpressionBinary(Context, left, right, opBin,pos));
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    Additive =>   Multiplier ( '+' Multiplier   | '-' Multiplier   )* .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool Additive()
        {
            if (!Multiplier())
            {
                return false;
            }
            while ((Source.MatchOneOf("+", "-")) >= 0)
            {
                OperatorEnum opBin = Source.Token;
                var pos = Source.Start;

                Source.ParseTokenSkipEol();

                if (!Multiplier())
                {
                    SyntaxError(pos,Source.Position-pos, "Expected an expression after + or - operator.");
                    return false;
                }


                //------------------------------------------------------------
                // Render the stack up correctly (i.e. Right op Left)
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                var left = ExpressionStack.Pop();
                var binaryExpression = new PExpressionBinary(Context, left, right, opBin, pos);


                ExpressionStack.Push(binaryExpression);
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    Multiplier =>   Unary ( '*' Postfix   | '/'   Postfix   | 'per' Postfix  | '%'   Postfix   )* .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool Multiplier()
        {
            if (!Exponent())
            {
                return false;
            }

            int index;
            while ((index = Source.MatchOneOf("*", "/", "MOD")) >= 0)
            {
                int opPos = Source.Start;
                if (index == 1 && Source.CurrentChar == '/')
                {
                    // oops - its a comment... back up 1 and return
                    Source.Position = Source.Position - 1;
                    return true;
                }

                OperatorEnum opBin = Source.Token;

                var pos = Source.Position;

                Source.ParseTokenSkipEol();

                if (!Exponent())
                {
                    SyntaxError(pos,Source.Position-pos, "Expected an expression after *,/ or mod operator.");
                    return false;
                }
                //------------------------------------------------------------
                //build the stack up correctly  Right op Left
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                var left = ExpressionStack.Pop();
                ExpressionStack.Push(new PExpressionBinary(Context, left, right, opBin,opPos));

            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Exponents this object.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool Exponent()
        {
            if (!Factor())
            {
                return false;
            }

            while ( Source.Match( '^' ) )
            {
                OperatorEnum opBin = Source.Token;
                var pos = Source.Start;

                Source.ParseTokenSkipEol();

                if (!Factor())
                {
                    SyntaxError(pos,Source.Position-pos, "Expected an expression after ^  operator." );
                    return false;
                }
                //------------------------------------------------------------
                //build the stack up correctly  Right op Left
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                var left = ExpressionStack.Pop();
                ExpressionStack.Push( new PExpressionBinary( Context, left, right, opBin, pos) );

            }
            return true;

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    Factor =>  Unary [ '%' | 'TimeUnit'] .
        /// </summary>
        /// <example>
        ///    x = (a+b) * y%;
        ///    x = 2 Hours;
        /// </example>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool Factor()
        {
            if (!Unary())
            {
                return false;
            }

            var pos = Source.Position;

            if (Source.Match("++"))
            {
                var expr = ExpressionStack.Pop();

                ExpressionStack.Push(new PExpressionUnary(Context, expr, OperatorEnum.POSTINC));
            }
            else if (Source.Match("--"))
            {
                var expr = ExpressionStack.Pop();
                ExpressionStack.Push(new PExpressionUnary(Context, expr, OperatorEnum.POSTDEC));
            }


            return true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Defines a unary expression.
        /// </summary>
        /// <remarks>
        ///     Unary =>  '-'  Expression
        ///             | '+'  Expression
        ///             | '!'  Expression
        ///             | '~'  Expression
        ///             | '++' Expression
        ///             | '--' Expression
        ///             | '(' Expression ')'                                            # simple nesting expression
        ///             | '(' type       ')'  Expression                                # cast
        ///             | Literal
        ///             | Reference.
        ///
        /// Reference   = Identifier [RefClause].                                       # reference another object
        ///
        /// RefClause   = '.' Reference
        ///             | '[' Instance ']'        [RefClause]                            # dict ref or array reference
        ///             | '(' ExpressionList ')'  [RefClause]                            # method call
        ///             .
        ///
        ///
        /// Instance    = '[' Expression [ '..' Expression | (',' Expression)* ] ']'     # 1 or range of
        ///             | '[' '..' ']'                                                   # all
        ///             .
        /// </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool Unary()
        {
            //------------------------------------------------------------
            //unary operators
            //------------------------------------------------------------
          //  Source.FlushWhitespace();

            OperatorEnum op = IsUnaryOp();

            if (op != OperatorEnum.INVALID)
            {
                var pos = Source.Position;

                if (!Unary())
                {
                    SyntaxError(pos,Source.Position-pos, "Expected expression following unary operator.");
                    return false;
                }
                //------------------------------------------------------------
                //build the stack up correctly  op Right
                //------------------------------------------------------------
                var right = ExpressionStack.Pop();
                ExpressionStack.Push(new PExpressionUnary(Context, right, op));
                return true;
            }

            //------------------------------------------------------------
            // parentheses can be:
            // (expression)        anything else (maintain parens)
            // (type)              single identifier (assume type)
            //------------------------------------------------------------
            if (Source.Token == "(")
            {
                Source.ParseTokenSkipEol();

                if (Expression())
                {
                    var pos = Source.Position;

                    if (Source.Match(')'))
                    {
                        return true;
                    }
                    SyntaxError(pos,1, "Missing closing ')'");
                    return false;
                }
            }

            if (Literal())
            {
                return true;
            }

            return Named();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse expression list.
        /// </summary>
        /// <returns>
        /// list  it succeeds, null if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PExpressionList ParseExpressionList()
        {
            var ilist = new PExpressionList(Context);

            do
            {
                Source.ParseTokenSkipEol();

                if (Expression())
                {
                    ilist.Add(ExpressionStack.Pop());
                }
                else
                {
                    break;
                }
                
            } while (Source.MatchFlush(','));

            return ilist;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    Literal =>   'Integer' 
        ///             | 'Real' 
        ///             | 'Character' 
        ///             | 'String' 
        ///             | Bool 
        ///             | null
        ///             .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool Literal()
        {
            if (Source.MatchTokenCheckCase("null","Expected {0} found {1}"))
            {
                ExpressionStack.Push(new PLiteralNull(Context));
                return true;
            }
            switch (Source.Type)
            {
                case TokenTypeEnum.NAME:
                    if (Source.MatchTokenCheckCase( "Tab", "Expected a {0} but found {1}", Source.Start ))
                    {
                        ExpressionStack.Push(new PLiteralString(Context,"\t"));
                        return true;
                    }
                    if (Source.MatchTokenCheckCase( "CR", "Expected a {0} but found {1}", Source.Start ))
                    {
                        ExpressionStack.Push( new PLiteralString( Context, "\r" ) );
                        return true;
                    }
                    if (Source.MatchTokenCheckCase( "BEL", "Expected a {0} but found {1}", Source.Start ))
                    {
                        ExpressionStack.Push( new PLiteralString( Context, "\a" ) );
                        return true;
                    }
                    if (Source.MatchTokenCheckCase("CLR", "Expected a {0} but found {1}", Source.Start))
                    {
                        ExpressionStack.Push(new PLiteralString(Context, "\f"));
                        return true;
                    }

                    break;
                case TokenTypeEnum.OPERATOR:
                    // missing an expression, create an expression
                    ExpressionStack.Push(new PExpressionReference(Context,"<Missing Expr>"));
                    SyntaxError(-1,-1, "Missing Expression, before the '{0}' operator.", Source.Token );
                    Source.Position = Source.Start - 1;
                    return true;

                case TokenTypeEnum.REAL:
                    ExpressionStack.Push(new PLiteralReal(Context, Source.Token));
                    return true;

                case TokenTypeEnum.INTEGER:
                    ExpressionStack.Push(new PLiteralInteger(Context, Source.Token));
                    return true;

                case TokenTypeEnum.CHAR:
                    ExpressionStack.Push(new PLiteralChar(Context, Source.Token));
                    return true;

                case TokenTypeEnum.STRING:
                    ExpressionStack.Push(new PLiteralString(Context, Source.Token));
                    return true;

                case TokenTypeEnum.RESERVED:
                    // check for boolean
                    return Bool();

                case TokenTypeEnum.COMMENT:
                case TokenTypeEnum.EOL:
                case TokenTypeEnum.EOF:
                case TokenTypeEnum.PUNCTUATION:

                default:
                    break;
            }



            return false;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///    Bool =>   'true' |'on'|'yes'  | 'false' |'off'|'no'  .
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Bool()
        {
            if (Source.MatchTokenCheckCase("True", "Case of {1} is wrong, should be {0}"))
            {
                Source.Token = "True";
            }
            else if (Source.MatchTokenCheckCase("False", "Case of {1} is wrong, should be {0}"))
            {
                Source.Token = "False";
            }
            else
            {
                return false;
            }
            ExpressionStack.Push(new PLiteralBool(Context, Source.Token));
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Defines a Reference.
        /// Enter with 
        /// </summary>
        /// <remarks>
        /// Reference   = Identifier [RefClause].                                       # reference another object
        ///
        /// RefClause   = '.' Reference
        ///             | '[' Instance ']'        [RefClause]                            # dict ref or array reference
        ///             | '(' ExpressionList ')'  [RefClause]                            # method call
        ///             .
        ///
        ///
        /// Instance    = '[' Expression [ '..' Expression | (',' Expression)* ] ']'     # 1 or range of
        ///             | '[' '..' ']'                                                   # all
        ///             .
        ///
        /// </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Named()
        {
            int start = Source.Start;

            if (Source.Type != TokenTypeEnum.NAME)
            {
                return false;
            }
            var reference = new PExpressionReference(Context, Source.Token);
            ExpressionStack.Push(reference);

            return reference.Parse();
            //if (Source.Match('.'))
            //{
            //    reference.ObjectName = reference.Name;
            //    reference.Name = ParseName();
            //}
            //if (Source.MatchFlush('('))
            //{
            //    // Method
            //    reference.Args = new PExpressionList(Context);
            //    reference.IsCall = true;
            //    reference.ParseArgs(')');

            //    Context.UnresolvedFunctions.Add(reference);
            //    ExpressionStack.Push(reference);
            //    return true;

            //}

            //reference.BindToVariable(start);

            //if (Source.Match('['))
            //{
            //    // array
            //    reference.Args = new PExpressionList(Context);
            //    reference.IsArray = true;
            //    Source.FlushWhitespace();
            //    reference.ParseArgs(']');
            //}

            //ExpressionStack.Push(reference);
            //return true;
        }

        #region Enumeration parsing helpers


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Determines whether [is unary op].
        /// </summary>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public OperatorEnum IsUnaryOp()
        {
            OperatorEnum op = OperatorEnum.INVALID;

            if (Source.MatchTokenCheckCase( "NOT" ,"Expected {0} found {1}. "))
            {
                op = OperatorEnum.NOT;
                Source.FlushToToken();
                return op;
            }
            if (!string.IsNullOrEmpty(Source.Token))
            {
                switch (Source.Token[0])
                {
                    case '-':
                        op = OperatorEnum.SUB;
                        if (Source.NextChar == '-')
                        {
                            op = OperatorEnum.DEC;
                            Source.MoveNext();
                        }
                        break;

                    case '+':
                        op = OperatorEnum.ADD;
                        if (Source.NextChar == '+')
                        {
                            op = OperatorEnum.INC;
                            Source.MoveNext();
                        }
                        break;

                    case '!':
                        op = OperatorEnum.NOT;
                        break;

                    case '~':
                        op = OperatorEnum.BINV;
                        break;

                    default:
                        return op;
                }
            }
            Source.FlushToToken();
            return op;
        }


        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Warning.
        /// </summary>
        /// <param name="offset">   The offset. </param>
        /// <param name="length">   The length. </param>
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Warning(int offset, int length, string format, params object[] args)
        {
            Record(offset,length, ErrorLevel.WARNING, -1, "Warning: " + format, args );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Syntax error.
        /// </summary>
        /// <param name="offset">   The offset. </param>
        /// <param name="length">   The length. </param>
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SyntaxError(int offset, int length, string format, params object[] args)
        {
            Record( offset,length , ErrorLevel.ERROR, -1, "Syntax error: " + format, args );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Lexical error.
        /// </summary>
        /// <param name="offset">   The offset. </param>
        /// <param name="length">   The length. </param>
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LexicalError(int offset, int length, string format, params object[] args)
        {
            Record(offset,length, ErrorLevel.ERROR, -1, "Lexical error: " + format, args );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Semantic error.
        /// </summary>
        /// <param name="offset">       The offset. </param>
        /// <param name="length">       The length. </param>
        /// <param name="lineNumber">   line number found or -1 if use current. </param>
        /// <param name="format">       Describes the format to use. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SemanticError(int offset, int length, int lineNumber, string format, params object[] args)
        {
            Record(offset,length, ErrorLevel.ERROR, lineNumber, "Semantic error: " + format, args );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Runtime error.
        /// </summary>
        /// <param name="offset">       The offset. </param>
        /// <param name="length">       The length. </param>
        /// <param name="lineNumber">   line number found or -1 if use current. </param>
        /// <param name="format">       Describes the format to use. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RuntimeError(int offset, int length, int lineNumber, string format, params object[] args)
        {
            Record(offset,length, ErrorLevel.ERROR, lineNumber,   "Runtime error: " + format, args );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Runtime fatal.
        /// </summary>
        /// <param name="offset">       The offset. </param>
        /// <param name="length">       The length. </param>
        /// <param name="lineNumber">   line number found or -1 if use current. </param>
        /// <param name="format">       Describes the format to use. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RuntimeFatal(int offset, int length, int lineNumber, string format, params object[] args)
        {
            Record( offset, length, ErrorLevel.FATAL, lineNumber, "Runtime fatal error: " + format, args );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Records and error and line info.
        /// </summary>
        /// <param name="offset">       The offset. </param>
        /// <param name="length">       The length. </param>
        /// <param name="level">        the error level </param>
        /// <param name="lineNumber">   line number found or -1 if use current. </param>
        /// <param name="format">       Describes the format to use. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Record(int offset, int length, ErrorLevel level, int lineNumber, string format, params object[] args)
        {
            var errorMsg = string.Format( format, args );
            var source   = Context.Source;
            int lineNum  = (lineNumber >= 0) ? lineNumber : source.LineNumber;
            string line  = source.CurrentLine;
            int column   = offset - source.LineStart;
            int len      = length<0 ? source.Token.Length:length;
            int pos      = offset<0 ? source.Start:offset;
            var filePath = source.FilePath;

            Context.Report( level, filePath, lineNum, column, pos, len, line, errorMsg );
        }

    }

}
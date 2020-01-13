////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the std class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   10/5/2015   rcs     Initial Implementation
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
using System.Diagnostics;
using System.Threading;

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Std. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static class Std
    {
        public static Stopwatch Stopwatch = new Stopwatch();

        /// <summary>
        ///  The Context.
        /// </summary>
        public static PContext Context;

        /// <summary>
        /// The random generator
        /// </summary>
        public static Random rand = new Random();

        /// <summary>
        /// The _methods
        /// </summary>
        private static Dictionary<string,FunctionInfo> _methods;

        /// <summary>
        ///  The  elevator Control.
        /// </summary>
        private static PCElevatorControl _elevatorControl;

        /// <summary>
        /// Gets the methods.
        /// </summary>
        public static Dictionary<string,FunctionInfo>  Methods { get { return _methods?? GetMethods(); }}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Check arguments.
        /// </summary>
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or illegal values. </exception>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        /// <param name="argTypes"> List of types of the arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void CheckArguments(Accumulator[]  args,params StoreEnum[] argTypes)
        {
            if (argTypes.Length > 0 && argTypes[0] == StoreEnum.LIST)
            {
                return;
            }
            CheckArgCount(args, argTypes.Length);
            for (int index = 0; index < argTypes.Length; index++)
            {
                var type = argTypes[index];
                if (args[index].Store != type)
                {
                    throw new ArgumentException(string.Format("Expected a type of {0} found type of {1}", type.ToString(), args[index].Store));
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Check argument count.
        /// </summary>
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or illegal values. </exception>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        /// <param name="argTypes"> List of types of the arguments. </param>
        /// <param name="length"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void CheckArgCount(Accumulator[] args,  int length)
        {
            if (args.Length != length)
            {
                throw new ArgumentException(string.Format("Expected  {0} arguments found  {1}", length, args.Length));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the methods.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, FunctionInfo> GetMethods()
        {
            _methods = new Dictionary<string, FunctionInfo>
            {
                {"append"         ,new FunctionInfo("append",Append,StoreEnum.STRING, StoreEnum.STRING,StoreEnum.STRING)},
                {"startsWith"     ,new FunctionInfo("startsWith",StartsWith,StoreEnum.BOOL, StoreEnum.STRING,StoreEnum.STRING)},
                {"length"         ,new FunctionInfo("length",Length,StoreEnum.INTEGER,StoreEnum.STRING)},
                {"toUpper"        ,new FunctionInfo("toUpper",ToUpper,StoreEnum.STRING, StoreEnum.STRING)},
                {"toLower"        ,new FunctionInfo("toLower",ToLower,StoreEnum.STRING, StoreEnum.STRING)},
                {"subString"      ,new FunctionInfo("subString",SubString,StoreEnum.STRING ,StoreEnum.STRING ,StoreEnum.INTEGER,StoreEnum.INTEGER)},
                {"currency"       ,new FunctionInfo("currency",Currency,StoreEnum.STRING ,StoreEnum.REAL)},
                {"contains"       ,new FunctionInfo("contains",Contains,StoreEnum.BOOL, StoreEnum.STRING,StoreEnum.STRING)},
                {"insert"         ,new FunctionInfo("insert",Insert,StoreEnum.STRING, StoreEnum.STRING,StoreEnum.INTEGER,StoreEnum.STRING)},
                {"delete"         ,new FunctionInfo("delete",Delete,StoreEnum.STRING,  StoreEnum.STRING , StoreEnum.INTEGER,StoreEnum.INTEGER)},
                {"stringToInteger",new FunctionInfo("stringToInteger",StringToInteger,StoreEnum.INTEGER, StoreEnum.STRING)},
                {"stringToReal"   ,new FunctionInfo("stringToReal",StringToReal,StoreEnum.REAL, StoreEnum.STRING)},
                {"isInteger"      ,new FunctionInfo("isInteger",IsInteger,StoreEnum.BOOL, StoreEnum.STRING)},
                {"isReal"         ,new FunctionInfo("isReal",IsReal,StoreEnum.BOOL, StoreEnum.STRING)},
                {"isDigit"        ,new FunctionInfo("isDigit",IsDigit,StoreEnum.BOOL, StoreEnum.CHAR)},
                {"isLetter"       ,new FunctionInfo("isLetter",IsLetter,StoreEnum.BOOL, StoreEnum.CHAR)},
                {"isLower"        ,new FunctionInfo("isLower",IsLower,StoreEnum.BOOL, StoreEnum.CHAR)},
                {"isUpper"        ,new FunctionInfo("isUpper",IsUpper,StoreEnum.BOOL, StoreEnum.CHAR)},
                {"isWhitespace"   ,new FunctionInfo("isWhitespace",IsWhitespace,StoreEnum.BOOL, StoreEnum.CHAR)},
                {"random"         ,new FunctionInfo("random",Random,StoreEnum.INTEGER,StoreEnum.INTEGER,StoreEnum.INTEGER)},
                {"sqrt"           ,new FunctionInfo("sqrt",Sqrt,StoreEnum.REAL,StoreEnum.REAL)},
                {"sin"            ,new FunctionInfo("sin",Sin,StoreEnum.REAL,StoreEnum.REAL)},
                {"cos"            ,new FunctionInfo("cos",Cos ,StoreEnum.REAL,StoreEnum.REAL)},
                {"tan"            ,new FunctionInfo("tan",Tan,StoreEnum.REAL,StoreEnum.REAL)},
                {"abs"            ,new FunctionInfo("abs",Abs,StoreEnum.REAL,StoreEnum.REAL)},
                {"toInteger"      ,new FunctionInfo("toInteger",ToInteger,StoreEnum.INTEGER,StoreEnum.REAL)},
                {"toReal"         ,new FunctionInfo("toReal",ToReal,StoreEnum.REAL,StoreEnum.INTEGER)},
                {"round"          ,new FunctionInfo("round",Round,StoreEnum.REAL,StoreEnum.REAL,StoreEnum.INTEGER)},
                {"format"         ,new FunctionInfo("format",Format,StoreEnum.STRING,StoreEnum.LIST)},
                {"sleep"          ,new FunctionInfo("sleep",Sleep,StoreEnum.VOID,StoreEnum.REAL)},
                {"beep"          ,new FunctionInfo("beep",Beep,StoreEnum.VOID,StoreEnum.INTEGER,StoreEnum.REAL)},
                {"charToString"  ,new FunctionInfo("charToString",charToString,StoreEnum.STRING,StoreEnum.INTEGER)},
                {"initElevator"  ,new FunctionInfo("initElevator",initElevator,StoreEnum.INTEGER,StoreEnum.INTEGER,StoreEnum.INTEGER,StoreEnum.INTEGER,StoreEnum.REAL,StoreEnum.REAL,StoreEnum.OBJECT)},
                {"startElevator" ,new FunctionInfo("startElevator",startElevator,StoreEnum.VOID)},
                {"isElevatorRunning" ,new FunctionInfo("isElevatorRunning",isElevatorRunning,StoreEnum.BOOL)},
                {"seconds",       new FunctionInfo("seconds",Seconds,StoreEnum.REAL)},     
             };
            return _methods;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: is the Elevator Running
        /// </summary>
        /// <param name="parameters"> The parameters.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static Accumulator isElevatorRunning(Accumulator[] parameters)
        {
            //todo make kparameter
            return new Accumulator(_elevatorControl != null && _elevatorControl.IsRunning());
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: initialise the Elevator
        /// </summary>
        /// <param name="parameters"> The parameters.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static Accumulator initElevator(Accumulator[] parameters)
        {
            CheckArguments(parameters, StoreEnum.INTEGER, StoreEnum.INTEGER, StoreEnum.INTEGER, StoreEnum.REAL, StoreEnum.REAL, StoreEnum.OBJECT);

            // parse variables

            _elevatorControl     = new PCElevatorControl(Context);

            int floorCount       = parameters[0].IValue;
            int elevatorCount    = parameters[1].IValue;
            int maxPassengers    = parameters[2].IValue;
            double spawnRate     = parameters[3].DValue;
            double maxTime       = parameters[4].DValue;
            PObject elevator     = parameters[5].OValue;
            PFunction myFunction = elevator.ClassRef.Functions.Find(x=>x.Name=="decideNextFloor");
            // extract the array's from the elevator
            //currentDirection                   
            //currentFloor
            //floorButtonPressedUp[FLOOR_COUNT]
            //floorButtonPressedDown[FLOOR_COUNT]
            //floorStops[FLOOR_COUNT]

            _elevatorControl.SetFields(
                FindField(elevator, "floorStops"),
                FindField(elevator, "floorButtonPressedDown"),
                FindField(elevator, "floorButtonPressedUp"),
                FindField(elevator, "currentFloor"), 
                FindField(elevator, "currentDirection")
                );

            _elevatorControl.Init(floorCount, elevatorCount, maxPassengers, spawnRate, maxTime, myFunction, elevator);
            return new Accumulator(0);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Find the Field
        /// </summary>
        /// <param name="elevator"> The elevator.</param>
        /// <param name="name">     The name.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static PField FindField(PObject elevator, string name)
        {
            return elevator.ClassRef.Fields.Find(x => x.Variable.Name == name);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: start the Elevator, blocks until done.
        /// </summary>
        /// <param name="parameters"> The parameters.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static Accumulator startElevator(Accumulator[] parameters)
        {
            _elevatorControl.Start();
            _elevatorControl = null;
            return new Accumulator(1);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: char To String
        /// </summary>
        /// <param name="parameters"> The parameters.</param>
        /// <returns>
        ///  The value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static Accumulator charToString(Accumulator[] parameters)
        {
            CheckArguments(parameters, StoreEnum.INTEGER);
            var c =(char) parameters[0].IValue;
            return new Accumulator(new String(c,1));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Beeps.
        /// </summary>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <returns>
        /// .
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static Accumulator Beep(Accumulator[] parameters)
        {
            CheckArguments( parameters, StoreEnum.INTEGER,StoreEnum.REAL );
            Console.Beep(parameters[0].IValue,(int) (parameters[1].DValue*1000));
            return new Accumulator(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Formats.
        /// </summary>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <returns>
        /// The formatted value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static Accumulator Format(Accumulator[] parameters)
        {
            string s = "";
            foreach (var parameter in parameters)
            {
                parameter.ConvertToString();
                s += parameter.SValue;
            }

            return new Accumulator(s);
        }
  
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// sleep for time.
        /// </summary>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <returns>
        /// The formatted value.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static Accumulator Sleep(Accumulator[] parameters)
        {
            CheckArguments( parameters, StoreEnum.REAL );
            Thread.Sleep( (int)(parameters[0].DValue * 1000) );

            return new Accumulator(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Built in delegate
        /// </summary>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <returns>
        /// an accumalator value
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public delegate Accumulator BuiltIn(params Accumulator[] parameters);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Currency formatter
        /// </summary>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <returns>
        /// Converts to string with $xxx,xxx,xxx.xx
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static Accumulator Currency(Accumulator[] parameters)
        {
            CheckArguments( parameters, StoreEnum.REAL );
            return new Accumulator(string.Format("{0:C}", parameters[0].DValue) );
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// length(string) returns length of string.
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// length
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Length(params Accumulator[] args)
        {
            CheckArguments(args,StoreEnum.STRING);
            return new Accumulator(args[0].SValue.Length);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// append(string,string) returns a new string appended of the members.
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// a concatenated string.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Append(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING , StoreEnum.STRING );
            return new Accumulator( args[0].SValue + args[1].SValue );

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Starts with a string
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// returns true if it starts with the second string.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator StartsWith(Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING, StoreEnum.STRING );
            return new Accumulator( args[0].SValue.ToLower().StartsWith( args[1].SValue.ToLower()) );
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// toUpper(string) convert to upper case.
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// the upper case string
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator ToUpper(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING );

            return new Accumulator( args[0].SValue.ToUpper() );

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// toLower(string) convert to lower case.
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// the lower case string
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator ToLower(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING );

            return new Accumulator( args[0].SValue.ToLower() );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// subString(string,start,end) gets a sub string.
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// the substring
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator SubString(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING ,StoreEnum.INTEGER,StoreEnum.INTEGER);

            var startIndex = args[1].IValue;

            var sValue = args[0].SValue;
            var length = args[2].IValue;
            return new Accumulator( sValue.Length>0 && startIndex<sValue.Length ? sValue.Substring(startIndex,length-startIndex+1):"" );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// contains(string,string) return true if string contains sencond string
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// true if contains string
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Contains(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING,StoreEnum.STRING );

            return new Accumulator( args[0].SValue.Contains(args[1].SValue) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// insert(string,position,string) inserts the second string at position
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// new string with insert
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Insert(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING,StoreEnum.INTEGER,StoreEnum.STRING );

            return new Accumulator( args[0].SValue.Insert(args[1].IValue,args[2].SValue) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// delete(string,start,end) deletes part of a string.
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// new string with delete
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Delete(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING , StoreEnum.INTEGER,StoreEnum.INTEGER);

            var startIndex = args[1].IValue;
            return new Accumulator( args[0].SValue.Remove( startIndex, args[2].IValue - startIndex+1) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// stringToInteger(string) convert to integer
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// integer
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator StringToInteger(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING );

            int i;

            return new Accumulator(int.TryParse( args[0].SValue,out i)?i:0 );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// stringToreal(string) convert to real
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// real
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator StringToReal(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING );

            double i;

            return new Accumulator( double.TryParse( args[0].SValue, out i ) ? i : 0 );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// isInteger(string) returns true if it's an integer
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// true if integer
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator IsInteger(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING );

            args[0].ConvertToInteger();
            return new Accumulator( args[0].Store== StoreEnum.INTEGER );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// isReal(string) returns true if is a real
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// true if real
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator IsReal(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.STRING );

            args[0].ConvertToDouble();
            return new Accumulator( args[0].Store == StoreEnum.REAL );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// isDigit(character) returns true if is a digit
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// true if digit
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator IsDigit(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.CHAR );

            char c = args[0].CValue;
            return new Accumulator(char.IsDigit(c));
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// isLetter(character) returns true if is a letter
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// true if letter
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator IsLetter(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.CHAR );
            char c = args[0].CValue;
            return new Accumulator( char.IsLetter( c ) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// isLower(character) returns true if is a lower case
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// true if lower case
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator IsLower(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.CHAR );
            char c = args[0].CValue;
            return new Accumulator( char.IsLower( c ) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// isUpper(character) returns true if is a upper case
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// true if upper
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator IsUpper(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.CHAR );

            char c = args[0].CValue;
            return new Accumulator(char.IsUpper(c));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// isWhitespace(character) returns true if is a whitespace
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// true if whitespace
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator IsWhitespace(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.CHAR );

            char c = args[0].CValue;
            return new Accumulator( char.IsWhiteSpace( c ) );
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// random(lower,upper) get a random number.    
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// return random in the range.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Random(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.INTEGER,StoreEnum.INTEGER );

            // fix to have inclusive upper bounds
            return new Accumulator( rand.Next(args[0].IValue, args[1].IValue+1));
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// random(lower,upper) get a random number.    
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// return random in the range.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Seconds(params Accumulator[] arg)
        {

            // fix to have inclusive upper bounds
            return new Accumulator((double)Stopwatch.ElapsedMilliseconds *.001);
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// sqrt(real) get the square root.
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// return square root
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Sqrt(params Accumulator[] args)
        {
           CheckArguments( args, StoreEnum.REAL);

            return new Accumulator(Math.Sqrt(args[0].DValue));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// abs(real) get the absolute value 
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// return abs
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Abs(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.REAL );
            return new Accumulator( Math.Abs( args[0].DValue ) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// cos(real) get the cosine
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// return cosine of number
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Cos(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.REAL );
            return new Accumulator( Math.Cos( args[0].DValue * ( Math.PI/180) ) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// sin(real) get the sine
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// return the sine
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Sin(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.REAL );
            return new Accumulator( Math.Sin( args[0].DValue * (Math.PI/180 )) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// tan(real) get the tangent
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// return the tangent
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Tan(params Accumulator[] args)
        {
            CheckArguments( args, StoreEnum.REAL );
            return new Accumulator( Math.Tan( args[0].DValue * (Math.PI / 180) ) );
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// round(real,digits) round the number
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// return random in the range.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator Round(params Accumulator[] args)
        {

            CheckArguments( args, StoreEnum.REAL,StoreEnum.INTEGER );
            return new Accumulator( Math.Round( args[0].DValue , args[1].IValue) );

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// toInteger(real) convert to integer  
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// integer value
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator ToInteger(params Accumulator[] args)
        {
            CheckArgCount( args, 1 );
             args[0].ConvertToInteger();
            return  args[0];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// toReal(integer) convert to real
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        /// <returns>
        /// real value
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Accumulator ToReal(params Accumulator[] args)
        {
            CheckArgCount( args,1);
            var ret = args[0].ConvertToDouble();
            return ret ? args[0] : new Accumulator(0.0);
        }

        //private const int CHAR_BIT = 8;
        //private const int SHIFT = sizeof(int) * CHAR_BIT - 1;

        ///*Function to find minimum of x and y*/
        //static int Min(int x, int y)
        //{
        //    return y + ((x - y) & ((x - y) >> SHIFT));
        //}

        ///*Function to find maximum of x and y*/
        //static int Max(int x, int y)
        //{
        //    return x - ((x - y) & ((x - y) >> SHIFT));
        //}

    }
}
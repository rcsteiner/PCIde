////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the class class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   4/14/2015   rcs     Initial Implementation
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

namespace pc
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class. 
    /// 
    /// Class Name
    ///     Private String Text
    ///     Private Function Integer MyFunction() End Function
    ///     Public  Module MyModule() End Module
    /// End Class
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PClass : PBlock
    {
        /// <summary>
        /// The name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets or sets the super class name.
        /// </summary>
        public string Super { get; set; }


        private PClass _superClass;

        /// <summary>
        /// Gets the super class.
        /// </summary>
        public PClass SuperClass { get {return _superClass ?? (Super != null ? (_superClass = Context.Classes.Find(x => x.Name == Super)) : null);} }



        /// <summary>
        /// The fields.
        /// </summary>
        public List<PField> Fields = new List<PField>();

        /// <summary>
        /// The modules.
        /// </summary>
        public List<PModule> Modules = new List<PModule>();

        /// <summary>
        /// The functions
        /// </summary>
        public List<PFunction> Functions = new List<PFunction>();

        /// <summary>
        /// Gets or sets the references.
        /// </summary>
        public List<int> References { get; set; }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        /// <param name="name">     (Optional) The name. </param>
        /// <param name="typPos"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PClass(PContext context, string name , int typPos) : base(context)
        {
            Name       = name;
            References = new List<int>();
            LineNumber = Source.LineNumber;
            Offset     = typPos;
            Length     = name != null ? name.Length:1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the final fields.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetFinalFields()
        {
            var fields = new List<PField>();

            RecurseFields(fields);

            for (int i = 0; i < fields.Count; ++i)
            {
                fields[i].Variable.ValueOffset = i;
            }
            Fields = fields;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Recurse fields.
        /// </summary>
        /// <param name="fields">   The fields. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void RecurseFields(List<PField> fields)
        {
            if (SuperClass != null)
            {
                    SuperClass.RecurseFields(fields);
            }
            fields.AddRange(Fields);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Reference search down the module list looking for a method name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// the method
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PModule RefModule(string name)
        {
            PModule  mod=null;
            var cls = this;
            var sc = SuperClass;

            if (sc == null && cls.Super != null)
            {
                sc = _superClass = Context.FindClass(cls.Super, cls.LineNumber);
                if (sc == null)
                {
                    SyntaxError(Offset,Length,"Super class {0} could not be found. Is it spelled right?",cls.Super);
                }
            }
            while (cls!=null && (mod = cls.Modules.Find(x => x.Name.Equals(name))) == null && sc != null)
            {
                cls = sc;  
                sc  = sc.SuperClass;
               if (sc!=null && cls!=null && sc.Name == cls.Name)
                {
                    SyntaxError( Offset, Length, "Class {0} can't have a super class of {1} (circular  reference.", cls.Name,
                        sc.Name );
                }
            }
            return mod;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Reference search down the module list looking for a method name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// the method
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PModule RefFunction(string name)
        {
            PFunction fun=null;
            var cls = this;
            while (cls != null && (fun = cls.Functions.Find( x => x.Name.Equals( name ) )) == null && SuperClass!=null)
            {
                if (cls == SuperClass) break;
                cls = SuperClass;
            }
            return fun;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parses this element.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Parse()
        {
            Context.CurrentClass = this;
            LineNumber = Source.LineNumber;
            Source.ExpectedEnd.Push("Class");

            EnterScope();
            int p = Source.Position;
            Source.FlushToToken();
            if (Source.MatchTokenCheckCase("Extends","Expected {0} but found {1}, correcting."))
            {
                if (Source.ParseToken() != TokenTypeEnum.NAME)
                {
                    SyntaxError(-1,-1, "Expected a super class name." );
                    Super = "<Missing Supper>";
                }
                else
                {
                    Super = Source.Token;
                }
            }
            else
            {
                Source.Position = p;
            }

            ParseDeclarations();
            LeaveScope();

            Context.CurrentClass = null;
            return true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse declarations.
        /// </summary>
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ParseDeclarations()
        {
            while (Source.ParseToken() != TokenTypeEnum.EOF)
            {
                if (Source.Type == TokenTypeEnum.RESERVED)
                {
                    switch (Source.Reserved.Value)
                    {
                        case ReservedEnum.END:
                            Source.MatchEndStatement("Class");
                            return;

                        case ReservedEnum.PUBLIC:
                        case ReservedEnum.PROTECTED:
                        case ReservedEnum.PRIVATE:
                            ParseDefnition();
                            break;

                        default:
                            SyntaxError( -1, -1, "{0} is not allowed in class defintion.", Source.Token );
                            continue;
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Parse defnition.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ParseDefnition()
        {

            VisibilityEnum visibility = (VisibilityEnum)(Source.Reserved.Value - ReservedEnum.PUBLIC);
            PStatement statement = null;
            int pos = Source.Position;
            if (Source.ParseToken() == TokenTypeEnum.RESERVED)
            {
                // its a standard field, module or function
                switch (Source.Reserved.Value)
                {
                    case ReservedEnum.FUNCTION:
                        var function = new PFunction(Context, visibility, true,this);
                        Functions.Add(function);
                        statement = function;
                        break;
                    case ReservedEnum.MODULE:
                        var module = new PModule(Context, visibility, true,this);
                        Modules.Add(module);
                        statement = module;
                        break;


                    case ReservedEnum.BOOLEAN:
                    case ReservedEnum.REAL:
                    case ReservedEnum.CHAR:
                    case ReservedEnum.STRING:
                    case ReservedEnum.INTEGER:
                    case ReservedEnum.OBJECT:
                        Source.Position = pos;
                        var field = new PField(Context, visibility);
                        field.Variable.ValueOffset = Fields.Count;
                        Fields.Add(field);
                        statement = field;
                        break;

                    default:
                        SyntaxError(-1,-1,"{0} is not allowed in class defintion.", Source.Token);
                        Source.MoveNextLine();
                        return;
                }
            }
            else if (Source.Type == TokenTypeEnum.NAME)
            {
                // its a name of a type
                // the current token is the type name
                var field = new PField( Context, visibility );
                // backup
                Source.Position = pos;
                Fields.Add( field );
                statement = field;
            }

            if (statement != null)
            {
                statement.Parse();
                Statements.Add( statement );
                var module = statement as PModule;
                if (module != null)
                {
                    if (module.Name == Name)
                    {
                        module.IsConstructor = true;
                    }
                }
            }
        }

        private PReturn ret()
        {
            return new PReturn(Context);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a string that represents the current element.
        /// </summary>
        /// <param name="builder">  The builder to add text to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ToString(StringBuilder builder)
        {
            builder.AppendFormat("Class {0}", Name);
            if (Super != null)
            {
                builder.AppendFormat(" Extends {0}", Super);
            }
            builder.AppendLine();
            base.ToString(builder);
            builder.Indent();
            builder.AppendLine("End Class");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Executes this element onto the runtime stack.
        /// </summary>
        /// <param name="argType"></param>
        /// <returns>
        ///  Next if continue else condition.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override ReturnState Execute()
        {
            Context.RunStack.SetCurrentLine( LineNumber, this ); // sets line number

            // process all statics and constants
            foreach (var field in Fields)
            {
                if (field.IsConstant || field.IsStatic)
                {
                    field.Execute();
                }
            }


            return ReturnState.NEXT;
        }
    }
}
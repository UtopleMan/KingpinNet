﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace KingpinNet
{
    using System.Linq;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class CommandHelp : CommandHelpBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write(" \r\n");
            this.Write("usage: ");
            
            #line 5 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
if (!string.IsNullOrEmpty(Application.Name)) { Write(Application.Name + " "); }
            
            #line default
            #line hidden
            
            #line 5 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Command.Item.Name));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 5 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
 if (Command.Item.Flags.Count > 0) { 
            
            #line default
            #line hidden
            this.Write("[<flags>] ");
            
            #line 5 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
 } if (Command.Item.Commands.Count > 0) { 
            
            #line default
            #line hidden
            this.Write("<command> ");
            
            #line 5 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
 } if (Command.Item.Arguments.Count > 1) { 
            
            #line default
            #line hidden
            this.Write("[<args> ...] ");
            
            #line 5 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
 } else if (Command.Item.Arguments.Count == 1) { 
            
            #line default
            #line hidden
            this.Write("[<");
            
            #line 5 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Command.Item.Arguments[0].Item.Name));
            
            #line default
            #line hidden
            this.Write(">]");
            
            #line 5 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n\r\n");
            
            #line 8 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Command.Item.Help ?? ""));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n");
            
            #line 10 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
 if (Command.Item.Flags != null) {
    var flags = Command.Item.Flags.Where(x => !x.Item.IsHidden).ToList();

    if (flags.Count != 0) {
        var maxFlagLength = flags.Max(x => x.Item.Name.Length + x.Item.DefaultValue.Length) + 9;
            
            #line default
            #line hidden
            this.Write("Flags:\r\n");
            
            #line 16 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
      foreach (var flag in flags) 
        {
            var defaultValue = "";
            if (!string.IsNullOrWhiteSpace(flag.Item.DefaultValue))
                defaultValue = "=" + flag.Item.DefaultValue;

            if (!string.IsNullOrWhiteSpace(flag.Item.ValueName))
                defaultValue = "=" + flag.Item.ValueName;

            if (flag.Item.ShortName != 0) { 
            
            #line default
            #line hidden
            this.Write("  -");
            
            #line 26 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(flag.Item.ShortName));
            
            #line default
            #line hidden
            this.Write(", --");
            
            #line 26 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(flag.Item.Name));
            
            #line default
            #line hidden
            
            #line 26 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(defaultValue));
            
            #line default
            #line hidden
            this.Write("   ");
            
            #line 26 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(flag.Item.Help));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 26 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateExamples(flag.Item.Examples)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 27 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
          }
            else { 
            
            #line default
            #line hidden
            this.Write("      --");
            
            #line 29 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(flag.Item.Name));
            
            #line default
            #line hidden
            
            #line 29 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(defaultValue));
            
            #line default
            #line hidden
            this.Write("   ");
            
            #line 29 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(flag.Item.Help));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 29 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateExamples(flag.Item.Examples)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 30 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
          }
        }
    }
}
            
            #line default
            #line hidden
            
            #line 34 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
if (Command.Item.Arguments != null && Command.Item.Arguments.Count != 0) { 
            
            #line default
            #line hidden
            this.Write("Args:\r\n");
            
            #line 36 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"

    var Arguments = new List<string>();
    var maxArgLength = Command.Item.Arguments.Max(x => x.Item.Name.Length) + 9;

    foreach (var arg in Command.Item.Arguments)
    {
        int spacing = maxArgLength - arg.Item.Name.Length;
        string finalString = $"  [<{arg.Item.Name}>]".PadRight(spacing); 
            
            #line default
            #line hidden
            
            #line 44 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(finalString));
            
            #line default
            #line hidden
            this.Write("   ");
            
            #line 44 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.Item.Help));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 44 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateExamples(arg.Item.Examples)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 45 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
  }
}
            
            #line default
            #line hidden
            
            #line 47 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
if (Command.Item.Commands != null && Command.Item.Commands.Count != 0) { 
            
            #line default
            #line hidden
            this.Write("Commands:\r\n");
            
            #line 49 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"

    var finalCommands = new List<Tuple<string, CommandLineItem>>();
    RecurseCommands("", Command.Item.Commands, finalCommands);

    var commandNameLength = finalCommands.Max(c => c.Item1.Length);

    foreach (var command in finalCommands)
    {
            
            #line default
            #line hidden
            this.Write("  ");
            
            #line 57 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(command.Item1));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 57 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CommandUsage(command.Item2)));
            
            #line default
            #line hidden
            this.Write("\r\n    ");
            
            #line 58 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(command.Item2.Help));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 58 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
GenerateExamples(command.Item2.Examples);
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 60 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
  }
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 62 "C:\Sources\KingpinNet\src\KingpinNet\CommandHelp.tt"
}
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public class CommandHelpBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}

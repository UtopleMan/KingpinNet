using System;
using System.Linq;
using System.Reflection;

namespace KingpinNet
{
    internal class OptionsParser
    {
        private KingpinApplication kingpinApplication;

        public OptionsParser(KingpinApplication kingpinApplication)
        {
            this.kingpinApplication = kingpinApplication;
        }

        internal void Parse(Type optionsType)
        {
            var properties = optionsType.GetProperties(BindingFlags.Public);
            foreach (var publicProperty in properties)
            {
                ParseProperty(publicProperty);
            }
        }

        private void ParseProperty(PropertyInfo publicProperty, CommandItem command)
        {
            if (publicProperty.CustomAttributes.Any(x => x.AttributeType is CommandAttribute))
            {
                ParseCommand(publicProperty, command);
            }
            else if (publicProperty.CustomAttributes.Any(x => x.AttributeType is FlagAttribute))
            {
                ParseFlag();
            }
            else if (publicProperty.CustomAttributes.Any(x => x.AttributeType is ArgumentAttribute))
            {
                ParseArgument();
            }
        }

        private void ParseCommand(PropertyInfo publicProperty, CommandItem command)
        {
            var propertyType = publicProperty.PropertyType;
            var name = propertyType.Name;
            var description = GetCommandDescription(publicProperty);
            // Create Command
            if (command == null)
            {
                // Create on application
                command = kingpinApplication.Command();
            }
            else
            {
                // create sub command
                command = command.Command();
            }
            // get ChildType
            var properties = optionsType.GetProperties(BindingFlags.Public);
            foreach (var publicProperty in properties)
            {
                ParseProperty(publicProperty);
            }
        }

        private string GetCommandDescription(PropertyInfo publicProperty)
        {
            var commandAttribute = publicProperty.GetCustomAttributes(typeof(CommandAttribute), true)[0] as CommandAttribute;
            if (commandAttribute != null)
            {
                return commandAttribute.Description;
            }
            return "";
        }

        private object GetDescription(Type optionsType)
        {
            throw new NotImplementedException();
        }
    }
    internal class CommandAttribute : Attribute
    {
        public string Description { get; private set; }

        public CommandAttribute(string description)
        {
            this.Description = description;
        }
    }

    internal class DefaultAttribute : Attribute
    {
        private string value;

        public DefaultAttribute(string value)
        {
            this.value = value;
        }
    }

    internal class ShortAttribute : Attribute
    {
        private char name;

        public ShortAttribute(char name)
        {
            this.name = name;
        }
    }

    internal class FlagAttribute : Attribute
    {
        private string description;

        public FlagAttribute(string description)
        {
            this.description = description;
        }
    }

    internal class ArgumentAttribute : Attribute
    {
        private string description;

        public ArgumentAttribute(string description)
        {
            this.description = description;
        }
    }
    internal class FileAttribute : Attribute
    {
        private bool fileExists;

        public FileAttribute(bool fileExists)
        {
            this.fileExists = fileExists;
        }
    }
    internal class CompletionsAttribute : Attribute
    {
        private string[] completions;

        public CompletionsAttribute(params string[] completions)
        {
            this.completions = completions;
        }
    }

}

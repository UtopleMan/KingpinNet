using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace KingpinNet.UI.DataTable
{
    public class DataTable : WidgetBase
    {
        private DataTableConfig config = new DataTableConfig
        {
            UseColor = false,
            BackgroundColor = ConsoleColor.Black,
            ForegroundColor = ConsoleColor.Gray,
            MinColumnLength = 6,
            MaxColumnLength = 20,
            Left = 30,
            Top = 30,
            Style = new SingleLine()
        };
        private object data;

        public DataTable(IConsole console, Action<DataTableConfig> configure = null) : base(console)
        {
            configure?.Invoke(config);
            SetConfig(config);
        }

        public void Update(object data)
        {
            this.data = data;
            Render();
        }
        protected override void Draw()
        {
            var values = new List<Dictionary<string, string>>();
            List<MemberInfo> fields = FindFields(data);
            var columnLengths = FindColumnLengths(data, config.MinColumnLength, config.MaxColumnLength, fields);
            var headers = FindHeaders(fields, columnLengths);

            if (data is IEnumerable)
            {
                foreach (var obj in (IEnumerable)data)
                {
                    var row = new Dictionary<string, string>();
                    foreach (var field in fields)
                    {
                        var valueString = GetValue(field, obj);
                        row[field.Name] = valueString.Length > columnLengths[field.Name] ? valueString.Substring(0, columnLengths[field.Name] - 2) + ".." : valueString.PadRight(columnLengths[field.Name]);
                    }
                    values.Add(row);
                }
            }
            else
            {
                var row = new Dictionary<string, string>();
                foreach (var field in fields)
                {
                    var valueString = GetValue(field, data);
                    row[field.Name] = valueString.Length > columnLengths[field.Name] ? valueString.Substring(0, columnLengths[field.Name] - 2) + ".." : valueString.PadRight(columnLengths[field.Name]);
                }
                values.Add(row);
            }

            console.SetCursorPosition(config.Left, config.Top);
            console.Write(config.Style.LeftVertical + " " + string.Join(" " + config.Style.ColumnVertical + " ", headers.Values) + " " + config.Style.RightVertical);
            var currentY = config.Top + 1;
            console.SetCursorPosition(config.Left, currentY++);
            console.Write(config.Style.StartTitleLine);

            var headerSeperators = headers.Values.Select(x => "".PadRight(x.Length + 2, config.Style.TitleLine));
            console.Write(string.Join(config.Style.ColumnSeperatorTitleLine, headerSeperators));
            console.Write(config.Style.EndTitleLine);
            foreach (var row in values)
            {
                console.SetCursorPosition(config.Left, currentY++);
                console.Write(config.Style.LeftVertical + " " + string.Join(" " + config.Style.ColumnVertical + " ", row.Values) + " " + config.Style.RightVertical);
            }
        }

        private Dictionary<string, string> FindHeaders(List<MemberInfo> fields, Dictionary<string, int> columnLengths)
        {
            var headers = new Dictionary<string, string>();

            foreach (var field in fields)
                headers[field.Name] = field.Name.Length > columnLengths[field.Name] - 2
                    ? field.Name.Substring(0, columnLengths[field.Name] - 2) + ".." : field.Name.PadRight(columnLengths[field.Name]);
            return headers;
        }

        private List<MemberInfo> FindFields(object data)
        {
            if (data is IEnumerable)
            {
                foreach (var obj in (IEnumerable)data)
                    return FindFieldsInSingleObject(obj);
            }
            else
            {
                return FindFieldsInSingleObject(data);
            }
            return new List<MemberInfo>();
        }

        private List<MemberInfo> FindFieldsInSingleObject(object data)
        {
            var type = data.GetType();
            var fields = type.GetFields().Where(x => x.IsPublic).Cast<MemberInfo>().ToList();
            fields.AddRange(type.GetProperties().Cast<MemberInfo>().ToList());
            return fields;
        }

        private Dictionary<string, int> FindColumnLengths(object data, int minColumnLength, int maxColumnLength, List<MemberInfo> fields)
        {
            var columnLengths = new Dictionary<string, int>();
            foreach (var field in fields)
                columnLengths[field.Name] = field.Name.Length > maxColumnLength ? maxColumnLength : Math.Max(field.Name.Length, minColumnLength);

            if (data is IEnumerable)
            {
                foreach (var obj in (IEnumerable)data)
                {
                    foreach (var field in fields)
                    {
                        var valueString = GetValueString(field, obj);
                        columnLengths[field.Name] = valueString.Length > columnLengths[field.Name] ? Math.Min(valueString.Length, maxColumnLength) : Math.Max(columnLengths[field.Name], minColumnLength);
                    }
                }
            }
            else
            {
                foreach (var field in fields)
                {
                    var valueString = GetValueString(field, data);
                    columnLengths[field.Name] = valueString.Length > columnLengths[field.Name] ? Math.Min(valueString.Length, maxColumnLength) : Math.Max(columnLengths[field.Name], minColumnLength);
                }
            }
            return columnLengths;
        }

        private string GetValueString(MemberInfo field, object obj)
        {
            object value;
            switch (field.MemberType)
            {
                case MemberTypes.Field:
                    value = ((FieldInfo)field).GetValue(obj);
                    break;
                case MemberTypes.Property:
                    value = ((PropertyInfo)field).GetValue(obj);
                    break;
                default:
                    return "-";
            }
            if (value == null)
                return "<null>";
            else
                return value.ToString();
        }
        private string GetValue(MemberInfo field, object obj)
        {
            object value;
            switch (field.MemberType)
            {
                case MemberTypes.Field:
                    value = ((FieldInfo)field).GetValue(obj);
                    break;
                case MemberTypes.Property:
                    value = ((PropertyInfo)field).GetValue(obj);
                    break;
                default:
                    throw new NotImplementedException();
            }
            if (value == null)
                return "<null>";
            return value.ToString();
        }
    }

    internal class SingleLine : IDataTableStyleContainer
    {
        public char StartTitleLine => '├';

        public char EndTitleLine => '┤';
        public char TitleLine => '─';

        public char ColumnSeperatorTitleLine => '┼';

        public char LeftVertical => '│';

        public char ColumnVertical => '│';

        public char RightVertical => '│';
    }

    public class DataTableConfig : IConfigBase
    {
        public bool UseColor { get; set; }

        public ConsoleColor ForegroundColor { get; set; }

        public ConsoleColor BackgroundColor { get; set; }
        public IDataTableStyleContainer Style { get; set; }
        public int MinColumnLength { get; set; }
        public int MaxColumnLength { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
    }

    public interface IDataTableStyleContainer
    {
        char StartTitleLine { get; }
        char EndTitleLine { get; }
        char TitleLine { get; }
        char ColumnSeperatorTitleLine { get; }
        char LeftVertical { get; }
        char ColumnVertical { get; }
        char RightVertical { get; }
    }
}


using System.IO;
using System.Text.Json;

namespace resource.preview
{
    public class JSON : cartridge.AnyPreview
    {
        protected override void _Execute(atom.Trace context, string url)
        {
            __Execute(JsonDocument.Parse(File.ReadAllText(url)).RootElement, 1, context, "");
        }

        private static void __Execute(JsonElement node, int level, atom.Trace context, string name)
        {
            var a_Index = 0;
            switch (node.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (JsonProperty a_Context in node.EnumerateObject())
                    {
                        __Execute(a_Context, level + 1, context, a_Context.Name);
                    }
                    break;
                case JsonValueKind.Array:
                    foreach (JsonElement a_Context in node.EnumerateArray())
                    {
                        a_Index++;
                        __Execute(a_Context, level + 1, context, "[" + a_Index.ToString() + "]");
                    }
                    break;
                default:
                    __Send(node, level, context, name);
                    break;
            }
        }

        private static void __Execute(JsonProperty node, int level, atom.Trace context, string name)
        {
            {
                __Send(node.Value, level, context, name);
            }
            switch (node.Value.ValueKind)
            {
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                    __Execute(node.Value, level + 1, context, name);
                    break;
            }
        }

        private static void __Send(JsonElement node, int level, atom.Trace context, string name)
        {
            if (string.IsNullOrEmpty(name) == false)
            {
                context.
                    Clear().
                    SetContent(GetCleanString(name)).
                    SetValue(__GetValue(node)).
                    SetComment(__GetComment(node)).
                    SetPattern(__GetPattern(node)).
                    SetHint("Data type").
                    SetLevel(level).
                    Send();
            }
        }

        private static string __GetComment(JsonElement node)
        {
            switch (node.ValueKind)
            {
                case JsonValueKind.Undefined: return "Undefined";
                case JsonValueKind.Object: return "Object";
                case JsonValueKind.Array: return "Array";
                case JsonValueKind.String: return "String";
                case JsonValueKind.Number: return "Number";
                case JsonValueKind.True: return "Boolean";
                case JsonValueKind.False: return "Boolean";
                case JsonValueKind.Null: return "Null";
            }
            return "";
        }

        private static string __GetValue(JsonElement node)
        {
            switch (node.ValueKind)
            {
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                    return "";
            }
            return GetCleanString(node.ToString());
        }

        private static string __GetPattern(JsonElement node)
        {
            switch (node.ValueKind)
            {
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                    return "";
            }
            return atom.Trace.NAME.PATTERN.VARIABLE;
        }
    };
}

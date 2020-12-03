
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace resource.preview
{
    internal class VSPreview : cartridge.AnyPreview
    {
        protected override void _Execute(atom.Trace context, string url)
        {
            var a_Context = JsonConvert.DeserializeObject(File.ReadAllText(url)) as JContainer;
            if ((a_Context != null) && a_Context.HasValues)
            {
                foreach (JToken a_Context1 in a_Context.Children())
                {
                    __Execute(a_Context1, 1, context, "");
                }
            }
            if (GetState() == STATE.CANCEL)
            {
                context.
                    SendWarning(1, NAME.WARNING.TERMINATED);
            }
        }

        private static void __Execute(JToken node, int level, atom.Trace context, string name)
        {
            if (GetState() == STATE.CANCEL)
            {
                return;
            }
            if ((node is JProperty) == false)
            {
                context.
                    SetContent(name).
                    SetValue(__GetValue(node)).
                    SetComment(__GetComment(node)).
                    SetType(__GetType(node)).
                    SetCommentHint("[[Data type]]").
                    SetLevel(level).
                    Send();
            }
            if (node.HasValues)
            {
                var a_Index = 0;
                foreach (JToken a_Context in node.Children())
                {
                    {
                        a_Index++;
                    }
                    if (node is JProperty)
                    {
                        __Execute(a_Context, level, context, (node as JProperty).Name);
                    }
                    else
                    {
                        __Execute(a_Context, level + 1, context, (node.Type == JTokenType.Array) ? "[" + a_Index.ToString() + "]" : "");
                    }
                }
            }
        }

        private static string __GetValue(JToken node)
        {
            return GetCleanString(node.ToString());
        }

        private static string __GetComment(JToken node)
        {
            switch (node.Type)
            {
                case JTokenType.None: return "[[None]]";
                case JTokenType.Object: return "[[Object]]";
                case JTokenType.Array: return "[[Array]]";
                case JTokenType.Constructor: return "[[Constructor]]";
                case JTokenType.Property: return "[[Property]]";
                case JTokenType.Comment: return "[[Comment]]";
                case JTokenType.Integer: return "[[Integer]]";
                case JTokenType.Float: return "[[Float]]";
                case JTokenType.String: return "[[String]]";
                case JTokenType.Boolean: return "[[Boolean]]";
                case JTokenType.Null: return "[[Null]]";
                case JTokenType.Undefined: return "[[Undefined]]";
                case JTokenType.Date: return "[[Time]]";
                case JTokenType.Raw: return "[[Raw]]";
                case JTokenType.Bytes: return "[[Bytes]]";
                case JTokenType.Guid: return "GUID";
                case JTokenType.Uri: return "URI";
                case JTokenType.TimeSpan: return "[[Time]]";
            }
            return "";
        }

        private static string __GetType(JToken node)
        {
            switch (node.Type)
            {
                case JTokenType.Object:
                case JTokenType.Array:
                    return NAME.TYPE.INFO;
            }
            return NAME.TYPE.VARIABLE;
        }
    };
}

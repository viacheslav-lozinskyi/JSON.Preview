using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace resource.preview
{
    internal class VSPreview : extension.AnyPreview
    {
        protected override void _Execute(atom.Trace context, int level, string url, string file)
        {
            var a_Context = JsonConvert.DeserializeObject(File.ReadAllText(file)) as JContainer;
            if ((a_Context != null) && a_Context.HasValues)
            {
                foreach (JToken a_Context1 in a_Context.Children())
                {
                    __Execute(context, level, a_Context1, "");
                }
            }
        }

        private static void __Execute(atom.Trace context, int level, JToken data, string name)
        {
            if (GetState() == NAME.STATE.WORK.CANCEL)
            {
                return;
            }
            if ((data is JProperty) == false)
            {
                context.
                    SetComment(__GetComment(data), "[[[Data Type]]]").
                    Send(NAME.SOURCE.PREVIEW, __GetType(data), level, name, __GetValue(data));
            }
            if (data.HasValues)
            {
                var a_Index = 0;
                foreach (JToken a_Context in data.Children())
                {
                    {
                        a_Index++;
                    }
                    if (data is JProperty)
                    {
                        __Execute(context, level, a_Context, (data as JProperty).Name);
                    }
                    else
                    {
                        __Execute(context, level + 1, a_Context, (data.Type == JTokenType.Array) ? "[" + a_Index.ToString() + "]" : "");
                    }
                }
            }
        }

        private static string __GetValue(JToken data)
        {
            return GetFinalText(data.ToString());
        }

        private static string __GetComment(JToken data)
        {
            switch (data.Type)
            {
                case JTokenType.None: return "[[[None]]]";
                case JTokenType.Object: return "[[[Object]]]";
                case JTokenType.Array: return "[[[Array]]]";
                case JTokenType.Constructor: return "[[[Constructor]]]";
                case JTokenType.Property: return "[[[Property]]]";
                case JTokenType.Comment: return "[[[Comment]]]";
                case JTokenType.Integer: return "[[[Integer]]]";
                case JTokenType.Float: return "[[[Float]]]";
                case JTokenType.String: return "[[[String]]]";
                case JTokenType.Boolean: return "[[[Boolean]]]";
                case JTokenType.Null: return "[[[Null]]]";
                case JTokenType.Undefined: return "[[[Undefined]]]";
                case JTokenType.Date: return "[[[Time]]]";
                case JTokenType.Raw: return "[[[Raw]]]";
                case JTokenType.Bytes: return "[[[Bytes]]]";
                case JTokenType.Guid: return "GUID";
                case JTokenType.Uri: return "URI";
                case JTokenType.TimeSpan: return "[[[Time]]]";
            }
            return "";
        }

        private static string __GetType(JToken data)
        {
            switch (data.Type)
            {
                case JTokenType.Object:
                case JTokenType.Array:
                    return NAME.EVENT.PARAMETER;
            }
            return NAME.EVENT.PARAMETER;
        }
    };
}

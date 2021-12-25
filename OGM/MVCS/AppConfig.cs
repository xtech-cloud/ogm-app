using System.Text.Json;
using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM
{
    class AppConfig : Config
    {
        public override void Merge(string _content)
        {
            Dictionary<string, object> schema = JsonSerializer.Deserialize<Dictionary<string, object>>(_content);
            foreach (var key in schema.Keys)
            {
                var value = (JsonElement)schema[key];
                if (JsonValueKind.String == value.ValueKind)
                    fields_[key] = Any.FromString(value.GetString());
                else if (JsonValueKind.Number == value.ValueKind)
                    fields_[key] = Any.FromFloat64(value.GetDouble());
                else if (JsonValueKind.True == value.ValueKind)
                    fields_[key] = Any.FromBool(true);
                else if (JsonValueKind.False == value.ValueKind)
                    fields_[key] = Any.FromBool(false);
            }
        }

        public void MergeDict(Dictionary<string, Any> _pairs)
        {
            foreach (var pair in _pairs)
            {
                fields_[pair.Key] = pair.Value; 
            }
        }

    }//class
}//namespace

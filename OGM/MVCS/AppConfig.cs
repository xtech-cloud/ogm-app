using System.Text.Json;
using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM
{
    class ConfigSchema
    {
        public string domain_public { get; set; }
        public string domain_private { get; set; }
        public string application { get; set; }
    }

    class AppConfig : Config
    {
        public override void Merge(string _content)
        {
            ConfigSchema schema = JsonSerializer.Deserialize<ConfigSchema>(_content);
            if (!string.IsNullOrEmpty(schema.domain_public))
                fields_["domain.public"] = Any.FromString(schema.domain_public);
            if (!string.IsNullOrEmpty(schema.domain_private))
                fields_["domain.private"] = Any.FromString(schema.domain_private);
            if (!string.IsNullOrEmpty(schema.application))
                fields_["application"] = Any.FromString(schema.application);
        }

    }//class
}//namespace

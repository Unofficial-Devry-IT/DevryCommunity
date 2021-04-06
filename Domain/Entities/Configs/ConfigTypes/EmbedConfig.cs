using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Configs.ConfigTypes
{
    public class EmbedConfig
    {
        public string Title { get; set; }
        public string Footer { get; set; }

        public string FieldsJSON { get; set; }

        [NotMapped]
        public List<string> Fields
        {
            get => Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(FieldsJSON);
            set
            {
                FieldsJSON = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            }
        }
    }
}
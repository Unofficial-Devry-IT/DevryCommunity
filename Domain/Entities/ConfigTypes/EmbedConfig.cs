using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Domain.Entities.ConfigTypes
{
    public class EmbedConfig
    {
        public string Title { get; set; }
        public string Footer { get; set; }

        public string FieldsJSON { get; set; }

        [NotMapped]
        public List<string> Fields
        {
            get => JsonConvert.DeserializeObject<List<string>>(FieldsJSON);
            set => FieldsJSON = JsonConvert.SerializeObject(value);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DevryCore.Common.Models;
using Newtonsoft.Json;

namespace DevryDomain.Models
{
    public class Config : EntityWithTypedId<string>, IExtendableObject
    {
        public Config()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string ConfigName { get; set; }
        
        /// <summary>
        /// The text used to invoke associated command
        /// </summary>
        public string CommandText { get; set; }

        public string Headline { get; set; }
        public string RestrictedRolesJSON { get; protected set; } = "{}";

        [NotMapped]
        public List<string> RestrictedRoles
        {
            get => JsonConvert.DeserializeObject<List<string>>(RestrictedRolesJSON);
            set => RestrictedRolesJSON = JsonConvert.SerializeObject(value);
        }
        
        public string ExtensionData { get; set; }
    }
}
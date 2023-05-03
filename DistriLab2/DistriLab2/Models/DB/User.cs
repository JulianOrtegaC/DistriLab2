using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DistriLab2.Models.DB
{
    public partial class User
    {
        public User()
        {
            Credentials = new HashSet<Credential>();
        }

        public string NameUser { get; set; } = null!;
        public string EmailUser { get; set; } = null!;
        public string StatusUser { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Credential> Credentials { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DistriLab2.Models.DB
{
    public partial class User
    {
        public User()
        {
            Credentials = new HashSet<Credential>();
        }

        public string CodUser { get; set; } = null!;
        public string NameUser { get; set; } = null!;
        public string EmailUser { get; set; } = null!;
        public string StatusUser { get; set; } = null!;

        public virtual ICollection<Credential> Credentials { get; set; }
    }
}

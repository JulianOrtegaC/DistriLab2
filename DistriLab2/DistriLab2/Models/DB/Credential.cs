using System;
using System.Collections.Generic;

namespace DistriLab2.Models.DB
{
    public partial class Credential
    {
        public string CodCredential { get; set; } = null!;
        public string HashUser { get; set; } = null!;
        public string EmailUser { get; set; } = null!;

        public virtual User? EmailUserNavigation { get; set; } = null;
    }
}

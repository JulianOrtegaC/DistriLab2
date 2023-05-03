using System;
using System.Collections.Generic;

namespace DistriLab2.Models.DB
{
    public partial class Subject
    {
        public Subject()
        {
            Inscriptions = new HashSet<Inscription>();
        }

        public int CodSubject { get; set; }
        public string NameSubject { get; set; } = null!;
        public int Quotas { get; set; }
        public string StatusSubject { get; set; } = null!;

        public virtual ICollection<Inscription> Inscriptions { get; set; }
    }
}

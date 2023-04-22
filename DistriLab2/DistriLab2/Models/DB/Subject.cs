using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DistriLab2.Models.DB
{
    public partial class Subject
    {
        public Subject()
        {
            Inscriptions = new HashSet<Inscription>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CodSubject { get; set; }

        public string NameSubject { get; set; } = null!;
        
        public int Quotas { get; set; }
        public string StatusSubject { get; set; } = null!;

        public virtual ICollection<Inscription> Inscriptions { get; set; }
    }
}

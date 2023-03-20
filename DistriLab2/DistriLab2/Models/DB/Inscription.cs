using System;
using System.Collections.Generic;

namespace DistriLab2.Models.DB
{
    public partial class Inscription
    {
        public int IdInscription { get; set; }
        public int CodStudent { get; set; }
        public int CodSubject { get; set; }
        public DateTime DateRegistration { get; set; }

        public virtual Student CodStudentNavigation { get; set; } = null!;
        public virtual Subject CodSubjectNavigation { get; set; } = null!;
    }
}

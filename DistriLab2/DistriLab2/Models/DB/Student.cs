using System;
using System.Collections.Generic;

namespace DistriLab2.Models.DB
{
    public partial class Student
    {
        public Student()
        {
            Inscriptions = new HashSet<Inscription>();
        }

        public int CodStudent { get; set; }
        public string FirstNameStudent { get; set; } = null!;
        public string LastNameStudent { get; set; } = null!;
        public string TypeDocument { get; set; } = null!;
        public string NumDocument { get; set; } = null!;
        public string StatusStudent { get; set; } = null!;
        public string GenderStudent { get; set; } = null!;

        public virtual ICollection<Inscription> Inscriptions { get; set; }
    }
}

using System;
using DistriLab2.Models.DB;

namespace DistriLab2.Models
{
	public class ResponseInscriptionJoin
	{
		public ResponseInscriptionJoin(int IdInscription, string FirstNameStudent,
            string LastNameStudent, string NameSubject)
		{
            this.IdInscription = IdInscription;
            this.FirstNameStudent = FirstNameStudent;
            this.LastNameStudent = LastNameStudent;
            this.NameSubject = NameSubject;
		}

        public int IdInscription { get; set; }

        public string FirstNameStudent { get; set; } = null!;
        public string LastNameStudent { get; set; } = null!;

        public string NameSubject { get; set; } = null!;
    }
}


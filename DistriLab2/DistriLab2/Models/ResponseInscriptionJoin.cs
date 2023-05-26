using System;
using DistriLab2.Models.DB;

namespace DistriLab2.Models
{
	public class ResponseInscriptionJoin
	{
		public ResponseInscriptionJoin(int idInscription, int codStudent,
            string nameStudent, int codSubject, string nameSubject, string dateRegistration)
		{
            this.idInscription = idInscription;
            this.codStudent = codStudent;
            this.nameStudent = nameStudent;
            this.codSubject = codSubject;
            this.nameSubject = nameSubject;
            this.dateRegistration = dateRegistration;
        }

        public int idInscription { get; set; }

        public int codStudent { get; set; }
        public string nameStudent { get; set; } = null!;

        public int codSubject { get; set; }
        public string nameSubject { get; set; } = null!;
        public string dateRegistration { get; set; } = null!;
    }
}


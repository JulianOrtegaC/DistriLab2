using System;
namespace DistriLab2.Models
{
	public class RequestSubject
	{
		public RequestSubject(){}

        public string NameSubject { get; set; } = null!;

        public int Quotas { get; set; }
        public string StatusSubject { get; set; } = null!;
    }
}


using System;
namespace DistriLab2.Models.DB
{
	public class RequestNewUser
	{
		public RequestNewUser()
		{}
        public string NameUser { get; set; } = null!;
        public string EmailUser { get; set; } = null!;
        public string StatusUser { get; set; } = null!;
        public string PasswordUser { get; set; } = null!;
    }
}


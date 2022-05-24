using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryMessage
{
    [Serializable]
    public class MySystemMessageQuery
    {
        public bool IsNewUser { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public byte[] Avatar { get; set; }



        public MySystemMessageQuery(bool isU, string login, string password)
        {
            IsNewUser = isU;
            Login = login;
            Password = password;
        }
    }
}

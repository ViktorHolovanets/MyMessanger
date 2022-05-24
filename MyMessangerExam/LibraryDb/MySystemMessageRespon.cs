using LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDb
{
    [Serializable]
    public class MySystemMessageRespon
    {
        public User user;
        public List<UserContact> users;
        public List<MyMessage> messages;
    }
}

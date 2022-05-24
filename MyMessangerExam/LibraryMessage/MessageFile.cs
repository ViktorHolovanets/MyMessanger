using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryMessage
{
    [Serializable]
    public class MessageFile
    {   
        public string ExtensionFile { get; set; }  // розширення
        public string NameFile { get; set; }  // назва файлу
        public byte[] ContentFile { get; set; }  // дані файлу
        DateTime dateTime { get; set; }  // час відправки
    }
}

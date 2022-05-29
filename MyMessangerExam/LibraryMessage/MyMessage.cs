using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LibraryMessage
{



    //  TypeMessage
    //  -1 - Disconnect
    //  0 - MySystemMessageQuery  повідомлення для серверу при вході/реєстрації клієнта
    //  0 - MySystemMessageRespon  повідомлення для клієнта при вході/реєстрації 
    //  1 - текстове повідомлення для іншого клієнта 
    //  2 - системне повідомлення для додавання нового клієнта
    //  3 - відправка/прийом колекції файлів
    //  4 - відeозв'язок
    //  5 - видалення повідомлення по ID
    [Serializable]
    public class MyMessage
    {
        public int Id { get; set; }
        public int TypeMessage { get; set; }  // тип повідомлення
        public bool IsMyMessage { get; set; } = false; //
        public byte[] Content { get; set; }  // дані
        public int UserFrom_Id { get; set; }  // від кого
        public int UserTo_Id { get; set; }  // кому
    }




}
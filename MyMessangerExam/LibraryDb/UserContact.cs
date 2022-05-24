using System;
using System.Collections.Generic;

namespace LibraryDb
{
    [Serializable]
    public class UserContact:NPrCh
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] AvatarContact { get; set; }
        private bool isnotread = false;
        public bool IsNotRead
        {
            get { return isnotread; }
            set => Set(ref isnotread, value);
        }       
    }
}
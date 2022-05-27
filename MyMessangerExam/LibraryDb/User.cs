using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDb
{
    [Serializable]
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Index(IsUnique = true)]
        [StringLength(200)]
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        public bool IsBlackList{get; set;} = false;
        [Required]
        public DateTime RegistrationTime { get; set; }= DateTime.Now;
        public byte[] Avatar { get; set; }
    }
}

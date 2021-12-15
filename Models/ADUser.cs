using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.ADExport.Models
{
    public class ADUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string AccountName { get; set; }
        public string AccountFullName { get; set; }
        public string UserPosition { get; set; }
        public string UserCompany { get; set; }
        public string UserDept { get; set; }
        public string AccountPath { get; set; }
    }
}

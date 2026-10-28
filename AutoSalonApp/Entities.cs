using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSalonApp
{
    public class User
    {
        public int UserID { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public bool IsManager => Role == "Manager" || Role == "Admin";
        public bool IsAdmin => Role == "Admin";
    }
}

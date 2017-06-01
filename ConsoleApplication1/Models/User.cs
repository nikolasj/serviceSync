using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models
{
    class User
    {
        public Int32 UserId { get; set; }
        public String Name { get; set; }
        public String Picture { get; set; }
        public Int32 City { get; set; }
        public Boolean DriverLicense { get; set; }
        public string Car { get; set; }
        public String AspNetUserId { get; set; }
    }
}

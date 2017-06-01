using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models
{
    class SocialConnection
    {
        public Int32 SocialConnectionId { get; set; }
        public Int32 UserId1 { get; set; }
        public Int32 UserId2 { get; set; }
        public Int32 ConnectionTypeId { get; set; }
        public String ConnectionTypeName { get; set; }
    }
}

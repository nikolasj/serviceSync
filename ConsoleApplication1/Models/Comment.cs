using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models
{
    class Comment
    {
        public Int32 CommentId { get; set; }
        public String Text { get; set; }
        public Int32 UserId { get; set; }
        public Int32 ParentId { get; set; }
    }
}

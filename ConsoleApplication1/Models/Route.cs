using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models
{
    class Route
    {
        public Int32 RouteId { get; set; }
        public String Name { get; set; }
        public String EventCost { get; set; }
        public String TransportCost { get; set; }
        public String TotalCost { get; set; }
        public String Duration { get; set; }
        public String Rating { get; set; }
        internal Int32 UserId { get; set; }
        public DateTime RouteDatesTo { get; set; }
        public DateTime RouteDatesFrom { get; set; }
    }
}

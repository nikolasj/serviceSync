using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models
{
    class Opportunity
    {
        public Int32 OpportunityId { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public String Duration { get; set; }
        public String Cost { get; set; }
        public Int32 PlaceId { get; set; }
        public Double Coordinates { get; set; }
        public String Rating { get; set; }
        public Int32 ShowId { get; set; }
        public Int32 EventId { get; set; }
    }
}

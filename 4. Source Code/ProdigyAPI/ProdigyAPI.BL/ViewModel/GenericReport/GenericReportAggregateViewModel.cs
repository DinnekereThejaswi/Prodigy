using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.GenericReport
{
    public class GenericReportAggregateViewModel
    {
        public int ID { get; set; }
        public int ReportID { get; set; }
        public string Name { get; set; }
        public string Method { get; set; }
        public DateTime InsertedOn { get; set; }
        public int InsertedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}

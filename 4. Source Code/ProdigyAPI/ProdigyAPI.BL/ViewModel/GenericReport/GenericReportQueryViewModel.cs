using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.GenericReport
{
    public class GenericReportQueryViewModel
    {
        public int ID { get; set; }
        public string ReportName { get; set; }
        public string Query { get; set; }
        public bool IsActive { get; set; }
        public int InsertedBy { get; set; }
        public DateTime InsertedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.GenericReport
{
    public class GenericReportFilterViewModel
    {
        public int ID { get; set; }
        public int QueryID { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public int ControlTypeID { get; set; }
        public string Control { get; set; }
        public string ControlType { get; set; }
        public string Query { get; set; }
        public string ValueMember { get; set; }
        public string DisplayMember { get; set; }
        public string ValidationMsg { get; set; }
        public bool? SelectAllRequired { get; set; }
        public int? DefaultSelectIndex { get; set; }
        public int? DateNos { get; set; }
        public string DefaultValue { get; set; }
        public string DefaultExpression { get; set; }
        public string ReplacementForSelectAll { get; set; }
        public string ReplacementForFilters { get; set; }
        public string DataSetAPIURL { get; set; }
        public DateTime InsertedOn { get; set; }
        public int InsertedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DataTable DataSet { get; set; }
    }
}

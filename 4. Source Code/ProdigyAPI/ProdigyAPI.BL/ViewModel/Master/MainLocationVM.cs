using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class MainLocationVM
    {
        public string ObjID { get; set; }

        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string BranchCode { get; set; }
        [Required]
        public string MainCounterCode { get; set; }
        [Required]
        public string MainCounterName { get; set; }
        public string ObjectStatus { get; set; }
    }
}

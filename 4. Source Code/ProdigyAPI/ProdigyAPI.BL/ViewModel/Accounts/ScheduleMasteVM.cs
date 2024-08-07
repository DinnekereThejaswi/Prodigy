using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Accounts
{
    public class ScheduleMasteVM
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public string Schedule_Type { get; set; }
        public string Schedule_Name { get; set; }
        public string obj_status { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}

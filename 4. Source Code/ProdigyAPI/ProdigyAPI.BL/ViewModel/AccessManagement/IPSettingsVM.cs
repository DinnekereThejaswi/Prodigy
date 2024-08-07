using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.AccessManagement
{
    public class IPSettingsVM
    {
        public int ID { get; set; }
        public string AllowDeny { get; set; }
        public string FromIP { get; set; }
        public string ToIP { get; set; }
        public string BranchCode { get; set; }
        public string Remarks { get; set; }
        public bool Active { get; set; }
    }
}

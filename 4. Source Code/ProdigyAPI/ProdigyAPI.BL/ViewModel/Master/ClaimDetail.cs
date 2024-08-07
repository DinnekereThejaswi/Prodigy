using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class ClaimDetail
    {
        public string UserID { get; set; }
        public string RoleID { get; set; }
        public string PwdStamp { get; set; }
        public string RowTimestamp { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
    }
}

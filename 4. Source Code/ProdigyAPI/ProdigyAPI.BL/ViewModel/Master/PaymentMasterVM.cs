using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
   public class PaymentMasterVM
    {
        public string PaymentCode { get; set; }
        public string PaymentName { get; set; }
        public int SeqNo { get; set; }
        public bool Active { get; set; }
    }
}

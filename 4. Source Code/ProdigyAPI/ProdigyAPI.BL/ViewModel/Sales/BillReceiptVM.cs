using ProdigyAPI.BL.ViewModel.Payment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    public class BillReceiptVM
    {
        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string BranchCode { get; set; }
        [Required]
        public int BillNo { get; set; }
        [Required]
        public List<PaymentVM> AddPayments { get; set; }
    }
}

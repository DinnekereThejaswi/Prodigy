using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Orders
{
    public class ChitAdjustVM
    {
        public decimal ChitAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal WinnerAmount { get; set; }
    }
}

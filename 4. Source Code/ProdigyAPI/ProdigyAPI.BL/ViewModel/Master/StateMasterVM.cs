using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class StateMasterVM
    {
        public int ID { get; set; }
        public string StateName { get; set; }
        public Nullable<int> TINNo { get; set; }
        public string ObjectStatus { get; set; }
    }
}

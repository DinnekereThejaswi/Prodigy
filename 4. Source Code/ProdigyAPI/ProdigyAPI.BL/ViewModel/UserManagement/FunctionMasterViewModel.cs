using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.UserManagement
{
    public class FunctionMasterViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GlobalName { get; set; }
        public bool IsActive { get; set; }
        public int InsertedBy { get; set; }
        public System.DateTime InsertedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}

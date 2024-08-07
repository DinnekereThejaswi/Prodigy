using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class DocumentCreationVM
    {
        public string DocumentNo { get; set; }
        public string Message { get; set; }
    }

    public class GeneralListVM
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class DocumentInfoVM
    {
        public int No { get; set; }
        public string Name  { get; set; }
    }
}

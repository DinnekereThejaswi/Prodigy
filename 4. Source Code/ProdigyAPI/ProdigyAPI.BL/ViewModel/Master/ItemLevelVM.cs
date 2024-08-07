using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class ItemLevelVM
    {
        public ProductTreeVM ItemLevel1 { get; set; }
        public List<ProductTreeVM> ItemLevel2 { get; set; }
    }
}

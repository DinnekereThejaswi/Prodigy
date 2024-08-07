using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Marketplace
{
    public class DocumentInfoOutputVM
    {
        public List<DocumentInformationVM> Data { get; set; }
    }

    public class DocumentInformationVM
    {
        public string Name { get; set; }
        public string DocumentNo { get; set; }
        public string FileData { get; set; }
        public string FileFormat { get; set; }
    }

    public class MarketPlaceMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<MarketPlaceMaster> InitializeMarketPlaces()
        {
            return new List<MarketPlaceMaster> {
                new MarketPlaceMaster { Id = 1, Name="AMAZON"},
                new MarketPlaceMaster { Id = 3, Name = "BHIMA"},
                new MarketPlaceMaster {Id = 5, Name = "FLIPKART" }
            };
        }

        public MarketPlaceMaster GetById(int Id)
        {
            return InitializeMarketPlaces().Where(x => x.Id == Id).FirstOrDefault();
        }
        public MarketPlaceMaster GetByName(string name)
        {
            return InitializeMarketPlaces().Where(x => x.Name == name).FirstOrDefault();
        }
        public string GetNameById(int Id)
        {
            return InitializeMarketPlaces().Where(x => x.Id == Id).Select(y => y.Name).FirstOrDefault();
        }
        public int GetIdByName(string name)
        {
            return InitializeMarketPlaces().Where(x => x.Name == name).Select(y => Id).FirstOrDefault();
        }
    }
}

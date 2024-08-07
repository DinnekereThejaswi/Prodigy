using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Marketplace
{
    public class OnlineMarketplaceFactory
    {
        public IMarketplace GetInstanceOf(string marketplaceCode, MagnaDbEntities dbContext)
        {
            IMarketplace marketplace = null;
            switch (marketplaceCode) {
                case "AMAZON":
                    marketplace = new AmazonMarketplaceBL(dbContext);
                    break;
                case "BHIMA":
                    marketplace = new BhimaECommMaretplaceBL(dbContext);
                    break;
                case "FLIPKART":
                    marketplace = new FlipkartMarketplaceBL(dbContext);
                    break;
                default:
                    break;
            }
            return marketplace;
        }
    }
}

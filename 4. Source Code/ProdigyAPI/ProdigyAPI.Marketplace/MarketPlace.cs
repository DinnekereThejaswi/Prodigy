using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace
{
    public abstract class MarketPlace
    {
        public HttpClient clients = new HttpClient();

        /// <summary>
        /// Get SKU Information.
        /// </summary>
        /// <param name="skuID"></param>
        /// <param name="clientID"></param>
        public virtual void GetSKUInfo(string skuID, string clientID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Inventory Details by Passing the required parameters
        /// </summary>
        /// <param name="skuID"></param>
        /// <param name="storeID"></param>
        /// <param name="qty"></param>
        public virtual void GetMarketplaceInvertory(string skuID, string storeID, int qty)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updating the Invernotry by passing the required parameters
        /// </summary>
        /// <param name="skuID"></param>
        /// <param name="storeID"></param>
        public virtual void UpdateMarketplaceInvertory(string skuID, string storeID, int qty)
        {
            throw new NotImplementedException();
        }
    }
}

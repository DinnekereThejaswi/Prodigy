using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Marketplace;
using ProdigyAPI.BL.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Marketplace
{
    public interface IMarketplace
    {
        bool CreateMarketplacePackage(PackingItemVM pack, out ErrorVM error);
        bool GenerateMarketplaceInvoice(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM invoiceOutputVM, out ErrorVM error);
        bool GenerateMarketplaceShiplabel(string companyCode, string branchCode, string userID, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM shiplabelOutput, out ErrorVM error);
        bool ShipMarketplaceOrder(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed order, int shipmentNo, out ErrorVM error);
        bool CancelMarketplaceOrder(string companyCode, string branchCode, int marketplaceID, int orderNo, string marketplaceOrderNo, string userId, string cancelRemarks, out ErrorVM error);
    }
}

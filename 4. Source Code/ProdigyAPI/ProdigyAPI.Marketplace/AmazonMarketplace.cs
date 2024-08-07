using Newtonsoft.Json;
using ProdigyAPI.Marketplace.Models;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 21/01/2021
/// </summary>
namespace ProdigyAPI.Marketplace
{
    /// <summary>
    /// This provides Amazon Marketplace Microservices.
    /// </summary>
    public class AmazonMarketplace : Marketplace.MarketPlace
    {
        #region Object Declaration
        AmazonConnectorAuth amazonAuth = new AmazonConnectorAuth();
        AmazonErrorLogs LogErrors = new AmazonErrorLogs();
        #endregion

        #region Variable Declaration
        string hostUrl = string.Empty;
        string buildRoute = string.Empty;
        #endregion

        public AmazonMarketplace()
        {

        }

        #region Method Implementation
        public SKUInfo GetSKUInfo(string skuID, SIGlobals.MarketPlace clientID, out ErrorCls error)
        {
            error = new ErrorCls();
            SKUInfo inv = new SKUInfo();
            try {
                string accessToken = amazonAuth.GetAccesstoken();
                hostUrl = amazonAuth.hostUrl;
                buildRoute = amazonAuth.tokenEndPoint;
                if (string.IsNullOrEmpty(accessToken)) {
                    return null;
                }
                HttpClient clients = new HttpClient();
                clients.BaseAddress = new Uri(hostUrl);
                clients.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                clients.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                string body = "";
                HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = null;
                response = clients.GetAsync("/" + buildRoute + "/api/Inventory/sku-info?skuID=" + skuID + "&clientID=" + clientID.ToString() + " ").Result;

                var values = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode) {
                    inv = JsonConvert.DeserializeObject<SKUInfo>(values);
                    return inv;
                }
                else {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {
                        error.errorInfo = new ErrorInfoCls() { description = "Inverntory Not Found." };
                        return null;
                    }
                    else {
                        error = JsonConvert.DeserializeObject<ErrorCls>(values);
                        return null;
                    }
                }
            }
            catch (Exception excp) {
                LogErrors.LogApplicationError(excp);
                return null;
            }
        }
        public MarketplaceInventories GetMarketplaceInventory(string skuID, string storeID, out ErrorCls errorcls)
        {
            errorcls = new ErrorCls();
            MarketplaceInventories inv = new MarketplaceInventories();

            string accessToken = amazonAuth.GetAccesstoken();
            string errorMsg = string.Empty;
            if (string.IsNullOrEmpty(accessToken)) {
                return null;
            }
            hostUrl = amazonAuth.hostUrl;
            buildRoute = amazonAuth.tokenEndPoint;
            try {
                HttpClient clients = new HttpClient();
                clients.BaseAddress = new Uri(hostUrl);
                clients.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                clients.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                string body = "";
                HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = null;
                response = clients.GetAsync("/" + buildRoute + "/api/Inventory/get?skuID=" + skuID + "&storeID=" + storeID + "").Result;

                var values = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode) {
                    inv = JsonConvert.DeserializeObject<MarketplaceInventories>(values);
                    if (inv == null) {
                        inv = new MarketplaceInventories
                        {
                            SellableQuantity = 0,
                            ReservedQuantity = 0
                        };
                    }
                    return inv;
                }
                else {
                    errorcls = JsonConvert.DeserializeObject<ErrorCls>(values);
                    errorMsg = errorcls.errorInfo.description;
                    return null;
                }
            }
            catch (Exception excp) {
                LogErrors.LogApplicationError(excp);
                return null;
            }
        }

        public bool UpdateMarketplaceInventory(string skuID, string storeID, int qty, out ErrorCls errorcls)
        {
            errorcls = new ErrorCls();
            string accessToken = amazonAuth.GetAccesstoken();
            LogErrors.LogInfo(string.Format("Update Inventory API invoked for skuID:{0}, store id:{1}, Qty={2}", skuID, storeID, qty));
            if (string.IsNullOrEmpty(accessToken)) {
                return false;
            }
            hostUrl = amazonAuth.hostUrl;
            buildRoute = amazonAuth.tokenEndPoint;
            try {
                HttpClient clients = new HttpClient();
                clients.BaseAddress = new Uri(hostUrl);
                clients.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                clients.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                string body = "";
                HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = null;
                response = clients.PutAsync("/" + buildRoute + "/api/Inventory/put?skuID=" + skuID + "&storeID=" + storeID + "&qty=" + qty + "&inventoryUpdateSequence=0", content).Result;

                var values = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode) {
                    return true;
                }
                else {
                    errorcls = JsonConvert.DeserializeObject<ErrorCls>(values);
                    return false;
                }
            }
            catch (Exception excp) {
                LogErrors.LogApplicationError(excp);
                return false;
            }
        }
        public string GetOrderStatus(string orderNo, out ErrorInfo error)
        {
            error = null;
            string orderStatus = string.Empty;
            try {
                string accessToken = amazonAuth.GetAccesstoken();
                if (string.IsNullOrEmpty(accessToken)) {
                    error = new ErrorInfo { Description = "Failed to get access token" };
                    return null;
                }
                hostUrl = amazonAuth.hostUrl;
                buildRoute = amazonAuth.tokenEndPoint;

                HttpClient clients = new HttpClient();
                clients.BaseAddress = new Uri(hostUrl);
                clients.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                clients.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                HttpResponseMessage response = null;
                response = clients.GetAsync("/" + buildRoute + "/api/ordering/get/" + orderNo + "").Result;
                var values = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode) {
                    OrderViewModel order = new OrderViewModel();
                    order = JsonConvert.DeserializeObject<OrderViewModel>(values);
                    orderStatus = order.OrderStatus;
                }
                else {
                    var errorcls = JsonConvert.DeserializeObject<ErrorCls>(values);
                    if (errorcls != null) {
                        if (errorcls.errorInfo != null) {
                            error = new ErrorInfo { Description = errorcls.errorInfo.description };
                        }
                    }
                }
            }
            catch (Exception ex) {
                LogErrors.LogApplicationError(ex);
                error = new ErrorInfo { Description = ex.Message };
                return string.Empty;
            }
            clients.Dispose();

            return orderStatus;
        }

        public bool CreatePackage(string marketplaceId, Packaging package, string orderId, out ErrorInfo error)
        {
            error = null;
            try {
                string accessToken = amazonAuth.GetAccesstoken();
                if (string.IsNullOrEmpty(accessToken)) {
                    error = new ErrorInfo { StatusCode = HttpStatusCode.BadRequest, Description = "Failed to get token." };
                    return false;
                }
                hostUrl = amazonAuth.hostUrl;
                buildRoute = amazonAuth.tokenEndPoint;

                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(hostUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    string body = JsonConvert.SerializeObject(package);
                    HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                    string postUrl = string.Format($"/{buildRoute}/api/ordering/create-packages/{orderId}");
                    HttpResponseMessage response = client.PutAsync(postUrl, content).Result;
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    if (!response.IsSuccessStatusCode) {
                        ErrorCls errorcls = JsonConvert.DeserializeObject<ErrorCls>(responseData);
                        error = new ErrorInfo { StatusCode = response.StatusCode, ErrorCode = errorcls.errorInfo.errorCode, Description = errorcls.errorInfo.description };
                        return false;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorInfo { Description = ex.Message };
                LogErrors.LogApplicationError(ex);
                return false;
            }
            return true;
        }

        public bool UpdatePackage(string marketplaceId, Packaging package, string orderId, out ErrorInfo error)
        {
            error = null;
            try {
                string accessToken = amazonAuth.GetAccesstoken();

                if (string.IsNullOrEmpty(accessToken)) {
                    error = new ErrorInfo { StatusCode = HttpStatusCode.BadRequest, Description = "Failed to get token." };
                    return false;
                }
                hostUrl = amazonAuth.hostUrl;
                buildRoute = amazonAuth.tokenEndPoint;

                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(hostUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    string body = JsonConvert.SerializeObject(package);
                    HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                    string postUrl = string.Format($"/{buildRoute}/api/ordering/update-packages/{orderId}");
                    HttpResponseMessage response = client.PutAsync(postUrl, content).Result;
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    if (!response.IsSuccessStatusCode) {
                        ErrorCls errorcls = JsonConvert.DeserializeObject<ErrorCls>(responseData);
                        error = new ErrorInfo { StatusCode = response.StatusCode, ErrorCode = errorcls.errorInfo.errorCode, Description = errorcls.errorInfo.description };
                        return false;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorInfo { Description = ex.Message };
                LogErrors.LogApplicationError(ex);
                return false;
            }
            return true;
        }

        public bool RetrievePickupSlots(string marketplaceId, string orderId, out ErrorInfo error)
        {
            error = null;
            try {
                string accessToken = amazonAuth.GetAccesstoken();
                if (string.IsNullOrEmpty(accessToken)) {
                    error = new ErrorInfo { StatusCode = HttpStatusCode.BadRequest, Description = "Failed to get token." };
                    return false;
                }
                hostUrl = amazonAuth.hostUrl;
                buildRoute = amazonAuth.tokenEndPoint;
                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(hostUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    string body = JsonConvert.SerializeObject(string.Empty);
                    HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                    string postUrl = string.Format($"/{buildRoute}/api/ordering/retrieve-pickup-slots/{orderId}");
                    HttpResponseMessage response = client.PutAsync(postUrl, content).Result;
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    if (response.StatusCode != HttpStatusCode.OK) {
                        ErrorCls errorcls = JsonConvert.DeserializeObject<ErrorCls>(responseData);
                        error = new ErrorInfo { StatusCode = response.StatusCode, ErrorCode = errorcls.errorInfo.errorCode, Description = errorcls.errorInfo.description };
                        return false;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorInfo { Description = ex.Message };
                LogErrors.LogApplicationError(ex);
                return false;
            }
            return true;
        }

        public bool GenerateInvoice(string marketplaceId, string orderId, out Invoice invoice, out ErrorInfo error)
        {
            error = null;
            invoice = null;
            try {
                string accessToken = amazonAuth.GetAccesstoken();

                if (string.IsNullOrEmpty(accessToken)) {
                    error = new ErrorInfo { StatusCode = HttpStatusCode.BadRequest, Description = "Failed to get token." };
                    return false;
                }
                hostUrl = amazonAuth.hostUrl;
                buildRoute = amazonAuth.tokenEndPoint;

                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(hostUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    string body = JsonConvert.SerializeObject(string.Empty);
                    HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                    string postUrl = string.Format($"/{buildRoute}/api/ordering/generate-invoice/{orderId}");
                    HttpResponseMessage response = client.PutAsync(postUrl, content).Result;
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode) {
                        invoice = JsonConvert.DeserializeObject<Invoice>(responseData);
                    }
                    else {
                        ErrorCls errorcls = JsonConvert.DeserializeObject<ErrorCls>(responseData);
                        error = new ErrorInfo { StatusCode = response.StatusCode, ErrorCode = errorcls.errorInfo.errorCode, Description = errorcls.errorInfo.description };
                        return false;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorInfo { Description = ex.Message };
                LogErrors.LogApplicationError(ex);
                return false;
            }
            return true;
        }

        public bool RetrieveInvoice(string marketplaceId, string orderId, out Invoice invoice, out ErrorInfo error)
        {
            error = null;
            invoice = null;
            try {
                string accessToken = amazonAuth.GetAccesstoken();

                if (string.IsNullOrEmpty(accessToken)) {
                    error = new ErrorInfo { StatusCode = HttpStatusCode.BadRequest, Description = "Failed to get token." };
                    return false;
                }
                hostUrl = amazonAuth.hostUrl;
                buildRoute = amazonAuth.tokenEndPoint;
                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(hostUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    string postUrl = string.Format($"/{buildRoute}/api/ordering/retrieve-invoice/{orderId}");
                    HttpResponseMessage response = client.GetAsync(postUrl).Result;
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode) {
                        invoice = JsonConvert.DeserializeObject<Invoice>(responseData);
                    }
                    else {
                        ErrorCls errorcls = JsonConvert.DeserializeObject<ErrorCls>(responseData);
                        error = new ErrorInfo { StatusCode = response.StatusCode, ErrorCode = errorcls.errorInfo.errorCode, Description = errorcls.errorInfo.description };
                        return false;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorInfo { Description = ex.Message };
                LogErrors.LogApplicationError(ex);
                return false;
            }
            return true;
        }

        public bool GenerateShipLabel(string marketplaceId, string orderId, string packageId, out ShipLabel shipLabel, out ErrorInfo error)
        {
            error = null;
            shipLabel = null;
            try {
                string accessToken = amazonAuth.GetAccesstoken();

                if (string.IsNullOrEmpty(accessToken)) {
                    error = new ErrorInfo { StatusCode = HttpStatusCode.BadRequest, Description = "Failed to get token." };
                    return false;
                }
                hostUrl = amazonAuth.hostUrl;
                buildRoute = amazonAuth.tokenEndPoint;

                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(hostUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    string body = JsonConvert.SerializeObject(string.Empty);
                    HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                    string postUrl = string.Format($"/{buildRoute}/api/ordering/generate-ship-label/{orderId}?packageID={packageId}&pickupTimeSlotID=");
                    HttpResponseMessage response = client.PutAsync(postUrl, content).Result;
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode) {
                        shipLabel = JsonConvert.DeserializeObject<ShipLabel>(responseData);
                    }
                    else {
                        ErrorCls errorcls = JsonConvert.DeserializeObject<ErrorCls>(responseData);
                        error = new ErrorInfo { StatusCode = response.StatusCode, ErrorCode = errorcls.errorInfo.errorCode, Description = errorcls.errorInfo.description };
                        return false;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorInfo { Description = ex.Message };
                LogErrors.LogApplicationError(ex);
                return false;
            }
            return true;
        }

        public bool RetrieveShipLabel(string marketplaceId, string orderId, string packageId, out ShipLabel shipLabel, out ErrorInfo error)
        {
            error = null;
            shipLabel = null;
            try {
                string accessToken = amazonAuth.GetAccesstoken();

                if (string.IsNullOrEmpty(accessToken)) {
                    error = new ErrorInfo { StatusCode = HttpStatusCode.BadRequest, Description = "Failed to get token." };
                    return false;
                }
                hostUrl = amazonAuth.hostUrl;
                buildRoute = amazonAuth.tokenEndPoint;

                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(hostUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    string postUrl = string.Format($"/{buildRoute}/api/ordering/retrieve-ship-label/{orderId}?packageID={packageId}&pickupTimeSlotID=");
                    HttpResponseMessage response = client.GetAsync(postUrl).Result;
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode) {
                        shipLabel = JsonConvert.DeserializeObject<ShipLabel>(responseData);
                    }
                    else {
                        ErrorCls errorcls = JsonConvert.DeserializeObject<ErrorCls>(responseData);
                        error = new ErrorInfo { StatusCode = response.StatusCode, ErrorCode = errorcls.errorInfo.errorCode, Description = errorcls.errorInfo.description };
                        return false;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorInfo { Description = ex.Message };
                LogErrors.LogApplicationError(ex);
                return false;
            }
            return true;
        }

        public bool Ship(string marketplaceId, string orderId, out ErrorInfo error)
        {
            error = null;
            try {
                string accessToken = amazonAuth.GetAccesstoken();
                
                if (string.IsNullOrEmpty(accessToken)) {
                    error = new ErrorInfo { StatusCode = HttpStatusCode.BadRequest, Description = "Failed to get token." };
                    return false;
                }
                hostUrl = amazonAuth.hostUrl;
                buildRoute = amazonAuth.tokenEndPoint;
                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(hostUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    string body = JsonConvert.SerializeObject(string.Empty);
                    HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                    string postUrl = string.Format($"/{buildRoute}/api/ordering/ship/{orderId}");
                    HttpResponseMessage response = client.PutAsync(postUrl, content).Result;
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode) {
                        ;
                    }
                    else {
                        ErrorCls errorcls = JsonConvert.DeserializeObject<ErrorCls>(responseData);
                        error = new ErrorInfo { StatusCode = response.StatusCode, ErrorCode = errorcls.errorInfo.errorCode, Description = errorcls.errorInfo.description };
                        return false;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorInfo { Description = ex.Message };
                LogErrors.LogApplicationError(ex);
                return false;
            }
            return true;
        }

        public bool RejectOrder(string marketplaceId, string orderId, out ErrorInfo error)
        {
            error = null;
            try {
                string accessToken = amazonAuth.GetAccesstoken();

                if (string.IsNullOrEmpty(accessToken)) {
                    error = new ErrorInfo { StatusCode = HttpStatusCode.BadRequest, Description = "Failed to get token." };
                    return false;
                }
                hostUrl = amazonAuth.hostUrl;
                buildRoute = amazonAuth.tokenEndPoint;

                #region Reject order input lines
                #region Complicated Assignment
                //RejectedLineItem rejectedLineItem = new RejectedLineItem
                //{
                //    lineItem = new OrderLineItem { id = "0" },
                //    reason = "OUT_OF_STOCK",
                //    quantity = 1
                //};
                //List<RejectedLineItem> rejectOrderList = new List<RejectedLineItem>();
                //rejectOrderList.Add(rejectedLineItem);
                //RejectOrderInput rejectOrderInput = new RejectOrderInput
                //{
                //    referenceId = orderId,
                //    rejectedLineItems = rejectOrderList
                //}; 
                #endregion

                RejectOrderInput rejectOrderInput = new RejectOrderInput
                {
                    referenceId = orderId,
                    rejectedLineItems = new List<RejectedLineItem> {
                        new RejectedLineItem {
                            lineItem = new OrderLineItem { id = "0" },
                            quantity = 1,
                            reason = "OUT_OF_STOCK"
                        }
                    }
                };
                #endregion
                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(hostUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    string body = JsonConvert.SerializeObject(rejectOrderInput);
                    HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                    string postUrl = string.Format($"/{buildRoute}/api/ordering/reject/{orderId}");
                    HttpResponseMessage response = client.PutAsync(postUrl, content).Result;
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode) {
                        ;
                    }
                    else {
                        ErrorCls errorcls = JsonConvert.DeserializeObject<ErrorCls>(responseData);
                        error = new ErrorInfo { StatusCode = response.StatusCode, ErrorCode = errorcls.errorInfo.errorCode, Description = errorcls.errorInfo.description };
                        return false;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorInfo { Description = ex.Message };
                LogErrors.LogApplicationError(ex);
                return false;
            }
            return true;
        }
        #endregion
    }
}

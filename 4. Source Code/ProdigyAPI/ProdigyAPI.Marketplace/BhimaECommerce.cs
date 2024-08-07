using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProdigyAPI.Marketplace.Models;
using ProdigyAPI.Marketplace.Models.BJEComm;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ProdigyAPI.ShipmentIntegration.com.bluedart.netconnect.waybill;
using ProdigyAPI.ShipmentIntegration.com.bluedart.netconnect.pickup;

namespace ProdigyAPI.Marketplace
{
    /// <summary>
    /// Provides Service related to Bhima E-Commerce Microservice.
    /// </summary>
    public class BhimaECommerce : Marketplace.MarketPlace
    {

        #region Methods
        /// <summary>
        /// Update Bhima e-Commerce Inverntory Update, Blocking and Unblocking.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="barcodeNo"></param>
        /// <param name="qty"></param>
        /// <param name="isInStock"></param>
        /// <param name="transType"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool UpdateBhimaEComMarketplaceInventory(string companyCode, string branchCode, string barcodeNo, int qty, int isInStock, string transType, out ErrorCls error)
        {
            error = new ErrorCls();
            
            try {
                string url = GetUrl(transType);
                string accessToken = new BhimaECommerceAuth().GetAccessToken();
                if (accessToken == "") {
                    error.errorInfo.description = "Token Expired, Unable to generate new Token, Pleae check the log.";
                    return false;
                }
                InventoryVM eCom = new InventoryVM();
                eCom.barcode = barcodeNo;
                eCom.qty = qty;
                eCom.is_in_stock = isInStock;
                eCom.branchcode = branchCode;
                clients.DefaultRequestHeaders.Add("authorization", "Bearer " + accessToken);
                string body = JsonConvert.SerializeObject(eCom);
                HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = response = clients.PostAsync(url, content).Result;
                var values = response.Content.ReadAsStringAsync().Result;

                JObject joResponse = JObject.Parse(values.ToString());
                string message = string.Empty, success = string.Empty, err = string.Empty;
                if ((int)response.StatusCode == 200) {
                    success = joResponse["success"].ToString();
                    if (Convert.ToBoolean(success) == false) {
                        err = joResponse["err"].ToString();
                    }
                    else {
                        message = joResponse["message"].ToString();
                    }
                }
                else {
                    message = joResponse["message"].ToString();
                    LogEcommInfo(companyCode, branchCode, "0", "0", "false", message, err, transType);
                }
            }
            catch (Exception ex) {
                LogEcommInfo(companyCode, branchCode, "0", "0", "false", ex.Message, "", transType);
                return false;
            }
            return true;
        }

        public bool PostOrderStatus(string companyCode, string branchCode, OrderVM order, string transType, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool isSucceeded = true;
            try {
                string url = GetUrl(transType: transType);
                string accessToken = new BhimaECommerceAuth().GetAccessToken();
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("authorization", "Bearer " + accessToken);
                string body = JsonConvert.SerializeObject(order);

                HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");

                HttpResponseMessage response = response = client.PostAsync(url, content).Result;
                var values = response.Content.ReadAsStringAsync().Result;

                JObject joResponse = JObject.Parse(values.ToString());
                string message = string.Empty, success = string.Empty, err = string.Empty;
                if (response.IsSuccessStatusCode) {
                    message = joResponse["message"].ToString();
                    isSucceeded = true;
                }
                else {
                    err = errorMessage = joResponse["err"].ToString();
                    isSucceeded = false;
                }
                LogEcommInfo(companyCode, branchCode, order.branchorderno, order.orderreferanceno, isSucceeded.ToString(), message, err, transType);
                return isSucceeded;
            }
            catch (Exception ex) {
                errorMessage = ex.Message;
                LogEcommInfo(companyCode, branchCode, order.branchorderno, order.orderreferanceno, "false", ex.Message, "", transType);
                isSucceeded = false;
            }
            return isSucceeded;
        }

        public bool GenerateEcomInvoice(string companyCode, string branchCode, string branchOrderNo, string marketplaceOrderRefNo, int billNo, string filePath, out string errorMessage)
        {
            errorMessage = string.Empty;
            string transType = "Invoice";
            bool apiInvokeSuccessfull = true;
            try {
                string url = GetUrl(transType: transType);
                //string subject = string.Format("{0}-{1}-{2}", "Sales Invoice", branchCode, billNo);
                //string filePath = string.Format(Application.StartupPath + @"\InvocePDF\{0}{1}", subject, ".pdf"); //FirstName[0] + " " + Subject
                
                string accessToken = new BhimaECommerceAuth().GetAccessToken();
                MultipartFormDataContent form = new MultipartFormDataContent();
                HttpContent content = new StringContent("fileToUpload");
                form.Add(content, "fileToUpload");
                File.SetAttributes(filePath, FileAttributes.Normal);
                var stream = new FileStream(filePath, FileMode.Open);
                content = new StreamContent(stream);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                form.Add(content, "file", Path.GetFileName(filePath));
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("authorization", "Bearer " + accessToken);

                string postUrl = string.Format($"{url}/{branchCode}/{marketplaceOrderRefNo}/{branchOrderNo}");
                HttpResponseMessage response = client.PostAsync(postUrl, form).Result;

                #region File Delete
                //File.SetAttributes(filePath, FileAttributes.Normal);
                //FileInfo file = new FileInfo(filePath.ToString());
                //file.Delete(); 
                #endregion

                var values = response.Content.ReadAsStringAsync().Result;
                JObject joResponse = JObject.Parse(values.ToString());
                string message = string.Empty, success = string.Empty, err = string.Empty;
                if ((int)response.StatusCode == 200) {
                    success = joResponse["success"].ToString();
                    if (Convert.ToBoolean(success) == false) {
                        errorMessage = err = joResponse["err"].ToString();
                        apiInvokeSuccessfull = false;
                    }
                    else {
                        message = joResponse["message"].ToString();
                        apiInvokeSuccessfull = true;
                    }
                }
                else {
                    errorMessage = message = joResponse["message"].ToString();
                    apiInvokeSuccessfull = false;
                }
                LogEcommInfo(companyCode, branchCode, branchOrderNo, marketplaceOrderRefNo, success, message, err, transType);

            }
            catch (Exception ex) {
                apiInvokeSuccessfull = false;
                errorMessage = ex.Message;
                LogEcommInfo(companyCode, branchCode, branchOrderNo, marketplaceOrderRefNo, "false", ex.Message, "", transType);
                return false;
            }
            return apiInvokeSuccessfull;
        }

        private void LogEcommInfo(string companyCode, string branchCode, string branchOrderNo, string orderRefNo, string status, string message, string error, string transType)
        {
            EcomOrderAPILog apiLog = new EcomOrderAPILog
            {
                company_code = companyCode,
                branch_code = branchCode,
                BranchOrderNo = branchOrderNo,
                CentralRefNo = orderRefNo,
                Error = error,
                Message = message,
                Status = status,
                RequestType = transType
            };
            new BhimaEComErrorLogs().LogInfo(apiLog);
        }

        private string GetUrl(string transType)
        {
            EcomAPIURL ecommUrl = new BhimaECommerceAuth().GetEcomAPIURL(transType);
            if (ecommUrl == null) {
                throw new Exception(string.Format($"No url found for the API call {transType}."));
            }

            return ecommUrl.Url;
        }
        #endregion

        #region e-way bill
        public bool GetWaybillGenerationResponse(MagnaDbEntities db, string companyCode, string branchCode, string orderNo, 
            out string statusCode, out string statusInfo, out string aWBNo, out string aWBPrintContent, out string isPickUpRegistered, out string pickUpTokenNo, out bool isError)
        {

            statusCode = string.Empty;
            statusInfo = string.Empty;
            aWBNo = string.Empty;
            aWBPrintContent = string.Empty;
            isError = true;
            isPickUpRegistered = string.Empty;
            pickUpTokenNo = string.Empty;
            bool IsErrorInPU = false;

            //// var _url = "http://netconnect.bluedart.com/Ver1.9/Demo/ShippingAPI/WayBill/WayBillGeneration.svc?wsdl"; 
            //var _url = "http://netconnect.bluedart.com/Ver1.9/demo/ShippingAPI/WayBill/WayBillGeneration.svc/Basic";
            //var _action = "http://tempuri.org/IWayBillGeneration/GenerateWayBill";  //for generate
            var _url = string.Empty;
            var _action = string.Empty;
            var awbUrls = db.APP_AUTH_TABLE.Where(x => x.UserID == "AWBurl").FirstOrDefault();
            if (awbUrls != null)
                _url = awbUrls.WSURL;
            awbUrls = db.APP_AUTH_TABLE.Where(x => x.UserID == "AWBaction").FirstOrDefault();
            if (awbUrls != null)
                _action = awbUrls.WSURL;


            XmlDocument soapEnvelopeXml = GenerateEwayBillRequestXML(companyCode, branchCode, orderNo);
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            webRequest.KeepAlive = false;
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            string[] soapResponse = new string[500];

            try {
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult)) {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream())) {
                        soapResult = rd.ReadToEnd();
                    }

                    XmlDocument EWBGenerationReq = new XmlDocument();
                    EWBGenerationReq.LoadXml(soapResult);

                    statusCode = EWBGenerationReq.GetElementsByTagName("a:StatusCode")[0].InnerText;
                    statusInfo = EWBGenerationReq.GetElementsByTagName("a:StatusInformation")[0].InnerText;
                    isError = Convert.ToBoolean(EWBGenerationReq.GetElementsByTagName("a:IsError")[0].InnerText);
                    IsErrorInPU = Convert.ToBoolean(EWBGenerationReq.GetElementsByTagName("a:IsErrorInPU")[0].InnerText);

                    if (!isError) {
                        aWBNo = EWBGenerationReq.GetElementsByTagName("a:AWBNo")[0].InnerText;
                        aWBPrintContent = EWBGenerationReq.GetElementsByTagName("a:AWBPrintContent")[0].InnerText;
                        if (!IsErrorInPU) {
                            isPickUpRegistered = "Y";
                            pickUpTokenNo = EWBGenerationReq.GetElementsByTagName("a:TokenNumber")[0].InnerText;
                        }
                    }
                }
            }
            catch (WebException Ex) {
                WebExceptionStatus status = Ex.Status;
                WebResponse responseEx = Ex.Response;
                statusCode = responseEx.ToString();
                statusInfo = status.ToString();
                isError = true;
                return false;
            }
            catch(Exception ex) {
                statusCode = statusInfo = ex.Message;
                isError = true;
                return false;
            }

            return true;

        }

        private HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;

        }

        private void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream()) {
                soapEnvelopeXml.Save(stream);
            }
        }
        private XmlDocument GenerateEwayBillRequestXML(string companyCode, string branchCode, string OrderNo)
        {

            string GetShipOrderDeatils = string.Format("EXEC usp_GetOrderDetailsforShipping '{0}', '{1}','{2}'", OrderNo, companyCode, branchCode);
            DataTable dtGetShipOrderDeatils = SIGlobals.Globals.GetDataTable(GetShipOrderDeatils);


            string[] ConsigneDetails = new string[7];
            ConsigneDetails = dtGetShipOrderDeatils.Rows[0]["Consigne_details"].ToString().Split('|');

            string ConName = ConsigneDetails[0];
            string ConAdd1 = ConsigneDetails[1];
            string ConAdd2 = ConsigneDetails[2];
            string ConAdd3 = ConsigneDetails[3];
            string ConAtten = ConsigneDetails[0];
            string ConCity = ConsigneDetails[2];
            string Conemail = ConsigneDetails[6];
            string conmobile = ConsigneDetails[5];
            string ConPin = ConsigneDetails[4];

            Consignee consignee = new Consignee
            {
                ConsigneeName = ConName,
                ConsigneeAddress1 = ConAdd1,
                ConsigneeAddress2 = ConAdd2,
                ConsigneeAddress3 = ConAdd3,
                ConsigneeAttention = ConAtten,
                ConsigneeCityName = ConCity,
                ConsigneeEmailID = Conemail,
                ConsigneeMobile = conmobile,
                ConsigneePincode = ConPin
            };

            string RetAdd1 = dtGetShipOrderDeatils.Rows[0]["shipper_address1"].ToString();
            string RetAdd2 = dtGetShipOrderDeatils.Rows[0]["shipper_address2"].ToString();
            string RetAdd3 = dtGetShipOrderDeatils.Rows[0]["shipper_address3"].ToString();
            string RetEmail = dtGetShipOrderDeatils.Rows[0]["shipper_email_id"].ToString();
            string RetPin = dtGetShipOrderDeatils.Rows[0]["shipper_pin_code"].ToString();
            string RetMob = dtGetShipOrderDeatils.Rows[0]["shipper_mobile_no"].ToString();
            ReturnAddress returnAddress = new ReturnAddress
            {
                ReturnAddress1 = RetAdd1,
                ReturnAddress2 = RetAdd2,
                ReturnAddress3 = RetAdd3,
                ReturnEmailID = RetEmail,
                ReturnPincode = RetPin,
                ReturnMobile = RetMob
            };

            string shipAdd1 = dtGetShipOrderDeatils.Rows[0]["shipper_address1"].ToString();
            string shipAdd2 = dtGetShipOrderDeatils.Rows[0]["shipper_address2"].ToString();
            string shipAdd3 = dtGetShipOrderDeatils.Rows[0]["shipper_address3"].ToString();
            string shipCode = dtGetShipOrderDeatils.Rows[0]["shipper_code"].ToString();
            string shipName = dtGetShipOrderDeatils.Rows[0]["shipper_name"].ToString();
            string shipTele = dtGetShipOrderDeatils.Rows[0]["shipper_mobile_no"].ToString();
            string shipPin = dtGetShipOrderDeatils.Rows[0]["shipper_pin_code"].ToString();
            string VendCode = dtGetShipOrderDeatils.Rows[0]["VendorCode"].ToString();
            string Origin = dtGetShipOrderDeatils.Rows[0]["Origin"].ToString();
            string Sender = dtGetShipOrderDeatils.Rows[0]["Sender"].ToString();
            string isPayCust = dtGetShipOrderDeatils.Rows[0]["isPayCust"].ToString();  //bool

            Shipper shipper = new Shipper
            {
                CustomerAddress1 = shipAdd1,
                CustomerAddress2 = shipAdd2,
                CustomerAddress3 = shipAdd3,
                CustomerCode = shipCode,
                CustomerName = shipName,
                CustomerPincode = shipPin,
                CustomerTelephone = shipTele,
                IsToPayCustomer = Convert.ToBoolean(isPayCust),
                Sender = Sender,
                VendorCode = VendCode
            };

            double ActualWt = Convert.ToDouble(dtGetShipOrderDeatils.Rows[0]["ActualWeight"].ToString());
            double CollAmt = Convert.ToDouble(dtGetShipOrderDeatils.Rows[0]["CollectableAmount"].ToString());
            string com1 = dtGetShipOrderDeatils.Rows[0]["CommodityDetail1"].ToString();
            string com2 = dtGetShipOrderDeatils.Rows[0]["CommodityDetail2"].ToString();
            string com3 = dtGetShipOrderDeatils.Rows[0]["CommodityDetail2"].ToString();

            

            string creditRef1 = dtGetShipOrderDeatils.Rows[0]["CreditReferenceNo"].ToString();
            string creditRef2 = dtGetShipOrderDeatils.Rows[0]["CreditReferenceNo2"].ToString();
            string creditRef3 = dtGetShipOrderDeatils.Rows[0]["CreditReferenceNo3"].ToString();

            double len = Convert.ToDouble(dtGetShipOrderDeatils.Rows[0]["Length"].ToString()); ;
            double Bdth = Convert.ToDouble(dtGetShipOrderDeatils.Rows[0]["Breadth"].ToString()); ;
            double height = Convert.ToDouble(dtGetShipOrderDeatils.Rows[0]["Height"].ToString()); ;
            int count = Convert.ToInt32(dtGetShipOrderDeatils.Rows[0]["Count"].ToString()); ;

            string apiType = dtGetShipOrderDeatils.Rows[0]["apiType"].ToString();
            string LogArea = dtGetShipOrderDeatils.Rows[0]["Area"].ToString();
            string LogCode = dtGetShipOrderDeatils.Rows[0]["Customercode"].ToString();
            string LogLicenceKey = dtGetShipOrderDeatils.Rows[0]["LicenceKey"].ToString();
            string LoginID = dtGetShipOrderDeatils.Rows[0]["LoginID"].ToString();
            string version = dtGetShipOrderDeatils.Rows[0]["Version"].ToString();
            string password = dtGetShipOrderDeatils.Rows[0]["Password"].ToString();

            string IsForcePickup = dtGetShipOrderDeatils.Rows[0]["IsForcePickup"].ToString();    //bool
            string IsReversePickup = dtGetShipOrderDeatils.Rows[0]["IsReversePickup"].ToString();  //bool
            string PDFOutputNotRequired = dtGetShipOrderDeatils.Rows[0]["PDFOutputNotRequired"].ToString();  //bool
            string RegisterPickup = dtGetShipOrderDeatils.Rows[0]["RegisterPickup"].ToString();  //bool
            string OTPBasedDelivary = dtGetShipOrderDeatils.Rows[0]["OTPBasedDelivery"].ToString();  //bool


            string ProductCode = dtGetShipOrderDeatils.Rows[0]["ProductCode"].ToString();
            string SpecialInstruction = dtGetShipOrderDeatils.Rows[0]["SpecialInstruction"].ToString();
            string SubProductCode = dtGetShipOrderDeatils.Rows[0]["SubProductCode"].ToString();
            string productType = dtGetShipOrderDeatils.Rows[0]["ProductType"].ToString();
            int ProdutCount = Convert.ToInt32(dtGetShipOrderDeatils.Rows[0]["ProductCount"].ToString());
            // DateTime Pickupdate = CGlobals.GetGlobalDateTime();
            string Pickupdate = dtGetShipOrderDeatils.Rows[0]["PickupDate"].ToString();
            string Pickuptime = dtGetShipOrderDeatils.Rows[0]["PickupTime"].ToString();
            int PieceCount = Convert.ToInt32(dtGetShipOrderDeatils.Rows[0]["PieceCount"].ToString());
            double DelaredValue = Convert.ToDouble(dtGetShipOrderDeatils.Rows[0]["DelaredValue"].ToString());

            WayBillGenerationRequest wayBillGenerationRequest = new WayBillGenerationRequest
            {
                Consignee = new Consignee
                {
                    ConsigneeName = ConName,
                    ConsigneeAddress1 = ConAdd1,
                    ConsigneeAddress2 = ConAdd2,
                    ConsigneeAddress3 = ConAdd3,
                    ConsigneeAttention = ConAtten,
                    ConsigneeCityName = ConCity,
                    ConsigneeEmailID = Conemail,
                    ConsigneeMobile = conmobile,
                    ConsigneePincode = ConPin
                },
                Returnadds = new ReturnAddress
                {
                    ReturnAddress1 = RetAdd1,
                    ReturnAddress2 = RetAdd2,
                    ReturnAddress3 = RetAdd3,
                    ReturnEmailID = RetEmail,
                    ReturnPincode = RetPin,
                    ReturnMobile = RetMob
                },
                Shipper = new Shipper
                {
                    CustomerAddress1 = shipAdd1,
                    CustomerAddress2 = shipAdd2,
                    CustomerAddress3 = shipAdd3,
                    CustomerCode = shipCode,
                    CustomerName = shipName,
                    CustomerPincode = shipPin,
                    CustomerTelephone = shipTele,
                    IsToPayCustomer = Convert.ToBoolean(isPayCust),
                    Sender = Sender,
                    VendorCode = VendCode
                },
                Services = new Services
                {
                    ActualWeight = ActualWt,
                    CollectableAmount = CollAmt,
                    CreditReferenceNo = creditRef1,
                    CreditReferenceNo2 = creditRef2,
                    CreditReferenceNo3 = creditRef3,
                    DeclaredValue = DelaredValue,
                    Dimensions = new Dimension[] { new Dimension { Length = len, Breadth = Bdth, Height = height, Count = count } },
                    IsForcePickup = Convert.ToBoolean(IsForcePickup),
                    IsReversePickup = Convert.ToBoolean(IsReversePickup),
                    PDFOutputNotRequired = Convert.ToBoolean(PDFOutputNotRequired),
                    PickupDate = Convert.ToDateTime(Pickupdate),
                    PickupTime = Pickuptime,
                    PieceCount = PieceCount,
                    ProductCode = ProductCode,
                    ProductType = ProductType.Dutiables,
                    RegisterPickup = Convert.ToBoolean(RegisterPickup),
                    OTPBasedDelivery = OTPBasedDelivery.Default,
                    SpecialInstruction = SpecialInstruction,
                    SubProductCode = SubProductCode,
                    Commodity = new CommodityDetail { CommodityDetail1 = com1, CommodityDetail2 = com2, CommodityDetail3 = com3 }
                }
            };

            ShipmentIntegration.com.bluedart.netconnect.waybill.UserProfile userProfile = new ShipmentIntegration.com.bluedart.netconnect.waybill.UserProfile
            {
                Api_type = apiType,
                Area = LogArea,
                Customercode = LogCode,
                LicenceKey = LogLicenceKey,
                LoginID = LoginID,
                Version = version,
                IsAdmin = "",
                Password = ""
            };

            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(
            @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:sapi=""http://schemas.datacontract.org/2004/07/SAPI.Entities.WayBillGeneration""
            xmlns:sapi1=""http://schemas.datacontract.org/2004/07/SAPI.Entities.Admin"">
            <soapenv:Header/>
            <soapenv:Body>
            <tem:GenerateWayBill>     
            <tem:Request>  
            <sapi:Consignee>
              
                 <sapi:ConsigneeAddress1>" + ConAdd1 + @"</sapi:ConsigneeAddress1>
               
                 <sapi:ConsigneeAddress2>" + ConAdd2 + @"</sapi:ConsigneeAddress2>
             
                 <sapi:ConsigneeAddress3>" + ConAdd3 + @"</sapi:ConsigneeAddress3>
               
                 <sapi:ConsigneeAttention>" + ConAtten + @"</sapi:ConsigneeAttention>
               
                 <sapi:ConsigneeCityName>" + ConCity + @"</sapi:ConsigneeCityName>
               
                 <sapi:ConsigneeEmailID>" + Conemail + @"</sapi:ConsigneeEmailID>
               
                 <sapi:ConsigneeMobile>" + conmobile + @"</sapi:ConsigneeMobile>
               
                <sapi:ConsigneeName>" + ConName + @"</sapi:ConsigneeName>
               
                <sapi:ConsigneePincode>" + ConPin + @"</sapi:ConsigneePincode>
               
             </sapi:Consignee>
            
            <sapi:Returnadds>
               
                 <sapi:ReturnAddress1>" + RetAdd1 + @"</sapi:ReturnAddress1>
               
                <sapi:ReturnAddress2>" + RetAdd2 + @"</sapi:ReturnAddress2>
               
                <sapi:ReturnAddress3>" + RetAdd3 + @"</sapi:ReturnAddress3>
                     
                <sapi:ReturnEmailID>" + RetEmail + @"</sapi:ReturnEmailID>
               
                 <sapi:ReturnMobile>" + RetMob + @"</sapi:ReturnMobile>
               
                <sapi:ReturnPincode>" + RetPin + @"</sapi:ReturnPincode>
               
             </sapi:Returnadds>
            
            <sapi:Services>
               
                    
                 <sapi:ActualWeight>" + ActualWt + @"</sapi:ActualWeight>
               
                 <sapi:CollectableAmount>" + CollAmt + @"</sapi:CollectableAmount>
               
               <sapi:Commodity>
                  
                  <sapi:CommodityDetail1>" + com1 + @"</sapi:CommodityDetail1>
                  
                  <sapi:CommodityDetail2>" + com2 + @"</sapi:CommodityDetail2>
                  
                  <sapi:CommodityDetail3>" + com3 + @"</sapi:CommodityDetail3>
               </sapi:Commodity>
               
               <sapi:CreditReferenceNo>" + creditRef1 + @"</sapi:CreditReferenceNo>
               
               <sapi:CreditReferenceNo2>" + creditRef2 + @"</sapi:CreditReferenceNo2>
               
               <sapi:CreditReferenceNo3>" + creditRef3 + @"</sapi:CreditReferenceNo3>
               
               <sapi:DeclaredValue>" + DelaredValue + @"</sapi:DeclaredValue>
               
                <sapi:Dimensions>
               
                  <sapi:Dimension>
                     
                     <sapi:Breadth>" + Bdth + @"</sapi:Breadth>
                     
                     <sapi:Count>" + count + @"</sapi:Count>
                     
                     <sapi:Height>" + height + @"</sapi:Height>
                     
                     <sapi:Length>" + len + @"</sapi:Length>

                  </sapi:Dimension>

               </sapi:Dimensions>
                    
               <sapi:IsForcePickup>" + IsForcePickup + @"</sapi:IsForcePickup>
               
               <sapi:IsReversePickup>" + IsReversePickup + @"</sapi:IsReversePickup>
               
                <sapi:PDFOutputNotRequired>" + PDFOutputNotRequired + @"</sapi:PDFOutputNotRequired>
               
                <sapi:PickupDate>" + Pickupdate + @"</sapi:PickupDate>
               
               <sapi:PickupTime>" + Pickuptime + @"</sapi:PickupTime>
               
               <sapi:PieceCount>" + PieceCount + @"</sapi:PieceCount>
               
               <sapi:ProductCode>" + ProductCode + @"</sapi:ProductCode>
               
               <sapi:ProductType>" + productType + @"</sapi:ProductType>
               
               <sapi:RegisterPickup>" + RegisterPickup + @"</sapi:RegisterPickup>

               <sapi:OTPBasedDelivery>" + OTPBasedDelivary + @"</sapi:OTPBasedDelivery>

               <sapi:SpecialInstruction>" + SpecialInstruction + @"</sapi:SpecialInstruction>
               
               <sapi:SubProductCode>" + SubProductCode + @"</sapi:SubProductCode>
               
             </sapi:Services>
            
             <sapi:Shipper>
               
               <sapi:CustomerAddress1>" + shipAdd1 + @"</sapi:CustomerAddress1>
               
               <sapi:CustomerAddress2>" + shipAdd2 + @"</sapi:CustomerAddress2>
               
               <sapi:CustomerAddress3>" + shipAdd3 + @"</sapi:CustomerAddress3>
               
               <sapi:CustomerCode>" + shipCode + @"</sapi:CustomerCode>
               
               <sapi:CustomerName>" + shipName + @"</sapi:CustomerName>
               
               <sapi:CustomerPincode>" + shipPin + @"</sapi:CustomerPincode>
               
               <sapi:CustomerTelephone>" + shipTele + @"</sapi:CustomerTelephone>
               
               <sapi:IsToPayCustomer>" + isPayCust + @"</sapi:IsToPayCustomer>
               
               <sapi:OriginArea>" + Origin + @"</sapi:OriginArea>
               
               <sapi:Sender>" + Sender + @"</sapi:Sender>
               
               <sapi:VendorCode>" + VendCode + @"</sapi:VendorCode>
            </sapi:Shipper>
         </tem:Request>
         
         <tem:Profile>
            
            <sapi1:Api_type>" + apiType + @"</sapi1:Api_type>
            
            <sapi1:Area>" + LogArea + @"</sapi1:Area>
            
            <sapi1:Customercode>" + LogCode + @"</sapi1:Customercode>
            
            <sapi1:IsAdmin></sapi1:IsAdmin>
            
            <sapi1:LicenceKey>" + LogLicenceKey + @"</sapi1:LicenceKey>
            
            <sapi1:LoginID>" + LoginID + @"</sapi1:LoginID>
            
            <sapi1:Password></sapi1:Password>
            
              <sapi1:Version>" + version + @"</sapi1:Version>
            </tem:Profile>
           </tem:GenerateWayBill>
         </soapenv:Body>
       </soapenv:Envelope>");
            return soapEnvelopeDocument;
        }

        public bool GenerateBluedartEwayBill(string companyCode, string branchCode, string OrderNo, out ShipmentVM shipment, out string errorMessage)
        {
            shipment = null;
            errorMessage = null;
            try {
                string sql = string.Format("EXEC usp_GetOrderDetailsforShipping '{0}', '{1}','{2}'", OrderNo, companyCode, branchCode);
                DataTable dt = SIGlobals.Globals.GetDataTable(sql);
                if (dt == null || dt.Rows.Count <= 0) {
                    errorMessage = "Failed to get order information.";
                    return false;
                }

                string[] ConsigneDetails = new string[7];
                ConsigneDetails = dt.Rows[0]["Consigne_details"].ToString().Split('|');

                string ConName = ConsigneDetails[0];
                string ConAdd1 = ConsigneDetails[1];
                string ConAdd2 = ConsigneDetails[2];
                string ConAdd3 = ConsigneDetails[3];
                string ConAtten = ConsigneDetails[0];
                string ConCity = ConsigneDetails[2];
                string Conemail = ConsigneDetails[6];
                string conmobile = ConsigneDetails[5];
                string ConPin = ConsigneDetails[4];

                string RetAdd1 = dt.Rows[0]["shipper_address1"].ToString();
                string RetAdd2 = dt.Rows[0]["shipper_address2"].ToString();
                string RetAdd3 = dt.Rows[0]["shipper_address3"].ToString();
                string RetEmail = dt.Rows[0]["shipper_email_id"].ToString();
                string RetPin = dt.Rows[0]["shipper_pin_code"].ToString();
                string RetMob = dt.Rows[0]["shipper_mobile_no"].ToString();

                string shipAdd1 = dt.Rows[0]["shipper_address1"].ToString();
                string shipAdd2 = dt.Rows[0]["shipper_address2"].ToString();
                string shipAdd3 = dt.Rows[0]["shipper_address3"].ToString();
                string shipCode = dt.Rows[0]["shipper_code"].ToString();
                string shipName = dt.Rows[0]["shipper_name"].ToString();
                string shipTele = dt.Rows[0]["shipper_mobile_no"].ToString();
                string shipPin = dt.Rows[0]["shipper_pin_code"].ToString();
                string VendCode = dt.Rows[0]["VendorCode"].ToString();
                string Origin = dt.Rows[0]["Origin"].ToString();
                string Sender = dt.Rows[0]["Sender"].ToString();
                string isPayCust = dt.Rows[0]["isPayCust"].ToString();  //bool

                double ActualWt = Convert.ToDouble(dt.Rows[0]["ActualWeight"].ToString());
                double CollAmt = Convert.ToDouble(dt.Rows[0]["CollectableAmount"].ToString());
                string com1 = dt.Rows[0]["CommodityDetail1"].ToString();
                string com2 = dt.Rows[0]["CommodityDetail2"].ToString();
                string com3 = dt.Rows[0]["CommodityDetail2"].ToString();

                string creditRef1 = dt.Rows[0]["CreditReferenceNo"].ToString();
                string creditRef2 = dt.Rows[0]["CreditReferenceNo2"].ToString();
                string creditRef3 = dt.Rows[0]["CreditReferenceNo3"].ToString();

                double len = Convert.ToDouble(dt.Rows[0]["Length"].ToString()); ;
                double Bdth = Convert.ToDouble(dt.Rows[0]["Breadth"].ToString()); ;
                double height = Convert.ToDouble(dt.Rows[0]["Height"].ToString()); ;
                int count = Convert.ToInt32(dt.Rows[0]["Count"].ToString()); ;

                string apiType = dt.Rows[0]["apiType"].ToString();
                string LogArea = dt.Rows[0]["Area"].ToString();
                string LogCode = dt.Rows[0]["Customercode"].ToString();
                string LogLicenceKey = dt.Rows[0]["LicenceKey"].ToString();
                string LoginID = dt.Rows[0]["LoginID"].ToString();
                string version = dt.Rows[0]["Version"].ToString();
                string password = dt.Rows[0]["Password"].ToString();
                string isAdmin = dt.Rows[0]["isAdmin"].ToString();

                string IsForcePickup = dt.Rows[0]["IsForcePickup"].ToString();    //bool
                string IsReversePickup = dt.Rows[0]["IsReversePickup"].ToString();  //bool
                string PDFOutputNotRequired = dt.Rows[0]["PDFOutputNotRequired"].ToString();  //bool
                string RegisterPickup = dt.Rows[0]["RegisterPickup"].ToString();  //bool
                string OTPBasedDelivary = dt.Rows[0]["OTPBasedDelivery"].ToString();  //bool


                string ProductCode = dt.Rows[0]["ProductCode"].ToString();
                string SpecialInstruction = dt.Rows[0]["SpecialInstruction"].ToString();
                string SubProductCode = dt.Rows[0]["SubProductCode"].ToString();
                string productType = dt.Rows[0]["ProductType"].ToString();
                int ProdutCount = Convert.ToInt32(dt.Rows[0]["ProductCount"].ToString());
                string Pickupdate = dt.Rows[0]["PickupDate"].ToString();
                string Pickuptime = dt.Rows[0]["PickupTime"].ToString();
                int PieceCount = Convert.ToInt32(dt.Rows[0]["PieceCount"].ToString());
                double DelaredValue = Convert.ToDouble(dt.Rows[0]["DelaredValue"].ToString());

                WayBillGenerationRequest wayBillGenerationRequest = new WayBillGenerationRequest
                {
                    Consignee = new Consignee
                    {
                        ConsigneeName = ConName,
                        ConsigneeAddress1 = ConAdd1,
                        ConsigneeAddress2 = ConAdd2,
                        ConsigneeAddress3 = ConAdd3,
                        ConsigneeAttention = ConAtten,
                        ConsigneeCityName = ConCity,
                        ConsigneeEmailID = Conemail,
                        ConsigneeMobile = conmobile,
                        ConsigneePincode = ConPin
                    },
                    Returnadds = new ReturnAddress
                    {
                        ReturnAddress1 = RetAdd1,
                        ReturnAddress2 = RetAdd2,
                        ReturnAddress3 = RetAdd3,
                        ReturnEmailID = RetEmail,
                        ReturnPincode = RetPin,
                        ReturnMobile = RetMob
                    },
                    Shipper = new Shipper
                    {
                        CustomerAddress1 = shipAdd1,
                        CustomerAddress2 = shipAdd2,
                        CustomerAddress3 = shipAdd3,
                        CustomerCode = shipCode,
                        CustomerName = shipName,
                        CustomerPincode = shipPin,
                        CustomerTelephone = shipTele,
                        IsToPayCustomer = Convert.ToBoolean(isPayCust),
                        OriginArea = Origin,
                        Sender = Sender,
                        VendorCode = VendCode
                    },
                    Services = new Services
                    {
                        ActualWeight = ActualWt,
                        ActualWeightSpecified = true,
                        CollectableAmount = CollAmt,
                        CreditReferenceNo = creditRef1,
                        CreditReferenceNo2 = creditRef2,
                        CreditReferenceNo3 = creditRef3,
                        DeclaredValue = DelaredValue,
                        DeclaredValueSpecified = true,
                        Dimensions = new Dimension[] { new Dimension { Length = len, Breadth = Bdth, Height = height, Count = count, BreadthSpecified = true, LengthSpecified = true, CountSpecified = true, HeightSpecified = true } },
                        IsForcePickup = Convert.ToBoolean(IsForcePickup),
                        IsReversePickup = Convert.ToBoolean(IsReversePickup),
                        PDFOutputNotRequired = Convert.ToBoolean(PDFOutputNotRequired),
                        PickupDate = Convert.ToDateTime(Pickupdate).AddDays(4),
                        PickupDateSpecified = true,
                        PickupTime = Pickuptime,
                        PieceCount = PieceCount,
                        PieceCountSpecified = true,
                        ProductCode = ProductCode,
                        ProductType = ProductType.Dutiables,
                        RegisterPickup = Convert.ToBoolean(RegisterPickup),
                        OTPBasedDelivery = OTPBasedDelivery.Default,
                        SpecialInstruction = SpecialInstruction,
                        SubProductCode = SubProductCode,
                        Commodity = new CommodityDetail { CommodityDetail1 = com1, CommodityDetail2 = com2, CommodityDetail3 = com3 }
                    }
                };

                ShipmentIntegration.com.bluedart.netconnect.waybill.UserProfile userProfile = new ShipmentIntegration.com.bluedart.netconnect.waybill.UserProfile
                {
                    Api_type = apiType,
                    Area = LogArea,
                    Customercode = LogCode,
                    LicenceKey = LogLicenceKey,
                    LoginID = LoginID,
                    Version = version,
                    IsAdmin = isAdmin,
                    Password = password
                };
                WayBillGeneration waybill = new WayBillGeneration();
                WayBillGenerationResponse response = waybill.GenerateWayBill(wayBillGenerationRequest, userProfile);
                if(response == null) {
                    errorMessage = "There is no response from the Bluedart API";
                    return false;
                }
                shipment = new ShipmentVM
                {
                    Error = response.IsError,
                    AWBNo = response.AWBNo,
                    CCRCrdRefNo = response.CCRCRDREF,
                    OrderNo = OrderNo,
                    AWBPdfPrintContent = response.AWBPrintContent != null ? Convert.ToBase64String(response.AWBPrintContent) : null,
                    ErrorInPU = response.IsErrorInPU,
                    PickUpTokenNo = response.TokenNumber
                };
                string statusMessages = string.Empty;
                foreach (var s in response.Status) {
                    statusMessages += string.Format($"Status code: {s.StatusCode} | Status Information: {s.StatusInformation} {Environment.NewLine}");
                }
                shipment.StatusInfo = statusMessages;
                return !response.IsError;                
            }
            catch (Exception ex) {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool CancelBluedartEwayBill(string companyCode, string branchCode, string awbNo, out string statusMessage)
        {
            statusMessage = string.Empty;
            try {
                string sql = string.Format("select *  from VGetBlueDartUserProfile");
                DataTable dt = SIGlobals.Globals.GetDataTable(sql);
                if (dt == null || dt.Rows.Count <= 0) {
                    statusMessage = "Failed to get order information.";
                    return false;
                }

                AWBCancelationRequest aWBCancelationRequest = new AWBCancelationRequest
                {
                    AWBNo = awbNo
                };
                string apiType = dt.Rows[0]["apiType"].ToString();
                string area = dt.Rows[0]["Area"].ToString();
                string logCode = dt.Rows[0]["Customercode"].ToString();
                string logLicenceKey = dt.Rows[0]["LicenceKey"].ToString();
                string loginID = dt.Rows[0]["LoginID"].ToString();
                string version = dt.Rows[0]["Version"].ToString();
                string password = dt.Rows[0]["Password"].ToString();
                string isAdmin = dt.Rows[0]["isAdmin"].ToString();
                ShipmentIntegration.com.bluedart.netconnect.waybill.UserProfile userProfile = new ShipmentIntegration.com.bluedart.netconnect.waybill.UserProfile
                {
                    Api_type = apiType,
                    Area = area,
                    Customercode = logCode,
                    LicenceKey = logLicenceKey,
                    LoginID = loginID,
                    Version = version,
                    IsAdmin = isAdmin,
                    Password = password
                };

                WayBillGeneration waybill = new WayBillGeneration();
                var response = waybill.CancelWaybill(aWBCancelationRequest, userProfile);
                if (response == null) {
                    return false;
                }
                var isError = response.IsError;
                statusMessage = string.Empty;
                foreach (var s in response.Status) {
                    statusMessage += string.Format($"Status code: {s.StatusCode} | Status Information: {s.StatusInformation} {Environment.NewLine}");
                }
                return !isError;
            }
            catch (Exception ex) {
                statusMessage = ex.Message;
                return false;
            }
        }

        public bool RegisterPickup(PickupRegistrationInputVM vm, string awbNo, out string tokenNo, out string statusMessage)
        {
            statusMessage = string.Empty;
            tokenNo = string.Empty;
            try {
                TimeSpan startOfficeHours = new TimeSpan(10, 00, 0); //10:00 am
                TimeSpan endOfficeHours = new TimeSpan(18,30, 0); //06:30 pm
                TimeSpan pkTime = vm.PickupDate.TimeOfDay;

                //check if the pikcup time is in between 9:30am and 6.30pm
                if ((pkTime >= startOfficeHours) && (pkTime <= endOfficeHours)) {
                    //match found
                    ;
                }
                else {
                    statusMessage = "Pickup time must be between 10:00:00 AM and 06:30:00 PM";
                    return false;
                }

                string sql = string.Format("EXEC usp_GetOrderDetailsforShipping '{0}', '{1}','{2}'", vm.OrderNo, vm.CompanyCode, vm.BranchCode);
                DataTable dt = SIGlobals.Globals.GetDataTable(sql);
                if (dt == null || dt.Rows.Count <= 0) {
                    statusMessage = "Failed to get order information.";
                    return false;
                }
                string[] ConsigneDetails = new string[7];
                ConsigneDetails = dt.Rows[0]["Consigne_details"].ToString().Split('|');

                string ConName = ConsigneDetails[0];
                string ConAdd1 = ConsigneDetails[1];
                string ConAdd2 = ConsigneDetails[2];
                string ConAdd3 = ConsigneDetails[3];
                string ConAtten = ConsigneDetails[0];
                string ConCity = ConsigneDetails[2];
                string Conemail = ConsigneDetails[6];
                string conmobile = ConsigneDetails[5];
                string ConPin = ConsigneDetails[4];
                
                string RetAdd1 = dt.Rows[0]["shipper_address1"].ToString();
                string RetAdd2 = dt.Rows[0]["shipper_address2"].ToString();
                string RetAdd3 = dt.Rows[0]["shipper_address3"].ToString();
                string RetEmail = dt.Rows[0]["shipper_email_id"].ToString();
                string RetPin = dt.Rows[0]["shipper_pin_code"].ToString();
                string RetMob = dt.Rows[0]["shipper_mobile_no"].ToString();

                string shipAdd1 = dt.Rows[0]["shipper_address1"].ToString();
                string shipAdd2 = dt.Rows[0]["shipper_address2"].ToString();
                string shipAdd3 = dt.Rows[0]["shipper_address3"].ToString();
                string shipCode = dt.Rows[0]["shipper_code"].ToString();
                string shipName = dt.Rows[0]["shipper_name"].ToString();
                string shipTele = dt.Rows[0]["shipper_mobile_no"].ToString();
                string shipPin = dt.Rows[0]["shipper_pin_code"].ToString();
                string VendCode = dt.Rows[0]["VendorCode"].ToString();
                string Origin = dt.Rows[0]["Origin"].ToString();
                string Sender = dt.Rows[0]["Sender"].ToString();
                string isPayCust = dt.Rows[0]["isPayCust"].ToString();  //bool
                string shipPhone = dt.Rows[0]["shipper_phone"].ToString();
                string shipEmail = dt.Rows[0]["shipper_email_id"].ToString();


                double ActualWt = Convert.ToDouble(dt.Rows[0]["ActualWeight"].ToString());
                double CollAmt = Convert.ToDouble(dt.Rows[0]["CollectableAmount"].ToString());
                string com1 = dt.Rows[0]["CommodityDetail1"].ToString();
                string com2 = dt.Rows[0]["CommodityDetail2"].ToString();
                string com3 = dt.Rows[0]["CommodityDetail2"].ToString();

                string creditRef1 = dt.Rows[0]["CreditReferenceNo"].ToString();
                string creditRef2 = dt.Rows[0]["CreditReferenceNo2"].ToString();
                string creditRef3 = dt.Rows[0]["CreditReferenceNo3"].ToString();

                double len = Convert.ToDouble(dt.Rows[0]["Length"].ToString()); ;
                double Bdth = Convert.ToDouble(dt.Rows[0]["Breadth"].ToString()); ;
                double height = Convert.ToDouble(dt.Rows[0]["Height"].ToString()); ;
                int count = Convert.ToInt32(dt.Rows[0]["Count"].ToString()); ;

                string apiType = dt.Rows[0]["apiType"].ToString();
                string LogArea = dt.Rows[0]["Area"].ToString();
                string LogCode = dt.Rows[0]["Customercode"].ToString();
                string LogLicenceKey = dt.Rows[0]["LicenceKey"].ToString();
                string LoginID = dt.Rows[0]["LoginID"].ToString();
                string version = dt.Rows[0]["Version"].ToString();
                string password = dt.Rows[0]["Password"].ToString();

                string IsForcePickup = dt.Rows[0]["IsForcePickup"].ToString();    //bool
                string IsReversePickup = dt.Rows[0]["IsReversePickup"].ToString();  //bool
                string PDFOutputNotRequired = dt.Rows[0]["PDFOutputNotRequired"].ToString();  //bool
                string RegisterPickup = dt.Rows[0]["RegisterPickup"].ToString();  //bool

                string ProductCode = dt.Rows[0]["ProductCode"].ToString();
                string SpecialInstruction = dt.Rows[0]["SpecialInstruction"].ToString();
                string SubProductCode = dt.Rows[0]["SubProductCode"].ToString();
                string SubProductName = dt.Rows[0]["SubProductName"].ToString();
                string ProductType = dt.Rows[0]["ProductType"].ToString();
                int ProdutCount = Convert.ToInt32(dt.Rows[0]["ProductCount"].ToString());

                string ShipPickupdate = dt.Rows[0]["PickupDate"].ToString();
                string ShipPickuptime = dt.Rows[0]["PickupTime"].ToString();
                int PieceCount = Convert.ToInt32(dt.Rows[0]["PieceCount"].ToString());
                double DelaredValue = Convert.ToDouble(dt.Rows[0]["DelaredValue"].ToString());
                string closingtime = dt.Rows[0]["closing_time"].ToString();
                
                string pickupTime = vm.PickupDate.ToString("HHmm");

                string DoxNDox = dt.Rows[0]["DoxNDox"].ToString();
                string RouteCode = dt.Rows[0]["RouteCode"].ToString();
                string RegRemarks = dt.Rows[0]["RegRemarks"].ToString();

                string[] awbArry = new string[] { awbNo };
                string[] subProdArray = new string[] { SubProductName };

                PickupRegistrationRequest pickupRegistrationRequest = new PickupRegistrationRequest
                {
                    AreaCode = LogArea,
                    AWBNo = awbArry,
                    ContactPersonName = vm.ContactPerson,
                    CustomerAddress1 = shipAdd1,
                    CustomerAddress2 = shipAdd2,
                    CustomerAddress3 = shipAdd3,
                    CustomerCode = LogCode,
                    CustomerName = shipName,
                    CustomerPincode = shipPin,
                    CustomerTelephoneNumber = shipPhone,
                    DoxNDox = DoxNDox,
                    EmailID = RetEmail,
                    IsForcePickup = Convert.ToBoolean(IsForcePickup),
                    IsReversePickup = Convert.ToBoolean(IsReversePickup),
                    MobileTelNo = vm.ContactPersonMobileNo,
                    NumberofPieces = PieceCount,
                    OfficeCloseTime = closingtime,
                    PackType = "",
                    SubProducts = subProdArray,
                    VolumeWeight = ActualWt,
                    WeightofShipment = ActualWt,
                    isToPayShipper = Convert.ToBoolean(isPayCust),
                    ShipmentPickupDate = vm.PickupDate.Date,
                    ShipmentPickupTime = pickupTime,
                    ProductCode = ProductCode,
                    VolumeWeightSpecified = true,
                    WeightofShipmentSpecified = true,
                    IsForcePickupSpecified = true,
                    isToPayShipperSpecified = true,
                    ShipmentPickupDateSpecified = true,
                    RouteCode = RouteCode,
                    Remarks = RegRemarks
                };
                ShipmentIntegration.com.bluedart.netconnect.pickup.UserProfile userProfile = new ShipmentIntegration.com.bluedart.netconnect.pickup.UserProfile
                {
                    Api_type = apiType,
                    Area = LogArea,
                    Customercode = LogCode,
                    LicenceKey = LogLicenceKey,
                    LoginID = LoginID,
                    Version = version,
                    IsAdmin = "",
                    Password = password
                };

                PickupRegistration pickupRegistration = new PickupRegistration();
                var response = pickupRegistration.RegisterPickup(pickupRegistrationRequest, userProfile);

                if (response == null) {
                    statusMessage = "Failed to get response from shipping API.";
                    return false;
                }
                var isError = response.IsError;
                if (!isError) {
                    tokenNo = response.TokenNumber;
                }
                
                statusMessage = string.Empty;
                foreach (var s in response.Status) {
                    statusMessage += string.Format($"Status code: {s.StatusCode} | Status Information: {s.StatusInformation} {Environment.NewLine}");
                }
                return !isError;
            }
            catch (Exception ex) {
                statusMessage = ex.Message;
                return false;
            }
        }
        public bool CancelPickupRegistration(string companyCode, string branchCode, int tokenNo, string Remarks, DateTime pickupDate, out string statusMessage)
        {
            statusMessage = string.Empty;
            try {
                string sql = string.Format("SELECT * FROM VGetBlueDartUserProfile");
                DataTable dtUserProfile = SIGlobals.Globals.GetDataTable(sql);
                if (dtUserProfile == null || dtUserProfile.Rows.Count <= 0) {
                    statusMessage = "Faile to get user profile information.";
                    return false;
                }

                string apiType = dtUserProfile.Rows[0]["apiType"].ToString();
                string area = dtUserProfile.Rows[0]["Area"].ToString();
                string logCode = dtUserProfile.Rows[0]["Customercode"].ToString();
                string logLicenceKey = dtUserProfile.Rows[0]["LicenceKey"].ToString();
                string loginID = dtUserProfile.Rows[0]["LoginID"].ToString();
                string version = dtUserProfile.Rows[0]["Version"].ToString();
                string password = dtUserProfile.Rows[0]["Password"].ToString();
                string isAdmin = dtUserProfile.Rows[0]["isAdmin"].ToString();

                CancelPickupRequest cancelPickupRequest = new CancelPickupRequest
                {
                    TokenNumber = tokenNo,
                    PickupRegistrationDate = pickupDate.Date,
                    PickupRegistrationDateSpecified = true,
                    Remarks = Remarks,
                    TokenNumberSpecified = true
                };


                ShipmentIntegration.com.bluedart.netconnect.pickup.UserProfile userProfile = new ShipmentIntegration.com.bluedart.netconnect.pickup.UserProfile
                {
                    Api_type = apiType,
                    Area = area,
                    Customercode = logCode,
                    LicenceKey = logLicenceKey,
                    LoginID = loginID,
                    Version = version,
                    IsAdmin = isAdmin,
                    Password = password
                };

                ShipmentIntegration.com.bluedart.netconnect.pickup.PickupRegistration pickupRegistration = new PickupRegistration();
                var response = pickupRegistration.CancelPickup(cancelPickupRequest, userProfile);

                if (response == null) {
                    statusMessage = "Failed to get response from shipping API.";
                    return false;
                }
                var isError = response.IsError;

                statusMessage = string.Empty;
                foreach (var s in response.Status) {
                    statusMessage += string.Format($"Status code: {s.StatusCode} | Status Information: {s.StatusInformation} {Environment.NewLine}");
                }
                return !isError;
            }
            catch (Exception ex) {
                statusMessage = ex.Message;
                return false;
            }

        }

        #endregion
    }
}

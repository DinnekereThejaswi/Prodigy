using NReco.PdfGenerator;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProdigyAPI.BL.BusinessLayer.Common
{
    public class DocumentPrintBL
    {
        MagnaDbEntities db = null;
        public DocumentPrintBL()
        {
            db = new MagnaDbEntities(true);
        }

        public DocumentPrintBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public string PrintSalesBillWithHeaderFooter(string companyCode, string branchCode, int salesbillno, out string errorMessage)
        {
            errorMessage = string.Empty;
            StringBuilder sbHTML = new StringBuilder();
            StringBuilder sbStart = new StringBuilder();
            try {
                string salesmaster = string.Empty;
                salesmaster = string.Format("select s.est_no,s.bill_no,s.cust_id,bill_date,s.salutation,s.cust_name,s.address1,s.address2,s.tin,s.address3,s.city,s.pan_no,s.state,s.state_code ,\n"
                                           + "s.pin_code,s.mobile_no,s.rate ,s.tax,s.discount_amount,s.total_tax_amount,s.order_amount,s.operator_code,s.karat,s.remarks,\n"
                                           + "s.bill_counter,s.gstype,excise_duty_percent,excise_duty_amount,ed_cess_percent,ed_cess,round_off,\n"
                                           + "hed_cess_percent,hed_cess,cm.Footer1,cm.Footer2,cflag,pos,order_no,order_date,case when inv_source='1' then (select isnull(market_place_order_no,0) as market_place_order_no from kttu_order_master where  order_no=s.order_no)  when inv_source='3' then (select isnull(order_reference_no,0) as order_reference_no  from kttu_order_master where  order_no=s.order_no)  else '' end as marketrefno  \n"
                                           + "from KSTU_COMPANY_MASTER CM,KTTU_SALES_MASTER S \n"
                                           + "where s.bill_no = {0} and s.company_code = '{1}' and s.branch_code = '{2}' and CM.company_code = '{1}' and CM.branch_code = '{2}' \n"
                                           + "and cm.company_code = s.company_code and cm.branch_code = s.branch_code"
                   , salesbillno, companyCode, branchCode);
                DataTable dtSalesBilling = Globals.GetDataTable(salesmaster);
                if (dtSalesBilling == null || dtSalesBilling.Rows.Count == 0) {
                    errorMessage = "Sales Bill Number does not Exist";
                    return null;
                }

                string comp = string.Format("select cm.company_name,address1,address2,address3,city,state,phone_no,tin_no,pan_no,cst_no,ED_Reg_No,cm.Header1,cm.Header2,cm.Header3,cm.Header4,cm.Header5,cm.Header6,cm.Header7, \n"
                              + "state,state_code,cm.Footer1,cm.Footer2, email_id,website,pin_code from KSTU_COMPANY_MASTER cm WHERE cm.company_code = '{0}' and cm.branch_code = '{1}'", companyCode, branchCode);
                DataTable dtCompanyDetails = Globals.GetDataTable(comp);


                string CompanyAddress = string.Empty;
                string companyName = dtCompanyDetails.Rows[0]["Header1"] == DBNull.Value ? string.Empty : dtCompanyDetails.Rows[0]["Header1"].ToString();
                if (dtCompanyDetails.Rows[0]["Header1"].ToString() != string.Empty)
                    CompanyAddress = dtCompanyDetails.Rows[0]["Header1"].ToString();
                if (dtCompanyDetails.Rows[0]["Header2"].ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br>" + dtCompanyDetails.Rows[0]["Header2"].ToString();

                if (dtCompanyDetails.Rows[0]["tin_no"].ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br>" + "TIN: " + dtCompanyDetails.Rows[0]["tin_no"].ToString();

                if (dtCompanyDetails.Rows[0]["email_id"].ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br> E-mail : " + dtCompanyDetails.Rows[0]["email_id"].ToString();

                if (dtCompanyDetails.Rows[0]["website"].ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br> Website : " + dtCompanyDetails.Rows[0]["website"].ToString();

                sbStart.AppendLine("<html>");
                sbStart.AppendLine("<head>");
                sbStart.AppendLine(Globals.GetStyle());
                sbStart.AppendLine("</head>");
                sbStart.AppendLine("<body>");

                //sbStart.AppendLine("<id =.watermark>");
                //sbStart.AppendLine("<id =.watermark::after>");
                string DateTime = string.Format("{0:dd/MM/yyyy} {0:T}", Convert.ToDateTime(dtSalesBilling.Rows[0]["bill_date"]));

                sbStart.AppendLine("<Table frame=\"border\" border=\"0\" width=\"900\">");  //FRAME=BOX RULES=NONE
                sbStart.AppendLine("<TR style=border:0>");
                string imageMapPath = Globals.GetWebConfigValue("ImageUrl");
                if(string.IsNullOrEmpty(imageMapPath))
                    imageMapPath = System.Web.HttpContext.Current.Request.MapPath(@"~\App_Data\");
                string headerimg = imageMapPath + string.Format(@"\image_assets\invoice_header.png");
                string logoImage = imageMapPath + string.Format(@"\image_assets\logo.png");
                string footerImg = imageMapPath + string.Format(@"\image_assets\invoice_footer.png");
                sbStart.AppendLine(string.Format("<img src=\"{0}\" width=\"900\" />", headerimg));

                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR style=border:0;\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                if (string.Compare(dtSalesBilling.Rows[0]["cflag"].ToString(), "Y") == 0) {
                    sbStart.AppendLine("<TD style=border:0 width=\"900\" colspan=0 ALIGN = \"CENTER\"><b><h3>TAX INVOICE/CANCELLED</h3></b></TD>");
                }
                else {
                    sbStart.AppendLine("<TD style=border:0 width=\"900\" colspan=0 ALIGN = \"CENTER\"><b><h3>TAX INVOICE</h3></b></TD>");
                }
                sbStart.AppendLine("</TR>");

                //for (int j = 0; j < 2; j++)
                //{
                //    sbStart.AppendLine("<TR style=border:0>");
                //    sbStart.AppendLine(string.Format("<TD style=border:0 colspan = 13 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                //    sbStart.AppendLine("</TR>");
                //}

                sbStart.AppendLine("</Table>");  //FRAME=BOX RULES=NONE
                sbStart.AppendLine("<div id=\"image\">");
                sbStart.AppendLine(string.Format("<img src=\"{0}\" width=\"900\" />", logoImage));//"https://www.google.co.in/images/srpr/logo1"  
                sbStart.AppendLine(WaterMarkStyle("0.1", "40", "#image"));
                //string salesdetails = string.Format("Select d.item_name as name,i.[Item Name] as Item,item_no,(gwt+AddWt-DeductWt) as gwt,D.counter_code, rate,isnull(d.hsn ,'') as hsn,isnull(SGST_Amount,0) as SGST_Amount, isnull(CGST_Amount,0) as CGST_Amount, isnull(IGST_Amount,0) as IGST_Amount,\n"
                //                 + "(nwt+AddWt-DeductWt) as nwt,swt,gold_value,stone_charges,diamond_charges,mc_percent,making_charge_per_rs, sal_code,mc_Discount_Amt, d.karat,d.item_final_amount ,d.item_total_after_discount,isnull(item_additional_discount,0) as item_additional_discount ,isnull(offer_value,0) as offer_value,\n"
                //                 + "total_amount,va_amount,D.gs_code,wastage_grms,hallmarcharges,mc_amount,SGST_Percent	,	CGST_Percent	,	IGST_Percent from KTTU_SALES_DETAILS D, [item_master] I \n"
                //                 + "where bill_no = {0} and D.company_code = '{1}' and D.branch_code = '{2}' and I.company_code = '{1}' and I.branch_code = '{2}' AND I.ITEM_CODE = D.ITEM_NAME AND I.GS_CODE = D.GS_CODE order by sl_no",
                //                 salesbillno, companyCode, branchCode);
                string salesdetails = string.Format(@"SELECT d.item_name  AS NAME, i.[item name]   AS Item, Sum(item_no)   AS item_no,  ( Sum(gwt) + Sum(addwt) - Sum(deductwt) ) AS gwt, D.counter_code, rate,  Isnull(d.hsn, '')  AS hsn,  Isnull(Sum(sgst_amount), 0)  AS SGST_Amount,  Isnull(Sum(cgst_amount), 0)  AS CGST_Amount, Isnull(Sum(igst_amount), 0)  AS IGST_Amount,  ( Sum(nwt) + Sum(addwt) - Sum(deductwt) ) AS nwt,  Sum(swt)   AS swt, Sum(gold_value) AS gold_value, 
                                                 Sum(stone_charges)  AS stone_charges,  Sum(diamond_charges)  AS diamond_charges,  sum(mc_percent)  as mc_percent,  Sum(making_charge_per_rs)   AS making_charge_per_rs, sal_code,  Sum(mc_discount_amt)  AS mc_Discount_Amt, d.karat,  Sum(d.item_final_amount) AS item_final_amount,  Sum(d.item_total_after_discount)  AS item_total_after_discount,  Isnull(Sum(item_additional_discount), 0)  AS item_additional_discount,
                                                 Isnull(Sum(offer_value), 0)  AS offer_value, Sum(total_amount)  AS total_amount,  Sum(va_amount)  AS va_amount,  D.gs_code,  Sum(wastage_grms)  AS wastage_grms, Sum(hallmarcharges)  AS hallmarcharges,  Sum(mc_amount)  AS mc_amount, sgst_percent , cgst_percent,  igst_percent FROM   kttu_sales_details D,  [item_master] I 
                                                 WHERE bill_no = {0} and D.company_code = '{1}' and D.branch_code = '{2}' and I.company_code = '{1}' and I.branch_code = '{2}' AND I.ITEM_CODE = D.ITEM_NAME AND I.GS_CODE = D.GS_CODE GROUP  BY d.item_name, i.[item name] , d.hsn,  sgst_percent , cgst_percent,  igst_percent, D.counter_code, rate, sal_code,  d.karat,  D.gs_code",
                                salesbillno, companyCode, branchCode);
                DataTable dtSalesDetails = Globals.GetDataTable(salesdetails);

                string metaltype = Globals.GetMetalType(db, dtSalesDetails.Rows[0]["gs_code"].ToString(), companyCode, branchCode);

                if (dtSalesDetails == null) {
                    errorMessage = "Sales details/lines are not found.";
                    return null;
                }

                sbStart.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\";  style=\"border-collapse:collapse;\" width=\"900\">");  //FRAME=BOX RULES=NONE

                string Sultation = dtSalesBilling.Rows[0]["salutation"].ToString();
                string Name = dtSalesBilling.Rows[0]["Cust_Name"].ToString();
                string Address1 = dtSalesBilling.Rows[0]["address1"].ToString();
                string Address2 = dtSalesBilling.Rows[0]["address2"].ToString();
                string Address3 = dtSalesBilling.Rows[0]["address3"].ToString();
                string Mobile = dtSalesBilling.Rows[0]["mobile_no"].ToString();
                string operator_code = dtSalesBilling.Rows[0]["operator_code"].ToString();
                string PAN = dtSalesBilling.Rows[0]["pan_no"].ToString();
                string custGSTIN = dtSalesBilling.Rows[0]["tin"].ToString();
                string state_code = dtSalesBilling.Rows[0]["state_code"].ToString();
                string state = dtSalesBilling.Rows[0]["state"].ToString();
                string placeOfSupply = Globals.GetScalarValue<string>(string.Format("select isnull(state_name,'') from ksts_state_master where tinno={0}", dtSalesBilling.Rows[0]["pos"].ToString()));

                //if (!string.IsNullOrEmpty(Sultation))
                //    Name = Sultation + " " + Name;

                string City = dtSalesBilling.Rows[0]["city"].ToString();
                string Address = string.Empty;

                if (Address1 != string.Empty)
                    Address = Address + "<br>" + Address1;
                if (Address2 != string.Empty)
                    Address = Address + "<br>" + Address2;
                if (Address3 != string.Empty)
                    Address = Address + "<br>" + Address3;

                int orderNo = Globals.GetScalarValue<int>(string.Format("select order_no from KTTU_SALES_MASTER  where bill_no='{0}' and company_code='{1}' and branch_code='{2}' ", salesbillno, companyCode, branchCode));
                int order_no = Convert.ToInt32(orderNo);
                string[] shippingAddress = new string[10];
                string shippingAdd = Globals.GetScalarValue<string>(string.Format("select shipping_address from KTTU_ORDER_MASTER where order_no={0} and company_code='{1}' and branch_code='{2}'", order_no, companyCode, branchCode));
                int orderSource = Globals.GetScalarValue<int>(string.Format("select isnull(order_source,0) as order_source from KTTU_ORDER_MASTER where order_no={0} and company_code='{1}' and branch_code='{2}'", order_no, companyCode, branchCode));
                if (!string.IsNullOrEmpty(shippingAdd)) {
                    shippingAddress = shippingAdd.Split('|');
                }

                sbStart.AppendLine("<tr>");
                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");  //FRAME=BOX RULES=NONE
                sbStart.AppendLine("<tr  style=\"border-right:0\"  >");
                sbStart.AppendLine("<td width=\"300\" style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>Sold By :</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>&nbsp</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" >" + "&nbsp" + dtCompanyDetails.Rows[0]["company_name"].ToString() + "</td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" >" + "&nbsp" + dtCompanyDetails.Rows[0]["address1"].ToString() + "</td>");
                sbStart.AppendLine("</tr>");
                if (!string.IsNullOrEmpty(dtCompanyDetails.Rows[0]["address2"].ToString())) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" >" + "&nbsp" + dtCompanyDetails.Rows[0]["address2"].ToString() + "</td>");
                    sbStart.AppendLine("</tr>");
                }

                if (!string.IsNullOrEmpty(dtCompanyDetails.Rows[0]["address3"].ToString())) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" >" + "&nbsp" + dtCompanyDetails.Rows[0]["address3"].ToString() + "</td>");
                    sbStart.AppendLine("</tr>");
                }

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + "&nbsp" + dtCompanyDetails.Rows[0]["city"].ToString() + "</td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine(string.Format("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" >&nbsp{0}-{1}</td>", dtCompanyDetails.Rows[0]["state"].ToString(), dtCompanyDetails.Rows[0]["pin_code"].ToString()));

                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine(string.Format("<td style=\"border-right:thin \"  style=\"border-top:thin\" >&nbspGSTN:{0}</td>", dtCompanyDetails.Rows[0]["tin_no"].ToString()));
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("</table>");

                sbStart.AppendLine("</td>");
                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");  //FRAME=BOX RULES=NONE

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td width=\"300\"  style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>Billing Address:</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>&nbsp</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + "&nbsp" + Name + "</td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + Address1 + "</td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + Address2 + "</td>");
                sbStart.AppendLine("</tr>");
                if (!string.IsNullOrEmpty(Address3)) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + Address3 + "</td>");
                    sbStart.AppendLine("</tr>");
                }

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + City + "</td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                if (!string.IsNullOrEmpty(Convert.ToString(dtSalesBilling.Rows[0]["pin_code"])))
                    sbStart.AppendLine(string.Format("<td style=\"border-right:thin \"  style=\"border-top:thin\" > " + "&nbsp" + state + "," + dtSalesBilling.Rows[0]["pin_code"].ToString() + "</td>"));
                else
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + state + "</td>");

                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >&nbsp State/UT Code:" + state_code + "</td>");
                sbStart.AppendLine("</tr>");

                if (!string.IsNullOrEmpty(PAN)) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + PAN + "</td>");
                    sbStart.AppendLine("</tr>");
                }

                sbStart.AppendLine("</table>");

                sbStart.AppendLine("</td>");

                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");//FRAME=BOX RULES=NONE
                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td width=\"300\"  style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>Shipping Address: </b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>&nbsp</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                if (Convert.ToString(orderSource) == "3")
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + "&nbsp" + Convert.ToString(shippingAddress[0]) + "</td>");
                else
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + "&nbsp" + Name + "</td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                if (Convert.ToString(orderSource) == "3")
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + Convert.ToString(shippingAddress[1]) + "</td>");
                else
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + Address1 + "</td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                if (Convert.ToString(orderSource) == "3")
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + Convert.ToString(shippingAddress[2]) + "</td>");
                else
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + Address2 + "</td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                if (Convert.ToString(orderSource) == "3")
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + Convert.ToString(shippingAddress[3]) + ", " + Convert.ToString(shippingAddress[4]) + "</td>");
                else
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + "&nbsp" + Address3 + ", " + City + "</td>");
                sbStart.AppendLine("</tr>");

                if (Convert.ToString(orderSource) == "3") {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >&nbsp State/UT Code:" + Convert.ToString(shippingAddress[7]) + "</td>");
                    sbStart.AppendLine("</tr>");

                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >&nbsp Place of supply:" + Convert.ToString(shippingAddress[3]) + "</td>");
                    sbStart.AppendLine("</tr>");
                }
                else {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >&nbsp State/UT Code:" + state_code + "</td>");
                    sbStart.AppendLine("</tr>");

                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >&nbsp Place of supply:" + state + "</td>");
                    sbStart.AppendLine("</tr>");
                }


                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table>");

                sbStart.AppendLine("</br>");

                sbStart.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; border-top:thin\" width=\"900\">");  //FRAME=BOX RULES=NONE
                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                if (!string.IsNullOrEmpty(Convert.ToString(dtSalesBilling.Rows[0]["marketrefno"])))
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >Market Place Order No:" + dtSalesBilling.Rows[0]["marketrefno"].ToString() + "</td>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                if (!string.IsNullOrEmpty(Convert.ToString(dtSalesBilling.Rows[0]["order_no"])))
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >Order Number:" + dtSalesBilling.Rows[0]["order_no"].ToString() + "</td>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                if (!string.IsNullOrEmpty(Convert.ToString(dtSalesBilling.Rows[0]["order_date"])))
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >Order Date:" + dtSalesBilling.Rows[0]["order_date"].ToString() + "</td>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table>");
                sbStart.AppendLine("</td>");
                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                if (!string.IsNullOrEmpty(Convert.ToString(dtSalesBilling.Rows[0]["order_no"])))
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >Invoice Number:" + salesbillno + "</td>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                if (!string.IsNullOrEmpty(Convert.ToString(dtSalesBilling.Rows[0]["order_date"])))
                    sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >Invoice Date:" + dtSalesBilling.Rows[0]["bill_date"].ToString() + "</td>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table>");
                sbStart.AppendLine("</td>");
                sbStart.AppendLine("</table>");
                //sbStart.AppendLine("<div id=\"image\">");
                //sbStart.AppendLine(string.Format("<img src=\"{0}\" width=\"900\" />", img + ".png"));//"https://www.google.co.in/images/srpr/logo1"            
                sbStart.AppendLine("</table>");


                sbStart.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; border-top:thin\" width=\"900\">");  //FRAME=BOX RULES=NONE
                sbStart.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ;border-top:thin \" ALIGN = \"CENTER\">Sl.No</TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\">Description</TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\">HSN Code</TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">Purity</TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">Rate/Gram</TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">Qty</TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">Gr.Wt</TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">St.Wt</TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">Net Wt</TH>");


                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\">Jewel Amount</TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">VA Amount</TH>");

                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\">Stone/Diamond Amount</TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\">GrossAmount</TH>");
                sbStart.AppendLine("</TR>");


                int Pcs = 0;
                decimal Gwt = 0, Swt = 0, Nwt = 0, GoldValue = 0, StoneChg = 0, ValueAmt = 0, VAPercent = 0, TotalShippingGST = 0, InvoiceAmount = 0;
                string VA = string.Empty;
                decimal TotalSGST = 0, TotalCGST = 0, TotalIGST = 0, dicountMc = 0, totalTaxableAmount = 0, AdditonalDiscAmount = 0, totalGrosAmount = 0, ItemGrosAmount = 0, offerDiscountAmount = 0;
                int MaxPageRow = 8;
                string metalType = Globals.GetMetalType(db, dtSalesDetails.Rows[0]["gs_code"].ToString(), companyCode, branchCode);

                for (int i = 0; i < dtSalesDetails.Rows.Count; i++) {
                    string IsPieceItem = Globals.IsPeiceItem(db, dtSalesDetails.Rows[i]["gs_code"].ToString(), dtSalesDetails.Rows[i]["name"].ToString(), companyCode, branchCode);

                    if (Convert.ToDecimal(dtSalesDetails.Rows[i]["va_amount"]) > 0) {
                        VAPercent = (Convert.ToDecimal(dtSalesDetails.Rows[i]["va_amount"]) * 100) / Convert.ToDecimal(dtSalesDetails.Rows[i]["gold_value"]);
                        VAPercent = decimal.Round(VAPercent, 2);
                    }
                    else {
                        VAPercent = 0.00M;
                    }
                    VA = Globals.GetVAcode(VAPercent);

                    decimal MCAmount = 0;
                    if (Convert.ToDecimal(dtSalesDetails.Rows[i]["making_charge_per_rs"]) > 0) {
                        MCAmount = Convert.ToDecimal(dtSalesDetails.Rows[i]["nwt"]) * Convert.ToDecimal(dtSalesDetails.Rows[i]["making_charge_per_rs"]);
                    }
                    else if (Convert.ToDecimal(dtSalesDetails.Rows[i]["mc_percent"]) > 0) {
                        MCAmount = (Convert.ToDecimal(dtSalesDetails.Rows[i]["gold_value"]) * Convert.ToDecimal(dtSalesDetails.Rows[i]["mc_percent"])) / 100;
                    }
                    else {
                        MCAmount = Convert.ToDecimal(dtSalesDetails.Rows[i]["mc_amount"]);
                    }
                    MCAmount = decimal.Round(MCAmount, 2);

                    string ItemName1 = dtSalesDetails.Rows[i]["Item"].ToString();
                    int qty = Convert.ToInt32(dtSalesDetails.Rows[i]["item_no"]);

                    sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"center\">{0}</TD>", i + 1));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\">{0}</TD>", ItemName1));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\">{0}</TD>", dtSalesDetails.Rows[i]["HSN"]));


                    if (!dtSalesDetails.Rows[i]["karat"].ToString().Equals("NA"))
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"CENTER\">{0}</TD>", dtSalesDetails.Rows[i]["karat"]));
                    else
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"CENTER\">{0}</TD>", "&nbsp"));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}</TD>", dtSalesDetails.Rows[i]["rate"], "&nbsp"));

                    if (qty == 0 && string.Compare(dtSalesDetails.Rows[i]["name"].ToString(), "CHSU") != 0 && string.Compare(dtSalesDetails.Rows[i]["name"].ToString(), "CHSU_18K") != 0) {
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1}</TD>"
                            , "1", "&nbsp"));
                    }
                    else {
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1}</TD>"
                           , dtSalesDetails.Rows[i]["item_no"], "&nbsp"));
                    }

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1}</TD>", dtSalesDetails.Rows[i]["gwt"], "&nbsp"));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1}</TD>", dtSalesDetails.Rows[i]["swt"], "&nbsp"));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1}</TD>", dtSalesDetails.Rows[i]["nwt"], "&nbsp"));

                    decimal vaAmount = Convert.ToDecimal(dtSalesDetails.Rows[i]["va_amount"]);
                    if (IsPieceItem.Equals("Y")) {
                        decimal goldValue = 0; // Convert.ToDecimal(dtSalesDetails.Rows[i]["gold_value"]) - (Convert.ToDecimal(dtSalesDetails.Rows[i]["stone_charges"]) + Convert.ToDecimal(dtSalesDetails.Rows[i]["diamond_charges"]) + Convert.ToDecimal(dtSalesDetails.Rows[i]["hallmarcharges"]));
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\">{0}{1}{1}</TD>", goldValue, "&nbsp"));
                        GoldValue += goldValue;
                    }
                    else {


                        decimal wastAmt = 0;
                        decimal goldValue = Convert.ToDecimal(dtSalesDetails.Rows[i]["gold_value"]);
                        decimal wastGrams = Convert.ToDecimal(dtSalesDetails.Rows[i]["wastage_grms"]);
                        if (wastGrams > 0) {
                            wastAmt = decimal.Round((wastGrams * Convert.ToDecimal(dtSalesDetails.Rows[0]["rate"])), 2);

                        }
                        GoldValue += goldValue;
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1}</TD>", goldValue, "&nbsp")); // dtSalesDetails.Rows[i]["gold_value"],
                    }

                    decimal StoneAmt = Convert.ToDecimal(dtSalesDetails.Rows[i]["stone_charges"])
                        + Convert.ToDecimal(dtSalesDetails.Rows[i]["diamond_charges"])
                        + Convert.ToDecimal(dtSalesDetails.Rows[i]["hallmarcharges"]);

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1}</TD>", Convert.ToDecimal(dtSalesDetails.Rows[i]["va_amount"]), "&nbsp"));

                    //if (StoneAmt > 0)
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}</TD>", StoneAmt, "&nbsp"));
                    //else
                    //  sbStart.AppendLine(string.Format("<TD ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));

                    Gwt += Convert.ToDecimal(dtSalesDetails.Rows[i]["gwt"]);
                    Swt += Convert.ToDecimal(dtSalesDetails.Rows[i]["swt"]);
                    Nwt += Convert.ToDecimal(dtSalesDetails.Rows[i]["nwt"]);
                    dicountMc += Convert.ToDecimal(dtSalesDetails.Rows[i]["mc_Discount_Amt"]);
                    StoneChg += StoneAmt;
                    ValueAmt += Convert.ToDecimal(dtSalesDetails.Rows[i]["va_amount"]);
                    if (IsPieceItem.Equals("Y"))
                        ItemGrosAmount = Convert.ToDecimal(dtSalesDetails.Rows[i]["gold_value"]) + Convert.ToDecimal(dtSalesDetails.Rows[i]["va_amount"]);
                    else

                        ItemGrosAmount = Convert.ToDecimal(dtSalesDetails.Rows[i]["gold_value"]) + StoneAmt + Convert.ToDecimal(dtSalesDetails.Rows[i]["va_amount"]);
                    totalGrosAmount += ItemGrosAmount;

                    AdditonalDiscAmount += Convert.ToDecimal(dtSalesDetails.Rows[i]["item_additional_discount"]);
                    offerDiscountAmount += Convert.ToDecimal(dtSalesDetails.Rows[i]["offer_value"]);

                    totalTaxableAmount += Convert.ToDecimal(dtSalesDetails.Rows[i]["item_total_after_discount"]);
                    TotalSGST += Convert.ToDecimal(dtSalesDetails.Rows[i]["SGST_Amount"]);
                    TotalCGST += Convert.ToDecimal(dtSalesDetails.Rows[i]["CGST_Amount"]);
                    TotalIGST += Convert.ToDecimal(dtSalesDetails.Rows[i]["IGST_Amount"]);


                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1}</TD>", ItemGrosAmount, "&nbsp")); //total_amount

                    Pcs += Convert.ToInt32(dtSalesDetails.Rows[i]["item_no"]);
                    InvoiceAmount += Convert.ToDecimal(dtSalesDetails.Rows[i]["item_total_after_discount"]);

                    if (i > MaxPageRow && (i % MaxPageRow) == 0 && i < dtSalesDetails.Rows.Count) {
                        MaxPageRow = 30;
                        sbStart.AppendLine("<TR>");
                        sbStart.AppendLine(string.Format("<TD colspan =13 ALIGN = \"RIGHT\">For {0}<br><br><br><br><br><br>{1}</TD>", companyName, "Authorized Signatory"));
                        sbStart.AppendLine("</TR>");
                        sbStart.AppendLine("</Table >");
                        sbStart.AppendLine("<DIV style=\"page-break-after:always\"></DIV>");
                        sbStart.AppendLine("<Table frame=\"border\" border=\"0\" width=\"1100\">");  //FRAME=BOX RULES=NONE

                        for (int j = 0; j < 10; j++) {
                            sbStart.AppendLine("<TR style=border:0>");
                            sbStart.AppendLine(string.Format("<TD style=border:0 colspan = 13 ALIGN = \"RIGHT\">{0}</TD>", "&nbsp"));
                            sbStart.AppendLine("</TR>");
                        }

                        sbStart.AppendLine("</table>");
                        sbStart.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; border-top:thin\" width=\"900\">");  //FRAME=BOX RULES=NONE

                        sbStart.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid ;border-top:thin \" ALIGN = \"CENTER\">Sl.No</TH>");
                        // sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin \" ALIGN = \"CENTER\"><b>C</b></TH>");
                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\">Description</TH>");
                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\">HSN Code</TH>");
                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">Purity</TH>");
                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">Rate/Gram</TH>");
                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">Qty</TH>");
                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">Gr.Wt</TH>");
                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">St.Wt</TH>");
                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">Net Wt</TH>");


                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\">Jewel Amount</TH>");
                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\">VA Amount</TH>");

                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\">Stone/Diamond Amount</TH>");
                        sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\">GrossAmount</TH>");
                        sbStart.AppendLine("</TR>");
                    }
                }

                for (int j = 0; j < MaxPageRow - dtSalesDetails.Rows.Count; j++) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD  style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                sbStart.AppendLine("<TR  bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH  style=\"border-bottom:thin solid\" colspan=5 ALIGN = \"LEFT\">Totals</TH>");

                sbStart.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\">{0}{1}{1}</TH>", Pcs, "&nbsp"));
                sbStart.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\">{0}{1}{1}</TH>", Gwt, "&nbsp"));
                sbStart.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\">{0}{1}{1}</TH>", Swt, "&nbsp"));
                sbStart.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\">{0}{1}{1}</TH>", Nwt, "&nbsp"));
                sbStart.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\">{0}{1}{1}</TH>", GoldValue, "&nbsp"));
                sbStart.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\">{0}{1}{1}</TH>", ValueAmt, "&nbsp"));
                sbStart.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\">{0}{1}{1}</TH>", StoneChg, "&nbsp"));
                sbStart.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\">{0}{1}{1}</TH>", totalGrosAmount, "&nbsp"));
                sbStart.AppendLine("</TR>");

                string Strstonedetails = string.Format("select est_Srno,qty,type,name,carrat,stone_wt,rate,amount \n"
                                        + "from KTTU_SALES_STONE_DETAILS where bill_no = {0} and company_code = '{1}' and branch_code = '{2}' order by est_Srno"
                                        , salesbillno, companyCode, branchCode);
                DataTable dtStoneDetails = Globals.GetDataTable(Strstonedetails);

                if (dtStoneDetails != null && dtStoneDetails.Rows.Count > 0) {
                    List<string> Stonelst = null;
                    string stone = string.Empty;
                    for (int j = 0; j < dtStoneDetails.Rows.Count; j++) {
                        string StoneName = dtStoneDetails.Rows[j]["name"].ToString();
                        StoneName = StoneName.Trim();
                        string[] arr = StoneName.Split('-');

                        if (j == 0 || (!dtStoneDetails.Rows[j]["est_Srno"].Equals(dtStoneDetails.Rows[j - 1]["est_Srno"]))) {
                            stone += string.Format("{0}{1}{2}{3}{4}{5}{6} ", dtStoneDetails.Rows[j]["est_Srno"], ")", arr[0], "\\", dtStoneDetails.Rows[j]["rate"], "*", dtStoneDetails.Rows[j]["carrat"]);
                        }
                        else {
                            stone += string.Format("{0}{1}{2}{3}{4} ", arr[0], "\\", dtStoneDetails.Rows[j]["rate"], "*", dtStoneDetails.Rows[j]["carrat"]);
                        }
                    }

                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=12 ALIGN = \"LEFT\">{0}</TD><TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b></b></TD>", stone));
                    sbStart.AppendLine("</TR>");

                    if (Stonelst != null && Stonelst.Count > 1) {
                        for (int x = 1; x < Stonelst.Count; x++) {
                            sbStart.AppendLine("<TR>");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=12 ALIGN = \"LEFT\">{0}{1}</TD> <TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b></b></TD><TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b></b></TD>", "- ", Stonelst[x].Trim()));
                            sbStart.AppendLine("</TR>");

                        }

                        stone = string.Empty;
                        Stonelst = null;
                    }

                }
                if (Convert.ToDecimal(AdditonalDiscAmount) > 0 || offerDiscountAmount > 0) {
                    sbStart.AppendLine("<tr>");
                    sbStart.AppendLine(string.Format("<td  style=\"border-bottom:thin; border-top:thin \"  colspan =11   ALIGN = \"right\"><b>{0}</b></td>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<td colspan =1   ALIGN = \"CENTER\">{0}</td>", "Discount"));
                    sbStart.AppendLine(string.Format("<td  colspan =1 ALIGN = \"right\">{0}</td>", AdditonalDiscAmount + offerDiscountAmount));
                    sbStart.AppendLine("</tr>");

                }
                if (Convert.ToDecimal(dicountMc) > 0) {
                    sbStart.AppendLine("<tr>");
                    sbStart.AppendLine(string.Format("<td  style=\"border-bottom:thin; border-top:thin \"  colspan =11   ALIGN = \"right\"><b>{0}</b></td>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<td colspan =1   ALIGN = \"CENTER\">{0}</td>", "Discount"));
                    sbStart.AppendLine(string.Format("<td  colspan =1 ALIGN = \"right\">{0}</td>", decimal.Round(dicountMc, 2)));
                    sbStart.AppendLine("</tr>");

                }

                if (Convert.ToDecimal(dtSalesBilling.Rows[0]["excise_duty_amount"]) > 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=12 ALIGN = \"CENTER\">Excise Duty @ {0}%</TD>", dtSalesBilling.Rows[0]["excise_duty_percent"]));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"{0}{1}{1}</TD>", dtSalesBilling.Rows[0]["excise_duty_amount"], "&nbsp"));
                    sbStart.AppendLine("</TR>");
                    InvoiceAmount += Convert.ToDecimal(dtSalesBilling.Rows[0]["excise_duty_amount"]);

                }

                string ShippingCharges = string.Format("select * from KTTU_SHIPPING_CHARGES where doc_no='{0}' and company_code='{1}' and branch_code='{2}'", salesbillno, companyCode, branchCode);
                DataTable dtShippingCharges = Globals.GetDataTable(ShippingCharges);
                decimal FinalIGST = 0, FinalCGST = 0, FinalSGST = 0, TotalShippingSGST = 0, TotalShippingCGST = 0, TotalShippingIGST = 0;
                decimal shippingChargesBeforeGST = 0;


                if (dtShippingCharges != null && dtShippingCharges.Rows.Count > 0) {

                    TotalShippingSGST = Convert.ToDecimal(dtShippingCharges.Rows[0]["SGST_Amount"]);
                    TotalShippingCGST = Convert.ToDecimal(dtShippingCharges.Rows[0]["CGST_Amount"]);
                    TotalShippingIGST = Convert.ToDecimal(dtShippingCharges.Rows[0]["IGST_Amount"]);
                    shippingChargesBeforeGST = Convert.ToDecimal(dtShippingCharges.Rows[0]["shippingChargesBeforeGST"]);
                }
                FinalSGST = TotalShippingSGST + TotalSGST;
                FinalCGST = TotalShippingCGST + TotalCGST;
                FinalIGST = TotalShippingIGST + TotalIGST;


                sbStart.AppendLine("<tr>");
                sbStart.AppendLine(string.Format("<td  style=\"border-bottom:thin; border-top:thin \" colspan =11   ALIGN = \"right\"><b>{0}</b></td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1   ALIGN = \"CENTER\">{0}</td>", "Taxable Value"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", totalTaxableAmount));
                sbStart.AppendLine("</tr>");

                if (dtShippingCharges != null && dtShippingCharges.Rows.Count > 0) {

                    sbStart.AppendLine("<tr>");
                    sbStart.AppendLine(string.Format("<td  style=\"border-bottom:thin; border-top:thin \" colspan =11   ALIGN = \"right\"><b>{0}</b></td>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<td colspan =1   ALIGN = \"CENTER\">{0}</td>", "Shipping Charges"));
                    sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", dtShippingCharges.Rows[0]["shippingChargesBeforeGST"]));
                    sbStart.AppendLine("</tr>");
                }

                sbStart.AppendLine("<tr>");
                sbStart.AppendLine(string.Format("<td style=\"border-bottom:thin; border-top:thin \" colspan =11   ALIGN = \"right\"><b>{0}</b></td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1   ALIGN = \"CENTER\">{0} </td>", "SGST"));
                sbStart.AppendLine(string.Format("<td  colspan =1 ALIGN = \"right\">{0}</td>", decimal.Round(FinalSGST, 2)));
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr>");
                sbStart.AppendLine(string.Format("<td style=\"border-bottom:thin; border-top:thin \"  colspan =11   ALIGN = \"right\"><b>{0}</b></td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1   ALIGN = \"CENTER\">{0} </td>", "CGST"));
                sbStart.AppendLine(string.Format("<td  colspan =1 ALIGN = \"right\">{0}</td>", decimal.Round(FinalCGST, 2)));
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr>");
                sbStart.AppendLine(string.Format("<td  style=\"border-bottom:thin; border-top:thin \"  colspan =11   ALIGN = \"right\"><b>{0}</b></td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1   ALIGN = \"CENTER\">{0} </td>", "IGST"));
                sbStart.AppendLine(string.Format("<td  colspan =1 ALIGN = \"right\">{0}</td>", decimal.Round(FinalIGST, 2)));
                sbStart.AppendLine("</tr>");

                InvoiceAmount += TotalIGST + TotalSGST + TotalCGST;
                TotalShippingGST = TotalShippingCGST + TotalShippingIGST + TotalShippingSGST;
                decimal FinalShippingCharge = shippingChargesBeforeGST + TotalShippingGST;
                decimal GrandTotalWithShippingCharge = InvoiceAmount + shippingChargesBeforeGST + TotalShippingGST;
                decimal round_off = Convert.ToDecimal(dtSalesBilling.Rows[0]["round_off"]);
                InvoiceAmount += round_off;
                decimal rndOff = Math.Round(InvoiceAmount, 0, MidpointRounding.ToEven) - InvoiceAmount;
                if (Math.Abs(rndOff) <= 0.02M) {
                    round_off += rndOff; //This is not necessary since it is taken care of in the sum.
                    InvoiceAmount = Math.Round(InvoiceAmount, 0, MidpointRounding.ToEven);
                }
                decimal InvoiceAmountInWords = InvoiceAmount;

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine(string.Format("<td   colspan =9   ALIGN = \"CENTER\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td  colspan =3   ALIGN = \"CENTER\"><b>{0}</b></td>", "Grand Total"));
                sbStart.AppendLine(string.Format("<td   colspan =1 ALIGN = \"right\"><b>{0}</b></td>", GrandTotalWithShippingCharge));
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<Table frame=\"border\" border=\"0\" width=\"900\">");  //FRAME=BOX RULES=NONE
                sbStart.AppendLine("<TR style=border:0;\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TD style=border:0 width=\"900\" colspan=0 ALIGN = \"left\"><b>Summary</b></TD>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table>");

                sbStart.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; border-top:thin\" width=\"900\">");  //FRAME=BOX RULES=NONE

                sbStart.AppendLine("<tr>");
                sbStart.AppendLine(string.Format("<td colspan =3   ALIGN = \"CENTER\">{0}</td>", "HSN/HSC"));
                sbStart.AppendLine(string.Format("<td colspan =2   ALIGN = \"CENTER\">{0}</td>", "Taxable Amount"));
                sbStart.AppendLine(string.Format("<td colspan =2 ALIGN = \"CENTER\">{0}</td>", "IGST"));
                sbStart.AppendLine(string.Format("<td colspan =2 ALIGN = \"CENTER\">{0}</td>", "CGST"));
                sbStart.AppendLine(string.Format("<td colspan =2 ALIGN = \"CENTER\">{0}</td>", "SGST"));
                sbStart.AppendLine(string.Format("<td colspan =2 ALIGN = \"CENTER\">{0}</td>", "Total Amount"));
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr>");
                sbStart.AppendLine(string.Format("<td colspan =3   ALIGN = \"CENTER\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =2   ALIGN = \"right\"><b>{0}</b></td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", "Rate(%)"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", "Amount"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", "Rate(%)"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", "Amount"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", "Rate(%)"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", "Amount"));
                sbStart.AppendLine(string.Format("<td colspan =2 ALIGN = \"CENTER\">{0}</td>", "&nbsp"));
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr>");
                sbStart.AppendLine(string.Format("<td colspan =3   ALIGN = \"CENTER\">{0}</td>", dtSalesDetails.Rows[0]["HSN"]));
                sbStart.AppendLine(string.Format("<td colspan =2   ALIGN = \"RIGHT\">{0}</td>", totalTaxableAmount));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", dtSalesDetails.Rows[0]["IGST_Percent"]));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"RIGHT\">{0}</td>", TotalIGST));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", dtSalesDetails.Rows[0]["CGST_Percent"]));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"RIGHT\">{0}</td>", TotalCGST));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", dtSalesDetails.Rows[0]["SGST_Percent"]));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", TotalSGST));
                sbStart.AppendLine(string.Format("<td colspan =2 ALIGN = \"right\">{0}</td>", InvoiceAmount));
                sbStart.AppendLine("</tr>");

                if (dtShippingCharges != null && dtShippingCharges.Rows.Count > 0) {
                    sbStart.AppendLine("<tr>");
                    sbStart.AppendLine(string.Format("<td colspan =3   ALIGN = \"CENTER\">{0}</td>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<td colspan =2   ALIGN = \"RIGHT\">{0}</td>", shippingChargesBeforeGST));
                    sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", dtShippingCharges.Rows[0]["IGST_Percent"]));
                    sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"RIGHT\">{0}</td>", TotalShippingIGST));
                    sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", dtShippingCharges.Rows[0]["CGST_Percent"]));
                    sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"RIGHT\">{0}</td>", TotalShippingCGST));
                    sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"CENTER\">{0}</td>", dtShippingCharges.Rows[0]["SGST_Percent"]));
                    sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"RIGHT\">{0}</td>", TotalShippingSGST));
                    sbStart.AppendLine(string.Format("<td colspan =2 ALIGN = \"right\">{0}</td>", FinalShippingCharge));
                    sbStart.AppendLine("</tr>");
                }

                sbStart.AppendLine("<tr>");
                sbStart.AppendLine(string.Format("<td colspan =3   ALIGN = \"CENTER\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =2   ALIGN = \"right\"><b>{0}</b></td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =2 ALIGN = \"right\">{0}</td>", "&nbsp"));
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr>");
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =5  ALIGN = \"CENTER\"><b>{0}</b></td>", "TOTAL"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\"><b>{0}</b></td>", FinalIGST));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\"><b>{0}</b></td>", FinalCGST));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\">{0}</td>", "&nbsp"));
                sbStart.AppendLine(string.Format("<td colspan =1 ALIGN = \"right\"><b>{0}</b></td>", FinalSGST));
                sbStart.AppendLine(string.Format("<td colspan =2 ALIGN = \"right\"><b>{0}</b></td>", GrandTotalWithShippingCharge));
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("</Table>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"0\"; style=\"border-collapse:collapse; border-top:thin\" width=\"900\">");  //FRAME=BOX RULES=NONE
                sbStart.AppendLine("<TR style=border:0;\"color:black; text-decoration:bold;\" align=\"right\" style=border-right-style:none>");
                sbStart.AppendLine(string.Format("<TD style=border:0  ALIGN = \"right\" style=border-right-style:none><b>For {0}</b></TD>", companyName));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<td style=border-right-style:none>");
                sbStart.AppendLine("<Table  >");
                sbStart.AppendLine("<TR style=border:0;\"color:black; text-decoration:bold;\" align=\"left\" style=border-right-style:none >");
                sbStart.AppendLine(string.Format("<td style=border-right-style:none >"));
                sbStart.AppendLine("<div id=\"imagefooter\">");
                sbStart.AppendLine(WaterMarkStyle("1.5", "100", "#imagefooter"));
                sbStart.AppendLine(string.Format("<img  src=\"{0}\" width=\"450\" />", footerImg));
                sbStart.AppendLine(string.Format("</td>"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table>");
                sbStart.AppendLine("</td>");

                sbStart.AppendLine("<TR style=border:0;\"color:black; text-decoration:bold;\" align=\"right\" style=border-right-style:none>");
                sbStart.AppendLine(string.Format("<TD style=border:0  ALIGN = \"right\" style=border-right-style:none><b> {0}</b></TD>", "Authorized Signatory"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("</TABLE>");
                sbStart.AppendLine("</Div>");
                sbStart.AppendLine("</body>");
                sbStart.AppendLine("</html>");
                sbStart.AppendLine();
                sbHTML.AppendLine(sbStart.ToString());
            }
            catch (Exception) {
                throw;
            }
            return sbHTML.ToString();
        }

        private string WaterMarkStyle(string opacity1, string opacity2, string id)
        {
            StringBuilder sbStyle = new StringBuilder();
            if (string.IsNullOrEmpty(sbStyle.ToString())) {
                #region Fill Style
                string OP1 = "opacity:" + opacity1 + ";\n";
                string OP2 = "filter: alpha(opacity=" + opacity2 + ");";
                string ID1 = id + "\n";
                string ID2 = id + " img\n";

                sbStyle.AppendLine("<style = 'text/css'>\n");
                sbStyle.AppendLine(ID1);
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("height: 110px;\n");
                sbStyle.AppendLine("width: 300px;\n");
                sbStyle.AppendLine("background-position:center;\n");
                sbStyle.AppendLine("background-repeat: no-repeat;\n");
                sbStyle.AppendLine("position: relative;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(ID2);
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("position: absolute;\n");
                sbStyle.AppendLine(OP1);
                sbStyle.AppendLine("top: 0;\n");
                sbStyle.AppendLine("left: 0;\n");
                sbStyle.AppendLine("bottom: 0;\n");
                sbStyle.AppendLine("right: 0;\n");
                sbStyle.AppendLine("position: absolute;");
                sbStyle.AppendLine(OP2);
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine("</style>\n");
                #endregion
            }
            return sbStyle.ToString();
        }

        public byte[] ConvertHtmlToPdf(string htmlContent)
        {
            try {
                HtmlToPdfConverter htmlToPdfConverter = InitializeHtmlToPdfConverter();
                var pdfBytes = htmlToPdfConverter.GeneratePdf(htmlContent);

                return pdfBytes;
            }
            catch (Exception) {
                throw;
            }
        }

        public bool ConvertHtmlToPdf(string htmlContent, string pdfFilePath)
        {
            try {
                File.WriteAllBytes(pdfFilePath, ConvertHtmlToPdf(htmlContent));
                return true;
            }
            catch (Exception) {
                throw;
            }
        }

        private HtmlToPdfConverter InitializeHtmlToPdfConverter()
        {
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();
            var margins = new PageMargins
            {
                Top = 10.00f,
                Bottom = 5.00f,
                Left = 10.00f,
                Right = 10.00f
            };
            htmlToPdfConverter.Margins = margins;
            return htmlToPdfConverter;
        }
    }
}


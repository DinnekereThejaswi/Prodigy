using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// 29/08/2020
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    /// <summary>
    /// All Ledger Realted Transactions
    /// </summary>
    public class LedgerBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        private const string MODULE_SEQ_NO = "02";
        private const string TABLE_NAME = "KSTU_ACC_LEDGER_MASTER";
        #endregion

        #region Methods
        /// <summary>
        /// Get all ledger master details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public IQueryable GetLedgerMasterDetails(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.usp_ACC_LedgerMaster_Load(companyCode, branchCode, "G", "").AsQueryable();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public LedgerMasterVM GetLedgerMasterDetails(int accCode, string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.KSTU_ACC_LEDGER_MASTER.Where(led => led.company_code == companyCode && led.branch_code == branchCode && led.acc_code == accCode)
                                            .Select(s => new LedgerMasterVM
                                            {
                                                ObjID = s.obj_id,
                                                CompanyCode = s.company_code,
                                                BranchCode = s.branch_code,
                                                AccCode = s.acc_code,
                                                AccName = s.acc_name,
                                                AccType = s.acc_type,
                                                GroupID = s.group_id,
                                                OpeBal = s.opn_bal,
                                                OpnBalType = s.opn_bal_type,
                                                CustID = s.cust_id,
                                                PartyCode = s.party_code,
                                                GSCode = s.gs_code,
                                                ObjStatus = s.obj_status,
                                                GSSeqNo = s.gs_seq_no,
                                                SchemeCode = s.scheme_code,
                                                VATID = s.vat_id,
                                                OdLimit = s.od_limit,
                                                JFlag = s.jflag,
                                                PANCard = s.pancard,
                                                TIN = s.TIN,
                                                LedgerType = s.ledger_type,
                                                Address1 = s.address1,
                                                Address2 = s.address2,
                                                Address3 = s.address3,
                                                City = s.city,
                                                State = s.state,
                                                District = s.district,
                                                Country = s.country,
                                                PinCode = s.pincode,
                                                Phone = s.phone,
                                                Mobile = s.mobile,
                                                FaxNo = s.FaxNo,
                                                WebSite = s.website,
                                                CSTNo = s.cst_no,
                                                BudgetAmt = s.budget_amt,
                                                TDSID = s.tds_id,
                                                EmailID = s.email_id,
                                                HSN = s.HSN,
                                                GSTGoodsGroupCode = s.GSTGoodsGroupCode,
                                                GSTServicesGroupCode = s.GSTServicesGroupCode,
                                                StateCode = s.state_code,
                                                VTYPE = s.V_TYPE,
                                                TransType = s.transType,
                                                IsAutomatic = s.IsAutomatic,
                                                Schedule_Name = s.Schedule_Name,
                                                NewAccCode = s.NewAccCode,
                                                PartyACNo = s.Party_AC_No,
                                                PartyACName = s.Party_AC_Name,
                                                PartyMICR_No = s.Party_MICR_No,
                                                PartyBankName = s.Party_BankName,
                                                PartyBankBranch = s.Party_BankBranch,
                                                PartyBankAddress = s.Party_BankAddress,
                                                PartyRTGScode = s.Party_RTGScode,
                                                PartyNEFTcode = s.Party_NEFTcode,
                                                PartyIFSCcode = s.Party_IFSCcode,
                                                swiftcode = s.swift_code
                                            }).FirstOrDefault();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<LedgerTypeVM> GetLedgerType(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.vAccountsVoucherTypes.Select(t => new LedgerTypeVM()
                {
                    Code = t.code,
                    Name = t.name
                }).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<SubGroupVM> GetSubGroup(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.KSTU_ACC_GROUP_MASTER.Where(g => g.company_code == companyCode && g.branch_code == branchCode && g.parent_group_id > 0)
                .Select(g => new SubGroupVM()
                {
                    GroupID = g.group_id,
                    GroupName = g.group_name
                }).OrderBy(g => g.GroupName).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public string GetGroup(string companyCode, string branchCode, int subGroupID, out ErrorVM error)
        {
            error = null;
            try {
                KSTU_ACC_GROUP_MASTER accGroupMaster = db.KSTU_ACC_GROUP_MASTER.Where(kag => kag.company_code == companyCode && kag.branch_code == branchCode && kag.group_id == subGroupID).FirstOrDefault();
                int groupID = accGroupMaster == null ? 0 : Convert.ToInt32(accGroupMaster.parent_group_id);
                var result = db.KSTU_ACC_GROUP_MASTER.Where(kagm => kagm.company_code == companyCode && kagm.branch_code == branchCode && kagm.group_id == groupID).FirstOrDefault().group_name;
                return result.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetTransactionType(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.usp_LoadTransType(companyCode, branchCode).Select(t => new { Code = t.trans_code, Name = t.trans_name }).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<GSTGroupCodeVM> GetGSTGroupCode(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.GSTGroups.Where(g => g.GSTGroupType == "Goods" && (g.IsActive == null || g.IsActive == true))
                .OrderBy(g => g.SortOrder)
                .Select(g => new GSTGroupCodeVM()
                {
                    Code = g.Code,
                    Type = g.Code + "-" + g.Description
                }).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<GSTGroupCodeVM> GetHSNByGSTGroupCode(string companyCode, string branchCode, int gstGroupCode, out ErrorVM error)
        {
            error = null;
            try {
                string strGstGroupCode = Convert.ToString(gstGroupCode);
                return db.HSNSUCs.Where(g => g.company_code == companyCode && g.branch_code == branchCode && g.GSTGroupCode == strGstGroupCode)
                .Select(g => new GSTGroupCodeVM()
                {
                    Code = g.Code,
                    Type = g.Code + "-" + g.Description
                }).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<GSTGroupCodeVM> GetGSTServiceGroupCode(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.GSTGroups.Where(g => g.GSTGroupType == "Service" && (g.IsActive == null || g.IsActive == true))
                .OrderBy(g => g.SortOrder)
                .Select(g => new GSTGroupCodeVM()
                {
                    Code = g.Code,
                    Type = g.Code + "-" + g.Description
                }).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetHSNNAC(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.HSNSUCs.Select(h => new { Code = h.Code, Description = h.Description }).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<TDSVM> GetTDS(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.KSTS_TDS_MASTER.Where(tds => tds.company_code == companyCode && tds.branch_code == branchCode && tds.obj_status != "C")
                .Select(t => new TDSVM()
                {
                    TDSID = t.tds_id,
                    TDSName = t.tds_name
                }).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<ScheduleMasteVM> GetScheduleType(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.KSTS_SCHEDULE_MASTER.Where(s => s.company_code == companyCode && s.branch_code == branchCode)
                .Select(s => new ScheduleMasteVM()
                {
                    company_code = s.company_code,
                    branch_code = s.branch_code,
                    obj_id = s.obj_id,
                    obj_status = s.obj_status,
                    Schedule_Name = s.Schedule_Name,
                    Schedule_Type = s.Schedule_Type,
                    UniqRowID = s.UniqRowID
                }).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetAccountTransactinType(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.AccTransactionTypes.Select(a => new { Code = a.Trans_Code, Name = a.Trans_Name }).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool SaveLedgerDetails(LedgerMasterVM ledger, out ErrorVM error)
        {
            error = null;
            try {
                int accCode = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(db, ledger.CompanyCode, ledger.BranchCode).ToString().Remove(0, 1) +
                    db.KSTS_ACC_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO
                                                && sq.company_code == ledger.CompanyCode
                                                && sq.branch_code == ledger.BranchCode).FirstOrDefault().nextno);

                KSTU_ACC_LEDGER_MASTER kalm = new KSTU_ACC_LEDGER_MASTER();
                kalm.obj_id = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, accCode, ledger.CompanyCode, ledger.BranchCode);
                kalm.company_code = ledger.CompanyCode;
                kalm.branch_code = ledger.BranchCode;
                kalm.acc_code = accCode;
                kalm.acc_name = ledger.AccName;
                kalm.acc_type = ledger.AccType;
                kalm.group_id = ledger.GroupID;
                kalm.opn_bal = ledger.OpeBal;
                kalm.opn_bal_type = ledger.OpnBalType;
                kalm.cust_id = ledger.CustID;
                kalm.party_code = ledger.PartyCode;
                kalm.gs_code = ledger.GSCode;
                kalm.obj_status = ledger.ObjStatus;
                kalm.gs_seq_no = ledger.GSSeqNo;
                kalm.scheme_code = ledger.SchemeCode;
                kalm.vat_id = ledger.VATID;
                kalm.od_limit = ledger.OdLimit;
                kalm.UpdateOn = SIGlobals.Globals.GetDateTime();
                kalm.jflag = ledger.JFlag;
                kalm.pancard = ledger.PANCard;
                kalm.TIN = ledger.TIN;
                kalm.ledger_type = ledger.LedgerType;
                kalm.address1 = ledger.Address1;
                kalm.address2 = ledger.Address2;
                kalm.address3 = ledger.Address3;
                kalm.city = ledger.City;
                kalm.state = ledger.State;
                kalm.district = ledger.District;
                kalm.country = ledger.Country;
                kalm.pincode = ledger.PinCode;
                kalm.phone = ledger.Phone;
                kalm.mobile = ledger.Mobile;
                kalm.FaxNo = ledger.FaxNo;
                kalm.website = ledger.WebSite;
                kalm.cst_no = ledger.CSTNo;
                kalm.budget_amt = ledger.BudgetAmt;
                kalm.tds_id = ledger.TDSID;
                kalm.email_id = ledger.EmailID;
                kalm.HSN = ledger.HSN;
                kalm.GSTGoodsGroupCode = ledger.GSTGoodsGroupCode;
                kalm.GSTServicesGroupCode = ledger.GSTServicesGroupCode;
                kalm.state_code = ledger.StateCode;
                kalm.V_TYPE = ledger.VTYPE;
                kalm.UniqRowID = Guid.NewGuid();
                kalm.transType = ledger.TransType;
                kalm.IsAutomatic = ledger.IsAutomatic;
                kalm.Schedule_Name = ledger.Schedule_Name;
                kalm.NewAccCode = ledger.NewAccCode;
                kalm.Party_AC_No = ledger.PartyACNo;
                kalm.Party_AC_Name = ledger.PartyACName;
                kalm.Party_MICR_No = ledger.PartyMICR_No;
                kalm.Party_BankName = ledger.PartyBankName;
                kalm.Party_BankBranch = ledger.PartyBankBranch;
                kalm.Party_BankAddress = ledger.PartyBankAddress;
                kalm.Party_RTGScode = ledger.PartyRTGScode;
                kalm.Party_NEFTcode = ledger.PartyNEFTcode;
                kalm.Party_IFSCcode = ledger.PartyIFSCcode;
                kalm.swift_code = ledger.swiftcode;
                db.KSTU_ACC_LEDGER_MASTER.Add(kalm);
                SIGlobals.Globals.UpdateAccountSeqenceNumber(db, MODULE_SEQ_NO, ledger.CompanyCode, ledger.BranchCode);
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool UpdateLedgerDetails(string objID, LedgerMasterVM ledger, out ErrorVM error)
        {
            error = null;
            try {
                KSTU_ACC_LEDGER_MASTER kalm = db.KSTU_ACC_LEDGER_MASTER.Where(led => led.obj_id == objID
                                                                                && led.company_code == ledger.CompanyCode
                                                                                && led.branch_code == ledger.BranchCode).FirstOrDefault();
                kalm.acc_name = ledger.AccName;
                kalm.acc_type = ledger.AccType;
                kalm.group_id = ledger.GroupID;
                kalm.opn_bal = ledger.OpeBal;
                kalm.opn_bal_type = ledger.OpnBalType;
                kalm.cust_id = ledger.CustID;
                kalm.party_code = ledger.PartyCode;
                kalm.gs_code = ledger.GSCode;
                kalm.obj_status = ledger.ObjStatus;
                kalm.gs_seq_no = ledger.GSSeqNo;
                kalm.scheme_code = ledger.SchemeCode;
                kalm.vat_id = ledger.VATID;
                kalm.od_limit = ledger.OdLimit;
                kalm.UpdateOn = SIGlobals.Globals.GetDateTime();
                kalm.jflag = ledger.JFlag;
                kalm.pancard = ledger.PANCard;
                kalm.TIN = ledger.TIN;
                kalm.ledger_type = ledger.LedgerType;
                kalm.address1 = ledger.Address1;
                kalm.address2 = ledger.Address2;
                kalm.address3 = ledger.Address3;
                kalm.city = ledger.City;
                kalm.state = ledger.State;
                kalm.district = ledger.District;
                kalm.country = ledger.Country;
                kalm.pincode = ledger.PinCode;
                kalm.phone = ledger.Phone;
                kalm.mobile = ledger.Mobile;
                kalm.FaxNo = ledger.FaxNo;
                kalm.website = ledger.WebSite;
                kalm.cst_no = ledger.CSTNo;
                kalm.budget_amt = ledger.BudgetAmt;
                kalm.tds_id = ledger.TDSID;
                kalm.email_id = ledger.EmailID;
                kalm.HSN = ledger.HSN;
                kalm.GSTGoodsGroupCode = ledger.GSTGoodsGroupCode;
                kalm.GSTServicesGroupCode = ledger.GSTServicesGroupCode;
                kalm.state_code = ledger.StateCode;
                kalm.V_TYPE = ledger.VTYPE;
                kalm.UniqRowID = Guid.NewGuid();
                kalm.transType = ledger.TransType;
                kalm.IsAutomatic = ledger.IsAutomatic;
                kalm.Schedule_Name = ledger.Schedule_Name;
                kalm.NewAccCode = ledger.NewAccCode;
                kalm.Party_AC_No = ledger.PartyACNo;
                kalm.Party_AC_Name = ledger.PartyACName;
                kalm.Party_MICR_No = ledger.PartyMICR_No;
                kalm.Party_BankName = ledger.PartyBankName;
                kalm.Party_BankBranch = ledger.PartyBankBranch;
                kalm.Party_BankAddress = ledger.PartyBankAddress;
                kalm.Party_RTGScode = ledger.PartyRTGScode;
                kalm.Party_NEFTcode = ledger.PartyNEFTcode;
                kalm.Party_IFSCcode = ledger.PartyIFSCcode;
                kalm.swift_code = ledger.swiftcode;
                db.Entry(kalm).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public string PrintLedgerDetailsHTML(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                string legder = string.Empty;
                List<LedgerBalance> ledger = db.LedgerBalances.Where(led => led.company_code == companyCode
                                                                        && led.branch_code == branchCode)
                                                              .OrderBy(led => new { led.SubGroupName, led.group }).ToList();
                StringBuilder sb = new StringBuilder();
                if (ledger == null || ledger.Count == 0) {
                    return "";
                }
                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine(GetStyle());
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");

                //int colspan = 4;
                sb.AppendLine("<table bgcolor=\"#FDFDF0\" class=\"boldText\" frame=\"border\" border=\"2\" width=\"800\" align=\"left\" >");  //FRAME=BOX RULES=NONE
                sb.AppendLine("<tr>");
                //sb.AppendLine(string.Format("<td  class=\"noborder\" colspan={1} align=\"center\" bgcolor=\"EBE7C6\" ><b style=\"color:'maroon'\">{0}</b></td>", FillCompanyDetails("Company_Name").ToString().Trim(), colspan));
                sb.AppendLine("</tr>");


                sb.AppendLine("<tr bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"center\">");
                sb.AppendLine("<td  width=\"200\" align=\"left\"><b>Account Code</b></td>");
                sb.AppendLine("<td  width=\"400\" align=\"left\"><b>Account Name</b></td>");
                sb.AppendLine("<td  width=\"100\" align=\"right\"><b>Debit</b></td>");
                sb.AppendLine("<td  width=\"100\" align=\"right\"><b>Credit</b></td>");
                sb.AppendLine("</tr>");

                decimal debit = 0;
                decimal credit = 0;
                decimal subCredit = 0;
                decimal subDebit = 0;
                string name = string.Empty;

                var lstOfGroup = (from lm in ledger
                                  group lm by new
                                  {
                                      lm.@group,
                                      lm.SubGroupName
                                  } into gcs
                                  select gcs).ToList();

                for (int s = 0; s < lstOfGroup.Count; s++) {
                    if (s > 0) {
                        if (string.Compare(lstOfGroup[s].Key.SubGroupName, lstOfGroup[s - 1].Key.SubGroupName.ToString()) != 0) {
                            string subGroupName = ledger[s - 1].SubGroupName.ToString();
                            object GroupCR = ledger.Where(l => l.SubGroupName == subGroupName).Sum(lsum => lsum.CR); // ledger.Compute("Sum(cr)", string.Format("SubGroupName in('{0}')", dtLed.Rows[s - 1][1].ToString()));
                            if (GroupCR != null && GroupCR != DBNull.Value)
                                subCredit = Convert.ToDecimal(GroupCR);

                            object GroupDR = ledger.Where(l => l.SubGroupName == subGroupName).Sum(lsum => lsum.DR);// dtLegder.Compute("Sum(dr)", string.Format("SubGroupName in('{0}')", dtLed.Rows[s - 1][1].ToString()));
                            if (GroupDR != null && GroupDR != DBNull.Value)
                                subDebit = Convert.ToDecimal(GroupDR);

                            //name = CGlobals.GetStringValue(string.Format("select g.group_name from kstu_Acc_group_master g,kstu_acc_group_master sg where g.group_id=sg.parent_group_id and sg.group_name='{0}'", (lstOfGroup[s].Key.group)));
                            name = ledger.Where(led => led.group == lstOfGroup[s].Key.group).FirstOrDefault().SubGroupName;
                            sb.AppendLine(string.Format("<tr bgcolor='#FFFACD'><td bgcolor='#F5DEB3' style=\"color:Brown; class=\"noborder\" colspan=8 align=\"center\" ><b>{0}</b></td></tr>", name));

                        }
                        sb.AppendLine(string.Format("<td bgcolor='#FFFACD' style=\"color:black; class=\"noborder\" colspan=8 align=\"left\" ><b>{0}</b></td>", lstOfGroup[s].Key.group));
                    }
                    else {
                        //name = CGlobals.GetStringValue(string.Format("select g.group_name from kstu_Acc_group_master g,kstu_acc_group_master sg where g.group_id=sg.parent_group_id and sg.group_name='{0}'", (dtLed.Rows[s][0].ToString())));
                        name = ledger.Where(led => led.group == lstOfGroup[s].Key.group).FirstOrDefault().SubGroupName;
                        sb.AppendLine(string.Format("<tr bgcolor='#FFFACD'><td bgcolor='#F5DEB3' style=\"color:Brown; class=\"noborder\" colspan=8 align=\"center\" ><b> {0}</b></td></tr>", name));
                        sb.AppendLine(string.Format("<tr><td bgcolor='#FFFACD' style=\"color:black; class=\"noborder\" colspan=8 align=\"left\" ><b> {0}</b></td></tr>", (lstOfGroup[s].Key.group)));
                    }
                    if (lstOfGroup[s].Key.group != null) {
                        string temp = lstOfGroup[s].Key.group.ToString();
                        temp = temp.Replace("'", " ");
                        var filterd = ledger.Where(l => l.group == temp).ToList();
                        for (int i = 0; i < filterd.Count; i++) {
                            sb.AppendLine(string.Format("<tr><td align = \"left\">{0}</td>", filterd[i].acc_code));
                            sb.AppendLine(string.Format("<td align = \"left\">{0}</td>", filterd[i].AName));
                            sb.AppendLine(string.Format("<td align = \"right\">{0}</td>", filterd[i].DR));
                            sb.AppendLine(string.Format("<td align = \"right\">{0}</td>", filterd[i].CR));
                            sb.AppendLine("</tr>");
                        }
                    }

                    object subGroupCR = ledger.Where(led => led.group == lstOfGroup[s].Key.group).Sum(total => total.CR);// dtLegder.Compute("Sum(cr)", string.Format("group in('{0}')", dtLed.Rows[s][0].ToString()));
                    if (subGroupCR != null && subGroupCR != DBNull.Value)
                        subCredit = Convert.ToDecimal(subGroupCR);

                    object subGroupDR = ledger.Where(led => led.group == lstOfGroup[s].Key.group).Sum(total => total.DR); //dtLegder.Compute("Sum(dr)", string.Format("group in('{0}')", dtLed.Rows[s][0].ToString()));
                    if (subGroupDR != null && subGroupDR != DBNull.Value)
                        subDebit = Convert.ToDecimal(subGroupDR);


                    sb.AppendLine(string.Format("<tr><td COLSPAN=2 bgcolor='#FFFACD' style=\"color:black; class=\"noborder\"  align=\"left\" ><b> {0}</b></td> <td bgcolor='#FFFACD' style=\"color:black; class=\"noborder\"  align=\"right\">{1}</td><td bgcolor='#FFFACD' style=\"color:black; class=\"noborder\"  align=\"right\" >{2}</td><tr>", "SUB GROUP TOTAL"
                        , subDebit.ToString("N"), subCredit.ToString("N")));

                    if (s + 1 == lstOfGroup.Count) {
                        object GroupCR = ledger.Where(led => led.SubGroupName == lstOfGroup[s].Key.SubGroupName).Sum(total => total.CR); // dtLegder.Compute("Sum(cr)", string.Format("SubGroupName in('{0}')", dtLed.Rows[s][1].ToString()));
                        if (GroupCR != null && GroupCR != DBNull.Value)
                            subCredit = Convert.ToDecimal(GroupCR);
                        object GroupDR = ledger.Where(led => led.SubGroupName == lstOfGroup[s].Key.SubGroupName).Sum(total => total.DR); //dtLegder.Compute("Sum(dr)", string.Format("SubGroupName in('{0}')", dtLed.Rows[s][1].ToString()));
                        if (GroupDR != null && GroupDR != DBNull.Value)
                            subDebit = Convert.ToDecimal(GroupDR);
                    }
                }

                object tempdebit = ledger.Where(led => led.SubGroupName != "").Sum(total => total.DR);// dtLegder.Compute("Sum(DR)", "SubGroupName not in('')");
                if (tempdebit != null && tempdebit != DBNull.Value)
                    debit = Convert.ToDecimal(tempdebit);

                object tempcredit = ledger.Where(led => led.SubGroupName != "").Sum(total => total.CR);// dtLegder.Compute("Sum(CR)", "SubGroupName not in('')");
                if (tempcredit != null && tempcredit != DBNull.Value)
                    credit = Convert.ToDecimal(tempcredit);

                if (ledger.Count > 1) {
                    sb.AppendLine(string.Format("<tr  bgcolor='FFFACD'style=\"color:black; text-decoration:bold\" class = \"tableStyle\" > <td COLSPAN=2 class=\"fullborderStyle\" align=\"left\"><b>{0}</b></td><td class=\"fullborderStyle\" align=\"right\"><b>{1}</b></td> <td class=\"fullborderStyle\" align=\"right\"><b>{2}</b></td>",
                     "Grand Total", debit.ToString("N"), credit.ToString("N")));
                }
                sb.AppendLine("</TABLE>");
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        /// <summary>
        /// Don't know why this Method written. Not using anywhere.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        public dynamic LedgerStockType(string companyCode, string branchCode)
        {
            return db.vLedgerStockTypes.Select(l => new { Type = l.StockType, Code = l.Stock_Code }).ToList();
        }
        #endregion

        #region Private Methods
        private static string GetStyle()
        {
            StringBuilder sbStyle = new StringBuilder();
            if (string.IsNullOrEmpty(sbStyle.ToString())) {
                #region Fill Style
                sbStyle.AppendLine("<style = 'text/css'>\n");
                sbStyle.AppendLine(".boldText, .boldtable TD, .boldtable TH\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("font-family: 'Times New Roman';\n");
                sbStyle.AppendLine("font-size:12pt;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".tableStyle\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left: 1px solid black;\n");
                sbStyle.AppendLine("border-right: 1px solid black;\n");
                sbStyle.AppendLine("border-top-style:none;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".table,TD,TH\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left-style:none;\n");
                sbStyle.AppendLine("border-right: 1px solid black;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-top-style:1px solid;\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".noborder\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left-style:none;\n");
                sbStyle.AppendLine("border-right-style:none;\n");
                sbStyle.AppendLine("border-top-style:none;;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".fullborder\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left-style:none;;\n");
                sbStyle.AppendLine("border-right: 1px solid black;\n");
                sbStyle.AppendLine("border-bottom: 1px solid black;\n");
                sbStyle.AppendLine("border-top: 1px solid black;\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine("</style>\n");
                #endregion
            }
            return sbStyle.ToString();
        }
        #endregion
    }
}

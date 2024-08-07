using NumberToWordsINR;
using ProdigyAPI.BL.BusinessLayer.Common;
using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.BL.ViewModel.Repair;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Repair
{
    public class RepairBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        private const string MODULE_SEQ_NO = "8";
        private const string ISSUE_MODULE_SEQ_NO = "7";
        private const string TABLE_NAME_RECEIPT = "KTTU_REPAIR_RECEIPT_MASTER";
        private const string TABLE_NAME_ISSUE = "KTTU_REPAIR_ISSUE_MASTER";
        #endregion

        #region Receipt
        public List<RepairGSVM> GetRepairGS(string companyCode, string branchCode)
        {
            List<RepairGSVM> lstRepariGS = new List<RepairGSVM>();
            lstRepariGS = (from items in db.KSTS_GS_ITEM_ENTRY
                           where items.bill_type == "R" && items.company_code == companyCode
                                && items.branch_code == branchCode && items.object_status != "C"
                                && (items.measure_type == "P" || items.measure_type == "W")
                           orderby items.display_order
                           select new RepairGSVM()
                           {
                               GSName = items.item_level1_name,
                               GSCode = items.gs_code
                           }).ToList();
            return lstRepariGS;
        }

        public List<RepairGSItemVM> GetRepairGSItems(string gsCode, string companyCode, string branchCode)
        {
            List<RepairGSItemVM> lstRepariGSItem = new List<RepairGSItemVM>();
            lstRepariGSItem = (from items in db.ITEM_MASTER
                               where items.gs_code == gsCode && items.company_code == companyCode
                                    && items.branch_code == branchCode && items.Item_code != "HIOG" && items.obj_status != "C"
                               orderby items.Item_code
                               select new RepairGSItemVM()
                               {
                                   ItemName = items.Item_code,
                                   ItemType = items.Item_Name,
                                   CodeName = items.Item_code + "-" + items.Item_Name
                               }).ToList();
            return lstRepariGSItem;
        }

        public RepairReceiptMasterVM GetReceiptDetails(string companyCode, string branchCode, int receiptNo, bool isPrint, out ErrorVM error)
        {
            error = null;
            try {
                RepairReceiptMasterVM rrvm = new RepairReceiptMasterVM();
                List<RepairReceiptDetailsVM> lstOfrrdvm = new List<RepairReceiptDetailsVM>();

                KTTU_REPAIR_RECEIPT_MASTER krrm = db.KTTU_REPAIR_RECEIPT_MASTER.Where(r => r.company_code == companyCode
                                                                                        && r.branch_code == branchCode
                                                                                        && r.Repair_no == receiptNo).FirstOrDefault();
                List<KTTU_REPAIR_RECEIPT_DETAILS> lstOfkrrd = db.KTTU_REPAIR_RECEIPT_DETAILS.Where(r => r.company_code == companyCode
                                                                                        && r.branch_code == branchCode
                                                                                        && r.Repair_no == receiptNo).ToList();
                KTTU_REPAIR_ISSUE_MASTER issue = db.KTTU_REPAIR_ISSUE_MASTER.Where(i => i.company_code == companyCode
                                                                                && i.branch_code == branchCode
                                                                        && i.receipt_no == receiptNo).FirstOrDefault();
                if (!isPrint) {
                    if (issue != null) {
                        error = new ErrorVM()
                        {
                            field = "Repair Number",
                            index = 0,
                            description = "Invalid Repair Number.",
                            ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                        };
                        return null;
                    }
                }
                if (krrm == null) {
                    error = new ErrorVM()
                    {
                        field = "Repair Number",
                        index = 0,
                        description = "Invalid Repair Number.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }
                if (!isPrint) {
                    if (krrm.cflag == "Y") {
                        error = new ErrorVM()
                        {
                            field = "Repair Number",
                            index = 0,
                            description = "Repair Receipt No: " + receiptNo + " is already cancelled.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return null;
                    }
                }

                rrvm.ObjID = krrm.obj_id;
                rrvm.CompanyCode = krrm.company_code;
                rrvm.BranchCode = krrm.branch_code;
                rrvm.RepairNo = krrm.Repair_no;
                rrvm.CustID = krrm.cust_id;
                rrvm.CustName = krrm.cust_name;
                rrvm.RepairDate = krrm.repair_date;
                rrvm.SalCode = krrm.sal_code;
                rrvm.OperatorCode = krrm.operator_code;
                rrvm.DueDate = krrm.due_date;
                rrvm.TagWt = krrm.tgwt;
                rrvm.CFlag = krrm.cflag;
                rrvm.IssueNo = krrm.issue_no;
                rrvm.RepairItems = krrm.repair_item;
                rrvm.CancelledBy = krrm.cancelled_by;
                rrvm.Remarks = krrm.remark;
                rrvm.CustDueDate = krrm.custduedate;
                rrvm.UpdateOn = krrm.UpdateOn;
                rrvm.ShiftID = krrm.ShiftID;
                rrvm.NewBillNo = krrm.New_Bill_No;
                rrvm.Address1 = krrm.address1;
                rrvm.Address2 = krrm.address2;
                rrvm.Address3 = krrm.address3;
                rrvm.City = krrm.city;
                rrvm.PinCode = krrm.pin_code;
                rrvm.MobileNo = krrm.mobile_no;
                rrvm.State = krrm.state;
                rrvm.StateCode = krrm.state_code;
                rrvm.TIN = krrm.tin;
                rrvm.PANNo = krrm.pan_no;

                foreach (KTTU_REPAIR_RECEIPT_DETAILS krrd in lstOfkrrd) {
                    RepairReceiptDetailsVM rrdvm = new RepairReceiptDetailsVM();
                    rrdvm.ObjID = krrd.obj_id;
                    rrdvm.CompanyCode = krrd.company_code;
                    rrdvm.BranchCode = krrd.branch_code;
                    rrdvm.RepaireNo = krrd.Repair_no;
                    rrdvm.SlNo = krrd.sl_no;
                    rrdvm.Item = krrd.item;
                    rrdvm.Units = krrd.units;
                    rrdvm.GrossWt = krrd.gwt;
                    rrdvm.StoneWt = krrd.swt;
                    rrdvm.NetWt = krrd.nwt;
                    rrdvm.Description = krrd.description;
                    rrdvm.UpdateOn = krrd.UpdateOn;
                    rrdvm.GSCode = krrd.gs_code;
                    rrdvm.FinYear = krrd.Fin_Year;
                    rrdvm.Rate = krrd.rate;
                    rrdvm.PartyCode = krrd.party_code;
                    rrdvm.Dcts = krrd.dcts;
                    lstOfrrdvm.Add(rrdvm);
                }
                rrvm.lstOfRepairReceiptDetails = lstOfrrdvm;
                return rrvm;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public RepairReceiptDetailsVM GetReceiptDetailsTotalForPrint(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            RepairReceiptDetailsVM totalReceiptDetVM = new RepairReceiptDetailsVM();
            List<KTTU_REPAIR_RECEIPT_DETAILS> lstOfkrrd = db.KTTU_REPAIR_RECEIPT_DETAILS.Where(r => r.company_code == companyCode
                                                                                                && r.branch_code == r.branch_code
                                                                                                && r.Repair_no == receiptNo).ToList();
            if (lstOfkrrd == null) {
                error = new ErrorVM()
                {
                    field = "Repair Number",
                    index = 0,
                    description = "Invalid Repair Number.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }
            foreach (KTTU_REPAIR_RECEIPT_DETAILS krrd in lstOfkrrd) {
                RepairReceiptDetailsVM rrdvm = new RepairReceiptDetailsVM();
                totalReceiptDetVM.Units = totalReceiptDetVM.Units + krrd.units;
                totalReceiptDetVM.GrossWt = totalReceiptDetVM.GrossWt + krrd.gwt;
                totalReceiptDetVM.StoneWt = totalReceiptDetVM.StoneWt + krrd.swt;
                totalReceiptDetVM.NetWt = totalReceiptDetVM.NetWt + krrd.nwt;
                totalReceiptDetVM.Rate = Convert.ToDecimal(totalReceiptDetVM.Rate) + krrd.rate;
                totalReceiptDetVM.Dcts = Convert.ToDecimal(totalReceiptDetVM.Dcts) + krrd.dcts;
            }
            return totalReceiptDetVM;
        }

        public int SaveReceiptDetails(RepairReceiptMasterVM repair, out ErrorVM error)
        {
            error = null;
            KTTU_REPAIR_RECEIPT_MASTER krrm = new KTTU_REPAIR_RECEIPT_MASTER();
            //KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == repair.CustID && cust.company_code == repair.CompanyCode && cust.branch_code == repair.BranchCode)
            //                                                    .FirstOrDefault();
            //KSTU_CUSTOMER_MASTER customer = SIGlobals.Globals.GetCustomerDetails(db, repair.CustID, repair.CompanyCode, repair.BranchCode);

            KSTU_CUSTOMER_MASTER customer = new CustomerBL().GetActualCustomerDetails(Convert.ToInt32(repair.CustID), repair.MobileNo, repair.CompanyCode, repair.BranchCode);
            int repairNo = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(db, repair.CompanyCode, repair.BranchCode).ToString().Remove(0, 1)
                                            + db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO && sq.company_code == repair.CompanyCode && sq.branch_code == repair.BranchCode).FirstOrDefault().nextno);
            int finYear = SIGlobals.Globals.GetFinancialYear(db, repair.CompanyCode, repair.BranchCode);
            string objectID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME_RECEIPT, repairNo, repair.CompanyCode, repair.BranchCode);
            try {
                krrm.obj_id = objectID;
                krrm.company_code = repair.CompanyCode;
                krrm.branch_code = repair.BranchCode;
                krrm.Repair_no = repairNo;
                krrm.cust_id = customer.cust_id;
                krrm.cust_name = customer.cust_name;
                krrm.repair_date = repair.RepairDate;
                krrm.sal_code = repair.SalCode;
                krrm.operator_code = repair.OperatorCode;
                krrm.due_date = repair.DueDate;
                krrm.tgwt = repair.TagWt;
                krrm.cflag = "N";
                krrm.issue_no = 0;
                krrm.repair_item = repair.lstOfRepairReceiptDetails[0].GSCode;// repair.RepairItems;
                krrm.cancelled_by = repair.CancelledBy;
                krrm.remark = repair.Remarks;
                krrm.custduedate = repair.DueDate;
                krrm.UpdateOn = Globals.GetDateTime();
                krrm.ShiftID = repair.ShiftID;
                krrm.New_Bill_No = Convert.ToString(repairNo);
                krrm.address1 = customer.address1;
                krrm.address2 = customer.address2;
                krrm.address3 = customer.address3;
                krrm.city = customer.city;
                krrm.pin_code = customer.pin_code;
                krrm.mobile_no = customer.mobile_no;
                krrm.state = customer.state;
                krrm.state_code = customer.state_code;
                krrm.tin = customer.tin;
                krrm.pan_no = customer.pan_no;
                krrm.UniqRowID = Guid.NewGuid();
                krrm.store_location_id = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == repair.CompanyCode
                                                                                && c.branch_code == repair.BranchCode).FirstOrDefault().store_location_id;
                db.KTTU_REPAIR_RECEIPT_MASTER.Add(krrm);

                int SlNo = 1;
                foreach (RepairReceiptDetailsVM rrd in repair.lstOfRepairReceiptDetails) {
                    KTTU_REPAIR_RECEIPT_DETAILS krrd = new KTTU_REPAIR_RECEIPT_DETAILS();
                    krrd.obj_id = krrm.obj_id;
                    krrd.company_code = repair.CompanyCode;
                    krrd.branch_code = repair.BranchCode;
                    krrd.Repair_no = repairNo;
                    krrd.sl_no = SlNo;
                    krrd.item = rrd.Item;
                    krrd.units = rrd.Units;
                    krrd.gwt = rrd.GrossWt;
                    krrd.swt = rrd.StoneWt;
                    krrd.nwt = rrd.NetWt;
                    krrd.description = rrd.Description;
                    krrd.UpdateOn = SIGlobals.Globals.GetDateTime();
                    krrd.gs_code = rrd.GSCode;
                    krrd.Fin_Year = finYear;
                    krrd.rate = rrd.Rate;
                    krrd.party_code = rrd.PartyCode;
                    krrd.dcts = rrd.Dcts == null ? 0 : rrd.Dcts;
                    krrd.UniqRowID = Guid.NewGuid();
                    krrd.finished_gwt = rrd.GrossWt;
                    krrd.finished_swt = rrd.StoneWt;
                    krrd.finished_nwt = rrd.NetWt;
                    krrd.wastage = rrd.Wastage == null ? 0 : rrd.Wastage;
                    krrd.rp_status = "P";
                    db.KTTU_REPAIR_RECEIPT_DETAILS.Add(krrd);
                    SlNo++;
                    ReceiptStockUpdate(krrd.gs_code, krrd.item, krrd.units, krrd.gwt, krrd.swt, krrd.nwt, krrd.company_code, krrd.branch_code);
                }
                // Updating Sequence Number
                Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, repair.CompanyCode, repair.BranchCode);
                #region Document Creation Posting, error will not be checked
                //Post to DocumentCreationLog Table
                DateTime billDate = SIGlobals.Globals.GetApplicationDate(repair.CompanyCode, repair.BranchCode);
                new Common.CommonBL().PostDocumentCreation(repair.CompanyCode, repair.BranchCode, 5, repairNo, billDate, repair.OperatorCode);
                #endregion
                db.SaveChanges();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return 0;
            }
            return krrm.Repair_no;
        }

        public int UpdateReceiptDetails(int receiptNo, RepairReceiptMasterVM repair, out ErrorVM error)
        {
            error = null;
            KTTU_REPAIR_RECEIPT_MASTER krrm = new KTTU_REPAIR_RECEIPT_MASTER();
            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == repair.CustID && cust.company_code == repair.CompanyCode && cust.branch_code == repair.BranchCode)
                                                                   .FirstOrDefault();

            int repairNo = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO && sq.company_code == repair.CompanyCode && sq.branch_code == repair.BranchCode).FirstOrDefault().nextno;
            try {

                #region Delete Existing Data
                KTTU_REPAIR_RECEIPT_MASTER delkrrm = db.KTTU_REPAIR_RECEIPT_MASTER.Where(r => r.Repair_no == receiptNo
                                                                                        && r.company_code == repair.CompanyCode
                                                                                        && r.branch_code == repair.BranchCode).FirstOrDefault();
                db.KTTU_REPAIR_RECEIPT_MASTER.Remove(delkrrm);

                List<KTTU_REPAIR_RECEIPT_DETAILS> delkrrd = db.KTTU_REPAIR_RECEIPT_DETAILS.Where(r => r.Repair_no == receiptNo
                                                                                        && r.company_code == repair.CompanyCode
                                                                                        && r.branch_code == repair.BranchCode).ToList();
                foreach (KTTU_REPAIR_RECEIPT_DETAILS krrd in delkrrd) {
                    db.KTTU_REPAIR_RECEIPT_DETAILS.Remove(krrd);
                }
                #endregion

                #region Save Master and Details
                string objectID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME_RECEIPT, repairNo, repair.CompanyCode, repair.BranchCode);
                krrm.obj_id = objectID;
                krrm.company_code = repair.CompanyCode;
                krrm.branch_code = repair.BranchCode;
                krrm.Repair_no = repairNo;
                krrm.cust_id = repair.CustID;
                krrm.cust_name = customer.cust_name;
                krrm.repair_date = repair.RepairDate;
                krrm.sal_code = repair.SalCode;
                krrm.operator_code = repair.OperatorCode;
                krrm.due_date = repair.DueDate;
                krrm.tgwt = repair.TagWt;
                krrm.cflag = repair.CFlag;
                krrm.issue_no = repair.IssueNo;
                krrm.repair_item = repair.RepairItems;
                krrm.cancelled_by = repair.CancelledBy;
                krrm.remark = repair.Remarks;
                krrm.custduedate = repair.CustDueDate;
                krrm.UpdateOn = Globals.GetDateTime();
                krrm.ShiftID = repair.ShiftID;
                krrm.New_Bill_No = repair.NewBillNo;
                krrm.address1 = customer.address1;
                krrm.address2 = customer.address2;
                krrm.address3 = customer.address3;
                krrm.city = customer.city;
                krrm.pin_code = customer.pin_code;
                krrm.mobile_no = customer.mobile_no;
                krrm.state = customer.state;
                krrm.state_code = customer.state_code;
                krrm.tin = customer.tin;
                krrm.pan_no = customer.pan_no;
                db.KTTU_REPAIR_RECEIPT_MASTER.Add(krrm);

                int SlNo = 1;
                foreach (RepairReceiptDetailsVM rrd in repair.lstOfRepairReceiptDetails) {
                    KTTU_REPAIR_RECEIPT_DETAILS krrd = new KTTU_REPAIR_RECEIPT_DETAILS();
                    krrd.obj_id = objectID;
                    krrd.company_code = rrd.CompanyCode;
                    krrd.branch_code = rrd.BranchCode;
                    krrd.Repair_no = rrd.RepaireNo;
                    krrd.sl_no = rrd.SlNo;
                    krrd.item = rrd.Item;
                    krrd.units = rrd.Units;
                    krrd.gwt = rrd.GrossWt;
                    krrd.swt = rrd.StoneWt;
                    krrd.nwt = rrd.NetWt;
                    krrd.description = rrd.Description;
                    krrd.UpdateOn = Globals.GetDateTime();
                    krrd.gs_code = rrd.GSCode;
                    krrd.Fin_Year = rrd.FinYear;
                    krrd.rate = rrd.Rate;
                    krrd.party_code = rrd.PartyCode;
                    krrd.dcts = rrd.Dcts;
                    db.KTTU_REPAIR_RECEIPT_DETAILS.Add(krrd);
                    SlNo++;
                }
                #endregion;
                // Updating Sequence Number
                Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, repair.CompanyCode, repair.BranchCode);
                db.SaveChanges();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return 0;
            }
            return krrm.Repair_no;
        }

        public IQueryable<RepairReceiptMasterVM> GetAllRepairDetails(string companyCode, string branchCode, string searchType, string searchValue)
        {
            List<RepairReceiptMasterVM> lstOfReceiptMaster = new List<RepairReceiptMasterVM>();
            List<KTTU_REPAIR_RECEIPT_MASTER> lstOfReceiptMasterObj = new List<KTTU_REPAIR_RECEIPT_MASTER>();

            List<KTTU_REPAIR_ISSUE_MASTER> lstOfIssueMaster = db.KTTU_REPAIR_ISSUE_MASTER.Where(rp => rp.cflag != "Y"
                                                                                && rp.company_code == companyCode
                                                                                && rp.branch_code == branchCode).ToList();
            int[] receiptNo = lstOfIssueMaster.Select(issue => issue.receipt_no).ToArray();
            IEnumerable<KTTU_REPAIR_RECEIPT_MASTER> lstOfRepair = db.KTTU_REPAIR_RECEIPT_MASTER.Where(rep => rep.company_code == companyCode
                                                                            && rep.branch_code == branchCode
                                                                            && rep.cflag != "Y").ToList().Where(sep => !receiptNo.Contains(sep.Repair_no));

            switch (searchType.ToUpper()) {
                case "REPAIRNO":
                    lstOfReceiptMasterObj = lstOfRepair.Where(search => search.Repair_no == Convert.ToInt32(searchValue)).ToList();
                    break;
                case "NAME":
                    lstOfReceiptMasterObj = lstOfRepair.Where(search => search.cust_name.Contains(searchValue)).ToList();
                    break;
                default:
                    lstOfReceiptMasterObj = lstOfRepair.ToList();
                    break;
            }

            foreach (KTTU_REPAIR_RECEIPT_MASTER krrm in lstOfReceiptMasterObj) {
                RepairReceiptMasterVM rrvm = new RepairReceiptMasterVM();
                rrvm.ObjID = krrm.obj_id;
                rrvm.CompanyCode = krrm.company_code;
                rrvm.BranchCode = krrm.branch_code;
                rrvm.RepairNo = krrm.Repair_no;
                rrvm.CustID = krrm.cust_id;
                rrvm.CustName = krrm.cust_name;
                rrvm.RepairDate = krrm.repair_date;
                rrvm.SalCode = krrm.sal_code;
                rrvm.OperatorCode = krrm.operator_code;
                rrvm.DueDate = krrm.due_date;
                rrvm.TagWt = krrm.tgwt;
                rrvm.CFlag = krrm.cflag;
                rrvm.IssueNo = krrm.issue_no;
                rrvm.RepairItems = krrm.repair_item;
                rrvm.CancelledBy = krrm.cancelled_by;
                rrvm.Remarks = krrm.remark;
                rrvm.CustDueDate = krrm.custduedate;
                rrvm.UpdateOn = krrm.UpdateOn;
                rrvm.ShiftID = krrm.ShiftID;
                rrvm.NewBillNo = krrm.New_Bill_No;
                rrvm.Address1 = krrm.address1;
                rrvm.Address2 = krrm.address2;
                rrvm.Address3 = krrm.address3;
                rrvm.City = krrm.city;
                rrvm.PinCode = krrm.pin_code;
                rrvm.MobileNo = krrm.mobile_no;
                rrvm.State = krrm.state;
                rrvm.StateCode = krrm.state_code;
                rrvm.TIN = krrm.tin;
                rrvm.PANNo = krrm.pan_no;
                lstOfReceiptMaster.Add(rrvm);
            }
            return lstOfReceiptMaster.AsQueryable<RepairReceiptMasterVM>();
        }

        public bool CancelRepairReceipt(RepairReceiptMasterVM receiptMaster, out ErrorVM error)
        {
            error = null;
            try {
                KTTU_REPAIR_RECEIPT_MASTER krim = db.KTTU_REPAIR_RECEIPT_MASTER.Where(i => i.Repair_no == receiptMaster.RepairNo
                                                                                    && i.company_code == receiptMaster.CompanyCode
                                                                                    && i.branch_code == receiptMaster.BranchCode).FirstOrDefault();
                List<KTTU_REPAIR_RECEIPT_DETAILS> krid = db.KTTU_REPAIR_RECEIPT_DETAILS.Where(d => d.Repair_no == receiptMaster.RepairNo
                                                                                    && d.company_code == receiptMaster.CompanyCode
                                                                                    && d.branch_code == receiptMaster.BranchCode).ToList();
                if (krim.cflag == "Y") {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Repair Receipt No: " + receiptMaster.RepairNo + " is already cancelled.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                krim.cflag = "Y";
                krim.remark = receiptMaster.Remarks;
                krim.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(krim).State = System.Data.Entity.EntityState.Modified;

                foreach (KTTU_REPAIR_RECEIPT_DETAILS krrd in krid) {
                    CancelReceiptStockUpdate(krrd.gs_code, krrd.item, krrd.units, krrd.gwt, krrd.swt, krrd.nwt, krrd.company_code, krrd.branch_code);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public string GetReceiptDotMatrixPrint(string companyCode, string branchCode, int repairNo, out ErrorVM error)
        {
            error = null;
            KTTU_REPAIR_RECEIPT_MASTER master = db.KTTU_REPAIR_RECEIPT_MASTER.Where(repair => repair.company_code == companyCode && repair.branch_code == branchCode && repair.Repair_no == repairNo).FirstOrDefault();
            if (master == null) {
                return "";
            }
            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == master.cust_id && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault();
            List<KTTU_REPAIR_RECEIPT_DETAILS> receiptDet = db.KTTU_REPAIR_RECEIPT_DETAILS.Where(r => r.company_code == companyCode && r.branch_code == branchCode && r.Repair_no == repairNo).ToList();
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode && com.branch_code == branchCode).FirstOrDefault();
            try {
                StringBuilder sbHTML = new StringBuilder();
                string strRepairRcpt = string.Empty;
                string cust_name = string.Empty;
                string cust_id = string.Empty;

                cust_name = Convert.ToString(customer.cust_name);
                if (string.IsNullOrEmpty(cust_name)) {
                    cust_name = customer.cust_name;
                    cust_id = customer.cust_id.ToString();
                }
                StringBuilder strLine = new StringBuilder();
                strLine.Append('-', 90);
                StringBuilder strTransLine = new StringBuilder();
                strTransLine.Append('-', 90);
                decimal Spaces = 0M;
                StringBuilder strSpaces = new StringBuilder();
                Spaces = ((90 - strTransLine.Length) / 3);
                strSpaces.Append(' ', Convert.ToInt32(Spaces));

                StringBuilder sb = new StringBuilder();
                sb.Append((string.Format("{0, 20}{1, -30}{2, 12}", "", SIGlobals.Globals.FillCompanyDetails("Company_Name", companyCode, branchCode), "")));
                sb.AppendLine();
                sb.Append(SIGlobals.Globals.GetCompanyname(db, companyCode, branchCode));
                sb.AppendLine(string.Format("{0, 20}{1, -30}{2, 12}", "", "Repair Receipt", ""));
                sb.AppendLine(string.Format("{0, 20}{1, -30}{2, 12}", "", "ORIGINAL / DUPLICATE", ""));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sbHTML.Append(sb);

                KSTU_SALESMAN_MASTER salesman = db.KSTU_SALESMAN_MASTER.Where(sal => sal.sal_code == master.sal_code && sal.company_code == master.company_code && sal.branch_code == master.branch_code).FirstOrDefault();
                string salesManName = salesman.sal_name;
                sbHTML.AppendLine(string.Format("{0,-10}{1,-19}{2,20}{3,-15}{4,15}", "Repair No: ", master.branch_code + "/" + master.Repair_no.ToString(), "", "Receipt Date: ", Convert.ToDateTime(master.repair_date).ToString("dd/MM/yyyy").Trim()));
                sbHTML.AppendLine(string.Format("{0,-5}{1,-44}{2,-15}{3,15}", "Customer id: ", master.cust_id, "Due Date: ", Convert.ToDateTime(master.due_date).ToString("dd/MM/yyyy").Trim()));
                sbHTML.AppendLine(string.Format("{0,-12}{1,-16}{2,21}{3,-15}{4,15}", "Name: ", master.cust_name.ToString().Trim(), "", "Salesman Code: ", master.sal_code.ToString().Trim()));

                if (customer != null) {
                    if (!string.IsNullOrEmpty(customer.mobile_no.ToString()) && !string.IsNullOrEmpty(customer.phone_no.ToString())) {
                        sbHTML.AppendLine(string.Format("{0,-12}{1,-16}{2,21}{3,-15}{4,15}", "Mobile No: ", customer.mobile_no.ToString().Trim(), " ", " Phone No: ", customer.phone_no.ToString().Trim()));
                    }
                    else if (!string.IsNullOrEmpty(customer.mobile_no.ToString())) {
                        sbHTML.AppendLine(string.Format("{0,-12}{1,-16}{2,21}{3,-15}{4,15}", "Mobile No: ", customer.mobile_no.ToString().Trim(), " ", " Phone No: ", customer.phone_no.ToString().Trim()));
                    }
                    else if (!string.IsNullOrEmpty(customer.mobile_no.ToString())) {
                        sbHTML.AppendLine(string.Format("{0,-12}{1,-16}{2,21}{3,-15}{4,15}", "Mobile No: ", " ", " ", " Phone No: ", ""));
                    }
                    if (!string.IsNullOrEmpty(customer.pan_no.ToString())) {
                        sbHTML.AppendLine(string.Format("{0,-12}{1,-16}", "PAN :", customer.pan_no.ToString().Trim()));
                    }
                    else {
                        KSTU_CUSTOMER_ID_PROOF_DETAILS customerIDProof = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(c => c.cust_id == master.cust_id && c.company_code == master.company_code && c.branch_code == master.branch_code).FirstOrDefault();
                        if (customerIDProof != null) {
                            if (!string.IsNullOrEmpty(customerIDProof.Doc_code.ToString())) {
                                sbHTML.AppendLine(string.Format("{0,-10}{1,-16}", customerIDProof.Doc_code.ToString(), ": " + customerIDProof.Doc_No.ToString()));
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(customer.state.ToString())) {
                        sbHTML.AppendLine(string.Format("{0,-12}{1,-16}", "State :", customer.state.ToString().Trim()));
                    }
                    if (!string.IsNullOrEmpty(customer.state_code.ToString())) {
                        sbHTML.AppendLine(string.Format("{0,-12}{1,-16}", "State Code :", customer.state_code.ToString().Trim()));
                    }
                }
                sbHTML.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sbHTML.AppendLine(string.Format("{0,-4}{1,-33}{2,4}{3, 9}{4,9}{5,9}{6,7}{7,15}", "SNo", "Description", "Qty", "Gr.Wt", "St.Wt", "Net Wt", "Dcts", "Repair Charges"));
                sbHTML.AppendLine();
                sbHTML.Append(string.Format("{0}{1}", strSpaces, strTransLine));

                sbHTML.AppendLine();
                decimal Grosswt = 0;
                decimal Units = 0;
                decimal NetWt = 0, Swt = 0, Dcts = 0, ReapirCharge = 0;

                for (int i = 0; i < receiptDet.Count; i++) {
                    sbHTML.Append(string.Format("{0,-4}", receiptDet[i].sl_no.ToString()));
                    sbHTML.Append(string.Format("{0,-33}", receiptDet[i].description.ToString()));
                    sbHTML.Append(string.Format("{0,4}", receiptDet[i].units.ToString()));
                    sbHTML.Append(string.Format("{0,9}", receiptDet[i].gwt.ToString()));
                    sbHTML.Append(string.Format("{0,9}", receiptDet[i].swt.ToString()));
                    sbHTML.Append(string.Format("{0,9}", receiptDet[i].nwt.ToString()));
                    sbHTML.Append(string.Format("{0,7}", receiptDet[i].dcts.ToString()));
                    sbHTML.Append(string.Format("{0,15}", receiptDet[i].rate.ToString()));
                    sbHTML.AppendLine();
                }
                sbHTML.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                object tempGUnits = receiptDet.Sum(r => r.units);
                if (tempGUnits != null && tempGUnits != DBNull.Value)
                    Units = Convert.ToDecimal(tempGUnits);

                object tempGgrosswt = receiptDet.Sum(r => r.gwt);
                if (tempGgrosswt != null && tempGgrosswt != DBNull.Value)
                    Grosswt = Convert.ToDecimal(tempGgrosswt);

                object tempGNetWt = receiptDet.Sum(r => r.nwt);
                if (tempGNetWt != null && tempGNetWt != DBNull.Value)
                    NetWt = Convert.ToDecimal(tempGNetWt);

                tempGNetWt = receiptDet.Sum(r => r.swt);
                if (tempGNetWt != null && tempGNetWt != DBNull.Value)
                    Swt = Convert.ToDecimal(tempGNetWt);

                object tempGRepairCharge = receiptDet.Sum(r => r.rate);
                if (tempGRepairCharge != null && tempGRepairCharge != DBNull.Value)
                    ReapirCharge = Convert.ToDecimal(tempGRepairCharge);

                object tempGDcts = receiptDet.Sum(r => r.dcts);
                if (tempGDcts != null && tempGDcts != DBNull.Value)
                    Dcts = Convert.ToDecimal(tempGDcts);

                sbHTML.AppendLine(string.Format("{0,-4}{1,-33}{2,4}{3,9}{4,9}{5,9}{6,7}{7,15}", "", "TOTAL", Units, Grosswt, Swt, NetWt, Dcts, ReapirCharge));
                sbHTML.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                sbHTML.AppendLine(string.Format("{0,-10}{1,-80}", "Remarks:", master.remark == null ? "" : master.remark.ToString().Trim()));

                sbHTML.AppendLine();
                sbHTML.AppendLine();
                sbHTML.AppendLine();

                sbHTML.AppendLine(string.Format("{0,-40}{1,40}", "Customer Signature", "Authorized Signature"));
                return sbHTML.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string GetReceiptHTMLPrint(string companyCode, string branchCode, int repairNo, string printType, out ErrorVM error)
        {
            error = null;
            KTTU_REPAIR_RECEIPT_MASTER receiptMaster = db.KTTU_REPAIR_RECEIPT_MASTER.Where(recep => recep.company_code == companyCode
                                                                                            && recep.branch_code == branchCode
                                                                                            && recep.Repair_no == repairNo).FirstOrDefault();
            if (receiptMaster == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Repair/Receipt Number",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return "";
            }

            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            List<KTTU_REPAIR_RECEIPT_DETAILS> receiptDet = db.KTTU_REPAIR_RECEIPT_DETAILS.Where(rp => rp.company_code == companyCode
                                                                                                && rp.branch_code == branchCode
                                                                                                && rp.Repair_no == repairNo).ToList();
            KSTU_CUSTOMER_ID_PROOF_DETAILS proof = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cust => cust.cust_id == receiptMaster.cust_id
                                                                                            && cust.company_code == companyCode
                                                                                            && cust.branch_code == branchCode).FirstOrDefault();
            try {
                string strRepairRcpt = string.Empty;
                string cust_name = string.Empty;
                string cust_id = string.Empty;

                if (string.IsNullOrEmpty(cust_name)) {
                    cust_name = receiptMaster.cust_name;
                    cust_id = Convert.ToString(receiptMaster.cust_id);
                }

                string CompanyAddress = string.Empty;
                CompanyAddress = company.address1;
                CompanyAddress = CompanyAddress + "<br>" + company.address2;
                CompanyAddress = CompanyAddress + "<br>" + company.address3;
                CompanyAddress = CompanyAddress + "<br>" + "TIN: " + company.tin_no;
                CompanyAddress = CompanyAddress + "<br>" + "Phone No: " + company.phone_no;
                CompanyAddress = CompanyAddress + "<br> E-mail : " + company.email_id;
                CompanyAddress = CompanyAddress + "<br> Website : " + company.website;

                StringBuilder sbStart = new StringBuilder();
                sbStart.AppendLine("<html>");
                sbStart.AppendLine("<head>");
                sbStart.AppendLine(SIGlobals.Globals.GetStyleOrder());
                sbStart.AppendLine("</head>");
                sbStart.AppendLine("<body>");

                string DateTime = string.Format("{0:dd/MM/yyyy}", (receiptMaster.repair_date));
                sbStart.AppendLine("<Table frame=\"border\" border=\"0\" width=\"800\">");
                for (int j = 0; j < 8; j++) {
                    sbStart.AppendLine("<TR style=border:0>");
                    sbStart.AppendLine(string.Format("<TD style=border:0 colspan = 8 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine("<TD style=border:0 width=\"800\" colspan=0 ALIGN = \"CENTER\"><b><h4>REPAIR RECEIPT </h4></b></TD>");
                sbStart.AppendLine("</TR>");
                for (int j = 0; j < 2; j++) {
                    sbStart.AppendLine("<TR style=border:0>");
                    sbStart.AppendLine(string.Format("<TD style=border:0 colspan = 8 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }

                sbStart.AppendLine("</Table>");
                string Name = receiptMaster.cust_name;
                string Address1 = receiptMaster.address1;
                string Address2 = receiptMaster.address2;
                string Address3 = receiptMaster.address3;
                string City = receiptMaster.city;
                string pincode = receiptMaster.pin_code;
                string Phone = receiptMaster.mobile_no;
                string Mobile = receiptMaster.mobile_no;
                string Sal_code = receiptMaster.sal_code;
                string operator_code = receiptMaster.operator_code;
                string PAN = receiptMaster.pan_no;
                string TIN = receiptMaster.tin;

                string state_code = receiptMaster.state_code.ToString();
                string state = receiptMaster.state;

                if (!string.IsNullOrEmpty(Mobile) && !string.IsNullOrEmpty(Phone))
                    Mobile = Mobile + "/" + Phone;
                if (string.IsNullOrEmpty(Mobile))
                    Mobile = Phone;
                if (!string.IsNullOrEmpty(pincode))
                    City = City + " - " + pincode;

                string Address = string.Empty;

                if (Address1 != string.Empty)
                    Address = Address + "<br>" + Address1;
                if (Address2 != string.Empty)
                    Address = Address + "<br>" + Address2;
                if (Address3 != string.Empty)
                    Address = Address + "<br>" + Address3;

                sbStart.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\";  style=\"border-collapse:collapse;\" width=\"900\">");
                sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine(string.Format("<TD width=\"350\" ALIGN = \"center\"><b>CUSTOMER DETAILS</b></TD>"));

                sbStart.AppendLine(string.Format("<TD width=\"250\"  ALIGN = \"center\"><b>GSTIN &nbsp&nbsp&nbsp&nbsp {0}</b></TD>", company.tin_no.ToString()));
                sbStart.AppendLine(string.Format("<TD width=\"300\"  ALIGN = \"center\"><b>{0}</b></TD>", "SHOWROOM ADDRESS"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<tr>");

                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + Name + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address1 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address2 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address3 + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" tyle=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + City + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + state + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + state_code + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Mobile + "</b></td>");
                sbStart.AppendLine("</tr>");

                if (!string.IsNullOrEmpty(PAN)) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + PAN + "</b></td>");
                    sbStart.AppendLine("</tr>");
                }
                else {
                    if (proof != null) {
                        if (!string.IsNullOrEmpty(proof.Doc_code.ToString())) {
                            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                            sbStart.AppendLine(string.Format("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>{0} &nbsp&nbsp</b></td>", proof.Doc_code.ToString()));
                            sbStart.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", proof.Doc_No.ToString()));
                            sbStart.AppendLine("</tr>");
                        }
                    }
                }
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b> GSTIN &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + TIN + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");

                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sbStart.AppendLine("<tr style=\"border-right:0\">");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"left \" ><b>Repair No</b></td>");
                sbStart.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" ><b>  {0}/{1}</b></TD>", branchCode, repairNo));
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"left\" ><b>Date &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + DateTime + "</b></td>");
                sbStart.AppendLine("</tr>");


                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" ><b> Place of supply &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + company.state + "</b></td>");
                sbStart.AppendLine("</tr>");


                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + company.pan_no.ToString() + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");

                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.company_name + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address1 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"12pt\" style=\"border-top:thin\" ><b>" + company.address2 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address3 + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.city + " - " + company.pin_code + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + company.state.ToString() + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin  ;border-top:thin\" ><b>" + company.state_code.ToString() + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + company.phone_no.ToString() + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table>");
                sbStart.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; border-top:thin\" width=\"900\">");  //FRAME=BOX RULES=NONE
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD  style=\"border-right:thin; border-top:thin \" colspan=7 ALIGN = \"CENTER\"><b>{0}<br></b></TD>", printType));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                sbStart.AppendLine("<TD style=\" border-bottom:thin solid; \" ALIGN = \"LEFT\"><b>Sl</b></TD>");
                sbStart.AppendLine("<TD style=\" border-bottom:thin solid; \"  ALIGN = \"LEFT\"><b>Description</b></TD>");
                sbStart.AppendLine("<TD style=\" border-bottom:thin solid; \"   ALIGN = \"CENTER\"><b>Qty</b></TD>");
                sbStart.AppendLine("<TD style=\" border-bottom:thin solid; \"  ALIGN = \"RIGHT\"><b>Gr.Wt (gms)</b></TD>");
                sbStart.AppendLine("<TD style=\" border-bottom:thin solid; \" ALIGN = \"RIGHT\"><b>St.Wt (gms)</b></TD>");
                sbStart.AppendLine("<TD style=\" border-bottom:thin solid; \"  ALIGN = \"RIGHT\"><b>Nt.Wt (gms)</b></TD>");
                sbStart.AppendLine("<TD style=\" border-bottom:thin solid; \" ALIGN = \"RIGHT\"><b>D.Cts</b></TD>");
                sbStart.AppendLine("<TD style=\" border-bottom:thin solid;  \" ALIGN = \"RIGHT\"><b>Repair Charge</b></TD>");
                sbStart.AppendLine("</TR>");

                decimal Grosswt = 0;
                decimal Units = 0;
                decimal NetWt = 0, Dcts = 0;
                decimal RepCharge = 0;
                int z = 1;

                for (int i = 0; i < receiptDet.Count; i++) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\">{0}{1}{1} </TD>", i + 1, "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\" width=\"37%\">{0} </TD>", receiptDet[i].description.ToString()));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"CENTER\">{0}{1}{1} </TD>", receiptDet[i].units.ToString(), "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1} </TD>", receiptDet[i].gwt.ToString(), "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1} </TD>", receiptDet[i].swt.ToString(), "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1} </TD>", receiptDet[i].nwt.ToString(), "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1} </TD>", receiptDet[i].dcts.ToString(), "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\">{0}{1}{1} </TD>", receiptDet[i].rate.ToString(), "&nbsp"));

                }

                int MaxPageRow = 8;
                for (int j = 0; j < MaxPageRow - receiptDet.Count; j++) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");

                }
                object tempGUnits = receiptDet.Sum(r => r.units);
                if (tempGUnits != null && tempGUnits != DBNull.Value)
                    Units = Convert.ToDecimal(tempGUnits);

                object tempGgrosswt = receiptDet.Sum(r => r.gwt);
                if (tempGgrosswt != null && tempGgrosswt != DBNull.Value)
                    Grosswt = Convert.ToDecimal(tempGgrosswt);

                object tempGNetWt = receiptDet.Sum(r => r.nwt);
                if (tempGNetWt != null && tempGNetWt != DBNull.Value)
                    NetWt = Convert.ToDecimal(tempGNetWt);

                object tempRepChrge = receiptDet.Sum(r => r.rate);
                if (tempRepChrge != null && tempRepChrge != DBNull.Value)
                    RepCharge = Convert.ToDecimal(tempRepChrge);

                object tempDcts = receiptDet.Sum(r => r.dcts);
                if (tempDcts != null && tempDcts != DBNull.Value)
                    Dcts = Convert.ToDecimal(tempDcts);


                sbStart.AppendLine("<TR  bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TD ALIGN = \"LEFT\"><b></b></TD>");
                sbStart.AppendLine(string.Format("<TD ALIGN = \"LEFT\" ><b>Total{0}</b></TD>", "&nbsp"));
                sbStart.AppendLine(string.Format("<TD ALIGN = \"CENTER\"><b>{0}</b></TD>", Units, "&nbsp"));
                sbStart.AppendLine(string.Format("<TD ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", Grosswt, "&nbsp"));
                sbStart.AppendLine(string.Format("<TD ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", "&nbsp", "&nbsp"));
                sbStart.AppendLine(string.Format("<TD ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", NetWt, "&nbsp"));
                sbStart.AppendLine(string.Format("<TD ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", Dcts, "&nbsp"));
                sbStart.AppendLine(string.Format("<TD ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", RepCharge, "&nbsp"));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin;  \" colspan=10 ALIGN = \"lef\"><b>SM Code : {0} </b></TD>", receiptMaster.sal_code));


                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD  style=\"border-right:thin \" colspan = 4 ALIGN = \"LEFT\"><b><br><br><br><br><br>{0}</b></TD>", "Customer Signature"));
                sbStart.AppendLine(string.Format("<TD colspan = 6 ALIGN = \"RIGHT\"><b>For {0}<br><br><br><br><br>{1}</b></TD>", company.company_name, "Authorized Signatory"));
                sbStart.AppendLine("</TR>");
                return sbStart.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public ProdigyPrintVM GetRepairReceiptPrint(string companyCode, string branchCode, int repairNo, string printType, out ErrorVM error)
        {
            error = null;
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            PrintConfiguration pc = new PrintConfiguration();
            var printConfig = pc.GetPrintConfigurationForDraftDocuments(companyCode, branchCode, "REP_REC");
            if (printConfig == "HTML") {
                var htmlPrintData = GetReceiptHTMLPrint(companyCode, branchCode, repairNo, printType, out error);
                printObject.ContinueNextPrint = false;
                printObject.Data = new PrintConfiguration().Base64Encode(htmlPrintData);
                printObject.PrintType = "HTML";
            }
            else {
                var dotMatrixPrintData = GetReceiptDotMatrixPrint(companyCode, branchCode, repairNo, out error);
                printObject.ContinueNextPrint = true;
                printObject.Data = dotMatrixPrintData;
                printObject.PrintType = "RAW";
            }
            return printObject;
        }

        #endregion

        #region Issues or Delivery
        public RepairIssueMasterVM GetRepairIssueDetails(string companyCode, string branchCode, int issueNo, bool isPrint, out ErrorVM error)
        {
            error = null;
            try {
                RepairIssueMasterVM rimv = new RepairIssueMasterVM();
                KTTU_REPAIR_ISSUE_MASTER krim = new KTTU_REPAIR_ISSUE_MASTER();
                List<KTTU_REPAIR_ISSUE_DETAILS> lstOfkrid = new List<KTTU_REPAIR_ISSUE_DETAILS>();
                List<KTTU_PAYMENT_DETAILS> lstOfPay = new List<KTTU_PAYMENT_DETAILS>();

                krim = db.KTTU_REPAIR_ISSUE_MASTER.Where(i => i.issue_no == issueNo
                                                            && i.company_code == companyCode
                                                            && i.branch_code == branchCode).FirstOrDefault();
                if (krim == null) {
                    error = new ErrorVM()
                    {
                        field = "Delivery Number",
                        index = 0,
                        description = "Invalid Delivery Number.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }

                if (!isPrint) {
                    if (krim.cflag == "Y") {
                        error = new ErrorVM()
                        {
                            field = "Delivery Number",
                            index = 0,
                            description = "Delivery No: " + issueNo + " is already cancelled.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return null;
                    }
                }

                lstOfkrid = db.KTTU_REPAIR_ISSUE_DETAILS.Where(i => i.issue_no == issueNo
                                                                && i.company_code == companyCode
                                                                && i.branch_code == branchCode).ToList();
                lstOfPay = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == issueNo
                                                        && pay.trans_type == "RO"
                                                        && pay.company_code == companyCode
                                                        && pay.branch_code == branchCode).ToList();
                #region IssueMaster
                rimv.ObjID = krim.obj_id;
                rimv.CompanyCode = krim.company_code;
                rimv.BranchCode = krim.branch_code;
                rimv.IssueNo = krim.issue_no;
                rimv.ReceiptNo = krim.receipt_no;
                rimv.IssueDate = krim.issue_date;
                rimv.SalCode = krim.sal_code;
                rimv.OperatorCode = krim.operator_code;
                rimv.TotalRepairAmount = krim.total_repair_amount;
                rimv.CFlag = krim.cflag;
                rimv.ItemType = krim.item_type;
                rimv.Remark = krim.remark;
                rimv.UpdateOn = krim.UpdateOn;
                rimv.TaxPercentage = krim.tax_percentage;
                rimv.TotalTaxAmount = krim.total_tax_amount;
                rimv.TotalAmountBeforeTax = krim.total_amount_before_tax;
                rimv.ShiftID = krim.ShiftID;
                rimv.NewBillNo = krim.New_Bill_No;
                #endregion

                #region Issue Details
                List<RepairIssueDetailsVM> lstOfridvm = new List<RepairIssueDetailsVM>();
                foreach (KTTU_REPAIR_ISSUE_DETAILS krid in lstOfkrid) {
                    RepairIssueDetailsVM rpid = new RepairIssueDetailsVM();
                    rpid.ObjID = krid.obj_id;
                    rpid.CompanyCode = krid.company_code;
                    rpid.BranchCode = krid.branch_code;
                    rpid.IssueNo = krid.issue_no;
                    rpid.ReceiptNo = krid.receipt_no;
                    rpid.SlNo = krid.sl_no;
                    rpid.Item = krid.item;
                    rpid.Units = krid.units;
                    rpid.GrossWt = krid.gwt;
                    rpid.StoneWt = krid.swt;
                    rpid.NetWt = krid.nwt;
                    rpid.Description = krid.description;
                    rpid.WastageGrams = krid.wastage_grms;
                    rpid.WtAdd = krid.wtadd;
                    rpid.GoldRate = krid.gold_rate;
                    rpid.GoldAmount = krid.gold_amount;
                    rpid.MiscAmount = krid.misc_amount;
                    rpid.RepairAmount = krid.repair_amount;
                    rpid.UpdateOn = krid.UpdateOn;
                    rpid.GSCode = krid.gs_code;
                    rpid.FinYear = krid.Fin_Year;
                    rpid.ItemwiseTaxPercentage = krid.itemwise_tax_percentage;
                    rpid.ItemwiseTaxAmount = krid.itemwise_tax_amount;
                    rpid.ItemwiseTotalAmountBeforeTax = krid.itemwise_total_amount_before_tax;
                    rpid.Rate = krid.rate;
                    rpid.PartyCode = krid.party_code;
                    rpid.GSTGroupCode = krid.GSTGroupCode;
                    rpid.SGSTPercent = krid.SGST_Percent;
                    rpid.SGSTAmount = krid.SGST_Amount;
                    rpid.CGSTPercent = krid.CGST_Percent;
                    rpid.CGSTAmount = krid.CGST_Amount;
                    rpid.IGSTPercent = krid.IGST_Percent;
                    rpid.IGSTAmount = krid.IGST_Amount;
                    rpid.HSN = krid.HSN;
                    rpid.Dcts = krid.dcts;
                    lstOfridvm.Add(rpid);
                }
                rimv.lstOfRepairIssueDetails = lstOfridvm;
                #endregion

                #region Payment Details
                if (lstOfPay != null && lstOfPay.Count > 0) {
                    List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                    foreach (KTTU_PAYMENT_DETAILS paymentDet in lstOfPay) {
                        PaymentVM payment = new PaymentVM();
                        payment.ObjID = paymentDet.obj_id;
                        payment.CompanyCode = paymentDet.company_code;
                        payment.BranchCode = paymentDet.branch_code;
                        payment.SeriesNo = paymentDet.series_no;
                        payment.ReceiptNo = paymentDet.receipt_no;
                        payment.SNo = paymentDet.sno;
                        payment.TransType = paymentDet.trans_type;
                        payment.PayMode = paymentDet.pay_mode;
                        payment.PayDetails = paymentDet.pay_details;
                        payment.PayDate = paymentDet.pay_date;
                        payment.PayAmount = paymentDet.pay_amt;
                        payment.RefBillNo = paymentDet.Ref_BillNo;
                        payment.PartyCode = paymentDet.party_code;
                        payment.BillCounter = paymentDet.bill_counter;
                        payment.IsPaid = paymentDet.is_paid;
                        payment.Bank = paymentDet.bank;
                        payment.BankName = paymentDet.bank_name;
                        payment.ChequeDate = paymentDet.cheque_date;
                        payment.CardType = paymentDet.card_type;
                        payment.ExpiryDate = paymentDet.expiry_date;
                        payment.CFlag = paymentDet.cflag;
                        payment.CardAppNo = paymentDet.card_app_no;
                        payment.SchemeCode = paymentDet.scheme_code;
                        payment.SalBillType = paymentDet.sal_bill_type;
                        payment.OperatorCode = paymentDet.operator_code;
                        payment.SessionNo = paymentDet.session_no;
                        payment.UpdateOn = paymentDet.UpdateOn;
                        payment.GroupCode = paymentDet.group_code;
                        payment.AmtReceived = paymentDet.amt_received;
                        payment.BonusAmt = paymentDet.bonus_amt;
                        payment.WinAmt = paymentDet.win_amt;
                        payment.CTBranch = paymentDet.ct_branch;
                        payment.FinYear = paymentDet.fin_year;
                        payment.CardCharges = paymentDet.CardCharges;
                        payment.ChequeNo = paymentDet.cheque_no;
                        payment.NewBillNo = paymentDet.new_receipt_no;
                        payment.AddDisc = paymentDet.Add_disc;
                        payment.IsOrderManual = paymentDet.isOrdermanual;
                        payment.CurrencyValue = paymentDet.currency_value;
                        payment.ExchangeRate = paymentDet.exchange_rate;
                        payment.CurrencyType = paymentDet.currency_type;
                        payment.TaxPercentage = paymentDet.tax_percentage;
                        payment.CancelledBy = paymentDet.cancelled_by;
                        payment.CancelledRemarks = paymentDet.cancelled_remarks;
                        payment.CancelledDate = paymentDet.cancelled_date;
                        payment.IsExchange = paymentDet.isExchange;
                        payment.ExchangeNo = paymentDet.exchangeNo;
                        payment.NewReceiptNo = paymentDet.new_receipt_no;
                        payment.GiftAmount = paymentDet.Gift_Amount;
                        payment.CardSwipedBy = paymentDet.cardSwipedBy;
                        payment.Version = paymentDet.version;
                        payment.GSTGroupCode = paymentDet.GSTGroupCode;
                        payment.SGSTPercent = paymentDet.SGST_Percent;
                        payment.CGSTPercent = paymentDet.CGST_Percent;
                        payment.IGSTPercent = paymentDet.IGST_Percent;
                        payment.HSN = paymentDet.HSN;
                        payment.SGSTAmount = paymentDet.SGST_Amount;
                        payment.CGSTAmount = paymentDet.CGST_Amount;
                        payment.IGSTAmount = paymentDet.IGST_Amount;
                        payment.PayAmountBeforeTax = paymentDet.pay_amount_before_tax;
                        payment.PayTaxAmount = paymentDet.pay_tax_amount;
                        lstOfPayment.Add(payment);
                    }
                    rimv.lstOfPayment = lstOfPayment;
                }
                #endregion
                return rimv;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public RepairIssueMasterVM GetRepairIssueDetailsTotalForPrint(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            try {
                RepairIssueDetailsVM totalRepairIssueDet = new RepairIssueDetailsVM();
                RepairIssueMasterVM rimv = new RepairIssueMasterVM();
                KTTU_REPAIR_ISSUE_MASTER krim = new KTTU_REPAIR_ISSUE_MASTER();
                List<KTTU_REPAIR_ISSUE_DETAILS> lstOfkrid = new List<KTTU_REPAIR_ISSUE_DETAILS>();
                List<KTTU_PAYMENT_DETAILS> lstOfPay = new List<KTTU_PAYMENT_DETAILS>();

                krim = db.KTTU_REPAIR_ISSUE_MASTER.Where(i => i.issue_no == issueNo
                                                            && i.company_code == companyCode
                                                            && i.branch_code == branchCode).FirstOrDefault();
                lstOfkrid = db.KTTU_REPAIR_ISSUE_DETAILS.Where(i => i.issue_no == issueNo
                                                            && i.company_code == companyCode
                                                            && i.branch_code == branchCode).ToList();
                lstOfPay = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == issueNo
                                                            && pay.trans_type == "RO"
                                                            && pay.company_code == companyCode
                                                            && pay.branch_code == branchCode).ToList();

                #region Issue Details
                List<RepairIssueDetailsVM> lstOfridvm = new List<RepairIssueDetailsVM>();
                foreach (KTTU_REPAIR_ISSUE_DETAILS krid in lstOfkrid) {
                    totalRepairIssueDet.Units = totalRepairIssueDet.Units + krid.units;
                    totalRepairIssueDet.GrossWt = totalRepairIssueDet.GrossWt + krid.gwt;
                    totalRepairIssueDet.StoneWt = totalRepairIssueDet.StoneWt + krid.swt;
                    totalRepairIssueDet.NetWt = Convert.ToInt32(totalRepairIssueDet.NetWt) + krid.nwt;
                    totalRepairIssueDet.Dcts = Convert.ToInt32(totalRepairIssueDet.Dcts) + krid.dcts;
                    totalRepairIssueDet.RepairAmount = Convert.ToInt32(totalRepairIssueDet.RepairAmount) + krid.repair_amount;
                    totalRepairIssueDet.SGSTPercent = Convert.ToInt32(totalRepairIssueDet.SGSTPercent) + krid.SGST_Percent;
                    totalRepairIssueDet.SGSTAmount = Convert.ToInt32(totalRepairIssueDet.SGSTAmount) + krid.SGST_Amount;
                    totalRepairIssueDet.CGSTPercent = Convert.ToInt32(totalRepairIssueDet.CGSTPercent) + krid.CGST_Percent;
                    totalRepairIssueDet.CGSTAmount = Convert.ToInt32(totalRepairIssueDet.CGSTAmount) + krid.CGST_Amount;
                    totalRepairIssueDet.IGSTPercent = Convert.ToInt32(totalRepairIssueDet.IGSTPercent) + krid.IGST_Percent;
                    totalRepairIssueDet.IGSTAmount = Convert.ToInt32(totalRepairIssueDet.IGSTAmount) + krid.IGST_Amount;
                    totalRepairIssueDet.Rate = Convert.ToInt32(totalRepairIssueDet.Rate) + krid.repair_amount + krid.SGST_Amount + krid.CGST_Amount + krid.IGST_Amount;
                }
                lstOfridvm.Add(totalRepairIssueDet);
                rimv.lstOfRepairIssueDetails = lstOfridvm;
                #endregion

                #region Payment Details
                if (lstOfPay != null && lstOfPay.Count > 0) {
                    List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                    foreach (KTTU_PAYMENT_DETAILS paymentDet in lstOfPay) {
                        PaymentVM payment = new PaymentVM();
                        payment.ObjID = paymentDet.obj_id;
                        payment.CompanyCode = paymentDet.company_code;
                        payment.BranchCode = paymentDet.branch_code;
                        payment.SeriesNo = paymentDet.series_no;
                        payment.ReceiptNo = paymentDet.receipt_no;
                        payment.SNo = paymentDet.sno;
                        payment.TransType = paymentDet.trans_type;
                        payment.PayMode = paymentDet.pay_mode;
                        payment.PayDetails = paymentDet.pay_details;
                        payment.PayDate = paymentDet.pay_date;
                        payment.PayAmount = paymentDet.pay_amt;
                        payment.RefBillNo = paymentDet.Ref_BillNo;
                        payment.PartyCode = paymentDet.party_code;
                        payment.BillCounter = paymentDet.bill_counter;
                        payment.IsPaid = paymentDet.is_paid;
                        payment.Bank = paymentDet.bank;
                        payment.BankName = paymentDet.bank_name;
                        payment.ChequeDate = paymentDet.cheque_date;
                        payment.CardType = paymentDet.card_type;
                        payment.ExpiryDate = paymentDet.expiry_date;
                        payment.CFlag = paymentDet.cflag;
                        payment.CardAppNo = paymentDet.card_app_no;
                        payment.SchemeCode = paymentDet.scheme_code;
                        payment.SalBillType = paymentDet.sal_bill_type;
                        payment.OperatorCode = paymentDet.operator_code;
                        payment.SessionNo = paymentDet.session_no;
                        payment.UpdateOn = paymentDet.UpdateOn;
                        payment.GroupCode = paymentDet.group_code;
                        payment.AmtReceived = paymentDet.amt_received;
                        payment.BonusAmt = paymentDet.bonus_amt;
                        payment.WinAmt = paymentDet.win_amt;
                        payment.CTBranch = paymentDet.ct_branch;
                        payment.FinYear = paymentDet.fin_year;
                        payment.CardCharges = paymentDet.CardCharges;
                        payment.ChequeNo = paymentDet.cheque_no;
                        payment.NewBillNo = paymentDet.new_receipt_no;
                        payment.AddDisc = paymentDet.Add_disc;
                        payment.IsOrderManual = paymentDet.isOrdermanual;
                        payment.CurrencyValue = paymentDet.currency_value;
                        payment.ExchangeRate = paymentDet.exchange_rate;
                        payment.CurrencyType = paymentDet.currency_type;
                        payment.TaxPercentage = paymentDet.tax_percentage;
                        payment.CancelledBy = paymentDet.cancelled_by;
                        payment.CancelledRemarks = paymentDet.cancelled_remarks;
                        payment.CancelledDate = paymentDet.cancelled_date;
                        payment.IsExchange = paymentDet.isExchange;
                        payment.ExchangeNo = paymentDet.exchangeNo;
                        payment.NewReceiptNo = paymentDet.new_receipt_no;
                        payment.GiftAmount = paymentDet.Gift_Amount;
                        payment.CardSwipedBy = paymentDet.cardSwipedBy;
                        payment.Version = paymentDet.version;
                        payment.GSTGroupCode = paymentDet.GSTGroupCode;
                        payment.SGSTPercent = paymentDet.SGST_Percent;
                        payment.CGSTPercent = paymentDet.CGST_Percent;
                        payment.IGSTPercent = paymentDet.IGST_Percent;
                        payment.HSN = paymentDet.HSN;
                        payment.SGSTAmount = paymentDet.SGST_Amount;
                        payment.CGSTAmount = paymentDet.CGST_Amount;
                        payment.IGSTAmount = paymentDet.IGST_Amount;
                        payment.PayAmountBeforeTax = paymentDet.pay_amount_before_tax;
                        payment.PayTaxAmount = paymentDet.pay_tax_amount;
                        lstOfPayment.Add(payment);
                    }
                    rimv.lstOfPayment = lstOfPayment;
                }
                #endregion

                return rimv;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public RepairReceiptMasterVM GetRepairIssueDetailsForCancel(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            int receiptNo = 0;
            KTTU_REPAIR_ISSUE_MASTER krim = db.KTTU_REPAIR_ISSUE_MASTER.Where(rep => rep.issue_no == issueNo
                                                                                && rep.company_code == companyCode
                                                                                && rep.branch_code == branchCode).FirstOrDefault();
            if (krim == null) {
                error = new ErrorVM()
                {
                    field = "Delivery Number",
                    index = 0,
                    description = "Invalid Delivery Number.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }
            else if (krim.cflag == "Y") {
                error = new ErrorVM()
                {
                    field = "Delivery Number",
                    index = 0,
                    description = "Delivery No: " + issueNo + " is already cancelled.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }
            else { receiptNo = krim.receipt_no; }
            RepairReceiptMasterVM details = new RepairBL().GetReceiptDetails(companyCode, branchCode, receiptNo, true, out error);
            if (error != null) {
                return null;
            }
            return details;
        }

        public int SaveRepairIssueDetails(RepairIssueMasterVM repairIssue, out ErrorVM error)
        {
            error = null;
            KTTU_REPAIR_ISSUE_MASTER krim = new KTTU_REPAIR_ISSUE_MASTER();
            KTTU_REPAIR_RECEIPT_MASTER receiptMaster = new KTTU_REPAIR_RECEIPT_MASTER();

            receiptMaster = db.KTTU_REPAIR_RECEIPT_MASTER.Where(receipt => receipt.Repair_no == repairIssue.RepairNo
                                                                && receipt.company_code == repairIssue.CompanyCode
                                                                && receipt.branch_code == repairIssue.BranchCode).FirstOrDefault();

            int finYear = SIGlobals.Globals.GetFinancialYear(db, repairIssue.CompanyCode, repairIssue.BranchCode);
            int issueNo = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.Where(f => f.company_code == repairIssue.CompanyCode
                                            && f.branch_code == repairIssue.BranchCode).FirstOrDefault().fin_year.ToString().Remove(0, 1)
                                            + db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == ISSUE_MODULE_SEQ_NO
                                                                && sq.company_code == repairIssue.CompanyCode
                                                                && sq.branch_code == repairIssue.BranchCode).FirstOrDefault().nextno);

            string objectID = SIGlobals.Globals.GetMagnaGUID(new string[] { TABLE_NAME_ISSUE, Convert.ToString(issueNo) }, repairIssue.CompanyCode, repairIssue.BranchCode);
            using (var transaction = db.Database.BeginTransaction()) {
                try {
                    krim.obj_id = objectID;
                    krim.company_code = repairIssue.CompanyCode;
                    krim.branch_code = repairIssue.BranchCode;
                    krim.issue_no = issueNo;
                    krim.receipt_no = repairIssue.RepairNo;
                    krim.issue_date = SIGlobals.Globals.GetApplicationDate(repairIssue.CompanyCode, repairIssue.BranchCode);
                    krim.sal_code = repairIssue.SalCode;
                    krim.operator_code = repairIssue.OperatorCode;
                    krim.total_repair_amount = repairIssue.TotalRepairAmount;
                    krim.cflag = "N";
                    krim.item_type = receiptMaster.repair_item;
                    krim.remark = repairIssue.Remark;
                    krim.UpdateOn = SIGlobals.Globals.GetDateTime();
                    krim.tax_percentage = repairIssue.TaxPercentage == null ? 0 : krim.tax_percentage;
                    krim.total_tax_amount = repairIssue.TotalTaxAmount;
                    krim.total_amount_before_tax = repairIssue.TotalAmountBeforeTax == null ? 0 : repairIssue.TotalAmountBeforeTax;
                    krim.ShiftID = repairIssue.ShiftID == null ? 0 : repairIssue.ShiftID;
                    krim.New_Bill_No = repairIssue.NewBillNo;
                    krim.UniqRowID = Guid.NewGuid();
                    krim.address1 = receiptMaster.address1 == null ? "" : receiptMaster.address1;
                    krim.address2 = receiptMaster.address2 == null ? "" : receiptMaster.address2;
                    krim.address3 = receiptMaster.address3 == null ? "" : receiptMaster.address3;
                    krim.city = receiptMaster.city == null ? "" : receiptMaster.city;
                    krim.cust_id = receiptMaster.cust_id;
                    krim.cust_name = receiptMaster.cust_name;
                    krim.mobile_no = receiptMaster.mobile_no;
                    krim.pan_no = receiptMaster.pan_no == null ? "" : receiptMaster.pan_no;
                    krim.pin_code = receiptMaster.pin_code == null ? "" : receiptMaster.pin_code;
                    krim.state = receiptMaster.state;
                    krim.state_code = receiptMaster.state_code;
                    krim.tin = receiptMaster.tin;
                    krim.store_location_id = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == repairIssue.CompanyCode
                                                                                && c.branch_code == repairIssue.BranchCode).FirstOrDefault().store_location_id;
                    db.KTTU_REPAIR_ISSUE_MASTER.Add(krim);

                    int SlNo = 1;
                    string gstGroupCode = string.Empty;
                    string HSN = string.Empty;
                    decimal sgstPercent = 0;
                    decimal cgstPercent = 0;
                    decimal igstPercent = 0;
                    foreach (RepairIssueDetailsVM ridvm in repairIssue.lstOfRepairIssueDetails) {
                        ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(item => item.gs_code == ridvm.GSCode && item.company_code == ridvm.CompanyCode && item.branch_code == ridvm.BranchCode).FirstOrDefault();
                        if (itemMaster != null) {
                            gstGroupCode = itemMaster.GSTGoodsGroupCode;
                            HSN = itemMaster.HSN;
                        }
                        KTTU_REPAIR_ISSUE_DETAILS krid = new KTTU_REPAIR_ISSUE_DETAILS();
                        krid.obj_id = objectID;
                        krid.company_code = repairIssue.CompanyCode;
                        krid.branch_code = repairIssue.BranchCode;
                        krid.issue_no = issueNo;
                        krid.receipt_no = repairIssue.RepairNo;
                        krid.sl_no = SlNo;
                        krid.item = ridvm.Item;
                        krid.units = ridvm.Units;
                        krid.gwt = ridvm.GrossWt;
                        krid.swt = ridvm.StoneWt;
                        krid.nwt = ridvm.NetWt;
                        krid.description = ridvm.Description;
                        krid.wastage_grms = ridvm.WastageGrams == null ? 0 : ridvm.WastageGrams;
                        krid.wtadd = ridvm.WtAdd == null ? 0 : ridvm.WtAdd;
                        krid.gold_rate = ridvm.GoldRate == null ? 0 : ridvm.GoldRate;
                        krid.gold_amount = ridvm.GoldAmount == null ? 0 : ridvm.GoldAmount;
                        krid.misc_amount = ridvm.MiscAmount == null ? 0 : ridvm.MiscAmount;
                        krid.repair_amount = ridvm.RepairAmount == null ? 0 : ridvm.RepairAmount;
                        krid.UpdateOn = ridvm.UpdateOn;
                        krid.gs_code = ridvm.GSCode;
                        krid.Fin_Year = ridvm.FinYear;
                        krid.itemwise_tax_percentage = ridvm.ItemwiseTaxPercentage == null ? 0 : ridvm.ItemwiseTaxPercentage;
                        krid.itemwise_tax_amount = ridvm.ItemwiseTaxAmount == null ? 0 : ridvm.ItemwiseTaxAmount;
                        krid.itemwise_total_amount_before_tax = ridvm.ItemwiseTotalAmountBeforeTax == null ? 0 : ridvm.ItemwiseTotalAmountBeforeTax;
                        krid.rate = ridvm.Rate == null ? 0 : ridvm.Rate;
                        krid.party_code = ridvm.PartyCode;
                        krid.GSTGroupCode = gstGroupCode; //ridvm.GSTGroupCode;
                        krid.SGST_Percent = ridvm.SGSTPercent == null ? 0 : ridvm.SGSTPercent;
                        sgstPercent = Convert.ToDecimal(krid.SGST_Percent);
                        krid.SGST_Amount = ridvm.SGSTAmount == null ? 0 : ridvm.SGSTAmount;
                        krid.CGST_Percent = ridvm.CGSTPercent == null ? 0 : ridvm.CGSTPercent;
                        cgstPercent = Convert.ToDecimal(krid.CGST_Percent);
                        krid.CGST_Amount = ridvm.CGSTAmount == null ? 0 : ridvm.CGSTAmount;
                        krid.IGST_Percent = ridvm.IGSTPercent == null ? 0 : ridvm.IGSTPercent;
                        igstPercent = Convert.ToDecimal(krid.IGST_Percent);
                        krid.IGST_Amount = ridvm.IGSTAmount == null ? 0 : ridvm.IGSTAmount;
                        krid.HSN = HSN;// Convert.ToString(db.usp_GetGSTHSNCode(ridvm.CompanyCode, ridvm.BranchCode, ridvm.GSCode, ridvm.Item).FirstOrDefault());// ridvm.HSN;
                        krid.dcts = ridvm.Dcts;
                        krid.UniqRowID = Guid.NewGuid();
                        db.KTTU_REPAIR_ISSUE_DETAILS.Add(krid);
                        SlNo++;

                        //Updating Stock
                        IssueStockUpdate(krid.gs_code, krid.item, krid.units, krid.gwt, krid.swt, krid.nwt, krid.company_code, krid.branch_code);
                    }

                    if (repairIssue.lstOfPayment != null && repairIssue.lstOfPayment.Count > 0) {
                        int paySlNo = 1;
                        string payGUID = objectID;
                        foreach (PaymentVM pvm in repairIssue.lstOfPayment) {
                            KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                            kpd.obj_id = payGUID;
                            kpd.company_code = repairIssue.CompanyCode;
                            kpd.branch_code = repairIssue.BranchCode;
                            kpd.series_no = krim.issue_no;
                            kpd.receipt_no = 0;
                            kpd.sno = paySlNo;
                            kpd.trans_type = "RO";
                            kpd.pay_mode = pvm.PayMode;
                            kpd.pay_details = pvm.PayDetails;
                            kpd.pay_date = SIGlobals.Globals.GetApplicationDate(repairIssue.CompanyCode, repairIssue.BranchCode);
                            kpd.pay_amt = pvm.PayAmount;
                            kpd.Ref_BillNo = pvm.RefBillNo;
                            kpd.party_code = pvm.CTBranch;
                            kpd.bill_counter = pvm.BillCounter;
                            kpd.is_paid = pvm.IsPaid;
                            kpd.bank = pvm.Bank;
                            kpd.bank_name = pvm.BankName;
                            kpd.cheque_date = pvm.ChequeDate == null ? SIGlobals.Globals.GetApplicationDate(pvm.CompanyCode, pvm.BranchCode) : pvm.ChequeDate;
                            kpd.card_type = pvm.CardType;
                            kpd.expiry_date = pvm.ChequeDate == null ? SIGlobals.Globals.GetApplicationDate(pvm.CompanyCode, pvm.BranchCode) : pvm.ChequeDate;
                            kpd.cflag = "N";
                            kpd.card_app_no = pvm.CardAppNo;
                            kpd.scheme_code = pvm.SchemeCode;
                            kpd.sal_bill_type = pvm.SalBillType;
                            kpd.operator_code = pvm.OperatorCode;
                            kpd.session_no = pvm.SessionNo;
                            kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                            kpd.group_code = pvm.GroupCode;
                            kpd.amt_received = pvm.AmtReceived;
                            kpd.bonus_amt = pvm.BonusAmt == null ? 0 : pvm.BonusAmt;
                            kpd.win_amt = pvm.WinAmt == null ? 0 : pvm.BonusAmt; ;
                            kpd.ct_branch = pvm.CTBranch;
                            kpd.fin_year = finYear;
                            kpd.CardCharges = pvm.CardCharges;
                            kpd.cheque_no = pvm.ChequeNo;
                            // Need to save Account code in ChequeNo column in the db which is used for account posting.
                            //kpd.cheque_no = pvm.PayMode == "R" ? db.KSTU_ACC_LEDGER_MASTER.Where(kalm => kalm.acc_name == pvm.BankName && kalm.acc_type == "R" && kalm.company_code == repairIssue.CompanyCode && kalm.branch_code == repairIssue.BranchCode).FirstOrDefault().acc_code.ToString() : pvm.ChequeNo;
                            //if (pvm.PayMode == "R") {
                            //    int accCode = Convert.ToInt32(pvm.BankName);
                            //    kpd.cheque_no = db.KSTU_ACC_LEDGER_MASTER.Where(kalm => kalm.acc_code == accCode && kalm.acc_type == "R" && pvm.CompanyCode == repairIssue.CompanyCode && pvm.BranchCode == repairIssue.BranchCode).FirstOrDefault().acc_code.ToString();
                            //}
                            //else {
                            //    kpd.cheque_no = pvm.ChequeNo;
                            //}
                            kpd.cheque_no = pvm.ChequeNo == null ? Convert.ToString(0) : pvm.ChequeNo;
                            kpd.New_Bill_No = pvm.NewBillNo;
                            kpd.Add_disc = pvm.AddDisc;
                            kpd.isOrdermanual = pvm.IsOrderManual;
                            kpd.currency_value = pvm.CurrencyValue;
                            kpd.exchange_rate = pvm.ExchangeRate;
                            kpd.currency_type = pvm.CurrencyType;
                            kpd.tax_percentage = pvm.TaxPercentage;
                            kpd.cancelled_by = pvm.CancelledBy;
                            kpd.cancelled_remarks = pvm.CancelledRemarks;
                            kpd.cancelled_date = pvm.CancelledDate;
                            kpd.isExchange = pvm.IsExchange;
                            kpd.exchangeNo = pvm.ExchangeNo;
                            kpd.new_receipt_no = pvm.NewReceiptNo;
                            kpd.Gift_Amount = pvm.GiftAmount;
                            kpd.cardSwipedBy = pvm.CardSwipedBy;
                            kpd.version = pvm.Version;
                            kpd.GSTGroupCode = gstGroupCode;//pvm.GSTGroupCode;
                            kpd.SGST_Percent = sgstPercent;// pvm.SGSTPercent;
                            kpd.CGST_Percent = cgstPercent;// pvm.CGSTPercent;
                            kpd.IGST_Percent = igstPercent;// pvm.IGSTPercent;
                            kpd.HSN = HSN;// pvm.HSN;
                            //kpd.SGST_Amount = pvm.SGSTAmount;
                            //kpd.CGST_Amount = pvm.CGSTAmount;
                            //kpd.IGST_Amount = pvm.IGSTAmount;
                            //kpd.pay_amount_before_tax = pvm.PayAmountBeforeTax;
                            //kpd.pay_tax_amount = pvm.PayTaxAmount;
                            kpd.UniqRowID = Guid.NewGuid();
                            db.KTTU_PAYMENT_DETAILS.Add(kpd);
                            paySlNo = paySlNo + 1;
                        }
                        db.SaveChanges();
                        // After doing db.SaveChanges() only we can get Issue Number in the db to do account posting.
                        // Account Update
                        ErrorVM procError = IssueAccountPostingWithProedure(krim.issue_no, repairIssue.CompanyCode, repairIssue.BranchCode);
                        if (procError != null) {
                            error = new ErrorVM()
                            {
                                field = "Account Update",
                                index = 0,
                                description = procError.description,
                                ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                            };
                            transaction.Rollback();
                            return 0;
                        }
                    }

                    // Updating Sequence Number
                    SIGlobals.Globals.UpdateSeqenceNumber(db, ISSUE_MODULE_SEQ_NO, repairIssue.CompanyCode, repairIssue.BranchCode);

                    //Update Issue No to Receipt No
                    receiptMaster.issue_no = issueNo;
                    receiptMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(receiptMaster).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();
                    return krim.issue_no;
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(excp);
                };
                return 0;
            }
        }

        public int UpdateRepairIssueDetails(int issueNo, RepairIssueMasterVM repairIssue, out ErrorVM error)
        {
            error = null;
            KTTU_REPAIR_ISSUE_MASTER krim = new KTTU_REPAIR_ISSUE_MASTER();
            int finYear = db.KSTU_ACC_FY_MASTER.Where(f => f.company_code == repairIssue.CompanyCode && f.branch_code == repairIssue.BranchCode).FirstOrDefault().fin_year;
            try {
                #region Delete Existing Info
                KTTU_REPAIR_ISSUE_MASTER delkrim = db.KTTU_REPAIR_ISSUE_MASTER.Where(r => r.issue_no == issueNo
                                                                                        && r.company_code == repairIssue.CompanyCode
                                                                                        && r.branch_code == repairIssue.BranchCode).FirstOrDefault();
                db.KTTU_REPAIR_ISSUE_MASTER.Remove(delkrim);

                List<KTTU_REPAIR_ISSUE_DETAILS> delkrid = db.KTTU_REPAIR_ISSUE_DETAILS.Where(r => r.issue_no == issueNo
                                                                                        && r.company_code == repairIssue.CompanyCode
                                                                                        && r.branch_code == repairIssue.BranchCode).ToList();
                foreach (KTTU_REPAIR_ISSUE_DETAILS krid in delkrid) {
                    db.KTTU_REPAIR_ISSUE_DETAILS.Remove(krid);
                }

                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(r => r.series_no == issueNo && r.trans_type == "RO"
                                                                                        && r.company_code == repairIssue.CompanyCode
                                                                                        && r.branch_code == repairIssue.BranchCode).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in payment) {
                    db.KTTU_PAYMENT_DETAILS.Remove(pay);
                }
                #endregion

                #region Update Repair Issue Master and Details
                string objectID = SIGlobals.Globals.GetMagnaGUID(new string[] { TABLE_NAME_ISSUE, Convert.ToString(issueNo) }, repairIssue.CompanyCode, repairIssue.BranchCode);
                krim.obj_id = objectID;
                krim.company_code = repairIssue.CompanyCode;
                krim.branch_code = repairIssue.BranchCode;
                krim.issue_no = issueNo;
                krim.receipt_no = repairIssue.ReceiptNo;
                krim.issue_date = repairIssue.IssueDate;
                krim.sal_code = repairIssue.SalCode;
                krim.operator_code = repairIssue.OperatorCode;
                krim.total_repair_amount = repairIssue.TotalRepairAmount;
                krim.cflag = repairIssue.CFlag;
                krim.item_type = repairIssue.ItemType;
                krim.remark = repairIssue.Remark;
                krim.UpdateOn = repairIssue.UpdateOn;
                krim.tax_percentage = repairIssue.TaxPercentage;
                krim.total_tax_amount = repairIssue.TotalTaxAmount;
                krim.total_amount_before_tax = repairIssue.TotalAmountBeforeTax;
                krim.ShiftID = repairIssue.ShiftID;
                krim.New_Bill_No = repairIssue.NewBillNo;
                db.KTTU_REPAIR_ISSUE_MASTER.Add(krim);
                db.SaveChanges();
                int SlNo = 1;
                foreach (RepairIssueDetailsVM ridvm in repairIssue.lstOfRepairIssueDetails) {
                    KTTU_REPAIR_ISSUE_DETAILS krid = new KTTU_REPAIR_ISSUE_DETAILS();
                    krid.obj_id = objectID;
                    krid.company_code = ridvm.CompanyCode;
                    krid.branch_code = ridvm.BranchCode;
                    krid.issue_no = krim.issue_no;
                    krid.receipt_no = ridvm.ReceiptNo;
                    krid.sl_no = SlNo;
                    krid.item = ridvm.Item;
                    krid.units = ridvm.Units;
                    krid.gwt = ridvm.GrossWt;
                    krid.swt = ridvm.StoneWt;
                    krid.nwt = ridvm.NetWt;
                    krid.description = ridvm.Description;
                    krid.wastage_grms = ridvm.WastageGrams;
                    krid.wtadd = ridvm.WtAdd;
                    krid.gold_rate = ridvm.GoldRate;
                    krid.gold_amount = ridvm.GoldAmount;
                    krid.misc_amount = ridvm.MiscAmount;
                    krid.repair_amount = ridvm.RepairAmount;
                    krid.UpdateOn = ridvm.UpdateOn;
                    krid.gs_code = ridvm.GSCode;
                    krid.Fin_Year = ridvm.FinYear;
                    krid.itemwise_tax_percentage = ridvm.ItemwiseTaxPercentage;
                    krid.itemwise_tax_amount = ridvm.ItemwiseTaxAmount;
                    krid.itemwise_total_amount_before_tax = ridvm.ItemwiseTotalAmountBeforeTax;
                    krid.rate = ridvm.Rate;
                    krid.party_code = ridvm.PartyCode;
                    krid.GSTGroupCode = ridvm.GSTGroupCode;
                    krid.SGST_Percent = ridvm.SGSTPercent;
                    krid.SGST_Amount = ridvm.SGSTAmount;
                    krid.CGST_Percent = ridvm.CGSTPercent;
                    krid.CGST_Amount = ridvm.CGSTAmount;
                    krid.IGST_Percent = ridvm.IGSTPercent;
                    krid.IGST_Amount = ridvm.IGSTAmount;
                    krid.HSN = ridvm.HSN;
                    krid.dcts = ridvm.Dcts;
                    db.KTTU_REPAIR_ISSUE_DETAILS.Add(krid);
                    db.SaveChanges();
                    SlNo++;
                }

                if (repairIssue.lstOfPayment != null) {
                    int paySlNo = 1;
                    foreach (PaymentVM pvm in repairIssue.lstOfPayment) {
                        KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                        kpd.obj_id = objectID;
                        kpd.company_code = pvm.CompanyCode;
                        kpd.branch_code = pvm.BranchCode;
                        kpd.series_no = krim.issue_no;
                        kpd.receipt_no = 0;
                        kpd.sno = paySlNo;
                        kpd.trans_type = "RO";
                        kpd.pay_mode = pvm.PayMode;
                        kpd.pay_details = pvm.PayDetails;
                        kpd.pay_date = SIGlobals.Globals.GetApplicationDate(repairIssue.CompanyCode, repairIssue.BranchCode);
                        kpd.pay_amt = pvm.PayAmount;
                        kpd.Ref_BillNo = pvm.RefBillNo;
                        kpd.party_code = pvm.CTBranch;
                        kpd.bill_counter = pvm.BillCounter;
                        kpd.is_paid = pvm.IsPaid;
                        kpd.bank = SIGlobals.Globals.GetBank(db, pvm.Bank, pvm.PayMode, pvm.CompanyCode, pvm.BranchCode);
                        kpd.bank_name = pvm.BankName;
                        kpd.cheque_date = pvm.ChequeDate;
                        kpd.card_type = pvm.CardType;
                        kpd.expiry_date = pvm.ExpiryDate;
                        kpd.cflag = "N";
                        kpd.card_app_no = pvm.CardAppNo;
                        kpd.scheme_code = pvm.SchemeCode;
                        kpd.sal_bill_type = pvm.SalBillType;
                        kpd.operator_code = pvm.OperatorCode;
                        kpd.session_no = pvm.SessionNo;
                        kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kpd.group_code = pvm.GroupCode;
                        kpd.amt_received = pvm.AmtReceived;
                        kpd.bonus_amt = pvm.BonusAmt == null ? 0 : pvm.BonusAmt;
                        kpd.win_amt = pvm.WinAmt == null ? 0 : pvm.BonusAmt; ;
                        kpd.ct_branch = pvm.CTBranch;
                        kpd.fin_year = finYear;
                        kpd.CardCharges = pvm.CardCharges;
                        kpd.cheque_no = pvm.ChequeNo;
                        kpd.cheque_no = pvm.PayMode == "R" ? pvm.Bank : pvm.ChequeNo;
                        kpd.New_Bill_No = pvm.NewBillNo;
                        kpd.Add_disc = pvm.AddDisc;
                        kpd.isOrdermanual = pvm.IsOrderManual;
                        kpd.currency_value = pvm.CurrencyValue;
                        kpd.exchange_rate = pvm.ExchangeRate;
                        kpd.currency_type = pvm.CurrencyType;
                        kpd.tax_percentage = pvm.TaxPercentage;
                        kpd.cancelled_by = pvm.CancelledBy;
                        kpd.cancelled_remarks = pvm.CancelledRemarks;
                        kpd.cancelled_date = pvm.CancelledDate;
                        kpd.isExchange = pvm.IsExchange;
                        kpd.exchangeNo = pvm.ExchangeNo;
                        kpd.new_receipt_no = pvm.NewReceiptNo;
                        kpd.Gift_Amount = pvm.GiftAmount;
                        kpd.cardSwipedBy = pvm.CardSwipedBy;
                        kpd.version = pvm.Version;
                        kpd.GSTGroupCode = pvm.GSTGroupCode;
                        kpd.SGST_Percent = pvm.SGSTPercent;
                        kpd.CGST_Percent = pvm.CGSTPercent;
                        kpd.IGST_Percent = pvm.IGSTPercent;
                        kpd.HSN = pvm.HSN;
                        kpd.SGST_Amount = pvm.SGSTAmount;
                        kpd.CGST_Amount = pvm.CGSTAmount;
                        kpd.IGST_Amount = pvm.IGSTAmount;
                        kpd.pay_amount_before_tax = pvm.PayAmountBeforeTax;
                        kpd.pay_tax_amount = pvm.PayTaxAmount;
                        kpd.UniqRowID = Guid.NewGuid();
                        db.KTTU_PAYMENT_DETAILS.Add(kpd);
                        paySlNo = paySlNo + 1;
                    }
                }
                #endregion
                db.SaveChanges();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return 0;
            }
            return krim.issue_no;
        }

        public bool CancelRepairDelivery(RepairReceiptMasterVM receipt, out ErrorVM error)
        {
            error = null;
            int issueNo = 0;
            try {

                KTTU_REPAIR_ISSUE_MASTER krish = db.KTTU_REPAIR_ISSUE_MASTER.Where(kri => kri.receipt_no == receipt.RepairNo
                                                                                    && kri.company_code == receipt.CompanyCode
                                                                                    && kri.branch_code == receipt.BranchCode).FirstOrDefault();
                if (krish == null) {
                    error = new ErrorVM()
                    {
                        field = "Delivery Number",
                        index = 0,
                        description = "Invalid Receipt Number.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return false;
                }
                else {
                    issueNo = krish.issue_no;
                }
                KTTU_REPAIR_ISSUE_MASTER krim = db.KTTU_REPAIR_ISSUE_MASTER.Where(i => i.issue_no == issueNo
                                                                                    && i.company_code == receipt.CompanyCode
                                                                                    && i.branch_code == receipt.BranchCode).FirstOrDefault();
                List<KTTU_REPAIR_ISSUE_DETAILS> krid = db.KTTU_REPAIR_ISSUE_DETAILS.Where(d => d.issue_no == issueNo && d.company_code == receipt.CompanyCode && d.branch_code == d.branch_code).ToList();

                foreach (KTTU_REPAIR_ISSUE_DETAILS rid in krid) {
                    //Updating Stock
                    CancelIssueStockUpdate(rid.gs_code, rid.item, rid.units, rid.gwt, rid.swt, rid.nwt, rid.company_code, rid.branch_code);
                }

                if (krim.cflag == "Y") {
                    error = new ErrorVM()
                    {
                        field = "Delivery Number",
                        index = 0,
                        description = "This delivery No already cancelled.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }

                //Todo: Need to check this condition after done day end
                if (Convert.ToDateTime(krim.issue_date).ToString("dd/MM/yyyy") != SIGlobals.Globals.GetApplicationDate(receipt.CompanyCode, receipt.BranchCode).ToString("dd/MM/yyyy")) {
                    error = new ErrorVM()
                    {
                        field = "Delivery Number",
                        index = 0,
                        description = "Only today's delivery can be cancelled",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                krim.cflag = "Y";
                krim.remark = receipt.Remarks;
                krim.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(krim).State = System.Data.Entity.EntityState.Modified;

                List<KTTU_PAYMENT_DETAILS> payments = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == issueNo
                                                                                    && pay.trans_type == "RO"
                                                                                    && pay.company_code == receipt.CompanyCode
                                                                                    && pay.branch_code == receipt.BranchCode).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in payments) {
                    pay.cflag = "Y";
                    pay.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(pay).State = System.Data.Entity.EntityState.Modified;

                    //Account Posting
                    //List<KSTU_ACC_VOUCHER_TRANSACTIONS> lstOfKAVT = db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(kavt => kavt.receipt_no == pay.receipt_no + "," + issueNo
                    //                                                                            && kavt.company_code == receipt.CompanyCode
                    //                                                                            && kavt.branch_code == receipt.BranchCode
                    //                                                                            && kavt.trans_type == "RO").ToList();

                    string strIssueNo = Convert.ToString(issueNo);
                    List<KSTU_ACC_VOUCHER_TRANSACTIONS> lstOfKAVT = db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(kavt => kavt.receipt_no == strIssueNo
                                                                                               && kavt.company_code == receipt.CompanyCode
                                                                                               && kavt.branch_code == receipt.BranchCode
                                                                                               && kavt.trans_type == "RO").ToList();
                    foreach (KSTU_ACC_VOUCHER_TRANSACTIONS kavt in lstOfKAVT) {
                        kavt.cflag = "Y";
                        kavt.UpdateOn = SIGlobals.Globals.GetDateTime();
                        db.Entry(kavt).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public RepairIssueMasterVM GetRepairIssueMasterWithReceipt(string companyCode, string branchCode, int receiptNo, bool isPrint, out ErrorVM error)
        {
            error = null;
            try {
                RepairIssueMasterVM rrvm = new RepairIssueMasterVM();
                List<RepairIssueDetailsVM> lstOfrrdvm = new List<RepairIssueDetailsVM>();

                KTTU_REPAIR_RECEIPT_MASTER krrm = db.KTTU_REPAIR_RECEIPT_MASTER.Where(r => r.Repair_no == receiptNo
                                                                                        && r.company_code == companyCode
                                                                                        && r.branch_code == branchCode).FirstOrDefault();
                List<KTTU_REPAIR_RECEIPT_DETAILS> lstOfkrrd = db.KTTU_REPAIR_RECEIPT_DETAILS.Where(r => r.Repair_no == receiptNo
                                                                                        && r.company_code == companyCode
                                                                                        && r.branch_code == branchCode).ToList();
                if (!isPrint) {
                    if (krrm == null) {
                        error = new ErrorVM()
                        {
                            field = "Repair Number",
                            index = 0,
                            description = "Invalid Repair Number.",
                            ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                        };
                        return null;
                    }

                    if (krrm.cflag == "Y") {
                        error = new ErrorVM()
                        {
                            field = "Repair Number",
                            index = 0,
                            description = "Repair Receipt No: " + receiptNo + " is already cancelled.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return null;
                    }

                    // Checking that already Issued or not
                    KTTU_REPAIR_ISSUE_MASTER issued = db.KTTU_REPAIR_ISSUE_MASTER.Where(r => r.company_code == companyCode && r.branch_code == branchCode && r.receipt_no == receiptNo).FirstOrDefault();
                    if (issued != null) {
                        error = new ErrorVM()
                        {
                            field = "Repair Number",
                            index = 0,
                            description = "Repaired Items are already delivered.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return null;
                    }
                }
                rrvm.ObjID = krrm.obj_id;
                rrvm.CompanyCode = krrm.company_code;
                rrvm.BranchCode = krrm.branch_code;
                rrvm.RepairNo = krrm.Repair_no;
                rrvm.CustID = krrm.cust_id;
                rrvm.CustName = krrm.cust_name;
                rrvm.RepairDate = krrm.repair_date;
                rrvm.SalCode = krrm.sal_code;
                rrvm.OperatorCode = krrm.operator_code;
                rrvm.DueDate = krrm.due_date;
                rrvm.TagWt = krrm.tgwt;
                rrvm.CFlag = krrm.cflag;
                rrvm.IssueNo = Convert.ToInt32(krrm.issue_no);
                rrvm.RepairItems = krrm.repair_item;
                rrvm.CancelledBy = krrm.cancelled_by;
                rrvm.Remarks = krrm.remark;
                rrvm.CustDueDate = krrm.custduedate;
                rrvm.UpdateOn = krrm.UpdateOn;
                rrvm.ShiftID = krrm.ShiftID;
                rrvm.NewBillNo = krrm.New_Bill_No;
                rrvm.Address1 = krrm.address1;
                rrvm.Address2 = krrm.address2;
                rrvm.Address3 = krrm.address3;
                rrvm.City = krrm.city;
                rrvm.PinCode = krrm.pin_code;
                rrvm.MobileNo = krrm.mobile_no;
                rrvm.State = krrm.state;
                rrvm.StateCode = krrm.state_code;
                rrvm.TIN = krrm.tin;
                rrvm.PANNo = krrm.pan_no;

                foreach (KTTU_REPAIR_RECEIPT_DETAILS krrd in lstOfkrrd) {
                    RepairIssueDetailsVM rrdvm = new RepairIssueDetailsVM();
                    rrdvm.ObjID = krrd.obj_id;
                    rrdvm.CompanyCode = krrd.company_code;
                    rrdvm.BranchCode = krrd.branch_code;
                    rrdvm.SlNo = krrd.sl_no;
                    rrdvm.Item = krrd.item;
                    rrdvm.Units = krrd.units;
                    rrdvm.GrossWt = krrd.gwt;
                    rrdvm.StoneWt = krrd.swt;
                    rrdvm.NetWt = krrd.nwt;
                    rrdvm.Description = krrd.description;
                    rrdvm.UpdateOn = krrd.UpdateOn;
                    rrdvm.GSCode = krrd.gs_code;
                    rrdvm.FinYear = krrd.Fin_Year;
                    rrdvm.Rate = krrd.rate;
                    rrdvm.PartyCode = krrd.party_code;
                    rrdvm.Dcts = krrd.dcts;
                    lstOfrrdvm.Add(rrdvm);
                }
                rrvm.lstOfRepairIssueDetails = lstOfrrdvm;
                return rrvm;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public RepairIssueDetailsVM RepairCalculation(RepairIssueDetailsVM details)
        {
            bool isInterState = false; // This value is hot coded;
            decimal SGST = 0.00M, CGST = 0.00M, IGST = 0.00M, SGST_Per = 0.00M, CGST_Per = 0.00M, IGST_Per = 0.00M, CESS_Per = 0.00M, CESS_Amt = 0.00M;
            string itemCode = db.ITEM_MASTER.Where(item => item.Item_Name == details.Item && item.gs_code == details.GSCode
                                                    && item.company_code == details.CompanyCode
                                                    && item.branch_code == details.BranchCode).FirstOrDefault().Item_code;
            string gstGroupCode = db.usp_GetGSTGoodsGroupCode(details.CompanyCode, details.BranchCode, details.GSCode, itemCode).FirstOrDefault();
            string hsnCode = db.usp_GetGSTHSNCode(details.CompanyCode, details.BranchCode, details.GSCode, itemCode).FirstOrDefault();
            GetGSTComponentValues(gstGroupCode, Convert.ToDecimal(details.RepairAmount), isInterState,
                out SGST_Per, out SGST, out CGST_Per, out CGST, out IGST_Per, out IGST, out CESS_Per, out CESS_Amt, details.CompanyCode, details.BranchCode);
            details.SGSTAmount = SGST;
            details.SGSTPercent = SGST_Per;
            details.CGSTAmount = CGST;
            details.CGSTPercent = CGST_Per;
            details.IGSTAmount = IGST;
            details.IGSTPercent = IGST_Per;
            details.HSN = hsnCode;
            details.GSTGroupCode = gstGroupCode;

            // Doing Reverse Calculation Here.
            decimal totalGSTPercent = 0;
            decimal amountBeforeTax = 0;
            if (SGST_Per != 0 && CGST_Per != 0) {
                totalGSTPercent = SGST_Per + CGST_Per;
                amountBeforeTax = Math.Round(Convert.ToDecimal(details.RepairAmount) / (1 + totalGSTPercent / 100), 2);
                details.RepairAmount = amountBeforeTax;
                details.SGSTAmount = Math.Round((amountBeforeTax * SGST_Per / 100), 2);
                details.CGSTAmount = Math.Round((amountBeforeTax * CGST_Per / 100), 2);
            }
            else {
                totalGSTPercent = IGST_Per;
                amountBeforeTax = Math.Round(Convert.ToDecimal(details.RepairAmount) / (1 + totalGSTPercent / 100), 2);
                details.RepairAmount = amountBeforeTax;
                details.IGSTAmount = Math.Round((amountBeforeTax * IGST_Per / 100), 2);
            }
            return details;
        }

        public string GetIssueDotMatrixPrint(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;

            KTTU_REPAIR_ISSUE_MASTER issueMaster = db.KTTU_REPAIR_ISSUE_MASTER.Where(issue => issue.company_code == companyCode && issue.branch_code == branchCode && issue.issue_no == issueNo).FirstOrDefault();
            if (issueMaster == null) return "";
            KTTU_REPAIR_RECEIPT_MASTER repairMaster = db.KTTU_REPAIR_RECEIPT_MASTER.Where(rep => rep.Repair_no == issueMaster.receipt_no && rep.company_code == companyCode && rep.branch_code == branchCode).FirstOrDefault();
            List<KTTU_REPAIR_ISSUE_DETAILS> issueDet = db.KTTU_REPAIR_ISSUE_DETAILS.Where(issue => issue.company_code == companyCode && issue.branch_code == branchCode && issue.issue_no == issueNo).ToList();
            try {

                KSTU_CUSTOMER_MASTER customer = new KSTU_CUSTOMER_MASTER();
                if (issueMaster.receipt_no > 0) {
                    customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.company_code == companyCode && cust.branch_code == branchCode && cust.cust_id == issueMaster.cust_id).FirstOrDefault();
                }

                StringBuilder sb = new StringBuilder();

                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode && com.branch_code == branchCode).FirstOrDefault();

                int width = 93;
                StringBuilder strdoubleTransLine = new StringBuilder();
                strdoubleTransLine.Append('=', width);
                StringBuilder strTransLine = new StringBuilder();
                strTransLine.Append('-', width);
                decimal Spaces = 0M;
                StringBuilder strSpaces = new StringBuilder();
                Spaces = ((width - strTransLine.Length) / 3);
                strSpaces.Append(' ', Convert.ToInt32(Spaces));
                string esc = Convert.ToChar(27).ToString();
                string Center = esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString();
                sb = SIGlobals.Globals.GetCompanyname(db, companyCode, branchCode);
                sb.AppendLine(string.Format("{0}{1}", Center, "Repair Delivery"));
                sb.AppendLine(string.Format("{0}{1}", Center, "ORIGINAL / DUPLICATE"));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.AppendLine(string.Format("{0,-14}{1,-42}{2,13}{3,11}", "Customer Name", ": " + customer.cust_name.ToString(), "Delivery No :", issueMaster.branch_code.ToString() + "/" + issueNo));
                if (Convert.ToString(customer.address1) != string.Empty)
                    sb.Append(string.Format("{0,-14}{1,-42}", "Address ", ": " + Convert.ToString(customer.address1)));
                else
                    sb.Append(string.Format("{0,14}{1,-42}", "", ""));
                sb.AppendLine(string.Format("{0,13}{1,11}", "Receipt No :", Convert.ToString(customer.branch_code) + "/" + repairMaster.Repair_no));
                if (Convert.ToString(customer.address2) != string.Empty)
                    sb.Append(string.Format("{0,15}{1,-42}", ":", Convert.ToString(customer.address2)));
                else
                    sb.Append(string.Format("{0,15}{1,-42}", "", ""));
                sb.AppendLine(string.Format("{0,13}{1,10}", "Date : ", Convert.ToString(issueMaster.issue_date)));
                if (customer.city.ToString() != string.Empty)
                    sb.AppendLine(string.Format("{0,15}{1,-65}", ":", Convert.ToString(customer.city).Trim() + " " + Convert.ToString(customer.pin_code)).Trim());

                if (Convert.ToString(customer.mobile_no) != string.Empty)
                    sb.AppendLine(string.Format("{0,-14}{1,-42}", "Mobile No ", ": " + Convert.ToString(customer.mobile_no)));
                if (Convert.ToString(customer.pan_no) != string.Empty)
                    sb.AppendLine(string.Format("{0,-14}{1,-42}", "PAN ", ": " + Convert.ToString(customer.pan_no)));

                else {
                    //string idquery = string.Format("select top(1) Doc_code,Doc_No from KSTU_CUSTOMER_ID_PROOF_DETAILS where cust_id='{0}' AND company_code='{1}' and branch_code='{2}'", dtCustDetails.Rows[0]["cust_id"].ToString(), CGlobals.CompanyCode, CGlobals.BranchCode);
                    //DataTable IdDt = CGlobals.GetDataTable(idquery);

                    List<KSTU_CUSTOMER_ID_PROOF_DETAILS> custIdProofDet = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(proof => proof.cust_id == customer.cust_id && proof.company_code == companyCode && proof.branch_code == branchCode).ToList();
                    if (custIdProofDet.Count > 0) {
                        if (!string.IsNullOrEmpty(custIdProofDet[0].Doc_code.ToString())) {
                            sb.AppendLine(string.Format("{0,-15}{1,-46}", custIdProofDet[0].Doc_code.ToString(), ": " + custIdProofDet[0].Doc_No.ToString()));
                        }
                    }
                }
                if (Convert.ToString(customer.state) != string.Empty)
                    sb.AppendLine(string.Format("{0,-14}{1,-42}", "State ", ": " + Convert.ToString(customer.state)));

                if (customer.state_code.ToString() != string.Empty)
                    sb.AppendLine(string.Format("{0,-14}{1,-42}", "State Code", ": " + Convert.ToString(customer.state_code)));


                sb.AppendLine(string.Format("{0,-14}{1,-11}{2,-55}", "", "#" + issueMaster.sal_code, issueMaster.operator_code));
                int Qty = 0;
                decimal Grosswt = 0, Stwt = 0, Ntwt = 0, Dcts = 0;
                decimal Amount = 0;
                decimal PaidAmount = 0;
                decimal CardCharges = 0;

                //string strReceiptDetails = string.Format("select description,units,gwt,swt,nwt,dcts,repair_amount,HSN ,SGST_Percent,CGST_Percent,IGST_Percent from KTTU_REPAIR_ISSUE_DETAILS where issue_no = {0} and company_code='{1}' and branch_code='{2}'", IssueNo, CGlobals.CompanyCode, CGlobals.BranchCode);
                //DataTable dtReceiptDetail = CGlobals.GetDataTable(strReceiptDetails);

                //string gstdetails = string.Format("select isnull(sum(SGST_Amount),0) as SGST_Amount,isnull(sum(CGST_Amount),0) as CGST_Amount,isnull(sum(IGST_Amount),0) as IGST_Amount from KTTU_REPAIR_ISSUE_DETAILS where issue_no = {0} and company_code='{1}' and branch_code='{2}'", IssueNo, CGlobals.CompanyCode, CGlobals.BranchCode);
                //DataTable dtGStDetail = CGlobals.GetDataTable(gstdetails);

                decimal sgstAmount = Convert.ToDecimal(issueDet.Sum(r => r.SGST_Amount));
                decimal cgstAmount = Convert.ToDecimal(issueDet.Sum(r => r.CGST_Amount));
                decimal igstAmount = Convert.ToDecimal(issueDet.Sum(r => r.IGST_Amount));

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.AppendLine(string.Format("{0,-6}{1,-36}{2,5}{3,4}{4,7}{5,7}{6,7}{7,7}{8,14}", "S.No", "Description", "HSN", "Qty", "Gr.Wt", "St.Wt", "Nt.Wt", "Dcts", "Taxable Amt"));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                decimal taxableAmount = 0;
                for (int i = 0; i < issueDet.Count; i++) {
                    int g = i;
                    sb.Append(string.Format("{0,-6}", ++g));
                    sb.Append(string.Format("{0,-36}", issueDet[i].description.ToString()));
                    sb.Append(string.Format("{0,5}", issueDet[i].HSN.ToString()));
                    sb.Append(string.Format("{0,4}", issueDet[i].units.ToString()));
                    sb.Append(string.Format("{0,7}", issueDet[i].gwt.ToString()));
                    sb.Append(string.Format("{0,7}", issueDet[i].swt.ToString()));
                    sb.Append(string.Format("{0,7}", issueDet[i].nwt.ToString()));
                    sb.Append(string.Format("{0,7}", issueDet[i].dcts.ToString()));
                    sb.AppendLine(string.Format("{0,14}", issueDet[i].repair_amount.ToString()));
                    Qty = Qty + Convert.ToInt32(issueDet[i].units);

                    Grosswt = Grosswt + Convert.ToDecimal(issueDet[i].gwt);
                    Stwt = Stwt + Convert.ToDecimal(issueDet[i].swt);
                    Ntwt = Ntwt + Convert.ToDecimal(issueDet[i].nwt);
                    Dcts = Dcts + Convert.ToDecimal(issueDet[i].dcts);
                    taxableAmount = taxableAmount + Convert.ToDecimal(issueDet[i].repair_amount);
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.AppendLine(string.Format("{0,-47}{1,-3}{2,-7}{3,-7}{4,-7}{5,-7}{6,14}", "TOTAL", Qty, Grosswt, Stwt, Ntwt, Dcts, taxableAmount));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                //Amount = taxableAmount + Convert.ToDecimal(dtGStDetail.Rows[0]["SGST_Amount"]) + Convert.ToDecimal(issueDet[0].CGST_Amount) + Convert.ToDecimal(issueDet[0].IGST_Amount);
                Amount = taxableAmount + Convert.ToDecimal(sgstAmount) + Convert.ToDecimal(cgstAmount) + Convert.ToDecimal(igstAmount);

                sb.AppendLine(string.Format("{0,75}{1,15}", "SGST " + issueDet[0].SGST_Percent + "% : ", issueDet[0].SGST_Amount));
                sb.AppendLine(string.Format("{0,75}{1,15}", "CGST " + issueDet[0].CGST_Percent + "% : ", issueDet[0].CGST_Amount));
                sb.AppendLine(string.Format("{0,75}{1,15}", "IGST " + issueDet[0].IGST_Percent + "% : ", issueDet[0].IGST_Amount));
                sb.AppendLine(string.Format("{0,75}{1,15}", "Grand Total : ", Amount));
                string billCounter = string.Empty;

                //string paymentdetails = string.Format("select pay_mode,pay_amt,bill_counter,card_type,bank_name,Ref_BillNo,cheque_no,CardCharges,pay_details from KTTU_PAYMENT_DETAILS where series_no = {0} and trans_type = 'RO' and company_code = '{1}' and branch_code = '{2}' order by sno", IssueNo, CGlobals.CompanyCode, CGlobals.BranchCode);
                //DataTable dtpaymentdetails = CGlobals.GetDataTable(paymentdetails);

                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode && pay.branch_code == branchCode && pay.trans_type == "RO" && pay.series_no == issueNo).ToList();

                if (payment != null && payment.Count > 0) {
                    for (int i = 0; i < payment.Count; i++) {
                        string mode = payment[i].pay_mode.ToString();
                        decimal PayAmt = Convert.ToDecimal(payment[i].pay_amt);
                        PayAmt = Math.Round(PayAmt, 0, MidpointRounding.ToEven);
                        if (string.Compare(mode, "C") == 0)
                            sb.AppendLine(string.Format("{0,67}{1,10}{2,13}", "Cash Received :", "RS   :", PayAmt.ToString("N")));
                        else if (string.Compare(mode, "Q") == 0)
                            sb.AppendLine(string.Format("{0,67}{1,10}{2,13}", "By Cheque Drawn on :" + Convert.ToString(payment[i].bank_name) + "/" + payment[i].cheque_no.ToString().Trim(), "RS   :", PayAmt.ToString("N")));
                        else if (string.Compare(mode, "R") == 0)
                            sb.AppendLine(string.Format("{0,67}{1,10}{2,13}", "By CC :" + Convert.ToString(payment[i].bank_name) + "/" + payment[i].card_type.ToString().Trim() + "/" + payment[i].Ref_BillNo.ToString(), "RS   :", PayAmt.ToString("N")));
                        else if (string.Compare(mode, "EP") == 0)
                            sb.AppendLine(string.Format("{0,67}{1,10}{2,13}", "E-Payment :", "", PayAmt.ToString("N")));
                        else {
                            string payname = SIGlobals.Globals.PaymentMode(mode);
                            sb.AppendLine(string.Format("{0,67}{1,7}{2,13}", payname + "-" + payment[i].pay_details.ToString(), "RS :", PayAmt.ToString("N")));
                        }
                    }
                    object temp = payment.Sum(p => p.pay_amt);
                    if (temp != null && temp != DBNull.Value)
                        PaidAmount = Convert.ToDecimal(temp);
                    billCounter = payment[0].bill_counter == null ? "" : payment[0].bill_counter.ToString();
                }
                Amount = Amount - PaidAmount;
                Amount = Math.Round(Amount, 0, MidpointRounding.ToEven);
                if (Amount > 0) {
                    System.Globalization.CultureInfo indianDefaultCulture = new System.Globalization.CultureInfo("hi-IN");
                    sb.Append(string.Format("{0,20}", ""));
                    sb.Append(esc + Convert.ToChar(69).ToString());
                    sb.Append(esc + Convert.ToChar(14).ToString() + Convert.ToChar(14).ToString());
                    sb.Append(esc + Convert.ToChar(52).ToString());
                    string temp = string.Format("{0,47}{1,5}", "Balance:", Amount.ToString("n", indianDefaultCulture));
                    sb.Append(string.Format("{0}", temp));
                    sb.Append(esc + Convert.ToChar(53).ToString());
                    sb.Append(Convert.ToChar(20).ToString() + Convert.ToChar(10).ToString());
                    sb.Append(esc + Convert.ToChar(70).ToString());
                }
                else if (Amount == 0) {
                    System.Globalization.CultureInfo indianDefaultCulture = new System.Globalization.CultureInfo("hi-IN");
                    sb.Append(string.Format("{0,20}", ""));
                    sb.Append(esc + Convert.ToChar(69).ToString());
                    sb.Append(esc + Convert.ToChar(14).ToString() + Convert.ToChar(14).ToString());
                    sb.Append(esc + Convert.ToChar(52).ToString());
                    string temp = string.Format("{0,47}{1,5}", "Balance: ", "NIL");
                    sb.Append(string.Format("{0}", temp));
                    sb.Append(esc + Convert.ToChar(53).ToString());
                    sb.Append(Convert.ToChar(20).ToString() + Convert.ToChar(10).ToString());
                    sb.Append(esc + Convert.ToChar(70).ToString());

                }
                object temp1 = payment.Sum(p => p.CardCharges);
                if (temp1 != null && temp1 != DBNull.Value)
                    CardCharges = Convert.ToDecimal(temp1);
                if (CardCharges > 0) {
                    sb.AppendLine(string.Format("{0,90}", "Card Charges : " + CardCharges));
                }
                if (!string.IsNullOrEmpty(issueMaster.remark == null ? "" : issueMaster.remark.ToString())) {
                    sb.AppendLine(string.Format("Remarks:{0}", issueMaster.remark == null ? "" : issueMaster.remark.ToString()));
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(70).ToString());
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(69).ToString());
                sb.AppendLine(string.Format("{0,-40}{1,40}", "Customer Signatory", "Authorized Signatory"));
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(70).ToString());
                sb.Append(Convert.ToChar(18).ToString());
                sb.Append(esc + Convert.ToChar(64).ToString());
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string GetIssueHTMLPrint(string companyCode, string branchCode, int issueNo, string printType, out ErrorVM error)
        {
            error = null;

            KTTU_REPAIR_ISSUE_MASTER issueMaster = db.KTTU_REPAIR_ISSUE_MASTER.Where(i => i.company_code == companyCode
                                                                                           && i.branch_code == branchCode
                                                                                           && i.issue_no == issueNo).FirstOrDefault();
            if (issueMaster == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Issue/Delivery Number",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return "";
            }

            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            List<KTTU_REPAIR_ISSUE_DETAILS> issueDet = db.KTTU_REPAIR_ISSUE_DETAILS.Where(i => i.company_code == companyCode
                                                                                     && i.branch_code == branchCode
                                                                                     && i.issue_no == issueNo).ToList();

            KSTU_CUSTOMER_MASTER customer = new CustomerBL().GetActualCustomerDetails(Convert.ToInt32(issueMaster.cust_id), issueMaster.mobile_no, companyCode, branchCode);
            KSTU_CUSTOMER_ID_PROOF_DETAILS proof = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cust => cust.cust_id == customer.cust_id
                                                                                            && cust.company_code == companyCode
                                                                                            && cust.branch_code == branchCode).FirstOrDefault();
            try {
                string CompanyAddress = string.Empty;
                if (company.address1.ToString() != string.Empty)
                    CompanyAddress = company.address1;
                if (company.address2.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br>" + company.address2;
                if (company.address3.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br>" + company.address3;
                if (company.tin_no.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br>" + "TIN: " + company.tin_no;
                if (company.phone_no.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br>" + "Phone No: " + company.phone_no;
                if (company.email_id.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br> E-mail : " + company.email_id;
                if (company.website.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br> Website : " + company.website;


                StringBuilder sbStart = new StringBuilder();
                sbStart.AppendLine("<html>");
                sbStart.AppendLine("<head>");
                sbStart.AppendLine(SIGlobals.Globals.GetStyleOrder());
                sbStart.AppendLine("</head>");
                sbStart.AppendLine("<body>");

                string DateTime = string.Format("{0:dd/MM/yyyy}", (issueMaster.issue_date));

                sbStart.AppendLine("<Table frame=\"border\" border=\"0\" width=\"800\">");
                for (int j = 0; j < 8; j++) {
                    sbStart.AppendLine("<TR style=border:0>");
                    sbStart.AppendLine(string.Format("<TD style=border:0 colspan = 8 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                sbStart.AppendLine("</Table>");
                sbStart.AppendLine("<Table font-size=14pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"800\">");
                string Name = customer.cust_name;
                string Address1 = customer.address1;
                string Address2 = customer.address2;
                string Address3 = customer.address3;
                string City = customer.city;
                string state = customer.state;
                string state_code = Convert.ToString(customer.state_code);
                string Mobile = customer.mobile_no;
                string PAN = customer.pan_no;
                string GSTIN = customer.tin;
                string Address = string.Empty;

                if (Address1 != string.Empty)
                    Address = Address + "<br>" + Address1;
                if (Address2 != string.Empty)
                    Address = Address + "<br>" + Address2;

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan =2 ALIGN = \"left\"><b>GSTIN:{0}</b></TD>", company.tin_no.ToString()));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.company_name.ToString()));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"CENTER\"><b>{0},{1},{2},{3}-{4}</b></TD>", company.address1, company.address2, company.address3, company.city, company.pin_code.ToString()));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; \"  colspan=2 ALIGN = \"CENTER\"><b>{0}</b></TD>", "GST INVOICE"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2  ALIGN = \"CENTER\"><b>{0}:{1}</b></TD>", "Supply of service", "Jewellery Repair Service"));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b> DETAILS OF CUSTOMER </b></TD>"));
                sbStart.AppendLine(string.Format("<TD width=\"400\"  ALIGN = \"LEFT\"><b>{0}{1}</b></TD>", "Receipt No :", issueMaster.issue_no.ToString()));
                sbStart.AppendLine("</TR>");


                sbStart.AppendLine("<tr>");


                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + Name + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address1 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address2 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address3 + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" tyle=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + City + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + state + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + state_code + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Mobile + "</b></td>");
                sbStart.AppendLine("</tr>");
                if (!string.IsNullOrEmpty(PAN)) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + PAN + "</b></td>");
                    sbStart.AppendLine("</tr>");
                }
                else {
                    if (proof != null) {
                        if (!string.IsNullOrEmpty(proof.Doc_code.ToString())) {
                            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                            sbStart.AppendLine(string.Format("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>{0} &nbsp&nbsp</b></td>", proof.Doc_code.ToString()));
                            sbStart.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", proof.Doc_No.ToString()));
                            sbStart.AppendLine("</tr>");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(GSTIN)) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>GSTIN &nbsp&nbsp</b></td>");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + GSTIN + "</b></td>");
                    sbStart.AppendLine("</tr>");
                }
                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");

                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sbStart.AppendLine("<tr style=\"border-right:0\">");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"center \" ><b>Repair No</b></td>");
                sbStart.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" ><b>  {0}/{1}</b></TD>", branchCode, issueMaster.receipt_no));
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"left\" ><b>Date &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + DateTime + "</b></td>");
                sbStart.AppendLine("</tr>");


                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" ><b> Place of supply &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + company.state + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + company.state + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin  ;border-top:thin\" ><b>" + company.state_code + "</b></td>");
                sbStart.AppendLine("</tr>");


                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table>");
                sbStart.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; border-top:thin\" width=\"800\">");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD  style=\"border-right:thin; border-top:thin \" colspan=16 ALIGN = \"CENTER\"><b>ORIGINAL / DUPLICATE <br></b></TD>"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR bgcolor='#FFFACD' align=\"CENTER\">");

                sbStart.AppendLine("<TD style=\"border-bottom:thin\" ALIGN = \"center\"><b></b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin\"   ALIGN = \"center\"><b>Service</b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin\"  ALIGN = \"center\"><b></b></TD>");

                sbStart.AppendLine("<TD style=\"border-bottom:thin\"   ALIGN = \"CENTER\"><b></b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin\"  ALIGN = \"center\"><b></b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin\"  ALIGN = \"center\"><b></b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin\"  ALIGN = \"center\"><b></b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin\"  ALIGN = \"center\"><b></b></TD>");

                sbStart.AppendLine("<TD style=\"border-bottom:thin\"    ALIGN = \"center\"><b>Taxable</b></TD>");
                sbStart.AppendLine("<TD  colspan=2 ALIGN = \"center\"><b>SGST</b></TD>");
                sbStart.AppendLine("<TD  colspan=2  ALIGN = \"center\"><b>CGST</b></TD>");
                sbStart.AppendLine("<TD  colspan=2  ALIGN = \"center\"><b>IGST</b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin\"   ALIGN = \"center\"><b>Total</b></TD>");
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR bgcolor='#FFFACD' align=\"CENTER\">");

                sbStart.AppendLine("<TD style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"center\"><b>Sl</b></TD>");
                sbStart.AppendLine("<TD  style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"center\"><b>Description</b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin solid; border-top:thin\"  ALIGN = \"center\"><b>HSN/SAC</b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Qty</b></TD>");

                sbStart.AppendLine("<TD style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Gr.Wt</b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>St.Wt</b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Nt.Wt</b></TD>");
                sbStart.AppendLine("<TD style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Dcts</b></TD>");

                sbStart.AppendLine("<TD style=\"border-bottom:thin solid; border-top:thin\"  ALIGN = \"center\"><b>Amount</b></TD>");
                sbStart.AppendLine("<TD  style=\"border-bottom:thin solid \"  ALIGN = \"center\"><b>Rate</b></TD>");
                sbStart.AppendLine("<TD  style=\"border-bottom:thin solid \"  ALIGN = \"center\"><b>Amount</b></TD>");
                sbStart.AppendLine("<TD  style=\"border-bottom:thin solid \"  ALIGN = \"center\"><b>Rate</b></TD>");
                sbStart.AppendLine("<TD  style=\"border-bottom:thin solid \"  ALIGN = \"center\"><b>Amount</b></TD>");
                sbStart.AppendLine("<TD  style=\"border-bottom:thin solid \"  ALIGN = \"center\"><b>Rate</b></TD>");
                sbStart.AppendLine("<TD  style=\"border-bottom:thin solid \" ALIGN = \"center\"><b>Amount</b></TD>");
                sbStart.AppendLine("<TD  style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"center\"><b>Amount</b></TD>");
                sbStart.AppendLine("</TR>");

                int Qty = 0;
                decimal Grosswt = 0, Stwt = 0, Ntwt = 0, Dcts = 0;
                decimal NetWt = 0;
                decimal Amount = 0, totalSGST = 0, totalCGST = 0, totalIGST = 0, totalAmount = 0, totAmount = 0;
                int z = 1;
                decimal PaidAmount = 0;
                decimal CardCharges = 0;

                for (int i = 0; i < issueDet.Count; i++) {

                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"LEFT\"><b>{0}</b>{1}{1} </TD>", i + 1, "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\"  ALIGN = \"center\" ><b>{0}</b> </TD>", issueDet[i].description.ToString()));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"center\" ><b>{0}</b></TD>", issueDet[i].HSN.ToString()));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\"  ALIGN = \"right\"><b>{0}</b> </TD>", issueDet[i].units.ToString()));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\"  ALIGN = \"right\"><b>{0}</b> </TD>", issueDet[i].gwt.ToString()));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\"  ALIGN = \"right\"><b>{0}</b> </TD>", issueDet[i].swt.ToString()));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\"  ALIGN = \"right\"><b>{0}</b> </TD>", issueDet[i].nwt.ToString()));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\"  ALIGN = \"right\"><b>{0}</b> </TD>", issueDet[i].dcts.ToString()));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"RIGHT\"><b>{0}</b> </TD>", issueDet[i].repair_amount.ToString()));
                    if (Convert.ToDecimal(issueDet[i].SGST_Percent) > 0)
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"right\"><b>{0} %</b> </TD>", issueDet[i].SGST_Percent.ToString()));
                    else
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"right\"><b>{0}</b> </TD>", issueDet[i].SGST_Percent.ToString()));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"right\"><b>{0}</b> </TD>", issueDet[i].SGST_Amount.ToString()));
                    if (Convert.ToDecimal(issueDet[i].CGST_Percent) > 0)
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"right\"><b>{0} %</b> </TD>", issueDet[i].CGST_Percent.ToString()));
                    else
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"right\"><b>{0}</b> </TD>", issueDet[i].CGST_Percent.ToString()));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\"  ALIGN = \"right\"><b>{0}</b> </TD>", issueDet[i].CGST_Amount.ToString()));
                    if (Convert.ToDecimal(issueDet[i].IGST_Percent) > 0)
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"right\"><b>{0} %</b> </TD>", issueDet[i].IGST_Percent.ToString()));
                    else
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"right\"><b>{0}</b> </TD>", issueDet[i].IGST_Percent.ToString()));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"right\"><b>{0}</b> </TD>", issueDet[i].IGST_Amount.ToString()));

                    totAmount = Convert.ToDecimal(issueDet[i].repair_amount) + Convert.ToDecimal(issueDet[i].SGST_Amount) + Convert.ToDecimal(issueDet[i].CGST_Amount) + Convert.ToDecimal(issueDet[i].IGST_Amount);
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ALIGN = \"right\"><b>{0}</b> </TD>", totAmount));

                    Qty = Qty + Convert.ToInt32(issueDet[i].units);


                    Grosswt = Grosswt + Convert.ToDecimal(issueDet[i].gwt);

                    Stwt = Stwt + Convert.ToDecimal(issueDet[i].swt);

                    Ntwt = Ntwt + Convert.ToDecimal(issueDet[i].nwt);

                    Dcts = Dcts + Convert.ToDecimal(issueDet[i].dcts);


                    Amount = Amount + Convert.ToDecimal(issueDet[i].repair_amount);

                    totalSGST += Convert.ToDecimal(issueDet[i].SGST_Amount);

                    totalCGST += Convert.ToDecimal(issueDet[i].CGST_Amount);

                    totalIGST += Convert.ToDecimal(issueDet[i].IGST_Amount);

                }
                totalAmount = Amount + totalSGST + totalIGST + totalCGST;

                int MaxPageRow = 5;
                for (int j = 0; j < MaxPageRow - issueDet.Count; j++) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin;  border-top:thin\" ><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }

                sbStart.AppendLine("<TR  bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine(string.Format("<TD  style=\" border-top:thin solid ; border-bottom:thin solid ;\" ALIGN = \"LEFT\" colspan=3 ><b>Total{0}</b></TD>", "&nbsp"));
                sbStart.AppendLine(string.Format("<TD style=\" border-top:thin solid; border-bottom:thin solid\"  ALIGN = \"right\"><b>{0}{1}</b></TD>", Qty, "&nbsp"));
                sbStart.AppendLine(string.Format("<TD style=\" border-top:thin solid; border-bottom:thin solid\"  ALIGN = \"right\"><b>{0}{1}</b></TD>", Grosswt, "&nbsp"));
                sbStart.AppendLine(string.Format("<TD style=\" border-top:thin solid; border-bottom:thin solid\"  ALIGN = \"right\"><b>{0}{1}</b></TD>", Stwt, "&nbsp"));
                sbStart.AppendLine(string.Format("<TD style=\" border-top:thin solid; border-bottom:thin solid\"  ALIGN = \"right\"><b>{0}{1}</b></TD>", Ntwt, "&nbsp"));
                sbStart.AppendLine(string.Format("<TD style=\" border-top:thin solid; border-bottom:thin solid\"  ALIGN = \"right\"><b>{0}{1}</b></TD>", Dcts, "&nbsp"));
                sbStart.AppendLine(string.Format("<TD style=\" border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>", Amount, "&nbsp"));
                sbStart.AppendLine("<TD style=\" border-top:thin solid; border-bottom:thin solid\" ALIGN = \"LEFT\"><b></b></TD>");
                sbStart.AppendLine(string.Format("<TD style=\" border-top:thin solid; border-bottom:thin solid; border-bottom:thin solid\"  ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>", totalSGST, "&nbsp"));
                sbStart.AppendLine("<TD style=\" border-top:thin solid; border-bottom:thin solid\" ALIGN = \"LEFT\"><b></b></TD>");
                sbStart.AppendLine(string.Format("<TD style=\" border-top:thin solid; border-bottom:thin solid\"  ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>", totalCGST, "&nbsp"));
                sbStart.AppendLine("<TD style=\" border-top:thin solid; border-bottom:thin solid\" ALIGN = \"LEFT\"><b></b></TD>");
                sbStart.AppendLine(string.Format("<TD style=\" border-top:thin solid; border-bottom:thin solid\"  ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>", totalIGST, "&nbsp"));
                sbStart.AppendLine(string.Format("<TD style=\" border-top:thin solid; border-bottom:thin solid\"  ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>", totalAmount, "&nbsp"));
                sbStart.AppendLine("</TR>");

                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                                    && pay.branch_code == branchCode
                                                                                    && pay.series_no == issueNo
                                                                                    && pay.trans_type == "RO").OrderBy(p => p.sno).ToList();
                if (payment != null && payment.Count > 0) {
                    for (int i = 0; i < payment.Count; i++) {
                        string mode = payment[i].pay_mode.ToString();
                        decimal PayAmt = Convert.ToDecimal(payment[i].pay_amt);
                        PayAmt = Math.Round(PayAmt, 0, MidpointRounding.ToEven);
                        if (string.Compare(mode, "C") == 0) {
                            sbStart.AppendLine("<TR>");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=15 ALIGN = \"RIGHT\"><h3>Cash Received</h3></TD>"));
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><h3>{0}{1}{1}</h3></TD>", PayAmt.ToString("N"), "&nbsp"));
                            sbStart.AppendLine("</TR>");
                        }
                        else if (string.Compare(mode, "Q") == 0) {
                            sbStart.AppendLine("<TR>");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=15 ALIGN = \"RIGHT\"><b>By Cheque Drawn on/{0}/{1} </b></TD>"
                                , payment[i].bank_name.ToString().Trim(), payment[i].cheque_no.ToString().Trim()));
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                            sbStart.AppendLine("</TR>");
                        }
                        else if (string.Compare(mode, "R") == 0) {
                            sbStart.AppendLine("<TR>");
                            sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" colspan=15 ALIGN = \"RIGHT\"><b>Card/{0}/{1}/{2} </b></TD>"
                                , payment[i].bank_name.ToString().Trim(), payment[i].card_type.ToString().Trim()
                                , payment[i].Ref_BillNo.ToString()));
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                            sbStart.AppendLine("</TR>");
                        }
                        else if (string.Compare(mode, "EP") == 0) {
                            sbStart.AppendLine("<TR>");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=15 ALIGN = \"RIGHT\"><b>E-Payment {0}</b></TD>", "&nbsp"));
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                            sbStart.AppendLine("</TR>");
                        }
                        else {
                            string payMode = payment[i].pay_mode;
                            KTTS_PAYMENT_MASTER payMaster = db.KTTS_PAYMENT_MASTER.Where(p => p.company_code == companyCode && p.branch_code == branchCode && p.payment_code == payMode).FirstOrDefault();
                            string payname = payMaster.payment_name;
                            sbStart.AppendLine("<TR>");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=15 ALIGN = \"RIGHT\"><b>{0} {1} </b></TD>", payname + "-" + payment[i].pay_details.ToString(), " & nbsp"));
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                            sbStart.AppendLine("</TR>");
                        }
                    }
                    object temp = payment.Sum(p => p.pay_amt);
                    if (temp != null && temp != DBNull.Value)
                        PaidAmount = Convert.ToDecimal(temp);
                }
                totalAmount = totalAmount - PaidAmount;
                totalAmount = Math.Round(totalAmount, 0, MidpointRounding.ToEven);
                if (totalAmount > 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=15 ALIGN = \"RIGHT\"><b>Balance</b></TD>"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", Math.Round(totalAmount, 0, MidpointRounding.ToEven), "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                else if (totalAmount == 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=15 ALIGN = \"RIGHT\"><b>Balance</b></TD>"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", "NIL", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                object temp1 = payment.Sum(p => p.CardCharges);
                if (temp1 != null && temp1 != DBNull.Value)
                    CardCharges = Convert.ToDecimal(temp1);
                if (CardCharges > 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=15 ALIGN = \"RIGHT\"><b>Card Charges :</b></TD>"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", CardCharges, "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }

                string strWords = string.Empty;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(totalAmount), out strWords);
                strWords = strWords.Replace("Rupees", "");
                strWords = "Rupees " + strWords;
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin \" colspan=16 ALIGN = \"lef\"><b> {0} </b></TD>", strWords));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\" border-top:thin ; border-bottom:thin \" colspan=16 ALIGN = \"lef\"><b>SM Code : {0} </b></TD>", issueMaster.sal_code));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD colspan=8 style=\"border-right:thin\"  ALIGN = \"LEFT\"><b><br><br><br><br><br>{0}</b></TD>", "Customer Signature"));
                sbStart.AppendLine(string.Format("<TD  colspan=8 ALIGN = \"RIGHT\"><b>For {0}<br><br><br><br><br>{1}</b></TD>", company.company_name, "Authorized Signatory"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table>");
                sbStart.AppendLine("</body>");
                sbStart.AppendLine("</html>");
                return sbStart.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string GetIssueHTMLPrintDirect(string companyCode, string branchCode, int issueNo, string printType, out ErrorVM error)
        {
            error = null;
            KTTU_REPAIR_ISSUE_MASTER issueMaster = db.KTTU_REPAIR_ISSUE_MASTER.Where(i => i.company_code == companyCode
                                                                                          && i.branch_code == branchCode
                                                                                          && i.issue_no == issueNo).FirstOrDefault();
            if (issueMaster == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Issue/Delivery Number",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return "";
            }

            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            List<KTTU_REPAIR_ISSUE_DETAILS> issueDet = db.KTTU_REPAIR_ISSUE_DETAILS.Where(i => i.company_code == companyCode
                                                                                     && i.branch_code == branchCode
                                                                                     && i.issue_no == issueNo).ToList();

            KSTU_CUSTOMER_MASTER customer = new CustomerBL().GetActualCustomerDetails(Convert.ToInt32(issueMaster.cust_id), issueMaster.mobile_no, companyCode, branchCode);
            KSTU_CUSTOMER_ID_PROOF_DETAILS proof = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cust => cust.cust_id == customer.cust_id
                                                                                            && cust.company_code == companyCode
                                                                                            && cust.branch_code == branchCode).FirstOrDefault();

            try {
                string CompanyAddress = string.Empty;
                if (company.address1.ToString() != string.Empty)
                    CompanyAddress = company.address1;
                if (company.address2.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br>" + company.address2;
                if (company.address3.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br>" + company.address3;
                if (company.tin_no.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br>" + "TIN: " + company.tin_no;
                if (company.phone_no.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br>" + "Phone No: " + company.phone_no;
                if (company.email_id.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br> E-mail : " + company.email_id;
                if (company.website.ToString() != string.Empty)
                    CompanyAddress = CompanyAddress + "<br> Website : " + company.website;


                StringBuilder sbStart = new StringBuilder();
                sbStart.AppendLine("<html>");
                sbStart.AppendLine("<head>");
                sbStart.AppendLine(SIGlobals.Globals.GetStyleOrder());
                sbStart.AppendLine("</head>");
                sbStart.AppendLine("<body>");

                NumberToWords objNWClass = new NumberToWords();
                string DateTime = string.Format("{0:dd/MM/yyyy}", (issueMaster.issue_date));
                sbStart.AppendLine("<Table frame=\"border\" border=\"0\" width=\"800\">");
                for (int j = 0; j < 8; j++) {
                    sbStart.AppendLine("<TR style=border:0>");
                    sbStart.AppendLine(string.Format("<TD style=border:0 colspan = 8 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                sbStart.AppendLine("</Table>");


                sbStart.AppendLine("<Table font-size=14pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"800\">");
                string Name = customer.cust_name;
                string Address1 = customer.address1;
                string Address2 = customer.address2;
                string Address3 = customer.address3;
                string City = customer.city;
                string state = customer.state;
                string state_code = Convert.ToString(customer.state_code);
                string Mobile = customer.mobile_no;
                string PAN = customer.pan_no;
                string GSTIN = customer.tin;
                string Address = string.Empty;

                if (Address1 != string.Empty)
                    Address = Address + "<br>" + Address1;
                if (Address2 != string.Empty)
                    Address = Address + "<br>" + Address2;

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan =2 ALIGN = \"left\"><b>GSTIN:{0}</b></TD>", company.tin_no));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.company_name));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"CENTER\"><b>{0},{1},{2},{3}-{4}</b></TD>", company.address1, company.address2, company.address3, company.city, company.pin_code));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; \"  colspan=2 ALIGN = \"CENTER\"><b>{0}</b></TD>", "GST INVOICE"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2  ALIGN = \"CENTER\"><b>{0}:{1}</b></TD>", "Supply of service", "Jewellery Repair Service"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b> DETAILS OF CUSTOMER </b></TD>"));
                sbStart.AppendLine(string.Format("<TD width=\"400\"  ALIGN = \"LEFT\"><b>{0}{1}</b></TD>", "Receipt No :", issueNo));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<tr>");
                sbStart.AppendLine("<td>");

                sbStart.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + Name + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address1 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address2 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address3 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" tyle=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + City + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + state + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + state_code + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Mobile + "</b></td>");
                sbStart.AppendLine("</tr>");
                if (!string.IsNullOrEmpty(PAN)) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + PAN + "</b></td>");
                    sbStart.AppendLine("</tr>");
                }
                else {
                    if (proof != null) {
                        if (!string.IsNullOrEmpty(proof.Doc_code)) {
                            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                            sbStart.AppendLine(string.Format("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>{0} &nbsp&nbsp</b></td>", proof.Doc_code));
                            sbStart.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", proof.Doc_No));
                            sbStart.AppendLine("</tr>");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(GSTIN)) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>GSTIN &nbsp&nbsp</b></td>");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + GSTIN + "</b></td>");
                    sbStart.AppendLine("</tr>");
                }
                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");

                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sbStart.AppendLine("<tr style=\"border-right:0\">");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"left\" ><b>Date &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + DateTime + "</b></td>");
                sbStart.AppendLine("</tr>");


                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" ><b> Place of supply &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + company.state + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + company.state + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin  ;border-top:thin\" ><b>" + Convert.ToString(company.state_code) + "</b></td>");
                sbStart.AppendLine("</tr>");


                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");

                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("</Table>");
                string strWords = string.Empty;

                sbStart.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; border-top:thin\" width=\"800\">");  //FRAME=BOX RULES=NONE



                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD  style=\"border-right:thin; border-top:thin \" colspan=16 ALIGN = \"CENTER\"><b>ORIGINAL / DUPLICATE <br></b></TD>"));
                sbStart.AppendLine("</TR>");

                //string strReceiptDetails = string.Format("select description,units,gwt,swt,nwt,dcts,repair_amount,isnull(SGST_Percent,0) as SGST_Percent,isnull(SGST_Amount,0) as SGST_Amount,isnull(CGST_Percent,0) as CGST_Percent,isnull(CGST_Amount,0) as CGST_Amount,isnull(IGST_Percent,0) as IGST_Percent,isnull(IGST_Amount,0) as IGST_Amount,isnull(HSN,'') as HSN from KTTU_REPAIR_ISSUE_DETAILS where issue_no = {0} and company_code='{1}' and branch_code='{2}'", ReceiptNo, CGlobals.CompanyCode, CGlobals.BranchCode);
                //DataTable dtReceiptDetail = CGlobals.GetDataTable(strReceiptDetails);


                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                                    && pay.branch_code == branchCode
                                                                                    && pay.series_no == issueNo
                                                                                    && pay.trans_type == "RO").OrderBy(p => p.sno).ToList();
                var repairAmount = Convert.ToDecimal(payment.Sum(x => x.pay_amt));
                if (repairAmount <= 0)
                    repairAmount = Convert.ToDecimal(issueDet[0].repair_amount);
                objNWClass.ConvertNumberToWords(Convert.ToDouble(Convert.ToString(repairAmount)), out strWords);
                strWords = strWords.Replace("Rupees", "");
                strWords = "Rupees " + strWords;

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>Received with thanks from Mr./Mrs./M/s.{0}{1}{1}</b></TD>", customer.cust_name, "&nbsp"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>The sum of Rs.{0}[{1}]</b></TD></TR>", Convert.ToString(issueDet[0].repair_amount), strWords + "&nbsp"));
                sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>Towards the IssueNo:{0}   Dated:{1}</b></TD></TR>", Convert.ToString(issueMaster.issue_no), "&nbsp" + Convert.ToString(issueMaster.issue_date) + "&nbsp"));
                
                for (int i = 0; i < payment.Count; i++) {
                    string payMode = payment[i].pay_mode;
                    decimal PayAmt = Convert.ToDecimal(payment[i].pay_amt);
                    string branch = payMode.Substring(0, 1);
                    sbStart.AppendLine("<TR>");
                    if (string.Compare(payMode, "C") == 0) {
                        sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>{0}{1}</b></TD>", "Cash Received :" + PayAmt.ToString("N"), "&nbsp"));

                    }
                    else if (string.Compare(payMode, "Q") == 0 || string.Compare(payMode, "D") == 0) {
                        sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>By Cheque/DD :{0}{1}</b></TD>", PayAmt.ToString("N") + "/" + "Cheque/DD No :" + payment[i].cheque_no + "/" + "Dated :" + "/" + "Drawn On:" + payment[i].bank_name.ToString(), "&nbsp"));

                    }
                    sbStart.AppendLine("</TR>");
                }

                sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>Balance Amount :{0}{1}{1}</b></TD></TR>", "NIL", "&nbsp"));
                sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"left\"><b>Operator Code :{0} </b></TD>", issueMaster.operator_code, "&nbsp"));

                sbStart.AppendLine("</TR>");

                sbStart.AppendLine(string.Format("<TD colspan=8 style=\"border-right:thin\"  ALIGN = \"LEFT\"><b><br><br><br><br><br>{0}</b></TD>", "Customer Signature"));
                sbStart.AppendLine(string.Format("<TD  colspan=8 ALIGN = \"RIGHT\"><b>For {0}<br><br><br><br><br>{1}</b></TD>", company.company_name, "Authorized Signatory"));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("</Table>");
                sbStart.AppendLine("</body>");
                sbStart.AppendLine("</html>");
                return sbStart.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }
        public ProdigyPrintVM GetRepairIssuetPrint(string companyCode, string branchCode, int repairNo, string printType, int isDirect, out ErrorVM error)
        {
            error = null;
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            PrintConfiguration pc = new PrintConfiguration();
            var printConfig = pc.GetPrintConfigurationForDraftDocuments(companyCode, branchCode, "REP_ISS");
            if (printConfig == "HTML") {
                string htmlPrintData = string.Empty;
                if (isDirect == 1) {
                    htmlPrintData = GetIssueHTMLPrintDirect(companyCode, branchCode, repairNo, printType, out error);
                }
                else {
                    htmlPrintData = GetIssueHTMLPrint(companyCode, branchCode, repairNo, printType, out error);
                }
                printObject.ContinueNextPrint = false;
                printObject.Data = new PrintConfiguration().Base64Encode(htmlPrintData);
                printObject.PrintType = "HTML";
            }
            else {
                var dotMatrixPrintData = GetIssueDotMatrixPrint(companyCode, branchCode, repairNo, out error);
                printObject.ContinueNextPrint = true;
                printObject.Data = dotMatrixPrintData;
                printObject.PrintType = "RAW";
            }
            return printObject;
        }
        #endregion

        #region Private Methods
        private bool GetGSTComponentValues(string gSTGoodsGroupCode, decimal Amount, bool isInterstateAssociate, out decimal sgstPercent, out decimal sgstAmt,
            out decimal cgstPercent, out decimal cgstAmt, out decimal igstPercent, out decimal igstAmt, out decimal cessPercent, out decimal cessAmt, string companyCode, string branchCode)
        {
            sgstPercent = sgstAmt = cgstPercent = cgstAmt = igstPercent = igstAmt = cessPercent = cessAmt = 0;
            var taxDet = db.usp_GSTGetGSTTaxValues(gSTGoodsGroupCode, Amount, isInterstateAssociate, companyCode, branchCode).ToList();

            if (taxDet == null || taxDet.Count() <= 0) {
                return false;
            }

            if (taxDet != null && taxDet.Count() > 0) {

                foreach (usp_GSTGetGSTTaxValues_Result det in taxDet) {
                    switch (det.GSTComponentCode) {
                        case "CGST":
                            cgstAmt = det.GSTAmount;
                            cgstPercent = det.GSTPercent;
                            break;
                        case "SGST":
                            sgstAmt = det.GSTAmount;
                            sgstPercent = det.GSTPercent;
                            break;
                        case "IGST":
                            igstAmt = det.GSTAmount;
                            igstPercent = det.GSTPercent;
                            break;
                        case "CESS":
                            cessAmt = det.GSTAmount;
                            cessPercent = det.GSTPercent;
                            break;
                    }
                }
            }
            return true;
        }

        private void ReceiptStockUpdate(string gsCode, string itemName, int units, decimal gwt, decimal swt, decimal nwt, string companyCode, string branchCode)
        {
            string itemCode = string.Empty;
            string counterCode = string.Empty;

            var data = (from item in db.ITEM_MASTER
                        join krrd in db.KTTU_REPAIR_RECEIPT_DETAILS
                        on new { Item = item.Item_Name, GSCode = item.gs_code, CompanyCode = item.company_code, BranchCode = item.branch_code }
                        equals new { Item = krrd.item, GSCode = krrd.gs_code, CompanyCode = krrd.company_code, BranchCode = krrd.branch_code }
                        where krrd.gs_code == gsCode && krrd.item == itemName && krrd.company_code == companyCode && krrd.branch_code == branchCode
                        select new
                        {
                            ItemCode = item.Item_code,
                            Counter = item.counter_code
                        }).FirstOrDefault();

            if (data != null) {
                itemCode = data.ItemCode;
                counterCode = data.Counter;

                KTTU_GS_SALES_STOCK gsStock = db.KTTU_GS_SALES_STOCK.Where(kgst => kgst.gs_code == gsCode && kgst.company_code == companyCode && kgst.branch_code == branchCode).FirstOrDefault();
                if (gsStock != null) {
                    //gsStock.receipt_units = gsStock.issue_units + units;
                    //gsStock.receipt_gwt = gsStock.issue_gwt + gwt;
                    //gsStock.receipt_nwt = gsStock.issue_nwt + nwt;
                    gsStock.receipt_units = gsStock.receipt_units + units;
                    gsStock.receipt_gwt = gsStock.receipt_gwt + gwt;
                    gsStock.receipt_nwt = gsStock.receipt_nwt + nwt;
                    gsStock.closing_units = gsStock.closing_units + units;
                    gsStock.closing_gwt = gsStock.closing_gwt + gwt;
                    gsStock.closing_nwt = gsStock.closing_nwt + nwt;
                    db.Entry(gsStock).State = System.Data.Entity.EntityState.Modified;
                }

                KTTU_COUNTER_STOCK kcsd = db.KTTU_COUNTER_STOCK.Where(kcs => kcs.gs_code == gsCode && kcs.item_name == itemCode && kcs.counter_code == counterCode && kcs.company_code == companyCode && kcs.branch_code == branchCode).FirstOrDefault();
                if (kcsd != null) {
                    kcsd.receipt_units += units;
                    kcsd.receipt_gwt += gwt;
                    kcsd.receipt_swt += swt;
                    kcsd.receipt_nwt += nwt;
                    kcsd.closing_units += units;
                    kcsd.closing_gwt += gwt;
                    kcsd.closing_swt += swt;
                    kcsd.closing_nwt += nwt;
                }
                else {
                    KTTU_COUNTER_STOCK counterStock = new KTTU_COUNTER_STOCK();
                    counterStock.obj_id = SIGlobals.Globals.GetNewGUID();
                    counterStock.gs_code = gsCode;
                    counterStock.item_name = itemName;
                    counterStock.counter_code = counterCode;
                    counterStock.date = SIGlobals.Globals.GetDateTime();
                    counterStock.company_code = companyCode;
                    counterStock.branch_code = branchCode;
                    counterStock.op_units = 0;
                    counterStock.op_gwt = 0.000M;
                    counterStock.op_nwt = 0.000M;
                    counterStock.op_swt = 0.000M;
                    counterStock.barcoded_units = 0;
                    counterStock.barcoded_gwt = 0.000M;
                    counterStock.barcoded_swt = 0.000M;
                    counterStock.barcoded_nwt = 0.000M;
                    counterStock.sales_units = 0;
                    counterStock.sales_gwt = 0.000M;
                    counterStock.sales_swt = 0.000M;
                    counterStock.sales_nwt = 0.000M;
                    counterStock.receipt_units = units;
                    counterStock.receipt_gwt = gwt;
                    counterStock.receipt_swt = swt;
                    counterStock.receipt_nwt = nwt;
                    counterStock.issues_units = 0;
                    counterStock.issues_gwt = 0.000M;
                    counterStock.issues_swt = 0.000M;
                    counterStock.issues_nwt = 0.000M;
                    counterStock.closing_units = units;
                    counterStock.closing_gwt = gwt;
                    counterStock.closing_swt = swt;
                    counterStock.closing_nwt = nwt;
                    db.KTTU_COUNTER_STOCK.Add(counterStock);
                }
            }
        }

        private void CancelReceiptStockUpdate(string gsCode, string itemName, int units, decimal gwt, decimal swt, decimal nwt, string companyCode, string branchCode)
        {
            string itemCode = string.Empty;
            string counterCode = string.Empty;

            var data = (from item in db.ITEM_MASTER
                        join krrd in db.KTTU_REPAIR_RECEIPT_DETAILS
                        on new { Item = item.Item_Name, GSCode = item.gs_code, CompanyCode = item.company_code, BranchCode = item.branch_code }
                        equals new { Item = krrd.item, GSCode = krrd.gs_code, CompanyCode = krrd.company_code, BranchCode = krrd.branch_code }
                        where krrd.gs_code == gsCode && krrd.item == itemName && krrd.company_code == companyCode && krrd.branch_code == branchCode
                        select new
                        {
                            ItemCode = item.Item_code,
                            Counter = item.counter_code
                        }).FirstOrDefault();

            if (data != null) {
                itemCode = data.ItemCode;
                counterCode = data.Counter;

                KTTU_GS_SALES_STOCK gsStock = db.KTTU_GS_SALES_STOCK.Where(kgst => kgst.gs_code == gsCode && kgst.company_code == companyCode && kgst.branch_code == branchCode).FirstOrDefault();
                if (gsStock != null) {
                    gsStock.receipt_units = gsStock.receipt_units - units;
                    gsStock.receipt_gwt = gsStock.receipt_gwt - gwt;
                    gsStock.receipt_nwt = gsStock.receipt_nwt - nwt;
                    gsStock.closing_units = gsStock.closing_units - units;
                    gsStock.closing_gwt = gsStock.closing_gwt - gwt;
                    gsStock.closing_nwt = gsStock.closing_nwt - nwt;
                    db.Entry(gsStock).State = System.Data.Entity.EntityState.Modified;
                }

                KTTU_COUNTER_STOCK kcsd = db.KTTU_COUNTER_STOCK.Where(kcs => kcs.gs_code == gsCode && kcs.item_name == itemCode && kcs.counter_code == counterCode && kcs.company_code == companyCode && kcs.branch_code == branchCode).FirstOrDefault();
                if (kcsd != null) {
                    kcsd.receipt_units -= units;
                    kcsd.receipt_gwt -= gwt;
                    kcsd.receipt_swt -= swt;
                    kcsd.receipt_nwt -= nwt;
                    kcsd.closing_units -= units;
                    kcsd.closing_gwt -= gwt;
                    kcsd.closing_swt -= swt;
                    kcsd.closing_nwt -= nwt;
                }
                else {
                    KTTU_COUNTER_STOCK counterStock = new KTTU_COUNTER_STOCK();
                    counterStock.obj_id = SIGlobals.Globals.GetNewGUID();
                    counterStock.gs_code = gsCode;
                    counterStock.item_name = itemName;
                    counterStock.counter_code = counterCode;
                    counterStock.date = SIGlobals.Globals.GetDateTime();
                    counterStock.company_code = companyCode;
                    counterStock.branch_code = branchCode;
                    counterStock.op_units = 0;
                    counterStock.op_gwt = 0.000M;
                    counterStock.op_nwt = 0.000M;
                    counterStock.op_swt = 0.000M;
                    counterStock.barcoded_units = 0;
                    counterStock.barcoded_gwt = 0.000M;
                    counterStock.barcoded_swt = 0.000M;
                    counterStock.barcoded_nwt = 0.000M;
                    counterStock.sales_units = 0;
                    counterStock.sales_gwt = 0.000M;
                    counterStock.sales_swt = 0.000M;
                    counterStock.sales_nwt = 0.000M;
                    counterStock.receipt_units = units;
                    counterStock.receipt_gwt = gwt;
                    counterStock.receipt_swt = swt;
                    counterStock.receipt_nwt = nwt;
                    counterStock.issues_units = 0;
                    counterStock.issues_gwt = 0.000M;
                    counterStock.issues_swt = 0.000M;
                    counterStock.issues_nwt = 0.000M;
                    counterStock.closing_units = units;
                    counterStock.closing_gwt = gwt;
                    counterStock.closing_swt = swt;
                    counterStock.closing_nwt = nwt;
                    db.KTTU_COUNTER_STOCK.Add(counterStock);
                }
            }
        }

        private void IssueStockUpdate(string gsCode, string itemName, int units, decimal gwt, decimal swt, decimal nwt, string companyCode, string branchCode)
        {
            string itemCode = string.Empty;
            string counterCode = string.Empty;

            var data = (from im in db.ITEM_MASTER
                        join rd in db.KTTU_REPAIR_RECEIPT_DETAILS
                        on new { Item = im.Item_Name, GS = im.gs_code, Company = im.company_code, Branch = im.branch_code }
                        equals new { Item = rd.item, GS = rd.gs_code, Company = rd.company_code, Branch = rd.branch_code }
                        where rd.gs_code == gsCode && rd.item == itemName && rd.company_code == companyCode && rd.branch_code == branchCode
                        select new
                        {
                            ItemCode = im.Item_code,
                            Counter = im.counter_code
                        }).FirstOrDefault();

            if (data != null) {
                itemCode = data.ItemCode;
                counterCode = data.Counter;

                KTTU_GS_SALES_STOCK gsStock = db.KTTU_GS_SALES_STOCK.Where(kgst => kgst.gs_code == gsCode && kgst.company_code == companyCode && kgst.branch_code == branchCode).FirstOrDefault();
                if (gsStock != null) {
                    gsStock.issue_units = gsStock.issue_units + units;
                    gsStock.issue_gwt = gsStock.issue_gwt + gwt;
                    gsStock.issue_nwt = gsStock.issue_nwt + nwt;
                    gsStock.closing_units = gsStock.closing_units - units;
                    gsStock.closing_gwt = gsStock.closing_gwt - gwt;
                    gsStock.closing_nwt = gsStock.closing_nwt - nwt;
                    db.Entry(gsStock).State = System.Data.Entity.EntityState.Modified;
                }

                KTTU_COUNTER_STOCK kcsd = db.KTTU_COUNTER_STOCK.Where(kcs => kcs.gs_code == gsCode && kcs.item_name == itemCode && kcs.counter_code == counterCode && kcs.company_code == companyCode && kcs.branch_code == branchCode).FirstOrDefault();
                if (kcsd != null) {
                    kcsd.issues_units += units;
                    kcsd.issues_gwt += gwt;
                    kcsd.issues_swt += swt;

                    kcsd.issues_nwt += nwt;
                    kcsd.closing_units -= units;
                    kcsd.closing_gwt -= gwt;
                    kcsd.closing_swt -= swt;
                    kcsd.closing_nwt -= nwt;
                    db.Entry(kcsd).State = System.Data.Entity.EntityState.Modified;
                }
            }
        }

        private void CancelIssueStockUpdate(string gsCode, string itemName, int units, decimal gwt, decimal swt, decimal nwt, string companyCode, string branchCode)
        {
            string itemCode = string.Empty;
            string counterCode = string.Empty;

            var data = (from im in db.ITEM_MASTER
                        join rd in db.KTTU_REPAIR_RECEIPT_DETAILS
                        on new { Item = im.Item_Name, GS = im.gs_code, Company = im.company_code, Branch = im.branch_code }
                        equals new { Item = rd.item, GS = rd.gs_code, Company = rd.company_code, Branch = rd.branch_code }
                        where rd.gs_code == gsCode && rd.item == itemName && rd.company_code == companyCode && rd.branch_code == branchCode
                        select new
                        {
                            ItemCode = im.Item_code,
                            Counter = im.counter_code
                        }).FirstOrDefault();

            if (data != null) {
                itemCode = data.ItemCode;
                counterCode = data.Counter;

                KTTU_GS_SALES_STOCK gsStock = db.KTTU_GS_SALES_STOCK.Where(kgst => kgst.gs_code == gsCode && kgst.company_code == companyCode && kgst.branch_code == branchCode).FirstOrDefault();
                if (gsStock != null) {
                    gsStock.issue_units = gsStock.issue_units - units;
                    gsStock.issue_gwt = gsStock.issue_gwt - gwt;
                    gsStock.issue_nwt = gsStock.issue_nwt - nwt;
                    gsStock.closing_units = gsStock.closing_units + units;
                    gsStock.closing_gwt = gsStock.closing_gwt + gwt;
                    gsStock.closing_nwt = gsStock.closing_nwt + nwt;
                    db.Entry(gsStock).State = System.Data.Entity.EntityState.Modified;
                }

                KTTU_COUNTER_STOCK kcsd = db.KTTU_COUNTER_STOCK.Where(kcs => kcs.gs_code == gsCode && kcs.item_name == itemCode && kcs.counter_code == counterCode && kcs.company_code == companyCode && kcs.branch_code == branchCode).FirstOrDefault();
                if (kcsd != null) {
                    kcsd.issues_units -= units;
                    kcsd.issues_gwt -= gwt;
                    kcsd.issues_swt -= swt;

                    kcsd.issues_nwt += nwt;
                    kcsd.closing_units += units;
                    kcsd.closing_gwt += gwt;
                    kcsd.closing_swt += swt;
                    kcsd.closing_nwt += nwt;
                    db.Entry(kcsd).State = System.Data.Entity.EntityState.Modified;
                }
            }
        }

        private ErrorVM IssueAccountPostingWithProedure(int issueNo, string companyCode, string branchCode)
        {
            //Important Note: There is a issue with Accounts update procedure after Upgrade to Multiple DB, Need to fix by Uday
            //try {
            //    ObjectParameter errorMessage = new ObjectParameter("errorMsg", typeof(string));
            //    ObjectParameter outVal = new ObjectParameter("outValue", typeof(int));
            //    var data = db.usp_createRepairPostingVouchers(companyCode, branchCode, Convert.ToString(issueNo),outVal, errorMessage);
            //    string message = Convert.ToString(errorMessage.Value);
            //    if (data.Count() <= 0 || message != "") {
            //        return new ErrorVM() { description = "Error Occurred while Updating Accounts. " + errorMessage, field = "Account Update", index = 0 };
            //    }
            //}
            //catch (Exception excp) {
            //    return new ErrorVM() { description = excp.InnerException.Message, field = "Account Update", index = 0 };
            //}
            //return null;

            try {
                string errorFromProc = string.Empty;
                ObjectParameter errorMessage = new ObjectParameter("errorMsg", typeof(string));
                ObjectParameter outputVal = new ObjectParameter("outValue", typeof(int));
                var result = db.usp_createRepairPostingVouchers_FuncImport(companyCode, branchCode, Convert.ToString(issueNo), outputVal, errorMessage);
                return new CommonBL().HandleAccountPostingProcs(outputVal, errorMessage);
            }
            catch (Exception excp) {
                return new ErrorVM() { description = excp.InnerException.Message, field = "Account Update", index = 0 };
            }
        }

        #endregion
    }
}

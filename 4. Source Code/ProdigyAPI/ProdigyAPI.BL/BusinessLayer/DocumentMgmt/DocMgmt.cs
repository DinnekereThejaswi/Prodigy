using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.DocumentMgmt
{
    public class DocMgmt
    {
        private KSTS_SEQ_NOS seqNos = null;
        MagnaDbEntities dbContext = null;
        string docSeqNoId = string.Empty;
        public Tuple<string, string> GetLastDocumentNo(string companyCode, string branchCode, string docType)
        {
            Tuple<string, string> doc = null;
            string objID = "0";
            int transType = 1; //1 for PoS transactions, 2 for Account transactions
            string finYearPrefix = string.Empty;
            using (MagnaDbEntities db = new MagnaDbEntities()) {
                var finYear = db.KSTU_ACC_FY_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode).FirstOrDefault();
                if(finYear != null) {
                    finYearPrefix = (finYear.fin_year % 1000).ToString();
                }
                switch (docType) {
                    #region PoS Transactions
                    case "SALEST":
                        objID = "3"; finYearPrefix = string.Empty; break;
                    case "PUREST":
                        objID = "3"; finYearPrefix = string.Empty; break;
                    case "SREST":
                        objID = "3"; finYearPrefix = string.Empty;  break;
                    case "ORDNO":
                        objID = "2"; break;
                    case "SALINV":
                        objID = "4"; break;
                    case "REPISS":
                        objID = "7"; break;
                    case "REPREC":
                        objID = "8"; break;
                    case "PURINV":
                        objID = "9"; break;
                    case "ORDREC":
                        objID = "11"; break;
                    case "CUSTID":
                        objID = "12"; finYearPrefix = string.Empty; break;
                    case "SRINV":
                        objID = "18"; break;
                    case "CREINV":
                        objID = "24"; break;
                    case "STKBAT":
                        objID = "38"; break;
                    case "NTGISS":
                    case "TAGISS":
                    case "OPGISS":
                    case "SRISS":
                        objID = "28"; break;
                    case "BARREC":
                    case "NTGREC":
                    case "AUTREC":
                        objID = "29"; break;
                    case "MLTISS":
                    case "CPCISS":
                        objID = "14"; break;
                    case "OPGSEG":
                        objID = "103"; break;
                    case "MLTREC":
                        objID = "15"; break;
                    #endregion
                    #region Account Txns
                    case "ACCGRP":
                        objID = "01"; transType = 2; break;
                    case "ACCACD":
                        objID = "02"; transType = 2; break;
                    case "ACCTRS":
                        objID = "03"; transType = 2; break;
                    case "ACCVCH":
                        objID = "04"; transType = 2; break;
                    case "ACCJNL":
                        objID = "05"; transType = 2; break;
                    case "ACCCON":
                        objID = "06"; transType = 2; break;
                    case "ACCOTH":
                        objID = "10"; transType = 2; break;
                    case "ACCCHT":
                        objID = "16"; transType = 2; break;
                    case "ACCORC":
                        objID = "20"; transType = 2; break;
                    case "ACCPOS":
                        objID = "27"; transType = 2; break;
                        #endregion
                }

                if (transType == 1) {
                    var docSeries = db.KSTS_SEQ_NOS.Where(x => x.obj_id == objID && x.company_code == companyCode
                        && x.branch_code == branchCode).FirstOrDefault();
                    if (docSeries != null)
                        doc = new Tuple<string, string>(docType, finYearPrefix + (docSeries.nextno - 1).ToString());
                }
                else if (transType == 2) {
                    var docSeries = db.KSTS_ACC_SEQ_NOS.Where(x => x.obj_id == objID && x.company_code == companyCode
                       && x.branch_code == branchCode).FirstOrDefault();
                    if (docSeries != null)
                        doc = new Tuple<string, string>(docType, (docSeries.nextno - 1).ToString());
                }
            }
            return doc;
        }

        public int GetDocumentNo(MagnaDbEntities db, string companyCode, string branchCode, string docSeqNo, bool prefixFinYear = true)
        {
            int documentNo = 0;
            dbContext = db;
            docSeqNoId = docSeqNo;
            seqNos = dbContext.KSTS_SEQ_NOS.Where(sq => sq.obj_id == docSeqNoId
                                     && sq.company_code == companyCode
                                     && sq.branch_code == branchCode).FirstOrDefault();
            if (seqNos == null) {
                throw new Exception("Document series " + docSeqNoId + " does not exist.");
            }
            int nextNo = seqNos.nextno;

            if (prefixFinYear == true) {
                string finYear = GetFinancialYear(dbContext, companyCode, branchCode).ToString().Remove(0, 1);
                documentNo = Convert.ToInt32(finYear + nextNo.ToString());
            }
            else {
                documentNo = nextNo;
            }
            return documentNo;
        }

        public void IncrementDocumentNo()
        {
            if (seqNos == null || string.IsNullOrEmpty(docSeqNoId)) {
                throw new Exception("Document series " + docSeqNoId + " does not exist.");
            }
            seqNos.nextno = seqNos.nextno + 1;
            dbContext.Entry(seqNos).State = System.Data.Entity.EntityState.Modified;
        }

        public int GetFinancialYear(MagnaDbEntities db, string companyCode, string branchCode)
        {
            var fyMaster = db.KSTU_ACC_FY_MASTER.Where(f => f.company_code == companyCode 
                && f.branch_code == branchCode).FirstOrDefault();
            int finYear = 2021;
            if (fyMaster != null)
                finYear = fyMaster.fin_year;
            else {
                DateTime applicationDate = GetApplicationDate(db, companyCode, branchCode);
                //Jan to March, it will be Calendar year - 1, else FY = Calendar Year.
                if (applicationDate.Month < 4) {
                    finYear = applicationDate.Year - 1;
                }
                else
                    finYear = applicationDate.Year;
            }
            return finYear;
        }

        public DateTime GetApplicationDate(MagnaDbEntities db, string companyCode, string branchCode)
        {
            DateTime dt = DateTime.ParseExact(db.KSTU_RATE_MASTER.Where(r => r.company_code == companyCode 
                && r.branch_code == branchCode).ToList()[0].ondate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            if (dt == null)
                throw new Exception("Unable to retreive application date.");
            DateTime resultDate = dt.Add(DateTime.Now.TimeOfDay);
            return resultDate;
        }
        
    }
}

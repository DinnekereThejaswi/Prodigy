using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace ProdigyAPI.Framework
{
    public static class Common
    {
        public static string CompanyCode = "";
        public static string BranchCode = "";

        static Common()
        {
            CompanyCode = ConfigurationManager.AppSettings["CompanyCode"].ToString();
            BranchCode = ConfigurationManager.AppSettings["BranchCode"].ToString();
        }

        /// <summary>
        /// Method is used to Get Date Time from the Server
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDateTime()
        {
            //DataTable tblDate = Common.ExecuteQuery("SELECT [dbo].[GetDateTime] ()");
            //return (Convert.ToDateTime(tblDate.Rows[0][0]));
            Model.MagnaDb.MagnaDbEntities db = new Model.MagnaDb.MagnaDbEntities();
            DateTime dt = DateTime.ParseExact(db.KSTU_RATE_MASTER.ToList()[0].ondate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime resultDate = dt.Add(DateTime.Now.TimeOfDay);
            return resultDate;
        }

        /// <summary>
        /// Method returns the Actual date and Time
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCountryDateTime()
        {
            DataTable tblDate = Common.ExecuteQuery("SELECT [mw].[ufn_GetDate]()");
            return (Convert.ToDateTime(tblDate.Rows[0][0]));
        }
        /// <summary>
        /// Method is used to Execute Query 
        /// </summary>
        /// <param name="Query"></param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteQuery(string Query)
        {
            var table = new DataTable();
            using (var ctx = new MagnaDbEntities()) {
                var cmd = ctx.Database.Connection.CreateCommand();
                cmd.CommandText = Query;

                cmd.Connection.Open();
                table.Load(cmd.ExecuteReader());
                cmd.Connection.Close();
            }
            return table;
        }

        public static void UpdateSeqenceNumber(string objID)
        {
            Model.MagnaDb.MagnaDbEntities db = new Model.MagnaDb.MagnaDbEntities();
            KSTS_SEQ_NOS sequeceNo = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == objID).FirstOrDefault();
            sequeceNo.nextno = sequeceNo.nextno + 1;
            db.Entry(sequeceNo).State = System.Data.Entity.EntityState.Modified;
            try {
                db.SaveChanges();
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public static void UpdateSeqenceNumber(Model.MagnaDb.MagnaDbEntities db, string companyCode, string branchCode, string objID)
        {
            try {
                KSTS_SEQ_NOS sequeceNo = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == objID && sq.company_code == companyCode && sq.branch_code == branchCode).FirstOrDefault();
                sequeceNo.nextno = sequeceNo.nextno + 1;
                db.Entry(sequeceNo).State = System.Data.Entity.EntityState.Modified;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public static void UpdateAccountSeqenceNumber(Model.MagnaDb.MagnaDbEntities db, string ObjID)
        {
            try {
                KSTS_ACC_SEQ_NOS sequeceNo = db.KSTS_ACC_SEQ_NOS.Where(sq => sq.obj_id == ObjID).FirstOrDefault();
                sequeceNo.nextno = sequeceNo.nextno + 1;
                db.Entry(sequeceNo).State = System.Data.Entity.EntityState.Modified;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public static void UpdateAccountVourcherSeqenceNumber(Model.MagnaDb.MagnaDbEntities db, string ObjID)
        {
            try {
                KSTS_ACC_VOUCHER_SEQ_NOS sequeceNo = db.KSTS_ACC_VOUCHER_SEQ_NOS.Where(sq => sq.obj_id == ObjID).FirstOrDefault();
                sequeceNo.nextno = sequeceNo.nextno + 1;
                db.Entry(sequeceNo).State = System.Data.Entity.EntityState.Modified;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public static string GetNewGUID()
        {
            return Guid.NewGuid().ToString().Replace("-", "").ToUpper();
        }

        public static string PaymentMode(string payMode)
        {
            string retValue = string.Empty;
            switch (payMode) {
                case "C": retValue = "Cash"; break;
                case "Q": retValue = "Cheque"; break;
                case "D": retValue = "Demand Draft"; break;
                case "R": retValue = "Card"; break;
                case "CT": retValue = "Scheme.Adj"; break;
                default: retValue = ""; break;
            }
            return retValue;
        }

        public static string OrderType(string orderType)
        {
            string retValue = string.Empty;
            switch (orderType) {
                case "R": retValue = "Reserved Order"; break;
                case "C": retValue = "Customized Order"; break;
                case "O": retValue = "Order Advance"; break;
                default: retValue = ""; break;
            }
            return retValue;
        }

        public static ClaimDetail GetClaims(ClaimsPrincipal principal)
        {
            var claimInfo = principal.Claims.Where(c => c.Type == "UserID").FirstOrDefault();
            if (claimInfo == null) {
                //No claim info found.
                return null;
            }
            ClaimDetail claim = new ClaimDetail();
            claim.UserID = principal.Claims.Where(c => c.Type == "UserID").FirstOrDefault().Value;
            claim.RoleID = principal.Claims.Where(c => c.Type == "RoleID").FirstOrDefault().Value;
            claim.PwdStamp = principal.Claims.Where(c => c.Type == "PwdStamp").FirstOrDefault().Value;
            claim.RowTimestamp = principal.Claims.Where(c => c.Type == "RowTimestamp").FirstOrDefault().Value;
            claim.CompanyCode = principal.Claims.Where(c => c.Type == "CompanyCode").FirstOrDefault().Value;
            claim.BranchCode = principal.Claims.Where(c => c.Type == "BranchCode").FirstOrDefault().Value;
            Common.BranchCode = claim.BranchCode;
            Common.CompanyCode = claim.CompanyCode;
            return claim;
        }
    }
}
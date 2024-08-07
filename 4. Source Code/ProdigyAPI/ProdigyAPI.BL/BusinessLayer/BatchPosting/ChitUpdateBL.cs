using System;
using System.Collections.Generic;
using System.Linq;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System.Data;
using ProdigyAPI.BL.ViewModel.BatchPosting;

namespace ProdigyAPI.BL.BusinessLayer.BatchPosting
{
    public class ChitUpdateBL
    {
        MagnaDbEntities db = null;

        public ChitUpdateBL()
        {
            db = new MagnaDbEntities(true);
        }

        public ChitUpdateBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public bool DownloadSchemes(string companyCode, string branchCode, DateTime txnDate, string userID, out int recordsDownloaded, out ErrorVM error)
        {
            error = null;
            recordsDownloaded = 0;           

            string billModeCondition = string.Empty;

            try {
                string linkedServer = string.Empty;
                string linkedServerDb = string.Empty;
                GetSchemeLinkedServerDetails(companyCode, branchCode, out linkedServer, out linkedServerDb);
                if(string.IsNullOrEmpty(linkedServer)) {
                    error = new ErrorVM { description = "Linked server detail not found." };
                    return false;
                }
                if (string.IsNullOrEmpty(linkedServer)) {
                    error = new ErrorVM { description = "Linked server database name not found." };
                    return false;
                }

                int onlyClosedViaBillMode = SIGlobals.Globals.GetConfigurationValue(db, "2018", companyCode, branchCode);
                if (onlyClosedViaBillMode == 0)
                    billModeCondition = " CLOSING_MODE=''B'' AND ";

                #region Do you want the count? No.
                //string sql = string.Format("SELECT COUNT(*) AS Cnt FROM OPENQUERY({2},'SELECT [obj_id],[Scheme_Code],[Group_Code],[Chit_MembShipNo],[Amt_Received],[Bonus_Amt],[Interest_Percentage], \n"
                //   + "[Interest_Amt],[Gift_Amount],[Deduction_Amt],[Closing_Mode],[Closing_Date],''0'',[Bill_Date],[Chit_Amt],[object_status],0,0,[Won_Amount], \n"
                //   + "updated_on,''{0}'',branch_code,null,null,null,0,[closed_branch],''{1}'',[Bill_Number] FROM {3}.dbo.CHTU_CHIT_CLOSURE   \n"
                //   + "where closed_branch=''{1}'' and {5} closing_date between ''{4} 00:00:00'' and ''{4} 23:59:59''')   \n"
                //   + "where obj_id not in(SELECT c.obj_id from CHTU_CHIT_CLOSURE c WHERE c.closed_At='{1}')",
                //   companyCode, branchCode, linkedServer, linkedServerDb, txnDate.Date.ToString("yyyy-MM-dd"), billModeCondition);

                //DataTable dt = SIGlobals.Globals.GetDataTable(sql);
                //if(dt == null || dt.Rows.Count <= 0) {
                //    error = new ErrorVM { description = "Unable to fetch data from the scheme server and hence cannot proceed further." };
                //    return false;
                //}
                //if(Convert.ToInt32(dt.Rows[0]["Cnt"]) <= 0) {
                //    error = new ErrorVM { description = "Unable to get the record count and therefore cannot proceed further." };
                //    return false;

                //} 
                #endregion
                using (var transaction = db.Database.BeginTransaction()) {
                    #region Delete existing records in Chit table of Magna
                    string deleteExisting =
                                    "DELETE  FROM   CHTU_CHIT_CLOSURE \n"
                                   + "WHERE  bill_number IN (@p3, @p4) \n"
                                   + "       AND closed_at = @p0 \n"
                                   + "       AND closing_date BETWEEN @p1 AND @p2";
                    List<object> deleteParams = new List<object>();
                    deleteParams.Add(branchCode);
                    deleteParams.Add(txnDate.Date);
                    deleteParams.Add(txnDate.Date.AddMilliseconds(998).AddSeconds(59).AddMinutes(59).AddHours(23));
                    deleteParams.Add("");
                    deleteParams.Add("0");
                    int recordsdeleted = SIGlobals.Globals.ExecuteSQL(deleteExisting, db, deleteParams.ToArray()); 
                    #endregion

                    #region Downloading of records
                    string insertSql = string.Format("INSERT INTO CHTU_CHIT_CLOSURE  \n"
                                  + "	SELECT * FROM OPENQUERY({2},'SELECT [obj_id],[Scheme_Code],[Group_Code],[Chit_MembShipNo],[Amt_Received],[Bonus_Amt],[Interest_Percentage], \n"
                                  + "[Interest_Amt],[Gift_Amount],[Deduction_Amt],[Closing_Mode],[Closing_Date],''0'',[Bill_Date],[Chit_Amt],[object_status],Weight_Accumulated,0, \n"
                                  //+ "[Won_Amount],updated_on,''{0}'',branch_code,null,null,null,0,[closed_branch],''{1}'',[Bill_Number],0,0,[uniq_id],0,DiscountAmount,AverageRate   \n"
                                  + "[Won_Amount],updated_on,''{0}'',branch_code,null,null,null,0,[closed_branch],''{1}'',[Bill_Number],0,0,[uniq_id],0,DiscountAmount   \n"
                                  + "FROM {3}.dbo.CHTU_CHIT_CLOSURE  where closed_branch=''{1}'' and {5} closing_date=''{4}''')   \n"
                                  + "WHERE obj_id NOT IN(SELECT c.obj_id FROM CHTU_CHIT_CLOSURE c WHERE c.closed_At=@p0)",
                                  companyCode, branchCode, linkedServer, linkedServerDb, txnDate.Date.ToString("yyyy-MM-dd"), billModeCondition);

                    List<object> paramListA = new List<object>();
                    paramListA.Add(branchCode);
                    recordsDownloaded = SIGlobals.Globals.ExecuteSQL(insertSql, db, paramListA.ToArray()); 
                    #endregion

                    #region Dormant records updation
                    int schemeDormantFlagUpdation = SIGlobals.Globals.GetConfigurationValue(db, "20200920", companyCode, branchCode);
                    if (schemeDormantFlagUpdation > 0) {
                        string dormantsql =
                            string.Format("UPDATE CHTU_CHIT_CLOSURE \n"
                           + "SET    isDormant = ISNULL( \n"
                           + "        ( \n"
                           + "            SELECT TOP(1) isDormant \n"
                           + "            FROM   {0}.{1}.dbo.chtu_scheme_members s \n"
                           + "            WHERE  c.Scheme_Code = s.scheme_code \n"
                           + "                    AND c.Group_Code = s.group_code \n"
                           + "                    AND c.Chit_MembShipNo = s.chit_membshipNo \n"
                           + "                    AND closed_at = @p0 \n"
                           + "        ), \n"
                           + "        0 \n"
                           + "    ) \n"
                           + "FROM   chtu_chit_closure c \n"
                           + "WHERE  ( \n"
                           + "        (Bill_Number IN ('0', '') AND Closing_Mode IN ('B', 'O')) \n"
                           + "        OR (Closing_Mode NOT IN ('B', 'O')) \n"
                           + "    ) ", linkedServer, linkedServerDb);
                        List<object> paramListB = new List<object>();
                        paramListB.Add(branchCode);
                        int recordsUpdated = SIGlobals.Globals.ExecuteSQL(dormantsql, db, paramListB.ToArray());                        
                    }
                    #endregion

                    transaction.Commit();//No rollback is needed, since I've inclosed in using, when the db object goes out of scope, the framework rollback the transaction
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool UpdateBillNumbers(string companyCode, string branchCode, DateTime txnDate, string userID, out int recordsAffected, out ErrorVM error)
        {
            error = null;
            recordsAffected = 0;

            try {
                string linkedServer = string.Empty;
                string linkedServerDb = string.Empty;
                GetSchemeLinkedServerDetails(companyCode, branchCode, out linkedServer, out linkedServerDb);
                if (string.IsNullOrEmpty(linkedServer)) {
                    error = new ErrorVM { description = "Linked server detail not found." };
                    return false;
                }
                if (string.IsNullOrEmpty(linkedServer)) {
                    error = new ErrorVM { description = "Linked server database name not found." };
                    return false;
                }

                string sql =
                    string.Format("UPDATE c \n"
                   + "SET    c.bill_number = d.bill_number, \n"
                   + "       c.ClosingType = ( \n"
                   + "           SELECT DISTINCT REPLACE(p.trans_type, 'S', 'B') \n"
                   + "           FROM   KTTU_PAYMENT_DETAILS p \n"
                   + "           WHERE  p.ref_billNo = CAST(d.chit_membshipNo AS VARCHAR) \n"
                   + "                  AND p.scheme_code = d.scheme_code \n"
                   + "                  AND p.group_code = d.group_code \n"
                   + "                  AND cflag != 'Y' \n"
                   + "       ) \n"
                   + "FROM   OPENQUERY( \n"
                   + "           {0}, \n"
                   + "           'SELECT * \n"
                   + "            FROM   {1}.dbo.CHTU_CHIT_CLOSURE \n"
                   + "            WHERE  closing_date BETWEEN ''{3} 00:00:00.000'' AND ''{3} 23:59:59.998'' \n"
                   + "                   AND closed_branch = ''{2}''' \n"
                   + "       ) c \n"
                   + "       JOIN CHTU_CHIT_CLOSURE d \n"
                   + "            ON  c.obj_id = d.obj_id \n"
                   + "WHERE  d.closing_date BETWEEN @p1 AND @p2 \n"
                   + "       AND closed_At = @p0 \n"
                   + "       AND d.group_code = c.group_code \n"
                   + "       AND d.chit_membshipNo = c.Chit_membshipNo ",
                   linkedServer, linkedServerDb, branchCode, txnDate.Date.ToString("yyyy-MM-dd"));
                List<object> paramList = new List<object>();
                paramList.Add(branchCode);
                paramList.Add(txnDate.Date);
                paramList.Add(txnDate.Date.AddMilliseconds(998).AddSeconds(59).AddMinutes(59).AddHours(23));
                recordsAffected = SIGlobals.Globals.ExecuteSQL(sql, db, paramList.ToArray());
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private void GetSchemeLinkedServerDetails(string companyCode, string branchCode, out string serverName, out string dbName)
        {
            serverName = string.Empty;
            dbName = string.Empty;
            var q = db.CHIT_CONNECTION_TABLE.Where(x => x.Company_Code == companyCode && x.Branch_Code == branchCode).FirstOrDefault();
            if(q != null) {
                serverName = q.ChitServer;
                dbName = q.DatabaseName;
            }
        }

        public bool GetPendingDetails(string companyCode, string branchCode, DateTime txnDate, bool pending, out List<SchemeInfoVM> pendingSchemes, out ErrorVM error)
        {
            pendingSchemes = new List<SchemeInfoVM>();
            error = null;
            //Need to include closed branch in this procedure. It is missing at present. After the selection, I'm filtering now.
            try {
                string sql = string.Format("EXEC dbo.usp_GetPendingChitClosureDetails '{0} 00:00:00.000', '{0} 23:59:59.998', {1}",
                        txnDate.Date.ToString("yyyy-MM-dd"), pending == true ? 1 : 0);
                DataTable dt = SIGlobals.Globals.GetDataTable(sql);
                if (dt == null || dt.Rows.Count <= 0) {
                    error = new ErrorVM { description = "No details found." };
                    return false;
                }
                var dataRows = dt.Select(string.Format("[Cls Branch] = '{0}'", branchCode), "[Cls Date]");
                if (dataRows == null) {
                    return true;
                }

                foreach (var dr in dataRows) {
                    var ps = new SchemeInfoVM
                    {
                        BranchCode = dr["Cls Branch"] != DBNull.Value ? dr["Cls Branch"].ToString() : null,
                        SchemeCode = dr["Scheme Code"] !=  DBNull.Value ? dr["Scheme Code"].ToString() : null,
                        GroupCode = dr["Group"] !=  DBNull.Value ? dr["Group"].ToString() : null,
                        AmountReceived = dr["Amt Rcvd"] !=  DBNull.Value ? Convert.ToDecimal(dr["Amt Rcvd"]) : 0,
                        BonusAmount = dr["Bonus Amt"] !=  DBNull.Value ? Convert.ToDecimal(dr["Bonus Amt"]) : 0,
                        ChitAmount = dr["Chit Amt"] !=  DBNull.Value ? Convert.ToDecimal(dr["Chit Amt"]) : 0,
                        WinnerAmount = dr["Winner Amt"] !=  DBNull.Value ? Convert.ToDecimal(dr["Winner Amt"]) : 0,
                        DiscountAmount = dr["Scheme Discount Amt"] !=  DBNull.Value ? Convert.ToDecimal(dr["Scheme Discount Amt"]) : 0,
                        ClosingDate = dr["Cls Date"] !=  DBNull.Value ? Convert.ToDateTime(dr["Cls Date"]) : DateTime.Now.Date,
                        MembershipNo = dr["MSNo"] !=  DBNull.Value ? Convert.ToInt32(dr["MSNo"]) : 0,
                        ClosingMode = dr["Cls Mode"] !=  DBNull.Value ? dr["Cls Mode"].ToString() : null

                    };
                    pendingSchemes.Add(ps);
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }
    }
}

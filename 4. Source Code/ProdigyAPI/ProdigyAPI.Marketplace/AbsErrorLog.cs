using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace
{
    public abstract class AbsErrorLog
    {
        string connString = ConfigurationManager.ConnectionStrings["MagnaDbEntities"].ConnectionString;
        public virtual void LogInfo(string log)
        {
            try {
                string directory = ConfigurationManager.AppSettings["InformationLogFileName"].ToString();
                if (!Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }
                string filePath = directory + "\\InfoLog" + DateTime.Now.ToString("_yyyy_MM_dd") + ".txt";
                if (!File.Exists(filePath)) {
                    File.Create(filePath).Dispose();
                }
                using (StreamWriter file = new StreamWriter(filePath, true)) {
                    file.WriteLine(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " :" + log);
                }
            }
            catch  {
            }
        }
        public virtual void LogException(Exception ex)
        {
            try {
                LogApplicationError(ex);
                string appPath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
                StringBuilder sb = new StringBuilder();
                CreateExceptionString(sb, ex, string.Empty);
                if (!Directory.Exists(appPath))
                    Directory.CreateDirectory(appPath);
                string strPath = GetErrorLogPath("26062020");
                string filePath = strPath + "\\ErrorLog" + DateTime.Now.ToString("_yyyy_MM_dd") + ".txt";
                using (StreamWriter file = new StreamWriter(filePath, true)) {
                    file.WriteLine(sb.ToString());
                }
            }
            catch (Exception e) {
                throw (e);
            }
        }
        public virtual void LogApplicationError(Exception ex)
        {
            string urlAbsolutePath = "";
            try {
                SqlConnection sqlConn = new SqlConnection(connString);
                string ErrorMessage = string.Empty,
                   ErrorSource = string.Empty,
                   HelpLink = string.Empty,
                   StackTrace1 = string.Empty,
                   InnerExpMsg = string.Empty,
                   InnerExpSource = string.Empty,
                   StackTrace2 = string.Empty,
                   InsertedBy = "-";
                if (ex != null) {
                    ErrorMessage = string.IsNullOrEmpty(ex.Message) ? string.Empty : ex.Message;
                    ErrorSource = string.IsNullOrEmpty(ex.Source) ? string.Empty : ex.Source;
                    HelpLink = string.IsNullOrEmpty(ex.HelpLink) ? string.Empty : ex.HelpLink;
                    StackTrace1 = ex.StackTrace;
                    if (ex.InnerException != null) {
                        InnerExpMsg = string.IsNullOrEmpty(ex.InnerException.Message) ? string.Empty : ex.InnerException.Message;
                        InnerExpSource = string.IsNullOrEmpty(ex.InnerException.Source) ? string.Empty : ex.InnerException.Source;
                        StackTrace2 = string.IsNullOrEmpty(ex.InnerException.StackTrace) ? string.Empty : ex.InnerException.StackTrace;
                    }
                }

                InsertedBy = "WS";


                sqlConn.Open();
                string errMsg = "";
                if (ex != null)
                    errMsg = ex.Message;
                using (SqlCommand command = sqlConn.CreateCommand()) {
                    command.CommandText =
                       "INSERT INTO [ApplicationErrorLog] \n"
                       + "  ( \n"
                       + "    [ErrorMessage], \n"
                       + "    [ErrorSource], \n"
                       + "    [HelpLink], \n"
                       + "    [HResult], \n"
                       + "    [StackTrace1], \n"
                       + "    [InnerExpMsg], \n"
                       + "    [InnerExpSource], \n"
                       + "    [StackTrace2], \n"
                       + "    [IPAddress], \n"
                       + "    [AbsolutePathUrl], \n"
                       + "    [OccurredOn], \n"
                       + "    [InsertedBy] \n"
                       + "  ) \n"
                       + "VALUES( @ErrorMessage, @ErrorSource, @HelpLink, @HResult, @StackTrace1, @InnerExpMsg, @InnerExpSource,  \n"
                       + "@StackTrace2, @IPAddress, @AbsolutePathUrl, GETDATE(), @InsertedBy)";

                    command.Parameters.AddWithValue("@ErrorMessage", ErrorMessage.Length > 1999 ? ErrorMessage.Substring(0, 1998) : ErrorMessage);
                    command.Parameters.AddWithValue("@ErrorSource", ErrorSource.Length > 199 ? ErrorSource.Substring(0, 198) : ErrorSource);
                    command.Parameters.AddWithValue("@HelpLink", HelpLink.Length > 199 ? HelpLink.Substring(0, 198) : HelpLink);
                    command.Parameters.AddWithValue("@HResult", DBNull.Value);
                    command.Parameters.AddWithValue("@StackTrace1", StackTrace1.Length > 1999 ? StackTrace1.Substring(0, 1998) : StackTrace1);
                    command.Parameters.AddWithValue("@InnerExpMsg", InnerExpMsg.Length > 1999 ? InnerExpMsg.Substring(0, 1998) : InnerExpMsg);
                    command.Parameters.AddWithValue("@InnerExpSource", InnerExpSource.Length > 199 ? InnerExpSource.Substring(0, 199) : InnerExpSource);
                    command.Parameters.AddWithValue("@StackTrace2", StackTrace2.Length > 1999 ? StackTrace2.Substring(0, 1998) : StackTrace2);
                    command.Parameters.AddWithValue("@IPAddress", "");
                    command.Parameters.AddWithValue("@AbsolutePathUrl", urlAbsolutePath.Length > 255 ? urlAbsolutePath.Substring(0, 254) : urlAbsolutePath);
                    command.Parameters.AddWithValue("@InsertedBy", InsertedBy);

                    command.ExecuteNonQuery();
                }

            }
            catch  {
                //Do Nothing
            }
            finally {
                //if (sqlConn.State == ConnectionState.Open)
                //    sqlConn.Close();
            }
        }
        private string GetErrorLogPath(string obj_id)
        {
            string query = string.Format("Select value from APP_UPDATE_VALUE where obj_id = {0}", obj_id);
            SqlCommand cmd = new SqlCommand(query, new SqlConnection(connString));
            return cmd.ExecuteScalar().ToString();
        }
        private void CreateExceptionString(StringBuilder sb, Exception ex, string indent)
        {
            if (indent == null) {
                indent = string.Empty;
            }
            else if (indent.Length > 0) {
                sb.AppendFormat("{0}Inner Exception Details:", indent);
            }
            sb.Append(Environment.NewLine + "==========================================================================" + Environment.NewLine);
            sb.AppendFormat("Exception Found on {0}:\n {1} Type: {2}", DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss"), indent, ex.GetType().FullName);
            sb.AppendFormat("{2} {0}Message: {1}", indent, ex.Message, Environment.NewLine);
            sb.AppendFormat("{2} {0}Source: {1}", indent, ex.Source, Environment.NewLine);
            sb.AppendFormat("{2} {0}Stacktr//ace: {1}", indent, ex.StackTrace, Environment.NewLine);
            sb.Append(Environment.NewLine + "--------------------------------------------------------------------------------------------------------- " + Environment.NewLine);
            if (ex.InnerException != null) {
                sb.Append("\n");
                CreateExceptionString(sb, ex.InnerException, indent + "  ");
            }
        }
    }
}

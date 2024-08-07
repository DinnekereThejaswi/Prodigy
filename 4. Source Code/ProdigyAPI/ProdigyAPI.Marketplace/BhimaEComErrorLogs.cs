using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace
{
    public class BhimaEComErrorLogs : AbsErrorLog
    {
        public override void LogInfo(string log)
        {

        }
        public override void LogException(Exception ex)
        {
            MagnaDbEntities dbContext = new MagnaDbEntities();
        }
        public void LogInfo(EcomOrderAPILog ecomLog)
        {
            try {
                MagnaDbEntities dbContext = new MagnaDbEntities();
                dbContext.EcomOrderAPILogs.Add(ecomLog);
                dbContext.SaveChanges();
            }
            catch  {

            }
        }
    }
}

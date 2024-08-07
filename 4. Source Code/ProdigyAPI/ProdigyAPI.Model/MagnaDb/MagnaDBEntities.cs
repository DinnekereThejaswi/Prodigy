using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Model.MagnaDb
{
    public partial class MagnaDbEntities
    {
        public void SetCommandTimeOut(int timeoutSeconds)
        {
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = timeoutSeconds;
        }

        public MagnaDbEntities(bool logSql)
            : base("name=MagnaDbEntities")
        {
            if (logSql)
                Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }
    }
}

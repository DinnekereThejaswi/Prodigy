using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 8th July 2021
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    interface ICessMasterBL
    {
        dynamic GetLedgers(string companyCode, string branchCode, out ErrorVM error);
        dynamic GetCessDetails(string companyCode, string branchCode, out ErrorVM error);
        bool Save(CessVM cess, out ErrorVM error);
        bool Update(CessVM cess, out ErrorVM error);
    }
    public sealed class CessMasterBL:ICessMasterBL
    {
        #region Declaration
        public readonly MagnaDbEntities dbContext = new MagnaDbEntities();
        public static readonly Lazy<CessMasterBL> cessBL = new Lazy<CessMasterBL>(() => new CessMasterBL());
        public static CessMasterBL GetInstance { get { return cessBL.Value; } }
        #endregion

        #region Constructor
        private CessMasterBL() { }
        #endregion

        #region Methods
        public dynamic GetLedgers(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return null;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public dynamic GetCessDetails(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return null;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public bool Save(CessVM cess, out ErrorVM error)
        {
            error = null;
            try {
                return true;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        public bool Update(CessVM cess, out ErrorVM error)
        {
            error = null;
            try {
                return true;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        #endregion

        #region Private Methods
        #endregion
    }
}

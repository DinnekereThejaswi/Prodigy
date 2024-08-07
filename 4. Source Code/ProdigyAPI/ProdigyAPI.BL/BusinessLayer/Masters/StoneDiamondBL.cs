using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    public class StoneDiamondBL
    {
        MagnaDbEntities db = null;

        public StoneDiamondBL()
        {
            db = new MagnaDbEntities(true);
        }

        public StoneDiamondBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        #region This won't work since Linq and EF are fully typed. So this functionis discarded. This code is for trial purpose
        //public List<ListOfValue> GetDiamondColor(string companyCode, string branchCode, string attributeType)
        //{
        //    List<ListOfValue> diaAttrList = null;
        //    DbSet dbEntry = null;

        //    switch (attributeType) {
        //        case "COL":
        //            dbEntry = db.KSTU_DIAMOND_COLOR;
        //            break;
        //        case "CUT":
        //            dbEntry = db.KSTU_DIAMOND_COLOR;
        //            break;
        //        case "CLA":
        //            dbEntry = db.KSTU_DIAMOND_COLOR;
        //            break;
        //        case "CER":
        //            dbEntry = db.KSTU_DIAMOND_CERTIFICATE;
        //            break;
        //        case "SHA":
        //            dbEntry = db.KSTU_DIAMOND_SHAPE;
        //            break;
        //        case "POL":
        //            dbEntry = db.KSTU_DIAMOND_POLISH;
        //            break;
        //        case "SYM":
        //            dbEntry = db.KSTU_DIAMOND_SYMMETRY;
        //            break;
        //        case "FLO":
        //            dbEntry = db.KSTU_DIAMOND_FLUORESCENCE;
        //            break;
        //        case "SIZ":
        //            dbEntry = db.KSTU_DIAMOND_SIZE;
        //            break;
        //    }

        //    //This won't work since Linq and EF are fully typed. So this functionis discarded. This code is for trial purpose
        //    return dbEntry.Where(x => x.company_code == companyCode && x.branch_code == branchCode
        //        && x.obj_status != "C").OrderByDescending(y => y.description).Select(lov => new ListOfValue
        //        {
        //            Code = lov.code,
        //            Name = lov.description
        //        }).ToList(); ;
        //} 
        #endregion

        public List<ListOfValue> GetStoneType()
        {
            List<ListOfValue> stoneTypes = new List<ListOfValue>();
            stoneTypes.Add(new ListOfValue { Code = "Ordinary", Name = "Ordinary" });
            stoneTypes.Add(new ListOfValue { Code = "Semi Precious", Name = "Semi Precious" });
            stoneTypes.Add(new ListOfValue { Code = "Precious", Name = "Precious" });
            return stoneTypes;
        }

        public IQueryable<StoneMasterVM> GetStoneOrDiamondList(string companyCode, string branchCode, string stoneType, bool isDiamond = false)
        {
            var stoneOrDia = isDiamond == true ? "DMD" : "STN";
            var stoneDiaDetail = db.KSTU_STONE_DIAMOND_MASTER
                    .Where(s => s.type == stoneOrDia)
                   .Select(s => new StoneMasterVM
                   {
                       CompanyCode = s.company_code,
                       BranchCode = s.branch_code,
                       Type = s.type,
                       StoneType = s.stone_types,
                       StoneName = s.stone_name,
                       CounterCode = s.counter_code,
                       BrandName = s.brand_name,
                       Color = s.color,
                       Cut = s.cut,
                       Clarity = s.clarity,
                       Status = s.obj_status == "O" ? "Active" : "Closed",
                       Code = s.code,
                       Batch = s.batch,
                       Uom = s.uom,
                       HSN = s.HSN,
                       StoneValue = s.stone_value,
                       GSTGroupCode = s.GSTGroupCode
                   }).OrderByDescending(c => c.StoneName);
            //If the request is for stones,I need to check to check for stoneType and that is null or empty, full list to be returned.
            if (isDiamond == false) {
                if (!string.IsNullOrEmpty(stoneType))
                    return stoneDiaDetail.Where(s => s.StoneType == stoneType);
                else
                    return stoneDiaDetail;
            }
            else
                return stoneDiaDetail;
        }

        public StoneMasterVM GetStoneOrDiamondDetail(string companyCode, string branchCode, string code, bool isDiamond = false)
        {
            return GetStoneOrDiamondList(companyCode, branchCode, string.Empty, isDiamond)
                .Where(x => x.StoneName == code).FirstOrDefault();
        }

        public List<ListOfValue> GetDiamondColor(string companyCode, string branchCode)
        {
            return db.KSTU_DIAMOND_COLOR.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_status != "C").OrderByDescending(y => y.description).Select(lov => new ListOfValue
                {
                    Code = lov.code,
                    Name = lov.description
                }).ToList();
        }

        public List<ListOfValue> GetDiamondCut(string companyCode, string branchCode)
        {
            return db.KSTU_DIAMOND_CUT.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_status != "C").OrderByDescending(y => y.description).Select(lov => new ListOfValue
                {
                    Code = lov.code,
                    Name = lov.description
                }).ToList();
        }

        public List<ListOfValue> GetDiamondClarity(string companyCode, string branchCode)
        {
            return db.KSTU_DIAMOND_CLARITY.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_status != "C").OrderByDescending(y => y.description).Select(lov => new ListOfValue
                {
                    Code = lov.code,
                    Name = lov.description
                }).ToList();
        }

        public List<ListOfValue> GetDiamondCertificate(string companyCode, string branchCode)
        {
            return db.KSTU_DIAMOND_CERTIFICATE.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_status != "C").OrderByDescending(y => y.description).Select(lov => new ListOfValue
                {
                    Code = lov.code,
                    Name = lov.description
                }).ToList();
        }

        public List<ListOfValue> GetDiamondShape(string companyCode, string branchCode)
        {
            return db.KSTU_DIAMOND_SHAPE.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_status != "C").OrderByDescending(y => y.description).Select(lov => new ListOfValue
                {
                    Code = lov.code,
                    Name = lov.description
                }).ToList();
        }

        public List<ListOfValue> GetDiamondPolish(string companyCode, string branchCode)
        {
            return db.KSTU_DIAMOND_POLISH.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_status != "C").OrderByDescending(y => y.description).Select(lov => new ListOfValue
                {
                    Code = lov.code,
                    Name = lov.description
                }).ToList();
        }

        public List<ListOfValue> GetDiamondSymmetry(string companyCode, string branchCode)
        {
            return db.KSTU_DIAMOND_SYMMETRY.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_status != "C").OrderByDescending(y => y.description).Select(lov => new ListOfValue
                {
                    Code = lov.code,
                    Name = lov.description
                }).ToList();
        }

        public List<ListOfValue> GetDiamondFluorescence(string companyCode, string branchCode)
        {
            return db.KSTU_DIAMOND_FLUORESCENCE.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_status != "C").OrderByDescending(y => y.description).Select(lov => new ListOfValue
                {
                    Code = lov.code,
                    Name = lov.description
                }).ToList();
        }

        public List<ListOfValue> GetDiamondSize(string companyCode, string branchCode)
        {
            return db.KSTU_DIAMOND_SIZE.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_status != "C").OrderByDescending(y => y.description).Select(lov => new ListOfValue
                {
                    Code = lov.code,
                    Name = lov.description
                }).ToList();
        }

    }
}

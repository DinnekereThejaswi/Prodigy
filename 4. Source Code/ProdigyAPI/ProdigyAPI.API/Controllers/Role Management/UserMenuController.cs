using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers
{
    [Authorize]
    public class UserMenuController : ApiController
    {
        private MagnaDbEntities dbContext = new MagnaDbEntities(true);
        [HttpGet]
        [Route("api/usermenu/get")]
        public IHttpActionResult Get()
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            string roleIDString = principal.Claims.Where(c => c.Type == "RoleID").FirstOrDefault().Value;
            string companyCode = principal.Claims.Where(c => c.Type == "CompanyCode").FirstOrDefault().Value;
            string branchCode = principal.Claims.Where(c => c.Type == "BranchCode").FirstOrDefault().Value;
            if (string.IsNullOrEmpty(roleIDString))
                return NotFound();
            int roleID = Convert.ToInt32(roleIDString);

            try {
                var rolePermissions = dbContext.RolePermissions.Where(rp => rp.RoleID == roleID &&
                    rp.CompanyCode == companyCode && rp.BranchCode == branchCode).OrderBy(rpo => rpo.ModuleID).OrderBy(rpo => rpo.ModuleSeqNo).ToList();
                if (rolePermissions == null)
                    return NotFound();
                var rolPerm = from r in rolePermissions
                              group r by new
                              {
                                  r.ModuleID,
                                  r.ModuleName,
                                  r.ModuleSeqNo,
                                  r.MasterAngularUIRoute,
                                  r.MasterIcon,
                                  r.MasterClass,
                                  r.MasterLabel,
                                  r.MasterLabelClass,
                                  r.FormType,
                                  r.ReportServerType,
                                  r.ReportApiRoute
                              } into rg
                              select (new ApplicationMenu
                              {
                                  ID = rg.Key.ModuleID,
                                  Tittle = rg.Key.ModuleName,
                                  SortOrder = rg.Key.ModuleSeqNo,
                                  Path = rg.Key.MasterAngularUIRoute,
                                  Icon = rg.Key.MasterIcon,
                                  Class = rg.Key.MasterClass,
                                  Label = rg.Key.MasterLabel,
                                  LabelClass = rg.Key.MasterLabelClass,
                                  lstOfMainMenuWithSubMenu = rg.Select(rl => new ApplicationMenu
                                  {
                                      ID = Convert.ToInt32(rl.SubModuleID),
                                      Tittle = rl.SubModuleName,
                                      Path = rl.AngularUIRoute + "/" + rl.ReportApiRoute,
                                      SortOrder = rl.SubModuleSeqNo,
                                      Icon = rl.Icon,
                                      Class = rl.Class,
                                      Label = rl.Label,
                                      LabelClass = rl.LabelClass,
                                      FormType = rl.FormType,
                                      ReportServerType = rl.ReportServerType,
                                      ReportApiRoute = rl.ReportApiRoute
                                  }).OrderBy(rlo => rlo.SortOrder).ToList()
                              });
                return Ok(rolPerm);
            }
            catch (Exception ex) {
                string errorMsg = ex.Message;
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, errorMsg));
            }

        }

        [HttpGet]
        [Route("api/usermenu/nestedmenu")]
        public IHttpActionResult GetNewMenu()
        {
            //ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            //string roleIDString = principal.Claims.Where(c => c.Type == "RoleID").FirstOrDefault().Value;
            //if (string.IsNullOrEmpty(roleIDString))
            //    return NotFound();
            //int roleID = Convert.ToInt32(roleIDString);

            try {
                var menuList = dbContext.Menus.ToList();
                var rolPerm = from m in menuList
                              where m.ParentID == null
                              orderby m.SeqNo
                              select (new ApplicationMenu
                              {
                                  ID = m.ID,
                                  Tittle = m.Name,
                                  Path = m.Route,
                                  Icon = m.Icon,
                                  Class = m.CssClass,
                                  ExtraLink = false,
                                  ParentID = m.ParentID,
                                  Label = m.Label,
                                  LabelClass = m.LabelClass,
                                  SortOrder = m.SeqNo,
                                  lstOfMainMenuWithSubMenu = GetChildren(menuList, m.ID)
                              });
                return Ok(rolPerm);
            }
            catch (Exception ex) {
                string errorMsg = ex.Message;
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, errorMsg));
            }

        }

        private List<ApplicationMenu> GetChildren(List<Menu> appMenu, int? parentId)
        {
            return appMenu
                    .Where(m => m.ParentID == parentId)
                    .Select(m => new ApplicationMenu
                    {
                        ID = m.ID,
                        Tittle = m.Name,
                        Path = m.Route,
                        Icon = m.Icon,
                        Class = m.CssClass,
                        ExtraLink = false,
                        ParentID = m.ParentID,
                        Label = m.Label,
                        LabelClass = m.LabelClass,
                        SortOrder = m.SeqNo,
                        lstOfMainMenuWithSubMenu = GetChildren(appMenu, m.ID)
                    }).OrderBy(t => t.SortOrder).ToList();
        }

        [HttpGet]
        [Route("api/usermenu/getmodulefunction")]
        public IHttpActionResult GetModuleFunctions(int id)
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            string roleIDString = principal.Claims.Where(c => c.Type == "RoleID").FirstOrDefault().Value;
            if (string.IsNullOrEmpty(roleIDString))
                return NotFound();

            //int roleID = 8;//For testing.
            int roleID = Convert.ToInt32(roleIDString);
            var rolePermissions = dbContext.ModuleFunctions.Where(rp => rp.SubModuleID == id && rp.RoleID == roleID).ToList();
            if (rolePermissions == null)
                return NotFound();

            return Ok(rolePermissions);
        }

        [HttpGet]
        [Route("api/Usermenu/GetAccessBranches")]
        public IHttpActionResult GetAccessBranches()
        {
            List<AccessCompany> lstCompany = new List<AccessCompany>();
            AccessCompany company1 = new AccessCompany();
            company1.CompanyCode = "BH";
            company1.CompanyName = "Bhima Jewellers";
            company1.Branches = new List<AcccessBranch>()
            {
                new AcccessBranch() {BranchCode = "JNR", BranchName="Jayanagar", IsDefault = true, SortOrder = 1},
                new AcccessBranch() { BranchCode = "DKN",BranchName="Dickenson", IsDefault = false, SortOrder = 2 },
                new AcccessBranch() { BranchCode = "RJR",BranchName="Rajajinagar", IsDefault = false, SortOrder = 3 }
            };
            lstCompany.Add(company1);

            AccessCompany company2 = new AccessCompany();
            company2.CompanyCode = "RN";
            company2.CompanyName = "Aryan Gems";
            company2.Branches = new List<AcccessBranch>()
            {
                new AcccessBranch() {BranchCode = "NRJ", BranchName="Jayanagar", IsDefault = true, SortOrder = 1},
                new AcccessBranch() { BranchCode = "KND",BranchName="Dickenson", IsDefault = false, SortOrder = 2 },
                new AcccessBranch() { BranchCode = "JRR",BranchName="Rajajinagar", IsDefault = false, SortOrder = 3}
            };
            lstCompany.Add(company2);
            return Ok(lstCompany);
        }
    }
    public class AccessCompany
    {
        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }
        public List<AcccessBranch> Branches { get; set; }
    }
    public class AcccessBranch
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public bool IsDefault { get; set; }
        public int SortOrder { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MagnaWeb.Models.TestModel;
using MagnaWeb.Controllers;
using MagnaWeb.Models.ViewModels;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Net;

namespace Prodigy.Tests
{
    [TestClass]
    public class SalesPersonControllerTest
    {
        [TestMethod]
        public void GetAllSalesPersons_CountTest()
        {
            SalesPersonsController controller = new SalesPersonsController();
            var salesPersons = controller.List() as IQueryable<SalesPersonsViewModel>;
            long expectedSalesPersonCount = 6; //ideally this value should be fetched from settings
            Assert.AreEqual(expectedSalesPersonCount, salesPersons.Count());
        }

        [TestMethod]
        public void GetSalesPerson_CorrectSalesPersonTest()
        {
            SalesPersonsController controller = new SalesPersonsController();
            var salesPerson = controller.Get(1) as OkNegotiatedContentResult<SalesPersonsViewModel>;
            string expectedName = "Amrutha A A";
            Assert.AreEqual(expectedName, salesPerson.Content.Name);
        }

        [TestMethod]
        public void PostSalesPerson_CheckIfPostingCorrectly()
        {
            SalesPersonsController controller = new SalesPersonsController();
            string controllerMethod = "salesperson/post";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(new CommonUtilities().GetUrl(controllerMethod))
            };
            
            string salesPersonName = "Kiran KK";
            SalesPersonsViewModel sv = new SalesPersonsViewModel
            {
                Name = salesPersonName,
                Designation = "ERP Asst. Manager",
                IncentivePercent = 100,
                JoiningDate = DateTime.Now,
                Blocked = false,
                WarehouseID = 1
            };
            IHttpActionResult postResult = controller.Post(sv);
            var createdResult = postResult as CreatedAtRouteNegotiatedContentResult<SalesPersonsViewModel>;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            //check ID
            //Assert.AreEqual(82, createdResult.RouteValues["id"]);
            //check name
            Assert.AreEqual(salesPersonName, createdResult.Content.Name);
        }

        [TestMethod]
        public void PutSalesPerson_CheckForUpdate()
        {
            SalesPersonsController controller = new SalesPersonsController();
            string controllerMethod = "salesperson/put";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(new CommonUtilities().GetUrl(controllerMethod))
            };

            SalesPersonsViewModel sv = new SalesPersonsViewModel
            {
                ID = 73,
                Name = "Sohan Flat 002",
                Designation = "Team Lead2.0",
                IncentivePercent = 68.95M,
                JoiningDate = DateTime.Now,
                Blocked = false,
                WarehouseID = 1
            };
            IHttpActionResult putResult = controller.Put(sv.ID, sv);
            var contentResult = putResult as NegotiatedContentResult<SalesPersonsViewModel>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.Accepted, contentResult.StatusCode);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual("Sohan Flat 002", contentResult.Content.Name);
        }

        [TestMethod]
        public void DeleteSalesPerson_CheckForDelete()
        {
            SalesPersonsController controller = new SalesPersonsController();
            string controllerMethod = "salesperson/delete";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(new CommonUtilities().GetUrl(controllerMethod))
            };

            int idToDelete = 171;
            IHttpActionResult deleteResult = controller.Delete(idToDelete);
            Assert.IsInstanceOfType(deleteResult, typeof(OkResult));

        }
    }
}

using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.UserManagement
{
    public interface IUserManagement
    {
        List<OperatorViewModel> List(string companyCode, string branchCode, out ErrorVM error);
        OperatorViewModel Get(string companyCode, string branchCode, string operatorCode, out ErrorVM error);
        bool Add(OperatorViewModel operatorViewModel, string userID, out ErrorVM error);
        bool Update(OperatorViewModel operatorViewModel, string userID, out ErrorVM error);
        bool Close(string companyCode, string branchCode, string operatorCode, string userID, out ErrorVM error);
        bool Open(string companyCode, string branchCode, string operatorCode, string userID, out ErrorVM error);
        bool ChangePasword(OperatorPasswordViewModel vm, string userID, out ErrorVM error);
    }
}

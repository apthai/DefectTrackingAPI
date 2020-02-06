using com.apthai.DefectAPI.Model.DefectAPI;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.DefectAPI.HttpRestModel
{
    public partial class LoginData
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AppCode { get; set; }

    }
    public class AutorizeDataJWT
    {
        public bool LoginResult { get; set; }
        public string LoginResultMessage { get; set; }
        public string UserPrincipalName { get; set; }
        public string DomainUserName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeID { get; set; }
        public string Email { get; set; }
        public string Division { get; set; }

        public string Token { get; set; }

        public DateTime? AccountExpirationDate { get; set; }
        public DateTime? LastLogon { get; set; }

        public string AuthenticationProvider { get; set; }
        public string SysUserId { get; set; }
        public string SysUserData { get; set; }
        public string SysUserRoles { get; set; }
        public string SysAppCode { get; set; }
        public string AppUserRole { get; set; }
        public string UserProject { get; set; }
        public string UserApp { get; set; }
        
    }
}

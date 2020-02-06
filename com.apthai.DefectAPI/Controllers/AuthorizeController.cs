using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using com.apthai.CoreApp.Data.Services;
using com.apthai.DefectAPI.CustomModel;
using com.apthai.DefectAPI.HttpRestModel;
using com.apthai.DefectAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; 
using Newtonsoft.Json.Linq;
using com.apthai.DefectAPI.Model.DefectAPI;   
using com.apthai.DefectAPI.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace com.apthai.DefectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : BaseController
    {


        private readonly IMasterRepository _masterRepo;
        private readonly IAuthorizeService _authorizeService;
        private readonly IUserRepository _UserRepository;
        public AuthorizeController(IMasterRepository masterRepo , IAuthorizeService authorizeService,IUserRepository userRepository)
        {

            _masterRepo = masterRepo;
            _authorizeService = authorizeService;
            _UserRepository = userRepository;
        }



        //[HttpPost]
        //[Route("login")]

        //public async Task<object> PostLogin([FromBody] LoginData data)
        //{
        //    try
        //    {
        //        var userName = data.UserName;
        //        var password = data.Password;
        //        var appCode = data.AppCode;
        //        var userQis = await _authorizeService.UserLoginAsync(userName, password, appCode);
        //        string userGuid = "";

        //        if (userQis.UserID > 0)
        //        {


        //            if (userQis != null && userQis.UserData != null && userQis.UserData["User"] != null)
        //            {
        //                userGuid = userQis.UserData["User"]["UserGUID"].ToString();
        //            }



        //            if (true)//userQis.Password  "1234" == Password
        //            {

        //                var permisionData = new PermisionData();
        //                permisionData.CanAccess = false;
        //                permisionData.VisibleProductList = new List<Model.QIS.vwMProject>();
        //                permisionData.VisibleProductType = new List<string>();
        //                var projectAc = await _masterRepo.GetViwewProjectByUser(userQis.UserID);
        //                if (projectAc != null)
        //                {
        //                    foreach (var v in projectAc)
        //                    {
        //                        permisionData.VisibleProductList.Add(v);
        //                        permisionData.VisibleProductType.Add(v.BU) ;

        //                    }
        //                }

        //                permisionData.CanAccess = (permisionData.VisibleProductList.Count > 0 || permisionData.VisibleProductType.Count > 0);
        //                //var userRolesData = userRoles.Select(e=> new {
        //                //    RoleID = e.RoleID,
        //                //    RoleCode = e.RoleCode,
        //                //    RoleName = e.RoleName
        //                //}).ToList();

        //                //------------------ Uncomment After SE Level Launch
        //                var _userLevel = await _masterRepo.GetUserLevel(userQis.UserID);
        //                if (userQis.Roles.Where(e => e.RoleCode == "SE").Any())
        //                {

        //                    var arrCode = projectAc.Select(e => e.ProjectCode).ToList().Distinct().ToList();
        //                    _userLevel = _userLevel.Where(e => arrCode.Contains(e.ProjectCode)).ToList();

        //                }

        //                return new
        //                {

        //                    success = true,
        //                    valid = true,
        //                    data = new
        //                    {

        //                        UserId = userQis.UserID,
        //                        //EmployeeId = userQis.EmpCode,
        //                        //UserName = userQis.UserName ,
        //                        EmployeeId = userQis.EmployeeID,
        //                        UserGuid = userGuid,                            // insert by K.Pacharavach
        //                        UserID = userQis.UserID, //UserId, //userQis.UserName,
        //                        UserFullName = userQis.DisplayName,// string.Format("{0}{1} {2}", userQis.TitleName ,userQis.FirstName , userQis.LastName) ,
        //                        Department = userQis.Department,// user.Department ,                                       
        //                        Email = userQis.Email,//empData.Email,
        //                        PermissionData = permisionData , // JsonConvert.SerializeObject(permisionData, Formatting.Indented),
        //                        UserRolesData = userQis.Roles , //JsonConvert.SerializeObject(userQis.Roles, Formatting.Indented),
        //                        JobTitle = userQis.Position, // empData.PositionName,// user.JobTitle,
        //                        LoginDate = DateTime.Now,
        //                        LaunchInformUrl = "",
        //                        PasswordMessageCode = userQis.PasswordMessageCode,
        //                        PasswordMessageText = userQis.PasswordMessageText,
        //                        PasswordRedirectUrl = userQis.PasswordRedirectUrl,
        //                        UserLevel = _userLevel
        //                    }

        //                };
        //            }

        //        }



        //        return new
        //        {
        //            success = true,
        //            valid = false
        //        };




        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Internal server error :: " + ex.Message );
        //    }
        //}

        [HttpPost]
        [Route("login")]
        [SwaggerOperation(Summary = "Log In เข้าสู้ระบบเพื่อรับ Access Key ",
        Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> PostLogin([FromBody] LoginData data)
        {
            try
            {
                var userName = data.UserName;
                var password = data.Password;
                var appCode = data.AppCode;

                string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                if (APApiKey == null)
                {
                    APApiKey = UtilsProvider.AppSetting.ApiKey;
                }
                string APApiToken = Environment.GetEnvironmentVariable("Api_Token");
                if (APApiToken == null)
                {
                    APApiToken = UtilsProvider.AppSetting.ApiToken;
                }
                
                var client = new HttpClient();
                var Content = new StringContent(JsonConvert.SerializeObject(data));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                Content.Headers.Add("api_key", APApiKey);
                Content.Headers.Add("api_token", APApiToken);
                string PostURL = Environment.GetEnvironmentVariable("AuthenticationURL");
                if (PostURL == null)
                {
                    PostURL = UtilsProvider.AppSetting.AuthorizeURL; 
                }
                var Respond = await client.PostAsync(PostURL, Content);
                if (Respond.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        valid = false
                    };
                }
                var RespondData = await Respond.Content.ReadAsStringAsync();
                var Result = JsonConvert.DeserializeObject<AutorizeDataJWT>(RespondData);
                if (Result.LoginResult == false)
                {
                    return new
                    {
                        success = false,
                        date = Result.LoginResultMessage,
                        valid = false
                    };
                }
                AccessKeyControl AC = _UserRepository.GetUserAccessKey(Result.EmployeeID);
                if (AC == null)
                {
                    AccessKeyControl accessKeyControl = new AccessKeyControl();
                    accessKeyControl.EmpCode = Result.EmployeeID;
                    accessKeyControl.AccessKey = generateAccessKey(Result.EmployeeID);
                    accessKeyControl.LoginDate = DateTime.Now;

                    bool Insert = _UserRepository.InsertUserAccessKey(accessKeyControl);

                    return new
                    {
                        success = true,
                        date = Result,
                        AccessKey = accessKeyControl.AccessKey,
                        valid = false
                    };
                }
                else
                {
                    AC.AccessKey = generateAccessKey(Result.EmployeeID);
                    AC.LoginDate = DateTime.Now;

                    bool Update = _UserRepository.UpdateUserAccessKey(AC);

                    return new
                    {
                        success = true,
                        date = Result,
                        AccessKey = AC.AccessKey,
                        valid = false
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string generateAccessKey(string EmpCode)
        {
            return string.Format("{0}_{1:N}", EmpCode, Guid.NewGuid());
        }
    }
}
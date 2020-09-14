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



        [HttpGet("FindUserProjectByNamenew/{EmpCode}")]
        public async Task<object> FindUserProjectByNamenew(string EmpCode)
        {
            try
            {
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
                string PostURL = Environment.GetEnvironmentVariable("AuthenticationURL");
                PostURL = PostURL + "FindUserProjectByNamenew/" + EmpCode;
                if (PostURL == null)
                {
                    PostURL = UtilsProvider.AppSetting.AuthorizeURL + "FindUserProjectByNamenew/" + EmpCode;
                }
                    var Respond = await client.GetAsync(PostURL);
                    if (Respond.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return new
                        {
                            success = false,
                            data = new AutorizeDataJWT(),
                            valid = false
                        };
                    }
                    List<vwUserProject> data = Respond.Content.ReadAsAsync<List<vwUserProject>>().Result;
                return new
                {
                    success = true,
                    data = data,
                    valid = false
                };




            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }



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
                PostURL = PostURL + "JWTUserLogin";
                if (PostURL == null)
                {
                    PostURL = UtilsProvider.AppSetting.AuthorizeURL + "JWTUserLogin";
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
                AutorizeDataJWT Result = JsonConvert.DeserializeObject<AutorizeDataJWT>(RespondData);
                AutorizeDataJWTReturnObject Return = new AutorizeDataJWTReturnObject();
                Return.AccountExpirationDate = Result.AccountExpirationDate;
                Return.AppUserRole = Result.AppUserRole;
                Return.AuthenticationProvider = Result.AuthenticationProvider;
                Return.CostCenterCode = Result.CostCenterCode;
                Return.CostCenterName = Result.CostCenterName;
                Return.DisplayName = Result.DisplayName;
                Return.Division = Result.Division;
                Return.DomainUserName = Result.DomainUserName;
                Return.Email = Result.Email;
                Return.EmployeeID = Result.EmployeeID;
                Return.FirstName = Result.FirstName;
                Return.LastLogon = Result.LastLogon;
                Return.LastName = Result.LastName;
                Return.LoginResult = Result.LoginResult;
                Return.LoginResultMessage = Result.LoginResultMessage;
                Return.SysAppCode = Result.SysAppCode;
                Return.SysUserData = JsonConvert.DeserializeObject<UserModel>(Result.SysUserData);
                Return.SysUserId = Result.SysUserId;
                Return.SysUserRoles = JsonConvert.DeserializeObject<vwUserRole>(Result.SysUserRoles);
                Return.Token = Result.Token;
                Return.UserApp = JsonConvert.DeserializeObject<List<vwUserApp>>(Result.UserApp);
                Return.UserPrincipalName = Result.UserPrincipalName;
                List<UserProject> userProjects = JsonConvert.DeserializeObject<List<UserProject>>(Result.UserProject);

                List<UserProjectType> userProjectTypes = new List<UserProjectType>();
                for (int i = 0; i < userProjects.Count(); i++)
                {
                    ICONEntFormsProduct  Prd = _masterRepo.GetProductDataFromCRM_Sync(userProjects[i].ProjectCode);
                    string obj = JsonConvert.SerializeObject(userProjects[i]);
                    UserProjectType ProductObj = JsonConvert.DeserializeObject<UserProjectType>(obj);
                    if (Prd != null)
                    {
                        if (Prd.Producttype == "โครงการแนวราบ")
                        {
                            ProductObj.producttypecate = "H";
                        }
                        if (Prd.Producttype == "โครงการแนวสูง")
                        {
                            ProductObj.producttypecate = "V";
                        }
                    }
                    
                    userProjectTypes.Add(ProductObj);
                }

                Return.UserProject = userProjectTypes;
                if (Result.LoginResult == false)
                {
                    return new
                    {
                        success = false,
                        data = Result.LoginResultMessage,
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
                        data = Return,
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
                        data = Return,
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

        [HttpPost]
        [Route("loginHash")]
        [SwaggerOperation(Summary = "Log In เข้าสู้ระบบเพื่อรับ Access Key ",
       Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> loginHash([FromBody] LoginHashData data)
        {
            try
            {

                var userName = SHAHelper.TwoWayDecrypt(data.UserName,"APFamilyKey");
                var password = SHAHelper.TwoWayDecrypt(data.Password, "APFamilyKey");


                var appCode = "Defect";

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
                PostURL = PostURL + "JWTUserLogin";
                if (PostURL == null)
                {
                    PostURL = UtilsProvider.AppSetting.AuthorizeURL + "JWTUserLogin";
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
                AutorizeDataJWT Result = JsonConvert.DeserializeObject<AutorizeDataJWT>(RespondData);
                AutorizeDataJWTReturnObject Return = new AutorizeDataJWTReturnObject();
                Return.AccountExpirationDate = Result.AccountExpirationDate;
                Return.AppUserRole = Result.AppUserRole;
                Return.AuthenticationProvider = Result.AuthenticationProvider;
                Return.CostCenterCode = Result.CostCenterCode;
                Return.CostCenterName = Result.CostCenterName;
                Return.DisplayName = Result.DisplayName;
                Return.Division = Result.Division;
                Return.DomainUserName = Result.DomainUserName;
                Return.Email = Result.Email;
                Return.EmployeeID = Result.EmployeeID;
                Return.FirstName = Result.FirstName;
                Return.LastLogon = Result.LastLogon;
                Return.LastName = Result.LastName;
                Return.LoginResult = Result.LoginResult;
                Return.LoginResultMessage = Result.LoginResultMessage;
                Return.SysAppCode = Result.SysAppCode;
                Return.SysUserData = JsonConvert.DeserializeObject<UserModel>(Result.SysUserData);
                Return.SysUserId = Result.SysUserId;
                Return.SysUserRoles = JsonConvert.DeserializeObject<vwUserRole>(Result.SysUserRoles);
                Return.Token = Result.Token;
                Return.UserApp = JsonConvert.DeserializeObject<List<vwUserApp>>(Result.UserApp);
                Return.UserPrincipalName = Result.UserPrincipalName;
                List<UserProject> userProjects = JsonConvert.DeserializeObject<List<UserProject>>(Result.UserProject);

                List<UserProjectType> userProjectTypes = new List<UserProjectType>();
                for (int i = 0; i < userProjects.Count(); i++)
                {
                    ICONEntFormsProduct Prd = _masterRepo.GetProductDataFromCRM_Sync(userProjects[i].ProjectCode);
                    string obj = JsonConvert.SerializeObject(userProjects[i]);
                    UserProjectType ProductObj = JsonConvert.DeserializeObject<UserProjectType>(obj);
                    if (Prd != null)
                    {
                        if (Prd.Producttype == "โครงการแนวราบ")
                        {
                            ProductObj.producttypecate = "H";
                        }
                        if (Prd.Producttype == "โครงการแนวสูง")
                        {
                            ProductObj.producttypecate = "V";
                        }
                    }

                    userProjectTypes.Add(ProductObj);
                }

                Return.UserProject = userProjectTypes;
                if (Result.LoginResult == false)
                {
                    return new
                    {
                        success = false,
                        data = Result.LoginResultMessage,
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
                        data = Return,
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
                        data = Return,
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
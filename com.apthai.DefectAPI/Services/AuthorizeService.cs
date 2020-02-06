using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
//using DapperExtensions;
//using System.DirectoryServices;
//using System.Web.Script.Serialization;
//using com.apthai.CoreApp.Data.webSerInstance;
//using com.apthai.CoreApp.Data.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using com.apthai.DefectAPI.Data;
using com.apthai.DefectAPI.Model.DefectAPI;
using webSerInstance;
using System.ServiceModel;
using System.ServiceModel.Channels;
using com.apthai.DefectAPI.Services;
using com.apthai.DefectAPI.Data.CustomModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using com.apthai.DefectAPI.Configuration;
using com.apthai.DefectAPI.Repositories;

namespace com.apthai.CoreApp.Data.Services
{
    public class AuthorizeService : IAuthorizeService
    {


        public const string TAG = "AuthourizeService";
        //private string WsUsername = ConfigurationManager.AppSettings["Webservice.Username"];
        //private string WsPassword = ConfigurationManager.AppSettings["Webservice.Password"];

        //public IScopeUnitOfWork UnitOfWork { get; set; }


        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private IUserRepository _UserRepository;
        private AppSettings _setting;

        public AuthorizeService(ILogger<AuthorizeService> logger, IConfiguration config , AppSettings setting,IUserRepository userRepository)
        {
            _logger = logger;
            _config = config;
            _setting = setting;
            _UserRepository = userRepository;
        }

        public async System.Threading.Tasks.Task<WSAuthorizeModel> UserLoginAsync(string UserName, string Password, string AppCode)
        {

            var resturnUser = new WSAuthorizeModel();
            var result = new AutorizeData();
            var AuthenticationProvider = "domain";
            var AuthorizeEndpointUrlAddress = Environment.GetEnvironmentVariable("AuthorizeEndpointUrlAddress");
            BasicHttpBinding binding = new BasicHttpBinding();

            // Use double the default value
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.MaxBufferSize = Int32.MaxValue;
            //Create instance of SOAP client
            webSerInstance.AuthorizeSoapClient soapClient = new AuthorizeSoapClient(
                //new BasicHttpBinding(BasicHttpSecurityMode.None)
                binding
                , new EndpointAddress(AuthorizeEndpointUrlAddress)); 
             
            //Create instance of credentials
            webSerInstance.AuthorizeSoapHeader soapHeader = new AuthorizeSoapHeader();
            soapHeader.username = _setting.SoapHeaderUsername;
            soapHeader.password = _setting.SoapHeaderPassword;


            try
            {

                    var header = MessageHeader.CreateHeader("AuthorizeSoapHeader", AuthorizeEndpointUrlAddress, soapHeader, new CFMessagingSerializer(typeof(AuthorizeSoapHeader)));
                    var userLogin = new UserLoginRequest(soapHeader, UserName, Password, AppCode);
                    var result1 = await soapClient.UserLoginAsync(userLogin);
                    result = result1.UserLoginResult;
                    result.AuthenticationProvider = AuthenticationProvider;
                    if (result.LoginResult)
                    {
                        resturnUser = new WSAuthorizeModel(result);
                    }                  

            }
            catch (Exception ex)
            {
                throw;
            }
            return resturnUser;

        }
        public bool AccessKeyAuthentication(string AC , string EmpCode)
        {
            AccessKeyControl accessKeyControl = _UserRepository.CheckUserAccessKey(EmpCode, AC);
            if (accessKeyControl != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //public CustomModels.WSAuthorizeModel GetUserPrincipal(string userName)
        //{

        //    var resturnUser = new CustomModels.WSAuthorizeModel();
        //    var result = new AutorizeData();
        //  //  var AuthenticationProvider = ConfigurationManager.AppSettings["Authorize.Provider"].ToLower();

        //    if (AuthenticationProvider == "domain")
        //    {
        //        //userName = "ap-thai\\chaituch";

        //        var webSerInstanceAuthorize = new webSerInstance.Authorize();
        //        webSerInstanceAuthorize.AuthorizeSoapHeaderValue = new AuthorizeSoapHeader() { username = WsUsername, password = WsPassword };
        //        result = webSerInstanceAuthorize.FindUserProfile(userName, "QIS");
        //        result.AuthenticationProvider = AuthenticationProvider;


        //        var jsonString = JsonConvert.SerializeObject(result, Formatting.Indented);


        //        if (result.LoginResult)
        //        {

        //            resturnUser = new CustomModels.WSAuthorizeModel(result);
        //        }

        //    }
        //    else if (AuthenticationProvider == "local")
        //    {

        //        var userUser = UnitOfWork.QIS.UserRepository.GetUserByUserId(result.SysUserId);
        //        var userEmp = UnitOfWork.QIS.UserRepository.GetUserEmployeeByUserId(userUser.EmpCode);

        //        if (userUser != null && userUser.UserID > 0)
        //        {
        //            var user = new { User = userUser, Employee = userEmp };

        //            result.LoginResult = true;
        //            result.SysUserId = userUser.UserID.ToString();
        //            result.SysUserData = JsonConvert.SerializeObject(userUser);

        //            var rolesObject = new AutorizeRoles();
        //            rolesObject.UserId = user.User.UserID.ToString();
        //            var Roles = UnitOfWork.QIS.UserRepository.GetUserRoleByUserId(user.User.UserID);
        //            rolesObject.Roles = JsonConvert.DeserializeObject<List<Role>>(JsonConvert.SerializeObject(Roles));
        //            result.SysUserData = JsonConvert.SerializeObject(user);
        //            result.SysUserRoles = JsonConvert.SerializeObject(rolesObject);

        //            resturnUser = new CustomModels.WSAuthorizeModel(result);

        //        }

        //    }

        //    return resturnUser;

        //}


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using com.apthai.DefectAPI.Model;
using com.apthai.DefectAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using Ionic.Zip;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using com.apthai.DefectAPI.Repositories;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using com.apthai.DefectAPI.Model.DefectAPI;
using Microsoft.Extensions.Configuration;
using com.apthai.DefectAPI.Configuration;
using Microsoft.AspNetCore.StaticFiles;
using com.apthai.DefectAPI.CustomModel;
using com.apthai.CoreApp.Data.Services;
using com.apthai.DefectAPI.HttpRestModel;
using Microsoft.Extensions.Primitives;

namespace com.apthai.DefectAPI.Controllers
{
    public class MasterController : ControllerBase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IMasterRepository _masterRepository;
        //private List<MStorageServer> _QISStorageServer;
        protected AppSettings _appSetting;

        public MasterController(IAuthorizeService authorizeService) {


            _hostingEnvironment  = UtilsProvider.HostingEnvironment;
            _config = UtilsProvider.Config;
            _appSetting = UtilsProvider.AppSetting;
            _unitOfWork = new UnitOfWork(_hostingEnvironment, _config);
            _masterRepository = new MasterRepository(_hostingEnvironment, _config);
            _authorizeService = authorizeService;
        }
        [HttpPost]
        [Route("GetCallType")]
        public async Task<object> GetMasterCallType([FromBody]GetCAllArea data)
        {
            try
            {
                //#region VerifyHeader
                //string ErrorHeader = "";
                //if (!VerifyHeader(out ErrorHeader))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = ErrorHeader ,
                //        valid = false
                //    };
                //}
                //#endregion
                List<calltype> calltypes = _masterRepository.GetCallCallType_Sync();

                return new
                {
                    success = true,
                    data = calltypes
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }
        [HttpPost]
        [Route("GetUnitByProject")]
        public async Task<object> GetMasterUnitByProject([FromBody]GetunitByProjectParam data)
        {
            try
            {
                //#region VerifyHeader
                //string ErrorHeader = "";
                //if (!VerifyHeader(out ErrorHeader))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = ErrorHeader ,
                //        valid = false
                //    };
                //}
                //#endregion
                List<GetUnitByProjectReturnObj> Units = _masterRepository.GetUnitByProduct(data.ProductID);

                return new
                {
                    success = true,
                    data = Units
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }
        [HttpPost]
        [Route("GetCallPoint")]
        public async Task<object> GetMasterCallPoint([FromBody]GetCAllArea data)
        {
            try
            {
                //#region VerifyHeader
                //string ErrorHeader = "";
                //if (!VerifyHeader(out ErrorHeader))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = ErrorHeader ,
                //        valid = false
                //    };
                //}
                //#endregion
                List<point> points = _masterRepository.GetCallPointByProductCat_Sync(data.ProductTypeCate);

                return new
                {
                    success = true,
                    data = points
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("GetCallArea")]
        public async Task<object> GetMasterCallArea([FromBody]GetCAllArea data)
        {
            try
            {
                //#region VerifyHeader
                //string ErrorHeader = "";
                //if (!VerifyHeader(out ErrorHeader))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = "Invalid AccessKey!!. ",
                //        valid = false
                //    };
                //}
                //#endregion
                
                List<callarea> callareas = _masterRepository.GetCallAreaByProductCat_Sync(data.ProductTypeCate);
                List<GetCAllAreaxDescroiption> ReturnObj = new List<GetCAllAreaxDescroiption>();
                for (int i = 0; i < callareas.Count(); i++)
                {
                    GetCAllAreaxDescroiption getCAllAreaxDescroiption = new GetCAllAreaxDescroiption();
                    List<calldescription> calldescriptions = _masterRepository.GetCallDescriptionByCallAreaID_Sync(callareas[i].callarea_id);
                    getCAllAreaxDescroiption.callarea = callareas[i];
                    getCAllAreaxDescroiption.calldescriptions = calldescriptions;
                    ReturnObj.Add(getCAllAreaxDescroiption);
                }
                return new
                {
                    success = true,
                    data = ReturnObj
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }
        [HttpPost]
        [Route("GetCallDescriptionByArea")]
        public async Task<object> GetCallDescriptionByArea([FromBody]GetCAllDescriptionParam data)
        {
            try
            {
                //#region VerifyHeader
                //string ErrorHeader = "";
                //if (!VerifyHeader(out ErrorHeader))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = "Invalid AccessKey!!. ",
                //        valid = false
                //    };
                //}
                //#endregion
                
               List<calldescription> calldescriptions = _masterRepository.GetCallDescriptionByCallAreaID_Sync(data.CallAreaID);
                
                return new
                {
                    success = true,
                    data = calldescriptions
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("GetCallTransactionDefect")]
        public async Task<object> GetCallTransactionDefect([FromBody]callTDefectObj data)
        {
            try
            {
                
                callTDefect callTDefect = _masterRepository.GetCallTDefect_Sync(data.TDefectID);
                List<callTDefectDetail> callTDefectDetails = _masterRepository.GetcallTDefectDetail_Sync(data.TDefectID);
                GetCallTransactionDefectObj ReturnObj = new GetCallTransactionDefectObj();
                ReturnObj.callTDefect = callTDefect;
                ReturnObj.callTDefectDetail = callTDefectDetails;
                
                return new
                {
                    success = true,
                    data = ReturnObj
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("CreateDefect")]
        public async Task<object> CreateDefect([FromBody]callTDefectObj data)
        {
            try
            {
                //bool CanAccess = _authorizeService.AccessKeyAuthentication(data.AccessKey, data.EmpCode);
                //if (CanAccess == false)
                //{
                //    return new
                //    {
                //        success = false,
                //        data = "AccessKey is Invalid!"
                //    };
                //}
                callTDefect callTDefect = _masterRepository.GetCallTDefect_Sync(data.TDefectID);
                List<callTDefectDetail> callTDefectDetails = _masterRepository.GetcallTDefectDetail_Sync(data.TDefectID);
                GetCallTransactionDefectObj ReturnObj = new GetCallTransactionDefectObj();
                ReturnObj.callTDefect = callTDefect;
                ReturnObj.callTDefectDetail = callTDefectDetails;

                return new
                {
                    success = true,
                    data = ReturnObj
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("GetCallTransactionDefectByProject")]
        public async Task<object> GetCallTransactionDefectByProject([FromBody]GetCallTransactionDefectByProject data)
        {
            try
            {

                List<GetCallTDefectByProjectObj> callTDefect = _masterRepository.GetCallTDefectByProject_Sync(data.ProductID);

                return new
                {
                    success = true,
                    data = callTDefect
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<string> GetQISFileStorageUrlAsync(int? StorageServerId, string FilePath)
        //{


        //    string url = "";


        //    if (!StorageServerId.HasValue)
        //        return url;

        //    if (_QISStorageServer == null)
        //        _QISStorageServer = (await _unitOfWork.MasterRepository.GetStorageServer()).ToList();


        //    var storageServer = _QISStorageServer.Where(e => e.StorageServerId == StorageServerId).FirstOrDefault();

        //    if (storageServer != null)
        //    {
        //        url = string.Format("{0}/{1}", storageServer.StorageServerRootUrl.TrimEnd('/'), FilePath.TrimStart('/'));
        //    }
        //    return url;


        //}

        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<string> GetQISResourceStorageUrlAsync(int? StorageServerId, string FilePath)
        //{

        //    string path = "";

        //    if (!StorageServerId.HasValue)
        //        return path;

        //    if (_QISStorageServer == null)
        //        _QISStorageServer = (await _unitOfWork.MasterRepository.GetStorageServer()).ToList();

        //    var storageServer = _QISStorageServer.Where(e => e.StorageServerId == StorageServerId).FirstOrDefault();

        //    if (storageServer != null)
        //    {
        //        if (string.IsNullOrEmpty(FilePath))
        //            path = storageServer.StorageServerRootUrl.TrimEnd('/');
        //        else
        //            path = string.Format("{0}/{1}", storageServer.StorageServerRootUrl.TrimEnd('/'), FilePath.TrimStart('/'));
        //    }
        //    return path;

        //}

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool VerifyHeader(out string ErrorMsg)
        {
            //if (data == null)
            //{
            //    return BadRequest();
            //}

            //string ipaddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            string ipaddress = "5555555";
            StringValues api_key;
            StringValues EmpCode;

            var isValidHeader = false;
            //APIITVendor //VendorData = new APIITVendor();
            if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
            {
                string AccessKey = api_key.First();
                string EmpCodeKey = EmpCode.First();

                if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                {
                    bool CanAccess = _authorizeService.AccessKeyAuthentication(AccessKey, EmpCodeKey);
                    if (CanAccess == true)
                    {
                        ErrorMsg = "Invalid User Authentication!";
                        return true;
                    }
                }
            }
            else
            {
                if (!isValidHeader)
                {
                    //_log.LogDebug(ipaddress + " :: Missing Authorization Header.");
                    ErrorMsg = ipaddress + " :: Missing Authorization Header.";
                    //VendorData = new APIITVendor();
                    return false;
                    //  return BadRequest("Missing Authorization Header.");
                }
            }
            //VendorData = new APIITVendor();
            ErrorMsg = "SomeThing Wrong with Header Contact Developer ASAP";
            return false;
        }
    }


}

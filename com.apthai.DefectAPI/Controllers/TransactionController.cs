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
    public class TransactionController : ControllerBase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IMasterRepository _masterRepository;
        private readonly ITransactionRepository _transactionRepository;
        //private List<MStorageServer> _QISStorageServer;
        protected AppSettings _appSetting;

        public TransactionController(IAuthorizeService authorizeService , ITransactionRepository transactionRepository) {


            _hostingEnvironment  = UtilsProvider.HostingEnvironment;
            _config = UtilsProvider.Config;
            _appSetting = UtilsProvider.AppSetting;
            _unitOfWork = new UnitOfWork(_hostingEnvironment, _config);
            _masterRepository = new MasterRepository(_hostingEnvironment, _config);
            _authorizeService = authorizeService;
            _transactionRepository = transactionRepository;
        }
        
        //[HttpPost]
        //[Route("GetCallTransactionDefect")]
        //public async Task<object> GetCallTransactionDefect([FromBody]callTDefectObj data)
        //{
        //    try
        //    {
        //        string ErrorMsg = "";
        //        if (!VerifyHeader(out ErrorMsg))
        //        {
        //            return new
        //            {
        //                success = false,
        //                data = ErrorMsg
        //            };
        //        }
        //        callTDefect callTDefect = _masterRepository.GetCallTDefect_Sync(data.TDefectID);
        //        List<callTDefectDetail> callTDefectDetails = _masterRepository.GetcallTDefectDetail_Sync(data.TDefectID);
        //        GetCallTransactionDefectObj ReturnObj = new GetCallTransactionDefectObj();
        //        ReturnObj.callTDefect = callTDefect;
        //        ReturnObj.callTDefectDetail = callTDefectDetails;
                
        //        return new
        //        {
        //            success = true,
        //            data = ReturnObj
        //        };

        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, "Internal server error");
        //    }

        //}

        [HttpPost]
        [Route("CreateDefectTransaction")]
        public async Task<object> CreateDefectTransaction([FromBody]CreateDefectObj data)
        {
            try
            {
                string ErrorMsg = "";
                if (!VerifyHeader(out ErrorMsg))
                {
                    return new
                    {
                        success = false,
                        data = ErrorMsg
                    };
                }
                int taskNo = 1;
                if (data.TDefectId != 0 )
                {
                    string DefectDocNo = "DefectDetail-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "");
                    callTDefectDetail tDefectDetail = new callTDefectDetail();
                    tDefectDetail.RowState = "Original";
                    tDefectDetail.RowActive = true;
                    tDefectDetail.Client_Id = "DefectDetail-"+ data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "") + Guid.NewGuid();
                    tDefectDetail.Client_SyncDate = DateTime.Now;
                    tDefectDetail.TDefectId = data.TDefectId;
                    tDefectDetail.TDefectDocNo = DefectDocNo;
                    tDefectDetail.ProductId = data.ProductId;
                    tDefectDetail.ItemId = data.ItemId;
                    tDefectDetail.TDefectDetailStatus = "001";
                    tDefectDetail.TDefectDetailSubStatus = "";
                    tDefectDetail.CallTypeId = data.CallTypeID;
                    tDefectDetail.CallAreaId = data.CallArea;
                    tDefectDetail.CallDescId = data.CallDescId;
                    tDefectDetail.CallPointId = data.CallSubPointId;
                    tDefectDetail.CallSubPointId = data.CallSubPointId;
                    tDefectDetail.DeviceId = data.DeviceId;
                    tDefectDetail.Tag = null;
                    tDefectDetail.CreateUserId = data.UserID;
                    tDefectDetail.UpdateDate = DateTime.Now;
                    tDefectDetail.FloorPlan_ImageId = null;
                    tDefectDetail.FloorPlan_X = 0;
                    tDefectDetail.FloorPlan_Y = 0;
                    tDefectDetail.TaskNo = taskNo;
                    tDefectDetail.TaskMarkName = "DummyData";
                    tDefectDetail.FloorPlanSet = data.FloorPlanSet;
                    tDefectDetail.CustRoundAuditNo = 1;
                    tDefectDetail.CustRoundAuditDate = DateTime.Now;
                    tDefectDetail.CustRoundAuditDueCloseDate = DateTime.Now.AddDays(20);
                    tDefectDetail.IsServerLockRow = false;
                    tDefectDetail.TaskOpenDate = DateTime.Now;
                    tDefectDetail.TaskProcessDate = null;
                    tDefectDetail.TaskActualFinishDate = null;
                    tDefectDetail.TaskActualCloseDate = null;

                    bool inserttdefectdetail = _transactionRepository.InsertTdefectDetail(tDefectDetail);
                    return new
                    {
                        success = true,
                        data = tDefectDetail
                    };
                }
                else
                {
                    callTDefect CreateDefect = new callTDefect();
                    CreateDefect.RowState = "Original";
                    CreateDefect.RowActive = true;
                    CreateDefect.Client_Id = "Defect-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "") + Guid.NewGuid();
                    CreateDefect.Client_SyncDate = DateTime.Now;
                    CreateDefect.TDefectDocNo = "Defect-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "/" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "");
                    CreateDefect.TDefectStatus = "001"; // หน้าจะเท่ากับ Open
                    CreateDefect.TDefectSubStatus = null;
                    CreateDefect.ProductId = data.ProductId;
                    CreateDefect.ItemId = data.ItemId;
                    CreateDefect.DeviceId = data.DeviceId;
                    CreateDefect.CreateUserId = data.EmpCode;
                    CreateDefect.UpdateUserId = null;
                    CreateDefect.CustRoundAuditNo_Rn = 1;
                    CreateDefect.CustRoundAuditDate_Last = DateTime.Now;
                    CreateDefect.CustRoundAudit_JsonLog = null;
                    CreateDefect.CreateDate = DateTime.Now;
                    CreateDefect.UpdateDate = null;
                    CreateDefect.Desciption = data.Description;
                    CreateDefect.DocOpenDate = DateTime.Now;
                    CreateDefect.DocDueCloseDate = DateTime.Now.AddDays(14);
                    CreateDefect.MechanicId = null;
                    CreateDefect.MechanicName = null;
                    CreateDefect.SellerId = null;
                    CreateDefect.SallerName = null;
                    CreateDefect.DocReceiveUnitDate = DateTime.Now;
                    CreateDefect.DocDueTransferDate = DateTime.Now;
                    CreateDefect.ContactID = null;
                    return new
                    {
                        success = true,
                        data = CreateDefect
                    };
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("CreateDefectListTransaction")]
        public async Task<object> CreateDefectListTransaction([FromBody]CreateDefectListObj data)
        {
            try
            {
                string ErrorMsg = "";
                if (!VerifyHeader(out ErrorMsg))
                {
                    return new
                    {
                        success = false,
                        data = ErrorMsg
                    };
                }
                bool insertTDefect = _transactionRepository.InsertTdefect(data.callTDefect);
                bool insertDefectDetail = _transactionRepository.InsertTdefectDetailList(data.callTDefectDetails);
                return new
                {
                    success = true,
                    data = ErrorMsg,
                    Message = "Create Defect and DefectDetail Complete!"
                };
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }

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
                        ErrorMsg = "";
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

    }


}

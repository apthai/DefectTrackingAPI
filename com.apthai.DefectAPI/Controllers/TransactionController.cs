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
using System.Globalization;
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
using Swashbuckle.AspNetCore.Annotations;
using static com.apthai.DefectAPI.CustomModel.RequestReportModel;
using System.Text;
using com.apthai.DefectAPI.Services;
using Microsoft.AspNetCore.Http.Internal;
using Hangfire;
using System.Diagnostics;

namespace com.apthai.DefectAPI.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : BaseController
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IMasterRepository _masterRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISyncRepository _syncRepository;
        MinioServices minio;
        //private List<MStorageServer> _QISStorageServer;
        protected AppSettings _appSetting;
        public TransactionController(IAuthorizeService authorizeService, ITransactionRepository transactionRepository, ISyncRepository syncRepository)
        {


            _hostingEnvironment = UtilsProvider.HostingEnvironment;
            _config = UtilsProvider.Config;
            //_appSetting = UtilsProvider.AppSetting;
            _unitOfWork = new UnitOfWork(_hostingEnvironment, _config);
            _masterRepository = new MasterRepository(_hostingEnvironment, _config);
            _authorizeService = authorizeService;
            _transactionRepository = transactionRepository;
            _syncRepository = syncRepository;
            minio = new MinioServices();
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
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "สร้างรายการ DefectDetail ทีละ 1 รายการ",
        Description = "สร้างรายการ DefectDetail ใน Unit นั้นๆ โดยถ้าสร้างครั้งแรกจะ Auto Gen Header ให้")]
        public async Task<object> CreateDefectTransaction([FromForm] CreateDefectObj data)
        {
            try
            {
                string ErrorMsg = "";
                //if (!VerifyHeader(out ErrorMsg))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = ErrorMsg
                //    };
                //}
                int taskNo = 1;
                if (data.Files == null)
                {
                    return new
                    {
                        success = false,
                        data = "Create DefectTransactino Require Before Picture!"
                    };

                }

                if (data.TDefectId != 0)
                {
                    bool IncreaseRound = false;
                    callTDefectDetail LatestdefectDetail = _masterRepository.GetcallTDefectDetailByTDefectID_Sync(data.TDefectId);
                    DateTime Today = DateTime.Now;
                    DateTime LatestTxnDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    callTDefect callTDefect = _masterRepository.GetCallTDefectByTDefectId_Sync(data.TDefectId.ToString());
                    if (LatestdefectDetail != null)
                    {
                        LatestTxnDate = Convert.ToDateTime(LatestdefectDetail.CreateDate.Value.ToShortDateString());
                    }


                    string DefectDocNo = "DefectDetail-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "");
                    callTDefectDetail tDefectDetail = new callTDefectDetail();
                    tDefectDetail.RowState = "Original";
                    tDefectDetail.RowActive = true;
                    tDefectDetail.Client_Id = "DefectDetail-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "") + Guid.NewGuid();
                    tDefectDetail.Client_SyncDate = DateTime.Now;
                    tDefectDetail.TDefectId = data.TDefectId;
                    tDefectDetail.TDefectDocNo = callTDefect.TDefectDocNo;
                    tDefectDetail.ProductId = data.ProductId;
                    tDefectDetail.ItemId = data.ItemId;
                    tDefectDetail.TDefectDetailStatus = "001";
                    tDefectDetail.TDefectDetailSubStatus = "";
                    tDefectDetail.CallTypeId = data.CallTypeID;
                    tDefectDetail.CallAreaId = data.CallArea;
                    tDefectDetail.CallDescId = data.CallDescId;
                    tDefectDetail.CallPointId = data.PointId;
                    tDefectDetail.CallSubPointId = data.CallSubPointId;
                    tDefectDetail.DeviceId = data.DeviceId;
                    tDefectDetail.Tag = null;
                    tDefectDetail.CreateUserId = data.UserID;
                    tDefectDetail.CreateDate = DateTime.Now;
                    tDefectDetail.UpdateDate = DateTime.Now;
                    tDefectDetail.FloorPlan_ImageId = null;
                    tDefectDetail.FloorPlan_X = 0;
                    tDefectDetail.FloorPlan_Y = 0;
                    tDefectDetail.TaskNo = taskNo;
                    //tDefectDetail.TaskMarkName = "DummyData";
                    tDefectDetail.FloorPlanSet = data.DefectType == "V" ? "1" : data.FloorPlanSet;

                    if (LatestTxnDate.Date < Today.Date)
                    {
                        tDefectDetail.CustRoundAuditNo = LatestdefectDetail.CustRoundAuditNo + 1;
                        callTDefect.CustRoundAuditDate_Last = LatestTxnDate.Date;
                        callTDefect.CustRoundAuditNo_Rn = LatestdefectDetail.CustRoundAuditNo + 1; 
                        IncreaseRound = true;
                        bool IncreaseRoundCalltDefectHeader = _transactionRepository.UpdateTdefect(callTDefect);
                    }
                    else
                        tDefectDetail.CustRoundAuditNo = 1;

                    tDefectDetail.CustRoundAuditDate = null;//DateTime.Now;
                    tDefectDetail.CustRoundAuditDueCloseDate = null;//DateTime.Now.AddDays(20);
                    tDefectDetail.IsServerLockRow = false;
                    tDefectDetail.TaskOpenDate = DateTime.Now;
                    tDefectDetail.TaskProcessDate = null;
                    tDefectDetail.TaskActualFinishDate = null;
                    tDefectDetail.TaskActualCloseDate = null;
                    tDefectDetail.TDefectDetailDesc = data.TDefectDetailDesc;

                    long inserttdefectdetail = _transactionRepository.InsertTdefectDetail(tDefectDetail);
                    tDefectDetail.TDefectDetailId = Convert.ToInt32(inserttdefectdetail);
                    TDefectDetailWithStatus Returnobj = new TDefectDetailWithStatus();
                    if (inserttdefectdetail > 0)
                    {
                        string a = JsonConvert.SerializeObject(tDefectDetail);
                        Returnobj = JsonConvert.DeserializeObject<TDefectDetailWithStatus>(a);
                        if (Returnobj.TDefectDetailStatus == "001")
                        {
                            Returnobj.StatusShow = "Open";
                        }
                        else if (Returnobj.TDefectDetailStatus == "003")
                        {
                            Returnobj.StatusShow = "Finish";
                        }
                        else if (Returnobj.TDefectDetailStatus == "001")
                        {
                            Returnobj.StatusShow = "Close";
                        }
                    }
                    if (inserttdefectdetail != 0)
                    {
                        int SuccessUploadCount = 0;
                        int count = 0;

                        callResource callResourceDate = new callResource();
                        if (data.Files != null)
                        {

                            var path = $"{data.ProductId}/{data.ItemId}/Before";
                            var fileName = data.Files.FileName;
                            var resultMinio = await minio.UploadFile(data.Files, path, fileName);
                            long size = data.Files.Length;
                            var message = "";

                            callResourceDate.FilePath = $"{path}/{fileName}";
                            callResourceDate.FileLength = size;
                            callResourceDate.CreateDate = DateTime.Now;
                            callResourceDate.RowState = "Original";
                            callResourceDate.ResourceType = 3;
                            callResourceDate.ResourceTagCode = "BF-RP";
                            callResourceDate.ResourceTagSubCode = "1";
                            callResourceDate.ResourceGroupSet = null;
                            callResourceDate.ResourceGroupOrder = 0;
                            callResourceDate.StorageServerId = 1400;
                            callResourceDate.TDefectDetailId = Convert.ToInt32(inserttdefectdetail);
                            callResourceDate.ProjectNo = data.ProductId;
                            callResourceDate.SerialNo = data.ItemId;
                            callResourceDate.Active = true;
                            callResourceDate.RowState = "AddNew";
                            callResourceDate.FullFilePath = resultMinio.Url;
                            callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6);
                            bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);
                            if (LatestTxnDate.Date < Today.Date)
                            {
                                List<callResource> callResources = _masterRepository.GetCallResourceAllSignatureByTdefect(data.TDefectId);
                                for (int i = 0; i < callResources.Count(); i++)
                                {
                                    callResources[i].Active = false;
                                }
                                var InactiveCallResource = _transactionRepository.UpdateCallResource(callResources);
                            }
                        }
                        else
                        {
                            _transactionRepository.UpdateInActiveSignature(data.TDefectId);
                            return new
                            {
                                success = true,
                                data = tDefectDetail,
                                message = string.Format("Create Defect Detail Success!")
                            };
                        }

                    }
                    _transactionRepository.UpdateInActiveSignature(data.TDefectId);
                    if (IncreaseRound)
                    {
                        callTDefectCustRoundAuditLog auditLog = new callTDefectCustRoundAuditLog();
                        auditLog.TDefectId = data.TDefectId;
                        //int Round = tDefectDetail.CustRoundAuditNo ?? 0 ;
                        auditLog.RoundAuditNo = tDefectDetail.CustRoundAuditNo ?? 0;
                        auditLog.RoundAuditDate = DateTime.Now;
                        long InsertAuditLog = _transactionRepository.InsertCustRoundAuditLog(auditLog);
                    }
                    return new
                    {
                        success = true,
                        data = Returnobj
                    };
                }
                else
                {
                    ViewUnitCustomer viewUnitCustomer = _masterRepository.GetViewUnitCustomer(data.ItemId, data.ProductId);
                    callTDefect CreateDefect = new callTDefect();
                    CreateDefect.RowState = "Original";
                    CreateDefect.RowActive = true;
                    CreateDefect.Client_Id = "Defect-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "") + Guid.NewGuid();
                    CreateDefect.Client_SyncDate = DateTime.Now;
                    CreateDefect.TDefectDocNo = "Defect-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "/" +
                                            DateTime.Now.ToString("yyyyMMdd-HHmm").Replace(" ", "");
                    CreateDefect.TDefectStatus = "001"; // หน้าจะเท่ากับ Open
                    CreateDefect.TDefectSubStatus = null;
                    CreateDefect.ProductId = data.ProductId;
                    CreateDefect.ItemId = data.ItemId;
                    CreateDefect.DeviceId = data.DeviceId;
                    CreateDefect.CreateUserId = data.UserID;
                    CreateDefect.UpdateUserId = null;
                    CreateDefect.CustRoundAuditNo_Rn = 1;
                    CreateDefect.CustRoundAuditDate_Last = DateTime.Now;
                    CreateDefect.CustRoundAudit_JsonLog = null;
                    CreateDefect.CreateDate = DateTime.Now;
                    CreateDefect.UpdateDate = null;
                    CreateDefect.Desciption = data.Description;
                    CreateDefect.DocOpenDate = DateTime.Now;
                    CreateDefect.DocDueCloseDate = null; //DateTime.Now.AddDays(14);
                    CreateDefect.MechanicId = null;
                    CreateDefect.MechanicName = null;
                    CreateDefect.SellerId = null;
                    CreateDefect.SallerName = null;
                    CreateDefect.DocReceiveUnitDate = null;
                    CreateDefect.DocDueTransferDate = null;
                    CreateDefect.ContactID = null;
                    if (viewUnitCustomer != null)
                    {
                        CreateDefect.ContactName = viewUnitCustomer.FirstName + "  " + viewUnitCustomer.LastName;
                    }
                    else
                    {
                        CreateDefect.ContactName = "";
                    }
                    CreateDefect.Desciption = data.Description;
                    CreateDefect.DocIsActive = true;
                    CreateDefect.DocIsExternalAudit = false;
                    CreateDefect.DocIsReqUnitReceiveAttachFile = false;
                    long DefectID = 0;
                    bool InsertData = _transactionRepository.InsertTdefectDetail(CreateDefect, ref DefectID);
                    CreateDefect.TDefectId = Convert.ToInt32(DefectID);
                    callTDefectWithStatus Returnobj = new callTDefectWithStatus();
                    if (InsertData == true)
                    {
                        string a = JsonConvert.SerializeObject(CreateDefect);
                        Returnobj = JsonConvert.DeserializeObject<callTDefectWithStatus>(a);
                        if (Returnobj.TDefectStatus == "001")
                        {
                            Returnobj.StatusShow = "Open";
                        }
                        else if (Returnobj.TDefectStatus == "003")
                        {
                            Returnobj.StatusShow = "Finish";
                        }
                        else if (Returnobj.TDefectStatus == "001")
                        {
                            Returnobj.StatusShow = "Close";
                        }
                    }
                    // --------------------------------------------------------------------
                    string DefectDocNo = "DefectDetail-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "");
                    callTDefectDetail tDefectDetail = new callTDefectDetail();
                    tDefectDetail.RowState = "Original";
                    tDefectDetail.RowActive = true;
                    tDefectDetail.Client_Id = "DefectDetail-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "") + Guid.NewGuid();
                    tDefectDetail.Client_SyncDate = DateTime.Now;
                    tDefectDetail.TDefectId = CreateDefect.TDefectId;
                    tDefectDetail.TDefectDocNo = CreateDefect.TDefectDocNo;
                    tDefectDetail.ProductId = data.ProductId;
                    tDefectDetail.ItemId = data.ItemId;
                    tDefectDetail.TDefectDetailStatus = "001";
                    tDefectDetail.TDefectDetailSubStatus = "";
                    tDefectDetail.CallTypeId = data.CallTypeID;
                    tDefectDetail.CallAreaId = data.CallArea;
                    tDefectDetail.CallDescId = data.CallDescId;
                    tDefectDetail.CallPointId = data.PointId;
                    tDefectDetail.CallSubPointId = data.CallSubPointId;
                    tDefectDetail.DeviceId = data.DeviceId;
                    tDefectDetail.Tag = null;
                    tDefectDetail.CreateUserId = data.UserID;
                    tDefectDetail.UpdateDate = DateTime.Now;
                    tDefectDetail.FloorPlan_ImageId = null;
                    tDefectDetail.FloorPlan_X = 0;
                    tDefectDetail.FloorPlan_Y = 0;
                    tDefectDetail.TaskNo = taskNo;
                    tDefectDetail.CreateDate = DateTime.Now;
                    tDefectDetail.TaskMarkName = "DummyData";
                    tDefectDetail.FloorPlanSet = data.FloorPlanSet;
                    tDefectDetail.CustRoundAuditNo = 1;
                    tDefectDetail.CustRoundAuditDate = null;//DateTime.Now;
                    tDefectDetail.CustRoundAuditDueCloseDate = null;//DateTime.Now.AddDays(20);
                    tDefectDetail.IsServerLockRow = false;
                    tDefectDetail.TaskOpenDate = DateTime.Now;
                    tDefectDetail.TaskProcessDate = null;
                    tDefectDetail.TaskActualFinishDate = null;
                    tDefectDetail.TaskActualCloseDate = null;
                    tDefectDetail.TDefectDetailDesc = data.TDefectDetailDesc;

                    long inserttdefectdetail = _transactionRepository.InsertTdefectDetail(tDefectDetail);

                    if (inserttdefectdetail != 0)
                    {
                        int SuccessUploadCount = 0;
                        int count = 0;

                        callResource callResourceDate = new callResource();
                        if (data.Files != null)
                        {

                            var path = $"{data.ProductId}/{data.ItemId}/Before";
                            var fileName = data.Files.FileName;
                            var resultMinio = await minio.UploadFile(data.Files, path, fileName);
                            long size = data.Files.Length;
                            var message = "";

                            callResourceDate.FilePath = $"{path}/{fileName}";
                            callResourceDate.FileLength = size;
                            callResourceDate.CreateDate = DateTime.Now;
                            callResourceDate.RowState = "Original";
                            callResourceDate.ResourceType = 3;
                            callResourceDate.ResourceTagCode = "BF-RP";
                            callResourceDate.StorageServerId = 1400;
                            callResourceDate.ResourceTagSubCode = "1";
                            callResourceDate.ResourceGroupSet = null;
                            callResourceDate.ResourceGroupOrder = 0;
                            callResourceDate.TDefectDetailId = Convert.ToInt32(inserttdefectdetail);
                            callResourceDate.ProjectNo = data.ProductId;
                            callResourceDate.SerialNo = data.ItemId;
                            callResourceDate.Active = true;
                            callResourceDate.RowState = "AddNew";
                            callResourceDate.FullFilePath = resultMinio.Url;
                            callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6);
                            bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);
                        }
                        else
                        {
                            _transactionRepository.UpdateInActiveSignature(data.TDefectId);
                            return new
                            {
                                success = true,
                                data = tDefectDetail,
                                message = string.Format("Create Defect Detail Success!")
                            };
                        }
                    }
                    _transactionRepository.UpdateInActiveSignature(data.TDefectId);

                    callTDefectCustRoundAuditLog auditLog = new callTDefectCustRoundAuditLog();
                    auditLog.TDefectId = Convert.ToInt32(DefectID);
                    auditLog.RoundAuditDate = DateTime.Now;
                    auditLog.RoundAuditNo = 1;

                    long insertAuditLog = _transactionRepository.InsertCustRoundAuditLog(auditLog);
                    return new
                    {
                        success = true,
                        data = Returnobj
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("UpdateDefectDetail")]
        [SwaggerOperation(Summary = "Update DefectDetail ซ้อมงานเสจแล้ว ",
Description = "Update DefectDetail ซ้อมงานเสร็จแล้ว ")]
        public async Task<object> UpdateDefectDetailStatusFinish([FromBody] UpdateDefectObj data)
        {
            try
            {

                callTDefectDetail callTDefectDetail = _masterRepository.GetcallTDefectDetailByDetailID_Sync(data.TDefectDetailId);
                callTDefect callTDefect = _masterRepository.GetCallTDefectByTDefectId_Sync(callTDefectDetail.TDefectId.ToString());
                // --------------------------------------------------------------------
                callTDefectDetail.RowState = "Original";
                callTDefectDetail.RowActive = true;
                callTDefectDetail.Client_SyncDate = DateTime.Now;
                callTDefectDetail.ProductId = data.ProductId;
                callTDefectDetail.ItemId = data.ItemId;
                callTDefectDetail.TDefectDetailSubStatus = "";
                callTDefectDetail.CallTypeId = data.CallTypeID; // 6
                callTDefectDetail.CallAreaId = data.CallArea; // 116
                callTDefectDetail.CallDescId = data.CallDescId;
                callTDefectDetail.CallPointId = data.PointID; // 16
                callTDefectDetail.CallSubPointId = data.Cate; // 17
                callTDefectDetail.DeviceId = data.DeviceId;
                callTDefectDetail.Tag = null;
                callTDefectDetail.CreateUserId = data.UserID;
                callTDefectDetail.UpdateDate = DateTime.Now;
                callTDefectDetail.FloorPlan_ImageId = null;
                callTDefectDetail.FloorPlan_X = 0;
                callTDefectDetail.FloorPlan_Y = 0;
                callTDefectDetail.TaskMarkName = "DummyData";
                callTDefectDetail.FloorPlanSet = data.FloorPlanSet;
                callTDefectDetail.CustRoundAuditNo = 1;
                callTDefectDetail.CustRoundAuditDate = DateTime.Now;
                callTDefectDetail.CustRoundAuditDueCloseDate = DateTime.Now.AddDays(20);
                callTDefectDetail.IsServerLockRow = false;
                callTDefectDetail.TaskOpenDate = DateTime.Now;
                callTDefectDetail.TaskProcessDate = null;
                callTDefectDetail.TaskActualFinishDate = null;
                callTDefectDetail.TaskActualCloseDate = null;
                callTDefectDetail.TDefectId = data.TDefectId;
                callTDefectDetail.TDefectDetailDesc = data.TDefectDetailDesc;
                var inserttdefectdetail = _transactionRepository.UpdateTdefectDetail(callTDefectDetail);
                callTDefect.UpdateDate = DateTime.Now;
                callTDefect.UpdateUserId = data.UserID;
                var UpdateHeader = _transactionRepository.UpdateTdefect(callTDefect);

                return new
                {
                    success = true,
                    data = callTDefectDetail
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpPost]
        [Route("UpdateDefectDetailStatusFinish")]
        [SwaggerOperation(Summary = "Update DefectDetail ซ้อมงานเสจแล้ว ",
        Description = "Update DefectDetail ซ้อมงานเสร็จแล้ว ")]
        public async Task<object> UpdateDefectDetailStatusFinish([FromBody] UpdateDefectDetailID data)
        {
            try
            {

                callTDefectDetail callTDefectDetail = _masterRepository.GetcallTDefectDetailByDetailID_Sync(data.TDefectDetailID);
                // --------------------------------------------------------------------
                callTDefectDetail.TDefectDetailStatus = "003";
                callTDefectDetail.TaskActualFinishDate = DateTime.Now;
                var inserttdefectdetail = _transactionRepository.UpdateTdefectDetail(callTDefectDetail);


                return new
                {
                    success = true,
                    data = callTDefectDetail
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("UpdateDefectDetailStatusFinishList")]
        [SwaggerOperation(Summary = "Update DefectDetail ซ้อมงานเสจแล้ว ",
      Description = "Update DefectDetail ซ้อมงานเสร็จแล้ว ")]
        public async Task<object> UpdateDefectDetailStatusFinishList([FromBody] UpdateDefectDetailIDList data)
        {
            try
            {
                List<callTDefectDetail> UpdateLists = new List<callTDefectDetail>();
                for (int i = 0; i < data.TDefectDetailIDList.Count(); i++)
                {
                    //if (Listed == "")
                    //{
                    //    Listed = Listed + "'" + data.TDefectDetailIDList[i].TDefectDetailID + "'";
                    //}
                    //else
                    //{
                    //    Listed = Listed + ",'" + data.TDefectDetailIDList[i].TDefectDetailID + "'";
                    //}
                    callTDefectDetail callTDefectDetail = new callTDefectDetail();
                    callTDefectDetail = _masterRepository.GetcallTDefectDetailByDetailID_Sync(data.TDefectDetailIDList[i].TDefectDetailID);
                    // --------------------------------------------------------------------
                    callTDefectDetail.TDefectDetailStatus = "003";
                    callTDefectDetail.TaskActualFinishDate = DateTime.Now;
                    UpdateLists.Add(callTDefectDetail);
                }
                var inserttdefectdetail = _transactionRepository.UpdateTdefectDetailList(UpdateLists);

                return new
                {
                    success = true,
                    data = UpdateLists
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("UpdateDefectHeaderDueTranferDateDate")]
        [SwaggerOperation(Summary = "Update วันที่ลูกค้าจะเข้ามาตรวจบ้านบน Defect Header ",
        Description = "Update วันที่ลูกค้าจะเข้ามาตรวจบ้านบน Defect Header ตอนเลิอกปฎิทิน โดยจะเอา DocDueCloseDate ปัจจุบันไป ใสใน AuditCustRoundAuditDate_last ")]
        public async Task<object> UpdateDefectHeaderDueTranferDateDate([FromBody] UpdateDefectHeaderTranferDate data)
        {
            try
            {

                callTDefect callTDefect = _masterRepository.GetCallTDefect_Sync(data.TDefectID);

                callTDefect.DocDueTransferDate = data.DocDueTranferDate;
                // --------------------------------------------------------------------


                var inserttdefectdetail = _transactionRepository.UpdateTdefect(callTDefect);


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

        [HttpPost]
        [Route("UpdateDefectHeaderDueCloseDateFirstTime")]
        [SwaggerOperation(Summary = "Update วันที่ลูกค้าจะเข้ามาตรวจบ้านบน Defect Header ",
        Description = "Update วันที่ลูกค้าจะเข้ามาตรวจบ้านบน Defect Header ตอนเลิอกปฎิทิน ครั้งแรก คือ พึ่งมีรายการซ้อม ")]
        public async Task<object> UpdateDefectHeaderDueCloseDateFirstTime([FromBody] UpdateDefectHeader data)
        {
            try
            {

                callTDefect callTDefect = _masterRepository.GetCallTDefect_Sync(data.TDefectID);
                callTDefect.DocDueCloseDate = data.DocDueCloseDate;

                List<callTDefectDetail> callTDefectDetails = _masterRepository.GetcallTDefectDetailByTDefectIDList_Sync(callTDefect.TDefectId);
                for (int i = 0; i < callTDefectDetails.Count(); i++)
                {
                    callTDefectDetails[i].CustRoundAuditDueCloseDate = data.DocDueCloseDate;
                }
                bool update = _transactionRepository.UpdateTdefectDetailList(callTDefectDetails);
                // --------------------------------------------------------------------


                var inserttdefectdetail = _transactionRepository.UpdateTdefect(callTDefect);


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


        [HttpPost]
        [Route("UpdateDefectHeaderCustomerNotSign")]
        [SwaggerOperation(Summary = "Update วันที่ลูกค้าจะเข้ามาตรวจบ้านบน Defect Header ",
Description = "Update วันที่ลูกค้าจะเข้ามาตรวจบ้านบน Defect Header ตอนเลิอกปฎิทิน โดยจะเอา DocDueCloseDate ปัจจุบันไป ใสใน AuditCustRoundAuditDate_last ")]
        public async Task<object> UpdateDefectHeaderCustomerNotSign([FromBody] UpdateDefectHeaderCustomerNotSign data)
        {
            try
            {

                callTDefect callTDefect = _masterRepository.GetCallTDefect_Sync(data.TDefectID);
                callTDefect.DocIsReqUnitReceiveAttachFile = true;
                callTDefect.UpdateDate = DateTime.Now;
                callTDefect.DocReceiveUnitDate = DateTime.Now;
                // --------------------------------------------------------------------


                var inserttdefectdetail = _transactionRepository.UpdateTdefect(callTDefect);


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

        [HttpPost]
        [Route("DeleteeDefectDetail")]
        [SwaggerOperation(Summary = "Update วันที่ลูกค้าจะเข้ามาตรวจบ้านบน Defect Header ",
        Description = "Update วันที่ลูกค้าจะเข้ามาตรวจบ้านบน Defect Header ตอนเลิอกปฎิทิน ครั้งแรก คือ พึ่งมีรายการซ้อม ")]
        public async Task<object> DeleteeDefectDetail([FromBody] UpdateDefectDetailID data)
        {
            try
            {

                callTDefectDetail callTDefect = _masterRepository.GetcallTDefectDetailByDetailID_Sync(data.TDefectDetailID);
                callTDefect.RowActive = false;
                // --------------------------------------------------------------------


                var inserttdefectdetail = _transactionRepository.UpdateTdefectDetail(callTDefect);


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

        [HttpPost]
        [Route("UpdateDefectHeaderDueCloseDate")]
        [SwaggerOperation(Summary = "Update วันที่ลูกค้าจะเข้ามาตรวจบ้านบน Defect Header ",
        Description = "Update วันที่ลูกค้าจะเข้ามาตรวจบ้านบน Defect Header ตอนเลิอกปฎิทิน โดยจะเอา DocDueCloseDate ปัจจุบันไป ใสใน AuditCustRoundAuditDate_last ")]
        public async Task<object> UpdateDefectHeaderDueCloseDate([FromBody] UpdateDefectHeader data)
        {
            try
            {

                callTDefect callTDefect = _masterRepository.GetCallTDefect_Sync(data.TDefectID);
                DateTime? LastCloseDate = callTDefect.DocDueCloseDate;
                callTDefect.CustRoundAuditDate_Last = LastCloseDate;
                callTDefect.DocDueCloseDate = data.DocDueCloseDate;
                List<callTDefectDetail> callTDefectDetails = _masterRepository.GetcallTDefectDetailByTDefectIDList_Sync(callTDefect.TDefectId);
                for (int i = 0; i < callTDefectDetails.Count(); i++)
                {
                    callTDefectDetails[i].CustRoundAuditDueCloseDate = data.DocDueCloseDate;
                }
                // --------------------------------------------------------------------
                bool update = _transactionRepository.UpdateTdefectDetailList(callTDefectDetails);
                var inserttdefectdetail = _transactionRepository.UpdateTdefect(callTDefect);


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

        [HttpPost]
        [Route("UpdateDefectDetailStatusClose")]
        [SwaggerOperation(Summary = "Update DefectDetail ปิดงาน ตรวจผ่านแล้ว ",
        Description = "Update DefectDetail ปิดงาน ตรวจผ่านแล้ว ")]
        public async Task<object> UpdateDefectDetailStatusClose([FromBody] UpdateDefectDetailID data)
        {
            try
            {

                callTDefectDetail callTDefectDetail = _masterRepository.GetcallTDefectDetailByDetailID_Sync(data.TDefectDetailID);
                // --------------------------------------------------------------------
                callTDefectDetail.TDefectDetailStatus = "005";

                var inserttdefectdetail = _transactionRepository.UpdateTdefectDetail(callTDefectDetail);


                return new
                {
                    success = true,
                    data = callTDefectDetail
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("UpdateDefectDetailStatusCloseList")]
        [SwaggerOperation(Summary = "Update DefectDetail ซ้อมงานเสจแล้ว ",
Description = "Update DefectDetail ซ้อมงานเสร็จแล้ว ")]
        public async Task<object> UpdateDefectDetailStatusCloseList([FromBody] UpdateDefectDetailIDList data)
        {
            try
            {

                List<callTDefectDetail> UpdateLists = new List<callTDefectDetail>();
                for (int i = 0; i < data.TDefectDetailIDList.Count(); i++)
                {
                    //if (Listed == "")
                    //{
                    //    Listed = Listed + "'" + data.TDefectDetailIDList[i].TDefectDetailID + "'";
                    //}
                    //else
                    //{
                    //    Listed = Listed + ",'" + data.TDefectDetailIDList[i].TDefectDetailID + "'";
                    //}
                    callTDefectDetail callTDefectDetail = new callTDefectDetail();
                    callTDefectDetail = _masterRepository.GetcallTDefectDetailByDetailID_Sync(data.TDefectDetailIDList[i].TDefectDetailID);
                    // --------------------------------------------------------------------
                    callTDefectDetail.TDefectDetailStatus = "005";
                    callTDefectDetail.TaskActualCloseDate = DateTime.Now;
                    UpdateLists.Add(callTDefectDetail);
                }
                for (int i = 0; i < UpdateLists.Count(); i++)
                {
                    callTDefect callTDefect = _masterRepository.GetCallTDefect_Sync(UpdateLists[i].TDefectId);
                    callTDefect.UpdateDate = DateTime.Now;
                    bool UpdateTdefect = _transactionRepository.UpdateTdefect(callTDefect);
                }
                var inserttdefectdetail = _transactionRepository.UpdateTdefectDetailList(UpdateLists);

                return new
                {
                    success = true,
                    data = UpdateLists
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("CreateDefectListTransaction")]
        [SwaggerOperation(Summary = "สร้างรายการ DefectDetail ทีละ หลายรายการ",
        Description = "สร้างรายการ DefectDetail ใน Unit นั้นๆ ")]
        public async Task<object> CreateDefectListTransaction([FromBody] CreateDefectListObj data)
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
                data.callTDefect.UpdateDate = DateTime.Now;
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

        [HttpPost]
        [Route("CreateDefectVendorTransaction")]
        [SwaggerOperation(Summary = "สร้างรายการ DefectDetail แบบจ้างตรวจ",
        Description = "สร้างรายการ DefectHeader ใน Unit นั้นๆ ให้เป็นแบบจ้างตรวจ")]
        public async Task<object> CreateDefectVendorTransaction([FromBody] CreateDefectObj data)
        {
            try
            {
                string ErrorMsg = "";
                //if (!VerifyHeader(out ErrorMsg))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = ErrorMsg
                //    };
                //}
                int taskNo = 1;
                if (data.TDefectId != 0)
                {
                    string DefectDocNo = "DefectDetail-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "");
                    callTDefectDetail tDefectDetail = new callTDefectDetail();
                    tDefectDetail.RowState = "Original";
                    tDefectDetail.RowActive = true;
                    tDefectDetail.Client_Id = "DefectDetail-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
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
                    tDefectDetail.TDefectDetailDesc = data.TDefectDetailDesc;

                    long inserttdefectdetail = _transactionRepository.InsertTdefectDetail(tDefectDetail);
                    return new
                    {
                        success = true,
                        data = tDefectDetail
                    };
                }
                else
                {
                    ViewUnitCustomer viewUnitCustomer = _masterRepository.GetViewUnitCustomer(data.ItemId, data.ProductId);
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
                    CreateDefect.Desciption = data.Description;
                    CreateDefect.DocIsActive = true;
                    CreateDefect.DocIsExternalAudit = true;
                    CreateDefect.DocIsReqUnitReceiveAttachFile = false;
                    long DefectID = 0;
                    if (viewUnitCustomer != null)
                    {
                        CreateDefect.ContactName = viewUnitCustomer.FirstName + "  " + viewUnitCustomer.LastName;
                    }
                    else
                    {
                        CreateDefect.ContactName = "";
                    }
                    bool InsertData = _transactionRepository.InsertTdefectDetail(CreateDefect, ref DefectID);
                    CreateDefect.TDefectId = Convert.ToInt32(DefectID);
                    // --------------------------------------------------------------------
                    //string DefectDocNo = "DefectDetail-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
                    //                        DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "");
                    //callTDefectDetail tDefectDetail = new callTDefectDetail();
                    //tDefectDetail.RowState = "Original";
                    //tDefectDetail.RowActive = true;
                    //tDefectDetail.Client_Id = "DefectDetail-" + data.DefectType + "-" + data.ProductId + "-" + data.ItemId + "-" +
                    //                        DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "") + Guid.NewGuid();
                    //tDefectDetail.Client_SyncDate = DateTime.Now;
                    //tDefectDetail.TDefectId = CreateDefect.TDefectId;
                    //tDefectDetail.TDefectDocNo = CreateDefect.TDefectDocNo;
                    //tDefectDetail.ProductId = data.ProductId;
                    //tDefectDetail.ItemId = data.ItemId;
                    //tDefectDetail.TDefectDetailStatus = "001";
                    //tDefectDetail.TDefectDetailSubStatus = "";
                    //tDefectDetail.CallTypeId = data.CallTypeID;
                    //tDefectDetail.CallAreaId = data.CallArea;
                    //tDefectDetail.CallDescId = data.CallDescId;
                    //tDefectDetail.CallPointId = data.CallSubPointId;
                    //tDefectDetail.CallSubPointId = data.CallSubPointId;
                    //tDefectDetail.DeviceId = data.DeviceId;
                    //tDefectDetail.Tag = null;
                    //tDefectDetail.CreateUserId = data.UserID;
                    //tDefectDetail.UpdateDate = DateTime.Now;
                    //tDefectDetail.FloorPlan_ImageId = null;
                    //tDefectDetail.FloorPlan_X = 0;
                    //tDefectDetail.FloorPlan_Y = 0;
                    //tDefectDetail.TaskNo = taskNo;
                    //tDefectDetail.TaskMarkName = "DummyData";
                    //tDefectDetail.FloorPlanSet = data.FloorPlanSet;
                    //tDefectDetail.CustRoundAuditNo = 1;
                    //tDefectDetail.CustRoundAuditDate = DateTime.Now;
                    //tDefectDetail.CustRoundAuditDueCloseDate = DateTime.Now.AddDays(20);
                    //tDefectDetail.IsServerLockRow = false;
                    //tDefectDetail.TaskOpenDate = DateTime.Now;
                    //tDefectDetail.TaskProcessDate = null;
                    //tDefectDetail.TaskActualFinishDate = null;
                    //tDefectDetail.TaskActualCloseDate = null;
                    //tDefectDetail.TDefectDetailDesc = data.TDefectDetailDesc;

                    //bool inserttdefectdetail = _transactionRepository.InsertTdefectDetail(tDefectDetail);

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

        //[HttpPost]
        //[Route("uploadTransactionPicture")]
        //[Consumes("multipart/form-data")]
        //public async Task<object> uploadTransactionPicture([FromForm] ParamUploadImage data)
        //{
        //    try
        //    {
        //        List<string> tempUploadPath = new List<string>();
        //        //<PlanImageUploadResult> tempUploadFiles = new List<PlanImageUploadResult>();
        //        string message = "";
        //        //var _ImageUnitMappingData = new List<ImageMappingData>();
        //        //if (!string.IsNullOrEmpty(data.ImageFiles))
        //        //    _ImageUnitMappingData = JsonConvert.DeserializeObject<List<ImageMappingData>>(data.ImageUnitMappingData);

        //        if (data.ImageFiles != null && data.ImageFiles.Count() > 0)
        //        {

        //            //var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "data", "uploads");


        //            foreach (var formFile in data.ImageFiles)
        //            {

        //                long size = data.ImageFiles.Sum(f => f.Length);

        //                var fileName = formFile.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());
        //                var yearPath = DateTime.Now.Year;
        //                var MonthPath = DateTime.Now.Month;
        //                int dataPath = 0; int dataPath2 = 0;

        //                dataPath = Convert.ToInt32(data.ProjectNO);
        //                dataPath2 = data.UnitNo;

        //                var dirPath = $"{yearPath}/{MonthPath}/P{dataPath}";
        //                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "data", "uploads/" + dirPath);
        //                var savePath = new FileInfo(Path.Combine(uploads, fileName));
        //                //if (!savePath.Directory.Exists)
        //                //    savePath.Directory.Create();
        //                if (!Directory.Exists(uploads))
        //                {
        //                    Directory.CreateDirectory(uploads);
        //                }

        //                if (formFile.Length > 0)
        //                {
        //                    var filePath = Path.Combine(uploads, formFile.FileName);
        //                    if (System.IO.File.Exists(filePath))
        //                    {
        //                        string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
        //                        string extension = Path.GetExtension(filePath);
        //                        string path = Path.GetDirectoryName(filePath);
        //                        string newFullPath = filePath;
        //                        string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
        //                        newFullPath = Path.Combine(path, tempFileName + extension);
        //                        using (var fileStream = new FileStream(newFullPath, FileMode.Create))
        //                        {
        //                            message = formFile.Length.ToString();
        //                            await formFile.CopyToAsync(fileStream);
        //                            fileName = tempFileName;

        //                        }
        //                    }
        //                    else
        //                    {
        //                        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //                        {
        //                            message = formFile.Length.ToString();
        //                            await formFile.CopyToAsync(fileStream);

        //                        }
        //                    }


        //                    if (System.IO.File.Exists(filePath))
        //                    {
        //                        //tempUploadPath.Add(filePath.FullName);

        //                        //var filePath = dirPath.Replace("\\", "/");
        //                        //var saveStoragePath = await base.GetQISResourceStoragePhysicalPathAsync(base._appSetting.StorageServerId, dirPath);
        //                        //var url = await base.GetQISFileStorageUrlAsync(base._appSetting.StorageServerId, filePath);

        //                        //if (!new FileInfo(saveStoragePath).Directory.Exists)
        //                        //    new FileInfo(saveStoragePath).Directory.Create();

        //                        // System.IO.File.Copy(savePath.FullName, saveStoragePath, true);
        //                        // var fi = new System.IO.FileInfo(saveStoragePath);

        //                        //var res = new TResource()
        //                        //{
        //                        //    CreatedDate = DateTime.Now,
        //                        //    CreateDeviceId = "",
        //                        //    CreateUserId = data.UserID,
        //                        //    Description = data.Description,
        //                        //    FilePath = filePath,
        //                        //    RowClientId = data.RowClientID,
        //                        //    IsActive = true,
        //                        //    StorageServerId = _appSetting.StorageServerId,
        //                        //    ProjectId = _ImageUnitMappingData.FirstOrDefault().ProjectID,
        //                        //    FileLength = fi.Length
        //                        //};

        //                        //var uPlanImages = new List<MImage>();
        //                        //List<ImageMappingData> Mappings = new List<ImageMappingData>();
        //                        //foreach (var d in _ImageUnitMappingData)
        //                        //{
        //                        //    uPlanImages.Add(new MImage()
        //                        //    {
        //                        //        ResourceRowClientID = res.RowClientId,
        //                        //        UnitID = d.UnitID,
        //                        //        ImageType = "PlanImage",
        //                        //        Description = d.Description,
        //                        //        ImageClientID = Guid.NewGuid(),
        //                        //        CreateDeviceDate = data.CreateDeviceDate,
        //                        //        CreateDeviceId = data.CreateDeviceId
        //                        //    });

        //                        //}


        //                        //await _unitOfWork.SyncRepository.AddPlanImages(res, uPlanImages);


        //                        //var dirExport = Path.Combine(_hostingEnvironment.WebRootPath, "data", "FloorPlanImages");
        //                        //if (!Directory.Exists(dirExport))
        //                        //    Directory.CreateDirectory(dirExport);

        //                        filePath = filePath.Replace("\\", "/");
        //                        var saveStoragePath = await base.GetQISResourceStoragePhysicalPathAsync(base._appSetting.StorageServerId, filePath);
        //                        // var savePath = new FileInfo(Path.Combine(saveStoragePath, dirPath));

        //                        //var url = await base.GetQISFileStorageUrlAsync(base._appSetting.StorageServerId, filePath);

        //                        //if (!new FileInfo(saveStoragePath).Directory.Exists)
        //                        //    new FileInfo(saveStoragePath).Directory.Create();

        //                        // System.IO.File.Copy(savePath.FullName, saveStoragePath, true);
        //                        // var fi = new System.IO.FileInfo(saveStoragePath);

        //                        //var res = new TResource()

        //                        var ImageId = data.RowClientID.ToLower();
        //                        if (string.IsNullOrEmpty(ImageId))
        //                            ImageId = Guid.NewGuid().ToString("N");

        //                        var tempUploadFile = savePath.FullName;

        //                        //var newDir = new DirectoryInfo(filePath);
        //                        //if (!newDir.Exists)
        //                        //    newDir.Create();

        //                        string mainResizePath = System.IO.Path.Combine(savePath.DirectoryName, string.Format("PLM{0}.jpg", ImageId));
        //                        mainResizePath = mainResizePath.Replace("\\", "/");

        //                        /*--- New Code ---*/


        //                        long ResFileLength = 0;

        //                        var tempUploadad = new FileInfo(mainResizePath);
        //                        ResFileLength = tempUploadad.Length;

        //                        string ResId = Guid.NewGuid().ToString();


        //                        //For Save Main Image
        //                        var valid = await func_SaveImageAsync(ResId.ToString(), dataPath, dataPath2, mainResizePath, ImageId, true, ResFileLength, data.Description, data.CreateDeviceId, data.UserID, data.CreateDeviceDate, _ImageUnitMappingData, data.CreatorFullName, data.UserID);


        //                        return new
        //                        {
        //                            success = true,
        //                            data = new { MappingPlanImages = tempUploadFiles },
        //                            // data = _data.ToList(),
        //                            //message = string.Format("found {0} items", _data.Count)
        //                        };

        //                    }


        //                    //var dataResult = await _unitOfWork.MasterRepository.GetUnitSearch(data.ProjectId, data.SearchText);
        //                    //var _data = dataResult.ToList();


        //                    return new
        //                    {
        //                        success = false,
        //                        message = "missing upload file."
        //                    };



        //                }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, "Internal server error :: " + ex.Message);
        //    }

        //}

        [HttpPost("uploadBeforePicture")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ Before",
        Description = "Upload รูปภาพของรายการ TDefectDetail Before ")]
        public async Task<object> uploadPicture([FromForm] ParamUploadImageBefore data)
        {
            int a = 0;
            //List<TResource> TresourceData = JsonConvert.DeserializeObject<List<TResource>>(data.Resource);
            //string StroageID = base._appSetting.StorageServerId.ToString();

            //com.apthai.QISAPINETCore.Model.QIS.MStorageServer StorageData = _ResourceRepo.GetStorageServerDetail(StroageID);

            //var UploadLoctaion = StorageData.StoragePhysicalPath;
            int SuccessUploadCount = 0;
            int count = 0;
            minio = new MinioServices();
            callResource callResourceDate = new callResource();
            if (data.Files != null)
            {
                var path = $"{data.ProjectCode}/{data.UnitNo}";
                var fileName = data.Files.FileName;
                var resultMinio = await minio.UploadFile(data.Files, path, fileName);
                long size = data.Files.Length;

                callResourceDate.FilePath = $"{path}/{fileName}";
                callResourceDate.FileLength = size;
                callResourceDate.CreateDate = DateTime.Now;
                callResourceDate.RowState = "Original";
                callResourceDate.ResourceType = 7;
                callResourceDate.ResourceTagCode = "BF-RP";
                callResourceDate.ResourceGroupSet = null;
                callResourceDate.ResourceGroupOrder = 0;
                callResourceDate.TDefectDetailId = data.TDefectDetailId == "" ? 0 : Convert.ToInt32(data.TDefectDetailId);
                callResourceDate.ProjectNo = data.ProjectCode;
                callResourceDate.SerialNo = data.UnitNo;
                callResourceDate.UserId = Convert.ToString(data.UserID);
                callResourceDate.UpdateUserId = Convert.ToString(data.UserID); 
                callResourceDate.Active = true;
                callResourceDate.TDefectId = Convert.ToInt32(data.TDefectID) ;
                callResourceDate.StorageServerId = 1400;
                callResourceDate.FullFilePath = resultMinio.Url;
                callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6); ;
                //TresourceData[i].FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                //TresourceData[i].FileLength = size;
                //TresourceData[i].CreatedDate = DateTime.Now;
                //TresourceData[i].CreateUserId = Convert.ToInt32(data.UserID);
                //TresourceData[i].RowSyncDate = DateTime.Now;
                //TresourceData[i].StorageServerId = StorageData.StorageServerId;
                bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);

                //if (InsertResult == true)
                //{
                //SuccessUploadCount++;
                //Model.QIS.TResource TresourceTransfer = new Model.QIS.TResource();
                //TresourceTransfer.ResourceType = TresourceData[i].ResourceType;
                //TresourceTransfer.ResourceTagCode = TresourceData[i].ResourceTagCode;
                //TresourceTransfer.ResourceTagSubCode = TresourceData[i].ResourceTagSubCode;
                //TresourceTransfer.ResourceGroupSet = TresourceData[i].ResourceGroupSet;
                //TresourceTransfer.ResourceGroupOrder = TresourceData[i].ResourceGroupOrder;
                //TresourceTransfer.ResourceMineType = TresourceData[i].ResourceMineType;
                //TresourceTransfer.ProjectId = TresourceData[i].ProjectId;
                //TresourceTransfer.UnitId = TresourceData[i].UnitId;
                ////TresourceTransfer.FilePath = _appSetting.PictureRootURL + TresourceData[i].FilePath;
                //TresourceTransfer.FilePath = Environment.GetEnvironmentVariable("PictureRootURL") + TresourceData[i].FilePath;
                //TresourceTransfer.Description = TresourceData[i].Description;
                //TresourceTransfer.IsActive = TresourceData[i].IsActive;
                //TresourceTransfer.StorageServerId = TresourceData[i].StorageServerId;
                //TresourceTransfer.PhaseId = TresourceData[i].PhaseId;
                //TresourceTransfer.HeaderId = TresourceData[i].HeaderId;
                //TresourceTransfer.DetailId = TresourceData[i].DetailId;
                //TresourceTransfer.UDetailId = TresourceData[i].UDetailId;
                //TresourceTransfer.UDetail_RowClientId = TresourceData[i].UDetail_RowClientId;
                //TresourceTransfer.Tag = TresourceData[i].Tag;
                //TresourceTransfer.RowClientId = TresourceData[i].RowClientId;
                //TresourceTransfer.RowState = TresourceData[i].RowState;
                //TresourceTransfer.RowVersion = TresourceData[i].RowVersion;
                //TresourceTransfer.RowSyncDate = TresourceData[i].RowSyncDate;
                //TresourceTransfer.CreateDeviceId = TresourceData[i].CreateDeviceId;
                //TresourceTransfer.CreateUserId = TresourceData[i].CreateUserId;
                //TresourceTransfer.ModifiedDeviceId = TresourceData[i].ModifiedDeviceId;
                //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                //TresourceTransfer.CreatedDate = TresourceData[i].CreatedDate;
                //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                //TresourceTransfer.UFHeaderId = TresourceData[i].UFHeaderId;
                //TresourceTransfer.UHeaderId = TresourceData[i].UHeaderId;
                //TresourceTransfer.UFDetailId = TresourceData[i].UFDetailID;
                //TresourceTransfer.FileLength = TresourceData[i].FileLength;
                //TresourceTransfer.UPhaseId = TresourceData[i].UPhaseId;
                //TresourceTransfer.UPhase_RowClientId = TresourceData[i].UPhase_RowClientId;
                //TresourceTransfer.UHeader_RowClientId = TresourceData[i].UHeader_RowClientId;
                //TresourceTransfer.UFPhase_RowClientId = TresourceData[i].UFPhase_RowClientId;
                //TresourceTransfer.UFHeader_RowClientId = TresourceData[i].UFHeader_RowClientId;
                //TresourceTransfer.UFDetail_RowClientId = TresourceData[i].UFDetail_RowClientId;
                //TresourceTransfer.TagState = TresourceData[i].TagState;
                //TresourceTransfer.ClientDataType = TresourceData[i].ClientDataType;

                //bool InsertResultWeb = _ResourceRepo.InsertTResourceWeb(TresourceTransfer);
                //}
            }
            else
            {
                callResource x = new callResource();
                return new
                {
                    success = false,
                    data = x,
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            callResource Result = new callResource();
            return new
            {
                success = true,
                data = Result,
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost("uploadAfterPicture")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ After",
        Description = "Upload รูปภาพของรายการ TDefectDetail After ")]
        public async Task<object> uploadAfterPicture([FromForm] ParamUploadImageAfter data)
        {
            int a = 0;
            //List<TResource> TresourceData = JsonConvert.DeserializeObject<List<TResource>>(data.Resource);
            //string StroageID = base._appSetting.StorageServerId.ToString();

            //com.apthai.QISAPINETCore.Model.QIS.MStorageServer StorageData = _ResourceRepo.GetStorageServerDetail(StroageID);

            //var UploadLoctaion = StorageData.StoragePhysicalPath;
            int SuccessUploadCount = 0;
            int count = 0;


            callResource callResourceDate = new callResource();
            if (data.Files != null)
            {
                // -- New ---- for Docker
                var yearPath = DateTime.Now.Year;
                var MonthPath = DateTime.Now.Month;
                var dirPath1 = $"{yearPath}/{MonthPath}";
                int dataPath = 0;
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "data", "uploads", dirPath1);

                string FileBinary;

                var resultMinio = await minio.UploadFile(data.Files, dirPath1, data.Files.FileName);
                long size = data.Files.Length;
                string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                var dirPath = $"{yearPath}\\M{dataPath}";

                // --- New Docker ----
                //if (!Directory.Exists(uploads))
                //{
                //    Directory.CreateDirectory(uploads);
                //}
                if (data.Files.Length > 0)
                {
                    //var filePath = Path.Combine(uploads, data.Files.FileName);
                    //var message = "";
                    //if (System.IO.File.Exists(filePath))
                    //{
                    //    string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                    //    string extension = Path.GetExtension(filePath);
                    //    string path = Path.GetDirectoryName(filePath);
                    //    string newFullPath = filePath;
                    //    string tempFileName = string.Format("{0}({1})", fileNameOnly);
                    //    newFullPath = Path.Combine(path, tempFileName + extension);
                    //    using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                    //    {
                    //        message = data.Files.Length.ToString();
                    //        await data.Files.CopyToAsync(fileStream);
                    //        fileName = tempFileName + extension;

                    //    }
                    //}
                    //else
                    //{
                    //    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    //    {
                    //        message = data.Files.Length.ToString();
                    //        await data.Files.CopyToAsync(fileStream);
                    //    }
                    //}

                    // -- -End New -----
                    //if (System.IO.File.Exists(savePath.FullName))
                    string FileExtention = Path.GetExtension(data.Files.FileName);
                    // ----- Old -----
                    //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                    // ----- New Docker -----
                    callResourceDate.FilePath = JsonConvert.SerializeObject(resultMinio).ToString(); //"data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                    callResourceDate.FileLength = size;
                    callResourceDate.CreateDate = DateTime.Now;
                    callResourceDate.RowState = "Original";
                    callResourceDate.ResourceType = 7;
                    callResourceDate.ResourceTagCode = "BF-RP";
                    callResourceDate.ResourceGroupSet = null;
                    callResourceDate.ResourceGroupOrder = 0;
                    callResourceDate.TDefectDetailId = data.TDefectDetailId == "" ? 0 : Convert.ToInt32(data.TDefectDetailId);
                    callResourceDate.ProjectNo = data.ProjectCode;
                    callResourceDate.SerialNo = data.UnitNo;
                    callResourceDate.Active = true;
                    callResourceDate.UpdateUserId = Convert.ToString(data.UserID);
                    callResourceDate.StorageServerId = 1400;
                    callResourceDate.FullFilePath = resultMinio.Url;
                    callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6);

                    //TresourceData[i].FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                    //TresourceData[i].FileLength = size;
                    //TresourceData[i].CreatedDate = DateTime.Now;
                    //TresourceData[i].CreateUserId = Convert.ToInt32(data.UserID);
                    //TresourceData[i].RowSyncDate = DateTime.Now;
                    //TresourceData[i].StorageServerId = StorageData.StorageServerId;
                    bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);

                    //if (InsertResult == true)
                    //{
                    //SuccessUploadCount++;
                    //Model.QIS.TResource TresourceTransfer = new Model.QIS.TResource();
                    //TresourceTransfer.ResourceType = TresourceData[i].ResourceType;
                    //TresourceTransfer.ResourceTagCode = TresourceData[i].ResourceTagCode;
                    //TresourceTransfer.ResourceTagSubCode = TresourceData[i].ResourceTagSubCode;
                    //TresourceTransfer.ResourceGroupSet = TresourceData[i].ResourceGroupSet;
                    //TresourceTransfer.ResourceGroupOrder = TresourceData[i].ResourceGroupOrder;
                    //TresourceTransfer.ResourceMineType = TresourceData[i].ResourceMineType;
                    //TresourceTransfer.ProjectId = TresourceData[i].ProjectId;
                    //TresourceTransfer.UnitId = TresourceData[i].UnitId;
                    ////TresourceTransfer.FilePath = _appSetting.PictureRootURL + TresourceData[i].FilePath;
                    //TresourceTransfer.FilePath = Environment.GetEnvironmentVariable("PictureRootURL") + TresourceData[i].FilePath;
                    //TresourceTransfer.Description = TresourceData[i].Description;
                    //TresourceTransfer.IsActive = TresourceData[i].IsActive;
                    //TresourceTransfer.StorageServerId = TresourceData[i].StorageServerId;
                    //TresourceTransfer.PhaseId = TresourceData[i].PhaseId;
                    //TresourceTransfer.HeaderId = TresourceData[i].HeaderId;
                    //TresourceTransfer.DetailId = TresourceData[i].DetailId;
                    //TresourceTransfer.UDetailId = TresourceData[i].UDetailId;
                    //TresourceTransfer.UDetail_RowClientId = TresourceData[i].UDetail_RowClientId;
                    //TresourceTransfer.Tag = TresourceData[i].Tag;
                    //TresourceTransfer.RowClientId = TresourceData[i].RowClientId;
                    //TresourceTransfer.RowState = TresourceData[i].RowState;
                    //TresourceTransfer.RowVersion = TresourceData[i].RowVersion;
                    //TresourceTransfer.RowSyncDate = TresourceData[i].RowSyncDate;
                    //TresourceTransfer.CreateDeviceId = TresourceData[i].CreateDeviceId;
                    //TresourceTransfer.CreateUserId = TresourceData[i].CreateUserId;
                    //TresourceTransfer.ModifiedDeviceId = TresourceData[i].ModifiedDeviceId;
                    //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                    //TresourceTransfer.CreatedDate = TresourceData[i].CreatedDate;
                    //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                    //TresourceTransfer.UFHeaderId = TresourceData[i].UFHeaderId;
                    //TresourceTransfer.UHeaderId = TresourceData[i].UHeaderId;
                    //TresourceTransfer.UFDetailId = TresourceData[i].UFDetailID;
                    //TresourceTransfer.FileLength = TresourceData[i].FileLength;
                    //TresourceTransfer.UPhaseId = TresourceData[i].UPhaseId;
                    //TresourceTransfer.UPhase_RowClientId = TresourceData[i].UPhase_RowClientId;
                    //TresourceTransfer.UHeader_RowClientId = TresourceData[i].UHeader_RowClientId;
                    //TresourceTransfer.UFPhase_RowClientId = TresourceData[i].UFPhase_RowClientId;
                    //TresourceTransfer.UFHeader_RowClientId = TresourceData[i].UFHeader_RowClientId;
                    //TresourceTransfer.UFDetail_RowClientId = TresourceData[i].UFDetail_RowClientId;
                    //TresourceTransfer.TagState = TresourceData[i].TagState;
                    //TresourceTransfer.ClientDataType = TresourceData[i].ClientDataType;

                    //bool InsertResultWeb = _ResourceRepo.InsertTResourceWeb(TresourceTransfer);
                    //}

                }

            }
            else
            {
                callResource x = new callResource();
                return new
                {
                    success = false,
                    data = x,
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            callResource Result = new callResource();
            return new
            {
                success = true,
                data = Result,
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost("uploadPDF")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadร ไฟล์ PDF",
      Description = "ลบข้อมูล T_resource จาก Database ของ Qis-SYnc")]
        public async Task<object> uploadPDF([FromForm] ParamUploadImage data)
        {
            int a = 0;
            //List<TResource> TresourceData = JsonConvert.DeserializeObject<List<TResource>>(data.Resource);
            //string StroageID = base._appSetting.StorageServerId.ToString();

            //com.apthai.QISAPINETCore.Model.QIS.MStorageServer StorageData = _ResourceRepo.GetStorageServerDetail(StroageID);

            //var UploadLoctaion = StorageData.StoragePhysicalPath;
            int SuccessUploadCount = 0;
            int count = 0;
            minio = new MinioServices();
            callResource callResourceDate = new callResource();
            if (data.Files != null)
            {
                var path = $"{data.ProjectCode}/{data.UnitNo}/PDF";
                var fileName = data.Files.FileName;
                var resultMinio = await minio.UploadFile(data.Files, path, fileName);
                long size = data.Files.Length;

                callResourceDate.FilePath = $"{path}/{fileName}";
                callResourceDate.FileLength = size;
                callResourceDate.CreateDate = DateTime.Now;
                callResourceDate.RowState = "Original";
                callResourceDate.ResourceType = data.ResourceType;
                callResourceDate.ResourceTagCode = data.ResourceTagCode;
                callResourceDate.ResourceGroupSet = null;
                callResourceDate.ResourceGroupOrder = 0;
                callResourceDate.TDefectId = data.TDefectID == "" ? 0 : Convert.ToInt32(data.TDefectID);
                callResourceDate.TDefectDetailId = data.TDefectDetailId == "" ? 0 : Convert.ToInt32(data.TDefectDetailId);
                callResourceDate.ProjectNo = data.ProjectCode;
                callResourceDate.SerialNo = data.UnitNo;
                callResourceDate.Active = true;
                callResourceDate.FullFilePath = resultMinio.Url;
                callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6); ;
                //TresourceData[i].FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                //TresourceData[i].FileLength = size;
                //TresourceData[i].CreatedDate = DateTime.Now;
                //TresourceData[i].CreateUserId = Convert.ToInt32(data.UserID);
                //TresourceData[i].RowSyncDate = DateTime.Now;
                //TresourceData[i].StorageServerId = StorageData.StorageServerId;
                bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);

                //if (InsertResult == true)
                //{
                //SuccessUploadCount++;
                //Model.QIS.TResource TresourceTransfer = new Model.QIS.TResource();
                //TresourceTransfer.ResourceType = TresourceData[i].ResourceType;
                //TresourceTransfer.ResourceTagCode = TresourceData[i].ResourceTagCode;
                //TresourceTransfer.ResourceTagSubCode = TresourceData[i].ResourceTagSubCode;
                //TresourceTransfer.ResourceGroupSet = TresourceData[i].ResourceGroupSet;
                //TresourceTransfer.ResourceGroupOrder = TresourceData[i].ResourceGroupOrder;
                //TresourceTransfer.ResourceMineType = TresourceData[i].ResourceMineType;
                //TresourceTransfer.ProjectId = TresourceData[i].ProjectId;
                //TresourceTransfer.UnitId = TresourceData[i].UnitId;
                ////TresourceTransfer.FilePath = _appSetting.PictureRootURL + TresourceData[i].FilePath;
                //TresourceTransfer.FilePath = Environment.GetEnvironmentVariable("PictureRootURL") + TresourceData[i].FilePath;
                //TresourceTransfer.Description = TresourceData[i].Description;
                //TresourceTransfer.IsActive = TresourceData[i].IsActive;
                //TresourceTransfer.StorageServerId = TresourceData[i].StorageServerId;
                //TresourceTransfer.PhaseId = TresourceData[i].PhaseId;
                //TresourceTransfer.HeaderId = TresourceData[i].HeaderId;
                //TresourceTransfer.DetailId = TresourceData[i].DetailId;
                //TresourceTransfer.UDetailId = TresourceData[i].UDetailId;
                //TresourceTransfer.UDetail_RowClientId = TresourceData[i].UDetail_RowClientId;
                //TresourceTransfer.Tag = TresourceData[i].Tag;
                //TresourceTransfer.RowClientId = TresourceData[i].RowClientId;
                //TresourceTransfer.RowState = TresourceData[i].RowState;
                //TresourceTransfer.RowVersion = TresourceData[i].RowVersion;
                //TresourceTransfer.RowSyncDate = TresourceData[i].RowSyncDate;
                //TresourceTransfer.CreateDeviceId = TresourceData[i].CreateDeviceId;
                //TresourceTransfer.CreateUserId = TresourceData[i].CreateUserId;
                //TresourceTransfer.ModifiedDeviceId = TresourceData[i].ModifiedDeviceId;
                //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                //TresourceTransfer.CreatedDate = TresourceData[i].CreatedDate;
                //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                //TresourceTransfer.UFHeaderId = TresourceData[i].UFHeaderId;
                //TresourceTransfer.UHeaderId = TresourceData[i].UHeaderId;
                //TresourceTransfer.UFDetailId = TresourceData[i].UFDetailID;
                //TresourceTransfer.FileLength = TresourceData[i].FileLength;
                //TresourceTransfer.UPhaseId = TresourceData[i].UPhaseId;
                //TresourceTransfer.UPhase_RowClientId = TresourceData[i].UPhase_RowClientId;
                //TresourceTransfer.UHeader_RowClientId = TresourceData[i].UHeader_RowClientId;
                //TresourceTransfer.UFPhase_RowClientId = TresourceData[i].UFPhase_RowClientId;
                //TresourceTransfer.UFHeader_RowClientId = TresourceData[i].UFHeader_RowClientId;
                //TresourceTransfer.UFDetail_RowClientId = TresourceData[i].UFDetail_RowClientId;
                //TresourceTransfer.TagState = TresourceData[i].TagState;
                //TresourceTransfer.ClientDataType = TresourceData[i].ClientDataType;

                //bool InsertResultWeb = _ResourceRepo.InsertTResourceWeb(TresourceTransfer);
                //}                    

            }
            else
            {
                callResource x = new callResource();
                return new
                {
                    success = false,
                    data = x,
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            callResource Result = new callResource();
            return new
            {
                success = true,
                data = Result,
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost("uploadDocCusNotSign")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadเอกสารไม่เซ็นบนมือถือของลูกค้า ",
Description = "ลบข้อมูล T_resource จาก Database ของ Qis-SYnc")]
        public async Task<object> uploadDocCusNotSign([FromForm] ParamUploadImageCusNotSign data)
        {
            int a = 0;
            //List<TResource> TresourceData = JsonConvert.DeserializeObject<List<TResource>>(data.Resource);
            //string StroageID = base._appSetting.StorageServerId.ToString();

            //com.apthai.QISAPINETCore.Model.QIS.MStorageServer StorageData = _ResourceRepo.GetStorageServerDetail(StroageID);

            //var UploadLoctaion = StorageData.StoragePhysicalPath;
            int SuccessUploadCount = 0;
            int count = 0;
            minio = new MinioServices();
            callResource callResourceDate = new callResource();
            if (data.Files != null)
            {
                var path = $"{data.ProjectCode}/{data.UnitNo}";
                var fileName = data.Files.FileName;
                var resultMinio = await minio.UploadFile(data.Files, path, fileName);
                long size = data.Files.Length;
                if (data.Files.Length > 0)
                {
                    var message = "";
                    callResourceDate.FilePath = $"{path}/{fileName}";
                    callResourceDate.FileLength = size;
                    callResourceDate.CreateDate = DateTime.Now;
                    callResourceDate.RowState = "Original";
                    callResourceDate.ResourceType = 12;
                    callResourceDate.ResourceTagCode = null;
                    callResourceDate.ResourceGroupSet = null;
                    callResourceDate.ResourceGroupOrder = 0;
                    callResourceDate.TDefectDetailId = data.TDefectDetailId == "" ? 0 : Convert.ToInt32(data.TDefectDetailId);
                    callResourceDate.ProjectNo = data.ProjectCode;
                    callResourceDate.SerialNo = data.UnitNo;
                    callResourceDate.Active = true;
                    callResourceDate.FullFilePath = resultMinio.Url;
                    callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6); ;
                    //TresourceData[i].FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                    //TresourceData[i].FileLength = size;
                    //TresourceData[i].CreatedDate = DateTime.Now;
                    //TresourceData[i].CreateUserId = Convert.ToInt32(data.UserID);
                    //TresourceData[i].RowSyncDate = DateTime.Now;
                    //TresourceData[i].StorageServerId = StorageData.StorageServerId;
                    bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);

                    //if (InsertResult == true)
                    //{
                    //SuccessUploadCount++;
                    //Model.QIS.TResource TresourceTransfer = new Model.QIS.TResource();
                    //TresourceTransfer.ResourceType = TresourceData[i].ResourceType;
                    //TresourceTransfer.ResourceTagCode = TresourceData[i].ResourceTagCode;
                    //TresourceTransfer.ResourceTagSubCode = TresourceData[i].ResourceTagSubCode;
                    //TresourceTransfer.ResourceGroupSet = TresourceData[i].ResourceGroupSet;
                    //TresourceTransfer.ResourceGroupOrder = TresourceData[i].ResourceGroupOrder;
                    //TresourceTransfer.ResourceMineType = TresourceData[i].ResourceMineType;
                    //TresourceTransfer.ProjectId = TresourceData[i].ProjectId;
                    //TresourceTransfer.UnitId = TresourceData[i].UnitId;
                    ////TresourceTransfer.FilePath = _appSetting.PictureRootURL + TresourceData[i].FilePath;
                    //TresourceTransfer.FilePath = Environment.GetEnvironmentVariable("PictureRootURL") + TresourceData[i].FilePath;
                    //TresourceTransfer.Description = TresourceData[i].Description;
                    //TresourceTransfer.IsActive = TresourceData[i].IsActive;
                    //TresourceTransfer.StorageServerId = TresourceData[i].StorageServerId;
                    //TresourceTransfer.PhaseId = TresourceData[i].PhaseId;
                    //TresourceTransfer.HeaderId = TresourceData[i].HeaderId;
                    //TresourceTransfer.DetailId = TresourceData[i].DetailId;
                    //TresourceTransfer.UDetailId = TresourceData[i].UDetailId;
                    //TresourceTransfer.UDetail_RowClientId = TresourceData[i].UDetail_RowClientId;
                    //TresourceTransfer.Tag = TresourceData[i].Tag;
                    //TresourceTransfer.RowClientId = TresourceData[i].RowClientId;
                    //TresourceTransfer.RowState = TresourceData[i].RowState;
                    //TresourceTransfer.RowVersion = TresourceData[i].RowVersion;
                    //TresourceTransfer.RowSyncDate = TresourceData[i].RowSyncDate;
                    //TresourceTransfer.CreateDeviceId = TresourceData[i].CreateDeviceId;
                    //TresourceTransfer.CreateUserId = TresourceData[i].CreateUserId;
                    //TresourceTransfer.ModifiedDeviceId = TresourceData[i].ModifiedDeviceId;
                    //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                    //TresourceTransfer.CreatedDate = TresourceData[i].CreatedDate;
                    //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                    //TresourceTransfer.UFHeaderId = TresourceData[i].UFHeaderId;
                    //TresourceTransfer.UHeaderId = TresourceData[i].UHeaderId;
                    //TresourceTransfer.UFDetailId = TresourceData[i].UFDetailID;
                    //TresourceTransfer.FileLength = TresourceData[i].FileLength;
                    //TresourceTransfer.UPhaseId = TresourceData[i].UPhaseId;
                    //TresourceTransfer.UPhase_RowClientId = TresourceData[i].UPhase_RowClientId;
                    //TresourceTransfer.UHeader_RowClientId = TresourceData[i].UHeader_RowClientId;
                    //TresourceTransfer.UFPhase_RowClientId = TresourceData[i].UFPhase_RowClientId;
                    //TresourceTransfer.UFHeader_RowClientId = TresourceData[i].UFHeader_RowClientId;
                    //TresourceTransfer.UFDetail_RowClientId = TresourceData[i].UFDetail_RowClientId;
                    //TresourceTransfer.TagState = TresourceData[i].TagState;
                    //TresourceTransfer.ClientDataType = TresourceData[i].ClientDataType;

                    //bool InsertResultWeb = _ResourceRepo.InsertTResourceWeb(TresourceTransfer);
                    //}
                }
            }
            else
            {
                callResource x = new callResource();
                return new
                {
                    success = false,
                    data = x,
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            callResource Result = new callResource();
            return new
            {
                success = true,
                data = Result,
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost("uploadSignatureCus")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
Description = "ลบข้อมูล T_resource จาก Database ของ Qis-SYnc")]
        public async Task<object> uploadSignature([FromForm] ParamUploadImageCusSignature data)
        {
            int SuccessUploadCount = 0;
            var pathUrlSig = "";
            callResource callResourceDate = new callResource();
            minio = new MinioServices();
            if (data.Files != null)
            {
                var path = $"{data.ProjectCode}/{data.UnitNo}/Inkpad";
                var fileName = data.Files.FileName;
                var resultMinio = await minio.UploadFile(data.Files, path, fileName);
                long size = data.Files.Length;


                callResourceDate.FilePath = $"{path}/{fileName}";
                callResourceDate.FileLength = size;
                callResourceDate.CreateDate = DateTime.Now;
                callResourceDate.RowState = "Original";
                callResourceDate.ResourceType = 1;
                callResourceDate.ResourceTagSubCode = "1";
                callResourceDate.ResourceMineType = data.Files.ContentType;
                if (data.IsBF == true)
                {
                    callResourceDate.ResourceTagCode = "CUST-BF";
                }
                else
                {
                    callResourceDate.ResourceTagCode = "CUST-AF";
                }
                callResourceDate.ResourceGroupSet = null;
                callResourceDate.ResourceGroupOrder = 0;
                callResourceDate.TDefectDetailId = 0;
                callResourceDate.TDefectId = data.TDefectID == "" ? 0 : Convert.ToInt32(data.TDefectID);
                callResourceDate.ProjectNo = data.ProjectCode;
                callResourceDate.SerialNo = data.UnitNo;
                callResourceDate.Active = true;
                callResourceDate.StorageServerId = 1400;
                callResourceDate.FullFilePath = resultMinio.Url;
                callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6);
                bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);
                pathUrlSig = callResourceDate.FilePath;

                await Task.Run(() => BackgroundJob.Enqueue(() => _syncRepository.GenerateReport(new ParamReportModel()
                {
                    ProjectCode = data.ProjectCode,
                    UnitNo = data.UnitNo,
                    TDefectId = Int32.Parse(data.TDefectID),
                    ProjectType = data.ProjectType,
                    SignatureType = callResourceDate.ResourceTagCode
                })));
            }

            else
            {
                callResource x = new callResource();
                return new
                {
                    success = false,
                    data = x,
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            if (String.IsNullOrEmpty(pathUrlSig))
            {
                return new
                {
                    success = false,
                    data = pathUrlSig,
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }

            return new
            {
                success = true,
                data = pathUrlSig,
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }
        [HttpPost("uploadSignatureLC")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพลายเซ็นของ LC หรือ ",
Description = "ลบข้อมูล T_resource จาก Database ของ Qis-SYnc")]
        public async Task<object> uploadSignatureSE([FromForm] ParamUploadImageCusSignature data)
        {
            int a = 0;
            //List<TResource> TresourceData = JsonConvert.DeserializeObject<List<TResource>>(data.Resource);
            //string StroageID = base._appSetting.StorageServerId.ToString();

            //com.apthai.QISAPINETCore.Model.QIS.MStorageServer StorageData = _ResourceRepo.GetStorageServerDetail(StroageID);

            //var UploadLoctaion = StorageData.StoragePhysicalPath;
            int SuccessUploadCount = 0;
            int count = 0;
            minio = new MinioServices();
            callResource callResourceDate = new callResource();
            if (data.Files != null)
            {
                var path = $"{data.ProjectCode}/{data.UnitNo}/Inkpad";
                var fileName = data.Files.FileName;
                var resultMinio = await minio.UploadFile(data.Files, path, fileName);
                long size = data.Files.Length;

                callResourceDate.FilePath = $"{path}/{fileName}";
                callResourceDate.FileLength = size;
                callResourceDate.CreateDate = DateTime.Now;
                callResourceDate.RowState = "Original";
                callResourceDate.ResourceType = 1;
                callResourceDate.ResourceTagSubCode = "1";
                if (data.IsBF == true)
                {
                    callResourceDate.ResourceTagCode = "SAL-LC-BF";
                }
                else
                {
                    callResourceDate.ResourceTagCode = "SAL-LC-AF";
                }
                callResourceDate.ResourceGroupSet = null;
                callResourceDate.ResourceGroupOrder = 0;
                callResourceDate.TDefectDetailId = 0;
                callResourceDate.TDefectId = data.TDefectID == "" ? 0 : Convert.ToInt32(data.TDefectID);
                callResourceDate.ProjectNo = data.ProjectCode;
                callResourceDate.SerialNo = data.UnitNo;
                callResourceDate.Active = true;
                callResourceDate.StorageServerId = 1400;
                callResourceDate.FullFilePath = resultMinio.Url;
                callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6);
                //TresourceData[i].FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                //TresourceData[i].FileLength = size;
                //TresourceData[i].CreatedDate = DateTime.Now;
                //TresourceData[i].CreateUserId = Convert.ToInt32(data.UserID);
                //TresourceData[i].RowSyncDate = DateTime.Now;
                //TresourceData[i].StorageServerId = StorageData.StorageServerId;
                bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);

                //if (InsertResult == true)
                //{
                //SuccessUploadCount++;
                //Model.QIS.TResource TresourceTransfer = new Model.QIS.TResource();
                //TresourceTransfer.ResourceType = TresourceData[i].ResourceType;
                //TresourceTransfer.ResourceTagCode = TresourceData[i].ResourceTagCode;
                //TresourceTransfer.ResourceTagSubCode = TresourceData[i].ResourceTagSubCode;
                //TresourceTransfer.ResourceGroupSet = TresourceData[i].ResourceGroupSet;
                //TresourceTransfer.ResourceGroupOrder = TresourceData[i].ResourceGroupOrder;
                //TresourceTransfer.ResourceMineType = TresourceData[i].ResourceMineType;
                //TresourceTransfer.ProjectId = TresourceData[i].ProjectId;
                //TresourceTransfer.UnitId = TresourceData[i].UnitId;
                ////TresourceTransfer.FilePath = _appSetting.PictureRootURL + TresourceData[i].FilePath;
                //TresourceTransfer.FilePath = Environment.GetEnvironmentVariable("PictureRootURL") + TresourceData[i].FilePath;
                //TresourceTransfer.Description = TresourceData[i].Description;
                //TresourceTransfer.IsActive = TresourceData[i].IsActive;
                //TresourceTransfer.StorageServerId = TresourceData[i].StorageServerId;
                //TresourceTransfer.PhaseId = TresourceData[i].PhaseId;
                //TresourceTransfer.HeaderId = TresourceData[i].HeaderId;
                //TresourceTransfer.DetailId = TresourceData[i].DetailId;
                //TresourceTransfer.UDetailId = TresourceData[i].UDetailId;
                //TresourceTransfer.UDetail_RowClientId = TresourceData[i].UDetail_RowClientId;
                //TresourceTransfer.Tag = TresourceData[i].Tag;
                //TresourceTransfer.RowClientId = TresourceData[i].RowClientId;
                //TresourceTransfer.RowState = TresourceData[i].RowState;
                //TresourceTransfer.RowVersion = TresourceData[i].RowVersion;
                //TresourceTransfer.RowSyncDate = TresourceData[i].RowSyncDate;
                //TresourceTransfer.CreateDeviceId = TresourceData[i].CreateDeviceId;
                //TresourceTransfer.CreateUserId = TresourceData[i].CreateUserId;
                //TresourceTransfer.ModifiedDeviceId = TresourceData[i].ModifiedDeviceId;
                //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                //TresourceTransfer.CreatedDate = TresourceData[i].CreatedDate;
                //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                //TresourceTransfer.UFHeaderId = TresourceData[i].UFHeaderId;
                //TresourceTransfer.UHeaderId = TresourceData[i].UHeaderId;
                //TresourceTransfer.UFDetailId = TresourceData[i].UFDetailID;
                //TresourceTransfer.FileLength = TresourceData[i].FileLength;
                //TresourceTransfer.UPhaseId = TresourceData[i].UPhaseId;
                //TresourceTransfer.UPhase_RowClientId = TresourceData[i].UPhase_RowClientId;
                //TresourceTransfer.UHeader_RowClientId = TresourceData[i].UHeader_RowClientId;
                //TresourceTransfer.UFPhase_RowClientId = TresourceData[i].UFPhase_RowClientId;
                //TresourceTransfer.UFHeader_RowClientId = TresourceData[i].UFHeader_RowClientId;
                //TresourceTransfer.UFDetail_RowClientId = TresourceData[i].UFDetail_RowClientId;
                //TresourceTransfer.TagState = TresourceData[i].TagState;
                //TresourceTransfer.ClientDataType = TresourceData[i].ClientDataType;

                //bool InsertResultWeb = _ResourceRepo.InsertTResourceWeb(TresourceTransfer);
                //}
            }
            else
            {
                callResource x = new callResource();
                return new
                {
                    success = false,
                    data = x,
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            callResource Result = new callResource();
            return new
            {
                success = true,
                data = Result,
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }
        [HttpPost("uploadSignatureManager")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพลานเซ็นของ Manager หรือ ",
        Description = "ลบข้อมูล T_resource จาก Database ของ Qis-SYnc")]
        public async Task<object> uploadSignatureManager([FromForm] ParamUploadImageCusSignature data)
        {
            int a = 0;
            //List<TResource> TresourceData = JsonConvert.DeserializeObject<List<TResource>>(data.Resource);
            //string StroageID = base._appSetting.StorageServerId.ToString();

            //com.apthai.QISAPINETCore.Model.QIS.MStorageServer StorageData = _ResourceRepo.GetStorageServerDetail(StroageID);

            //var UploadLoctaion = StorageData.StoragePhysicalPath;
            int SuccessUploadCount = 0;
            int count = 0;
            minio = new MinioServices();
            callResource callResourceDate = new callResource();
            if (data.Files != null)
            {
                var path = $"{data.ProjectCode}/{data.UnitNo}/Inkpad";
                var fileName = data.Files.FileName;
                var resultMinio = await minio.UploadFile(data.Files, path, fileName);
                long size = data.Files.Length;
                callResourceDate.FilePath = $"{path}/{fileName}";
                callResourceDate.FileLength = size;
                callResourceDate.CreateDate = DateTime.Now;
                callResourceDate.RowState = "Original";
                callResourceDate.ResourceType = 1;
                callResourceDate.ResourceTagSubCode = "1";
                if (data.IsBF == true)
                {
                    callResourceDate.ResourceTagCode = "CON-MGR-BF";
                }
                else
                {
                    callResourceDate.ResourceTagCode = "CON-MGR-AF";
                }
                callResourceDate.ResourceGroupSet = null;
                callResourceDate.ResourceGroupOrder = 0;
                callResourceDate.TDefectDetailId = 0;
                callResourceDate.TDefectId = data.TDefectID == "" ? 0 : Convert.ToInt32(data.TDefectID);
                callResourceDate.ProjectNo = data.ProjectCode;
                callResourceDate.SerialNo = data.UnitNo;
                callResourceDate.Active = true;
                callResourceDate.StorageServerId = 1400;
                callResourceDate.FullFilePath = resultMinio.Url;
                callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6); ;
                //TresourceData[i].FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                //TresourceData[i].FileLength = size;
                //TresourceData[i].CreatedDate = DateTime.Now;
                //TresourceData[i].CreateUserId = Convert.ToInt32(data.UserID);
                //TresourceData[i].RowSyncDate = DateTime.Now;
                //TresourceData[i].StorageServerId = StorageData.StorageServerId;
                bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);

                //if (InsertResult == true)
                //{
                //SuccessUploadCount++;
                //Model.QIS.TResource TresourceTransfer = new Model.QIS.TResource();
                //TresourceTransfer.ResourceType = TresourceData[i].ResourceType;
                //TresourceTransfer.ResourceTagCode = TresourceData[i].ResourceTagCode;
                //TresourceTransfer.ResourceTagSubCode = TresourceData[i].ResourceTagSubCode;
                //TresourceTransfer.ResourceGroupSet = TresourceData[i].ResourceGroupSet;
                //TresourceTransfer.ResourceGroupOrder = TresourceData[i].ResourceGroupOrder;
                //TresourceTransfer.ResourceMineType = TresourceData[i].ResourceMineType;
                //TresourceTransfer.ProjectId = TresourceData[i].ProjectId;
                //TresourceTransfer.UnitId = TresourceData[i].UnitId;
                ////TresourceTransfer.FilePath = _appSetting.PictureRootURL + TresourceData[i].FilePath;
                //TresourceTransfer.FilePath = Environment.GetEnvironmentVariable("PictureRootURL") + TresourceData[i].FilePath;
                //TresourceTransfer.Description = TresourceData[i].Description;
                //TresourceTransfer.IsActive = TresourceData[i].IsActive;
                //TresourceTransfer.StorageServerId = TresourceData[i].StorageServerId;
                //TresourceTransfer.PhaseId = TresourceData[i].PhaseId;
                //TresourceTransfer.HeaderId = TresourceData[i].HeaderId;
                //TresourceTransfer.DetailId = TresourceData[i].DetailId;
                //TresourceTransfer.UDetailId = TresourceData[i].UDetailId;
                //TresourceTransfer.UDetail_RowClientId = TresourceData[i].UDetail_RowClientId;
                //TresourceTransfer.Tag = TresourceData[i].Tag;
                //TresourceTransfer.RowClientId = TresourceData[i].RowClientId;
                //TresourceTransfer.RowState = TresourceData[i].RowState;
                //TresourceTransfer.RowVersion = TresourceData[i].RowVersion;
                //TresourceTransfer.RowSyncDate = TresourceData[i].RowSyncDate;
                //TresourceTransfer.CreateDeviceId = TresourceData[i].CreateDeviceId;
                //TresourceTransfer.CreateUserId = TresourceData[i].CreateUserId;
                //TresourceTransfer.ModifiedDeviceId = TresourceData[i].ModifiedDeviceId;
                //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                //TresourceTransfer.CreatedDate = TresourceData[i].CreatedDate;
                //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                //TresourceTransfer.UFHeaderId = TresourceData[i].UFHeaderId;
                //TresourceTransfer.UHeaderId = TresourceData[i].UHeaderId;
                //TresourceTransfer.UFDetailId = TresourceData[i].UFDetailID;
                //TresourceTransfer.FileLength = TresourceData[i].FileLength;
                //TresourceTransfer.UPhaseId = TresourceData[i].UPhaseId;
                //TresourceTransfer.UPhase_RowClientId = TresourceData[i].UPhase_RowClientId;
                //TresourceTransfer.UHeader_RowClientId = TresourceData[i].UHeader_RowClientId;
                //TresourceTransfer.UFPhase_RowClientId = TresourceData[i].UFPhase_RowClientId;
                //TresourceTransfer.UFHeader_RowClientId = TresourceData[i].UFHeader_RowClientId;
                //TresourceTransfer.UFDetail_RowClientId = TresourceData[i].UFDetail_RowClientId;
                //TresourceTransfer.TagState = TresourceData[i].TagState;
                //TresourceTransfer.ClientDataType = TresourceData[i].ClientDataType;

                //bool InsertResultWeb = _ResourceRepo.InsertTResourceWeb(TresourceTransfer);
                //}
            }
            else
            {
                callResource x = new callResource();
                return new
                {
                    success = false,
                    data = x,
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            callResource Result = new callResource();
            return new
            {
                success = true,
                data = Result,
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost("uploadReceiveSignature")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพของลายเซ็นลูกค้าเวลาเซ็นรับบ้าน",
        Description = "ลบข้อมูล T_resource จาก Database ของ Qis-SYnc")]
        public async Task<object> uploadReceiveSignature([FromForm] ParamUploadImageCusSignature data)
        {
            int SuccessUploadCount = 0;
            int count = 0;
            var pathUrlSig = "";
            var reusult = "";
            callResource callResourceDate = new callResource();
            //List<callTDefectDetail> callTDefectDetails = _masterRepository.GetcallTDefectDetailStatusNotClodeByTDefectIDList_Sync(Convert.ToInt32(data.TDefectID));
            //if (callTDefectDetails.Count > 0)
            //{
            //    return new
            //    {
            //        success = false,
            //        data = callResourceDate,
            //        message = string.Format(" Upload File Fail Error : Cannot Sign if all Work didn't Close ")
            //    };
            //}
            minio = new MinioServices();
            if (data.Files != null)
            {
                var path = $"{data.ProjectCode}/{data.UnitNo}/Inkpad";
                var fileName = data.Files.FileName;
                var resultMinio = await minio.UploadFile(data.Files, path, fileName);
                long size = data.Files.Length;

                callResourceDate.FilePath = $"{path}/{fileName}";
                callResourceDate.FileLength = size;
                callResourceDate.CreateDate = DateTime.Now;
                callResourceDate.RowState = "Original";
                callResourceDate.ResourceType = 6;
                callResourceDate.ResourceTagSubCode = "1";
                callResourceDate.ResourceTagCode = "CUST-RECE";
                callResourceDate.ResourceMineType = data.Files.ContentType;
                callResourceDate.ResourceGroupSet = null;
                callResourceDate.ResourceGroupOrder = 0;
                callResourceDate.TDefectDetailId = 0;
                callResourceDate.TDefectId = data.TDefectID == "" ? 0 : Convert.ToInt32(data.TDefectID);
                callResourceDate.ProjectNo = data.ProjectCode;
                callResourceDate.SerialNo = data.UnitNo;
                callResourceDate.Active = true;
                callResourceDate.StorageServerId = 1400;
                callResourceDate.FullFilePath = resultMinio.Url;
                callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6); ;
                bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);
                callTDefect defectModel = _masterRepository.GetCallTDefectByTDefectId_Sync(data.TDefectID);
                if (defectModel == null)
                {
                    ViewUnitCustomer viewUnitCustomer = _masterRepository.GetViewUnitCustomer(data.UnitNo, data.ProjectCode);
                    callTDefect CreateDefect = new callTDefect();
                    CreateDefect.RowState = "Original";
                    CreateDefect.RowActive = true;
                    CreateDefect.Client_Id = "Defect-" + data.DefectType + "-" + data.ProjectCode + "-" + data.UnitNo + "-" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "") + Guid.NewGuid();
                    CreateDefect.Client_SyncDate = DateTime.Now;
                    CreateDefect.TDefectDocNo = "Defect-" + data.DefectType + "-" + data.ProjectCode + "-" + data.UnitNo + "/" +
                                            DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss.ffffff").Replace(" ", "");
                    CreateDefect.TDefectStatus = "003"; // หน้าจะเท่ากับ Open
                    CreateDefect.TDefectSubStatus = null;
                    CreateDefect.ProductId = data.ProjectCode;
                    CreateDefect.ItemId = data.UnitNo;
                    CreateDefect.DeviceId = data.DeviceID;
                    CreateDefect.CreateUserId = data.EmpCode;
                    CreateDefect.UpdateUserId = null;
                    CreateDefect.CustRoundAuditNo_Rn = 1;
                    CreateDefect.CustRoundAuditDate_Last = DateTime.Now;
                    CreateDefect.CustRoundAudit_JsonLog = null;
                    CreateDefect.CreateDate = DateTime.Now;
                    CreateDefect.UpdateDate = null;
                    CreateDefect.Desciption = "0 Defect";
                    CreateDefect.DocOpenDate = DateTime.Now;
                    CreateDefect.DocDueCloseDate = DateTime.Now.AddDays(14);
                    CreateDefect.MechanicId = null;
                    CreateDefect.MechanicName = null;
                    CreateDefect.SellerId = null;
                    CreateDefect.SallerName = null;
                    CreateDefect.DocReceiveUnitDate = DateTime.Now;
                    //CreateDefect.DocDueTransferDate = DateTime.Now;
                    CreateDefect.ContactID = null;
                    if (viewUnitCustomer != null)
                    {
                        CreateDefect.ContactName = viewUnitCustomer.FirstName + "  " + viewUnitCustomer.LastName;
                    }
                    else
                    {
                        CreateDefect.ContactName = "";
                    }
                    CreateDefect.DocIsActive = true;
                    CreateDefect.DocIsExternalAudit = false;
                    CreateDefect.DocIsReqUnitReceiveAttachFile = false;
                    long DefectID = 0;
                    bool InsertData = _transactionRepository.InsertTdefectDetail(CreateDefect, ref DefectID);
                    CreateDefect.TDefectId = Convert.ToInt32(DefectID);
                }
                else
                {
                    defectModel.DocReceiveUnitDate = DateTime.Now;
                    defectModel.TDefectStatus = "003";
                    bool update = _transactionRepository.UpdateTdefect(defectModel);
                }
                pathUrlSig = callResourceDate.FilePath;
                await Task.Run(() => BackgroundJob.Enqueue(() => _syncRepository.GenerateReport(new ParamReportModel()
                {
                    ProjectCode = data.ProjectCode,
                    UnitNo = data.UnitNo,
                    TDefectId = Int32.Parse(data.TDefectID),
                    ProjectType = data.ProjectType,
                    SignatureType = callResourceDate.ResourceTagCode
                })));
            }
            else
            {
                return new
                {
                    success = false,
                    data = pathUrlSig,
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }

            if (String.IsNullOrEmpty(pathUrlSig))
            {
                return new
                {
                    success = false,
                    data = pathUrlSig,
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }

            return new
            {
                success = true,
                data = reusult,
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost]
        [Route("DefectUploadedList")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "ดึงข้อมูล รุปภาพ ของรายการ DefectDetail ด้วย DefectDetailID",
        Description = "ดึงข้อมูล รุปภาพทั้งหมด ของรายการ DefectDetail ด้วย DefectDetailID แยก Type ด้วย ResourceTagCode")]
        public async Task<object> DefectUploadList([FromForm] GetDefectUploadListParam data)
        {
            try
            {
                callTDefectDetail callTDefectDetail = _masterRepository.GetcallTDefectDetailByDetailID_Sync(data.TDefectDetailID);

                if (callTDefectDetail != null)
                {
                    List<callResource> callResources = _masterRepository.GetCallResourceByTdefectDetailID(callTDefectDetail.TDefectDetailId);
                    var Json = JsonConvert.SerializeObject(callResources);
                    CallresouceWithURL callresouceWithURL = new CallresouceWithURL();
                    callresouceWithURL = JsonConvert.DeserializeObject<CallresouceWithURL>(Json);
                    callresouceWithURL.URL = Environment.GetEnvironmentVariable("BaseURL") + "/" + callresouceWithURL.FilePath;
                    callresouceWithURL.URL = ReplaceWithPublicURL(callresouceWithURL.URL);
                    return new
                    {
                        success = true,
                        data = callresouceWithURL
                    };
                }
                else
                {
                    return new
                    {
                        success = true,
                        data = new callTDefectDetail()
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("uploadAfterPictureList")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
        Description = "Upload รูปภาพของรายการ TDefectDetail After แบบหลายๆรูปพร้อมกัน")]
        public async Task<object> uploadAfterPictureList([FromForm] ParamUploadImageAfterList data)
        {
            int SuccessUploadCount = 0;
            var path = $"{data.ProjectCode}/{data.UnitNo}/After";
            minio = new MinioServices();
            foreach (var elFile in data.Files)
            {
                if (elFile != null)
                {
                    var fileName = elFile.FileName;
                    var resultMinio = await minio.UploadFile(elFile, path, fileName);
                    long size = elFile.Length;

                    callResource callResourceDate = new callResource();
                    callResourceDate.FilePath = $"{path}/{fileName}";
                    callResourceDate.FileLength = size;
                    callResourceDate.CreateDate = DateTime.Now;
                    callResourceDate.RowState = "Original";
                    callResourceDate.ResourceType = 7;
                    callResourceDate.ResourceTagCode = "AF-RP";
                    callResourceDate.ResourceGroupSet = null;
                    callResourceDate.ResourceGroupOrder = 0;
                    callResourceDate.TDefectDetailId = data.TDefectDetailId == "" ? 0 : Convert.ToInt32(data.TDefectDetailId);
                    callResourceDate.ProjectNo = data.ProjectCode;
                    callResourceDate.SerialNo = data.UnitNo;
                    callResourceDate.Active = true;
                    callResourceDate.StorageServerId = 1400;
                    callResourceDate.FullFilePath = resultMinio.Url;
                    callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6);
                    bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);
                }
                else
                {
                    callResource x = new callResource();
                    return new
                    {
                        success = false,
                        data = x,
                        message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                    };
                }
            }
            callResource Result = new callResource();
            return new
            {
                success = true,
                data = Result,
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }


        [HttpPost]
        [Route("uploadFloorPlanPicture")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
        Description = "Upload รูปภาพของรายการ TDefectDetail After แบบหลายๆรูปพร้อมกัน")]
        public async Task<object> uploadFloorPlanPicture([FromForm] ParamUploadFloorPlan data)
        {
            int a = 0;
            //List<TResource> TresourceData = JsonConvert.DeserializeObject<List<TResource>>(data.Resource);
            //string StroageID = base._appSetting.StorageServerId.ToString();

            //com.apthai.QISAPINETCore.Model.QIS.MStorageServer StorageData = _ResourceRepo.GetStorageServerDetail(StroageID);

            //var UploadLoctaion = StorageData.StoragePhysicalPath;
            int SuccessUploadCount = 0;
            int count = 0;
            var path = $"{data.ProjectCode}/{data.UnitNo}/FloorPlan";
            minio = new MinioServices();
            foreach (var elFile in data.Files)
            {
                if (elFile != null)
                {
                    var fileName = elFile.FileName;
                    var resultMinio = await minio.UploadFile(elFile, path, fileName);
                    long size = elFile.Length;

                    callResource callResourceDate = new callResource();
                    callResourceDate.FilePath = $"{path}/{fileName}";
                    callResourceDate.FileLength = size;
                    callResourceDate.CreateDate = DateTime.Now;
                    callResourceDate.RowState = "Original";
                    callResourceDate.ResourceType = 9;
                    callResourceDate.ResourceTagCode = data.Floor;
                    callResourceDate.ResourceGroupSet = null;
                    callResourceDate.ResourceGroupOrder = 0;
                    callResourceDate.TDefectDetailId = data.TDefectDetailId == "" ? 0 : Convert.ToInt32(data.TDefectDetailId);
                    callResourceDate.ProjectNo = data.ProjectCode;
                    callResourceDate.SerialNo = data.UnitNo;
                    callResourceDate.Active = true;
                    callResourceDate.StorageServerId = 1400;
                    callResourceDate.TDefectId = data.TDefectID == "" ? 0 : Convert.ToInt32(data.TDefectID);
                    callResourceDate.FullFilePath = resultMinio.Url;
                    callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6); ;
                    //TresourceData[i].FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                    //TresourceData[i].FileLength = size;
                    //TresourceData[i].CreatedDate = DateTime.Now;
                    //TresourceData[i].CreateUserId = Convert.ToInt32(data.UserID);
                    //TresourceData[i].RowSyncDate = DateTime.Now;
                    //TresourceData[i].StorageServerId = StorageData.StorageServerId;
                    bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);

                    //if (InsertResult == true)
                    //{
                    //SuccessUploadCount++;
                    //Model.QIS.TResource TresourceTransfer = new Model.QIS.TResource();
                    //TresourceTransfer.ResourceType = TresourceData[i].ResourceType;
                    //TresourceTransfer.ResourceTagCode = TresourceData[i].ResourceTagCode;
                    //TresourceTransfer.ResourceTagSubCode = TresourceData[i].ResourceTagSubCode;
                    //TresourceTransfer.ResourceGroupSet = TresourceData[i].ResourceGroupSet;
                    //TresourceTransfer.ResourceGroupOrder = TresourceData[i].ResourceGroupOrder;
                    //TresourceTransfer.ResourceMineType = TresourceData[i].ResourceMineType;
                    //TresourceTransfer.ProjectId = TresourceData[i].ProjectId;
                    //TresourceTransfer.UnitId = TresourceData[i].UnitId;
                    ////TresourceTransfer.FilePath = _appSetting.PictureRootURL + TresourceData[i].FilePath;
                    //TresourceTransfer.FilePath = Environment.GetEnvironmentVariable("PictureRootURL") + TresourceData[i].FilePath;
                    //TresourceTransfer.Description = TresourceData[i].Description;
                    //TresourceTransfer.IsActive = TresourceData[i].IsActive;
                    //TresourceTransfer.StorageServerId = TresourceData[i].StorageServerId;
                    //TresourceTransfer.PhaseId = TresourceData[i].PhaseId;
                    //TresourceTransfer.HeaderId = TresourceData[i].HeaderId;
                    //TresourceTransfer.DetailId = TresourceData[i].DetailId;
                    //TresourceTransfer.UDetailId = TresourceData[i].UDetailId;
                    //TresourceTransfer.UDetail_RowClientId = TresourceData[i].UDetail_RowClientId;
                    //TresourceTransfer.Tag = TresourceData[i].Tag;
                    //TresourceTransfer.RowClientId = TresourceData[i].RowClientId;
                    //TresourceTransfer.RowState = TresourceData[i].RowState;
                    //TresourceTransfer.RowVersion = TresourceData[i].RowVersion;
                    //TresourceTransfer.RowSyncDate = TresourceData[i].RowSyncDate;
                    //TresourceTransfer.CreateDeviceId = TresourceData[i].CreateDeviceId;
                    //TresourceTransfer.CreateUserId = TresourceData[i].CreateUserId;
                    //TresourceTransfer.ModifiedDeviceId = TresourceData[i].ModifiedDeviceId;
                    //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                    //TresourceTransfer.CreatedDate = TresourceData[i].CreatedDate;
                    //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                    //TresourceTransfer.UFHeaderId = TresourceData[i].UFHeaderId;
                    //TresourceTransfer.UHeaderId = TresourceData[i].UHeaderId;
                    //TresourceTransfer.UFDetailId = TresourceData[i].UFDetailID;
                    //TresourceTransfer.FileLength = TresourceData[i].FileLength;
                    //TresourceTransfer.UPhaseId = TresourceData[i].UPhaseId;
                    //TresourceTransfer.UPhase_RowClientId = TresourceData[i].UPhase_RowClientId;
                    //TresourceTransfer.UHeader_RowClientId = TresourceData[i].UHeader_RowClientId;
                    //TresourceTransfer.UFPhase_RowClientId = TresourceData[i].UFPhase_RowClientId;
                    //TresourceTransfer.UFHeader_RowClientId = TresourceData[i].UFHeader_RowClientId;
                    //TresourceTransfer.UFDetail_RowClientId = TresourceData[i].UFDetail_RowClientId;
                    //TresourceTransfer.TagState = TresourceData[i].TagState;
                    //TresourceTransfer.ClientDataType = TresourceData[i].ClientDataType;

                    //bool InsertResultWeb = _ResourceRepo.InsertTResourceWeb(TresourceTransfer);
                    //}                       
                }
                else
                {
                    callResource x = new callResource();
                    return new
                    {
                        success = false,
                        data = x,
                        message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                    };
                }
            }
            callResource Result = new callResource();
            return new
            {
                success = true,
                data = Result,
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost]
        [Route("uploadBeforePictureList")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
            Description = "Upload รูปภาพของรายการ TDefectDetail Before แบบหลายๆรูปพร้อมกัน")]
        public async Task<object> uploadBeforePictureList([FromForm] ParamUploadImageAfterList data)
        {
            int SuccessUploadCount = 0;
            var path = $"{data.ProjectCode}/{data.UnitNo}/Before";
            minio = new MinioServices();
            foreach (var elFile in data.Files)
            {
                if (elFile != null)
                {
                    var fileName = elFile.FileName;
                    var resultMinio = await minio.UploadFile(elFile, path, fileName);
                    long size = elFile.Length;

                    callResource callResourceDate = new callResource();
                    callResourceDate.FilePath = $"{path}/{fileName}";
                    callResourceDate.FileLength = size;
                    callResourceDate.CreateDate = DateTime.Now;
                    callResourceDate.RowState = "Original";
                    callResourceDate.ResourceType = 7;
                    callResourceDate.ResourceTagCode = "BF-RP";
                    callResourceDate.ResourceGroupSet = null;
                    callResourceDate.ResourceGroupOrder = 0;
                    callResourceDate.TDefectDetailId = data.TDefectDetailId == "" ? 0 : Convert.ToInt32(data.TDefectDetailId);
                    callResourceDate.ProjectNo = data.ProjectCode;
                    callResourceDate.SerialNo = data.UnitNo;
                    callResourceDate.Active = true;
                    callResourceDate.FullFilePath = resultMinio.Url;
                    callResourceDate.StorageServerId = 1400;
                    callResourceDate.ExpirePathDate = DateTime.Now.AddDays(6); ;
                    bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);
                }
                else
                {
                    callResource x = new callResource();
                    return new
                    {
                        success = false,
                        data = x,
                        message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                    };
                }
            }
            callResource Result = new callResource();
            return new
            {
                success = true,
                data = Result,
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost]
        [Route("DefectReport")]
        [SwaggerOperation(Summary = "Log In เข้าสู้ระบบเพื่อรับ Access Key ",
        Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> DefectReport([FromBody] GenerateReportByTDefectId data)
        {
            try
            {
                //http://192.168.0.209/ReportPortalViewer_API/api/CrystalReport/GenPDF
                var client = new HttpClient();
                var Content = new StringContent(JsonConvert.SerializeObject(data));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //Content.Headers.Add("api_key", APApiKey);
                //Content.Headers.Add("api_token", APApiToken);
                string PostURL = Environment.GetEnvironmentVariable("ReportURL");
                string ReportKey = Environment.GetEnvironmentVariable("ReportKey");
                //PostURL = PostURL + "JWTUserLogin";
                Content.Headers.Add("api_accesskey", ReportKey);
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
                return new
                {
                    success = false,
                    data = new AutorizeDataJWT(),
                    valid = false
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public string ReplaceWithPublicURL(string url)
        {
            //string _minioEndpoint = "http://192.168.2.29:9001";
            string _minioEndpoint = Environment.GetEnvironmentVariable("Minio_Endpoint");
            if (_minioEndpoint == null)
            {
                _minioEndpoint = UtilsProvider.AppSetting.MinioEndpoint;
            }
            //string _tempBucket = "timeattendence";
            string PublicMinioURL = Environment.GetEnvironmentVariable("MinioPublicEndpoint");
            if (PublicMinioURL == null)
            {
                PublicMinioURL = UtilsProvider.AppSetting.MinioPublicEndpoint;
            }
            if (!string.IsNullOrEmpty(PublicMinioURL))
            {
                url = url.Replace("https://", "");
                url = url.Replace("http://", "");

                url = url.Replace(_minioEndpoint, PublicMinioURL);
            }
            return url;
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
        [HttpPost("GenerateReport")]
        public async Task GenerateReport([FromBody] ParamReportModel model)
        {
            try
            {
                await Task.Run(() => BackgroundJob.Enqueue(() => _syncRepository.GenerateReport(model)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
    }
}

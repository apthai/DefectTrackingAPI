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
                    tDefectDetail.CallPointId = data.Cate;
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

                    TDefectDetailWithStatus Returnobj = new TDefectDetailWithStatus();
                    if (inserttdefectdetail == 1)
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
                            // -- New ---- for Docker
                            var yearPath = DateTime.Now.Year;
                            var MonthPath = DateTime.Now.Month;
                            var dirPath1 = $"{yearPath}/{MonthPath}";
                            int dataPath = 0;
                            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "data", "uploads", dirPath1);

                            string FileBinary;


                            long size = data.Files.Length;
                            string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                            var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                            var dirPath = $"{yearPath}\\M{dataPath}";

                            // --- New Docker ----
                            if (!Directory.Exists(uploads))
                            {
                                Directory.CreateDirectory(uploads);
                            }
                            if (data.Files.Length > 0)
                            {
                                var filePath = Path.Combine(uploads, data.Files.FileName);
                                var message = "";
                                if (System.IO.File.Exists(filePath))
                                {
                                    string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                                    string extension = Path.GetExtension(filePath);
                                    string path = Path.GetDirectoryName(filePath);
                                    string newFullPath = filePath;
                                    string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                                    newFullPath = Path.Combine(path, tempFileName + extension);
                                    using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                                    {
                                        message = data.Files.Length.ToString();
                                        await data.Files.CopyToAsync(fileStream);
                                        fileName = tempFileName + extension;

                                    }
                                }
                                else
                                {
                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                                    {
                                        message = data.Files.Length.ToString();
                                        await data.Files.CopyToAsync(fileStream);
                                    }
                                }

                                // -- -End New -----
                                //if (System.IO.File.Exists(savePath.FullName))
                                if (System.IO.File.Exists(filePath))
                                {
                                    string FileExtention = Path.GetExtension(filePath);
                                    // ----- Old -----
                                    //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                                    // ----- New Docker -----
                                    callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                                    callResourceDate.FileLength = size;
                                    callResourceDate.CreateDate = DateTime.Now;
                                    callResourceDate.RowState = "Original";
                                    callResourceDate.ResourceType = 3;
                                    callResourceDate.ResourceTagCode = "BF-RP";
                                    callResourceDate.ResourceGroupSet = null;
                                    callResourceDate.ResourceGroupOrder = 0;
                                    callResourceDate.TDefectDetailId = Convert.ToInt32(inserttdefectdetail);
                                    callResourceDate.ProjectNo = data.ProductId;
                                    callResourceDate.SerialNo = data.ItemId;
                                    callResourceDate.Active = true;
                                    callResourceDate.RowState = "AddNew";
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

                        }
                        else
                        {
                            return new
                            {
                                success = true,
                                data = tDefectDetail,
                                message = string.Format("Create Defect Detail Success!")
                            };
                        }

                    }

                    return new
                    {
                        success = true,
                        data = Returnobj
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
                    tDefectDetail.CallPointId = data.Cate;
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

                    if (inserttdefectdetail != 0)
                    {
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


                            long size = data.Files.Length;
                            string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                            var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                            var dirPath = $"{yearPath}\\M{dataPath}";

                            // --- New Docker ----
                            if (!Directory.Exists(uploads))
                            {
                                Directory.CreateDirectory(uploads);
                            }
                            if (data.Files.Length > 0)
                            {
                                var filePath = Path.Combine(uploads, data.Files.FileName);
                                var message = "";
                                if (System.IO.File.Exists(filePath))
                                {
                                    string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                                    string extension = Path.GetExtension(filePath);
                                    string path = Path.GetDirectoryName(filePath);
                                    string newFullPath = filePath;
                                    string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                                    newFullPath = Path.Combine(path, tempFileName + extension);
                                    using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                                    {
                                        message = data.Files.Length.ToString();
                                        await data.Files.CopyToAsync(fileStream);
                                        fileName = tempFileName + extension;

                                    }
                                }
                                else
                                {
                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                                    {
                                        message = data.Files.Length.ToString();
                                        await data.Files.CopyToAsync(fileStream);
                                    }
                                }

                                // -- -End New -----
                                //if (System.IO.File.Exists(savePath.FullName))
                                if (System.IO.File.Exists(filePath))
                                {
                                    string FileExtention = Path.GetExtension(filePath);
                                    // ----- Old -----
                                    //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                                    // ----- New Docker -----
                                    callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                                    callResourceDate.FileLength = size;
                                    callResourceDate.CreateDate = DateTime.Now;
                                    callResourceDate.RowState = "Original";
                                    callResourceDate.ResourceType = 2;
                                    callResourceDate.ResourceTagCode = "BF-RP";
                                    callResourceDate.ResourceGroupSet = null;
                                    callResourceDate.ResourceGroupOrder = 0;
                                    callResourceDate.TDefectDetailId = Convert.ToInt32(inserttdefectdetail);
                                    callResourceDate.ProjectNo = data.ProductId;
                                    callResourceDate.SerialNo = data.ItemId;
                                    callResourceDate.Active = true;
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

                        }
                        else
                        {
                            return new
                            {
                                success = true,
                                data = CreateDefect,
                                message = string.Format("Create Defect Detail Success!")
                            };
                        }
                    }

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
        [Route("UpdateDefectDetailStatusFinish")]
        [Consumes("multipart/form-data")]
        public async Task<object> UpdateDefectDetailStatusFinish([FromForm] UpdateDefectDetailID data)
        {
            try
            {

                callTDefectDetail callTDefectDetail = _masterRepository.GetcallTDefectDetailByDetailID_Sync(data.TDefectDetailID);
                // --------------------------------------------------------------------
                callTDefectDetail.TDefectDetailStatus = "003";

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
        [Route("UpdateDefectDetailStatusClose")]
        [Consumes("multipart/form-data")]
        public async Task<object> UpdateDefectDetailStatusClose([FromForm] UpdateDefectDetailID data)
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
        [Route("CreateDefectListTransaction")]
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
                    CreateDefect.DocIsExternalAudit = false;
                    CreateDefect.DocIsReqUnitReceiveAttachFile = false;
                    long DefectID = 0;
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


        //            return new
        //            {
        //                success = true,
        //                data = new { MappingPlanImages = tempUploadFiles },
        //                // data = _data.ToList(),
        //                //message = string.Format("found {0} items", _data.Count)
        //            };

        //        }


        //        //var dataResult = await _unitOfWork.MasterRepository.GetUnitSearch(data.ProjectId, data.SearchText);
        //        //var _data = dataResult.ToList();


        //        return new
        //        {
        //            success = false,
        //            message = "missing upload file."
        //        };



        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, "Internal server error :: " + ex.Message);
        //    }

        //}

        [HttpPost("uploadBeforePicture")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
       Description = "ลบข้อมูล T_resource จาก Database ของ Qis-SYnc")]
        public async Task<object> uploadPicture([FromForm] ParamUploadImageBefore data)
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


                long size = data.Files.Length;
                string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                var dirPath = $"{yearPath}\\M{dataPath}";

                // --- New Docker ----
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                if (data.Files.Length > 0)
                {
                    var filePath = Path.Combine(uploads, data.Files.FileName);
                    var message = "";
                    if (System.IO.File.Exists(filePath))
                    {
                        string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath);
                        string path = Path.GetDirectoryName(filePath);
                        string newFullPath = filePath;
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                        newFullPath = Path.Combine(path, tempFileName + extension);
                        using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                            fileName = tempFileName + extension;

                        }
                    }
                    else
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                        }
                    }

                    // -- -End New -----
                    //if (System.IO.File.Exists(savePath.FullName))
                    if (System.IO.File.Exists(filePath))
                    {
                        string FileExtention = Path.GetExtension(filePath);
                        // ----- Old -----
                        //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                        // ----- New Docker -----
                        callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
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

            }
            else
            {
                return new
                {
                    success = false,
                    data = "[]",
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            return new
            {
                success = true,
                data = "[]",
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost("uploadAfterPicture")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
       Description = "ลบข้อมูล T_resource จาก Database ของ Qis-SYnc")]
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


                long size = data.Files.Length;
                string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                var dirPath = $"{yearPath}\\M{dataPath}";

                // --- New Docker ----
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                if (data.Files.Length > 0)
                {
                    var filePath = Path.Combine(uploads, data.Files.FileName);
                    var message = "";
                    if (System.IO.File.Exists(filePath))
                    {
                        string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath);
                        string path = Path.GetDirectoryName(filePath);
                        string newFullPath = filePath;
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                        newFullPath = Path.Combine(path, tempFileName + extension);
                        using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                            fileName = tempFileName + extension;

                        }
                    }
                    else
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                        }
                    }

                    // -- -End New -----
                    //if (System.IO.File.Exists(savePath.FullName))
                    if (System.IO.File.Exists(filePath))
                    {
                        string FileExtention = Path.GetExtension(filePath);
                        // ----- Old -----
                        //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                        // ----- New Docker -----
                        callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
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

            }
            else
            {
                return new
                {
                    success = false,
                    data = "[]",
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            return new
            {
                success = true,
                data = "[]",
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost("uploadPDF")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
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


                long size = data.Files.Length;
                string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                var dirPath = $"{yearPath}\\M{dataPath}";

                // --- New Docker ----
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                if (data.Files.Length > 0)
                {
                    var filePath = Path.Combine(uploads, data.Files.FileName);
                    var message = "";
                    if (System.IO.File.Exists(filePath))
                    {
                        string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath);
                        string path = Path.GetDirectoryName(filePath);
                        string newFullPath = filePath;
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                        newFullPath = Path.Combine(path, tempFileName + extension);
                        using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                            fileName = tempFileName + extension;

                        }
                    }
                    else
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                        }
                    }

                    // -- -End New -----
                    //if (System.IO.File.Exists(savePath.FullName))
                    if (System.IO.File.Exists(filePath))
                    {
                        string FileExtention = Path.GetExtension(filePath);
                        // ----- Old -----
                        //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                        // ----- New Docker -----
                        callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                        callResourceDate.FileLength = size;
                        callResourceDate.CreateDate = DateTime.Now;
                        callResourceDate.RowState = "Original";
                        callResourceDate.ResourceType = data.ResourceType;
                        callResourceDate.ResourceTagCode = data.ResourceTagCode;
                        callResourceDate.ResourceGroupSet = null;
                        callResourceDate.ResourceGroupOrder = 0;
                        callResourceDate.TDefectDetailId = data.TDefectDetailId == "" ? 0 : Convert.ToInt32(data.TDefectDetailId);
                        callResourceDate.ProjectNo = data.ProjectCode;
                        callResourceDate.SerialNo = data.UnitNo;
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
            }
            else
            {
                return new
                {
                    success = false,
                    data = "[]",
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            return new
            {
                success = true,
                data = "[]",
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost("uploadDocCusNotSign")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
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


                long size = data.Files.Length;
                string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                var dirPath = $"{yearPath}\\M{dataPath}";

                // --- New Docker ----
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                if (data.Files.Length > 0)
                {
                    var filePath = Path.Combine(uploads, data.Files.FileName);
                    var message = "";
                    if (System.IO.File.Exists(filePath))
                    {
                        string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath);
                        string path = Path.GetDirectoryName(filePath);
                        string newFullPath = filePath;
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                        newFullPath = Path.Combine(path, tempFileName + extension);
                        using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                            fileName = tempFileName + extension;

                        }
                    }
                    else
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                        }
                    }

                    // -- -End New -----
                    //if (System.IO.File.Exists(savePath.FullName))
                    if (System.IO.File.Exists(filePath))
                    {
                        string FileExtention = Path.GetExtension(filePath);
                        // ----- Old -----
                        //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                        // ----- New Docker -----
                        callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
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

            }
            else
            {
                return new
                {
                    success = false,
                    data = "[]",
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            return new
            {
                success = true,
                data = "[]",
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost("uploadSignatureCus")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
Description = "ลบข้อมูล T_resource จาก Database ของ Qis-SYnc")]
        public async Task<object> uploadSignature([FromForm] ParamUploadImageCusSignature data)
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


                long size = data.Files.Length;
                string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                var dirPath = $"{yearPath}\\M{dataPath}";

                // --- New Docker ----
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                if (data.Files.Length > 0)
                {
                    var filePath = Path.Combine(uploads, data.Files.FileName);
                    var message = "";
                    if (System.IO.File.Exists(filePath))
                    {
                        string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath);
                        string path = Path.GetDirectoryName(filePath);
                        string newFullPath = filePath;
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                        newFullPath = Path.Combine(path, tempFileName + extension);
                        using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                            fileName = tempFileName + extension;

                        }
                    }
                    else
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                        }
                    }

                    // -- -End New -----
                    //if (System.IO.File.Exists(savePath.FullName))
                    if (System.IO.File.Exists(filePath))
                    {
                        string FileExtention = Path.GetExtension(filePath);
                        // ----- Old -----
                        //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                        // ----- New Docker -----
                        callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                        callResourceDate.FileLength = size;
                        callResourceDate.CreateDate = DateTime.Now;
                        callResourceDate.RowState = "Original";
                        callResourceDate.ResourceType = 1;
                        callResourceDate.ResourceTagSubCode = "1";
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

            }
            else
            {
                return new
                {
                    success = false,
                    data = "[]",
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            return new
            {
                success = true,
                data = "[]",
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }
        [HttpPost("uploadSignatureLC")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
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


                long size = data.Files.Length;
                string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                var dirPath = $"{yearPath}\\M{dataPath}";

                // --- New Docker ----
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                if (data.Files.Length > 0)
                {
                    var filePath = Path.Combine(uploads, data.Files.FileName);
                    var message = "";
                    if (System.IO.File.Exists(filePath))
                    {
                        string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath);
                        string path = Path.GetDirectoryName(filePath);
                        string newFullPath = filePath;
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                        newFullPath = Path.Combine(path, tempFileName + extension);
                        using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                            fileName = tempFileName + extension;

                        }
                    }
                    else
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                        }
                    }

                    // -- -End New -----
                    //if (System.IO.File.Exists(savePath.FullName))
                    if (System.IO.File.Exists(filePath))
                    {
                        string FileExtention = Path.GetExtension(filePath);
                        // ----- Old -----
                        //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                        // ----- New Docker -----
                        callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
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

            }
            else
            {
                return new
                {
                    success = false,
                    data = "[]",
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            return new
            {
                success = true,
                data = "[]",
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }
        [HttpPost("uploadSignatureManager")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
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


                long size = data.Files.Length;
                string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                var dirPath = $"{yearPath}\\M{dataPath}";

                // --- New Docker ----
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                if (data.Files.Length > 0)
                {
                    var filePath = Path.Combine(uploads, data.Files.FileName);
                    var message = "";
                    if (System.IO.File.Exists(filePath))
                    {
                        string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath);
                        string path = Path.GetDirectoryName(filePath);
                        string newFullPath = filePath;
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                        newFullPath = Path.Combine(path, tempFileName + extension);
                        using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                            fileName = tempFileName + extension;

                        }
                    }
                    else
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                        }
                    }

                    // -- -End New -----
                    //if (System.IO.File.Exists(savePath.FullName))
                    if (System.IO.File.Exists(filePath))
                    {
                        string FileExtention = Path.GetExtension(filePath);
                        // ----- Old -----
                        //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                        // ----- New Docker -----
                        callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
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

            }
            else
            {
                return new
                {
                    success = false,
                    data = "[]",
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            return new
            {
                success = true,
                data = "[]",
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost("uploadAcceptSignature")]
        [Consumes("multipart/form-data")] // บอก Swagger ว่าเป็น Multipath 
        [SwaggerOperation(Summary = "Uploadรูปภาพ หรือ ไฟล์ PDF",
Description = "ลบข้อมูล T_resource จาก Database ของ Qis-SYnc")]
        public async Task<object> uploadAcceptSignature([FromForm] ParamUploadImageCusSignature data)
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


                long size = data.Files.Length;
                string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                var dirPath = $"{yearPath}\\M{dataPath}";

                // --- New Docker ----
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                if (data.Files.Length > 0)
                {
                    var filePath = Path.Combine(uploads, data.Files.FileName);
                    var message = "";
                    if (System.IO.File.Exists(filePath))
                    {
                        string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath);
                        string path = Path.GetDirectoryName(filePath);
                        string newFullPath = filePath;
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                        newFullPath = Path.Combine(path, tempFileName + extension);
                        using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                            fileName = tempFileName + extension;
                        }
                    }
                    else
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            message = data.Files.Length.ToString();
                            await data.Files.CopyToAsync(fileStream);
                        }
                    }

                    // -- -End New -----
                    //if (System.IO.File.Exists(savePath.FullName))
                    if (System.IO.File.Exists(filePath))
                    {
                        string FileExtention = Path.GetExtension(filePath);
                        // ----- Old -----
                        //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                        // ----- New Docker -----
                        callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                        callResourceDate.FileLength = size;
                        callResourceDate.CreateDate = DateTime.Now;
                        callResourceDate.RowState = "Original";
                        callResourceDate.ResourceType = 2;
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

            }
            else
            {
                return new
                {
                    success = false,
                    data = "[]",
                    message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                };
            }
            return new
            {
                success = true,
                data = "[]",
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost]
        [Route("DefectUploadedList")]
        [Consumes("multipart/form-data")]
        public async Task<object> DefectUploadList([FromForm] GetDefectUploadListParam data)
        {
            try
            {

                callTDefectDetail callTDefectDetail = _masterRepository.GetcallTDefectDetailByDetailID_Sync(data.TDefectDetailID);

                if (callTDefectDetail != null)
                {
                    List<callResource> callResources = _masterRepository.GetCallResourceByTdefectDetailID(callTDefectDetail.TDefectDetailId);
                    return new
                    {
                        success = true,
                        data = callResources
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

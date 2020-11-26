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
using Swashbuckle.AspNetCore.Annotations;

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
        [Route("GetCallTypewithArea")]
        [SwaggerOperation(Summary = "ดึงข้อมูลบริเวณ ",
        Description = "ไม่ต้องส่ง อะไรมา ")]
        public async Task<object> GetCallTypewithArea([FromBody] GetCAllArea data)
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
                List<GetcallTypeWithArea> calltypes = _masterRepository.GetCallCallTypeWithArea_Sync();

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
        [Route("GetProjectInformationDetail")]
        public async Task<object> GetProjectInformationDetail([FromBody]GetProjectInformationDetail data)
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
                List<Tower> Tower = _masterRepository.GetTowersByProject(data.ProjectCode);
                ProjectObjList projectObjList = new ProjectObjList();
                projectObjList.ProjectList = new List<ProjectObj>();
                for (int i = 0; i < Tower.Count(); i++)
                {
                    ProjectObj projectObj = new ProjectObj();
                    projectObj.TowerID = Tower[i];
                    List<Floor> Floor = _masterRepository.GetFloorsByProjectTower(data.ProjectCode, Tower[i].TowerID);
                    projectObj.FloorID = Floor;
                    projectObjList.ProjectList.Add(projectObj);
                }
                

                return new
                {
                    success = true,
                    data = projectObjList
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }
        [HttpGet]
        [Route("GetFloor")]
        public async Task<object> GetFloorDistinct()
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
                List<point> points = _masterRepository.GetFloorDistinct();
                List<FloorObj> Floor = new List<FloorObj>();
                for (int i = 0; i < points.Count(); i++)
                {
                    FloorObj obj = new FloorObj();
                    obj.FloorName = points[i].floorplantset;
                    Floor.Add(obj);
                }
                return new
                {
                    success = true,
                    data = Floor
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
                if (data.IsRecent == true)
                {
                    List<GetUnitByProjectReturnObj> callTDefects = _masterRepository.GetRecentcallTDefect_Sync(data.EmpCode);
                    return new
                    {
                        success = true,
                        data = callTDefects
                    };
                }
                else
                {
                    List<GetUnitByProjectReturnObj> Units = _masterRepository.GetUnitByProduct(data.ProjectID, data.SearchText);

                    return new
                    {
                        success = true,
                        data = Units
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpGet]
        [Route("GetCallPointHorizontal")]
        public async Task<object> GetCallPointHorizontal()
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
                List<Point> points = _masterRepository.GetCallPointByProductCat_Sync("H");
                List<PointCamel> PointCamel = new List<PointCamel>();
                for (int i = 0; i < points.Count(); i++)
                {
                    PointCamel camels = new PointCamel();
                    camels.Cate = points[i].Cate;
                    camels.ChkMainPoint = points[i].Chkmainpoint;
                    camels.CompPointId = points[i].Comppoint_id;
                    camels.EndPoint = points[i].End_point;
                    camels.FloorPlantSet = points[i].Floorplantset;
                    camels.PointName = points[i].Point_name;
                    camels.ProductTypeCate = points[i].Producttypecate;
                    camels.Project = points[i].Project;
                    camels.SubPoint = points[i].Sub_point;
                    PointCamel.Add(camels);
                }
                return new
                {
                    success = true,
                    data = PointCamel
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }
        [HttpGet]
        [Route("GetCallPointVertical")]
        public async Task<object> GetCallPointVertical()
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
                List<Point> points = _masterRepository.GetCallPointByProductCat_Sync("V");
                List<PointCamel> PointCamel = new List<PointCamel>();
                for (int i = 0; i < points.Count(); i++)
                {
                    PointCamel camels = new PointCamel();
                    camels.Cate = points[i].Cate;
                    camels.ChkMainPoint = points[i].Chkmainpoint;
                    camels.CompPointId = points[i].Comppoint_id;
                    camels.EndPoint = points[i].End_point;
                    camels.FloorPlantSet = points[i].Floorplantset;
                    camels.PointName = points[i].Point_name;
                    camels.ProductTypeCate = points[i].Producttypecate;
                    camels.Project = points[i].Project;
                    camels.SubPoint = points[i].Sub_point;
                    PointCamel.Add(camels);
                }
                return new
                {
                    success = true,
                    data = PointCamel
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet]
        [Route("GetCallAreaHorizontal")]
        public async Task<object> GetCallAreaHorizontal()
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
                
                List<Callarea> callareas = _masterRepository.GetCallAreaByProductCat_Sync("H");
                List<CallareaCamel> callareaCamels = new List<CallareaCamel>();
                for (int i = 0; i < callareas.Count(); i++)
                {
                    CallareaCamel camel = new CallareaCamel();
                    camel.Active = callareas[i].Active;
                    camel.CallAreaId = callareas[i].Callarea_id;
                    camel.CallAreaName = callareas[i].Callarea_name;
                    camel.CallTypeId = callareas[i].Calltype_id;
                    camel.ChkType = callareas[i].Chk_type;
                    camel.ProductTypeCate = callareas[i].Producttypecate;
                    camel.Responsible = callareas[i].Responsible;
                    camel.Sequence = callareas[i].Sequence;
                    callareaCamels.Add(camel);
                }
                return new
                {
                    success = true,
                    data = callareaCamels
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet]
        [Route("GetCallAreaVertical")]
        public async Task<object> GetCallAreaVertical()
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

                List<Callarea> callareas = _masterRepository.GetCallAreaByProductCat_Sync("V");
                List<CallareaCamel> callareaCamels = new List<CallareaCamel>();
                for (int i = 0; i < callareas.Count(); i++)
                {
                    CallareaCamel camel = new CallareaCamel();
                    camel.Active = callareas[i].Active;
                    camel.CallAreaId = callareas[i].Callarea_id;
                    camel.CallAreaName = callareas[i].Callarea_name;
                    camel.CallTypeId = callareas[i].Calltype_id;
                    camel.ChkType = callareas[i].Chk_type;
                    camel.ProductTypeCate = callareas[i].Producttypecate;
                    camel.Responsible = callareas[i].Responsible;
                    camel.Sequence = callareas[i].Sequence;
                    callareaCamels.Add(camel);
                }
                return new
                {
                    success = true,
                    data = callareaCamels
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [Route("GetCallAreaNew")]
        public async Task<object> GetMasterCallAreaNew([FromBody]GetCAllArea data)
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

                List<Callarea> callareas = _masterRepository.GetCallAreaByProductCat_Sync(data.ProductTypeCate);
                //List<GetCAllAreaxDescroiption> ReturnObj = new List<GetCAllAreaxDescroiption>();
                //for (int i = 0; i < callareas.Count(); i++)
                //{
                //    GetCAllAreaxDescroiption getCAllAreaxDescroiption = new GetCAllAreaxDescroiption();
                //    List<calldescription> calldescriptions = _masterRepository.GetCallDescriptionByCallAreaID_Sync(callareas[i].callarea_id);
                //    getCAllAreaxDescroiption.callarea = callareas[i];
                //    getCAllAreaxDescroiption.calldescriptions = calldescriptions;
                //    ReturnObj.Add(getCAllAreaxDescroiption);
                //}
                return new
                {
                    success = true,
                    data = callareas
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

        //[HttpPost]
        //[Route("GetCallTransactionDefect")]
        //public async Task<object> GetCallTransactionDefect([FromBody]callTDefectObj data)
        //{
        //    try
        //    {
                
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

        //[HttpPost]
        //[Route("GetCreatedDefectByID")]
        //public async Task<object> CreateDefect([FromBody]callTDefectObj data)
        //{
        //    try
        //    {
        //        //bool CanAccess = _authorizeService.AccessKeyAuthentication(data.AccessKey, data.EmpCode);
        //        //if (CanAccess == false)
        //        //{
        //        //    return new
        //        //    {
        //        //        success = false,
        //        //        data = "AccessKey is Invalid!"
        //        //    };
        //        //}
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
        [Route("GetCreatedDefectByProjectCodeAndUnit")]
        public async Task<object> GetCreatedDefectByProjectCodeAndUnitHorizontal([FromBody] GetDefectTransactionByUnitID data)
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
                CallTdefectMObj callTDefect = _masterRepository.GetCallTDefectByUnitID_Sync(data.ProjectCode, data.UnitNo);
                if (callTDefect == null)
                {
                    return new
                    {
                        success = false,
                        data = callTDefect,
                        message = "Cannot Find Any Defect Header!"
                    };
                }
                List<GetCallTransactionDefectObj> Return = new List<GetCallTransactionDefectObj>();

                List<CallTdefectDetailCustom> callTDefectDetails = _masterRepository.GetcallTDefectDetailShow_Sync(callTDefect.TDefectId);
                List<CallTdefectDetailCustomShow> DefectDetailCustomList = new List<CallTdefectDetailCustomShow>();
                    for (int a = 0; a < callTDefectDetails.Count(); a++)
                    {

                        CallTdefectDetailCustomShow obj = new CallTdefectDetailCustomShow();
                        obj.Active = callTDefectDetails[a].Active;
                        obj.CallAreaId = callTDefectDetails[a].CallArea_Id;
                        obj.CallAreaName = callTDefectDetails[a].CallArea_Name;
                        obj.CallDescId = callTDefectDetails[a].CallDescId;
                        obj.CallPointId = callTDefectDetails[a].CallPointId;
                        obj.CallSubPointId = callTDefectDetails[a].CallSubPointId;
                        obj.CallTypeId = callTDefectDetails[a].CallType_Id;
                        obj.Cate = callTDefectDetails[a].Cate;
                        obj.ChkMainPoint = callTDefectDetails[a].ChkMainPoint;
                        obj.ChkType = callTDefectDetails[a].Chk_Type;
                        obj.ClientId = callTDefectDetails[a].Client_Id;
                        obj.ClientSyncDate = callTDefectDetails[a].Client_SyncDate;
                        obj.ComPointId = callTDefectDetails[a].ComPoint_Id;
                        obj.CreateDate = callTDefectDetails[a].CreateDate;
                        obj.CreateUserId = callTDefectDetails[a].CreateUserId;
                        obj.CustRoundAuditDate = callTDefectDetails[a].CustRoundAuditDate;
                        obj.CustRoundAuditDueCloseDate = callTDefectDetails[a].CustRoundAuditDueCloseDate;
                        obj.CustRoundAuditNo = callTDefectDetails[a].CustRoundAuditNo;
                        obj.DeviceId = callTDefectDetails[a].DeviceId;
                        obj.EndPoint = callTDefectDetails[a].End_Point;
                        obj.FloorPlanSet = callTDefectDetails[a].FloorPlanSet;
                        obj.FloorPlanImageId = callTDefectDetails[a].FloorPlan_ImageId;
                        obj.FloorPlanX = callTDefectDetails[a].FloorPlan_X;
                        obj.FloorPlanY = callTDefectDetails[a].FloorPlan_Y;
                        obj.IsServerLockRow = callTDefectDetails[a].IsServerLockRow;
                        obj.ItemId = callTDefectDetails[a].ItemId;
                        obj.PointName = callTDefectDetails[a].Point_Name;
                        obj.ProductId = callTDefectDetails[a].ProductId;
                        obj.ProductTypeCate = callTDefectDetails[a].ProductTypeCate;
                        obj.Respondsible = callTDefectDetails[a].Respondsible;
                        obj.RowActive = callTDefectDetails[a].RowActive;
                        obj.RowState = callTDefectDetails[a].RowState;
                        obj.Sequence = callTDefectDetails[a].Sequence;
                        obj.SubPoint = callTDefectDetails[a].Sequence;
                        obj.SubPoint = callTDefectDetails[a].Sub_Point;
                        obj.Tag = callTDefectDetails[a].Tag;
                        obj.TaskActualCloseDate = callTDefectDetails[a].TaskActualCloseDate;
                        obj.TaskActualFinishDate = callTDefectDetails[a].TaskActualFinishDate;
                        obj.TaskMarkName = callTDefectDetails[a].TaskMarkName;
                        obj.TaskNo = callTDefectDetails[a].TaskNo;
                        obj.TaskOpenDate = callTDefectDetails[a].TaskOpenDate;
                        obj.TaskProcessDate = callTDefectDetails[a].TaskProcessDate;
                        obj.TDefectDetailDesc = callTDefectDetails[a].TDefectDetailDesc;
                        obj.TDefectDetailId = callTDefectDetails[a].TDefectDetailId;
                        obj.TDefectDetailStatus = callTDefectDetails[a].TDefectDetailStatus;
                        obj.TDefectDetailSubStatus = callTDefectDetails[a].TDefectDetailSubStatus;
                        obj.TDefectDocNo = callTDefectDetails[a].TDefectDocNo;
                        obj.TDefectId = callTDefectDetails[a].TDefectId;
                        obj.UpdateDate = callTDefectDetails[a].UpdateDate;
                        obj.UpdateUserId = callTDefectDetails[a].UpdateUserId;
                        DefectDetailCustomList.Add(obj);
                    }
                    GetCallTransactionDefectObj ReturnObj = new GetCallTransactionDefectObj();
                    ReturnObj.callTDefect = callTDefect;
                    ReturnObj.callTDefectDetail = DefectDetailCustomList;
                    Return.Add(ReturnObj);


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

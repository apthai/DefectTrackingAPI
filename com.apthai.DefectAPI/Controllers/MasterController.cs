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

        public MasterController(IAuthorizeService authorizeService)
        {


            _hostingEnvironment = UtilsProvider.HostingEnvironment;
            _config = UtilsProvider.Config;
            _appSetting = UtilsProvider.AppSetting;
            _unitOfWork = new UnitOfWork(_hostingEnvironment, _config);
            _masterRepository = new MasterRepository(_hostingEnvironment, _config);
            _authorizeService = authorizeService;
        }

        [HttpPost]
        [Route("GetAllSignatureByCallTDefectID")]
        public async Task<object> GetAllSignatureByCallTDefectID([FromBody] int TDefectID )
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
                List<callResource> callResources = _masterRepository.GetSignatureCallResourceByTdefectID(TDefectID);
                
                return new
                {
                    success = true,
                    data = callResources
                };

            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }

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
        public async Task<object> GetProjectInformationDetail([FromBody] GetProjectInformationDetail data)
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
                List<PointURL> points = _masterRepository.GetFloorDistinct("H");
                List<pointCamel> ReturnObj = new List<pointCamel>();
                for (int i = 0; i < points.Count(); i++)
                {
                    pointCamel point = new pointCamel();
                    point.Cate = points[i].Cate;
                    point.ChkMainPoint = points[i].Chkmainpoint;
                    point.CompPointId = points[i].Comppoint_id;
                    point.EndPoint = points[i].End_point;
                    point.FloorPlantset = points[i].Floorplantset;
                    point.PointName = points[i].Point_name;
                    point.ProductTypeCate = points[i].Producttypecate;
                    point.Project = points[i].Project;
                    point.SubPoint = points[i].Sub_point;
                    point.URL = points[i].ImageURL;
                    ReturnObj.Add(point);
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
        [Route("GetUnitByProject")]
        public async Task<object> GetMasterUnitByProject([FromBody] GetunitByProjectParam data)
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
                    for (int i = 0; i < callTDefects.Count(); i++)
                    {
                        CallTdefectCheckCustomer callTDefect = _masterRepository.GetCallTDefectByUnitNumber_Sync(callTDefects[i].UnitNumber);
                        if (callTDefect != null)
                        {
                            callTDefects[i].TDefectId = callTDefect.TDefectId;
                            if (callTDefect.ContactName != null && (callTDefect.CurrentCustomerFirstName == null ? false : callTDefect.ContactName.Contains(callTDefect.CurrentCustomerFirstName))
                            && (callTDefect.CurrentCustomerLastName == null ? false : callTDefect.ContactName.Contains(callTDefect.CurrentCustomerLastName)))
                            {
                                callTDefects[i].IsNew = false;
                            }
                            else
                            {
                                callTDefects[i].IsNew = true;
                            }
                            callTDefects[i].DocIsExternalAudit = callTDefect.DocIsExternalAudit;
                        }
                        else
                        {
                            callTDefects[i].TDefectId = 0;
                            callTDefects[i].IsNew = true;
                        }
                        
                    }

                    return new
                    {
                        success = true,
                        data = callTDefects
                    };
                }
                else
                {
                    List<GetUnitByProjectReturnObj> Units = _masterRepository.GetUnitByProduct(data.ProjectID, data.SearchText);

                    for (int i = 0; i < Units.Count(); i++)
                    {
                        CallTdefectCheckCustomer callTDefect = _masterRepository.GetCallTDefectByUnitNumber_Sync(Units[i].UnitNumber);
                        if (callTDefect != null)
                        {
                            Units[i].TDefectId = callTDefect.TDefectId;
                            if (callTDefect.ContactName != null && (callTDefect.CurrentCustomerFirstName == null ? false : callTDefect.ContactName.Contains(callTDefect.CurrentCustomerFirstName))
                            && (callTDefect.CurrentCustomerLastName == null ? false : callTDefect.ContactName.Contains(callTDefect.CurrentCustomerLastName)))
                            {
                                Units[i].IsNew = false;
                            }
                            else
                            {
                                Units[i].IsNew = true;
                            }
                            Units[i].DocIsExternalAudit = callTDefect.DocIsExternalAudit;
                        }
                        else
                        {
                            Units[i].TDefectId = 0;
                            Units[i].IsNew = true;
                        }
                        
                    }

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
        [HttpPost]
        [Route("GetCallPointHorizontal")]
        public async Task<object> GetCallPointHorizontal([FromBody] GetPointParam data)
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
                List<PointURL> points = _masterRepository.GetCallPointByProductCat_Sync("H", data.Cate);
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
                    camels.URL = points[i].ImageURL;
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
        [HttpPost]
        [Route("GetCallPointVertical")]
        public async Task<object> GetCallPointVertical([FromBody] GetPointParam data)
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
                List<PointURL> points = _masterRepository.GetCallPointByProductCat_Sync(null, data.Cate);
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
                    camels.URL = points[i].ImageURL;
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
                    camel.URL = callareas[i].ImageURL;
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
                    camel.URL = callareas[i].ImageURL;
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
        public async Task<object> GetMasterCallAreaNew([FromBody] GetCAllArea data)
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
        public async Task<object> GetCallDescriptionByArea([FromBody] GetCAllDescriptionParam data)
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
                if (callTDefect.TDefectStatus == "001")
                {
                    callTDefect.StatusShow = "Open";
                }
                else if (callTDefect.TDefectStatus == "003")
                {
                    callTDefect.StatusShow = "Finish";
                }
                else if (callTDefect.TDefectStatus == "001")
                {
                    callTDefect.StatusShow = "Close";
                }
                List<GetCallTransactionDefectObj> Return = new List<GetCallTransactionDefectObj>();
               
                List<CallTdefectDetailCustom> callTDefectDetails = _masterRepository.GetcallTDefectDetailShow_Sync(callTDefect.TDefectId);
                List<CallTdefectDetailCustomShow> DefectDetailCustomList = new List<CallTdefectDetailCustomShow>();
                for (int a = 0; a < callTDefectDetails.Count(); a++)
                {

                    CallTdefectDetailCustomShow obj = new CallTdefectDetailCustomShow();
                    Model.DefectAPI.point CurrentPoint = new Model.DefectAPI.point();
                    CurrentPoint = _masterRepository.GetCallPointByPointID_Sync(callTDefectDetails[a].CallPointId);
                    obj.TDefectDetailId = callTDefectDetails[a].TDefectDetailId;
                    obj.Active = callTDefectDetails[a].Active;
                    obj.CallAreaId = callTDefectDetails[a].CallArea_Id;
                    obj.CallAreaName = callTDefectDetails[a].CallArea_Name;
                    obj.CallDescId = callTDefectDetails[a].CallDescId;
                    obj.CallPointId = callTDefectDetails[a].CallPointId;
                    obj.ComPointId = callTDefectDetails[a].CallPointId.ToString();
                    obj.CallSubPointId = callTDefectDetails[a].CallSubPointId;
                    obj.CallTypeId = callTDefectDetails[a].CallType_Id;
                    obj.Cate = CurrentPoint.cate.ToString();
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
                    obj.PointName = CurrentPoint.point_name;
                    obj.ProductId = callTDefectDetails[a].ProductId;
                    obj.ProductTypeCate = callTDefectDetails[a].ProductTypeCate;
                    obj.Respondsible = callTDefectDetails[a].Respondsible;
                    obj.RowActive = callTDefectDetails[a].RowActive;
                    obj.RowState = callTDefectDetails[a].RowState;
                    obj.Sequence = callTDefectDetails[a].Sequence;
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
                    if (obj.TDefectDetailStatus == "001")
                    {
                        obj.StatusShow = "Open";
                    }
                    else if (obj.TDefectDetailStatus == "003")
                    {
                        obj.StatusShow = "Finish";
                    }
                    else if (obj.TDefectDetailStatus == "001")
                    {
                        obj.StatusShow = "Close";
                    }


                    List<callResource> BF = _masterRepository.GetCallResourceBeforeByTdefectDetailID(callTDefectDetails[a].TDefectDetailId);
                    List<callResource> AF = _masterRepository.GetCallResourceAfterByTdefectDetailID(callTDefectDetails[a].TDefectDetailId);
                    string WebBaseUrl = Environment.GetEnvironmentVariable("BaseURL");
                    List<PicInDetailObj> BFObject = new List<PicInDetailObj>();
                    List<PicInDetailObj> AFObject = new List<PicInDetailObj>();
                    if (BF.Count > 0)
                    {
                        
                        for (int i = 0; i < BF.Count(); i++)
                        {
                            PicInDetailObj BFURL = new PicInDetailObj();
                            BFURL.URL = WebBaseUrl + "/" + BF[i].FilePath;
                            BFURL.ResourceId = BF[i].ResourceId;
                            BFObject.Add(BFURL);
                        }
                        obj.BeforePic = BFObject;
                    }
                    else
                    {
                        obj.BeforePic = new List<PicInDetailObj>();
                    }
                    if (AF.Count > 0)
                    {
                        for (int i = 0; i < AF.Count(); i++)
                        {
                            PicInDetailObj AFURL = new PicInDetailObj();
                            AFURL.URL = WebBaseUrl + "/" + AF[i].FilePath;
                            AFURL.ResourceId = AF[i].ResourceId;
                            AFObject.Add(AFURL);
                        }
                        obj.AfterPic = AFObject;
                    }
                    else
                    {
                        obj.AfterPic = new List<PicInDetailObj>();

                    }
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
        [Route("GetCreatedVendorDefectByProjectCodeAndUnit")]
        public async Task<object> GetCreatedVendorDefectByProjectCodeAndUnit([FromBody] GetDefectTransactionByUnitID data)
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
                CallTdefectVendorMObj callTDefect = _masterRepository.GetCallTDefectVendorByUnitID_Sync(data.ProjectCode, data.UnitNo);
                if (callTDefect == null)
                {
                    return new
                    {
                        success = false,
                        data = callTDefect,
                        message = "Cannot Find Any Defect Header!"
                    };
                }
                if (callTDefect.TDefectStatus == "001")
                {
                    callTDefect.StatusShow = "Open";
                }
                else if (callTDefect.TDefectStatus == "003")
                {
                    callTDefect.StatusShow = "Finish";
                }
                else if (callTDefect.TDefectStatus == "001")
                {
                    callTDefect.StatusShow = "Close";
                }
                List<callResource> callResources = _masterRepository.GetCallResourceByTdefect(callTDefect.TDefectId);
                string WebBaseUrl = Environment.GetEnvironmentVariable("BaseURL");
                List<PicInDetailObj> ReturnObj = new List<PicInDetailObj>();
                if (callResources.Count > 0)
                    {

                    for (int i = 0; i < callResources.Count(); i++)
                    {
                        PicInDetailObj BFURL = new PicInDetailObj();
                        BFURL.URL = WebBaseUrl + "/" + callResources[i].FilePath;
                        BFURL.ResourceId = callResources[i].ResourceId;
                        ReturnObj.Add(BFURL);
                    }
                }
                callTDefect.Resource = ReturnObj;
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
        [Route("GetCallTransactionDefectByProject")]
        public async Task<object> GetCallTransactionDefectByProject([FromBody] GetCallTransactionDefectByProject data)
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

        [HttpGet]
        [Route("GetMasterFloorPlanImage")]
        public async Task<object> GetMasterFloorPlanImage()
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
                List<PointURL> points = _masterRepository.GetFloorDistinct("H");
                List<pointCamel> ReturnObj = new List<pointCamel>();
                for (int i = 0; i < points.Count(); i++)
                {
                    pointCamel point = new pointCamel();
                    point.Cate = points[i].Cate;
                    point.ChkMainPoint = points[i].Chkmainpoint;
                    point.CompPointId = points[i].Comppoint_id;
                    point.EndPoint = points[i].End_point;
                    point.FloorPlantset = points[i].Floorplantset;
                    point.PointName = points[i].Point_name;
                    point.ProductTypeCate = points[i].Producttypecate;
                    point.Project = points[i].Project;
                    point.SubPoint = points[i].Sub_point;
                    point.URL = points[i].ImageURL;
                    ReturnObj.Add(point);
                }
                FloorPlanImageObj Result = new FloorPlanImageObj();
                Result.ProjectId = "10060";
                Result.UnitId = "A01";
                Result.URL = "http://appprod01.ap-crm.com/StorageResources/APDefect/Data/FloorPlan/P_70013/FLP70013-1-CL%20.jpg";
                return new
                {
                    success = true,
                    data = Result
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

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
using com.apthai.DefectAPI.Services;

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
        MinioServices minio;

        public MasterController(IAuthorizeService authorizeService)
        {


            _hostingEnvironment = UtilsProvider.HostingEnvironment;
            _config = UtilsProvider.Config;
            _appSetting = UtilsProvider.AppSetting;
            _unitOfWork = new UnitOfWork(_hostingEnvironment, _config);
            _masterRepository = new MasterRepository(_hostingEnvironment, _config);
            _authorizeService = authorizeService;
        }

        //[HttpPost]
        //[Route("GetAllSignatureByCallTDefectID")]
        //public async Task<object> GetAllSignatureByCallTDefectID([FromBody] int TDefectID )
        //{
        //    try
        //    {
        //        //#region VerifyHeader
        //        //string ErrorHeader = "";
        //        //if (!VerifyHeader(out ErrorHeader))
        //        //{
        //        //    return new
        //        //    {
        //        //        success = false,
        //        //        data = ErrorHeader ,
        //        //        valid = false
        //        //    };
        //        //}
        //        //#endregion
        //        List<callResource> callResources = _masterRepository.GetSignatureCallResourceByTdefectID(TDefectID);

        //        return new
        //        {
        //            success = true,
        //            data = callResources
        //        };

        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, "Internal server error");
        //    }

        //}

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
                    List<GetUnitByProjectReturnObj> callTDefects = _masterRepository.GetRecentcallTDefect_Sync(data.EmpCode, data.ProjectID);
                    for (int i = 0; i < callTDefects.Count(); i++)
                    {
                        CallTdefectCheckCustomer callTDefect = _masterRepository.GetCallTDefectByUnitNumberAndProject_Sync(callTDefects[i].UnitNumber, Convert.ToInt32(data.ProjectID));
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
                        CallTdefectCheckCustomer callTDefect = _masterRepository.GetCallTDefectByUnitNumberAndProject_Sync(Units[i].UnitNumber, Convert.ToInt32(data.ProjectID));
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
                string WebBaseUrl = Environment.GetEnvironmentVariable("Minio_Endpoint") ?? UtilsProvider.AppSetting.MinioEndpoint;
                CallTdefectMObj callTDefect = _masterRepository.GetCallTDefectByUnitID_Sync(data.ProjectCode, data.UnitNo);
                if (callTDefect == null)
                {
                    GetCallTransactionDefectObj a = new GetCallTransactionDefectObj();
                    a.callTDefect = new CallTdefectMObj();
                    a.callTDefectDetail = new List<CallTdefectDetailCustomShow>();
                    return new
                    {
                        success = false,
                        data = a,
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
                else if (callTDefect.TDefectStatus == "004" || callTDefect.TDefectStatus == "005")
                {
                    callTDefect.StatusShow = "Close";
                }
                // ----------- Get Signature From CallResource -----------------------

                //List<callResource> Signature = _masterRepository.GetSignatureByTdefectID(callTDefect.TDefectId);
                //CustomerSignature customerSignature = new CustomerSignature();
                //for (int i = 0; i < Signature.Count(); i++)
                //{
                //    if (Signature[i].ResourceTagCode == "SAL-LC-AF")
                //    {
                //        string a = JsonConvert.SerializeObject(Signature[i]);
                //        customerSignature.AfterSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                //        customerSignature.AfterSig.URL = WebBaseUrl + "/" + Signature[i].FilePath;
                //    }
                //    else if (Signature[i].ResourceTagCode == "SAL-LC-BF")
                //    {
                //        string a = JsonConvert.SerializeObject(Signature[i]);
                //        customerSignature.BeforeSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                //        customerSignature.BeforeSig.URL = WebBaseUrl + "/" + Signature[i].FilePath;
                //    }
                //    else if (Signature[i].ResourceTagCode == "CUST-AF")
                //    {
                //        string a = JsonConvert.SerializeObject(Signature[i]);
                //        customerSignature.AfterSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                //        customerSignature.AfterSig.URL = WebBaseUrl + "/" + Signature[i].FilePath;
                //    }
                //    else if (Signature[i].ResourceTagCode == "CUST-BF")
                //    {
                //        string a = JsonConvert.SerializeObject(Signature[i]);
                //        customerSignature.BeforeSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                //        customerSignature.BeforeSig.URL = WebBaseUrl + "/" + Signature[i].FilePath;
                //    }
                //    else if (Signature[i].ResourceTagCode == "CON-MGR-AF")
                //    {
                //        string a = JsonConvert.SerializeObject(Signature[i]);
                //        customerSignature.AfterSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                //        customerSignature.AfterSig.URL = WebBaseUrl + "/" + Signature[i].FilePath;
                //    }
                //    else if (Signature[i].ResourceTagCode == "CON-MGR-BF")
                //    {
                //        string a = JsonConvert.SerializeObject(Signature[i]);
                //        customerSignature.BeforeSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                //        customerSignature.BeforeSig.URL = WebBaseUrl + "/" + Signature[i].FilePath;
                //    }
                //}
                //List<GetCallTransactionDefectObj> Return = new List<GetCallTransactionDefectObj>();

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
                    else if (obj.TDefectDetailStatus == "004" || obj.TDefectDetailStatus == "005")
                    {
                        obj.StatusShow = "Close";
                    }


                    List<callResource> BF = _masterRepository.GetCallResourceBeforeByTdefectDetailID(callTDefectDetails[a].TDefectDetailId);
                    List<callResource> AF = _masterRepository.GetCallResourceAfterByTdefectDetailID(callTDefectDetails[a].TDefectDetailId);
                    if (BF.Count > 0)
                    {
                        obj.IsHasBFPic = true;
                    }

                    if (AF.Count > 0)
                    {
                        obj.IsHasAFPic = true;
                    }

                    DefectDetailCustomList.Add(obj);
                }
                GetCallTransactionDefectObj ReturnObj = new GetCallTransactionDefectObj();
                ReturnObj.callTDefect = callTDefect;

                ReturnObj.callTDefectDetail = DefectDetailCustomList;
                //Return.Add(ReturnObj);

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
        [Route("GetDefectImagesDetail")]
        public async Task<object> GetDefectImagesDetail([FromBody] callTDefectImagesDetailObj data)
        {
            try
            {
                string bucketName = Environment.GetEnvironmentVariable("Minio_DefaultBucket") ?? UtilsProvider.AppSetting.MinioDefaultBucket;
                CallTDefectImagesDetailModel returnModel = new CallTDefectImagesDetailModel();

                List<callResource> BF = _masterRepository.GetCallResourceBeforeByTdefectDetailID(data.TDefectDetailID);
                List<callResource> AF = _masterRepository.GetCallResourceAfterByTdefectDetailID(data.TDefectDetailID);
                List<PicInDetailObj> BFObject = new List<PicInDetailObj>();
                List<PicInDetailObj> AFObject = new List<PicInDetailObj>();
                minio = new MinioServices();
                if (BF.Count > 0)
                {

                    for (int i = 0; i < BF.Count(); i++)
                    {
                        PicInDetailObj BFURL = new PicInDetailObj();
                        BFURL.URL = await minio.GetFileUrlAsync(bucketName, BF[i].FilePath);
                        BFURL.ResourceId = BF[i].ResourceId;
                        BFObject.Add(BFURL);
                    }
                    returnModel.BeforePic = BFObject;
                }
                else
                {
                    returnModel.BeforePic = new List<PicInDetailObj>();
                }
                if (AF.Count > 0)
                {
                    for (int i = 0; i < AF.Count(); i++)
                    {
                        PicInDetailObj AFURL = new PicInDetailObj();
                        AFURL.URL = await minio.GetFileUrlAsync(bucketName, AF[i].FilePath);
                        AFURL.ResourceId = AF[i].ResourceId;
                        AFObject.Add(AFURL);
                    }
                    returnModel.AfterPic = AFObject;
                }
                else
                {
                    returnModel.AfterPic = new List<PicInDetailObj>();

                }
                return new
                {
                    success = true,
                    data = returnModel
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
                    CallTdefectVendorMObj a = new CallTdefectVendorMObj();
                    a.Resource = new List<PicInDetailObj>();
                    return new
                    {
                        success = false,
                        data = a,
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
                        if (callResources[i].ResourceType != 1) // ไม่เอา ลานเซ็นมา
                        {
                            PicInDetailObj BFURL = new PicInDetailObj();
                            BFURL.URL = WebBaseUrl + "/" + callResources[i].FilePath;
                            BFURL.ResourceId = callResources[i].ResourceId;
                            ReturnObj.Add(BFURL);
                        }
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

        [HttpPost]
        [Route("GetMasterFloorPlanImage")]
        public async Task<object> GetMasterFloorPlanImage([FromBody] GetFloorPlan data)
        {
            try
            {
                #region VerifyHeader
                //string ErrorHeader = "";
                //if (!VerifyHeader(out ErrorHeader))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = ErrorHeader,
                //        valid = false
                //    };
                //}
                #endregion

                string bucketName = Environment.GetEnvironmentVariable("Minio_DefaultBucket") ?? UtilsProvider.AppSetting.MinioDefaultBucket;
                string webUrl = Environment.GetEnvironmentVariable("WebURL");
                List<callResource> CheckIsPlanDraw = _masterRepository.GetFloorPlanByTdefectID(data.TDefectID);
                List<callTFloorPlanImage> callTFloorPlanImage = _masterRepository.GetUnitFloorPlanByUnitAndFloor(data.UnitID, data.Floor, data.ProjectNo); // master
               
                List<FloorPlanImageObj> ResultObj = new List<FloorPlanImageObj>();
                minio = new MinioServices();
                foreach (var floorPlan in callTFloorPlanImage)
                {
                    var getFloorRs = CheckIsPlanDraw.Where(w => w.ResourceTagCode == floorPlan.Floor).ToList();
                    if(getFloorRs.Count > 0)
                    {
                        var latestFloor = getFloorRs.OrderByDescending(o => o.ResourceId).FirstOrDefault();
                        FloorPlanImageObj Result = new FloorPlanImageObj();
                        Result.ProjectId = latestFloor.ProjectNo;
                        Result.UnitId = latestFloor.SerialNo;
                        Result.URL = await minio.GetFileUrlAsync(bucketName, latestFloor.FilePath);
                        Result.Floor = floorPlan.Floor;
                        ResultObj.Add(Result);
                    }
                    else
                    {
                        FloorPlanImageObj Result = new FloorPlanImageObj();
                        Result.ProjectId = floorPlan.ProductId;
                        Result.UnitId = floorPlan.UnitLayoutType;
                        Result.URL = webUrl + floorPlan.FilePath;
                        Result.Floor = floorPlan.Floor;
                        ResultObj.Add(Result);
                    }
                }

                return new
                {
                    success = true,
                    data = ResultObj
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Route("GetTDefectSignature")]
        public async Task<object> GetTDefectSignature([FromBody] GetTDefectSignature data)
        {
            try
            {
                #region VerifyHeader
                //string ErrorHeader = "";
                //if (!VerifyHeader(out ErrorHeader))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = ErrorHeader,
                //        valid = false
                //    };
                //}
                #endregion
                string WebCRMUrl = Environment.GetEnvironmentVariable("WebURL");
                string WebBaseUrl = Environment.GetEnvironmentVariable("BaseURL");
                string bucketName = Environment.GetEnvironmentVariable("Minio_DefaultBucket") ?? UtilsProvider.AppSetting.MinioDefaultBucket;
                List<callResource> Signature = _masterRepository.GetSignatureByTdefectID(data.TDefectId ?? 0);
                GetTDefectSignatureObj ReturnObj = new GetTDefectSignatureObj();
                CustomerSignature customerSignature = new CustomerSignature();
                SESignature sESignature = new SESignature();
                LCSignature lCSignature = new LCSignature();
                minio = new MinioServices();
                if (Signature == null)
                {

                    List<CustomerSignature> a = new List<CustomerSignature>();
                    return new
                    {
                        success = false,
                        data = a
                    };
                }
                for (int i = 0; i < Signature.Count(); i++)
                {
                    if (Signature[i].ResourceTagCode == "SAL-LC-AF")
                    {
                        string a = JsonConvert.SerializeObject(Signature[i]);
                        lCSignature.AfterSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                        lCSignature.AfterSig.URL = await minio.GetFileUrlAsync(bucketName, Signature[i].FilePath);
                    }
                    else if (Signature[i].ResourceTagCode == "SAL-LC-BF")
                    {
                        string a = JsonConvert.SerializeObject(Signature[i]);
                        lCSignature.BeforeSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                        lCSignature.BeforeSig.URL = await minio.GetFileUrlAsync(bucketName, Signature[i].FilePath);
                    }
                    else if (Signature[i].ResourceTagCode == "CUST-AF")
                    {
                        string a = JsonConvert.SerializeObject(Signature[i]);
                        customerSignature.AfterSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                        customerSignature.AfterSig.URL = await minio.GetFileUrlAsync(bucketName, Signature[i].FilePath);
                    }
                    else if (Signature[i].ResourceTagCode == "CUST-BF")
                    {
                        string a = JsonConvert.SerializeObject(Signature[i]);
                        customerSignature.BeforeSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                        customerSignature.BeforeSig.URL = await minio.GetFileUrlAsync(bucketName, Signature[i].FilePath);
                    }
                    else if (Signature[i].ResourceTagCode == "CUST-RECE")
                    {
                        string a = JsonConvert.SerializeObject(Signature[i]);
                        customerSignature.ReceiveSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                        customerSignature.ReceiveSig.URL = await minio.GetFileUrlAsync(bucketName, Signature[i].FilePath);
                    }
                    else if (Signature[i].ResourceTagCode == "CON-MGR-AF")
                    {
                        string a = JsonConvert.SerializeObject(Signature[i]);
                        sESignature.AfterSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                        sESignature.AfterSig.URL = await minio.GetFileUrlAsync(bucketName, Signature[i].FilePath);
                    }
                    else if (Signature[i].ResourceTagCode == "CON-MGR-BF")
                    {
                        string a = JsonConvert.SerializeObject(Signature[i]);
                        sESignature.BeforeSig = JsonConvert.DeserializeObject<CallresouceWithURL>(a);
                        sESignature.BeforeSig.URL = await minio.GetFileUrlAsync(bucketName, Signature[i].FilePath);
                    }
                }
                ReturnObj.CustomerSignature = customerSignature;
                ReturnObj.SESignature = sESignature;
                ReturnObj.LCSignature = lCSignature;
                // --------------- กัน Object แตก ของ บอม --------------------
                if (ReturnObj.CustomerSignature.AfterSig == null)
                {
                    ReturnObj.CustomerSignature.AfterSig = new CallresouceWithURL();
                }
                if (ReturnObj.CustomerSignature.BeforeSig == null)
                {
                    ReturnObj.CustomerSignature.BeforeSig = new CallresouceWithURL();
                }

                if (ReturnObj.CustomerSignature.ReceiveSig == null)
                {
                    ReturnObj.CustomerSignature.ReceiveSig = new CallresouceWithURL();
                }

                if (ReturnObj.LCSignature.AfterSig == null)
                {
                    ReturnObj.LCSignature.AfterSig = new CallresouceWithURL();
                }
                if (ReturnObj.LCSignature.BeforeSig == null)
                {
                    ReturnObj.LCSignature.BeforeSig = new CallresouceWithURL();
                }

                if (ReturnObj.SESignature.AfterSig == null)
                {
                    ReturnObj.SESignature.AfterSig = new CallresouceWithURL();
                }
                if (ReturnObj.SESignature.BeforeSig == null)
                {
                    ReturnObj.SESignature.BeforeSig = new CallresouceWithURL();
                }
                //-----------------------------------------------------------
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

        [HttpPost("GetDefectPdfDocument")]
        public async Task<object> GetDefectPdfDocument([FromBody] GetPdfReportByTDefectId model)
        {
            try
            {
                var result = _masterRepository.GetDefectPdfDocument(model.TDefectId);
                string bucketName = Environment.GetEnvironmentVariable("Minio_DefaultBucket") ?? UtilsProvider.AppSetting.MinioDefaultBucket;
                minio = new MinioServices();
                CallTDefectPdfDocumentModel returnModel = new CallTDefectPdfDocumentModel();
                if (!String.IsNullOrEmpty(result))
                    returnModel.FilePath = await minio.GetFileUrlAsync(bucketName, result);
                else
                    return new
                    {
                        success = false,
                        data = new CallTDefectPdfDocumentModel(),
                        ErrorMsg = "Invalid User Defect Document"
                    };

                return new
                {
                    success = true,
                    data = returnModel
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

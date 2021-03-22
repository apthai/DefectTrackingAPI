using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using com.apthai.DefectAPI.Model.DefectAPI;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Filters;

namespace com.apthai.DefectAPI.CustomModel
{
    public partial class ParamUploadImage
    {
        [Required]
        public string TDefectID { get; set; }
        [Required]
        public string TDefectDetailId { get; set; }
        [Required]
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int ResourceType { get; set; }
        public string Description { get; set; }
        public string ResourceTagCode { get; set; }
        public string ProjectCode { get; set; }
        public string UnitNo { get; set; }
        public string SerialNo { get; set; }
        public string DeviceID { get; set; }
        public IFormFile Files { get; set; }

    }
    public partial class ParamUploadImageBefore
    {
        [Required]
        public string TDefectID { get; set; }
        [Required]
        public string TDefectDetailId { get; set; }
        [Required]
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string ProjectCode { get; set; }
        public string UnitNo { get; set; }
        public string SerialNo { get; set; }
        public string DeviceID { get; set; }
        public IFormFile Files { get; set; }


    }
    public partial class ParamUploadImageAfter
    {
        [Required]
        public string TDefectID { get; set; }
        [Required]
        public string TDefectDetailId { get; set; }
        [Required]
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string ProjectCode { get; set; }
        public string UnitNo { get; set; }
        public string SerialNo { get; set; }
        public string DeviceID { get; set; }
        public IFormFile Files { get; set; }


    }
    public partial class ParamUploadImageCusNotSign
    {
        [Required]
        public string TDefectID { get; set; }
        [Required]
        public string TDefectDetailId { get; set; }
        [Required]
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string ProjectCode { get; set; }
        public string UnitNo { get; set; }
        public string SerialNo { get; set; }
        public string DeviceID { get; set; }
        public IFormFile Files { get; set; }


    }
    public partial class ParamUploadImageCusSignature
    {
        [Required]
        public string TDefectID { get; set; }
        [Required]
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string ProjectCode { get; set; }
        public string UnitNo { get; set; }
        public string SerialNo { get; set; }
        public string DeviceID { get; set; }
        public IFormFile Files { get; set; }
        public bool IsBF { get; set; }
        public string EmpCode { get; set; }
        public string DefectType { get; set; }
    }
    public partial class TDefectDetailWithStatus : callTDefectDetail
    {
        public string StatusShow { get; set; }

    }
    public partial class CallresouceWithURL : callResource
    {
        public string URL { get; set; }
    }
    public partial class callTDefectWithStatus : callTDefect
    {
        public string StatusShow { get; set; }

    }
    public partial class CallTdefectCheckCustomer : callTDefect
    {
        public string CurrentCustomerFirstName { get; set; }
        public string CurrentCustomerLastName { get; set; }
    }
    public partial class ParamUploadImagess
    {
        [Required]
        public string TDefectID { get; set; }
        [Required]
        public string TDefectDetailId { get; set; }
        [Required]
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int ResourceType { get; set; }
        public string Description { get; set; }
        public string ResourceTagCode { get; set; }
        public string ProjectCode { get; set; }
        public string UnitNo { get; set; }
        public string SerialNo { get; set; }
        public string DeviceID { get; set; }
        public IFormFile Files { get; set; }


    }
    public class GetCAllAreaxDescroiption
    {
        public callarea callarea { get; set; }
        public List<calldescription> calldescriptions { get; set; }
    }

    public class GetCallTransactionDefectObj
    {
        public CallTdefectMObj callTDefect { get; set; }
        public List<CallTdefectDetailCustomShow> callTDefectDetail { get; set; }
    }
    public class CallTdefectMObj : callTDefect
    {
        public string PhaseID { get; set; }
        public string BlockID { get; set; }
        public string TowerID { get; set; }
        public string FloorID { get; set; }
        public string StatusShow { get; set; }
        public string CurrentCustomerFirstName { get; set; }
        public string CurrentCustomerLastName { get; set; }
        public bool IsNew { get; set; }
        public CustomerSignature CustomerSignature { get; set; }
        public SESignature SESignature { get; set; }
        public LCSignature LCSignature { get; set; }
    }
    public class GetTDefectSignatureObj
    {
        public CustomerSignature CustomerSignature { get; set; }
        public SESignature SESignature { get; set; }
        public LCSignature LCSignature { get; set; }
    }
    public class CustomerSignature
    {
        public CallresouceWithURL BeforeSig { get; set; }
        public CallresouceWithURL AfterSig { get; set; }
        public CallresouceWithURL ReceiveSig { get; set; }
    }

    public class SESignature
    {
        public CallresouceWithURL BeforeSig { get; set; }
        public CallresouceWithURL AfterSig { get; set; }
    }
    public class LCSignature
    {
        public CallresouceWithURL BeforeSig { get; set; }
        public CallresouceWithURL AfterSig { get; set; }
    }
    public class CallTdefectVendorMObj : callTDefect
    {
        public string PhaseID { get; set; }
        public string BlockID { get; set; }
        public string TowerID { get; set; }
        public string FloorID { get; set; }
        public string StatusShow { get; set; }
        public string CurrentCustomerFirstName { get; set; }
        public string CurrentCustomerLastName { get; set; }
        public bool IsNew { get; set; }
        public List<PicInDetailObj> Resource { get; set; }
    }

    public class GetCallTDefectByProjectObj : Model.DefectAPI.callTDefect
    {
        public string Project { get; set; }
    }
    public class FloorObj
    {
        public string FloorName { get; set; }
        public string URL { get; set; }
    }
    public class FloorPlanImageObj
    {
        public string ProjectId { get; set; }
        public string UnitId { get; set; }
        public string URL { get; set; }
        public string Floor { get; set; }
    }
    public class CallTdefectDetailCustom : callTDefectDetail
    {
        public string ComPoint_Id { get; set; }
        public string Point_Name { get; set; }
        public string Sub_Point { get; set; }
        public string End_Point { get; set; }
        public string Cate { get; set; }
        public string ChkMainPoint { get; set; }
        public string FloorPlanSet { get; set; }
        public string ProductTypeCate { get; set; }
        //-----------------------------------------
        public string CallArea_Id { get; set; }
        public string CallType_Id { get; set; }
        public string CallArea_Name { get; set; }
        public string Chk_Type { get; set; }
        public string Active { get; set; }
        public string Respondsible { get; set; }
        public string Sequence { get; set; }
        public string StaComplain { get; set; }

    }
    public class CallTdefectDetailCustomShow : callTDefectDetail
    {
        public string ClientId { get; set; }
        public DateTime? ClientSyncDate { get; set; }
        public string FloorPlanImageId { get; set; }
        public double? FloorPlanX { get; set; }
        public double? FloorPlanY { get; set; }
        public string ComPointId { get; set; }
        public string PointName { get; set; }
        public string SubPoint { get; set; }
        public string EndPoint { get; set; }
        public string Cate { get; set; }
        public string ChkMainPoint { get; set; }
        public string FloorPlanSet { get; set; }
        public string ProductTypeCate { get; set; }
        //-----------------------------------------
        public string CallAreaId { get; set; }
        public string CallTypeId { get; set; }
        public string CallAreaName { get; set; }
        public string ChkType { get; set; }
        public string Active { get; set; }
        public string Respondsible { get; set; }
        public string Sequence { get; set; }
        public string StaComplain { get; set; }
        public List<PicInDetailObj> BeforePic { get; set; }
        public List<PicInDetailObj> AfterPic { get; set; }
        public string StatusShow { get; set; }
        public bool IsHasBFPic { get; set; }
        public bool IsHasAFPic { get; set; }
    }
    public partial class PicInDetailObj
    {
        public string URL { get; set; }
        public int ResourceId { get; set; }
    }
    public partial class vwUserRole
    {
        public int? ID { get; set; }
        public int UserID { get; set; }
        public string UserGUID { get; set; }
        public string UserName { get; set; }
        public string EmpCode { get; set; }
        public string TitleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PositionName { get; set; }
        public int RoleID { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string AssignType { get; set; }
        public string SourceType { get; set; }
        public string Remark { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public partial class UserProject
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ProjectID { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string AssignType { get; set; }
        public string SourceType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Remark { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public partial class UserProjectType
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ProjectID { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string AssignType { get; set; }
        public string SourceType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Remark { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string producttypecate { get; set; }
    }

    public class UserModel
    {
        public vwUser User { get; set; }
        public string TitleMsg { get; set; }
        public string RedirectMsg { get; set; }
        public string TypeMsg { get; set; }
    }
    public class AutorizeRoles
    {

        public string UserId { get; set; }
        public List<vwUserRole> Roles { get; set; }

    }
    public partial class pointCamel
    {
        public int CompPointId { get; set; }
        public string Project { get; set; }
        public string PointName { get; set; }
        public string SubPoint { get; set; }
        public string EndPoint { get; set; }
        public int? Cate { get; set; }
        public int? ChkMainPoint { get; set; }
        public string FloorPlantset { get; set; }
        public string ProductTypeCate { get; set; }
        public string URL { get; set; }
    }
    public partial class PointURL : Point
    {
        public string ImageURL { get; set; }
    }
    public class GetUnitByProjectReturnObj : ViewUnitCustomer
    {
        public string PhaseID { get; set; }
        public string BlockID { get; set; }
        public string TowerID { get; set; }
        public string FloorID { get; set; }
        public int TDefectId { get; set; }
        public bool IsNew { get; set; }
        public bool? DocIsExternalAudit { get; set; }
    }
    public class vwUserProject
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserGUID { get; set; }
        public string EmpCode { get; set; }
        public string TitleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int ProjectID { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectCategory { get; set; }
        public string ProjectWBS { get; set; }
        public string ProjectName { get; set; }
        public string BU { get; set; }
        public string ProjectType { get; set; }
        public int? CompanyID { get; set; }
        public string CompanyCode { get; set; }
        public string CompanySAPCode { get; set; }
        public string CompanyName { get; set; }
        public string AssignType { get; set; }
        public string SourceType { get; set; }
        public string ProjectGroup { get; set; }
        public string Remark { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProjectGroupName { get; set; }
        public string PositionName { get; set; }
    }
    public class DefectUserProject : vwUserProject
    {
        public string ProjectDefectType { get; set; }
    }
    public class ProjectObj
    {
        public Tower TowerID { get; set; }
        public List<Floor> FloorID { get; set; }
    }
    public class ProjectObjList
    {
        public List<ProjectObj> ProjectList { get; set; }
    }
    public class Floor
    {
        public string FloorID { get; set; }
    }
    public class Tower
    {
        public string TowerID { get; set; }
    }
    public class vwUser
    {
        public int UserID { get; set; }
        public string UserGUID { get; set; }
        public string UserName { get; set; }
        public string EmpCode { get; set; }
        public string TitleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PositionName { get; set; }
        public string Email { get; set; }
        public string FullCodeName { get; set; }
        public string UserNameLogin { get; set; }
        public string RGUID { get; set; }
    }
    public partial class vwUserApp
    {
        public int? ID { get; set; }
        public int? UserID { get; set; }
        public string UserName { get; set; }
        public string UserGUID { get; set; }
        public string EmpCode { get; set; }
        public string TitleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int AppID { get; set; }
        public string AppCode { get; set; }
        public string AppName { get; set; }
        public string AssignType { get; set; }
        public string SourceType { get; set; }
        public string PositionName { get; set; }
        public string Remark { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public partial class GetcallTypeWithArea : calltype
    {
        public string chk_type { get; set; }
        public string responsible { get; set; }
        public string callarea_name { get; set; }
        public string producttypecate { get; set; }
    }
    public partial class Callarea
    {
        public int Callarea_id { get; set; }
        public int? Calltype_id { get; set; }
        public string Callarea_name { get; set; }
        public int? Chk_type { get; set; }
        public int? Active { get; set; }
        public string Responsible { get; set; }
        public string Producttypecate { get; set; }
        public int? Sequence { get; set; }
        public string ImageURL { get; set; }
    }


    public partial class Point
    {
        [Key]
        public int Comppoint_id { get; set; }
        public string Project { get; set; }
        public string Point_name { get; set; }
        public string Sub_point { get; set; }
        public string End_point { get; set; }
        public int? Cate { get; set; }
        public int? Chkmainpoint { get; set; }
        public string Floorplantset { get; set; }
        public string Producttypecate { get; set; }
    }
    public partial class PointCamel
    {
        public int CompPointId { get; set; }
        public string Project { get; set; }
        public string PointName { get; set; }
        public string SubPoint { get; set; }
        public string EndPoint { get; set; }
        public int? Cate { get; set; }
        public int? ChkMainPoint { get; set; }
        public string FloorPlantSet { get; set; }
        public string ProductTypeCate { get; set; }
        public string URL { get; set; }
    }
    public partial class CallareaCamel
    {
        public int CallAreaId { get; set; }
        public int? CallTypeId { get; set; }
        public string CallAreaName { get; set; }
        public int? ChkType { get; set; }
        public int? Active { get; set; }
        public string Responsible { get; set; }
        public string ProductTypeCate { get; set; }
        public int? Sequence { get; set; }
        public string URL { get; set; }
    }
    public partial class GeneratePDFParam
    {
        public string Folder { get; set; }
        public string FileName { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<Parameters> parameters { get; set; }
    }
    public partial class Parameters
    {
        public FirstRootSubParameters First { get; set; }
        public SecondRootSubParameters Second { get; set; }
    }
    public partial class FirstRootSubParameters
    {
        public string Name { get; set; } // TDfectID
        public string Value { get; set; } = "111";
    }
    public partial class SecondRootSubParameters
    {
        public string Name { get; set; } // CustRoundAuditNo
        public string Value { get; set; } = "1";
    }

    public partial class RequestReportModel
    {
        public string Folder { get; set; }
        public string FileName { get; set; }
        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<ParameterReport> Parameters { get; set; }
        public class ParameterReport
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
    public partial class ResponsetReportModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string URL { get; set; }
        public string FileName { get; set; }
    }

    public partial class ParamReportModel
    {
        public int TDefectId { get; set; }
        public string ProjectCode { get; set; }
        public string UnitNo { get; set; }
        public bool IsBF { get; set; }
    }

    public partial class FileUploadResult
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string BucketName { get; set; }
    }

    public partial class CallTDefectImagesDetailModel
    {
        public List<PicInDetailObj> BeforePic { get; set; }
        public List<PicInDetailObj> AfterPic { get; set; }

    }
}


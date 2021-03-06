﻿using com.apthai.DefectAPI.CustomModel;
using com.apthai.DefectAPI.Model.DefectAPI;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.DefectAPI.HttpRestModel
{
    public partial class LoginData
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AppCode { get; set; }

    }
    public partial class GetFloorPlan
    {
        public string UnitID { get; set; }
        public string ProjectNo { get; set; }
        public string Floor { get; set; }
        public int TDefectID { get; set; }
    }
    public partial class GetTDefectSignature
    {
        public int? TDefectId { get; set; }
    }
    public partial class GenerateReportByTDefectId
    {
        public int? TDefectId { get; set; } 

    }
    public partial class ParamUploadImageAfterList
    {
        [Required]
        public string TDefectID { get; set; }
        [Required]
        public string TDefectDetailId { get; set; }
        [Required]
        public string UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string ProjectCode { get; set; }
        public string UnitNo { get; set; }
        public string SerialNo { get; set; }
        public string DeviceID { get; set; }
        public List<IFormFile> Files { get; set; }


    }
    public partial class ParamUploadFloorPlan
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
        public string Floor { get; set; }
        public List<IFormFile> Files { get; set; }


    }
    public partial class LoginHashData
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }
    public class AutorizeDataJWT
    {
        public bool LoginResult { get; set; }
        public string LoginResultMessage { get; set; }
        public string UserPrincipalName { get; set; }
        public string DomainUserName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeID { get; set; }
        public string Email { get; set; }
        public string Division { get; set; }

        public string Token { get; set; }

        public DateTime? AccountExpirationDate { get; set; }
        public DateTime? LastLogon { get; set; }

        public string AuthenticationProvider { get; set; }
        public string SysUserId { get; set; }
        public string SysUserData { get; set; }
        public string SysUserRoles { get; set; }
        public string SysAppCode { get; set; }
        public string AppUserRole { get; set; }
        public string UserProject { get; set; }
        public string UserApp { get; set; }
        
    }
    public class AutorizeDataJWTReturnObject
    {
        public bool LoginResult { get; set; }
        public string LoginResultMessage { get; set; }
        public string UserPrincipalName { get; set; }
        public string DomainUserName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeID { get; set; }
        public string Email { get; set; }
        public string Division { get; set; }

        public string Token { get; set; }

        public DateTime? AccountExpirationDate { get; set; }
        public DateTime? LastLogon { get; set; }

        public string AuthenticationProvider { get; set; }
        public string SysUserId { get; set; }
        public UserModel SysUserData { get; set; }
        public CustomModel.vwUserRole SysUserRoles { get; set; }
        public string SysAppCode { get; set; }
        public string AppUserRole { get; set; }
        public List<UserProjectType> UserProject { get; set; }
        public List<vwUserApp> UserApp { get; set; }

    }
    public class GetCAllArea
    {
        public string ProductTypeCate { get; set; }
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
    }
    public class GetunitByProjectParam
    {
        public string ProjectID { get; set; }
        public string SearchText { get; set; }
        public string EmpCode { get; set; }
        public bool IsRecent { get; set; }
    }
    public class GetPointParam
    {
        public string Cate { get; set; }
    }
    public class GetProjectInformationDetail
    {
        public string ProjectCode { get; set; }
    }
    public class GetCAllDescriptionParam
    {
        public int CallAreaID { get; set; }
    }
    public class GetCAllPoint
    {
        public string ProductTypeCate { get; set; }
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
    }
    public class callTDefectObj
    {
        public int TDefectID { get; set; }
    }
    public class callTDefectImagesDetailObj
    {
        public int TDefectDetailID { get; set; }
    }
    public class GetDefectTransactionByUnitID
    {
        public string UnitNo { get; set; }
        public string ProjectCode { get; set; }
    }
    public class CreateDefectTransactionParam
    {
        public string DefectType { get; set; }
        public string ProductID { get; set; }
        public string ItemID { get; set; }
        public string Description { get; set; }
        public string DeviceId { get; set; }
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
    }

    public class GetCAllType
    {
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
    }
    public class GetDefectUploadListParam
    {
        public int TDefectDetailID { get; set; }
    }
    public partial class CreateDefectObj
    {
        public int TDefectId { get; set; }
        public string ProductId { get; set; }
        public string ItemId { get; set; }
        public string DeviceId { get; set; }
        public string UserID { get; set; }
        public string SellerId { get; set; }
        public string Description { get; set; }
        public int Cate { get; set; }
        public string DefectType { get; set; } // H = Horizontal หรือ V = Vertical
        //------ ของ TdefectDetail ------------
        public string TDefectDetailStatus { get; set; }
        public string TDefectDetailDesc { get; set; }
        public int CallTypeID { get; set; }
        public int CallArea { get; set; }
        public int CallDescId { get; set; }
        public int PointId { get; set; }
        public int CallSubPointId { get; set; }
        public string FloorPlanSet { get; set; }
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
        public IFormFile Files { get; set; }
    }
    public partial class UpdateDefectObj
    {
        public int TDefectDetailId { get; set; }
        public int TDefectId { get; set; }
        public string ProductId { get; set; }
        public string ItemId { get; set; }
        public string DeviceId { get; set; }
        public string UserID { get; set; }
        public string SellerId { get; set; }
        public string Description { get; set; }
        public int PointID { get; set; }
        public int Cate { get; set; }
        public string DefectType { get; set; } // H = Horizontal หรือ V = Vertical
        //------ ของ TdefectDetail ------------
        public string TDefectDetailStatus { get; set; }
        public string TDefectDetailDesc { get; set; }
        public int CallTypeID { get; set; }
        public int CallArea { get; set; }
        public int CallDescId { get; set; }
        public int CallSubPointId { get; set; }
        public string FloorPlanSet { get; set; }
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
        public IFormFile Files { get; set; }
    }
    public partial class UpdateDefectDetailID
    {
        public int TDefectDetailID { get; set; }
    }
    public partial class UpdateDefectDetailIDList
    {
        public List<UpdateDefectDetailID> TDefectDetailIDList { get; set; }
    }
    public partial class UpdateDefectHeader
    {
        public int TDefectID { get; set; }
        public DateTime DocDueCloseDate { get; set; }
    }
    public partial class UpdateDefectHeaderTranferDate
    {
        public int TDefectID { get; set; }
        public DateTime DocDueTranferDate { get; set; }
    }
    public partial class UpdateDefectHeaderCustomerNotSign
    {
        public int TDefectID { get; set; }
    }
    public class CreateDefectListObj
    {
        public callTDefect callTDefect { get; set; }
        public List<callTDefectDetail> callTDefectDetails { get; set; }
    }
    public class GetCallTransactionDefectByProject
    {
        public int ProductID { get; set; }
    }
    public partial class GetPdfReportByTDefectId
    {
        public int TDefectId { get; set; }

    }
}

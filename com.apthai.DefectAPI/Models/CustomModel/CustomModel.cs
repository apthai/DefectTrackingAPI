using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using com.apthai.DefectAPI.Model.DefectAPI;
using Microsoft.AspNetCore.Http;

namespace com.apthai.DefectAPI.CustomModel
{
    public partial class ParamUploadImage
    {
        [Required]
        public string TDefectID { get; set; }
        [Required]
        public string TDefectDetailIDID { get; set; }
        [Required]
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int ResourceType { get; set; }
        public string Description { get; set; }
        public string ResourceTagCode { get; set; }
        public string ProjectNO { get; set; }
        public int UnitNo { get; set; }
        public string SerialNo { get; set; }
        public string DeviceID { get; set; }
        public List<IFormFile> Files { get; set; }


    }
    public class GetCAllAreaxDescroiption
    {
        public callarea callarea { get; set; }
        public List<calldescription> calldescriptions { get; set; }
    }
    public class GetCallTransactionDefectObj
    {
        public callTDefect callTDefect { get; set; }
        public List<callTDefectDetail> callTDefectDetail { get; set; }
    }
    public class GetCallTDefectByProjectObj : Model.DefectAPI.callTDefect
    {
        public string Project { get; set; }
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
    public class GetUnitByProjectReturnObj : ViewUnitCustomer
    {
        public string PhaseID { get; set; }
        public string BlockID { get; set; }
        public string TowerID { get; set; }
        public string FloorID { get; set; }
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
}

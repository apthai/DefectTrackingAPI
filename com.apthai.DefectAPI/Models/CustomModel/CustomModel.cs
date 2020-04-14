using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.apthai.DefectAPI.Model.DefectAPI;

namespace com.apthai.DefectAPI.CustomModel
{
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
}

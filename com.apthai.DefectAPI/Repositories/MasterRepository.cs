using Dapper;
using com.apthai.DefectAPI.Model.DefectAPI;
using com.apthai.DefectAPI.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Hosting;
using com.apthai.DefectAPI.CustomModel;

namespace com.apthai.DefectAPI.Repositories
{
    public class MasterRepository : BaseRepository, IMasterRepository
    {


        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        //string TAG = "MasterRepository";

        public MasterRepository(IHostingEnvironment environment, IConfiguration config) : base(environment, config)
        {
            _config = config;
            _hostingEnvironment = environment;

        }
        public ICONEntFormsProduct GetProductDataFromCRM_Sync(string ProductID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                        string sQuery = "Select * From dbo.ICON_EntForms_Products WHERE ProductID = @ProductID ";
                        var result = conn.Query<ICONEntFormsProduct>(sQuery, new { ProductID = ProductID }).FirstOrDefault();
                        return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetProductDataFromCRM_Sync() :: Error ", ex);
                }
            }
        }
        public List<Callarea> GetCallAreaByProductCat_Sync(string ProductTypeCate)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    if (ProductTypeCate == null || ProductTypeCate == "")
                    {
                        string sQuery = "Select callarea.*,MobileAreaIcons.ImageURL From callarea " +
                            " left join MobileAreaIcons on callarea.callarea_id = MobileAreaIcons.callareaid " +
                            " where Active = 1 ";
                        var result = conn.Query<Callarea>(sQuery).ToList();
                        return result;
                    }
                    else
                    {
                        string sQuery = "Select callarea.*,MobileAreaIcons.ImageURL From callarea " +
                            " left join MobileAreaIcons on callarea.callarea_id = MobileAreaIcons.callareaid " +
                            "where callarea.ProductTypeCate = @ProductTypeCate And callarea.Active = 1 ";
                        var result = conn.Query<Callarea>(sQuery, new { ProductTypeCate = ProductTypeCate }).ToList();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
                }
            }
        }
        public List<callResource> GetSignatureCallResourceByTdefectID(int TDefectId)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {

                    string sQuery = "select * from callResource " +
                        " where TDefectId = @TDefectId and Active = 1 and ResourceType in (1,2,6 )";
                    var result = conn.Query<callResource>(sQuery, new { TDefectId = TDefectId }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetSignatureCallResourceByTdefectID() :: Error ", ex);
                }
            }
        }
        public List<callResource> GetCallResourceByTdefectDetailID(int TDefectDetailId)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    
                        string sQuery = "select * from callResource " +
                            " where TDefectDetailId = @TDefectDetailId and Active = 1 ";
                        var result = conn.Query<callResource>(sQuery, new { TDefectDetailId = TDefectDetailId }).ToList();
                        return result;
                    
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallResourceByTdefectDetailID() :: Error ", ex);
                }
            }
        }
        public List<callResource> GetCallResourceByTdefect(int TDefectId)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {

                    string sQuery = "select * from callResource " +
                        " where TDefectId = @TDefectId ";
                    var result = conn.Query<callResource>(sQuery, new { TDefectId = TDefectId }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallResourceByTdefectDetailID() :: Error ", ex);
                }
            }
        }
        public List<callResource> GetCallResourceBeforeByTdefectDetailID(int TDefectDetailId)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {

                    string sQuery = "select * from callResource " +
                        " where TDefectDetailId = @TDefectDetailId and ResourceTagCode = 'BF-RP' and Active = 1 ";
                    var result = conn.Query<callResource>(sQuery, new { TDefectDetailId = TDefectDetailId }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallResourceByTdefectDetailID() :: Error ", ex);
                }
            }
        }
        public List<callResource> GetCallResourceAfterByTdefectDetailID(int TDefectDetailId)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {

                    string sQuery = "select * from callResource " +
                        " where TDefectDetailId = @TDefectDetailId and ResourceTagCode = 'AF-RP' and Active = 1 ";
                    var result = conn.Query<callResource>(sQuery, new { TDefectDetailId = TDefectDetailId }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallResourceByTdefectDetailID() :: Error ", ex);
                }
            }
        }
        public List<PointURL> GetFloorDistinct(string CateType)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    if (CateType == "H")
                    {
                        string sQuery = " select distinct(point.cate),point_name,MobileFloorIcons.ImageURL,point.floorplantset from point " +
                            " left join MobileFloorIcons on point.cate = MobileFloorIcons.cate " +
                            " where point.producttypecate = 'H'";
                        var result = conn.Query<PointURL>(sQuery).ToList();
                        return result;
                    }
                    else
                    {
                        string sQuery = "select distinct(point.cate),point_name,MobileFloorIcons.ImageURL  from point " +
                            " left join MobileFloorIcons on point.cate = MobileFloorIcons.cate " +
                            "where point.producttypecate = 'V' ";
                        var result = conn.Query<PointURL>(sQuery).ToList();
                        return result;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
                }
            }
        }
        public List<PointURL> GetCallPointByProductCat_Sync(string ProductTypeCate,string Cate)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    if (ProductTypeCate == null || ProductTypeCate == "")
                    {
                        string sQuery = "Select Point.*,MobilePointIcons.ImageURL From Point " +
                            " left join MobilePointIcons on point.comppoint_id = MobilePointIcons.ComppointId " +
                            "  where Point.producttypecate = 'V' ";
                        var result = conn.Query<PointURL>(sQuery).ToList();
                        return result;
                    }
                    else
                    {
                        string sQuery = "Select Point.*,MobilePointIcons.ImageURL From Point " +
                            " left join MobilePointIcons on point.comppoint_id = MobilePointIcons.ComppointId " +
                            " where Point.ProductTypeCate = @ProductTypeCate and Point.cate = @Cate ";
                        var result = conn.Query<PointURL>(sQuery, new { ProductTypeCate = ProductTypeCate , Cate = Cate }).ToList();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
                }
            }
        }
        public Model.DefectAPI.point GetCallPointByPointID_Sync(int? comppoint_id)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                        string sQuery = "Select * From Point " +
                            " where comppoint_id = @comppoint_id  ";
                        var result = conn.Query<Model.DefectAPI.point>(sQuery, new { comppoint_id = comppoint_id }).FirstOrDefault();
                        return result;
                    
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
                }
            }
        }
        public List<GetUnitByProjectReturnObj> GetUnitByProduct(string ProductID ,string SearchText)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string UnitNumber = "";
                    if (UnitNumber == "")
                    {
                        UnitNumber = "%"+ SearchText  +"%";
                    }
                    string FirstName = "";
                    if (FirstName == "")
                    {
                        FirstName = "%" + SearchText + "%";
                    }
                    string LastName = "";
                    if (LastName == "")
                    {
                        LastName = "%" + SearchText + "%";
                    }
                    string AddressNumber = "";
                    if (AddressNumber == "")
                    {
                        AddressNumber = "%" + SearchText + "%";
                    }
                    string sQuery = "";

                    //sQuery = "Select top 50 * From ICON_EntForms_Unit u  " +
                    //"left join View_UnitCustomer c on u.ProductID=c.ProductID and  u.UnitNumber = c.UnitNumber  " +
                    //"where c.ContactID IS NOT NULL And u.ProductID = '@ProductID' " +
                    //" AND ( u.UnitNumber like @UnitNumber OR FirstName like @FirstName OR LastName like @LastName OR AddressNumber like @AddressNumber )";
                    sQuery = "select top 50 * From ICON_EntForms_Unit u " +
                        "left join View_UnitCustomer c on u.ProductID=c.ProductID and  u.UnitNumber = c.UnitNumber  " +
                        "where c.ContactID IS NOT NULL And u.ProductID = @ProductID " +
                        " AND ( u.UnitNumber like @UnitNumber OR FirstName like @FirstName OR LastName like @LastName OR AddressNumber like @AddressNumber )";
                    var result = conn.Query<GetUnitByProjectReturnObj>(sQuery, new
                    {
                        ProductID = ProductID,
                        UnitNumber = FirstName,
                        FirstName = FirstName,
                        LastName = LastName,
                        AddressNumber = AddressNumber
                    }).ToList();
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetUnitByProduct() :: Error ", ex);
                }
            }
        }
        public List<calltype> GetCallCallPoint_Sync()
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select * From callPoint where Active = 1 ";
                    var result = conn.Query<calltype>(sQuery).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
                }
            }
        }
        public List<calltype> GetCallCallType_Sync(string CalltypeID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select * From callPoint where calltype_id = @CalltypeID  And Active = 1 ";
                    var result = conn.Query<calltype>(sQuery,new { CalltypeID = CalltypeID }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
                }
            }
        }
        public ViewUnitCustomer GetViewUnitCustomer(string UnitNumber , string ProductID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "select * from View_UnitCustomer where UnitNumber = @UnitNumber and ProductID = @ProductID ";
                    var result = conn.Query<ViewUnitCustomer>(sQuery, new { UnitNumber = UnitNumber , ProductID = ProductID }).FirstOrDefault();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetViewUnitCustomer() :: Error ", ex);
                }
            }
        }
        public List<GetcallTypeWithArea> GetCallCallTypeWithArea_Sync()
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "select t.calltype_id,t.calltype,callarea_name,a.chk_type,a.responsible,a.producttypecate " +
                           " from calltype t " +
                           " left join callarea a on t.calltype_id = a.calltype_id " +
                           " where t.active = 1";
                    var result = conn.Query<GetcallTypeWithArea>(sQuery).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallCallTypeWithArea_Sync() :: Error ", ex);
                }
            }
        }
        public callTDefect GetCallTDefect_Sync(int TDefectID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select * From callTDefect " +
                        "where TDefectID = @TDefectID And DocIsActive = 1 ";
                    var result = conn.Query<callTDefect>(sQuery, new { TDefectID = TDefectID }).FirstOrDefault();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
                }
            }
        }
        public CallTdefectCheckCustomer GetCallTDefectByUnitNumber_Sync(string UnitNumber)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select c.*,VC.FirstName as CurrentCustomerFirstName , VC.LastName as CurrentCustomerLastName From callTDefect c" +
                        " left join View_UnitCustomer VC on c.ItemID = VC.UnitNumber and c.ProductId = VC.ProductID " +
                        "where c.ItemId = @UnitNumber And DocIsActive = 1 order by CreateDate desc ";
                    var result = conn.Query<CallTdefectCheckCustomer>(sQuery, new { UnitNumber = UnitNumber }).FirstOrDefault();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallTDefectByUnitNumber_Sync() :: Error ", ex);
                }
            }
        }
        public CallTdefectMObj GetCallTDefectByUnitID_Sync(string ProjectCode,string UnitID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select * From callTDefect c " +
                        " left join ICON_EntForms_Unit i on c.ItemId = i.UnitNumber  and c.ProductId = i.ProductID " +
                        " left join View_UnitCustomer VC on i.UnitNumber = VC.UnitNumber and c.ProductId = VC.ProductID " +
                        "where c.ProductId = @ProjectCode and c.ItemId = @UnitID And c.DocIsActive = 1 ";
                    var result = conn.Query<CallTdefectMObj>(sQuery, new { ProjectCode = ProjectCode, UnitID= UnitID }).FirstOrDefault();
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
                }
            }
        }
        public CallTdefectVendorMObj GetCallTDefectVendorByUnitID_Sync(string ProjectCode, string UnitID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select * From callTDefect c " +
                        " left join ICON_EntForms_Unit i on c.ItemId = i.UnitNumber  and c.ProductId = i.ProductID " +
                        " left join View_UnitCustomer VC on i.UnitNumber = VC.UnitNumber and c.ProductId = VC.ProductID " +
                        "where c.ProductId = @ProjectCode and c.ItemId = @UnitID And c.DocIsActive = 1 ";
                    var result = conn.Query<CallTdefectVendorMObj>(sQuery, new { ProjectCode = ProjectCode, UnitID = UnitID }).FirstOrDefault();
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
                }
            }
        }
        public List<GetCallTDefectByProjectObj> GetCallTDefectByProject_Sync(int ProductID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "SELECT callTDefect.* , dbo.ICON_EntForms_Products.Project FROM dbo.callTDefect" +
                        " LEFT JOIN ICON_EntForms_Products ON dbo.callTDefect.ProductId = dbo.ICON_EntForms_Products.ProductID " +
                        " Where callTDefect.ProductID = @ProductID AND DocIsActive = 1 " +
                        " Order By callTDefect.CreateDate desc ";
                    var result = conn.Query<GetCallTDefectByProjectObj>(sQuery, new { ProductID = ProductID }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallTDefectByProject_Sync() :: Error ", ex);
                }
            }
        }
        public List<callTDefectDetail> GetcallTDefectDetail_Sync(int TDefectID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select * From callTDefectDetail " +
                        "where TDefectID = @TDefectID And RowActive = 1 ";
                    var result = conn.Query<callTDefectDetail>(sQuery, new { TDefectID = TDefectID }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetcallTDefectDetail_Sync() :: Error ", ex);
                }
            }
        }
        public callTDefectDetail GetcallTDefectDetailByDetailID_Sync(int TDefectDetailID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select * From callTDefectDetail " +
                        "where TDefectDetailId = @TDefectDetailID And RowActive = 1 ";
                    var result = conn.Query<callTDefectDetail>(sQuery, new { TDefectDetailID = TDefectDetailID }).FirstOrDefault();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetcallTDefectDetailByDetailID_Sync() :: Error ", ex);
                }
            }
        }
        public List<callTDefectDetail> GetcallTDefectDetailByDetailIDList_Sync(string TDefectDetailIDList)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select * From callTDefectDetail " +
                        "where TDefectDetailId in ( @TDefectDetailIDList ) And RowActive = 1 ";
                    var result = conn.Query<callTDefectDetail>(sQuery, new { TDefectDetailIDList = TDefectDetailIDList }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetcallTDefectDetailByDetailIDList_Sync() :: Error ", ex);
                }
            }
        }

        public List<callTDefectDetail> GetcallTDefectDetailByTDefectIDList_Sync(int TDefectID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select * From callTDefectDetail " +
                        "where TDefectID = @TDefectID And RowActive = 1  and CustRoundAuditDueCloseDate = null ";
                    var result = conn.Query<callTDefectDetail>(sQuery, new { TDefectID = TDefectID }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetcallTDefectDetailByDetailIDList_Sync() :: Error ", ex);
                }
            }
        }
        public List<CallTdefectDetailCustom> GetcallTDefectDetailShow_Sync(int TDefectID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "select * from callTDefectDetail " +
                                    "left join callarea on callTDefectDetail.CallAreaId = callarea.callarea_id " +
                                    "left join point on callTDefectDetail.CallPointId = point.comppoint_id and callTDefectDetail.CallSubPointId = point.cate " +
                                    "left join callStaComplain on callStaComplain.IdStaComplain = callTDefectDetail.TDefectDetailStatus "+ 
                        "where callTDefectDetail.TDefectID = @TDefectID And callTDefectDetail.RowActive = 1 ";
                    var result = conn.Query<CallTdefectDetailCustom>(sQuery, new { TDefectID = TDefectID }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetcallTDefectDetail_Sync() :: Error ", ex);
                }
            }
        }
        public List<GetUnitByProjectReturnObj> GetRecentcallTDefect_Sync(string EmpCode)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "select top 20 * from View_UnitCustomer where productID in " +
                                    " (Select top 20 ProductId From callTDefect " +
                                    " where UpdateUserID = '@EmpCode' And DocIsActive = 1 Order by 1 desc) AND TransferDate is not null order by 1 desc";
                    var result = conn.Query<GetUnitByProjectReturnObj>(sQuery, new { EmpCode = EmpCode }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetcallTDefectDetail_Sync() :: Error ", ex);
                }
            }
        }
        public List<calldescription> GetCallDescriptionByCallAreaID_Sync(int CallAreaID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select * From calldescriptions where callarea_id = @CallAreaID ";
                    var result = conn.Query<calldescription>(sQuery, new { CallAreaID = CallAreaID }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
                }
            }
        }
        public List<Floor> GetFloorsByProjectTower(string ProjectID,string TowerID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "SELECT DISTINCT(FloorID) FROM  ICON_EntForms_Unit WHERE ProductID = @ProjectID AND TowerID = @TowerID";
                    var result = conn.Query<Floor>(sQuery, new { ProjectID = ProjectID , TowerID = TowerID }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetFloorsByProject() :: Error ", ex);
                }
            }
        }
        public List<Tower> GetTowersByProject(string ProjectID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "SELECT DISTINCT(TowerID) FROM  ICON_EntForms_Unit WHERE ProductID = @ProjectID ";
                    var result = conn.Query<Tower>(sQuery, new { ProjectID = ProjectID }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetTowersByProject() :: Error ", ex);
                }
            }
        }
        public Model.DefectAPISync.vwProject GetVwProjects(string ProjectID)
        {
            using (IDbConnection conn = AuthConnection)
            {
                try
                {
                    string sQuery = "SELECT * FROM  vw_Project WHERE ProjectCode = @ProjectID ";
                    var result = conn.Query<Model.DefectAPISync.vwProject>(sQuery, new { ProjectID = ProjectID }).FirstOrDefault();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetVwProjects() :: Error ", ex);
                }
            }
        }
    }
}

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
        string TAG = "MasterRepository";

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
        public List<callarea> GetCallAreaByProductCat_Sync(string ProductTypeCate)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    if (ProductTypeCate == null || ProductTypeCate == "")
                    {
                        string sQuery = "Select * From callarea where Active = 1 ";
                        var result = conn.Query<callarea>(sQuery).ToList();
                        return result;
                    }
                    else
                    {
                        string sQuery = "Select * From callarea where ProductTypeCate = @ProductTypeCate And Active = 1 ";
                        var result = conn.Query<callarea>(sQuery, new { ProductTypeCate = ProductTypeCate }).ToList();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
                }
            }
        }
        public List<point> GetCallPointByProductCat_Sync(string ProductTypeCate)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    if (ProductTypeCate == null || ProductTypeCate == "")
                    {
                        string sQuery = "Select * From Point ";
                        var result = conn.Query<point>(sQuery).ToList();
                        return result;
                    }
                    else
                    {
                        string sQuery = "Select * From Point where ProductTypeCate = @ProductTypeCate ";
                        var result = conn.Query<point>(sQuery, new { ProductTypeCate = ProductTypeCate }).ToList();
                        return result;
                    }
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
                        UnitNumber = "%"+ SearchText  +"% ";
                    }
                    string FirstName = "";
                    if (FirstName == "")
                    {
                        FirstName = "%" + SearchText + "% ";
                    }
                    string LastName = "";
                    if (LastName == "")
                    {
                        LastName = "%" + SearchText + "% ";
                    }
                    string AddressNumber = "";
                    if (AddressNumber == "")
                    {
                        AddressNumber = "%" + SearchText + "% ";
                    }
                    string sQuery = "";

                    sQuery = "Select top 50 * From ICON_EntForms_Unit u  " +
                    "left join View_UnitCustomer c on u.ProductID=c.ProductID and  u.UnitNumber = c.UnitNumber  " +
                    "where c.ContactID IS NOT NULL And u.ProductID = @ProductID " +
                    " AND ( u.UnitNumber like @UnitNumber OR FirstName like @FirstName OR LastName like @LastName OR AddressNumber like @AddressNumber )";
                    var result = conn.Query<GetUnitByProjectReturnObj>(sQuery, new
                    {
                        ProductID = ProductID,
                        UnitNumber = UnitNumber,
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
        public List<calltype> GetCallCallType_Sync()
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
        public List<GetCallTDefectByProjectObj> GetCallTDefectByProject_Sync(int ProductID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "SELECT callTDefect.* , dbo.ICON_EntForms_Products.Project FROM dbo.callTDefect" +
                        " LEFT JOIN ICON_EntForms_Products ON dbo.callTDefect.ProductId = dbo.ICON_EntForms_Products.ProductID " +
                        " Where callTDefect,ProductID = @ProductID AND DocIsActive = 1 " +
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
                        "where TDefectID = @TDefectID And DocIsActive = 1 ";
                    var result = conn.Query<callTDefectDetail>(sQuery, new { TDefectID = TDefectID }).ToList();
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

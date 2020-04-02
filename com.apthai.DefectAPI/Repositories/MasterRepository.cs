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
                        string sQuery = "Select * From Point where Active = 1 ";
                        var result = conn.Query<point>(sQuery).ToList();
                        return result;
                    }
                    else
                    {
                        string sQuery = "Select * From Point where ProductTypeCate = @ProductTypeCate And Active = 1 ";
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
        public List<ICONEntFormsUnit> GetUnitByProduct(string ProductID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                        string sQuery = "Select * From ICON_EntForms_Unit where ProductID = @ProductID And Active = 1 ";
                        var result = conn.Query<ICONEntFormsUnit>(sQuery, new { ProductID = ProductID }).ToList();
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
    }
}

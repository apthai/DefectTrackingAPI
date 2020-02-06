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
        public List<calltype> GetCallCallType_Sync()
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "Select * From callType where Active = 1 ";
                    var result = conn.Query<calltype>(sQuery).ToList();
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

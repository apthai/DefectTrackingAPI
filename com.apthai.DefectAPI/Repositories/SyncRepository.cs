using Dapper;
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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Extensions;

namespace com.apthai.DefectAPI.Repositories
{
    public class SyncRepository : BaseRepository, ISyncRepository
    {

        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hostingEnvironment;


        // int ConnctionLongMaxQuerryTimeOut = 100;

        public SyncRepository(IHostingEnvironment environment, IConfiguration config) : base(environment, config)
        {
            _config = config;
            _hostingEnvironment = environment;

        }



        //private void proc_SaveResource(
        // Guid SynSessionId, int SynUserId, string SynUserRole,
        // string SynDeviceId,
        // DateTime? SynDate,
        // IDbTransaction tran,
        // ref List<TResource> resources,
        // ref Dictionary<string, int> resourceUdetailTemparyKey
        // )
        //{


        //    foreach (var res in resources)
        //    {

        //        // scan for client multi upload protection
        //        if (res.ResourceId <= 0)
        //        {
        //            var dbItemScan = WebConnection.Query<TResource>("SELECT * FROM T_Resource WHERE RowClientId=@RowClientId", new { RowClientId = res.RowClientId }, tran, true, ConnctionLongQuerryTimeOut).ToList();
        //            if (dbItemScan.Any())
        //                res.ResourceId = dbItemScan.FirstOrDefault().ResourceId;
        //        }


        //        if (res.ResourceId > 0)
        //        {


        //            var restT = WebConnection.Get<TResource>(res.ResourceId, tran) ?? new TResource();
        //            if (restT.ResourceId > 0)
        //            {
        //                var jsonObj = JObject.Parse(JsonConvert.SerializeObject(res));
        //                jsonObj.RemoveFields("ResourceId");
        //                jsonObj.RemoveFields("CreateDeviceId", "CreateUserId", "CreatedDate");
        //                JsonConvert.PopulateObject(jsonObj.ToString(), restT);
        //                restT.RowState = RowStates.Original;
        //                WebConnection.Update<TResource>(restT, tran, ConnctionLongQuerryTimeOut);

        //            }
        //        }
        //        else
        //        {

        //            var udetailMap = resourceUdetailTemparyKey.Where(e => e.Key == res.UDetail_RowClientId).ToList();
        //            if (udetailMap.Count() > 0)
        //            {
        //                res.UDetailId = udetailMap.FirstOrDefault().Value;

        //            }
        //            res.IsActive = true;
        //            res.RowState = RowStates.Original;
        //            var newResourceId = WebConnection.Insert<TResource>(res, tran, ConnctionLongQuerryTimeOut);

        //        }

        //    }


        //}
        

        public async Task<int> TanonchaiJobSample()
        {
            int a = 20;
            int b = 60;
            int c = a + b;
            return c;
        }



        
    }

}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
//using com.apthai.CoreApp.API.Base.Infrastructure;
using System.Configuration;
//using com.apthai.CoreApp.Model.DefectAPI;
using Newtonsoft.Json.Linq;
using System.Data;
using Newtonsoft.Json.Extensions;
using Newtonsoft.Json;
using com.apthai.DefectAPI.Model.DefectAPI;
using com.apthai.DefectAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using com.apthai.DefectAPI.Repositories;
using Microsoft.Extensions.Configuration;
using com.apthai.DefectAPI.Models;
//using com.apthai.CoreApp.Model;
//using System.Web.Hosting;
//using com.apthai.CoreApp.Model.CustomModel;
//using com.apthai.CoreApp.Model.DefectAPI.CustomModel;
//using com.apthai.CoreApp.Model.CS.CustomModel;


namespace com.apthai.CoreApp.Data.Services
{
    public partial class DataCrawlerServices : IDataCrawlerServices
    {

        private readonly IMasterRepository _masterRepo;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        private readonly int StorageServerId;

        public DataCrawlerServices(IHostingEnvironment environment, IMasterRepository masterRepo, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _masterRepo = masterRepo;
            _hostingEnvironment = environment;
            _unitOfWork = unitOfWork;
            _config = config;
            StorageServerId = config.GetValue<int>("AppSettings:StorageServerId");

        }
    }
}
        //public (MProject, List<SyncModelUnitData>, List<DefectAPI.Model.DefectAPISync.TResource>) UnitDataCrawler(Guid sessionId, IEnumerable<JToken> jsonListObjects, DateTime syncDate, string sessionUserId, string userRole, Dictionary<string, string> fileTemp, ref List<int> returnIds, ref Dictionary<string, string> files_copy)
//        {




//            var EffectRows = new List<int>();

//            MProject head = new MProject();
//            var rows_detail = new List<SyncModelUnitData>();
//            //var  rows_detail_po = new List<SyncPO>();
//            var rows_resourcce = new List<DefectAPI.Model.DefectAPISync.TResource>();
//            Dictionary<string, string> resource_mapping = new Dictionary<string, string>();
//            Dictionary<string, string> resource_mappingUF = new Dictionary<string, string>();

//            var storageServerIdConfig = _config.GetValue<int>("AppSettings:StorageServerId");
//            //int StorageServerId = Int32.Parse("0" + _app ConfigurationManager.AppSettings["api.qis.StorageServerId"]);
//            int uploadUserId = Int32.Parse("0" + sessionUserId);



//            foreach (JObject result in jsonListObjects)
//            {

//                head = new MProject();
//                rows_detail = new List<SyncModelUnitData>();
//                //rows_detail_po = new List<SyncPO>();
//                rows_resourcce = new List<DefectAPI.Model.DefectAPISync.TResource>();
//                resource_mapping = new Dictionary<string, string>();


//                var JsonTQIS = result["HEADER"];
//                if (JsonTQIS != null)
//                {
//                    head = JsonConvert.DeserializeObject<MProject>(JsonTQIS.ToString());
//                    var details = result["DETAILS"].Children();



//                    //if(HttpContext.Current.IsDebuggingEnabled)
//                    //    rows_resourcce = new List<TResource>();


//                    if (details.Any())
//                        foreach (var detail in details)
//                        {

//                            var it = new SyncModelUnitData("U");

//                            it.ProjectID = detail.GetValue<int>("ProjectID");
//                            it.UnitID = detail.GetValue<int>("UnitID");
//                            it.ModelID = detail.GetValue<int>("ModelID");
//                            it.POID = null;

//                            var _uPhasesJsonString = detail.GetValue<string>("UPhases");
//                            var _uHeadersJsonString = detail.GetValue<string>("UHeaders");
//                            var _uDetailsJsonString = detail.GetValue<string>("UDetails");
//                            var _uPlanPointImagesJsonString = detail.GetValue<string>("UPlanPointImages");
//                            var _uDefectDetailsJsonString = detail.GetValue<string>("UDefectDetails");


//                            it.UPhases = string.IsNullOrEmpty(_uPhasesJsonString) ? new List<SyncUPhase>() : JsonConvert.DeserializeObject<List<SyncUPhase>>(_uPhasesJsonString);
//                            it.UHeaders = string.IsNullOrEmpty(_uHeadersJsonString) ? new List<SyncUHeader>() : JsonConvert.DeserializeObject<List<SyncUHeader>>(_uHeadersJsonString);
//                            it.UDetails = string.IsNullOrEmpty(_uDetailsJsonString) ? new List<SyncUDetail>() : JsonConvert.DeserializeObject<List<SyncUDetail>>(_uDetailsJsonString);
//                            it.UDefectDetails = string.IsNullOrEmpty(_uDefectDetailsJsonString) ? new List<SyncUDefectDetail>() : JsonConvert.DeserializeObject<List<SyncUDefectDetail>>(_uDefectDetailsJsonString);


//                            it.LastSyncDate = syncDate;
//                            it.UPlanPointImages = string.IsNullOrEmpty(_uPlanPointImagesJsonString) ? new List<SyncUPlanPointImage>() : JsonConvert.DeserializeObject<List<SyncUPlanPointImage>>(_uPlanPointImagesJsonString) ?? new List<SyncUPlanPointImage>();

//                            it.LastSyncDate = syncDate;


//                            if (it.UPlanPointImages != null)
//                                foreach (var p in it.UPlanPointImages)
//                                {
//                                    p.RowSyncSessionId = sessionId;
//                                }


//                            if (it.UDetails.Any() || it.UHeaders.Any() || it.UPhases.Any() || it.UFFacility.Any())
//                                rows_detail.Add(it);


//                        }

//                    // Phase 2
//                    if (result["DETAILS_PROJ"] != null)
//                    {

//                        var details_proj = result["DETAILS_PROJ"].Children();
//                        if (details_proj.Any())
//                        {
//                            foreach (var detail in details_proj)
//                            {



//                                var it = new SyncModelUnitData("P");

//                                it.ProjectID = detail.GetValue<int>("ProjectID");
//                                it.UnitID = 0;// detail.GetValue<int>("UnitID");
//                                it.ModelID = detail.GetValue<int>("ModelID");
//                                it.POID = detail.GetValue<int?>("POID");

//                                it.FacilityType = detail.GetValue<string>("FacilityType");
//                                it.IssueDocNo = detail.GetValue<string>("IssueDocNo");
//                                var _uFFacilityJsonString = detail.GetValue<string>("UFFacility");
//                                var _uPhasesJsonString = detail.GetValue<string>("UFPhases");
//                                var _uHeadersJsonString = detail.GetValue<string>("UFHeaders");
//                                var _uDetailsJsonString = detail.GetValue<string>("UFDetails");


//                                it.UFFacility = string.IsNullOrEmpty(_uFFacilityJsonString) ? new List<SyncUFFacility>() : JsonConvert.DeserializeObject<List<SyncUFFacility>>(_uFFacilityJsonString);
//                                it.UFPhases = string.IsNullOrEmpty(_uPhasesJsonString) ? new List<SyncUFPhase>() : JsonConvert.DeserializeObject<List<SyncUFPhase>>(_uPhasesJsonString);
//                                it.UFHeaders = string.IsNullOrEmpty(_uHeadersJsonString) ? new List<SyncUFHeader>() : JsonConvert.DeserializeObject<List<SyncUFHeader>>(_uHeadersJsonString);
//                                it.UFDetails = string.IsNullOrEmpty(_uDetailsJsonString) ? new List<SyncUFDetail>() : JsonConvert.DeserializeObject<List<SyncUFDetail>>(_uDetailsJsonString);
//                                it.LastSyncDate = syncDate;

//                                if (it.UFFacility.Any() || it.UFDetails.Any() || it.UFHeaders.Any() || it.UFPhases.Any() || it.UDefectDetails.Any() )
//                                    rows_detail.Add(it);

//                            }

//                        }
//                    }

//                    //if (result["DETAILS_PO"] != null)
//                    //{
//                    //    var details_po = result["DETAILS_PO"].Children();
//                    //if (details_po.Any())
//                    //{
//                    //    foreach (var detail in details_po)
//                    //    {


//                    //        var JsonData = detail.SelectToken("JsonData");
//                    //        var it = JsonConvert.DeserializeObject<SyncPO>(JsonData.ToString());
//                    //        if (it.RowState == RowStates.Modified || it.RowState == null)
//                    //        {
//                    //            if(!rows_detail_po.Where(e=>e.POID == it.POID).Any())
//                    //                rows_detail_po.Add(it);

//                    //        }
//                    //    }

//                    //}

//                    if (result["RESOURCES"] != null)
//                    {
//                        var resources = result["RESOURCES"].Children();

//                        if (resources.Any())
//                            foreach (JToken res in resources)
//                            {


//                                res.RemoveFields("Tag");

//                                var reWritePath = res.GetValue<string>("FilePath");
//                                if (!string.IsNullOrEmpty(reWritePath))
//                                {
//                                    reWritePath = reWritePath.Replace("\\", "/").TrimStart('/');

//                                }

//                                var resObj = JsonConvert.DeserializeObject<DefectAPI.Model.DefectAPISync.TResource>(res.ToString(), new JsonSerializerSettings());
//                                if ( resObj.ResourceId > 0)
//                                {
//                                    //var dbItem = UnitOfWork.QIS.SyncRepository.DefectRepository.GetCallResource(resObj.ResourceId);
//                                    //JsonConvert.PopulateObject(res.ToString(), dbItem, serializerSettings);
//                                    //resObj = dbItem;
//                                }
//                                else
//                                {
//                                    resObj.FilePath = reWritePath;
//                                    resObj.StorageServerId = StorageServerId;

//                                }
//                                rows_resourcce.Add(resObj);
//                            }

//                    }


//                }



//            }

//            //fill sessionid to transection
//            foreach (var s in rows_detail)
//            {

//                foreach (var u in s.UPhases)
//                    u.SynSessionId = sessionId;

//                foreach (var h in s.UHeaders)
//                    h.SynSessionId = sessionId;

//                foreach (var d in s.UDetails)
//                    d.SynSessionId = sessionId;

//                foreach (var d in s.UDefectDetails)
//                    d.SynSessionId = sessionId;

//            }

//            //returnIds = EffectRows;

//            return (head, rows_detail, rows_resourcce);

//        }



//    }
//}
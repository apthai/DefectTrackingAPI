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
using com.apthai.DefectAPI.Model.DefectAPI;
using com.apthai.DefectAPI.CustomModel;
using com.apthai.DefectAPI.Services;
using com.apthai.DefectAPI.Repositories.Interfaces;
using static com.apthai.DefectAPI.CustomModel.RequestReportModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.Globalization;

namespace com.apthai.DefectAPI.Repositories
{
    public class SyncRepository : BaseRepository, ISyncRepository
    {

        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMasterRepository _masterRepository;
        MinioServices minio;
        // int ConnctionLongMaxQuerryTimeOut = 100;

        public SyncRepository(IHostingEnvironment environment, IConfiguration config) : base(environment, config)
        {
            _config = config;
            _hostingEnvironment = environment;
            minio = new MinioServices();
            _masterRepository = new MasterRepository(_hostingEnvironment, _config);
        }

        public bool InsertCallResource(callResource data)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Insert(data, tran);
                    tran.Commit();


                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.updateMUnit_Sync() :: Error ", ex);
                }
            }
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

        public async Task GenerateReport(ParamReportModel model)
        {
            try
            {
                var reportName = "RPT_ReceiveUnit_Horizontal";
                if (model.ProjectType.Equals("V"))
                {
                    reportName = "RPT_ReceiveUnit_Vertical";
                }

                // V แนวสูง
                // H แนวราบ

                // RPT_ReceiveUnit_H แนวสูง
                // RPT_ReceiveUnit   แนวราบ
                await UpdatePathUrlFile(model.TDefectId);
                string bucketName = Environment.GetEnvironmentVariable("Minio_DefaultBucket") ?? UtilsProvider.AppSetting.MinioDefaultBucket;
                minio = new MinioServices();
                bool insertPDF = false;
                List<callResource> Signature = _masterRepository.GetSignatureByTdefectID(model.TDefectId);
                string lcSigAf = Signature.Where(w => w.ResourceTagCode == "SAL-LC-AF").Any() ? Signature.Where(w => w.ResourceTagCode == "SAL-LC-AF").FirstOrDefault().FilePath : null;
                string cusSigBf = Signature.Where(w => w.ResourceTagCode == "CUST-BF").Any() ? Signature.Where(w => w.ResourceTagCode == "CUST-BF").FirstOrDefault().FilePath : null;
                string cusSigAf = Signature.Where(w => w.ResourceTagCode == "CUST-AF").Any() ? Signature.Where(w => w.ResourceTagCode == "CUST-AF").FirstOrDefault().FilePath : null;
                string conSigBf = Signature.Where(w => w.ResourceTagCode == "CON-MGR-BF").Any() ? Signature.Where(w => w.ResourceTagCode == "CON-MGR-BF").FirstOrDefault().FilePath : null;
                string conSigAf = Signature.Where(w => w.ResourceTagCode == "CON-MGR-AF").Any() ? Signature.Where(w => w.ResourceTagCode == "CON-MGR-AF").FirstOrDefault().FilePath : null;
                string cusSigRe = Signature.Where(w => w.ResourceTagCode == "CUST-RECE").Any() ? Signature.Where(w => w.ResourceTagCode == "CUST-RECE").FirstOrDefault().FilePath : null;

                var listCus = new List<string>() { "CUST", "CUST-AF", "CUST-RECE" };
                var orderCusSignature = Signature.Where(w => listCus.Contains(w.ResourceTagCode)).OrderBy(o => o.CreateDate).ToList();

                var cusReDatetime = "";
                if (orderCusSignature.Count() == 2)
                {
                    if (orderCusSignature[1].ResourceTagCode == "CUST-RECE")
                    {
                        var signDatetime = Signature.Where(w => w.ResourceTagCode == "CUST-RECE").FirstOrDefault().CreateDate;
                        cusReDatetime = signDatetime.Value.ToString("d/M/yyyy");

                        if (model.ProjectType.Equals("V"))
                        {
                            reportName = "RPT_ReceiveUnit_Vertical_CUST_RECE";
                        }
                        else
                        {
                            reportName = "RPT_ReceiveUnit_Horizontal_CUST_RECE";
                        }
                    }
                    else
                    {
                        if (model.ProjectType.Equals("V"))
                        {
                            reportName = "RPT_ReceiveUnit_Vertical";
                        }
                        else
                        {
                            reportName = "RPT_ReceiveUnit_Horizontal";
                        }
                    }
                }
                else if (orderCusSignature.Count() == 3)
                {
                    if (orderCusSignature[2].ResourceTagCode == "CUST-RECE")
                    {
                        var signDatetime = Signature.Where(w => w.ResourceTagCode == "CUST-RECE").FirstOrDefault().CreateDate;
                        cusReDatetime = signDatetime.Value.ToString("d/M/yyyy");
                        if (model.ProjectType.Equals("V"))
                        {
                            reportName = "RPT_ReceiveUnit_Vertical_CUST_RECE";
                        }
                        else
                        {
                            reportName = "RPT_ReceiveUnit_Horizontal_CUST_RECE";
                        }
                    }
                    else
                    {
                        if (model.ProjectType.Equals("V"))
                        {
                            reportName = "RPT_ReceiveUnit_Vertical";
                        }
                        else
                        {
                            reportName = "RPT_ReceiveUnit_Horizontal";
                        }
                    }
                }
                else
                {
                    if (orderCusSignature[0].ResourceTagCode == "CUST-RECE")
                    {
                        var signDatetime = Signature.Where(w => w.ResourceTagCode == "CUST-RECE").FirstOrDefault().CreateDate;
                        cusReDatetime = signDatetime.Value.ToString("d/M/yyyy");
                        if (model.ProjectType.Equals("V"))
                        {
                            reportName = "RPT_ReceiveUnit_Vertical_CUST_RECE";
                        }
                        else
                        {
                            reportName = "RPT_ReceiveUnit_Horizontal_CUST_RECE";
                        }
                    }
                    else
                    {
                        if (model.ProjectType.Equals("V"))
                        {
                            reportName = "RPT_ReceiveUnit_Vertical";
                        }
                        else
                        {
                            reportName = "RPT_ReceiveUnit_Horizontal";
                        }
                    }
                }

                string lcSigAffilePath = String.IsNullOrEmpty(lcSigAf) ? null : await minio.GetFileUrlAsync(bucketName, lcSigAf);
                string cusSigBfFilePath = String.IsNullOrEmpty(cusSigBf) ? null : await minio.GetFileUrlAsync(bucketName, cusSigBf);
                string cusSigAfFilePath = String.IsNullOrEmpty(cusSigAf) ? null : await minio.GetFileUrlAsync(bucketName, cusSigAf);
                string conSigBfFilePath = String.IsNullOrEmpty(conSigBf) ? null : await minio.GetFileUrlAsync(bucketName, conSigBf);
                string conSigAfFilePath = String.IsNullOrEmpty(conSigAf) ? null : await minio.GetFileUrlAsync(bucketName, conSigAf);
                string cusSigReFilePath = String.IsNullOrEmpty(cusSigRe) ? null : await minio.GetFileUrlAsync(bucketName, cusSigRe);



                var requestMode = new RequestReportModel()
                {
                    Folder = "defect",
                    FileName = reportName,
                    Server = Environment.GetEnvironmentVariable("ReportServer") ?? UtilsProvider.AppSetting.ReportServer,
                    DatabaseName = Environment.GetEnvironmentVariable("ReportDataBase") ?? UtilsProvider.AppSetting.ReportDataBase,
                    UserName = Environment.GetEnvironmentVariable("ReportUserName") ?? UtilsProvider.AppSetting.ReportUserName,
                    Password = Environment.GetEnvironmentVariable("ReportPassword") ?? UtilsProvider.AppSetting.ReportPassword,
                    Parameters = new List<ParameterReport>()
                            {
                               new ParameterReport(){Name="@TDefectId",Value=model.TDefectId.ToString()},
                               new ParameterReport(){Name="@CustRoundAuditNo",Value="1"},
                               new ParameterReport(){Name="@CON_MGR_AF_URL",Value=conSigAfFilePath},
                               new ParameterReport(){Name="@CON_MGR_BF_URL",Value=conSigBfFilePath},
                               new ParameterReport(){Name="@CUST_AF_URL",Value=cusSigAfFilePath},
                               new ParameterReport(){Name="@CUST_BF_URL",Value=cusSigBfFilePath},
                               new ParameterReport(){Name="@CUST_RECE",Value=cusSigReFilePath},
                               new ParameterReport(){Name="@SAL_LC_AF",Value=lcSigAffilePath},
                               new ParameterReport(){Name="@CUS_RECE_SIGN_DATE",Value=cusReDatetime}
                            }
                };
                ResponsetReportModel resultObject = new ResponsetReportModel();
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, 1000);
                    var urlReport = UtilsProvider.AppSetting.ReportURL;
                    var reportKey = Environment.GetEnvironmentVariable("ReportKey") ?? UtilsProvider.AppSetting.ReportKey;
                    var Content = new StringContent(JsonConvert.SerializeObject(requestMode));
                    Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    Content.Headers.Add("api_accesskey", reportKey);
                    var response = await client.PostAsync(urlReport, Content);
                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();
                        var result = await response.Content.ReadAsStringAsync();
                        resultObject = JsonConvert.DeserializeObject<ResponsetReportModel>(result);
                    }
                    client.Dispose();
                }
                await Task.Delay(3000);
                long sizeFile = 0;
                var fullUrl = "";
                var path = $"{model.ProjectCode}/{model.UnitNo}/DefectDocument";
                if (resultObject.Success)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage resDownload = await client.GetAsync(resultObject.URL.ToString()).ConfigureAwait(false);
                        HttpContent content = resDownload.Content;

                        // ... Read the string.
                        var result = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        Stream stream = new MemoryStream(result);
                        var file = new FormFile(stream, 0, stream.Length, null, resultObject.FileName)
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = "application/pdf"
                        };
                        sizeFile = file.Length;
                        var resultMinio = await minio.UploadFile(file, path, resultObject.FileName);
                        fullUrl = resultMinio.Url;

                        client.Dispose();
                    }

                    callResource callResourcePDF = new callResource();
                    callResourcePDF.FilePath = $"{path}/{resultObject.FileName}";
                    callResourcePDF.FileLength = sizeFile;
                    callResourcePDF.CreateDate = DateTime.Now;
                    callResourcePDF.RowState = "Original";
                    callResourcePDF.ResourceType = 8;
                    callResourcePDF.ResourceTagCode = "1";
                    callResourcePDF.ResourceTagSubCode = "1";
                    callResourcePDF.ResourceGroupSet = null;
                    callResourcePDF.StorageServerId = 1400;
                    callResourcePDF.ResourceGroupOrder = 0;
                    callResourcePDF.TDefectDetailId = 0;
                    callResourcePDF.TDefectId = (int)model.TDefectId;
                    callResourcePDF.ProjectNo = model.ProjectCode;
                    callResourcePDF.SerialNo = model.UnitNo;
                    callResourcePDF.Active = true;
                    callResourcePDF.FullFilePath = fullUrl;
                    callResourcePDF.ExpirePathDate = DateTime.Now.AddDays(6); ;
                    insertPDF = InsertCallResource(callResourcePDF);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ReplaceWithPublicURL(string url)
        {
            //string _minioEndpoint = "http://192.168.2.29:9001";
            string _minioEndpoint = Environment.GetEnvironmentVariable("Minio_Endpoint");
            if (_minioEndpoint == null)
            {
                _minioEndpoint = UtilsProvider.AppSetting.MinioEndpoint;
            }
            //string _tempBucket = "timeattendence";
            string PublicMinioURL = Environment.GetEnvironmentVariable("MinioPublicEndpoint");
            if (PublicMinioURL == null)
            {
                PublicMinioURL = UtilsProvider.AppSetting.MinioPublicEndpoint;
            }
            if (!string.IsNullOrEmpty(PublicMinioURL))
            {
                url = url.Replace("https://", "");
                url = url.Replace("http://", "");

                url = url.Replace(_minioEndpoint, PublicMinioURL);
            }
            return url;
        }

        private async Task<bool> UpdatePathUrlFile(int TdefectId)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string bucketName = Environment.GetEnvironmentVariable("Minio_DefaultBucket") ?? UtilsProvider.AppSetting.MinioDefaultBucket;
                    var queryString = String.Format(@"SELECT
	                                callResource.*
                                FROM
	                                dbo.callTDefectDetail
	                                INNER JOIN
	                                dbo.callResource
	                                ON 
		                                callTDefectDetail.TDefectDetailId = callResource.TDefectDetailId
                                WHERE
	                                callTDefectDetail.TDefectId = {0}", TdefectId.ToString());

                    var listResource = conn.Query<callResource>(queryString).ToList();

                    minio = new MinioServices();
                    foreach (var element in listResource)
                    {
                        element.FullFilePath = await minio.GetFileUrlAsync(bucketName, element.FilePath);
                        element.ExpirePathDate = DateTime.Now.AddDays(6);
                    }

                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Update(listResource, tran);
                    tran.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}

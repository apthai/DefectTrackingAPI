using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using com.apthai.DefectAPI.Model;
using com.apthai.DefectAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using Ionic.Zip;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using com.apthai.DefectAPI.Repositories;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using com.apthai.DefectAPI.Model.DefectAPI;
using Microsoft.Extensions.Configuration;
using com.apthai.DefectAPI.Configuration;
using Microsoft.AspNetCore.StaticFiles;

namespace com.apthai.DefectAPI.Controllers
{
    public class BaseController : ControllerBase
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        //private List<MStorageServer> _QISStorageServer;
        protected AppSettings _appSetting;

        public BaseController() {


            _hostingEnvironment  = UtilsProvider.HostingEnvironment;
            _config = UtilsProvider.Config;
            _appSetting = UtilsProvider.AppSetting;
            _unitOfWork = new UnitOfWork(_hostingEnvironment, _config);          

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }



        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> GetQISResourceStoragePhysicalPathAsync(int? StorageServerId)
        {

            return await GetQISResourceStoragePhysicalPathAsync(StorageServerId, null);

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<String> GetQISResourceStoragePhysicalPathAsync(int? StorageServerId, string FilePath)
        {

            string path = "";
            //string baserootPath = _appSetting.PictureRootURL;
            string baserootPath = Environment.GetEnvironmentVariable("PictureRootURL");
            path = baserootPath + FilePath;
            //if (!StorageServerId.HasValue)
            //    return path;

            //if (_QISStorageServer == null)
            //{
            //    var dtServer =  await _unitOfWork.MasterRepository.GetStorageServer();
            //    _QISStorageServer = dtServer.ToList();
            //}

            //var storageServer = _QISStorageServer.Where(e => e.StorageServerId == StorageServerId).FirstOrDefault();

            //if (storageServer != null)
            //{
            //    if (string.IsNullOrEmpty(FilePath))
            //        path = storageServer.StoragePhysicalPath.TrimEnd('\\');
            //    else
            //        path = string.Format("{0}\\{1}", storageServer.StoragePhysicalPath.TrimEnd('\\'), FilePath.TrimStart('\\'));
            //}
            return path;

        }


        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<string> GetQISFileStorageUrlAsync(int? StorageServerId, string FilePath)
        //{


        //    string url = "";


        //    if (!StorageServerId.HasValue)
        //        return url;

        //    if (_QISStorageServer == null)
        //        _QISStorageServer = (await _unitOfWork.MasterRepository.GetStorageServer()).ToList();


        //    var storageServer = _QISStorageServer.Where(e => e.StorageServerId == StorageServerId).FirstOrDefault();

        //    if (storageServer != null)
        //    {
        //        url = string.Format("{0}/{1}", storageServer.StorageServerRootUrl.TrimEnd('/'), FilePath.TrimStart('/'));
        //    }
        //    return url;


        //}

        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<string> GetQISResourceStorageUrlAsync(int? StorageServerId, string FilePath)
        //{

        //    string path = "";

        //    if (!StorageServerId.HasValue)
        //        return path;

        //    if (_QISStorageServer == null)
        //        _QISStorageServer = (await _unitOfWork.MasterRepository.GetStorageServer()).ToList();

        //    var storageServer = _QISStorageServer.Where(e => e.StorageServerId == StorageServerId).FirstOrDefault();

        //    if (storageServer != null)
        //    {
        //        if (string.IsNullOrEmpty(FilePath))
        //            path = storageServer.StorageServerRootUrl.TrimEnd('/');
        //        else
        //            path = string.Format("{0}/{1}", storageServer.StorageServerRootUrl.TrimEnd('/'), FilePath.TrimStart('/'));
        //    }
        //    return path;

        //}




    }

  
}

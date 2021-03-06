﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.DefectAPI.Configuration
{
    public class AppSettings
    {
        public int StorageServerId { get; set; }
        public string AuthorizeEndpointUrlAddress { get; set; }
        public string BaseRootPath { get; set; }
        public string SoapHeaderUsername { get; set; }
        public string SoapHeaderPassword { get; set; }
        public string TresourceFilePath { get; set; }
        public string LogPath { get; set; }
        public string PictureRootURL { get; set; }
        public string ApiKey { get; set; }
        public string ApiToken { get; set; }
        public string AuthorizeURL { get; set; }
        public string ReportURL { get; set; }
        public string ReportServer { get; set; }
        public string ReportUserName { get; set; }
        public string ReportPassword { get; set; }
        public string ReportDataBase { get; set; }
        public string ReportKey { get; set; }
        public string MinioEndpoint { get; set; }
        public string MinioAccessKey { get; set; }
        public string MinioSecretKey { get; set; }
        public string MinioDefaultBucket { get; set; }
        public string MinioTempBucketName { get; set; }
        public string MinioWithSSL { get; set; }
        public string MinioPublicEndpoint { get; set; }
    }
}

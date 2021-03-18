using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using com.apthai.DefectAPI;
using com.apthai.DefectAPI.CustomModel;
using Minio;

namespace com.apthai.APTimeStamp.Services
{
    public partial class MinioServices 
    {
        private int _expireHours = 24;
        public string _minioEndpoint;
        public string _minioAccessKey;
        public string _minioSecretKey;
        public string _defaultBucket;
        public string _tempBucket;
        private bool _withSSL;
        public string _publicURL;

        //public MinioServices(string minioEndpoint, string minioAccessKey, string minioSecretKey, string defaultBucket, string tempBucket, string publicURL, bool withSSL = false)
        public MinioServices()
        {
            string minioEndpoint = Environment.GetEnvironmentVariable("Minio_Endpoint");
            if (minioEndpoint == null)
            {
                minioEndpoint = UtilsProvider.AppSetting.MinioEndpoint;
            }
            string minioAccessKey = Environment.GetEnvironmentVariable("Minio_AccessKey");
            if (minioAccessKey == null)
            {
                minioAccessKey = UtilsProvider.AppSetting.MinioAccessKey;
            }
            string minioSecretKey = Environment.GetEnvironmentVariable("Minio_SecretKey");
            if (minioSecretKey == null)
            {
                minioSecretKey = UtilsProvider.AppSetting.MinioSecretKey;
            }
            string minioBucketName = Environment.GetEnvironmentVariable("Minio_DefaultBucket");
            if (minioBucketName == null)
            {
                minioBucketName = UtilsProvider.AppSetting.MinioDefaultBucket;
            }
            string minioTempBucketName = Environment.GetEnvironmentVariable("Minio_TempBucket");
            if (minioTempBucketName == null )
            {
                minioTempBucketName = UtilsProvider.AppSetting.MinioTempBucketName;
            }
            string minioWithSSL = Environment.GetEnvironmentVariable("Minio_WithSSL");
            if (minioWithSSL == null)
            {
                minioWithSSL = UtilsProvider.AppSetting.MinioWithSSL;
            }
            //string publicURL = Environment.GetEnvironmentVariable("minio_PublicURL");
            //if (publicURL == null)
            //{
            //    publicURL = UtilsProvider.AppSetting.minio_PublicURL;
            //}
            //FileHelper a = new FileHelper(minioEndpoint, minioAccessKey, minioSecretKey, minioBucketName, minioTempBucketName, publicURL, minioWithSSL == "true");

            _minioEndpoint = minioEndpoint;
            _minioAccessKey = minioAccessKey;
            _minioSecretKey = minioSecretKey;
            _defaultBucket = minioBucketName;
            _tempBucket = minioTempBucketName;
            //_publicURL = publicURL;
            if (minioWithSSL == "true")
            {
                _withSSL = true;
            }
            else
            {
                _withSSL = false;
            }
        }

        public async Task MoveTempFileAsync(string sourceObjectName, string destObjectName)
        {
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            await minio.CopyObjectAsync(_tempBucket, sourceObjectName, _defaultBucket, destObjectName);
            await minio.RemoveObjectAsync(_tempBucket, sourceObjectName);
        }

        public async Task<string> GetFileUrlAsync(string name)
        {
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            var url = await minio.PresignedGetObjectAsync(_defaultBucket, name, (int)TimeSpan.FromHours(_expireHours).TotalSeconds);

            url = ReplaceWithPublicURL(url);

            return url;
        }

        public async Task<string> GetFileUrlAsync(string bucket, string name)
        {
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            var url = await minio.PresignedGetObjectAsync(bucket, name, (int)TimeSpan.FromHours(_expireHours).TotalSeconds);
            //url = (!string.IsNullOrEmpty(_publicURL)) ? url.Replace(_minioEndpoint, _publicURL) : url;
            url = ReplaceWithPublicURL(url);

            return url;
        }

        public async Task<Stream> GetStreamFromUrlAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(url);
                if (result.IsSuccessStatusCode)
                    return await result.Content.ReadAsStreamAsync();
                else
                    return null;
            }
        }

        public async Task<FileUploadResult> UploadFileFromStreamWithTimestamp(Stream fileStream, string filePath, string fileName, string contentType)
        {
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            bool bucketExisted = await minio.BucketExistsAsync(_defaultBucket);

            if (!bucketExisted)
                await minio.MakeBucketAsync(_defaultBucket);

            string timestamp = Convert.ToString(DateTime.Today.Year) + DateTime.Now.ToString("MMdd_HHmmss");

            string objectName = "";

            var fileObj = fileName.Split(".");

            if (fileObj.Length > 1)
                objectName = $"{fileObj[0]}_{timestamp}.{fileObj[1]}";
            else
                objectName = $"{fileName}_{timestamp}";

            objectName = Path.Combine(filePath, objectName);
            objectName = objectName.Replace('\\', '/');

            await minio.PutObjectAsync(_defaultBucket, objectName, fileStream, fileStream.Length, contentType);

            var url = await minio.PresignedGetObjectAsync(_defaultBucket, objectName, (int)TimeSpan.FromHours(_expireHours).TotalSeconds);

            return new FileUploadResult()
            {
                Name = objectName,
                BucketName = _defaultBucket,
                Url = url
            };
        }

        public async Task<FileUploadResult> UploadFileFromStream(Stream fileStream, string filePath, string fileName, string contentType)
        {
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            bool bucketExisted = await minio.BucketExistsAsync(_defaultBucket);
            if (!bucketExisted)
                await minio.MakeBucketAsync(_defaultBucket);

            string objectName = $"{Guid.NewGuid().ToString()}_{fileName}";
            objectName = Path.Combine(filePath, objectName);
            objectName = objectName.Replace('\\', '/');
            await minio.PutObjectAsync(_defaultBucket, objectName, fileStream, fileStream.Length, contentType);

            // expire in 1 day
            var url = await minio.PresignedGetObjectAsync(_defaultBucket, objectName, (int)TimeSpan.FromHours(_expireHours).TotalSeconds);
            //url = (!string.IsNullOrEmpty(_publicURL)) ? url.Replace(_minioEndpoint, _publicURL) : url;
            url = ReplaceWithPublicURL(url);

            return new FileUploadResult()
            {
                Name = objectName,
                BucketName = _defaultBucket,
                Url = url
            };
        }

        public async Task<FileUploadResult> UploadFileFromStreamWithOutGuid(Stream fileStream, string bucketName, string filePath, string fileName, string contentType)
        {
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            bool bucketExisted = await minio.BucketExistsAsync(bucketName);
            if (!bucketExisted)
                await minio.MakeBucketAsync(bucketName);

            string objectName = fileName;
            objectName = Path.Combine(filePath, objectName);
            objectName = objectName.Replace('\\', '/');
            await minio.PutObjectAsync(bucketName, objectName, fileStream, fileStream.Length, contentType);
            // expire in 1 day
            var url = await minio.PresignedGetObjectAsync(bucketName, objectName, (int)TimeSpan.FromHours(_expireHours).TotalSeconds);
            url = ReplaceWithPublicURL(url);
            return new FileUploadResult()
            {
                Name = objectName,
                BucketName = bucketName,
                Url = url
            };
        }

        public async Task<List<string>> GetListFile(string bucketName, string prefix)
        {
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            bool bucketExisted = await minio.BucketExistsAsync(bucketName);
            if (bucketExisted)
            {
                List<string> bucketKeys = new List<string>();
                var observable = minio.ListObjectsAsync(bucketName, prefix);
                IDisposable subscription = observable.Subscribe(
                item => bucketKeys.Add(item.Key),
                ex => throw new Exception("Error", ex),
                () => Console.WriteLine("OnComplete: {0}"));
                observable.Wait();

                return bucketKeys;
            }
            else
            {
                return null;
            }
        }

        public async Task MoveFileAsync(string sourceBucket, string sourceObjectName, string destBucket, string destObjectName)
        {
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            await minio.CopyObjectAsync(sourceBucket, sourceObjectName, destBucket, destObjectName);
        }

        public async Task MoveAndRemoveFileAsync(string sourceBucket, string sourceObjectName, string destBucket, string destObjectName)
        {
            MinioClient minio;

            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            await minio.CopyObjectAsync(sourceBucket, sourceObjectName, destBucket, destObjectName);
            await minio.RemoveObjectAsync(sourceBucket, sourceObjectName);
        }

        public static string GetApplicationRootPath()
        {
            var result = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            result = result.Replace("file:\\", string.Empty);
            result = result.Replace("file:", string.Empty);
            return result;
        }

        public async Task<string> DownLoadToTempFileAsync(string bucketName, string prefix, string filename)
        {
            string pathTempFile = Path.GetTempPath();
            string tempFilename = Guid.NewGuid() + "_" + filename;
            MinioClient minio;
            if (_withSSL)
            {
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            }
            else
            {
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);
            }
            await minio.GetObjectAsync(bucketName, prefix + filename,
                                    (stream) =>
                                    {
                                        using (Stream fs = File.OpenWrite(pathTempFile + tempFilename))
                                        {
                                            stream.CopyTo(fs);
                                        }
                                    });
            return pathTempFile + tempFilename;
        }

        private string ReplaceWithPublicURL(string url)
        {
            if (!string.IsNullOrEmpty(_publicURL))
            {
                url = url.Replace("https://", "");
                url = url.Replace("http://", "");

                url = url.Replace(_minioEndpoint, _publicURL);
            }
            return url;
        }
    }
}

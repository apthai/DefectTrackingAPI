using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using Ionic.Zip;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Drawing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Globalization;
using Serilog;
using com.apthai.DefectAPI.Repositories.Interfaces;
using com.apthai.DefectAPI.Repositories;
using com.apthai.DefectAPI.CustomModel;
using com.apthai.DefectAPI.Model.DefectAPI;

namespace com.apthai.DefectAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : BaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _config;
        private readonly IMasterRepository _masterRepository;
        private readonly IAuthenticationModule _authorizeService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISyncRepository _syncRepository;
        public ImageController(IHostingEnvironment environment, IMasterRepository masterRepo, IUnitOfWork unitOfWork, IConfiguration config, ISyncRepository syncRepository)
        {
            _masterRepository = masterRepo;
            _hostingEnvironment = environment;
            _unitOfWork = unitOfWork;
            _syncRepository = syncRepository;
            _config = config;

        }


        //        API Upload Image
        //POST Param
        //String RowClientID
        //String PlanImageName
        //File PlanImage
        //String UserID

        //API Mapping Image
        //POST Param
        //String[] RowClientIDs
        //String[] UnitIDs

        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost]
        [Route("upload")]
        [Consumes("multipart/form-data")]
        public async Task<object> uploadPicture([FromForm] ParamUploadImage data)
        {
            int a = 0;
            //List<TResource> TresourceData = JsonConvert.DeserializeObject<List<TResource>>(data.Resource);
            //string StroageID = base._appSetting.StorageServerId.ToString();

            //com.apthai.QISAPINETCore.Model.QIS.MStorageServer StorageData = _ResourceRepo.GetStorageServerDetail(StroageID);

            //var UploadLoctaion = StorageData.StoragePhysicalPath;
            int SuccessUploadCount = 0;
            int count = 0;
            for (int i = 0; i < data.Files.Count(); i++)
            {
                callResource callResourceDate = new callResource();
                if (data.Files != null && data.Files.Count() > 0)
                {
                    // -- New ---- for Docker
                    var yearPath = DateTime.Now.Year;
                    var MonthPath = DateTime.Now.Month;
                    var dirPath1 = $"{yearPath}/{MonthPath}";
                    int dataPath = 0;
                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "data", "uploads", dirPath1);

                    string FileBinary;


                    long size = data.Files.Sum(f => f.Length);
                    string FileExtension = Path.GetExtension(data.Files[i].FileName);  // -------------- > Get File Extention
                    var fileName = data.Files[i].FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                    var dirPath = $"{yearPath}\\M{dataPath}";

                    // --- New Docker ----
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    if (data.Files[i].Length > 0)
                    {
                        var filePath = Path.Combine(uploads, data.Files[i].FileName);
                        var message = "";
                        if (System.IO.File.Exists(filePath))
                        {
                            string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                            string extension = Path.GetExtension(filePath);
                            string path = Path.GetDirectoryName(filePath);
                            string newFullPath = filePath;
                            string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                            newFullPath = Path.Combine(path, tempFileName + extension);
                            using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                            {
                                message = data.Files[i].Length.ToString();
                                await data.Files[i].CopyToAsync(fileStream);
                                fileName = tempFileName + extension;

                            }
                        }
                        else
                        {
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                message = data.Files[i].Length.ToString();
                                await data.Files[i].CopyToAsync(fileStream);
                            }
                        }

                        // -- -End New -----
                        //if (System.IO.File.Exists(savePath.FullName))
                        if (System.IO.File.Exists(filePath))
                        {
                            string FileExtention = Path.GetExtension(filePath);
                            // ----- Old -----
                            //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                            // ----- New Docker -----
                            callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                            callResourceDate.FileLength = size;
                            callResourceDate.CreateDate = DateTime.Now;
                            callResourceDate.RowState = "Original";
                            callResourceDate.ResourceType = data.ResourceType;
                            callResourceDate.ResourceTagCode = data.ResourceTagCode;
                            callResourceDate.ResourceGroupSet = null;
                            callResourceDate.ResourceGroupOrder = 0;
                            callResourceDate.TDefectDetailId = data.TDefectDetailId == "" ? 0 : Convert.ToInt32(data.TDefectDetailId);
                            callResourceDate.ProjectNo = data.ProjectCode;
                            callResourceDate.SerialNo = data.UnitNo;
                            //TresourceData[i].FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                            //TresourceData[i].FileLength = size;
                            //TresourceData[i].CreatedDate = DateTime.Now;
                            //TresourceData[i].CreateUserId = Convert.ToInt32(data.UserID);
                            //TresourceData[i].RowSyncDate = DateTime.Now;
                            //TresourceData[i].StorageServerId = StorageData.StorageServerId;
                            bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);

                            //if (InsertResult == true)
                            //{
                            //SuccessUploadCount++;
                            //Model.QIS.TResource TresourceTransfer = new Model.QIS.TResource();
                            //TresourceTransfer.ResourceType = TresourceData[i].ResourceType;
                            //TresourceTransfer.ResourceTagCode = TresourceData[i].ResourceTagCode;
                            //TresourceTransfer.ResourceTagSubCode = TresourceData[i].ResourceTagSubCode;
                            //TresourceTransfer.ResourceGroupSet = TresourceData[i].ResourceGroupSet;
                            //TresourceTransfer.ResourceGroupOrder = TresourceData[i].ResourceGroupOrder;
                            //TresourceTransfer.ResourceMineType = TresourceData[i].ResourceMineType;
                            //TresourceTransfer.ProjectId = TresourceData[i].ProjectId;
                            //TresourceTransfer.UnitId = TresourceData[i].UnitId;
                            ////TresourceTransfer.FilePath = _appSetting.PictureRootURL + TresourceData[i].FilePath;
                            //TresourceTransfer.FilePath = Environment.GetEnvironmentVariable("PictureRootURL") + TresourceData[i].FilePath;
                            //TresourceTransfer.Description = TresourceData[i].Description;
                            //TresourceTransfer.IsActive = TresourceData[i].IsActive;
                            //TresourceTransfer.StorageServerId = TresourceData[i].StorageServerId;
                            //TresourceTransfer.PhaseId = TresourceData[i].PhaseId;
                            //TresourceTransfer.HeaderId = TresourceData[i].HeaderId;
                            //TresourceTransfer.DetailId = TresourceData[i].DetailId;
                            //TresourceTransfer.UDetailId = TresourceData[i].UDetailId;
                            //TresourceTransfer.UDetail_RowClientId = TresourceData[i].UDetail_RowClientId;
                            //TresourceTransfer.Tag = TresourceData[i].Tag;
                            //TresourceTransfer.RowClientId = TresourceData[i].RowClientId;
                            //TresourceTransfer.RowState = TresourceData[i].RowState;
                            //TresourceTransfer.RowVersion = TresourceData[i].RowVersion;
                            //TresourceTransfer.RowSyncDate = TresourceData[i].RowSyncDate;
                            //TresourceTransfer.CreateDeviceId = TresourceData[i].CreateDeviceId;
                            //TresourceTransfer.CreateUserId = TresourceData[i].CreateUserId;
                            //TresourceTransfer.ModifiedDeviceId = TresourceData[i].ModifiedDeviceId;
                            //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                            //TresourceTransfer.CreatedDate = TresourceData[i].CreatedDate;
                            //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                            //TresourceTransfer.UFHeaderId = TresourceData[i].UFHeaderId;
                            //TresourceTransfer.UHeaderId = TresourceData[i].UHeaderId;
                            //TresourceTransfer.UFDetailId = TresourceData[i].UFDetailID;
                            //TresourceTransfer.FileLength = TresourceData[i].FileLength;
                            //TresourceTransfer.UPhaseId = TresourceData[i].UPhaseId;
                            //TresourceTransfer.UPhase_RowClientId = TresourceData[i].UPhase_RowClientId;
                            //TresourceTransfer.UHeader_RowClientId = TresourceData[i].UHeader_RowClientId;
                            //TresourceTransfer.UFPhase_RowClientId = TresourceData[i].UFPhase_RowClientId;
                            //TresourceTransfer.UFHeader_RowClientId = TresourceData[i].UFHeader_RowClientId;
                            //TresourceTransfer.UFDetail_RowClientId = TresourceData[i].UFDetail_RowClientId;
                            //TresourceTransfer.TagState = TresourceData[i].TagState;
                            //TresourceTransfer.ClientDataType = TresourceData[i].ClientDataType;

                            //bool InsertResultWeb = _ResourceRepo.InsertTResourceWeb(TresourceTransfer);
                            //}
                        }
                    }

                }
                else
                {
                    return new
                    {
                        success = false,
                        data = "[]",
                        message = string.Format(" Upload File Fail Error Binary is Null : 0 Uploaded")
                    };
                }

            }


            return new
            {
                success = true,
                data = "[]",
                message = string.Format(" Upload File Success : {0} Uploaded  ", SuccessUploadCount)
            };
        }

        [HttpPost]
        [Route("UploadSingleFiles")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSingleFiles([FromForm] ParamUploadImagess data)
        {

            int a = 0;
            //List<TResource> TresourceData = JsonConvert.DeserializeObject<List<TResource>>(data.Resource);
            //string StroageID = base._appSetting.StorageServerId.ToString();

            //com.apthai.QISAPINETCore.Model.QIS.MStorageServer StorageData = _ResourceRepo.GetStorageServerDetail(StroageID);

            //var UploadLoctaion = StorageData.StoragePhysicalPath;
            int SuccessUploadCount = 0;
            int count = 0;

                callResource callResourceDate = new callResource();
                if (data.Files != null)
                {
                    // -- New ---- for Docker
                    var yearPath = DateTime.Now.Year;
                    var MonthPath = DateTime.Now.Month;
                    var dirPath1 = $"{yearPath}/{MonthPath}";
                    int dataPath = 0;
                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "data", "uploads", dirPath1);

                    string FileBinary;


                long size = data.Files.Length;
                    string FileExtension = Path.GetExtension(data.Files.FileName);  // -------------- > Get File Extention
                    var fileName = data.Files.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());

                    var dirPath = $"{yearPath}\\M{dataPath}";

                    // --- New Docker ----
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    if (data.Files.Length > 0)
                    {
                        var filePath = Path.Combine(uploads, data.Files.FileName);
                        var message = "";
                        if (System.IO.File.Exists(filePath))
                        {
                            string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                            string extension = Path.GetExtension(filePath);
                            string path = Path.GetDirectoryName(filePath);
                            string newFullPath = filePath;
                            string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
                            newFullPath = Path.Combine(path, tempFileName + extension);
                            using (var fileStream = new FileStream(newFullPath, FileMode.Create))
                            {
                                message = data.Files.Length.ToString();
                                await data.Files.CopyToAsync(fileStream);
                                fileName = tempFileName + extension;

                            }
                        }
                        else
                        {
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                message = data.Files.Length.ToString();
                                await data.Files.CopyToAsync(fileStream);
                            }
                        }

                        // -- -End New -----
                        //if (System.IO.File.Exists(savePath.FullName))
                        if (System.IO.File.Exists(filePath))
                        {
                            string FileExtention = Path.GetExtension(filePath);
                            // ----- Old -----
                            //TresourceData[i].FilePath = "data\\uploads\\" + dirPath + "\\" + fileName;
                            // ----- New Docker -----
                            callResourceDate.FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                            callResourceDate.FileLength = size;
                            callResourceDate.CreateDate = DateTime.Now;
                            callResourceDate.RowState = "Original";
                            callResourceDate.ResourceType = data.ResourceType;
                            callResourceDate.ResourceTagCode = data.ResourceTagCode;
                            callResourceDate.ResourceGroupSet = null;
                            callResourceDate.ResourceGroupOrder = 0;
                            callResourceDate.TDefectDetailId = data.TDefectDetailId == "" ? 0 : Convert.ToInt32(data.TDefectDetailId);
                            callResourceDate.ProjectNo = data.ProjectCode;
                            callResourceDate.SerialNo = data.UnitNo;
                            //TresourceData[i].FilePath = "data/uploads/" + yearPath + "/" + MonthPath + "/" + fileName;
                            //TresourceData[i].FileLength = size;
                            //TresourceData[i].CreatedDate = DateTime.Now;
                            //TresourceData[i].CreateUserId = Convert.ToInt32(data.UserID);
                            //TresourceData[i].RowSyncDate = DateTime.Now;
                            //TresourceData[i].StorageServerId = StorageData.StorageServerId;
                            bool InsertResult = _syncRepository.InsertCallResource(callResourceDate);

                            //if (InsertResult == true)
                            //{
                            //SuccessUploadCount++;
                            //Model.QIS.TResource TresourceTransfer = new Model.QIS.TResource();
                            //TresourceTransfer.ResourceType = TresourceData[i].ResourceType;
                            //TresourceTransfer.ResourceTagCode = TresourceData[i].ResourceTagCode;
                            //TresourceTransfer.ResourceTagSubCode = TresourceData[i].ResourceTagSubCode;
                            //TresourceTransfer.ResourceGroupSet = TresourceData[i].ResourceGroupSet;
                            //TresourceTransfer.ResourceGroupOrder = TresourceData[i].ResourceGroupOrder;
                            //TresourceTransfer.ResourceMineType = TresourceData[i].ResourceMineType;
                            //TresourceTransfer.ProjectId = TresourceData[i].ProjectId;
                            //TresourceTransfer.UnitId = TresourceData[i].UnitId;
                            ////TresourceTransfer.FilePath = _appSetting.PictureRootURL + TresourceData[i].FilePath;
                            //TresourceTransfer.FilePath = Environment.GetEnvironmentVariable("PictureRootURL") + TresourceData[i].FilePath;
                            //TresourceTransfer.Description = TresourceData[i].Description;
                            //TresourceTransfer.IsActive = TresourceData[i].IsActive;
                            //TresourceTransfer.StorageServerId = TresourceData[i].StorageServerId;
                            //TresourceTransfer.PhaseId = TresourceData[i].PhaseId;
                            //TresourceTransfer.HeaderId = TresourceData[i].HeaderId;
                            //TresourceTransfer.DetailId = TresourceData[i].DetailId;
                            //TresourceTransfer.UDetailId = TresourceData[i].UDetailId;
                            //TresourceTransfer.UDetail_RowClientId = TresourceData[i].UDetail_RowClientId;
                            //TresourceTransfer.Tag = TresourceData[i].Tag;
                            //TresourceTransfer.RowClientId = TresourceData[i].RowClientId;
                            //TresourceTransfer.RowState = TresourceData[i].RowState;
                            //TresourceTransfer.RowVersion = TresourceData[i].RowVersion;
                            //TresourceTransfer.RowSyncDate = TresourceData[i].RowSyncDate;
                            //TresourceTransfer.CreateDeviceId = TresourceData[i].CreateDeviceId;
                            //TresourceTransfer.CreateUserId = TresourceData[i].CreateUserId;
                            //TresourceTransfer.ModifiedDeviceId = TresourceData[i].ModifiedDeviceId;
                            //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                            //TresourceTransfer.CreatedDate = TresourceData[i].CreatedDate;
                            //TresourceTransfer.ModifiedUserId = TresourceData[i].ModifiedUserId;
                            //TresourceTransfer.UFHeaderId = TresourceData[i].UFHeaderId;
                            //TresourceTransfer.UHeaderId = TresourceData[i].UHeaderId;
                            //TresourceTransfer.UFDetailId = TresourceData[i].UFDetailID;
                            //TresourceTransfer.FileLength = TresourceData[i].FileLength;
                            //TresourceTransfer.UPhaseId = TresourceData[i].UPhaseId;
                            //TresourceTransfer.UPhase_RowClientId = TresourceData[i].UPhase_RowClientId;
                            //TresourceTransfer.UHeader_RowClientId = TresourceData[i].UHeader_RowClientId;
                            //TresourceTransfer.UFPhase_RowClientId = TresourceData[i].UFPhase_RowClientId;
                            //TresourceTransfer.UFHeader_RowClientId = TresourceData[i].UFHeader_RowClientId;
                            //TresourceTransfer.UFDetail_RowClientId = TresourceData[i].UFDetail_RowClientId;
                            //TresourceTransfer.TagState = TresourceData[i].TagState;
                            //TresourceTransfer.ClientDataType = TresourceData[i].ClientDataType;

                            //bool InsertResultWeb = _ResourceRepo.InsertTResourceWeb(TresourceTransfer);
                            //}
                        }
                    }

                }
                else
                {
                    return Ok(new { count = 1 });
                }


            return Ok(new { count = 1, });
        }
        //[HttpPost]
        //[Route("uploadFacilities")]
        //[Consumes("multipart/form-data")]
        //public async Task<object> uploadFacilities([FromForm] ParamUploadPlanImage data)
        //{

        //    try
        //    {

        //        List<string> tempUploadPath = new List<string>();
        //        List<PlanImageSplitUploadResultFacilities> tempUploadFiles = new List<PlanImageSplitUploadResultFacilities>();
        //        string message = "";
        //        var _ImageUnitMappingData = new List<ImageMappingData>();
        //        if (!string.IsNullOrEmpty(data.ImageUnitMappingData))
        //            _ImageUnitMappingData = JsonConvert.DeserializeObject<List<ImageMappingData>>(data.ImageUnitMappingData);

        //        if (data.PlanImageFiles != null && data.PlanImageFiles.Count() > 0)
        //        {

        //            //var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "data", "uploads");


        //            foreach (var formFile in data.PlanImageFiles)
        //            {

        //                long size = data.PlanImageFiles.Sum(f => f.Length);

        //                var fileName = formFile.FileName;// string.Format("{0}{1}" , DateTime.Now.ToString("DDMMyy") , Path.GetExtension(formFile.FileName)); //Path.GetFileName(Path.GetTempFileName());
        //                var yearPath = DateTime.Now.Year;
        //                var MonthPath = DateTime.Now.Month;
        //                int dataPath = 0; int dataPath2 = 0;

        //                if (_ImageUnitMappingData != null && _ImageUnitMappingData.Count() > 0)
        //                {
        //                    dataPath = _ImageUnitMappingData.FirstOrDefault().ProjectID;
        //                    dataPath2 = _ImageUnitMappingData.FirstOrDefault().UnitID;
        //                }

        //                var dirPath = $"{yearPath}/{MonthPath}/P{dataPath}";
        //                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "data", "uploads/" + dirPath);
        //                var savePath = new FileInfo(Path.Combine(uploads, fileName));
        //                //if (!savePath.Directory.Exists)
        //                //    savePath.Directory.Create();
        //                if (!Directory.Exists(uploads))
        //                {
        //                    Directory.CreateDirectory(uploads);
        //                }

        //                if (formFile.Length > 0)
        //                {
        //                    var filePath = Path.Combine(uploads, formFile.FileName);
        //                    if (System.IO.File.Exists(filePath))
        //                    {
        //                        string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
        //                        string extension = Path.GetExtension(filePath);
        //                        string path = Path.GetDirectoryName(filePath);
        //                        string newFullPath = filePath;
        //                        string tempFileName = string.Format("{0}({1})", fileNameOnly, Guid.NewGuid().ToString());
        //                        newFullPath = Path.Combine(path, tempFileName + extension);
        //                        using (var fileStream = new FileStream(newFullPath, FileMode.Create))
        //                        {
        //                            message = formFile.Length.ToString();
        //                            await formFile.CopyToAsync(fileStream);
        //                            fileName = tempFileName;

        //                        }
        //                    }
        //                    else
        //                    {
        //                        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //                        {
        //                            message = formFile.Length.ToString();
        //                            await formFile.CopyToAsync(fileStream);

        //                        }
        //                    }

        //                    // old
        //                    //using (var stream = new FileStream(savePath.FullName, FileMode.Create))
        //                    //{
        //                    //    await formFile.CopyToAsync(stream);
        //                    //}


        //                    if (System.IO.File.Exists(filePath))
        //                    {

        //                        filePath = filePath.Replace("\\", "/");
        //                        var saveStoragePath = await base.GetQISResourceStoragePhysicalPathAsync(base._appSetting.StorageServerId, filePath);


        //                        var ImageId = data.RowClientID.ToLower();
        //                        if (string.IsNullOrEmpty(ImageId))
        //                            ImageId = Guid.NewGuid().ToString("N");

        //                        var tempUploadFile = savePath.FullName;

        //                        //var newDir = new DirectoryInfo(filePath);
        //                        //if (!newDir.Exists)
        //                        //    newDir.Create();

        //                        string mainResizePath = System.IO.Path.Combine(savePath.DirectoryName, string.Format("PLM{0}.jpg", ImageId));
        //                        mainResizePath = mainResizePath.Replace("\\", "/");
        //                        // Resize/Crop MainImage
        //                        var mainImage = ProcessResizeMainImage(tempUploadFile, mainResizePath);
        //                        // Split MainImage to Multiple Image
        //                        var childImages = ProcesSplitImages(string.Format("{0}", ImageId), mainResizePath, savePath.DirectoryName.Replace("\\", "/"));

        //                        /*--- New Code ---*/


        //                        long ResFileLength = 0;

        //                        var tempUploadad = new FileInfo(mainResizePath);
        //                        ResFileLength = tempUploadad.Length;

        //                        string ResId = Guid.NewGuid().ToString();


        //                        //For Save Main Image
        //                        var valid = await func_SaveImageFaculitiesAsync(ResId.ToString(), dataPath, dataPath2, mainResizePath, ImageId, true, ResFileLength, data.Description, data.CreateDeviceId, data.UserID, data.CreateDeviceDate, _ImageUnitMappingData, data.CreatorFullName, data.UserID);

        //                        if (valid)
        //                        {
        //                            //For Save Sub Image
        //                            var ResourceIds = new List<string>();
        //                            foreach (var a in childImages)
        //                            {
        //                                var d = new FileInfo(a.Value);
        //                                ResFileLength = d.Length;
        //                                var subResizePath = d.FullName;

        //                                ResId = ResId = Guid.NewGuid().ToString();
        //                                valid = await func_SaveImageFaculitiesAsync(ResId.ToString(), dataPath, dataPath2, subResizePath, ImageId, false, ResFileLength, data.Description, data.CreateDeviceId, data.UserID, data.CreateDeviceDate, _ImageUnitMappingData, data.CreatorFullName, data.UserID);
        //                                ResourceIds.Add(ResId);
        //                            }

        //                            ////For Mapping Main/Sub Image
        //                            await _unitOfWork.MasterRepository.SaveImageManagementSplit(ImageId, ResourceIds);


        //                        }


        //                        /*--- End ---*/


        //                        // clear upload temp
        //                        try
        //                        {
        //                            System.IO.File.Delete(tempUploadFile);
        //                        }
        //                        catch (Exception ex) { }


        //                        tempUploadFiles.Add(new PlanImageSplitUploadResultFacilities()
        //                        {
        //                            MainImageClientID = data.RowClientID

        //                        });

        //                    }
        //                }
        //            }
        //            for (int i = 0; i < tempUploadFiles.Count(); i++)
        //            {
        //                tempUploadFiles[i].CreateDateText = tempUploadFiles[i].CreateDate.ToString("dd-MM-yyyy HH:mm:ss", new CultureInfo("en-US"));
        //            }

        //            return new
        //            {
        //                success = true,
        //                data = new { MappingPlanImages = tempUploadFiles },
        //                // data = _data.ToList(),
        //                //message = string.Format("found {0} items", _data.Count)
        //            };

        //        }


        //        //var dataResult = await _unitOfWork.MasterRepository.GetUnitSearch(data.ProjectId, data.SearchText);
        //        //var _data = dataResult.ToList();


        //        return new
        //        {
        //            success = false,
        //            message = "missing upload file."
        //        };



        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, "Internal server error :: " + ex.Message);
        //    }

        //}



        //private async Task<bool> func_SaveImageAsync(string ResourceClientId, int dataPath, int dataPath2, string imagePath, string ImageClientID, bool IsMainImage, long fileLength, string Description, string createDeviceId, int? createUserId, DateTime? createDeviceDate, List<ImageMappingData> ImageUnitMappingData,string CreatorFullName,int UserID = 0)
        //{
        //    try
        //    {
        //        var storageServerId = _appSetting.StorageServerId;
        //        var resultMsg = "Successful";
        //        //var valid = true;

        //        /*--- Step 1 : Add Image Resource ---*/
        //        // var itemImage = await _unitOfWork.MasterRepository.GetImageManagementDetail(ImageId);
        //        var itemImageRes = new MImageResource();
        //        itemImageRes.ResourceClientId = ResourceClientId;
        //        itemImageRes.CreatedDate = DateTime.Now;
        //        //itemImageRes.cre = createDeviceId;
        //        itemImageRes.CreateUserId = createUserId;
        //        itemImageRes.Description = Description;

        //        itemImageRes.IsActive = true;
        //        itemImageRes.StorageServerId = storageServerId;
        //        //itemImageRes.pro = _ImageUnitMappingData.FirstOrDefault().ProjectID;
        //        itemImageRes.FileLength = fileLength;

        //        if (IsMainImage)
        //        {
        //            itemImageRes.ResourceTagCode = "FLMain";
        //        }
        //        else
        //        {
        //            itemImageRes.ResourceTagCode = "FLSplit";
        //        }
        //        var fileName = Path.GetFileName(imagePath);
        //        var baseStoragePath = await base.GetQISResourceStoragePhysicalPathAsync(storageServerId);
        //        var relativeFileName = imagePath.Replace(baseStoragePath, string.Empty);
        //        var relativeFileNameFixed = relativeFileName.Replace("\\", "/").TrimStart('/');
        //        itemImageRes.FileName = fileName;
        //        itemImageRes.FilePath = relativeFileNameFixed.Substring(12);
        //        itemImageRes.FileLength = fileLength;
        //        itemImageRes.MineType = GetMimeType(imagePath);
        //        itemImageRes.IsActive = true;
        //        itemImageRes.StorageServerId = storageServerId;
        //        itemImageRes.CreateUserId = UserID;
        //        itemImageRes.CreatedDate = DateTime.Now;
        //        itemImageRes.ModifiedUserId = UserID;
        //        itemImageRes.ModifiedDate = DateTime.Now;

        //        List<MImageMapping> _mapping = new List<MImageMapping>();
        //        foreach (var m in ImageUnitMappingData)
        //        {
        //            _mapping.Add(new MImageMapping() { ImageMappingID = Guid.NewGuid(), ImageClientID = ImageClientID, ProjectID = m.ProjectID, UnitID = m.UnitID });
        //        }

        //        Tuple<bool, string> valid = await _unitOfWork.MasterRepository.SaveImageResource(ImageClientID, itemImageRes, _mapping, dataPath, dataPath2, Description, createDeviceId, createDeviceDate, CreatorFullName, IsMainImage);

        //        return true;


        //    }
        //    catch (Exception ex)
        //    {


        //        return false;

        //    }

        //}
        //private async Task<bool> func_SaveImageFaculitiesAsync(string ResourceClientId, int dataPath, int dataPath2, string imagePath, string ImageClientID, bool IsMainImage, long fileLength, string Description, string createDeviceId, int? createUserId, DateTime? createDeviceDate, List<ImageMappingData> ImageUnitMappingData, string CreatorFullName, int UserID = 0)
        //{



        //    try
        //    {

        //        var storageServerId = _appSetting.StorageServerId;
        //        var resultMsg = "Successful";
        //        //var valid = true;

        //        /*--- Step 1 : Add Image Resource ---*/
        //        // var itemImage = await _unitOfWork.MasterRepository.GetImageManagementDetail(ImageId);
        //        var itemImageRes = new MImageResource();
        //        itemImageRes.ResourceClientId = ResourceClientId;
        //        itemImageRes.CreatedDate = DateTime.Now;
        //        //itemImageRes.cre = createDeviceId;
        //        itemImageRes.CreateUserId = createUserId;
        //        itemImageRes.Description = Description;

        //        itemImageRes.IsActive = true;
        //        itemImageRes.StorageServerId = storageServerId;
        //        //itemImageRes.pro = _ImageUnitMappingData.FirstOrDefault().ProjectID;
        //        itemImageRes.FileLength = fileLength;

        //        if (IsMainImage)
        //        {
        //            itemImageRes.ResourceTagCode = "FLMain";
        //        }
        //        else
        //        {
        //            itemImageRes.ResourceTagCode = "FLSplit";
        //        }
        //        var fileName = Path.GetFileName(imagePath);
        //        var baseStoragePath = await base.GetQISResourceStoragePhysicalPathAsync(storageServerId);
        //        var relativeFileName = imagePath.Replace(baseStoragePath, string.Empty);
        //        var relativeFileNameFixed = relativeFileName.Replace("\\", "/").TrimStart('/');
        //        itemImageRes.FileName = fileName;
        //        itemImageRes.FilePath = relativeFileNameFixed.Substring(12);
        //        itemImageRes.FileLength = fileLength;
        //        itemImageRes.MineType = GetMimeType(imagePath);
        //        itemImageRes.IsActive = true;
        //        itemImageRes.StorageServerId = storageServerId;
        //        itemImageRes.CreateUserId = UserID;
        //        itemImageRes.CreatedDate = DateTime.Now;
        //        itemImageRes.ModifiedUserId = UserID;
        //        itemImageRes.ModifiedDate = DateTime.Now;

        //        List<MImageMapping> _mapping = new List<MImageMapping>();
        //        foreach (var m in ImageUnitMappingData)
        //        {
        //            _mapping.Add(new MImageMapping() { ImageMappingID = Guid.NewGuid(), ImageClientID = ImageClientID, ProjectID = m.ProjectID, UnitID = m.UnitID, MaterialCode = m.MaterialCode });
        //        }

        //        Tuple<bool, string> valid = await _unitOfWork.MasterRepository.SaveImageResource(ImageClientID, itemImageRes, _mapping, dataPath, dataPath2, Description, createDeviceId, createDeviceDate, CreatorFullName, IsMainImage);

        //        return true;


        //    }
        //    catch (Exception ex)
        //    {


        //        return false;

        //    }

        //}

        //private async Task<bool> func_SaveImageFacilitiesAsync(string ResourceClientId, int dataPath, int dataPath2, string imagePath, string ImageClientID, bool IsMainImage, long fileLength, string Description, string createDeviceId, int? createUserId, DateTime? createDeviceDate, List<ImageMappingData> ImageUnitMappingData, string CreatorFullName, int UserID = 0)
        //{



        //    try
        //    {

        //        var storageServerId = _appSetting.StorageServerId;
        //        var resultMsg = "Successful";
        //        //var valid = true;

        //        /*--- Step 1 : Add Image Resource ---*/
        //        // var itemImage = await _unitOfWork.MasterRepository.GetImageManagementDetail(ImageId);
        //        var itemImageRes = new MImageResource();
        //        itemImageRes.ResourceClientId = ResourceClientId;
        //        itemImageRes.CreatedDate = DateTime.Now;
        //        //itemImageRes.cre = createDeviceId;
        //        itemImageRes.CreateUserId = createUserId;
        //        itemImageRes.Description = Description;

        //        itemImageRes.IsActive = true;
        //        itemImageRes.StorageServerId = storageServerId;
        //        //itemImageRes.pro = _ImageUnitMappingData.FirstOrDefault().ProjectID;
        //        itemImageRes.FileLength = fileLength;

        //        if (IsMainImage)
        //        {
        //            itemImageRes.ResourceTagCode = "FLMain";
        //        }
        //        else
        //        {
        //            itemImageRes.ResourceTagCode = "FLSplit";
        //        }
        //        var fileName = Path.GetFileName(imagePath);
        //        var baseStoragePath = await base.GetQISResourceStoragePhysicalPathAsync(storageServerId);
        //        var relativeFileName = imagePath.Replace(baseStoragePath, string.Empty);
        //        var relativeFileNameFixed = relativeFileName.Replace("\\", "/").TrimStart('/');
        //        itemImageRes.FileName = fileName;
        //        itemImageRes.FilePath = relativeFileNameFixed.Substring(12);
        //        itemImageRes.FileLength = fileLength;
        //        itemImageRes.MineType = GetMimeType(imagePath);
        //        itemImageRes.IsActive = true;
        //        itemImageRes.StorageServerId = storageServerId;
        //        itemImageRes.CreateUserId = UserID;
        //        itemImageRes.CreatedDate = DateTime.Now;
        //        itemImageRes.ModifiedUserId = UserID;
        //        itemImageRes.ModifiedDate = DateTime.Now;


        //        List<MImageMapping> _mapping = new List<MImageMapping>();
        //        foreach (var m in ImageUnitMappingData)
        //        {
        //            _mapping.Add(new MImageMapping() { ImageMappingID = Guid.NewGuid(), ImageClientID = ImageClientID, ProjectID = m.ProjectID, UnitID = m.UnitID });
        //        }

        //        Tuple<bool, string> valid = await _unitOfWork.MasterRepository.SaveImageResource(ImageClientID, itemImageRes, _mapping, dataPath, dataPath2, Description, createDeviceId, createDeviceDate, CreatorFullName, IsMainImage);

        //        return true;


        //    }
        //    catch (Exception ex)
        //    {


        //        return false;

        //    }

        //}



        private FileInfo ProcessResizeMainImage(string srcImagePath, string descImagePath)
        {


            int width = 2000;
            int height = 2000;

            using (Image<Rgba32> img = Image.Load(srcImagePath))
            {
                var newImg = img.Clone();
                if (newImg.Height > height || newImg.Width > width)
                {
                    newImg = img.Clone(x => x.Resize(new ResizeOptions()
                    {
                        Mode = ResizeMode.Max,
                        Position = AnchorPositionMode.Center,
                        Size = new SixLabors.Primitives.Size(width, height)
                    }));
                }
                newImg.Save(descImagePath, new JpegEncoder() { Quality = 100 });


                //if (img.Height == img.Width) // vertical
                //{
                //    //Crops to a square (in place)
                //    var newImg = img.Clone(x => x.Resize(new ResizeOptions() { Mode = ResizeMode.Max , Position = AnchorPositionMode.Center , Size = new SixLabors.Primitives.Size(2000,2000)  }));
                //    newImg.Save(descImagePath, new JpegEncoder() { Quality = 90 });
                //    //ImageBuilder.Current.Build(srcImagePath, descImagePath, new ResizeSettings("width=800&height=800&format=jpg&crop=auto"));

                //}
                //else if (img.Height > img.Width) // vertical
                //{
                //    //Crops to a square (in place)
                //    ImageBuilder.Current.Build(srcImagePath, descImagePath, new ResizeSettings("width=600&height=800&format=jpg&crop=auto"));
                //}
                //else
                //{
                //    //Crops to a square (in place)
                //    ImageBuilder.Current.Build(srcImagePath, descImagePath, new ResizeSettings("width=800&height=600&format=jpg&crop=auto"));
                //}

            }


            return new FileInfo(descImagePath);

        }

        private List<KeyValuePair<int, string>> ProcesSplitImages(string ImageId, string srcOriginalImage, string destinationFolderName)
        {


            int cols = 5;
            int rows = 5;
            //int width = 600;
            //int height = 600;

            var sfileSplit = new List<KeyValuePair<int, string>>();
            using (Image<Rgba32> img = Image.Load(srcOriginalImage))
            {


                // var imgDummy = img.Clone();
                //if (img.Width > 600 || img.Height > 600)
                //{
                //    imgDummy = img.Clone(x =>
                //     x.Resize(new ResizeOptions
                //     {
                //         Mode = ResizeMode.Max,
                //         Position = AnchorPositionMode.Center,
                //         Size = new SixLabors.Primitives.Size(width, height),
                //         Compand = false
                //     }));
                //}

                int _img_width = img.Width;
                int _img_height = img.Height;
                string filename = System.IO.Path.GetFileNameWithoutExtension(srcOriginalImage);
                var indexFile = 0;
                for (int x = 0; x < cols; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        indexFile += 1;
                        var cropArea = new SixLabors.Primitives.Rectangle(x * (_img_width / cols), y * (_img_height / rows), _img_width / cols, _img_height / rows);
                        var newImg = img.Clone(e => e.Crop(cropArea));

                        var x_fileName = string.Format("PLS{0}_x{1}_{2}.jpg", ImageId, x, y);
                        string pathSave = @"" + destinationFolderName.TrimEnd('/') + "/" + x_fileName;

                        newImg.Save(pathSave, new JpegEncoder { Quality = 100 });
                        sfileSplit.Add(new KeyValuePair<int, string>(indexFile, pathSave));

                    }
                }

            }

            return sfileSplit;

        }

        //[HttpPost]
        //[Route("lists")]
        //[Consumes("application/json")]
        //public async Task<object> GetImageSplitlist([FromBody] ParamMappingPlanImageCheck data)
        //{

        //    try
        //    {

        //        if (data != null)
        //        {
        //            if (data.UnitIDs != null && data.UnitIDs.Length > 0)
        //            {
        //                List<PlanImageUploadResult> tempUploadFiles = new List<PlanImageUploadResult>();
        //                var result = new List<Model.QISSync.vwPlanImage>();
        //                result = (await _unitOfWork.SyncRepository.GetPlanImages(data.UnitIDs)).ToList();

        //                var dat1a = result.ToList();

        //                var group = dat1a.Select(d => new { d.ImageClientID, d.FileUrl, d.ResourceClientId, d.Description }).Distinct().ToList();


        //                foreach (var d in group)
        //                {
        //                    var MImage = dat1a.Where(e => e.ImageClientID == d.ImageClientID).FirstOrDefault();

        //                    //  public List<KeyValuePair<string, string>> MappingUnits { get; set; }
        //                    var assets = dat1a.Where(e => e.ImageClientID == d.ImageClientID).Select(s => new PlanImageAsset()
        //                    {
        //                        ProjectID = s.ProjectID,
        //                        UnitID = s.UnitID
        //                    })
        //                        .ToList().Distinct().ToList();
        //                    var kk = new PlanImageUploadResult()
        //                    {
        //                        MainImageClientID = d.ImageClientID,
        //                        MainImageUrl = d.FileUrl,
        //                        MainResourceClientId = d.ResourceClientId,
        //                        Description = d.Description,
        //                        MappingData = assets,
        //                        CreatorFullName = MImage.CreatorFullName,
        //                        CreateDate = DateTime.Now,
        //                        CreateDateText = MImage.CreatedDate?.ToString("dd-MM-yyyy HH:mm:ss", new CultureInfo("en-US"))
        //                    };
        //                    //kk.Mappings = dat1a.Where(e => e.ImageClientID == kk.MainImageID).Select(e => new ImageMappingData()
        //                    //{
        //                    //    UnitID = e.UnitID ?? 0,
        //                    //    ProjectID = e.ProjectID ?? 0 , Description = 
        //                    //    e.Description

        //                    //}).Distinct().ToList();

        //                    tempUploadFiles.Add(kk);
        //                }


        //                return new
        //                {
        //                    success = true,
        //                    data = new { MappingPlanImages = tempUploadFiles }//,
        //                                                                      // data = _data.ToList(),
        //                                                                      //message = string.Format("found {0} items", _data.Count)
        //                };
        //            }
        //            else
        //            {
        //                List<PlanImageSplitUploadResultFacilities> tempUploadFiles = new List<PlanImageSplitUploadResultFacilities>();
        //                var result = new List<Model.QISSync.vwPlanImagesFacility>();

        //                    result = (await _unitOfWork.SyncRepository.GetPlanImagesFacilitiesByProjectID(data.ProjectID)).ToList();


        //                var dat1a = result.ToList();

        //                var group = dat1a.Select(d => new { d.ImageClientID, d.FileUrl, d.ResourceClientId, d.Description }).Distinct().ToList();


        //                foreach (var d in group)
        //                {
        //                    var MImage = dat1a.Where(e => e.ImageClientID == d.ImageClientID).FirstOrDefault();

        //                    //  public List<KeyValuePair<string, string>> MappingUnits { get; set; }
        //                    var assets = dat1a.Where(e => e.ImageClientID == d.ImageClientID).Select(s => new PlanImageAssetFacilities()
        //                    {
        //                        ProjectID = s.ProjectID,
        //                        MaterialCode = s.MaterialCode
        //                    })
        //                        .ToList().Distinct().ToList();
        //                    var kk = new PlanImageSplitUploadResultFacilities()
        //                    {
        //                        MainImageClientID = d.ImageClientID,
        //                        MainImageUrl = d.FileUrl,
        //                        MainResourceClientId = d.ResourceClientId,
        //                        Description = d.Description,
        //                        MappingData = assets,
        //                        CreatorFullName = MImage.CreatorFullName,
        //                        CreateDate = DateTime.Now,
        //                        CreateDateText = MImage.CreatedDate?.ToString("dd-MM-yyyy HH:mm:ss", new CultureInfo("en-US"))
        //                    };
        //                    //kk.Mappings = dat1a.Where(e => e.ImageClientID == kk.MainImageID).Select(e => new ImageMappingData()
        //                    //{
        //                    //    UnitID = e.UnitID ?? 0,
        //                    //    ProjectID = e.ProjectID ?? 0 , Description = 
        //                    //    e.Description

        //                    //}).Distinct().ToList();

        //                    tempUploadFiles.Add(kk);
        //                }


        //                return new
        //                {
        //                    success = true,
        //                    data = new { MappingPlanImages = tempUploadFiles }//,
        //                                                                      // data = _data.ToList(),
        //                                                                      //message = string.Format("found {0} items", _data.Count)
        //                };

        //            }


        //            //return new
        //            //{
        //            //    success = true,
        //            //    data = dat1a,
        //            //    message = string.Format("found {0} items", dat1a.Count())
        //            //};

        //        }

        //        return new
        //        {
        //            success = false,
        //            message = "missing parameter."
        //        };

        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        //[HttpPost]
        //[Route("delete")]
        //[Consumes("application/json")]
        //public async Task<object> SetDelete([FromBody] ParamMappingPlanImageDelete data)
        //{

        //    try
        //    {

        //        List<PlanImageUploadResult> tempUploadFiles = new List<PlanImageUploadResult>();
        //        if (data != null)
        //        {


        //            var main = await _unitOfWork.SyncRepository.GetPlanImagesByImageClient(data.RowClientID);
        //            var split = await _unitOfWork.SyncRepository.GetImageSplitListByClient(data.RowClientID);
        //            if (main.Count() > 0)
        //            {
        //                UDetail uDetails = _TransactionRepo.GetUDetailByMainImageID(data.RowClientID);
        //                if (uDetails != null)
        //                {
        //                    return new
        //                    {
        //                        success = false,
        //                        data = uDetails,
        //                        message = string.Format(" Call : Cannot Delete This PlanPointImage Becuase it is in use!  ")
        //                    };
        //                }
        //                var result = await _unitOfWork.SyncRepository.DeletePlanImagesResource(data.RowClientID);
        //                if (result > 0)
        //                {
                           
        //                    foreach (var m in main)
        //                    {

        //                        try
        //                        {
        //                            if (System.IO.File.Exists(m.FilePhyPath))
        //                            {
        //                                System.IO.File.Delete(m.FilePhyPath);
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {

        //                        }
        //                    }

        //                    foreach (var f in split)
        //                    {

        //                        try
        //                        {
        //                            if (System.IO.File.Exists(f.FilePhyPath))
        //                            {
        //                                System.IO.File.Delete(f.FilePhyPath);
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {

        //                        }
        //                    }
        //                }
        //            }

        //            return new
        //            {
        //                success = true,
        //                data = new { MappingPlanImages = tempUploadFiles }//,
        //                // data = _data.ToList(),
        //                //message = string.Format("found {0} items", _data.Count)
        //            };
        //        }

        //        return new
        //        {
        //            success = false,
        //            message = "missing parameter."
        //        };



        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, "Internal server error");
        //    }

        //}

    }
}
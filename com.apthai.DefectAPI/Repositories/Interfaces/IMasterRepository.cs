using com.apthai.DefectAPI.CustomModel;
using com.apthai.DefectAPI.Model;
using com.apthai.DefectAPI.Model.DefectAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.DefectAPI.Repositories.Interfaces
{
    public interface IMasterRepository
    {
        List<calltype> GetCallCallType_Sync(string CalltypeID);
        List<GetcallTypeWithArea> GetCallCallTypeWithArea_Sync();
        List<PointURL> GetCallPointByProductCat_Sync(string ProductTypeCate, string Cate);
        List<PointURL> GetFloorDistinct(string CateType);
        List<callResource> GetSignatureCallResourceByTdefectID(int TDefectId);
        List<Callarea> GetCallAreaByProductCat_Sync(string ProductTypeCate);
        callTFloorPlanImage GetUnitFloorPlanByUnitAndFloor(string UnitNumber, int Floor , string ProjectNo);
        List<callResource> GetCallResourceByTdefectDetailID(int TDefectDetailId);
        List<callResource> GetCallResourceByTdefect(int TDefectId);
        List<callResource> GetCallResourceBeforeByTdefectDetailID(int TDefectDetailId);
        List<callResource> GetCallResourceAfterByTdefectDetailID(int TDefectDetailId);
        ICONEntFormsProduct GetProductDataFromCRM_Sync(string ProductID);
        List<GetUnitByProjectReturnObj> GetUnitByProduct(string ProductID, string SearchText);
        ViewUnitCustomer GetViewUnitCustomer(string UnitNumber, string ProductID);
        List<calldescription> GetCallDescriptionByCallAreaID_Sync(int CallAreaID);
        callTDefect GetCallTDefect_Sync(int TDefectID);
        CallTdefectCheckCustomer GetCallTDefectByUnitNumber_Sync(string UnitNumber);
        CallTdefectCheckCustomer GetCallTDefectByUnitNumberAndProject_Sync(string UnitNumber, int ProductId);
        CallTdefectMObj GetCallTDefectByUnitID_Sync(string ProjectCode, string UnitID);
        CallTdefectVendorMObj GetCallTDefectVendorByUnitID_Sync(string ProjectCode, string UnitID);
        List<callTDefectDetail> GetcallTDefectDetail_Sync(int TDefectID);
        callTDefectDetail GetcallTDefectDetailByDetailID_Sync(int TDefectDetailID);
        List<callTDefectDetail> GetcallTDefectDetailByDetailIDList_Sync(string TDefectDetailIDList);
        List<callTDefectDetail> GetcallTDefectDetailByTDefectIDList_Sync(int TDefectID);
        List<CallTdefectDetailCustom> GetcallTDefectDetailShow_Sync(int TDefectID);
        List<GetUnitByProjectReturnObj> GetRecentcallTDefect_Sync(string EmpCode);
        List<GetCallTDefectByProjectObj> GetCallTDefectByProject_Sync(int ProductID);
        List<Floor> GetFloorsByProjectTower(string ProjectID, string TowerID);
        Model.DefectAPISync.vwProject GetVwProjects(string ProjectID);
        List<Tower> GetTowersByProject(string ProjectID);
        Model.DefectAPI.point GetCallPointByPointID_Sync(int? comppoint_id);
    }
}

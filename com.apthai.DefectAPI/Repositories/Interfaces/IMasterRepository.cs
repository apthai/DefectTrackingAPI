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
        List<calltype> GetCallCallType_Sync();
        List<GetcallTypeWithArea> GetCallCallTypeWithArea_Sync();
        List<point> GetCallPointByProductCat_Sync(string ProductTypeCate);
        List<callarea> GetCallAreaByProductCat_Sync(string ProductTypeCate);
        ICONEntFormsProduct GetProductDataFromCRM_Sync(string ProductID);
        List<GetUnitByProjectReturnObj> GetUnitByProduct(string ProductID, string SearchText);
        List<calldescription> GetCallDescriptionByCallAreaID_Sync(int CallAreaID);
        callTDefect GetCallTDefect_Sync(int TDefectID);
        callTDefect GetCallTDefectByUnitID_Sync(string ProjectCode, string UnitID);
        List<callTDefectDetail> GetcallTDefectDetail_Sync(int TDefectID);
        List<GetUnitByProjectReturnObj> GetRecentcallTDefect_Sync(string EmpCode);
        List<GetCallTDefectByProjectObj> GetCallTDefectByProject_Sync(int ProductID);
        List<Floor> GetFloorsByProjectTower(string ProjectID, string TowerID);
        Model.DefectAPISync.vwProject GetVwProjects(string ProjectID);
        List<Tower> GetTowersByProject(string ProjectID);
    }
}

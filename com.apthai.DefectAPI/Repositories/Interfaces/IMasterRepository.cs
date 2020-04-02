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
        List<point> GetCallPointByProductCat_Sync(string ProductTypeCate);
        List<callarea> GetCallAreaByProductCat_Sync(string ProductTypeCate);
        List<ICONEntFormsUnit> GetUnitByProduct(string ProductID);
        List<calldescription> GetCallDescriptionByCallAreaID_Sync(int CallAreaID);
        callTDefect GetCallTDefect_Sync(int TDefectID);
        List<callTDefectDetail> GetcallTDefectDetail_Sync(int TDefectID);
        List<GetCallTDefectByProjectObj> GetCallTDefectByProject_Sync(int ProductID);
    }
}

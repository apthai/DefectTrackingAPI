using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using com.apthai.DefectAPI.Model.DefectAPI;

namespace com.apthai.DefectAPI.Repositories
{
    public interface ITransactionRepository
    {
        AccessKeyControl CheckUserAccessKey(string EmpCode, string AccessKey);
        bool InsertUserAccessKey(AccessKeyControl AC);
        bool UpdateUserAccessKey(AccessKeyControl AC);
        long InsertTdefectDetail(callTDefectDetail defectDetail);
        bool UpdateTdefectDetail(callTDefectDetail defectDetail);
        bool UpdateTdefectDetailList(List<callTDefectDetail> defectDetailList);
        bool UpdateTdefect(callTDefect callTDefect);
        bool UpdateCallResource(List<callResource> callResources);
        bool InsertTdefectDetail(callTDefect defectDetail, ref long DefectID);
        bool InsertTdefectDetailList(List<callTDefectDetail> defectDetails);
        bool InsertTdefect(callTDefect callTDefect);
       //Task<List<vwUser>> GetAllUser();
       // Model.QISAuth.vwUser GetUserData(int UserID);

    }
}
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using com.apthai.DefectAPI.Model.DefectAPI;

namespace com.apthai.DefectAPI.Repositories
{
    public interface IUserRepository
    {

        // Task<vwUser> GetUser(string userId);
        AccessKeyControl GetUserAccessKey(string EmpCode);
        bool InsertUserAccessKey(AccessKeyControl AC);
        bool UpdateUserAccessKey(AccessKeyControl AC);
        AccessKeyControl CheckUserAccessKey(string EmpCode, string AccessKey);
       //Task<List<vwUser>> GetAllUser();
       // Model.QISAuth.vwUser GetUserData(int UserID);

    }
}
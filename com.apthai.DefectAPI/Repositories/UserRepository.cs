using Dapper;
using com.apthai.DefectAPI.Model.DefectAPI;
using com.apthai.DefectAPI.Repositories.Interfaces;
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

namespace com.apthai.DefectAPI.Repositories
{
    public class UserRepository : BaseRepository , IUserRepository
    {

        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hostingEnvironment;

        public UserRepository(IHostingEnvironment environment, IConfiguration config) : base(environment, config)
        {
            _config = config;
            _hostingEnvironment = environment;

        }

        public AccessKeyControl GetUserAccessKey(string EmpCode)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<AccessKeyControl>("select * from AccessKeyControl WITH(NOLOCK) where EmpCode=@EmpCode", new { EmpCode = EmpCode }).FirstOrDefault();

                return result;
            }
        }
        public AccessKeyControl CheckUserAccessKey(string EmpCode , string AccessKey)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<AccessKeyControl>("select * from AccessKeyControl WITH(NOLOCK) where EmpCode=@EmpCode and AccessKey=@AccessKey", new { EmpCode = EmpCode, AccessKey=AccessKey }).FirstOrDefault();

                return result;
            }
        }
        public bool InsertUserAccessKey(AccessKeyControl AC)
        {
            try
            {
                using (IDbConnection conn = WebConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Insert(AC, tran);
                    tran.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateUserAccessKey(AccessKeyControl AC)
        {
            try
            {
                using (IDbConnection conn = WebConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Update(AC, tran);
                    tran.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public async Task<vwUser> GetUser(string userId)
        //{
        //    using (IDbConnection conn = WebConnection)
        //    {
        //        conn.Open();
        //        var result = await conn.QueryAsync<vwUser>("select * from vw_User WITH(NOLOCK) where UserID=@ID", new { ID = userId });
        //        if (result.Any())
        //        {
        //            return result.FirstOrDefault();
        //        }
        //        else
        //        {
        //            return new vwUser();
        //        }
        //    }
        //}
        //public Model.QISAuth.vwUser GetUserData(int UserID)
        //{
        //    using (IDbConnection conn = AuthConnection)
        //    {
        //        conn.Open();
        //        var result = conn.Query<Model.QISAuth.vwUser>("select * from vw_User WITH(NOLOCK) where UserID=@UserID", new { UserID = UserID });
        //        if (result.Any())
        //        {
        //            return result.FirstOrDefault();
        //        }
        //        else
        //        {
        //            return new Model.QISAuth.vwUser();
        //        }
        //    }
        //}
        //public async Task<List<vwUser>> GetAllUser()
        //{
        //    using (IDbConnection conn = WebConnection)
        //    {
        //        conn.Open();
        //        var result = await conn.QueryAsync<vwUser>("select * from vw_User  WITH(NOLOCK)");
        //        return result.ToList() ?? new List<vwUser>();
        //    }
        //}

    }

}

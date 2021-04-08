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
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {

        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hostingEnvironment;

        public TransactionRepository(IHostingEnvironment environment, IConfiguration config) : base(environment, config)
        {
            _config = config;
            _hostingEnvironment = environment;

        }

        //public callTDefect GetDefectTransactionBy(string EmpCode)
        //{
        //    using (IDbConnection conn = WebConnection)
        //    {
        //        conn.Open();
        //        var result = conn.Query<AccessKeyControl>("select * from AccessKeyControl WITH(NOLOCK) where EmpCode=@EmpCode", new { EmpCode = EmpCode }).FirstOrDefault();

        //        return result;
        //    }
        //}
        public AccessKeyControl CheckUserAccessKey(string EmpCode, string AccessKey)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<AccessKeyControl>("select * from AccessKeyControl WITH(NOLOCK) where EmpCode=@EmpCode and AccessKey=@AccessKey", new { EmpCode = EmpCode, AccessKey = AccessKey }).FirstOrDefault();

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
        public bool InsertTdefectDetail(callTDefect defectDetail, ref long DefectID)
        {
            try
            {
                using (IDbConnection conn = WebConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    DefectID = conn.Insert(defectDetail, tran);
                    tran.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long InsertTdefectDetail(callTDefectDetail defectDetail)
        {
            try
            {
                using (IDbConnection conn = WebConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    long result = conn.Insert(defectDetail, tran);
                    tran.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateTdefectDetail(callTDefectDetail defectDetail)
        {
            try
            {
                using (IDbConnection conn = WebConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Update(defectDetail, tran);
                    tran.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateTdefectDetailList(List<callTDefectDetail> defectDetailList)
        {
            try
            {
                using (IDbConnection conn = WebConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Update(defectDetailList, tran);
                    tran.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool InsertTdefectDetailList(List<callTDefectDetail> defectDetails)
        {
            try
            {
                using (IDbConnection conn = WebConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Insert(defectDetails, tran);
                    tran.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool InsertTdefect(callTDefect callTDefect)
        {
            try
            {
                using (IDbConnection conn = WebConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Insert(callTDefect, tran);
                    tran.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateTdefect(callTDefect callTDefect)
        {
            try
            {
                using (IDbConnection conn = WebConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Update(callTDefect, tran);
                    tran.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateCallResource(List<callResource> callResources)
        {
            try
            {
                using (IDbConnection conn = WebConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Update(callResources, tran);
                    tran.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateInActiveSignature(int TdefectId)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    var queryString = String.Format(
                        @"Update callResource 
                        set Active = 0 
                        where TDefectId = {0} and (ResourceTagCode like 'CUST-AF' OR ResourceTagCode like 'CUST-BF')", TdefectId.ToString());
                    var result = conn.Query(queryString);
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }

}

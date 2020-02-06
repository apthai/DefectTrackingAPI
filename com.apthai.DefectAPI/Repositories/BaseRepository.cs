using com.apthai.DefectAPI.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.DefectAPI.Repositories
{
    public abstract class BaseRepository
    {
        private readonly string _ConnectionString;
        private readonly IConfiguration _config;
        protected int ConnctionLongQuerryTimeOut = 600;
        private readonly IHostingEnvironment _hostingEnvironment;
        protected AppSettings _appSettings;

        public BaseRepository(IHostingEnvironment environment, IConfiguration config)
        {



            _config = config;
            _hostingEnvironment = environment;
            _ConnectionString = _config.GetConnectionString("DefaultConnection");
            _appSettings = config.GetSection("AppSettings").Get<AppSettings>();



        }
        protected IDbConnection WebConnection
        {
            get
            {
                var conn = Environment.GetEnvironmentVariable("DefaultConnection");
                if (conn == null)
                {
                    conn = _config.GetConnectionString("DefaultConnection");
                }
                
                return new SqlConnection(conn);
            }
        }

        protected IDbConnection SyncConnection
        {
            get
            {
                var conn = Environment.GetEnvironmentVariable("DefaultSyncConnection");
                return new SqlConnection(conn);
            }
        }

        protected IDbConnection AuthConnection
        {
            get
            {
                var conn = Environment.GetEnvironmentVariable("DefaultAuthorizeConnection");
                if (conn == null)
                {
                    conn = _config.GetConnectionString("DefaultAuthorizeConnection");
                }
                return new SqlConnection(conn);
            }
        }

        protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> getData)
        {
            try
            {
                using (IDbConnection connection = WebConnection)
                {
                    connection.Open(); //.OpenAsync(); // Asynchronously open a connection to the database
                    return await getData(connection); // Asynchronously execute getData, which has been passed in as a Func<IDBConnection, Task<T>>
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }
    }
}

using com.apthai.DefectAPI.Repositories.Interfaces;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.DefectAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly IConfiguration _config;
        IHostingEnvironment _environment;


        private IMasterRepository _masterRepository;
        private IUserRepository _userRepository;
        //private ISyncRepository _syncRepository;

        // private IGenericRepository<Post> _postRepository;
        public UnitOfWork(IHostingEnvironment environment, IConfiguration config)
        {
            _config = config;
            _environment = environment;
            //var name = TableNameMapper(typeof(User));

        }
        //private static string TableNameMapper(Type type)
        //{
        //    dynamic tableattr = type.GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == "TableAttribute");
        //    var name = string.Empty;

        //    if (tableattr != null)
        //    {
        //        name = tableattr.Name;
        //    }

        //    return name;
        //}


        
        public IMasterRepository MasterRepository
        {
            get
            {
                return _masterRepository = _masterRepository ?? new MasterRepository(_environment, _config);
            }
        }


        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository = _userRepository ?? new UserRepository(_environment, _config);
            }
        }

        //public IGenericRepository<Post> PostRepository
        //{
        //    get
        //    {
        //        return _postRepository = _postRepository ?? new GenericRepository<Post>(_bloggingContext);
        //    }
        //}

        public void Save()
        {
            // _bloggingContext.SaveChanges();
        }
    }
}

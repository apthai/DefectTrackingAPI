using com.apthai.DefectAPI.Repositories.Interfaces;

namespace com.apthai.DefectAPI.Repositories
{
    public interface IUnitOfWork
    {
        IMasterRepository MasterRepository { get; }
        //ISyncRepository SyncRepository { get; }
        IUserRepository UserRepository { get; }
        void Save();
    }
}
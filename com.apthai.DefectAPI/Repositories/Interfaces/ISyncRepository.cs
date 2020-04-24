using com.apthai.DefectAPI.Model.DefectAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.apthai.DefectAPI.Repositories
{
    public interface ISyncRepository
    {
        bool InsertCallResource(callResource data);
    }
}
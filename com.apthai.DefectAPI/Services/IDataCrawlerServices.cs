using System;
using System.Collections.Generic;
using com.apthai.DefectAPI.Model.DefectAPI;
using Newtonsoft.Json.Linq;
namespace com.apthai.CoreApp.Data.Services
{
    public interface IDataCrawlerServices
    {
        //(MProject, List<SyncModelUnitData>, List<com.apthai.DefectAPI.Model.DefectAPISync.TResource>) UnitDataCrawler(Guid sessionId, IEnumerable<JToken> jsonListObjects, DateTime syncDate, string sessionUserId, string userRole, Dictionary<string, string> fileTemp, ref List<int> returnIds, ref Dictionary<string, string> files_copy);
    }
}
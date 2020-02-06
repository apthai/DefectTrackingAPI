using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.apthai.DefectAPI.Models
{

    public class SyncRootFormat
    {

        public Guid SessionId { get; set; }
        public DateTime SessionDate { get; set; }
        public string SessionDataPath { get; set; }
       // public List<SyncData> Data { get; set; }
     
        

    }

    //public class SyncData
    //{

    //    public callTDefect HEADER { get; set; }
    //    public List<callTDefectDetail> DETAILS { get; set; }
    //    public List<callResource> RESOURCES { get; set; }

    //}

    public static class RowStates
    {

        public const string Original = "Original";// server original
        public const string AddNew = "AddNew";
        public const string Modified = "Modified";
        public const string Deleted = "Deleted";


    }


}

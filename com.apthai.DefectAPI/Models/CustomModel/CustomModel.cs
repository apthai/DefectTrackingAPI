using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.apthai.DefectAPI.Model.DefectAPI;

namespace com.apthai.DefectAPI.CustomModel
{
    public class GetCAllArea
    {
        public string ProductTypeCate { get; set; }
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
    }
    public class GetCAllType
    {
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
    }
}

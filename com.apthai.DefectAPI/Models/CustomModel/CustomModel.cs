using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.apthai.DefectAPI.Model.DefectAPI;

namespace com.apthai.DefectAPI.CustomModel
{
    public class GetCAllAreaxDescroiption
    {
        public callarea callarea { get; set; }
        public List<calldescription> calldescriptions { get; set; }
    }
    public class GetCallTransactionDefectObj 
    {
        public callTDefect callTDefect { get; set; }
        public List<callTDefectDetail> callTDefectDetail { get; set; }
    }
    public class GetCallTDefectByProjectObj : Model.DefectAPI.callTDefect   
    {
        public string Project { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApplication.Models
{
    public class mdlRecord
    {
        public Int32 PrimaryKey { get; set; }
        public string CompanyName { get; set; }
        public string GSTIN { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TrunOverAmount { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }
        public string RowValid { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(tblCngClientAccount))]
    public partial class CngClientAccount
    {
       
    }

    public class tblCngClientAccount
    {
        [Key]
        public string ID_NO { get; set; }
        public System.DateTime CNG_DATE { get; set; }
        public string LOGIN { get; set; }
        public short ACC_CODE { get; set; }
        public string LOG { get; set; }
    }
}
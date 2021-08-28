using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(tblpermissionsAssigned))]
    public partial class permissionsAssigned
    {
     
    }

    public class tblpermissionsAssigned {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int roleid { get; set; }
        public string userid { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
namespace ITC_Matrix.Models
{
    [MetadataType(typeof(tblAdminMetaData))]
    public partial class AccountCode
    {
        public IEnumerable<SelectListItem> PrevSelectaccountslist { get; set; }
        public IEnumerable<SelectListItem> Selectaccountslist { get; set; }
        public IEnumerable<SelectListItem> Accountslist { get;  }
        public string CostCentreName { get; set; }
        public bool isSelected { get; set; }

    }
    public class tblAdminMetaData
    {
       
        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter the AccCode")]
        [StringLength(50, ErrorMessage = "Please enter no more than 50 characters. ")]
        [Remote("ISAccountCodeExist", "AccountCodes", ErrorMessage = "Account Code already in use.")]
        public string AccCode { get; set; }
        
        
        [Display(Name = "Cost Center")]
        [Required(ErrorMessage = "Please select the Cost Centre")]
        public int CostCentreID { get; set; }


        [StringLength(50, ErrorMessage = "Please enter no more than 50 characters. ")]
        public string Description { get; set; }

        //[ScaffoldColumn(false)]
        public string Accounts { get; set; }

        public IEnumerable<SelectListItem> PrevSelectaccountslist { get; set; }
        public IEnumerable<SelectListItem> Selectaccountslist { get; set; }
        public IEnumerable<SelectListItem> Accountslist { get; set; }
    }
   
}
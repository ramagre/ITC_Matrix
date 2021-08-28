using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(MetaDataRightGroup))]
    public partial  class RightGroup
    { 
        public IEnumerable<SelectListItem> RegisterList { get; }
        public IEnumerable<SelectListItem> Register { get; }
        public IEnumerable<SelectListItem> ProfileList { get; }
        public IEnumerable<SelectListItem> Profile { get; }
        public IEnumerable<SelectListItem> PayMethodList { get; }
        public IEnumerable<SelectListItem> PayMethod { get; }       
    }

    public class MetaDataRightGroup
    {
        public int Code { get; set; }

        [Required(ErrorMessage = "Please enter Name.")]
        [Display(Name = "Name")]

        [Remote("IsDescAvailable", "OperatorRoles", AdditionalFields = "exitingName", ErrorMessage = "Operator name already in use.")]
        [StringLength(20, ErrorMessage = "Operator description should not be more than 20 characters.")]
        public string Dscr { get; set; }
    }
}

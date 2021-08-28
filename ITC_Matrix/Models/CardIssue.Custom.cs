using System.ComponentModel.DataAnnotations;


namespace ITC_Matrix.Models
{
    using System.Web.Mvc;

    [MetadataType(typeof(tblCardIssueData))]
    public partial class CardIssue
    {
        public string Tag_Code { get; set; }
        public string Confirmtag_Code { get; set; }

        [Required(ErrorMessage = "Please Select Expiry Date")]
        public string Expiry_Date { get; set; }
        public string StrIssue_Date { get; set; }
        public string StrReturn_Date { get; set; }

    }

    public class tblCardIssueData
    {
        public string ID_NO { get; set; }

        [Required]
        [Remote("IsTagNoAvailble", "CardIssues", ErrorMessage = "Card Number already in use. Please enter a different card number.")]
        public string TAG_NO { get; set; }

        [Required(ErrorMessage = "Please enter pin.")]        
        public string Tag_Code { get; set; }

        [Required(ErrorMessage = "Please enter confirm pin.")]
        [Compare("Tag_Code", ErrorMessage = "Confirm pin must be equal to pin entered above.")]
        public string Confirmtag_Code { get; set; }

        public System.DateTime ISSUED_AT { get; set; }
        public System.DateTime RETURNED_AT { get; set; }
        public string COMMENT { get; set; }
    }
}
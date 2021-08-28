using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(PayMethodData))]
    public partial class PayMethod
    {

    }

    public class PayMethodData
    {
        [Remote("IsCodeAvailble", "PayMethod", ErrorMessage = "CODE already in use.Please try another code.")]
        [Required(ErrorMessage = "Plese enter code.")]       
        public short CODE { get; set; }

        [Required(ErrorMessage ="Please enter description.")]
        [MaxLength(50, ErrorMessage = "Description should not be more than 50 characters.")]
        public string DSCR { get; set; }
    }
}

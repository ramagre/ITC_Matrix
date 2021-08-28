using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(RegisterGroupMetaData))]
    public partial class RegisterGroup
    {

    }

    public class RegisterGroupMetaData {
        [Remote("IsCODEAvailble", "RegisterGroup", ErrorMessage = "CODE already in use.Try another CODE")]
        [Required(ErrorMessage = "Please enter code. ")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "please enter only numbers.")]
        [Range(Int16.MinValue, Int16.MaxValue, ErrorMessage = "Code can not be grater than 32767.")]
        public short CODE { get; set; }

        [MaxLength(40, ErrorMessage = "Desciption should not be more than 40 characters.")]   
        public string DSCR { get; set; }
    }

}

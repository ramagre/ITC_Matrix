using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(tblthemeData))]
    public partial class theme
    {       
    }

    public class tblthemeData
    {
        [RegularExpression (@"^([a-zA-Z])+$", ErrorMessage ="Please enter only alphabets.")]
        [Required(ErrorMessage = "Please enter theme name.")]
        [Remote("IsExistTheme", "Themes", AdditionalFields = "themename,id", ErrorMessage = "Theme name already in use. Please enter a different theme name.")]
        public string themename { get; set; }           
    }
}
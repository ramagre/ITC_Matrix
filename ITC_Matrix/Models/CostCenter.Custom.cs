using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(MetaDataCostCenter))]

    public partial class CostCentre
    {

    }
    public class MetaDataCostCenter
    {
        [Range(0, int.MaxValue, ErrorMessage = "Code can not be grater than 2147483647.")]
        [Required(ErrorMessage = "Please enter numberic code.")]
        [RegularExpression("[0-9]*$", ErrorMessage = "Please enter only numbers.")]
        [Remote("IsCostCentreIDExist", "CostCenter", HttpMethod = "POST", ErrorMessage = "Code id already exists. Please enter a different Code Id.")]
        public int CostCentreID { get; set; }

        [Required(ErrorMessage = "Please enter description.")]
        [Display(Name = "Discription")]
        [StringLength(30, ErrorMessage = "CostCenter name should not be more than 30 characters.")]
        public string CostCentreName { get; set; }
    }

}
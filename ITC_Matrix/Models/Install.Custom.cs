using System.ComponentModel.DataAnnotations;

namespace ITC_Matrix.Models
{
    public class Install
    {
        [Required(ErrorMessage = "Please enter host name.")]
        public string HostName { get; set; }

        [Required(ErrorMessage = "Please enter maultiplan database name.")]
        public string DBMultiplan { get; set; }

        [Required(ErrorMessage = "Please enter WMM database name.")]
        public string DBWMM { get; set; }

        [Required(ErrorMessage = "Please enter user name.")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "Please enter password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }    
}
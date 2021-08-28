using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Collections.Generic;


namespace ITC_Matrix.Models
{
    [MetadataType(typeof(tblOperatorData))]
    public partial class Operator
    {
        public IEnumerable<SelectListItem> SelectGrouplist { get; set; }
        public IEnumerable<SelectListItem> RightGrouplist { get; }

        [MaxLength(20, ErrorMessage = "Password should not be more than 20 characters.")]
        //[Required(ErrorMessage = "Please enter password.")]
        [DataType(DataType.Password)]
        public string PASSWORD { get; set; }

        public string OldPassword { get; set; }


        [Required(ErrorMessage = "Please enter Login name")]
        [Remote("CheckUserNameExists", "Operators", ErrorMessage = "Username already exists!")]
        [MaxLength(9, ErrorMessage = "Login name should not be more than 9 characters.")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Please enter email.")]
        [Remote("CheckEmailExists", "Operators", ErrorMessage = "Email already exists!")]
        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please enter correct email.")]
        public string OperatorEmail { get; set; }

        [MaxLength(20, ErrorMessage = "Confirm password should not be more than 20 characters.")]
        //[Required(ErrorMessage = "Please enter confirm password.")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("PASSWORD", ErrorMessage = "The password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter Login name")]

        [MaxLength(9, ErrorMessage = "Login name should not be more than 9 characters.")]
        public string LOGIN { get; set; }


        [MaxLength(20, ErrorMessage = "Password should not be more than 20 characters.")]
        [Required(ErrorMessage = "Please enter password.")]        
        [DataType(DataType.Password)]
        public string CreatePASSWORD { get; set; }

        [MaxLength(20, ErrorMessage = "Confirm password should not be more than 20 characters.")]
        [Required(ErrorMessage = "Please enter confirm password.")]       
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("CreatePASSWORD", ErrorMessage = "The password and confirm password do not match.")]
        public string CreateConfirmPassword { get; set; }


    }

    public class tblOperatorData
    {
        [MaxLength(20, ErrorMessage = "Confirm password should not be more than 20 characters.")]
        //[Required(ErrorMessage = "Please enter confirm password.")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("PASSWORD", ErrorMessage = "The password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter Login name")]

        [MaxLength(9, ErrorMessage = "Login name should not be more than 9 characters.")]
        public string LOGIN { get; set; }

        [Required(ErrorMessage = "Please enter Last name")]
        [MaxLength(40, ErrorMessage = "Last name should not be more than 40 characters.")]
        public string FAMILY { get; set; }

        [Required(ErrorMessage = "Please enter family name")]
        [MaxLength(20, ErrorMessage = "First name should not be more than 20 characters.")]
        public string GIVEN { get; set; }

        public short RightGroup { get; set; }

        [MaxLength(50, ErrorMessage = "Comment should not be more than 50 characters.")]
        public string COMMENT { get; set; }

        public string DevGroup { get; set; }
        public string Profile { get; set; }
        public string Account { get; set; }
        public Nullable<short> DP { get; set; }
        
        [Required(ErrorMessage = "Please enter email.")]
        //[Remote("CheckEmailExists", "Operators", ErrorMessage = "Email already exists!")]
        //[DataType(DataType.EmailAddress)]     
        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please enter correct email.")]
        public string Email { get; set; }



    }
}
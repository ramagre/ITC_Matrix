using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(MetaDataAccount))]
    public partial class Account
    {

        //public short FreePrint { get; set; }

        [NotMapped]
        public bool AllowFreeBool           //for FreePrint Value true or false
        {
            get { return FreePrint == 1 ? true : false; }
            set { FreePrint = value ? (short)1 : (short)0; }
        }
        public bool Isselected { get; set; }
        public string AccountDiscount { get; set; }
    }
    public class MetaDataAccount
        {
        //for Auto-Increment Code
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        [Remote("doesCodeIDExist", "AccountType", HttpMethod = "POST", ErrorMessage = "Code Id already exists.Please enter a different Code Id.")]
        public short CODE { get; set; }
            // [Key]


            [Required(ErrorMessage = "Please enter description.")]
            [Display(Name = "Description")]
            [StringLength(20, ErrorMessage = "Description should not be more than 20 characters.")]
            public string DSCR { get; set; }

            [Display(Name = "UP800 Total")]
            public string TotalUse { get; set; }

           [Display(Name = "Allow Free Print")]
           public short FreePrint { get; set; }

        [Display(Name = " Second Description")]
            [StringLength(20, ErrorMessage = "Second description should not be more than 20 characters.")]
            public string SecondDSCR { get; set; }
        }

    }

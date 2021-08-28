using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(ProfileMetaData))]
    public partial class Profile
    {
        public Profile()
        {
            DSCR = string.Empty;
            DiscountPercent = 0;
            ResetBalanceEnabled = 0;
            ResetBalanceValue = 0;
            ResetFrequency = 0;
            ResetDayOfWeek = 1;
            ResetDayOfMonth = 1;
            ResetBalanceFrom = 0;
            ResetBalanceTo = 0;
            ResetBalanceLast = System.DateTime.Now;
            IsCashOnly = 0;
            RemoveTax1 = 0;
            RemoveTax2 = 0;
            RemoveTax3 = 0;
            RemoveTax4 = 0;
            RemoveTax5 = 0;
            ProCompleteNonTaxableAccount = 0;
            AccCode = 1;
            SecondDSCR = string.Empty;
            isSelected = false;
        }

        public short ResetBalanceEnabled { get; set; }

        [NotMapped]
        public bool ResetBalanceEnabledBool
        {
            get { return ResetBalanceEnabled == 1 ? true : false; }
            set { ResetBalanceEnabled = value ? (short)1 : (short)0; }
        }

        public short ResetFrequency { get; set; }

        [NotMapped]
        public bool ResetFrequencyBool
        {
            get { return ResetFrequency == 1 ? true : false; }
            set { ResetFrequency = value ? (short)1 : (short)0; }
        }

        public short RemoveTax1 { get; set; }

        [NotMapped]
        public bool RemoveTax1Bool
        {
            get { return RemoveTax1 == 1 ? true : false; }
            set { RemoveTax1 = value ? (short)1 : (short)0; }
        }

        public short RemoveTax2 { get; set; }
        [NotMapped]
        public bool RemoveTax2Bool
        {
            get { return RemoveTax2 == 1 ? true : false; }
            set { RemoveTax2 = value ? (short)1 : (short)0; }
        }

        public short RemoveTax3 { get; set; }
        [NotMapped]
        public bool RemoveTax3Bool
        {
            get { return RemoveTax3 == 1 ? true : false; }
            set { RemoveTax3 = value ? (short)1 : (short)0; }
        }

        public short RemoveTax4 { get; set; }

        [NotMapped]
        public bool RemoveTax4Bool
        {
            get { return RemoveTax4 == 1 ? true : false; }
            set { RemoveTax4 = value ? (short)1 : (short)0; }
        }

        public short RemoveTax5 { get; set; }

        [NotMapped]
        public bool RemoveTax5Bool
        {
            get { return RemoveTax5 == 1 ? true : false; }
            set { RemoveTax5 = value ? (short)1 : (short)0; }
        }

        public short ProCompleteNonTaxableAccount { get; set; }

        [NotMapped]
        public bool ProCompleteNonTaxableAccountBool
        {
            get { return ProCompleteNonTaxableAccount == 1 ? true : false; }
            set { ProCompleteNonTaxableAccount = value ? (short)1 : (short)0; }
        }

        public short FromHH { get; set; }
        public short FromMM { get; set; }

        public short ToHH { get; set; }
        public short ToMM { get; set; }

        public short WeekDays { get; set; }
    }
    public class ProfileMetaData
    {
        [Remote("IsClientProfileIDAvailble", "Profile", ErrorMessage = "ClientID already in use.")]
        [Required(ErrorMessage = "Please enter code.")]
        [RegularExpression("^[0-9]*$", ErrorMessage ="Please enter only numbers.")]

        [Range(Int16.MinValue, Int16.MaxValue, ErrorMessage = "Code can not be grater than 32767.")]
        public short? CODE { get; set; }

        [MaxLength(20,ErrorMessage = "Desciption should not be more than 20 characters.")]
        [Required(ErrorMessage = "Please enter the desciption.")]
        public string DSCR { get; set; }

        [MaxLength(20, ErrorMessage = "Second description should not be more than 20 characters.")]
        public string SecondDSCR { get; set; }

        public int? DiscountPercent { get; set; }

        public short? ResetBalanceEnabled { get; set; }

        [NotMapped]
        public bool ResetBalanceEnabledBool
        {
            get { return ResetBalanceEnabled == 1 ? true : false; }
            set { ResetBalanceEnabled = value ? (short)1 : (short)0; }
        }
        
        [Required(ErrorMessage ="Please enter 0 for blank value.")]
        [RegularExpression("^\\d+(?:\\.\\d{0,2})?$", ErrorMessage = "Please enter only numbers.")]       
        public int ResetBalanceValue { get; set; }

        public short? ResetFrequency { get; set; }

        public short? ResetDayOfWeek { get; set; }

        public short? ResetDayOfMonth { get; set; }

        public short? ResetBalanceFrom { get; set; }

        public short? ResetBalanceTo { get; set; }

        

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ResetBalanceLast { get; set; }
        
    }
}

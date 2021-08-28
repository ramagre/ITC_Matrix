using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(tblPlanData))]
    public partial class Plan
    {
        public string sun { get; set; }
        public string mon { get; set; }
        public string tues { get; set; }
        public string wesd { get; set; }
        public string thur { get; set; }
        public string fri { get; set; }
        public string sat { get; set; }
        public string mealType { get; set; }
        [NotMapped]
        public bool Bool_IsTotalMealsPerPlan
        {
            get { return IsTotalMealsPerPlan == 1 ? true : false; }
            set { IsTotalMealsPerPlan = value ? (short)1 : (short)0; }
        }
        [NotMapped]
        public bool Bool_IsTotalMealsPerWeek
        {
            get { return IsTotalMealsPerWeek == 1 ? true : false; }
            set { IsTotalMealsPerWeek = value ? (short)1 : (short)0; }
        }
        [NotMapped]
        public bool Bool_IsTotalAmountPerPlan
        {
            get { return IsTotalAmountPerPlan == 1 ? true : false; }
            set { IsTotalAmountPerPlan = value ? (short)1 : (short)0; }
        }
        [NotMapped]
        public bool Bool_IS_EXPIRY_DATE
        {
            get { return IS_EXPIRY_DATE == 1 ? true : false; }
            set { IS_EXPIRY_DATE = value ? (short)1 : (short)0; }
        }
        [NotMapped]
        public bool Bool_IS_DAYS
        {
            get { return IS_DAYS == 1 ? true : false; }
            set { IS_DAYS = value ? (short)1 : (short)0; }
        }
        [NotMapped]
        public bool Bool_AuthorizationRequired
        {
            get { return AuthorizationRequired == 1 ? true : false; }
            set { AuthorizationRequired = value ? (short)1 : (short)0; }
        }
        [NotMapped]
        public bool Bool_EnableMultipleMeals
        {
            get { return EnableMultipleMeals == 1 ? true : false; }
            set { EnableMultipleMeals = value ? (short)1 : (short)0; }
        }
        
    }
    public class tblPlanData
    {
        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter the code")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Please enter only numbers.")]
        [Remote("IsCodeExist", "MealPlans", HttpMethod = "POST", ErrorMessage = "Code already in use.Please try another Code.")]
        public int CODE { get; set; }

        [Required(ErrorMessage = "Please enter description")]
        [DisplayName("Description")]
        public string DSCR { get; set; }
        public string REGS { get; set; }
        public string MEALS { get; set; }
        public short IS_DAYS { get; set; }
        [Required(ErrorMessage = "Please enter days")]
        public short DAYS { get; set; }
        public System.DateTime START_DATE { get; set; }
        public short IS_EXPIRY_DATE { get; set; }
        public System.DateTime EXPIRY_DATE { get; set; }
        public short IsTotalMealsPerPlan { get; set; }
        [Required(ErrorMessage = "Please enter total meals per plan")]
        public short TotalMealsPerPlan { get; set; }
        public int IsTotalAmountPerPlan { get; set; }
        [Required(ErrorMessage = "Please enter amount per plan")]
        public int TotalAmountPerPlan { get; set; }
        public short IsTotalMealsPerWeek { get; set; }
        [Required(ErrorMessage = "Please enter total meals per week")]
        public short TotalMealsPerWeek { get; set; }
        public short EnableMultipleMeals { get; set; }
        [Required(ErrorMessage = "Please enter price")]
        public int PRICE { get; set; }
        [Required(ErrorMessage ="Please select print mode")]
        public short PrintMode { get; set; }
        public string LastUpdatedBy { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public short AccountID { get; set; }
        public int Bonus { get; set; }
        [Required(ErrorMessage = "Please enter value of PassBack")]
        public short PassBack { get; set; }
        public short AuthorizationRequired { get; set; }
        public short DaysPerWeek { get; set; }
        public string SecondDSCR { get; set; }
        public Nullable<short> AllowParking { get; set; }
        public Nullable<int> ParkGroup { get; set; }
        public Nullable<short> TaxExempt { get; set; }
        public Nullable<int> BonusPlanCode { get; set; }
    }
}
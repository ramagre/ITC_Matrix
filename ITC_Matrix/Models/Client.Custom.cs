/*  
    File Name : Client.Custom.cs
    File For : adding the valiadtions on properties
    Created Date : 11-9-2015
    created by : Sandip Katore
    Modified Date : 
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(ClientMetaData))]

    public partial class Client
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();

        public string Profile_DSCR { get; set; }

        public int CODE { get; set; }
        
        public List<string> ActBalList { get; set; }
                
        public static IEnumerable<AccountCode> accountsCodeList()
        {
            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
            return db.AccountCodes.ToList();
        }

        public Client()
        {
            this.ClientAccounts = new HashSet<ClientAccount>();
            this.ClientPlansUnlimiteds = new HashSet<ClientPlansUnlimited>();

            ADDRESS = string.Empty;
            GIVEN = string.Empty;
            PHONE = string.Empty;
            CITY = string.Empty; ;
            STATE = string.Empty; ;
            ZIP = string.Empty; ;
            COUNTRY = string.Empty; ;
            COMMENT = string.Empty; ;
            ACC_BALANCE1 = 0;
            ACC_LIMIT1 = 0;
            ACC_UNLIMITED1 = 0;
            ACC_BALANCE2 = 0;
            ACC_LIMIT2 = 0;
            ACC_UNLIMITED2 = 0;
            ACC_BALANCE3 = 0;
            ACC_LIMIT3 = 0;
            ACC_UNLIMITED3 = 0;
            PLAN_START1 = System.DateTime.Now;
            PLAN_START2 = System.DateTime.Now;
            PLAN_START3 = System.DateTime.Now;
            TAG_CODE = string.Empty;
            TAG_NO = string.Empty;
            TAG_ISSUE = System.DateTime.Now;
            TAG_EXPIRY = System.DateTime.Now;
            TAG_STATUS = 1;
            LastMealTime = System.DateTime.Now;
            WeekMealCount = 0;
            LastPlanCode = 0;
            HoldExpiry = System.DateTime.Now;
            HoldRegisterID = 0;
            Faculty = string.Empty; ;
            PrOfStudy = string.Empty; ;
            Department = string.Empty; ;
            Title = string.Empty;
            EMail = string.Empty; ;
            NetID = string.Empty; ;
            Allow_Undefined = 0;
            RefundLastMealTime = System.DateTime.Now;
            RefundLastPlanCode = 0;
            ACC_BALANCE4 = 0;
            ACC_LIMIT4 = 0;
            ACC_UNLIMITED4 = 0;
            ACC_BALANCE5 = 0;
            ACC_LIMIT5 = 0;
            ACC_UNLIMITED5 = 0;
            Allow_FreePrint = 0;
            WeekDayCount = 0;
            EasyConvert_Admin = 0;
            WeekMealCount2 = 0;
            WeekMealCount3 = 0;
            DOB = System.DateTime.Now;
            LanguagePreference = 1;
            TermCondAccept = 0;
            TermCondAcceptDate = System.DateTime.Now;
            LastMealTime2 = System.DateTime.Now;
            LastMealTime3 = System.DateTime.Now;
            GradeLevel = string.Empty; ;
            ResidencyStatus = string.Empty; ;
            EmailFlag = 0;
            PinMustChange = 0;
            LockoutAttempts = 0;
            Lockout = 0;
        }

        [NotMapped]
        public bool AllowMailingBool
        {
            get { return EmailFlag == 1 ? true : false; }
            set { EmailFlag = value ? (short)1 : (short)0; }
        }

        [NotMapped]
        public bool EasyConvert_AdminBool
        {
            get { return EasyConvert_Admin == 1 ? true : false; }
            set { EasyConvert_Admin = value ? (short)1 : (short)0; }
        }
        [NotMapped]
        public bool Allow_UndefinedBool
        {
            get { return Allow_Undefined == 1 ? true : false; }
            set { Allow_Undefined = value ? (short)1 : (short)0; }
        }

        [NotMapped]
        public bool Allow_FreePrintBool
        {
            get { return Allow_FreePrint == 1 ? true : false; }
            set { Allow_FreePrint = value ? (short)1 : (short)0; }
        }
    }

    public class ClientMetaData
    {
        [Remote("IsClientIDAvailble", "Clients", ErrorMessage = "ClientID already in use.")]
        [Required(ErrorMessage = "Please enter clientId.")]
        public string ID_NO { get; set; }

        [Required(ErrorMessage = "Please select profile.")]
        public short PROFILE { get; set; }

        [MaxLength(20, ErrorMessage = "Family Name should not be more than 20 characters.")]
        [RegularExpression("^[a-zA-Z][a-zA-Z\\s]+$", ErrorMessage = "Please enter only characters.")]
        [Required(ErrorMessage = "Please enter family.")]
        public string FAMILY { get; set; }

        [MaxLength(20, ErrorMessage = "Given Name should not be more than 20 characters.")]
        [RegularExpression("^[a-zA-Z][a-zA-Z\\s]+$", ErrorMessage = "Enter only characters.")]
        [Required(ErrorMessage = "Please enter given name.")]
        public string GIVEN { get; set; }

        [RegularExpression("^[0-9\\(\\)\\-\\+\\s]+$", ErrorMessage = "Enter valid phone number.")]
        [MaxLength(25, ErrorMessage = "Phone should not be more than 25 characters.")]
        public string PHONE { get; set; }

        [MaxLength(250, ErrorMessage = "Address should not be more than 250 characters.")]
        public string ADDRESS { get; set; }

        [MaxLength(20, ErrorMessage = "City should not be more than 20 characters.")]
        [RegularExpression("^[a-zA-Z][a-zA-Z\\s]+$", ErrorMessage = "Please enter only characters.")]
        public string CITY { get; set; }

        [RegularExpression("^[a-zA-Z][a-zA-Z\\s]+$", ErrorMessage = "Please enter only characters.")]
        [MaxLength(2, ErrorMessage = "State should not be more than 2 characters. ")]
        public string STATE { get; set; }
        
        [MaxLength(10,ErrorMessage = "Please enter valid ZIP number.")]
        [DataType(DataType.PostalCode)]
        public string ZIP { get; set; }

        [RegularExpression("^[a-zA-Z][a-zA-Z\\s]+$", ErrorMessage = "Please enter only characters.")]
        [MaxLength(20, ErrorMessage = "Country should not be more than 20 characters.")]
        public string COUNTRY { get; set; }

        [MaxLength(40, ErrorMessage = "Comment should not be more than 40 characters.")]
        public string COMMENT { get; set; }

        [MaxLength(40, ErrorMessage = "Faculty should not be more than 40 characters.")]
        [RegularExpression("^[a-zA-Z][a-zA-Z\\s\\&.]+$", ErrorMessage = "Please enter only Characters.")]
        public string Faculty { get; set; }

        [MaxLength(40, ErrorMessage = "Program should not be more than 40 characters.")]
        [RegularExpression("^[a-zA-Z][a-zA-Z\\s\\.&]+$", ErrorMessage = "Please  enter only characters.")]
        public string PrOfStudy { get; set; }

        [MaxLength(40, ErrorMessage = "Title should not be more than 40 characters.")]
        [RegularExpression("^[a-zA-Z][a-zA-Z\\s\\.]+$", ErrorMessage = "Enter only Characters.")]
        public string Title { get; set; }

        [MaxLength(250, ErrorMessage = "Email should not be more than 250 characters.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter valid email ID.")]
        public string EMail { get; set; }

        [MaxLength(255, ErrorMessage = "Network should not be more than 255 characters.")]
        [RegularExpression("^[^<> '\"/;`%]*$", ErrorMessage = "Please enter only characters.")]
        [Remote("IsNetIDAvailble", "Clients", ErrorMessage = "NetworkID already in use.")]
        public string NetID { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? PLAN_START1 { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? PLAN_START2 { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? PLAN_START3 { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? TAG_ISSUE { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? TAG_EXPIRY { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? LastMealTime { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? HoldExpiry { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? RefundLastMealTime { get; set; }

        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Please select DOB.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? DOB { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? TermCondAcceptDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? LastMealTime2 { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? LastMealTime3 { get; set; }

    }
}
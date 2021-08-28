using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace ITC_Matrix.Models
{

    [MetadataType(typeof(MetaDataDelClient))]
    public  partial class DelClient
    {
       
        public int Transactions { get; set; }
        public string Profile_DSCR { get; set; }

        public class Paging_list
        {
            public PagedList.IPagedList<DelClient> Paging { get; set; }
        }

        public DelClient()
        {
            //this.ClientAccounts = new HashSet<ClientAccount>();
            //ClientPlansUnlimiteds = new HashSet<ClientPlansUnlimited>();

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
            Title = string.Empty; ;
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
            LanguagePreference = 0;
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
            Transactions = 0;
        }
    }

    public class MetaDataDelClient
    {
        public string ID_NO { get; set; }
        public short PROFILE { get; set; }
        public string FAMILY { get; set; }
        public string GIVEN { get; set; }
        public string PHONE { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ZIP { get; set; }
        public string COUNTRY { get; set; }
        public string COMMENT { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public int ACC_BALANCE1 { get; set; }
        public int ACC_LIMIT1 { get; set; }
        public short ACC_UNLIMITED1 { get; set; }
        public int ACC_BALANCE2 { get; set; }
        public int ACC_LIMIT2 { get; set; }
        public short ACC_UNLIMITED2 { get; set; }
        public int ACC_BALANCE3 { get; set; }
        public int ACC_LIMIT3 { get; set; }
        public short ACC_UNLIMITED3 { get; set; }
        public int PLAN_CODE1 { get; set; }
        public int PLAN_CODE2 { get; set; }
        public int PLAN_CODE3 { get; set; }
        public System.DateTime PLAN_START1 { get; set; }
        public System.DateTime PLAN_START2 { get; set; }
        public System.DateTime PLAN_START3 { get; set; }
        public string TAG_CODE { get; set; }
        public string TAG_NO { get; set; }
        public System.DateTime TAG_ISSUE { get; set; }
        public System.DateTime TAG_EXPIRY { get; set; }
        public short TAG_STATUS { get; set; }
        public System.DateTime LastMealTime { get; set; }
        public short WeekMealCount { get; set; }
        public int LastPlanCode { get; set; }
        public System.DateTime HoldExpiry { get; set; }
        public int HoldRegisterID { get; set; }
        public string Faculty { get; set; }
        public string PrOfStudy { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public string EMail { get; set; }
        public string NetID { get; set; }
        public short Allow_Undefined { get; set; }
        public System.DateTime RefundLastMealTime { get; set; }
        public int RefundLastPlanCode { get; set; }
        public int ACC_BALANCE4 { get; set; }
        public int ACC_LIMIT4 { get; set; }
        public short ACC_UNLIMITED4 { get; set; }
        public int ACC_BALANCE5 { get; set; }
        public int ACC_LIMIT5 { get; set; }
        public short ACC_UNLIMITED5 { get; set; }
        public short Allow_FreePrint { get; set; }
        public short WeekDayCount { get; set; }
        public short EasyConvert_Admin { get; set; }
        public Nullable<short> WeekMealCount2 { get; set; }
        public Nullable<short> WeekMealCount3 { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<short> LanguagePreference { get; set; }
        public Nullable<short> TermCondAccept { get; set; }
        public Nullable<System.DateTime> TermCondAcceptDate { get; set; }
        public Nullable<System.DateTime> LastMealTime2 { get; set; }
        public Nullable<System.DateTime> LastMealTime3 { get; set; }
        public string GradeLevel { get; set; }
        public string ResidencyStatus { get; set; }
        public short EmailFlag { get; set; }
        public Nullable<short> PinMustChange { get; set; }
        public Nullable<System.DateTime> LastWebPurchaseLogin { get; set; }
        public Nullable<short> LockoutAttempts { get; set; }
        public Nullable<System.DateTime> PinChangeDate { get; set; }
        public Nullable<short> Lockout { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public string ParkMember { get; set; }
        public string OperatorID { get; set; }
        public string AppName { get; set; }
        public string AppID { get; set; }
        public string IPAddress { get; set; }

        //public int Transactions { get; set; }
    }
}
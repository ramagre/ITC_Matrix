//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ITC_Matrix.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Client
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public Client()
        //{
        //    this.ClientAccounts = new HashSet<ClientAccount>();
        //    this.ClientPlansUnlimiteds = new HashSet<ClientPlansUnlimited>();
        //    this.ClientAlerts = new HashSet<ClientAlert>();
        //}
    
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
        public short WeekMealCount2 { get; set; }
        public short WeekMealCount3 { get; set; }
        public System.DateTime DOB { get; set; }
        public short LanguagePreference { get; set; }
        public short TermCondAccept { get; set; }
        public System.DateTime TermCondAcceptDate { get; set; }
        public System.DateTime LastMealTime2 { get; set; }
        public System.DateTime LastMealTime3 { get; set; }
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
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientAccount> ClientAccounts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientPlansUnlimited> ClientPlansUnlimiteds { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientAlert> ClientAlerts { get; set; }
    }
}

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
    
    public partial class TrnPlan
    {
        public string ID_NO { get; set; }
        public int PLAN_CODE { get; set; }
        public System.DateTime PLAN_START { get; set; }
        public int PLAN_PRICE { get; set; }
        public System.DateTime TRN_DATE { get; set; }
        public string LOGIN { get; set; }
        public string COMMENT { get; set; }
        public Nullable<short> CreditCardType { get; set; }
        public string CreditCardAuthNumber { get; set; }
        public Nullable<short> SOURCE { get; set; }
        public Nullable<System.DateTime> PLAN_END { get; set; }
        public Nullable<short> CardPurchased { get; set; }
        public Nullable<short> ACC_CODE { get; set; }
        public Nullable<short> IsPlanNew { get; set; }
        public long ID { get; set; }
    }
}

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
    
    public partial class CreditCardProcessSession
    {
        public string RemittanceId { get; set; }
        public Nullable<int> Amount { get; set; }
        public string ClientId { get; set; }
        public Nullable<short> AccountId { get; set; }
        public string NotifyResponse { get; set; }
        public Nullable<int> IsFullAccess { get; set; }
        public Nullable<int> MealPlanCode { get; set; }
        public Nullable<int> BonusAmount { get; set; }
        public Nullable<int> MealPlanPrice { get; set; }
        public System.DateTime Created { get; set; }
        public Nullable<System.DateTime> PlanStartDate { get; set; }
        public Nullable<System.DateTime> PlanEndDate { get; set; }
    }
}

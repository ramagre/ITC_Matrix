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
    
    public partial class reportsStored
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string @class { get; set; }
        public string reportType { get; set; }
        public string reportFlags { get; set; }
        public string dateType { get; set; }
        public Nullable<System.DateTime> dateFrom { get; set; }
        public Nullable<System.DateTime> dateTo { get; set; }
        public string profiles { get; set; }
        public string accounts { get; set; }
        public string plans { get; set; }
        public string devices { get; set; }
        public string transactionTypes { get; set; }
        public string groupBy { get; set; }
        public Nullable<int> status { get; set; }
        public string deviceGroups { get; set; }
        public string deviceTypes { get; set; }
        public string identifier { get; set; }
        public string operators { get; set; }
        public string payMethods { get; set; }
        public string creator_id { get; set; }
        public string modifier_id { get; set; }
        public string orientation { get; set; }
        public int restrictrole { get; set; }
    }
}

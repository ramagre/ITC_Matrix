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
    
    public partial class TrnRegNoClient
    {
        public int REG_CODE { get; set; }
        public System.DateTime REG_DATE { get; set; }
        public int TRN_NO { get; set; }
        public short TrnCode { get; set; }
        public string TAG_NO { get; set; }
        public short Desktop { get; set; }
        public System.DateTime TRN_DATE { get; set; }
        public int AMOUNT { get; set; }
        public string AccCode { get; set; }
        public int Tax1 { get; set; }
        public int Tax2 { get; set; }
        public int Tax3 { get; set; }
        public int Tax4 { get; set; }
        public short ApplicationId { get; set; }
        public string AppSpecificData { get; set; }
    }
}

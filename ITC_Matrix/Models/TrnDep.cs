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
    
    public partial class TrnDep
    {
        public string ID_NO { get; set; }
        public short ACC_CODE { get; set; }
        public System.DateTime TRN_DATE { get; set; }
        public int AMOUNT { get; set; }
        public int BALANCE { get; set; }
        public string LOGIN { get; set; }
        public short SOURCE { get; set; }
        public string COMMENT { get; set; }
        public short PLANCODE { get; set; }
        public short MEALCODE { get; set; }
        public short MEALS { get; set; }
        public short CreditCardType { get; set; }
        public string CreditCardAuthNumber { get; set; }
        public long ID { get; set; }
    }
}

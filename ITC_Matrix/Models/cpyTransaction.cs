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
    
    public partial class cpyTransaction
    {
        public int DeviceID { get; set; }
        public System.DateTime TransDate { get; set; }
        public int TransNo { get; set; }
        public short PriceLine { get; set; }
        public short PaperID { get; set; }
        public int Copies { get; set; }
        public int Amount { get; set; }
        public short JobID { get; set; }
    }
}

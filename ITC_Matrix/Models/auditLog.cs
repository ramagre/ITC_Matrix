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
    
    public partial class auditLog
    {
        public int id { get; set; }
        public string tablename { get; set; }
        public string tableid { get; set; }
        public string @operator { get; set; }
        public System.DateTime dateis { get; set; }
        public string action { get; set; }
        public string actiondetail { get; set; }
        public string ipaddress { get; set; }
        public string useragent { get; set; }
        public string sessionid { get; set; }
    }
}
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
    
    public partial class loginLog
    {
        public int id { get; set; }
        public string @operator { get; set; }
        public System.DateTime logindate { get; set; }
        public string ipaddress { get; set; }
        public string useragent { get; set; }
        public Nullable<System.DateTime> logoutTime { get; set; }
        public int logoutTrue { get; set; }
        public string sessionid { get; set; }
    }
}
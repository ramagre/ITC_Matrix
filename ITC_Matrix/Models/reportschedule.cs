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
    
    public partial class reportschedule
    {
        public int id { get; set; }
        public string scheduleName { get; set; }
        public int runtime { get; set; }
        public int timezone { get; set; }
        public string shortDesc { get; set; }
        public string day { get; set; }
        public string month { get; set; }
        public int status { get; set; }
        public int locked { get; set; }
        public string typekey { get; set; }
        public Nullable<System.DateTime> specificDay { get; set; }
    }
}

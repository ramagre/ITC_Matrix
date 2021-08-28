using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITC_Matrix.Models
{
    public class Setup
    {
        public string MatrixKey { get; set; }
        public string ReportFromEmail { get; set; }
        public string BaseURL { get; set; }
        public string HtmlDocPath { get; set; }
        public string PDFStorage { get; set; }
        public string SiteRoot { get; set; }
        public string SessionName { get; set; }
        public int SessionTimeout { get; set; }
    }
}
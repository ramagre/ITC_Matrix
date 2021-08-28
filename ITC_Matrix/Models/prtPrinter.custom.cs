using System.ComponentModel.DataAnnotations;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(tblprtPrinterMetaData))]
    public partial class prtPrinter
    {
        public string PrinterGroupName { get; set; }
    }
    public class tblprtPrinterMetaData
    {
        public int PrinterID { get; set; }
        public string PrinterName { get; set; }
        public int PrinterGroupID { get; set; }
        public int DeviceID { get; set; }
    }
}
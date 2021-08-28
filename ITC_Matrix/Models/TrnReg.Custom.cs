using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(tblTrnRegData))]
    public partial class TrnReg
    {
        public bool flgCredit { get; set; }
        public string DeviceDesc { get; set; }
        public string MealDesc { get; set; }
        public string strTrnDate { get; set; }
        public bool IsTaken { get; set; }
        public string strPaidby { get; set; }
    }
    public class tblTrnRegData
    {
        public int REG_CODE { get; set; }
        public System.DateTime REG_DATE { get; set; }
        public int TRN_NO { get; set; }
        public short TrnCode { get; set; }
        public short ACC_CODE { get; set; }
        public string ID_NO { get; set; }
        public int AMOUNT { get; set; }
        public string TAG_NO { get; set; }
        public short Desktop { get; set; }
        public int PLAN_CODE { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public System.DateTime TRN_DATE { get; set; }
        public int BALANCE { get; set; }
        public short ON_LINE { get; set; }
        public short MEAL_CODE { get; set; }
        public string AccCode { get; set; }
        public short ApplicationId { get; set; }
        public int DISCOUNT { get; set; }
        public short PaymentId { get; set; }
        public string Comment { get; set; }
        public Nullable<short> CreditCardType { get; set; }
        public Nullable<int> CreditCardAuthNumber { get; set; }
        

    }
}
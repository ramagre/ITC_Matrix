using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(tblRegisterData))]
    public partial class Register
    {
        public IEnumerable<SelectListItem> PrevSelectaccountslist { get; set; }
        public IEnumerable<SelectListItem> Selectaccountslist { get; set; }
        public IEnumerable<SelectListItem> Accountslist { get; }
        public IEnumerable<SelectListItem> Timelist { get; }
        public string GroupDesc { get; set; }
        public string deviceTypeName { get; set; }
        public int status { get; set; }

        public bool IsSelectedCheck { get; set; }

        [NotMapped]
        public bool Enabled_Bool
        {
            get { return Enabled == 1 ? true : false; }
            set { Enabled = value ? (short)1 : (short)0; }
        }
       

        [NotMapped]
        public bool UseExtendedPassBack_Bool
        {
            get { return UseExtendedPassBack == 1 ? true : false; }
            set { UseExtendedPassBack = value ? (short)1 : (short)0; }
        }

        [NotMapped]
        public bool ModemEnabled_Bool
        {
            get { return ModemEnabled == 1 ? true : false; }
            set { ModemEnabled = value ? (short)1 : (short)0; }
        }

        [NotMapped]
        public bool ExtendedPassBackReset_Bool
        {
            get { return ExtendedPassBackReset == 1 ? true : false; }
            set { ExtendedPassBackReset = value ? (short)1 : (short)0; }
        }

    }
    public class tblRegisterData
    {
        [Required(ErrorMessage = "Please enter the code")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Please enter only numbers.")]
        [Remote("IsCodeExist", "Registers", HttpMethod = "POST", ErrorMessage = "Code already exists.Please try another code.")]
        public int CODE { get; set; }
        [Required(ErrorMessage = "Please enter the description")]
        public string DSCR { get; set; }
        public short Enabled { get; set; }
        [RegularExpression(@"^([\d]{1,3}\.){3}[\d]{1,3}$", ErrorMessage = "Please check the IP address format")]
        public string IPAddress { get; set; }
        public string ACCOUNTS { get; set; }
        public string LoadOldTimeMap { get; set; }
        public string OffFileTimeMap { get; set; }
        public short OffFileNegativeEnabled { get; set; }
        public int OffFileMaxNumber { get; set; }
        public short ModemEnabled { get; set; }
        public string ModemPhoneNumber { get; set; }
        public int RegisterType { get; set; }
        public short GROUP_CODE { get; set; }
        public short Desktop { get; set; }
        public int ERA570OfflineMax { get; set; }
        public string Budget { get; set; }
        public string AccountDiscounts { get; set; }
        public short UseExtendedPassBack { get; set; }
        [Required(ErrorMessage = "Please enter Extended PassBack")]
        public short ExtendedPassBack { get; set; }
        public System.DateTime LastConnectionTime { get; set; }

        public string MACAddress { get; set; }
        public short Initializetype { get; set; }
        public short ExtendedPassBackReset { get; set; }
        public Nullable<System.DateTime> NewCardsLastSyncTime { get; set; }
        public Nullable<System.DateTime> DeletedCardsLastSyncTime { get; set; }
        public Nullable<short> PrimaryCredentialOnly { get; set; }
        public Nullable<short> CreateVirtualPlan { get; set; }
    }
}
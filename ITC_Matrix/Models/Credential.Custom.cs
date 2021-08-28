using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(tblCredentialData))]

    public partial class Credential
    {
        public Credential()
        {
            Auth2 = string.Empty;
            ChangedDate = DateTime.Now;
            Locked = 0;
            LockoutAttempts = 0;
            Reset = 0;
            Valid = 1;
            ReturnedDate = Convert.ToDateTime("2173-10-14");
        }

        [NotMapped]
        public bool Auth2Required_bool
        {
            get { return Auth2Required == 1 ? true : false; }
            set { Auth2Required = value ? (short)1 : (short)0; }
        }

        [NotMapped]
        public bool PrimaryCredential_bool
        {
            get { return PrimaryCredential == 1 ? true : false; }
            set { PrimaryCredential = value ? (short)1 : (short)0; }
        }

        [NotMapped]
        public bool ManuelEntryAllowed_bool
        {
            get { return ManuelEntryAllowed == 1 ? true : false; }
            set { ManuelEntryAllowed = value ? (short)1 : (short)0; }
        }

        [NotMapped]
        public bool Valid_bool
        {
            get { return Valid == 1 ? true : false; }
            set { Valid = value ? (short)1 : (short)0; }
        }

        public string Confirm_Auth2 { get; set; }

        public class tblCredentialData
        {
            [Required(ErrorMessage ="Please enter credential.")]
            [Remote("IsCredentialAvailble", "Credentials", AdditionalFields = "Auth1,CredentialTypeID, Flag", ErrorMessage = "Credential already in use. Please enter a different credentials.")]
            public string Auth1 { get; set; }

            //[Required(ErrorMessage = "Please enter pin.")]
            public string Auth2 { get; set; }

            //[Required(ErrorMessage = "Please enter confirm pin.")]
            [System.ComponentModel.DataAnnotations.Compare("Auth2", ErrorMessage = "Confirm pin must be equal to pin entered above.")]
            public string Confirm_Auth2 { get; set; }
        }
    }
}
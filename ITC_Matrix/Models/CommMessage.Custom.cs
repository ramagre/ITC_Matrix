using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static ITC_Matrix.Common.CommonEnum;

namespace ITC_Matrix.Models
{
    public partial class CommMessage
    {
        public CommMessage()
        {
            MessageID = 0;
            DefaultMessage = string.Empty;
            CustomMessage = string.Empty;
            MessageLineLenght = 20;
            UseCustom = 0;
            Language = 0;
            MsgGroup = string.Empty;
            MsgType = string.Empty;
            DSCR = string.Empty;
            CODE = 0;
        }

        [NotMapped]
        public bool UseCustom_bool
        {
            get { return UseCustom == 1 ? true : false; }
            set { UseCustom = value ? (short)1 : (short)0; }
        }

        public string LanguageName { get; set; }

        public MessageGroup MessageGroup { get; set; }
    }
}
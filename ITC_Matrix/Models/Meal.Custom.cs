using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace ITC_Matrix.Models
{
    [MetadataType(typeof(MealMetaData))]
    public partial class Meal
    {
        public short FromHH{ get; set; }
        public short FromMM { get; set; }
        public short ToHH { get; set; }
        public short ToMM{ get; set; }
    }
    public class MealMetaData {

    }

}
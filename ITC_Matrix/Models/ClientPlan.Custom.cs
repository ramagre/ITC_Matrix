using System.ComponentModel.DataAnnotations;

namespace ITC_Matrix.Models
{
    [MetadataType(typeof(tblClientPlanData))]
    public partial class ClientPlan
    {
        public string Description { get; set; }
        public string Max { get; set; }
        public double MaxAmount { get; set; }
        public decimal TotalAmountPerPlan { get; set; }
    }

    public class tblClientPlanData
    {
        public string ID_NO { get; set; }
        public int PlanCode { get; set; }
        public short MealCode { get; set; }
        public short Meals { get; set; }
        public int Amount { get; set; }
    }

}
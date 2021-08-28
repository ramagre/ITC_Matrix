using System.ComponentModel.DataAnnotations;

namespace ITC_Matrix.Models
{

    [MetadataType(typeof(MetaDatapermissionsTask))]
    public partial class permissionsTask
    {
        public bool isSelected { get; set; }
    }

    class MetaDatapermissionsTask
    {
        public bool isSelected { get; set; }
    }
}

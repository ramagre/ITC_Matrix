using System.ComponentModel.DataAnnotations;

namespace ITC_Matrix.Models
{

    [MetadataType(typeof(MetaDatapermissionsTasks))]
    public partial class permissionsTasks
    {
        public bool isSelected { get; set; }
    }

    class MetaDatapermissionsTasks
    {
        public bool isSelected { get; set; }
    }
}

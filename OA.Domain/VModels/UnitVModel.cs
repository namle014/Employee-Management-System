using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.VModels
{
    public class UnitCreateVModel
    {
        public string? Name { get; set; }
        public string? Note { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }

    public class UnitUpdateVModel : UnitCreateVModel
    {
        [Required]
        public long Id { get; set; }
    }

    public class UnitGetByIdVModel : UnitUpdateVModel
    {
        public string? UnitCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class UnitGetAllVModel : UnitGetByIdVModel
    {

    }

    [DataContract]
    public class UnitExportVModel
    {
        [DataMember(Name = @"UnitCode")]
        public string? UnitCode { get; set; }
        [DataMember(Name = @"Name")]
        public string? Name { get; set; }
        [DataMember(Name = @"Note")]
        public string? Note { get; set; }
        [DataMember(Name = @"CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [DataMember(Name = @"CreatedBy")]
        public string? CreatedBy { get; set; }
        [DataMember(Name = @"UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [DataMember(Name = @"UpdatedBy")]
        public string? UpdatedBy { get; set; }
        [DataMember(Name = @"IsActive")]
        public bool IsActive { get; set; }
    }
}

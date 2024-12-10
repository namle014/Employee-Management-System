using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OA.Infrastructure.EF.Entities
{
    public class TimeOff
    {
       
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Reason { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsAccepted { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}

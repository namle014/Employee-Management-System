namespace OA.Core.Models
{
    public class Pagination
    {
        public long TotalRecords { get; set; }
        public IEnumerable<dynamic> Records { get; set; } = Enumerable.Empty<dynamic>();
    }
}

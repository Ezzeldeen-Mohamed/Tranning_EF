namespace MeterManagement.Application.DTOs.MeterDtos
{
    public class MeterQueryParameters
    {
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Filters
        public string? SerialNumber { get; set; }
        public string? Status { get; set; } // "Assigned", "Installed", etc.
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
